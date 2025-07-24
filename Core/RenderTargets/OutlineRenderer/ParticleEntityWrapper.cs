using Microsoft.Xna.Framework;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Particles;
using Terraria;

namespace OvermorrowMod.Core.RenderTargets
{
    /// <summary>
    /// Wrapper class to make particles work with the Entity-based system
    /// </summary>
    public class ParticleEntityWrapper : Entity
    {
        private ParticleInstance particle;
        private IOutlineEntity outlineParticle;

        public ParticleEntityWrapper(ParticleInstance particleInstance, IOutlineEntity outlineEntity)
        {
            this.particle = particleInstance;
            this.outlineParticle = outlineEntity;

            // Get dimensions from the particle
            var customParticle = particleInstance.cParticle;
            this.width = customParticle.Width;
            this.height = customParticle.Height;

            // Map other particle properties to entity properties
            this.position = particleInstance.position - new Vector2(width / 2f, height / 2f);
            this.whoAmI = particleInstance.id;
            this.active = particleInstance.cParticle != null;

        }

        public new Vector2 Center
        {
            get => particle.position;
            set
            {
                particle.position = value;
                this.position = value - new Vector2(width / 2f, height / 2f);
            }
        }

        public override Vector2 VisualPosition => particle.position;

        public void UpdateFromParticle()
        {
            this.oldPosition = this.position;
            this.oldVelocity = this.velocity;

            this.position = particle.position - new Vector2(width / 2f, height / 2f);
            this.velocity = particle.velocity;
            this.active = particle.cParticle != null && particle.alpha > 0;
        }

        public ParticleInstance GetParticleInstance() => particle;
        public IOutlineEntity GetOutlineEntity() => outlineParticle;
    }
}