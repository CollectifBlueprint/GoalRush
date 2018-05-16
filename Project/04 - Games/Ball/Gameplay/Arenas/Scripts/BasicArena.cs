using Ball.Gameplay.Arenas;
using Ball.Physics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using LBE.Physics;
using Microsoft.Xna.Framework;

namespace Content.Arenas
{
    public class BasicArena : ArenaScript
    {
        public override void OnInitGeometry()
        {
            var rbCmp = Arena.Owner.RigidBodyCmp;
            rbCmp.Body.CollidesWith = (Category)CollisionType.All | (Category)CollisionType.Ball;
            rbCmp.Body.CollisionCategories = (Category)CollisionType.Wall;
        }

        public override void OnUpdate()
        {
        }
    }
}
