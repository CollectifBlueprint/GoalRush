using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LBE.Utils;
using LBE.Core;
using LBE.Assets;
using System.ComponentModel;

namespace LBE.Physics
{
    [Serializable]
    public class CollisionDefinitionEntry
    {
        public int[] Indices;
        public Vector2[] Vertices;
    }

    [Serializable]
    public class CollisionDefinition
    {
        public CollisionDefinitionEntry[] Entries;
    }

    public class CollisionDefinitionHelper
    {
        public static CollisionDefinition FromTexture(Texture2D texture, float tolerance)
        {
            return FromTexture(texture, tolerance, new Transform());
        }

        public static CollisionDefinition FromTexture(Texture2D texture, float tolerance, Transform transform)
        {
            List<CollisionDefinitionEntry> collisions = new List<CollisionDefinitionEntry>();

            //Get the pixellated outline
            TextureToPerimeter texP = new TextureToPerimeter();
            foreach (var perimeter in texP.March(texture))
            {
                //Remove redundant aligned points
                var simplifiedLine = new List<Vector2>();
                Vector2 lastDir = Vector2.Zero;
                for (int i = 1; i < perimeter.Count; i++)
                {
                    Vector2 dir = perimeter[i] - perimeter[i - 1];
                    if (lastDir != dir)
                        simplifiedLine.Add(perimeter[i - 1]);

                    lastDir = dir;
                }

                //Use dougles peucker to further simplify the outline, and fix local coordinates
                simplifiedLine = DouglasPeucker.DouglasPeuckerReduction(simplifiedLine, tolerance);
                for (int i = 0; i < simplifiedLine.Count; i++)
                {
                    simplifiedLine[i] -= new Vector2(texture.Width, texture.Height) * 0.5f;
                    simplifiedLine[i] = new Vector2(simplifiedLine[i].X, -simplifiedLine[i].Y);

                    simplifiedLine[i] = simplifiedLine[i].Rotate(transform.Orientation);
                    simplifiedLine[i] += transform.Position;
                }

                //Triangulate the resulting outline
                Triangulator tr = new Triangulator(simplifiedLine.ToArray());
                var indices = tr.Triangulate();

                collisions.Add( new CollisionDefinitionEntry() { Indices = indices, Vertices = simplifiedLine.ToArray() });
            }

            return new CollisionDefinition() { Entries = collisions.ToArray()};
        }
    }
}
