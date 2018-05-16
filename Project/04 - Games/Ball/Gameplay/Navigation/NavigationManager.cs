using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE;
using LBE.Gameplay;
using Microsoft.Xna.Framework;
using LBE.Assets;
using FarseerPhysics.Dynamics;
using LBE.Physics;
using Ball.Physics;
using System.Threading.Tasks;
using LBE.Input;
using Microsoft.Xna.Framework.Input;
using Ball.Gameplay.Navigation.PotentialMaps;
using Ball.Gameplay.Players.AI;
using FarseerPhysics.Collision;

namespace Ball.Gameplay.Navigation
{
    public class NavigationParameters
    {
        public bool Debug;

        public int MapUpdateMultiplier = 1;

        public int GridUpdateCount;
        public float GridSpacing;
    }

    public class NavigationManager : GameObjectComponent
    {
        Grid<NavigationCell> m_navigationGrid;
        public Grid<NavigationCell> NavigationGrid
        {
            get { return m_navigationGrid; }
        }

        Vector2 m_navigationGridOrigin;

        Asset<NavigationParameters> m_paramAsset;
        public NavigationParameters Parameters
        {
            get { return m_paramAsset.Content; }
        }

        Dictionary<String, PotentialMap> m_potentialMaps;
        public Dictionary<String, PotentialMap> PotentialMaps
        {
            get { return m_potentialMaps; }
        }

        int m_mapDebugIndex = 0;
        KeyControl m_debugIndexPlusCtrl;
        KeyControl m_debugIndexMinusCtrl;

        int m_updateIndex;

        PotentialMapInfluencePoint[] m_shootLeftPlayerCosts;
        PotentialMapInfluencePoint[] m_shootRightPlayerCosts;
        PotentialMapInfluencePoint m_ballSource;

