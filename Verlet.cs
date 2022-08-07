using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using System;
using System.Collections.Generic;

namespace OvermorrowMod
{
    public class VerletPoint
    {
        public Vector2 prevPosition;
        public Vector2 position;
        public VerletPoint[] connections;
        public bool locked;

        public VerletPoint(Vector2 _prevPosition, Vector2 _position, VerletPoint[] _connections, bool _locked)
        {
            prevPosition = _prevPosition;
            position = _position;
            connections = _connections;
            locked = _locked;
        }
    }

    public class VerletStick
    {
        public VerletPoint point1;
        public VerletPoint point2;
        public float length;

        public VerletStick(VerletPoint _point1, VerletPoint _point2)
        {
            point1 = _point1;
            point2 = _point2;
            length = Vector2.Distance(point1.position, point2.position);
        }
    }

    static class Verlet
    {

        public static VerletStick[] GetVerletSticks(VerletPoint[] points)
        {
            int stickCount = 0;
            int maxSticks = 0;
            foreach (VerletPoint p in points)
            {
                if (p.connections == null) continue;

                foreach (VerletPoint p2 in p.connections) maxSticks++;
            }

            if (maxSticks < 1) return null;

            VerletStick[] sticks = new VerletStick[maxSticks];
            foreach (VerletPoint p in points)
            {
                if (p.connections == null) continue;

                foreach (VerletPoint p2 in p.connections)
                {
                    sticks[stickCount] = new VerletStick(p, p2);
                    stickCount++;
                }

            }
            return sticks;
        }

        public static VerletPoint[] VerletPointsInLine_Offset(Vector2 one, Vector2 two, float[] offsets, int num_points, bool first_Locked = true, bool final_locked = false)
        {
            VerletPoint[] points = new VerletPoint[num_points];
            Vector2 direction = Vector2.Normalize(one - two);
            float dst = Vector2.Distance(one, two) / (float)num_points;
            for (int i = 0; i < num_points; i++)
            {
                Vector2 pos = one + direction * (dst * offsets[i]);
                VerletPoint[] connections = null;
                points[i] = new VerletPoint(pos, pos, connections, false);
            }
            for (int i = 0; i < num_points; i++)
            {
                int connectCount = i > 0 && i < num_points - 1 ? 2 : 1;
                VerletPoint[] connections = new VerletPoint[connectCount];
                bool is_locked = false;
                if (i == 0)
                {
                    connections[0] = points[1];
                    is_locked = first_Locked;
                }
                else if (i == num_points - 1)
                {
                    connections[0] = points[i - 1];
                    is_locked = final_locked;
                }
                else
                {
                    connections[0] = points[i - 1];
                    connections[1] = points[i + 1];
                }

                points[i].connections = connections;
                points[i].locked = is_locked;
            }
            return points;

        }
        public static VerletPoint[] VerletPointsInLine_Even(Vector2 one, Vector2 two, int num_points, bool first_Locked = true, bool final_locked = false)
        {
            VerletPoint[] points = new VerletPoint[num_points];
            Vector2 direction = Vector2.Normalize(one - two);
            float dst = Vector2.Distance(one, two) / (float)num_points;
            for (int i = 0; i < num_points; i++)
            {
                Vector2 pos = one + direction * i * dst;
                VerletPoint[] connections = null;

                points[i] = new VerletPoint(pos, pos, connections, false);
            }
            for (int i = 0; i < num_points; i++)
            {
                int connectCount = i > 0 && i < num_points - 1 ? 2 : 1;
                VerletPoint[] connections = new VerletPoint[connectCount];
                bool is_locked = false;
                if (i == 0)
                {
                    connections[0] = points[1];
                    is_locked = first_Locked;
                }
                else if (i == num_points - 1)
                {
                    connections[0] = points[i - 1];
                    is_locked = final_locked;
                }
                else
                {
                    connections[0] = points[i - 1];
                    connections[1] = points[i + 1];
                }

                points[i].connections = connections;
                points[i].locked = is_locked;
            }
            return points;

        }

