using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Core;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Primitives;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace OvermorrowMod.Content.NPCs.Bosses.Eye
{
    /*public class Verlet
    {
        public struct ChainSegment
        {
            public Vector2 currentPosition;
            public Vector2 oldPosition;

            public ChainSegment(Vector2 position)
            {
                currentPosition = position;
                oldPosition = position;
            }
        }
        private List<ChainSegment> chainSegments = new List<ChainSegment>();
        private float chainLength = 0.25f;
        private int segmentLength = 35;
        private float lineWidth = 0.1f;

        public void Initialize(Vector2 startPoint)
        {
            Vector2 calculatedPoint = startPoint;
            for (int i = 0; i < segmentLength; i++)
            {
                chainSegments.Add(new ChainSegment(calculatedPoint));
                calculatedPoint.Y -= segmentLength;
            }
        }

        // This is called in PreDraw
        void Update() { }

        public void Draw()
        {
            float lineWidth = this.lineWidth;

            Vector2[] ropePositions = new Vector2[segmentLength];
            for (int i = 0; i < segmentLength; i++)
            {
                ropePositions[i] = chainSegments[i].currentPosition;
            }


        }
    }

    */

    public class Verlet
    {
        private List<VerletNode> nodes;
        private List<VerletConnection> connections;

        private float bounce = 0.9f;
        private float gravity = 0.98f;
        private float friction = 0.999f;
        public void UpdatePoints()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                var velocityX = (node.position.X - node.oldPosition.X) * friction;
                var velocityY = (node.position.Y - node.oldPosition.Y) * friction;

                node.oldPosition.X = node.position.X;
                node.oldPosition.Y = node.position.Y;

                node.position.X += velocityX;
                node.position.Y += velocityY;
                node.position.Y += gravity;

            }
        }

        public void UpdateConnections()
        {
            for (int i = 0; i < connections.Count; i++)
            {
                var connection = connections[i];
                var dx = connection.endNode.position.X - connection.startNode.position.X;
                var dy = connection.endNode.position.Y - connection.startNode.position.Y;

                var distance = Math.Sqrt(dx * dx + dy * dy);
                var difference = connection.length - distance;
                var percent = difference / distance / 2;

                var offsetX = dx * percent;
                var offsetY = dy * percent;
            }
        }

        public void RenderPoints(SpriteBatch spriteBatch)
        {
            Texture2D texture = TextureAssets.MagicPixel.Value;
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                float rotation = Vector2.Distance(node.position, nodes[i + 1].position).ToRotationVector2().ToRotation();
                spriteBatch.Draw(texture, node.position - Main.screenPosition, null, Color.White, rotation, texture.Size() / 2, 1f, SpriteEffects.None, 1);
            }
        }
    }

    public class VerletNode
    {
        public Vector2 position, oldPosition;

        public VerletNode(Vector2 startPosition)
        {
            position = startPosition;
            oldPosition = startPosition;
        }
    }

    public class VerletConnection
    {
        public VerletNode startNode, endNode;
        public float length;

        public VerletConnection(VerletNode startNode, VerletNode endNode)
        {
            this.startNode = startNode;
            this.endNode = endNode;

            length = Vector2.Distance(startNode.position, endNode.position);
        }
    }
}