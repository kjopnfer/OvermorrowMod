using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core.Interfaces;
using System.Collections.Generic;
using System;
using Terraria;
using Microsoft.Xna.Framework;

namespace OvermorrowMod.Core.RenderTargets
{
    public class EntityGroup
    {
        public List<(Entity entity, IOutlineEntity outlineEntity)> Entities { get; set; }
        public Color OutlineColor { get; set; }
        public Color FillColor { get; set; }
        public Texture2D FillTexture { get; set; }
        public bool UseFillColor { get; set; }
        public System.Action<SpriteBatch, GraphicsDevice, int, int, Entity> CustomDrawFunction { get; set; }
    }
}