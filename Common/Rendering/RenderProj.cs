using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Rendering
{
    public class RenderProj : GlobalProjectile
    {
        private bool draw;
        private ExternalRenderable render;
        private Renderer renderer;
        public override bool InstancePerEntity => true;

        public Renderer Renderer
        {
            get => renderer;
            set
            {
                renderer?.Remove(render);
                renderer = value;
                renderer?.Add(render);
            }
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            render = new ExternalRenderable(() =>
            {
                draw = true;
                Main.instance.DrawProj(projectile.whoAmI);
                draw = false;
            });
            //Renderer = Rendering.GetRenderer<TestRenderer>();
        }

        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            return Renderer == null || draw;
        }

        public override void Kill(Projectile projectile, int timeLeft)
        {
            Renderer?.Remove(render);
        }
    }
}