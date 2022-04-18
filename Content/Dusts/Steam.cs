using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Dusts
{
    public class Steam : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = false;
            dust.frame = new Rectangle(0, 0, 64, 64);
            dust.fadeIn = 0;
            dust.rotation = Main.rand.NextFloat(0, MathHelper.Pi);
        }

        public override bool Update(Dust dust)
        {
            dust.rotation += 0.005f;
            //dust.position += dust.velocity;
            //dust.position.X += (float)Math.Sin(dust.fadeIn) * 0.25f;

            dust.alpha++;
            dust.velocity.Y -= 0.03f;
            dust.scale *= 1.005f;

            if (dust.fadeIn > 120)
                dust.active = false;

            return false;
        }
    }
}