        public static VerletPoint[] GenerateVerlet(Texture2D texture, Vector2 startPosition, Vector2 endPosition, bool firstLocked = true, bool lastLocked = false)
        {
            // Generate the number of points according to texture size and the start/end positions
            float distance = Vector2.Distance(startPosition, endPosition);
            int numPoints = (int)(distance / texture.Height);
            VerletPoint[] points = new VerletPoint[numPoints];

            // Initialize and add the verlet points into the array
            Vector2 direction = Vector2.Normalize(startPosition - endPosition);
            float pointDistance = distance / (float)numPoints;
            for (int i = 0; i < numPoints; i++)
            {
                Vector2 position = startPosition + direction * i * pointDistance;
                VerletPoint[] connections = null;

                points[i] = new VerletPoint(position, position, connections, false);
            }

            // Add in the connections for the points
            for (int i = 0; i < numPoints; i++)
            {
                // Determine the number of connections, if its the end then it just has one connection
                int connectCount = i > 0 && i < numPoints - 1 ? 2 : 1;
                VerletPoint[] connections = new VerletPoint[connectCount];

                if (i == 0) // Determine if first point is locked
                {
                    connections[0] = points[1];
                    points[i].locked = firstLocked;
                }
                else if (i == numPoints - 1) // Determine if last point is locked
                {
                    connections[0] = points[i - 1];
                    points[i].locked = lastLocked;
                }
                else // All the other points inbetween, get the previous point and the next point
                {
                    connections[0] = points[i - 1];
                    connections[1] = points[i + 1];
                }

                points[i].connections = connections;
            }

            return points;
        }

        public static VerletPoint[] SimulateVerlet(VerletPoint[] points, VerletStick[] sticks, Vector2 down, float delta, int depth = 10, float gravity = 100f, bool wind = true)
        {
            // Simulate the movement of the points if the point isn't locked in place
            foreach (VerletPoint p in points)
            {
                if (!p.locked)
                {
                    Vector2 positionBeforeUpdate = p.position;
                    p.position += p.position - p.prevPosition;
                    p.position += down * gravity * delta * delta;

                    if (wind) p.position.X += Main.windSpeedCurrent / 2;

                    p.prevPosition = positionBeforeUpdate;
                }
            }

            // Simulate the points based on the connections
            for (int i = 0; i < depth; i++)
            {
                if (sticks == null) continue;

                foreach (VerletStick stick in sticks)
                {
                    Vector2 stickCentre = (stick.point1.position + stick.point2.position) / 2;
                    Vector2 stickDir = Vector2.Normalize(stick.point1.position - stick.point2.position);
                    if (!stick.point1.locked) stick.point1.position = stickCentre + stickDir * stick.length / 2;

                    if (!stick.point2.locked) stick.point2.position = stickCentre - stickDir * stick.length / 2;
                }
            }

            return points;
        }

        public static void DrawVerlet(VerletPoint[] points, SpriteBatch spriteBatch)
        {
            foreach (VerletPoint point in points)
            {
                if (point.connections == null) continue;

                foreach (VerletPoint p2 in point.connections)
                {
                    Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Chains + "Bones").Value;
                    spriteBatch.Draw(texture, point.position - Main.screenPosition, null, Color.White, point.position.DirectionTo(p2.position).ToRotation() + MathHelper.PiOver2, texture.Size() / 2, 1f, SpriteEffects.None, 1); ;
                }
            }
        }

        public static void DrawVerlet(VerletPoint[] points)
        {
            foreach (VerletPoint point in points)
            {
                if (point.connections == null) continue;

                foreach (VerletPoint p2 in point.connections)
                {
                    Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Chains + "Bones").Value;
                    Main.EntitySpriteDraw(texture, point.position - Main.screenPosition, null, Color.White, point.position.DirectionTo(p2.position).ToRotation() + MathHelper.PiOver2, texture.Size() / 2, 1f, SpriteEffects.None, 1);
                }
            }
        }
    }
}