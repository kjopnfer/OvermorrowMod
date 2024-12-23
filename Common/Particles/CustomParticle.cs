using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using Terraria;

namespace OvermorrowMod.Common.Particles
{
    public class CustomParticle
    {
        public static Dictionary<int, CustomParticle> CustomParticles;
        public OvermorrowModFile mod;
        public static CustomParticle GetCParticle(int type) => CustomParticles[type];
        public Particle particle;
        //public virtual void OnSpawn() { }
        public virtual void OnSpawn() { }

        public virtual void Update() { }
        public virtual string Texture { get { return null; } private set { } }
        public virtual bool ShouldUpdatePosition() => true;
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Particle.GetTexture(particle.type);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, particle.rotation, new Vector2(texture.Width / 2, texture.Height / 2), particle.scale, SpriteEffects.None, 0f);
        }
    }
}