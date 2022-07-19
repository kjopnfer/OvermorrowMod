using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Dusts
{
    public class ShadowForm : ModDust
    {
        int counter = 0;

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
            dust.frame = new Rectangle(0, 0, 10, 10);
            dust.fadeIn = 0;
            dust.rotation = Main.rand.NextFloat(0, MathHelper.Pi);
        }

        public override bool Update(Dust dust)
        {
            dust.velocity *= 0.99f;
            dust.scale *= 0.99f;

            if (counter++ % 2 == 0) dust.alpha++;

            if (dust.alpha == 255) dust.active = false;

            return false;
        }
    }
}