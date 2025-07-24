using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Particles;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Particles
{
    public abstract class CustomParticle
    {
        public OvermorrowModFile mod;
        public ParticleInstance particle;

        public float rotationAmount = 0f;
        public int intensity = 1;

        public virtual int Width { get; protected set; } = 16;
        public virtual int Height { get; protected set; } = 16;

        public virtual void OnSpawn() { }
        public virtual void Update() { }
        public virtual string Texture { get; protected set; } = AssetDirectory.Empty;
        public virtual bool ShouldUpdatePosition() => true;

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (string.IsNullOrEmpty(Texture) || Texture == AssetDirectory.Empty) return;

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null,
                particle.color * particle.alpha, particle.rotation,
                new Vector2(texture.Width / 2, texture.Height / 2),
                particle.scale, SpriteEffects.None, 0f);
        }
    }
}