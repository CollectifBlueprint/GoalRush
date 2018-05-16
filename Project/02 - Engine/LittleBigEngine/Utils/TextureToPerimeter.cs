using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;

namespace LBE.Utils
{
    // Licensed under the creative commons Atrribution 3.0 license
    // You are free to share, copy, distribute, and transmit this work
    // You are free to alter this work
    // If you use this work, please attribute it somewhere in the supporting
    // documentation of your work. (A mention in a readme or credits, for example.)
    // Please leave a comment or email me if you use this work, so I am compelled
    // to produce more of it's kind.
    // 2010 - Phillip Spiess
    public class TextureToPerimeter
    {
        // A simple enumeration to represent the direction we
        // just moved, and the direction we will next move.
        private enum StepDirection
        {
            None,
            Up,
            Left,
            Down,
            Right
        }

        // Some variables to make our function
        // calls a little smaller, probably shouldn't
        // expose these to the public though.

        // Holds the color information for our texture
        private Color[] colorData;

        // Our texture
        private Texture2D texture;

        // The direction we previously stepped
        private StepDirection previousStep;

        // Our next step direction:
        private StepDirection nextStep;

        public IEnumerable<List<Vector2>> March(Texture2D target)
        {
            texture = target;

            // Create an array large enough to hold our texture data
            colorData = new Color[texture.Height * texture.Width];

            // Get our color information out of the texture and 
            // into our traversable array
            texture.GetData<Color>(colorData);

            return DoMarch();
        }

        // Takes a texture and returns a list of pixels that
        // define the perimeter of the upper-left most
        // object in that texture, using alpha==0 to determine
        // the boundary.
        public IEnumerable<List<Vector2>> DoMarch()
        {
            while(true) {                

                 // Find the start points
                Vector2 perimeterStart = FindStartPoint();

                // Find the perimeter
                var perimeter = WalkPerimeter((int)perimeterStart.X, (int)perimeterStart.Y);

                if (perimeter.Count < 3)
                    break;
                
                //Fill the current perimeter to transparent so that we don't find it again
                ErasePerimeterRec((int)perimeterStart.X, (int)perimeterStart.Y);
                yield return perimeter;
            }
        }

        static int stackSize = 1;
        // Recursively flood the texture with transparency
        void ErasePerimeterRec(int xStart, int yStart)
        {
            Stack<Point> floodStack = new Stack<Point>();
            floodStack.Push(new Point(xStart, yStart));

            while (floodStack.Count > 0)
            {
                var point = floodStack.Pop();
                int x = point.X; int y = point.Y;

                // Do some sanity checking, so we aren't
                // walking outside the image
                if (x < 0 || x >= texture.Width)
                    continue;
                if (y < 0 || y >= texture.Height)
                    continue;

                if (!IsPixelSolid(x, y))
                    continue;

                colorData[x + y * texture.Width].A = 0;
                floodStack.Push(new Point(x + 1, y));
                floodStack.Push(new Point(x - 1, y));
                floodStack.Push(new Point(x, y - 1));
                floodStack.Push(new Point(x, y + 1));

                stackSize = Math.Max(stackSize, floodStack.Count);
                Engine.Log.Debug("TexturePerimter.StackSize", stackSize);
            }
        }

        // Finds the first pixel in the perimeter of the image
        private Vector2 FindStartPoint()
        {
            // Scan along the whole image
            for (int pixel = 0; pixel < colorData.Length; pixel++)
            {
                int x = pixel % texture.Width; 
                int y = pixel / texture.Width;
                bool onEdge = x == 0 || x == texture.Width - 1 || y == 0 || y == texture.Height - 1;
                bool solid = IsPixelSolid(x, y);
                bool perimeter = !IsPixelSolid(x - 1, y) || !IsPixelSolid(x - 1, y) || !IsPixelSolid(x - 1, y - 1); //The walk routine need to start in this particular quadrant

                // If the pixel is not entirely transparent
                // we've found a start point
                if (!onEdge && solid && perimeter)
                    return new Vector2(pixel % texture.Width,
                            pixel / texture.Width);
            }

            // If we get here
            // we've scanned the whole image and found nothing.
            return Vector2.Zero;
        }

