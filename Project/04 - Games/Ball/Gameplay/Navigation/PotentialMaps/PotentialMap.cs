using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using LBE;
using Ball.Gameplay.Navigation.PotentialMaps;
using System.Threading.Tasks;

namespace Ball.Gameplay.Navigation
{
    public enum PotentialMapType
    {
        Linear,
        Exponential,
    }

    public enum PotentialMapUpdateFrequency
    {
        None,
        Slow,
        Normal,
        Fast,
    }

    public class PotentielMapCell
    {
        public NavigationCell NavCell;

        public float SourceValue;
        public float CostValue;

        public float Value;
        public Vector2 Gradient;
        public float Influence;
    }

    public class PotentialMapExponentialParameters
    {
        public float TimeDecay;
        public float SpatialDecay;
        public float Inertia;
    }

    public class PotentialMapLinearParameters
    {
        public float SpatialDecay = 0.01f;
        public float MinValue = 0;
    }

    public class PotentialMap
    {
        PotentialMapUpdateFrequency m_updateFrequency = PotentialMapUpdateFrequency.Normal;
        public PotentialMapUpdateFrequency UpdateFrequency
        {
            get { return m_updateFrequency; }
            set { m_updateFrequency = value; }
        }

        Grid<PotentielMapCell> m_grid;
        public Grid<PotentielMapCell> Grid
        {
            get { return m_grid; }
            set { m_grid = value; }
        }

        float m_decay = 0.1f;
        public float Decay
        {
            get { return m_decay; }
            set { m_decay = value; }
        }

        float m_inertia = 0.5f;
        public float Inertia
        {
            get { return m_inertia; }
            set { m_inertia = value; }
        }

        float m_influenceDecay = 0.5f;

        float m_cutoff = 0.0001f;
        public float Cutoff
        {
            get { return m_cutoff; }
            set { m_cutoff = value; }
        }

        PotentialMapLinearParameters m_linearParameters;
        public PotentialMapLinearParameters LinearParameters
        {
            get { return m_linearParameters; }
            set { m_linearParameters = value; }
        }

        PotentialMapType m_type;
        public PotentialMapType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        List<PotentialMapInfluence> m_sources;
        public List<PotentialMapInfluence> Sources
        {
            get { return m_sources; }
        }

        List<PotentialMapInfluence> m_costs;
        public List<PotentialMapInfluence> Costs
        {
            get { return m_costs; }
        }

        NavigationManager m_navManager;

        public PotentialMap(NavigationManager navManager, PotentialMapType type = PotentialMapType.Exponential)
        {
            m_navManager = navManager;

            m_type = type;

            m_grid = new Grid<PotentielMapCell>(navManager.NavigationGrid.Width, navManager.NavigationGrid.Height);
            m_sources = new List<PotentialMapInfluence>();
            m_costs = new List<PotentialMapInfluence>();

            m_linearParameters = new PotentialMapLinearParameters();
            m_linearParameters.MinValue = 0;
            m_linearParameters.SpatialDecay = Engine.Debug.EditSingle("LinearDecay", 0.1f);

            //Initialise data points
            for (int i = 0; i < m_grid.Width; i++)
            {
                for (int j = 0; j < m_grid.Height; j++)
                {
                    Point index = new Point(i, j);
                    m_grid[index] = new PotentielMapCell();
                    m_grid[index].NavCell = navManager.NavigationGrid[index];
                }
            }
        }