        public void Init()
        {
            m_paramAsset = Engine.AssetManager.GetAsset<NavigationParameters>("Game/Navigation.lua::Navigation");

            m_potentialMaps = new Dictionary<string, PotentialMap>();

            m_mapDebugIndex = 0;
            m_debugIndexPlusCtrl = new KeyControl(Keys.PageDown);
            m_debugIndexMinusCtrl = new KeyControl(Keys.PageUp);

            Vector2 arenaSize = Game.Arena.Description.Size;
            if (arenaSize == Vector2.Zero)
                arenaSize = new Vector2(1280, 720);

            int nPointX = (int)(arenaSize.X / Parameters.GridSpacing);
            int nPointY = (int)(arenaSize.Y / Parameters.GridSpacing);

            m_navigationGridOrigin = new Vector2(
                -nPointX * Parameters.GridSpacing * 0.5f,
                -nPointY * Parameters.GridSpacing * 0.5f);

            //Create the grid
            m_navigationGrid = new Grid<NavigationCell>(nPointX, nPointY);

            //Initialise data points
            for (int i = 0; i < m_navigationGrid.Width; i++)
            {
                for (int j = 0; j < m_navigationGrid.Height; j++)
                {
                    Point index = new Point(i, j);
                    m_navigationGrid[index] = new NavigationCell();
                    m_navigationGrid[index].Index = index;
                    m_navigationGrid[index].Position = new Vector2(
                            m_navigationGridOrigin.X + i * Parameters.GridSpacing,
                            m_navigationGridOrigin.Y + j * Parameters.GridSpacing);

                    m_navigationGrid[index].CanSeePlayer = new bool[4];
                }
            }
            
            //Initialise neighbours in a second pass
            for (int i = 0; i < m_navigationGrid.Width; i++)
            {
                for (int j = 0; j < m_navigationGrid.Height; j++)
                {
                    Point index = new Point(i, j);
                    List<NavigationCell> neighbours = new List<NavigationCell>(8);
                    foreach (var p in index.Neighbours())
                        if (m_navigationGrid.TestBounds(p))
                            neighbours.Add(m_navigationGrid[p]);
                        else
                            neighbours.Add(null);

                    m_navigationGrid[index].Neighbours = neighbours.ToArray();
                    m_navigationGrid[index].CanNavigateToNeighbour = new bool[8];
                }
            }

            var ballMap = new PotentialMap(Game.GameManager.Navigation, PotentialMapType.Linear);
            m_ballSource = new PotentialMapInfluencePoint();
            m_ballSource.Radius = 60;
            m_ballSource.Value = 1;
            ballMap.Sources.Add(m_ballSource);
            ballMap.UpdateFrequency = PotentialMapUpdateFrequency.Fast;
            Game.GameManager.Navigation.PotentialMaps.Add("BallMap", ballMap);

            m_shootLeftPlayerCosts = new PotentialMapInfluencePoint[2];
            m_shootLeftPlayerCosts[0] = new PotentialMapInfluencePoint();
            m_shootLeftPlayerCosts[1] = new PotentialMapInfluencePoint();
            m_shootRightPlayerCosts = new PotentialMapInfluencePoint[2];
            m_shootRightPlayerCosts[0] = new PotentialMapInfluencePoint();
            m_shootRightPlayerCosts[1] = new PotentialMapInfluencePoint();
            foreach (var playerCost in m_shootLeftPlayerCosts)
            {
                playerCost.Value = Engine.Debug.EditSingle("PlayerValue", 3);
                playerCost.Radius = Engine.Debug.EditSingle("PlayerCostRadius", 350);
                playerCost.Attenuation = 0;
            }
            foreach (var playerCost in m_shootRightPlayerCosts)
            {
                playerCost.Value = Engine.Debug.EditSingle("PlayerValue", 3);
                playerCost.Radius = Engine.Debug.EditSingle("PlayerCostRadius", 350);
                playerCost.Attenuation = 0;
            }

            var shootLeftMap = new PotentialMap(Game.GameManager.Navigation, PotentialMapType.Linear);
            shootLeftMap.Sources.Add(new CanShootLeftPotentialSource());
            shootLeftMap.Costs.Add(m_shootLeftPlayerCosts[0]);
            shootLeftMap.Costs.Add(m_shootLeftPlayerCosts[1]);
            shootLeftMap.LinearParameters.SpatialDecay = 0.01f;
            shootLeftMap.LinearParameters.MinValue = -10;
            shootLeftMap.UpdateFrequency = PotentialMapUpdateFrequency.Normal;
            Game.GameManager.Navigation.PotentialMaps.Add("ShootLeft", shootLeftMap);

            var shootRightMap = new PotentialMap(Game.GameManager.Navigation, PotentialMapType.Linear);
            shootRightMap.Sources.Add(new CanShootRightPotentialSource());
            shootRightMap.Costs.Add(m_shootRightPlayerCosts[0]);
            shootRightMap.Costs.Add(m_shootRightPlayerCosts[1]);
            shootRightMap.LinearParameters.SpatialDecay = 0.01f;
            shootRightMap.LinearParameters.MinValue = -10;
            shootRightMap.UpdateFrequency = PotentialMapUpdateFrequency.Normal;
            Game.GameManager.Navigation.PotentialMaps.Add("ShootRight", shootRightMap);
        }

