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
            if (!Pixelated)
            {
                pos -= Main.screenPosition;
            }
            else
            {
                pos = (pos - Main.screenPosition) / 2;
            }

            Vertices.Add(new VertexPositionColorTexture(new Vector3(pos.X, pos.Y, 0), color, texCoord));
        }

        public abstract void Update();
        public abstract void PrepareTrail();
        public abstract void Draw();
        public abstract void UpdateDead();
    }
}