        public void Update()
        {
            m_decay = Engine.Debug.EditSingle("MapDecay", 0.1f);
            m_inertia = Engine.Debug.EditSingle("MapIntertia", 0.05f);
            m_influenceDecay = Engine.Debug.EditSingle("MapInfluenceDecay", 0.05f);

            float linearDecay = m_linearParameters.SpatialDecay;

            //Compute sources
            //Parallel.For(0, m_grid.Data.Length, i => 
            for (int i = 0; i< m_grid.Data.Length; i++)
            {
                var cell = m_grid.Data[i];
                var navCell = cell.NavCell;

                cell.SourceValue = 0;
                foreach (var source in m_sources)
                {
                    float sourceValue = source.GetValue(navCell);
                    if (Math.Abs(sourceValue) > Math.Abs(cell.SourceValue))
                        cell.SourceValue = sourceValue;
                    cell.SourceValue = LBE.MathHelper.Clamp(-1, 1, cell.SourceValue);
                }

                cell.CostValue = 0;
                foreach (var cost in m_costs)
                {
                    cell.CostValue = Math.Max(cell.CostValue, cost.GetValue(navCell));
                    cell.CostValue = LBE.MathHelper.Clamp(0, float.PositiveInfinity, cell.CostValue);
                }
            }//);

            //Decay
            //Parallel.For(0, m_grid.Data.Length, i => 
            for (int i = 0; i< m_grid.Data.Length; i++)
            {
                var cell = m_grid.Data[i];
                //cell.Value = (1 - m_decay) * cell.Value + m_decay * cell.SourceValue;
            }//);

            //Diffuse influence
            //Parallel.For(0, m_grid.Data.Length, i => 
            for (int i = 0; i< m_grid.Data.Length; i++)
            {
                var cell = m_grid.Data[i];
                var navCell = cell.NavCell;

                if (m_type == PotentialMapType.Linear)
                {
                    LinearInfluence(linearDecay, cell, navCell);
                }
                else if(m_type == PotentialMapType.Exponential)
                {
                    ExponentialInfluence(cell, navCell);
                }
            }//);

            //Update to influence            
            Parallel.For(0, m_grid.Data.Length, i => 
            //for (int i = 0; i< m_grid.Data.Length; i++)
            {
                var cell = m_grid.Data[i];
                cell.Value = m_inertia * cell.Value + (1 - m_inertia) * cell.Influence;

                int navCost = 0;
                foreach (var neigh in cell.NavCell.CanNavigateToNeighbour)
                {
                    if (!neigh)
                        navCost++;
                }

                if (m_type == PotentialMapType.Linear)
                    cell.Value = LBE.MathHelper.Clamp(0, 1, cell.Value - navCost * linearDecay);
                else if (m_type == PotentialMapType.Exponential)
                    cell.Value = cell.Value / (1 + navCost);

                if (Math.Abs(cell.Value) < m_cutoff)
                    cell.Value = 0;
                
            });

            //Update to source
            //Parallel.For(0, m_grid.Data.Length, i => 
            for (int i = 0; i< m_grid.Data.Length; i++)
            {
                var cell = m_grid.Data[i];

                if (cell.SourceValue > 0 && cell.SourceValue > cell.Value)
                    cell.Value = cell.SourceValue;
                else if (cell.SourceValue < 0 && cell.SourceValue < cell.Value)
                    cell.Value = cell.SourceValue;
            }//);
        }

        private void LinearInfluence(float linearDecay, PotentielMapCell cell, NavigationCell navCell)
        {
            float influence = float.NegativeInfinity;
            for (int iNeighbour = 0; iNeighbour < navCell.Neighbours.Length; iNeighbour++)
            {
                var nextNavCell = navCell.Neighbours[iNeighbour];
                if (navCell.Neighbours[iNeighbour] == null || !navCell.CanNavigateToNeighbour[iNeighbour])
                    continue;

                var nextCell = m_grid[nextNavCell.Index];

                //Linear approx. of distance, that is exact for distSq = 1 or distSq = 2
                float distSq = Vector2.DistanceSquared(nextNavCell.Position, navCell.Position) / (m_navManager.Parameters.GridSpacing * m_navManager.Parameters.GridSpacing);
                float sq2minusOne = 1.414213f - 1;
                float fastDist = (1 - sq2minusOne) + sq2minusOne * distSq;

                float cellInfluence = nextCell.Value - linearDecay * (1 + cell.CostValue) * fastDist * fastDist;
                influence = Math.Max(influence, cellInfluence);
            }
            cell.Influence = LBE.MathHelper.Clamp(m_linearParameters.MinValue, 1, influence);
        }

        private void ExponentialInfluence(PotentielMapCell cell, NavigationCell navCell)
        {
            float influence = 0;
            for (int iNeighbour = 0; iNeighbour < navCell.Neighbours.Length; iNeighbour++)
            {
                var nextNavCell = navCell.Neighbours[iNeighbour];
                if (navCell.Neighbours[iNeighbour] == null || !navCell.CanNavigateToNeighbour[iNeighbour])
                    continue;
                var nextCell = m_grid[nextNavCell.Index];

                float dist = Vector2.Distance(nextNavCell.Position, navCell.Position) / m_navManager.Parameters.GridSpacing;
                float costFactor = 1 / (1 + cell.CostValue); costFactor = LBE.MathHelper.Clamp(0, 1, costFactor);
                float cellInfluence = nextCell.Value * (float)Math.Exp(-dist * m_influenceDecay * (1 + cell.CostValue));
                if (cellInfluence > Math.Abs(influence))
                    influence = cellInfluence;
            }
            cell.Influence = LBE.MathHelper.Clamp(-1, 1, influence);
        }

