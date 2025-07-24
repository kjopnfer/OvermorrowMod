using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Particles;
using System;
using Terraria;

namespace OvermorrowMod.Common.Particles
{
    /// <summary>
    /// Draws a CustomParticle using a RenderTarget.
    /// </summary>
    public class OutlineParticle : CustomParticle, IOutlineEntity
    {
        public bool ShouldDrawOutline { get; set; } = false;
        public Color OutlineColor { get; set; } = Color.White;
        public Color? FillColor { get; set; } = null;
        public Texture2D FillTexture { get; set; } = null;
        public int MaxLifetime { get; set; } = 300;
        public bool FadeOut { get; set; } = true;
        public Action<SpriteBatch, GraphicsDevice, int, int> SharedGroupDrawFunction { get; set; } = null;
        public Action<SpriteBatch, GraphicsDevice, Entity> IndividualEntityDrawFunction { get; set; } = null;

        /// <summary>
        /// A way to get the particle's position/properties
        /// </summary>
        public ParticleInstance ParticleInstance { get; set; }

        public OutlineParticle(string texture = null, int width = 16, int height = 16) : base()
        {
            Width = width;
            Height = height;

            if (!string.IsNullOrEmpty(texture))
            {
                Texture = texture;
            }
        }

        public override void Update()
        {
            base.Update();

            if (FadeOut && particle.activeTime > MaxLifetime * 0.7f)
            {
                float fadeProgress = (particle.activeTime - MaxLifetime * 0.7f) / (MaxLifetime * 0.3f);
                particle.alpha = MathHelper.Lerp(1f, 0f, fadeProgress);
            }

            if (particle.activeTime >= MaxLifetime || particle.alpha <= 0.01f)
            {
                particle.Kill();
            }
        }
    }
}