        public override void Update()
        {
            if (m_navigationGrid == null)
                return;

            if (Game.GameManager.Ball != null)
                m_ballSource.Position = Game.GameManager.Ball.Position;

            //Random rand = new Random();
            //int nAABB = 128;
            //AABB[] aabbs = new AABB[nAABB];
            //for (int i = 0; i < nAABB; i++)
            //{
            //    aabbs[i] = new AABB(rand.NextVector2(), rand.NextFloat(), rand.NextFloat());
            //}

            //int t = 0;
            //int n = 1000000;
            //for (int i = 0; i < n; i++)
            //{
            //    if (AABB.TestOverlapFast(ref aabbs[(i) % nAABB], ref aabbs[(i + 1) % nAABB])) t++;
            //    if (AABB.TestOverlap(ref aabbs[(i) % nAABB], ref aabbs[(i + 1) % nAABB])) t++;
            //}

            //Engine.Log.Debug("T", t);

            m_shootLeftPlayerCosts[0].Position = Game.Arena.LeftGoal.Team.Players[0].Position;
            m_shootLeftPlayerCosts[1].Position = Game.Arena.LeftGoal.Team.Players[1].Position;
            m_shootRightPlayerCosts[0].Position = Game.Arena.RightGoal.Team.Players[0].Position;
            m_shootRightPlayerCosts[1].Position = Game.Arena.RightGoal.Team.Players[1].Position;

            if (m_debugIndexPlusCtrl.KeyPressed())
                m_mapDebugIndex++;

            if (m_debugIndexMinusCtrl.KeyPressed())
                m_mapDebugIndex--;

            int nMap = m_potentialMaps.Keys.Count;
            m_mapDebugIndex = (m_mapDebugIndex + nMap) % nMap;

            int iMap = 0; //Use an incrementing index to spread the updates across frames
            Parallel.ForEach(m_potentialMaps.Values, map =>
            {
                if (map.UpdateFrequency == PotentialMapUpdateFrequency.Slow)
                {
                    if ((Engine.FrameCount + iMap++) % (3 * Parameters.MapUpdateMultiplier) == 0) map.Update();
                }
                else if (map.UpdateFrequency == PotentialMapUpdateFrequency.Normal)
                {
                    if ((Engine.FrameCount + iMap++) % (2 * Parameters.MapUpdateMultiplier) == 0) map.Update();
                }
                else if (map.UpdateFrequency == PotentialMapUpdateFrequency.Fast)
                {
                    if ((Engine.FrameCount + iMap++) % (1 * Parameters.MapUpdateMultiplier) == 0) map.Update();
                }
            });

            if (Parameters.Debug && Engine.Debug.Flags.RenderDebug && m_potentialMaps.Count != 0)
            {
                var keyValue = m_potentialMaps.ElementAt(m_mapDebugIndex);
                var map = keyValue.Value;
                Engine.Log.Debug("Potential Map", keyValue.Key); 
                map.Debug();
            }

            int arraySize = m_navigationGrid.Data.Length;
            int nUpdate = Parameters.GridUpdateCount;
            for (int i = m_updateIndex; i < m_updateIndex + nUpdate; i++)
            {
                int idx = i % arraySize;
                var cell = m_navigationGrid.Data[idx];

                //Update connectivity
                for (int iNeighbour = 0; iNeighbour < cell.Neighbours.Length; iNeighbour++)
                {
                    var nextCell = cell.Neighbours[iNeighbour];
                    if(nextCell == null)
                        continue;

                    cell.CanNavigateToNeighbour[iNeighbour] = RayCastVisibility(cell.Position, nextCell.Position);
                }

                //Raycast to left goal
                cell.CanShootLeft = RayCastVisibility(cell.Position, Game.Arena.LeftGoal.Position);
                cell.CanShootLeftValue = 0.0f;                
                if (cell.CanShootLeft)
                {
                    float maxShootDistancePenalty = Engine.Debug.EditSingle("MaxShootDistancePenalty", 0.8f);
                    float minShootDistance = Engine.Debug.EditSingle("MinShootDistance", 40.0f);
                    float maxShootDistance = Engine.Debug.EditSingle("MaxShootDistance", 600.0f);

                    bool intercept1 = TestInterception(Game.GameManager.Teams[0], cell.Position, Game.Arena.LeftGoal.Position);
                    bool intercept2 = TestInterception(Game.GameManager.Teams[1], cell.Position, Game.Arena.LeftGoal.Position);

                    if (intercept1 && intercept2)
                    {
                        float distToGoal = Vector2.Distance(Game.Arena.LeftGoal.Position, cell.Position);
                        float distCoef = LBE.MathHelper.LinearStep(minShootDistance, maxShootDistance, distToGoal);
                        distCoef = (float)Math.Pow(distCoef, 0.5f);
                        float shootValue = LBE.MathHelper.Lerp(1 - maxShootDistancePenalty, 1.0f, 1 - distCoef);

                        cell.CanShootLeftValue = shootValue;
                    }
                }

                //Raycast to right goal
                cell.CanShootRight = RayCastVisibility(cell.Position, Game.Arena.RightGoal.Position);
                cell.CanShootRightValue = 0.0f;
                if (cell.CanShootRight)
                {
                    float maxShootDistancePenalty = Engine.Debug.EditSingle("MaxShootDistancePenalty", 0.8f);
                    float minShootDistance = Engine.Debug.EditSingle("MinShootDistance", 40.0f);
                    float maxShootDistance = Engine.Debug.EditSingle("MaxShootDistance", 600.0f);

                    bool intercept1 = TestInterception(Game.GameManager.Teams[0], cell.Position, Game.Arena.RightGoal.Position);
                    bool intercept2 = TestInterception(Game.GameManager.Teams[1], cell.Position, Game.Arena.RightGoal.Position);

                    if (intercept1 && intercept2)
                    {
                        float distToGoal = Vector2.Distance(Game.Arena.RightGoal.Position, cell.Position);
                        float distCoef = LBE.MathHelper.LinearStep(minShootDistance, maxShootDistance, distToGoal);
                        distCoef = (float)Math.Pow(distCoef, 0.5f);
                        float shootValue = LBE.MathHelper.Lerp(1 - maxShootDistancePenalty, 1.0f, 1 - distCoef);

                        cell.CanShootRightValue = shootValue;
                    }
                }

                for (int iPlayer = 0; iPlayer < 4; iPlayer++)
                {
                    cell.CanSeePlayer[iPlayer] = RayCastVisibility(cell.Position, Game.GameManager.Players[iPlayer].Position);
                }
            }
            m_updateIndex = (m_updateIndex + nUpdate) % arraySize;

            DebugGrid();
        }

