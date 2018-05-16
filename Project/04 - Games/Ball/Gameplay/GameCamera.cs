using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LBE;
using LBE.Gameplay;
using LBE.Graphics;
using Microsoft.Xna.Framework;

namespace Ball.Gameplay
{
    public class GameCamera : GameObjectComponent
    {
        GameObject m_targetGO;

        float m_speedFocus;

        SmoothValue m_currentZoom;

        SmoothValue m_camPositionX;
        SmoothValue m_camPositionY;

        Vector2 m_boundaries;

        float m_targetZoom;

        Box2D m_viewZoomBoundingBox;

        List<Box2D> m_gameplayObjZoomBoundingBoxes;

        bool m_disableZoomUpdate = false;
        public bool DisableZoomUpdate
        {
            set { m_disableZoomUpdate = value; }
            get { return m_disableZoomUpdate; }
        }


        public override void Start()
        {
            m_speedFocus = 0.003f;
            m_targetZoom = 1.0f;
            m_currentZoom = new SmoothValue(m_targetZoom, 0.96f);

            Engine.Renderer.Cameras[(int)CameraId.Game].Position = Vector2.Zero;

            m_gameplayObjZoomBoundingBoxes = new List<Box2D>();

        }

        public void SetCamZoneBoundaries(float x, float y)
        {
            m_boundaries = new Vector2(x, y);// *Engine.Renderer.CameraZoom * Engine.Renderer.Cameras[(int)CameraId.Game].Zoom;
        }


        public Vector2 GetPosition()
        {
            return Engine.Renderer.Cameras[(int)CameraId.Game].Position;
        }

        public override void Update()
        {
            if (!m_disableZoomUpdate)
                UpdateZoom();
            
            UpdatePosition();
            
            bool debug = false;
            if (debug)
            {
                //Debug
                Engine.Debug.Screen.Brush.LineColor = Color.Crimson;
                for (int i = 0; i < m_gameplayObjZoomBoundingBoxes.Count; i++)
                {
                    Engine.Debug.Screen.AddRectangle(m_gameplayObjZoomBoundingBoxes[i].Center(), m_gameplayObjZoomBoundingBoxes[i].HalfSize);
                }

                Engine.Debug.Screen.Brush.LineColor = Color.Blue;
                Engine.Debug.Screen.AddRectangle(Vector2.Zero, m_boundaries);

                Engine.Debug.Screen.Brush.LineColor = Color.Green;
                Engine.Debug.Screen.AddRectangle(Owner.Position, m_viewZoomBoundingBox.HalfSize);
                Engine.Debug.Screen.AddCross(Owner.Position, 10);
            }
        }



        public void Focus(GameObject go)
        {
            m_targetGO = go;
            
            if (m_camPositionX == null && m_camPositionY == null)
            {
                m_camPositionX = new SmoothValue(go.Position.X, 0.92f);
                m_camPositionY = new SmoothValue(go.Position.Y, 0.92f);
            }
        }

        public void FocusAuto()
        {
            Focus(this.Owner);
        }

        public void ResetFocus()
        {
            m_targetGO = null;
            Engine.Renderer.Cameras[(int)CameraId.Game].Position = Vector2.Zero;
        }

        public void SetZoom(float zoomValue)
        {
            m_targetZoom = zoomValue;
        }