        // Performs the main while loop of the algorithm
        private List<Vector2> WalkPerimeter(int startX, int startY)
        {
            // Do some sanity checking, so we aren't
            // walking outside the image
            if (startX < 0)
                startX = 0;
            if (startX > texture.Width)
                startX = texture.Width;
            if (startY < 0)
                startY = 0;
            if (startY > texture.Height)
                startY = texture.Height;

            // Set up our return list
            List<Vector2> pointList = new List<Vector2>();

            // Our current x and y positions, initialized
            // to the init values passed in
            int x = startX;
            int y = startY;

            // The main while loop, continues stepping until 
            // we return to our initial points
            do
            {
                // Evaluate our state, and set up our next direction
                Step(x, y);

                // If our current point is within our image
                // add it to the list of points
                if (x >= 0 &&
                    x < texture.Width &&
                    y >= 0 &&
                    y < texture.Height)
                    pointList.Add(new Vector2(x, y));

                switch (nextStep)
                {
                    case StepDirection.Up: y--; break;
                    case StepDirection.Left: x--; break;
                    case StepDirection.Down: y++; break;
                    case StepDirection.Right: x++; break;
                    default:
                        break;
                }
            } while (x != startX || y != startY);

            return pointList;
        }

        // Determines and sets the state of the 4 pixels that
        // represent our current state, and sets our current and
        // previous directions
        private void Step(int x, int y)
        {
            // Scan our 4 pixel area
            bool upLeft = IsPixelSolid(x - 1, y - 1);
            bool upRight = IsPixelSolid(x, y - 1);
            bool downLeft = IsPixelSolid(x - 1, y);
            bool downRight = IsPixelSolid(x, y);

            // Store our previous step
            previousStep = nextStep;

            // Determine which state we are in
            int state = 0;

            if (upLeft)
                state |= 1;
            if (upRight)
                state |= 2;
            if (downLeft)
                state |= 4;
            if (downRight)
                state |= 8;

            // State now contains a number between 0 and 15
            // representing our state.
            // In binary, it looks like 0000-1111 (in binary)

            // An example. Let's say the top two pixels are filled,
            // and the bottom two are empty.
            // Stepping through the if statements above with a state 
            // of 0b0000 initially produces:
            // Upper Left == true ==>  0b0001
            // Upper Right == true ==> 0b0011
            // The others are false, so 0b0011 is our state 
            // (That's 3 in decimal.)

            // Looking at the chart above, we see that state
            // corresponds to a move right, so in our switch statement
            // below, we add a case for 3, and assign Right as the
            // direction of the next step. We repeat this process
            // for all 16 states.

            // So we can use a switch statement to determine our
            // next direction based on
            switch (state)
            {
                case 1: nextStep = StepDirection.Up; break;
                case 2: nextStep = StepDirection.Right; break;
                case 3: nextStep = StepDirection.Right; break;
                case 4: nextStep = StepDirection.Left; break;
                case 5: nextStep = StepDirection.Up; break;
                case 6:
                    if (previousStep == StepDirection.Up)
                    {
                        nextStep = StepDirection.Left;
                    }
                    else
                    {
                        nextStep = StepDirection.Right;
                    }
                    break;
                case 7: nextStep = StepDirection.Right; break;
                case 8: nextStep = StepDirection.Down; break;
                case 9:
                    if (previousStep == StepDirection.Right)
                    {
                        nextStep = StepDirection.Up;
                    }
                    else
                    {
                        nextStep = StepDirection.Down;
                    }
                    break;
                case 10: nextStep = StepDirection.Down; break;
                case 11: nextStep = StepDirection.Down; break;
                case 12: nextStep = StepDirection.Left; break;
                case 13: nextStep = StepDirection.Up; break;
                case 14: nextStep = StepDirection.Left; break;
                default:
                    nextStep = StepDirection.None;
                    break;
            }
        }

        // Determines if a single pixel is solid (we test against
        // alpha values, you can write your own test if you want
        // to test for a different color.)
        private bool IsPixelSolid(int x, int y)
        {
            // Make sure we don't pick a point outside our
            // image boundary!
            if (x < 0 || y < 0 ||
                x >= texture.Width || y >= texture.Height)
                return false;

            // Check the color value of the pixel
            // If it isn't 100% transparent, it is solid
            if (colorData[x + y * texture.Width].A > 0)
                return true;

            // Otherwise, it's not solid
            return false;
        }
    }
}