        public static bool RayCastVisibility(Vector2 from, Vector2 to)
        {
            bool lineClear = true;
            Engine.PhysicsManager.World.RayCast(
                (Fixture fixture, Vector2 point, Vector2 normal, float fraction) =>
                {
                    if (fixture.CollidesWith.HasFlag((Category)CollisionType.Ball)
                        && !fixture.CollisionCategories.HasFlag((Category)CollisionType.Player)
                        && !fixture.CollisionCategories.HasFlag((Category)CollisionType.Ball)
                        && !fixture.IsSensor)
                    {
                        lineClear = false;
                        return 0;
                    }
                    return 1;
                },
                ConvertUnits.ToSimUnits(from),
                ConvertUnits.ToSimUnits(to));
            return lineClear;
        }

        public static bool DoubleRayCast(Vector2 from, Vector2 to, float offset = 10)
        {
            Vector2 dir = to - from; dir.Normalize();
            Vector2 orthoDir = dir.Orthogonal();

            bool ray1 = RayCastVisibility(from + orthoDir * offset * 0.5f, to + orthoDir * offset * 0.5f);
            bool ray2 = RayCastVisibility(from - orthoDir * offset * 0.5f, to - orthoDir * offset * 0.5f);

            return ray1 && ray2;
        }

        public static bool TestInterception(Team team, Vector2 from, Vector2 to, bool debug = false)
        {
            Vector2 dir = to - from;
            float distance = dir.Length();
            dir = dir / distance;

            float distanceCoef = 1.25f;
            float interceptConeAngle = Engine.Debug.EditSingle("InterceptConeAngle", 0.8f);
            Vector2 dirBot = dir.Rotate(-interceptConeAngle);
            Vector2 dirTop = dir.Rotate(interceptConeAngle);
            float endDistance = Vector2.Distance(dirTop, dirBot) * distance * distanceCoef;

            foreach (var player in Game.GameManager.Players)
            {
                if (player.Team != team)
                    continue;

                if (player.Properties.Stunned)
                    continue;

                Vector2 playerDir = player.Position - from;
                float angle1 = LBE.MathHelper.Angle(dirBot, playerDir);
                float angle2 = LBE.MathHelper.Angle(dirTop, playerDir);
                float playerDistance = Vector2.Distance(player.Position, from);

                if (angle1 > 0 && angle1 < interceptConeAngle * 2 && angle2 > 0 && angle2 < interceptConeAngle * 2 && playerDistance < endDistance)
                    return false;
            }
            return true;
        }

        public Point IndexFromPosition(Vector2 pos)
        {
            Vector2 offset = pos - m_navigationGridOrigin;
            Point index = new Point(
                (int)(offset.X / Parameters.GridSpacing + 0.5f),
                (int)(offset.Y / Parameters.GridSpacing + 0.5f));

            return index;
        }

        public void DebugGrid()
        {
        }
    }
}
