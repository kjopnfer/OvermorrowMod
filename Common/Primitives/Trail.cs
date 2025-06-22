using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Detours;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using Terraria;

namespace OvermorrowMod.Common.Primitives
{
    public abstract class Trail
    {
        protected Effect Effect { get; }
        public List<VertexPositionColorTexture> Vertices { get; } = new List<VertexPositionColorTexture>();
        protected Texture2D Texture { get; }
        protected TrailPositionBuffer Positions { get; }
        public bool Dying = false;
        public bool Dead = false;
        public int EntityID = -1;
        public Entity Entity;
        public ITrailEntity TrailEntity;
        public DrawType DrawType = DrawType.Projectile;
        public bool Pixelated = false;

        private TrailConfig _config;

        public TrailConfig Config
        {
            get => _config;
            set
            {
                if (_config == null) _config = value;
                else throw new InvalidOperationException("Config can only be set once.");
            }
        }

        protected Trail(int length, Texture2D texture, Effect effect = null)
        {
            Positions = new TrailPositionBuffer(length);
            Texture = texture;
            Effect = effect ?? OvermorrowModFile.Instance.TrailShader.Value;
        }

        public void AddVertex(Vector2 pos, Color color, Vector2 texCoord)
        {
            Vector2 drawPos = GetGravityAwarePosition(pos);

            if (!Pixelated)
            {
                drawPos -= Main.screenPosition;
            }
            else
            {
                drawPos = (drawPos - Main.screenPosition) / 2;
            }

            Vertices.Add(new VertexPositionColorTexture(new Vector3(drawPos.X, drawPos.Y, 0), color, texCoord));
        }

        private Vector2 GetGravityAwarePosition(Vector2 worldPosition)
        {
            if (Main.LocalPlayer.gravDir == -1f)
            {
                // Flip Y position relative to world center or screen center
                Vector2 screenCenter = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
                return new Vector2(
                    worldPosition.X,
                    screenCenter.Y - (worldPosition.Y - screenCenter.Y)
                );
            }

            return worldPosition;
        }

        public abstract void Update();
        public abstract void PrepareTrail();
        public abstract void Draw();
        public abstract void UpdateDead();
    }
}
