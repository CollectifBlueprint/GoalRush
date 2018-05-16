using LBE;
using LBE.Gameplay;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ball.Gameplay.Players
{
    public class PlayerAntenna : GameObjectComponent
    {
        Player m_player;
        Vector2 m_currentPos;
        Vector2 m_currentSpeed;

        public override void Start()
        {
            m_player = Owner.FindComponent<Player>();
            m_currentPos = m_player.Position;
        }

        public override void Update()
        {
            var length = Engine.Debug.EditSingle("AntennaLength");
            var sprintConstant = Engine.Debug.EditSingle("AntennaSpringConstant");
            var springDamp = Engine.Debug.EditSingle("AntennaSpringDampening");

            var playerForward = Vector2.UnitX.Rotate(m_player.Owner.Orientation);
            var targetPos = Owner.Position - length * playerForward;

            var dv = targetPos - m_currentPos;
            if (dv != Vector2.Zero)
            {
                var deltaDist = dv.Length();
                var deltaDir = Vector2.Normalize(dv);

                var strength = deltaDist * sprintConstant;
                m_currentSpeed = m_currentSpeed * springDamp + strength * Engine.GameTime.ElapsedMS * deltaDir;
            }
            var newPos = m_currentPos + Engine.GameTime.ElapsedMS * m_currentSpeed;
            if (float.IsNaN(newPos.X) || float.IsNaN(m_currentSpeed.X) || float.IsInfinity(newPos.X))
                System.Diagnostics.Debugger.Break();

            m_currentPos = newPos;

            Engine.Debug.Screen.AddLine(Owner.Position, m_currentPos);
            Engine.Debug.Screen.AddCircle(m_currentPos, 10);
        }

        public override void End()
        {
        }
    }
}
