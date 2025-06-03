using Microsoft.Xna.Framework.Graphics;
using System;

namespace OvermorrowMod.Common.Particles
{
    public class ConfigurableParticle : Particle
    {
        private Action<ConfigurableParticle> updateCallback;
        public Action<SpriteBatch> CustomDraw { get; set; }

        public ConfigurableParticle(Action<ConfigurableParticle> callback, string texture = null) : base(texture)
        {
            this.updateCallback = callback;
        }

        public override void Update()
        {
            base.Update(); // Run standard behaviors first
            updateCallback?.Invoke(this); // Then run custom callback logic
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (CustomDraw != null)
                CustomDraw(spriteBatch);
            else
                base.Draw(spriteBatch); // Fall back to default drawing
        }
    }
}