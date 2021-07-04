using Terraria;
using System;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Particles
{
    public class Explosion : CustomParticle
    {
        float maxTime = 45f;
        public int frame;
        public int maxFrames = 7;
        public override void OnSpawn()
        {
        }
        public override void Update()
        {
            frame = (int)(maxFrames / maxTime * particle.activeTime);
            if (particle.activeTime > maxTime) particle.Kill();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Particle.ParticleTextures[particle.type];
            int frameHeight = texture.Height / maxFrames;
            int curFrame = frameHeight * frame;
            Rectangle source = new Rectangle(0, curFrame, texture.Width, frameHeight);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, source, particle.color, particle.rotation, source.Size() / 2, particle.scale, SpriteEffects.None, 0f);
        }
    }
}