        public void UpdateZoom()
        {
            m_currentZoom.Update(m_targetZoom);
            Engine.Renderer.Cameras[(int)CameraId.Game].Zoom = m_currentZoom.Value;

            m_gameplayObjZoomBoundingBoxes.Clear();

            float m_rendererZoom = Engine.Renderer.CameraZoom;

            // Create Zoom Boxes
            if (Game.GameManager.Ball != null)
            {
                float ballBoxWidth = 800 / m_rendererZoom;
                float ballBoxHeight = 600 / m_rendererZoom;
                m_gameplayObjZoomBoundingBoxes.Add(new Box2D(Game.GameManager.Ball.Position, ballBoxWidth, ballBoxHeight));
            }

            for (int i = 0; i < Game.GameManager.Players.Length; i++)
            {
                float playerBoxWidth;
                float playerBoxHeight;
                if (Game.GameManager.Players[i].Ball == null)
                {
                    playerBoxWidth = 250 / m_rendererZoom;
                    playerBoxHeight = 250 / m_rendererZoom;
                }
                else
                {
                    playerBoxWidth = 250 / m_rendererZoom;
                    playerBoxHeight = 250 / m_rendererZoom;
                }

                m_gameplayObjZoomBoundingBoxes.Add(new Box2D(Game.GameManager.Players[i].Position, playerBoxWidth, playerBoxHeight));
            }

            Vector2 minCamBoundingBox = m_gameplayObjZoomBoundingBoxes[0].Min;
            Vector2 maxCamBoundingBox = m_gameplayObjZoomBoundingBoxes[0].Max;

            for (int i = 0; i < m_gameplayObjZoomBoundingBoxes.Count; i++)
            {
                minCamBoundingBox.X = Math.Min(minCamBoundingBox.X, m_gameplayObjZoomBoundingBoxes[i].Min.X);
                minCamBoundingBox.Y = Math.Min(minCamBoundingBox.Y, m_gameplayObjZoomBoundingBoxes[i].Min.Y);
                maxCamBoundingBox.X = Math.Max(maxCamBoundingBox.X, m_gameplayObjZoomBoundingBoxes[i].Max.X);
                maxCamBoundingBox.Y = Math.Max(maxCamBoundingBox.Y, m_gameplayObjZoomBoundingBoxes[i].Max.Y);
            }

            m_viewZoomBoundingBox = new Box2D(minCamBoundingBox, maxCamBoundingBox);

            Box2D cameraBoundingBox = new Box2D(Owner.Position, Engine.Renderer.Cameras[(int)CameraId.Game].Width,
                Engine.Renderer.Cameras[(int)CameraId.Game].Height);

            // Fix new view aspect ratio
            float cameraAspectRatio = cameraBoundingBox.HalfSize.X / cameraBoundingBox.HalfSize.Y;
            float viewZoomBoxAspectRatio = m_viewZoomBoundingBox.HalfSize.X / m_viewZoomBoundingBox.HalfSize.Y;

            if (viewZoomBoxAspectRatio > cameraAspectRatio)
            {
                //new height
                float width = m_viewZoomBoundingBox.Max.X - m_viewZoomBoundingBox.Min.X;
                float height = width / cameraAspectRatio;
                Vector2 center = m_viewZoomBoundingBox.Center();
                m_viewZoomBoundingBox = new Box2D(center, width, height);
            }
            else
            {
                //new width
                float height = m_viewZoomBoundingBox.Max.Y - m_viewZoomBoundingBox.Min.Y;
                float width = height * cameraAspectRatio;
                Vector2 center = m_viewZoomBoundingBox.Center();
                m_viewZoomBoundingBox = new Box2D(center, width, height);
            }

            // Min zoom clamp
            float zoomFactor = cameraBoundingBox.HalfSize.X / m_viewZoomBoundingBox.HalfSize.X;


            Engine.Log.Debug("zoomFactorTmp", zoomFactor);


            float zoomFactorMin = 1.3f;

            if (zoomFactor < zoomFactorMin)
            {
                float zoomAdjust = zoomFactorMin / zoomFactor;
                float zoomHalfWidth = m_viewZoomBoundingBox.HalfSize.X;
                float zoomHalfHeight = m_viewZoomBoundingBox.HalfSize.Y;
                Vector2 zoomCenter = m_viewZoomBoundingBox.Center();

                m_viewZoomBoundingBox.Min = new Vector2(zoomCenter.X - zoomHalfWidth / zoomAdjust, zoomCenter.Y - zoomHalfHeight / zoomAdjust);
                m_viewZoomBoundingBox.Max = new Vector2(zoomCenter.X + zoomHalfWidth / zoomAdjust, zoomCenter.Y + zoomHalfHeight / zoomAdjust);

                zoomFactor = zoomFactorMin;

                // Adjust the view to always keep the ball visible
                if (Game.GameManager.Ball != null)
                {
                    Vector2 newViewZoomPos = m_viewZoomBoundingBox.Center();

                    Box2D ballBB = m_gameplayObjZoomBoundingBoxes[0];
                    if ((ballBB.Center().X - ballBB.HalfSize.X) < (m_viewZoomBoundingBox.Center().X - m_viewZoomBoundingBox.HalfSize.X))
                    {
                        float distAdjust = m_viewZoomBoundingBox.Min.X - ballBB.Min.X;
                        m_viewZoomBoundingBox.Max = new Vector2(m_viewZoomBoundingBox.Max.X - distAdjust, m_viewZoomBoundingBox.Max.Y);
                        m_viewZoomBoundingBox.Min = new Vector2(m_viewZoomBoundingBox.Min.X - distAdjust, m_viewZoomBoundingBox.Min.Y);
                    }
                    else if ((ballBB.Center().X + ballBB.HalfSize.X) > (m_viewZoomBoundingBox.Center().X + m_viewZoomBoundingBox.HalfSize.X))
                    {
                        float distAdjust = ballBB.Max.X - m_viewZoomBoundingBox.Max.X;
                        m_viewZoomBoundingBox.Min = new Vector2(m_viewZoomBoundingBox.Min.X + distAdjust, m_viewZoomBoundingBox.Min.Y);
                        m_viewZoomBoundingBox.Max = new Vector2(m_viewZoomBoundingBox.Max.X + distAdjust, m_viewZoomBoundingBox.Max.Y);
                    }

                    if ((ballBB.Center().Y - ballBB.HalfSize.Y) < (m_viewZoomBoundingBox.Center().Y - m_viewZoomBoundingBox.HalfSize.Y))
                    {
                        float distAdjust = m_viewZoomBoundingBox.Min.Y - ballBB.Min.Y;
                        m_viewZoomBoundingBox.Max = new Vector2(m_viewZoomBoundingBox.Max.X, m_viewZoomBoundingBox.Max.Y - distAdjust);
                        m_viewZoomBoundingBox.Min = new Vector2(m_viewZoomBoundingBox.Min.X, m_viewZoomBoundingBox.Min.Y - distAdjust);
                    }
                    else if ((ballBB.Center().Y + ballBB.HalfSize.Y) > (m_viewZoomBoundingBox.Center().Y + m_viewZoomBoundingBox.HalfSize.Y))
                    {
                        float distAdjust = ballBB.Max.Y - m_viewZoomBoundingBox.Max.Y;
                        m_viewZoomBoundingBox.Min = new Vector2(m_viewZoomBoundingBox.Min.X, m_viewZoomBoundingBox.Min.Y + distAdjust);
                        m_viewZoomBoundingBox.Max = new Vector2(m_viewZoomBoundingBox.Max.X, m_viewZoomBoundingBox.Max.Y + distAdjust);
                    }
                }
            }

            // Apply zoom
            Owner.Position = m_viewZoomBoundingBox.Center();
            SetZoom(zoomFactor);

            bool debug = false;
            if(debug){
                // Debug
                Engine.Debug.Screen.Brush.LineColor = Color.Pink;
                Engine.Debug.Screen.AddRectangle(m_viewZoomBoundingBox.Center(), m_viewZoomBoundingBox.HalfSize);

                Engine.Log.Debug("zoomFactor", zoomFactor);

                Engine.Debug.Screen.Brush.LineColor = Color.PaleVioletRed;
                Engine.Debug.Screen.AddCross(Owner.Position, 10);
            }

        }