        Color ColorLerp(Color c1, Color c2, float x)
        {
            return new Color(c1.ToVector3() * (1 - x) + c2.ToVector3() * x);
        }

        public Vector2 Gradient(Point index)
        {
            var cell = m_grid[index];
            var navCell = cell.NavCell;
            Vector2 grad = Vector2.Zero;
            for (int iNeighbour = 0; iNeighbour < navCell.Neighbours.Length; iNeighbour++)
            {
                var nextNavCell = navCell.Neighbours[iNeighbour];

                if (nextNavCell == null || !navCell.CanNavigateToNeighbour[iNeighbour])
                    continue;

                var nextCell = m_grid[nextNavCell.Index];

                //Compute gradient
                Vector2 dir = nextNavCell.Position - navCell.Position; dir.Normalize();
                float delta = nextCell.Value - cell.Value;
                delta = LBE.MathHelper.Clamp(0, 1, delta);
                grad += delta * dir;
            }

            if(grad != Vector2.Zero)
                grad.Normalize();

            return grad;
        }

        public void Debug()
        {
            float circleRadius = 8;

            float minValue = float.PositiveInfinity;
            float maxValue = float.NegativeInfinity;
            for (int i = 0; i < m_grid.Width; i++)
            {
                for (int j = 0; j < m_grid.Height; j++)
                {
                    var cell = m_grid[i, j];
                    if (cell.Value == m_linearParameters.MinValue)
                        continue;

                    if(cell.Value < minValue)
                        minValue = Math.Min(minValue, cell.Value);

                    maxValue = Math.Max(maxValue, cell.Value);
                }
            }


            Engine.Debug.Screen.ResetBrush();
            Engine.Debug.Screen.Brush.LineColor = Color.White;
            for (int i = 0; i < m_grid.Width; i++)
            {
                for (int j = 0; j < m_grid.Height; j++)
                {
                    var cell = m_grid[i, j];
                    var navCell = cell.NavCell;

                    //Cell circle
                    float colorLerp = 0;

                    if (m_type == PotentialMapType.Linear)
                    {
                        colorLerp = LBE.MathHelper.LinearStep(minValue, maxValue, cell.Value);
                        colorLerp = 2 * colorLerp - 1;
                    }
                    else if (m_type == PotentialMapType.Exponential)
                    {
                        colorLerp = LBE.MathHelper.Clamp(-1, 1, cell.Value);
                    }

                    Color color = Color.Black;
                    if (colorLerp > 0.5f)
                        color = ColorLerp(Color.Red, Color.Orange, 2 * colorLerp - 1);
                    else if (colorLerp > 0.0f)
                        color = ColorLerp(Color.Black, Color.Red, 2 * colorLerp);
                    else if (colorLerp > -0.5f)
                        color = ColorLerp(Color.DarkBlue, Color.Black, 1 + 2 * colorLerp);
                    else
                        color = ColorLerp(Color.LightBlue, Color.DarkBlue, 2 + 2 * colorLerp);

                    if (cell.Value == 0)
                        continue;

                    Engine.Debug.Screen.Brush.DrawSurface = true;
                    Engine.Debug.Screen.Brush.SurfaceColor = color;
                    Engine.Debug.Screen.Brush.SurfaceAlpha = 0.6f;
                    Engine.Debug.Screen.Brush.LineColor = Color.White;
                    Engine.Debug.Screen.Brush.DrawWireframe = false;
                    Engine.Debug.Screen.AddSquare(navCell.Position, m_navManager.Parameters.GridSpacing * 0.5f);
                    //Engine.Debug.Screen.AddString(cell.Value.ToString("0.00"), navCell.Position + 0 * (10 + circleRadius) * Vector2.UnitY);

                    Vector2 grad = Gradient(new Point(i,j));
                    Engine.Debug.Screen.Brush.DrawWireframe = true;
                    Engine.Debug.Screen.Brush.LineColor = Color.White;
                    Engine.Debug.Screen.AddArrow(navCell.Position, navCell.Position + grad * 15);
                }
            }
        }
    }
}