        public void UpdatePosition()
        {
            // Restrain the camera in the arena boundaries
            if (m_targetGO != null)
            {
                Vector2 target = m_targetGO.Position;

                Box2D cameraBoundingBox = new Box2D(Owner.Position, Engine.Renderer.Cameras[(int)CameraId.Game].Width,
                Engine.Renderer.Cameras[(int)CameraId.Game].Height);

                float viewHalfWidth = cameraBoundingBox.HalfSize.X;
                float viewHalfHeight = cameraBoundingBox.HalfSize.Y;

                // override view size with zoom bounding box if not nnull
                if (m_viewZoomBoundingBox.HalfSize != Vector2.Zero)
                {
                     viewHalfWidth = m_viewZoomBoundingBox.HalfSize.X;
                    viewHalfHeight = m_viewZoomBoundingBox.HalfSize.Y;
                }

                // Is view size smaller than arena size?
                if (m_boundaries.X > viewHalfWidth)
                {
                    if ((target.X + viewHalfWidth) > m_boundaries.X)
                        target.X = m_boundaries.X - viewHalfWidth;
                    else if ((target.X - viewHalfWidth) < -m_boundaries.X)
                        target.X = -m_boundaries.X + viewHalfWidth;
                }
                else
                {
                    target.X = 0;
                }

                if (m_boundaries.Y > viewHalfHeight)
                {
                    if ((target.Y + viewHalfHeight) > m_boundaries.Y)
                        target.Y = m_boundaries.Y - viewHalfHeight;
                    else if ((target.Y - viewHalfHeight) < -m_boundaries.Y)
                        target.Y = -m_boundaries.Y + viewHalfHeight;
                }
                else
                {
                    target.Y = 0;
                }

                // Cam pos update : old way 
                //if (m_disableZoomUpdate)
                //{
                //    Vector2 camPos = Engine.Renderer.Cameras[(int)CameraId.Game].Position;
                //    Vector2 newCamPos = camPos;
                //    float delta = Engine.GameTime.ElapsedMS;
                //    newCamPos.X += (target.X - camPos.X) * m_speedFocus * delta;
                //    newCamPos.Y += (target.Y - camPos.Y) * m_speedFocus * delta;
                //    Engine.Renderer.Cameras[(int)CameraId.Game].Position = newCamPos;
                //}
                //else
                // Cam pos update
                //{
                    m_camPositionX.Update(target.X);
                    m_camPositionY.Update(target.Y);
                    Vector2 shake = ScreenShake.Shake;
                    Engine.Renderer.Cameras[(int)CameraId.Game].Position =shake +  new Vector2(m_camPositionX.Value, m_camPositionY.Value);
                //}
            }

            // ?
            //Owner.Position = Engine.Renderer.Cameras[(int)CameraId.Game].Position;
        }
    }
}
        