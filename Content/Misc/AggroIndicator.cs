using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Misc
{
    public class AggroIndicator : ModProjectile
    {
        public override string Texture => AssetDirectory.Misc + Name;
        public override bool ShouldUpdatePosition() => false;
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.timeLeft = 120;
        }

        public ref float ParentID => ref Projectile.ai[0];
        public ref float AICounter => ref Projectile.ai[1];

        public override void OnSpawn(IEntitySource source)
        {
        }

        public override void AI()
        {
            NPC npc = Main.npc[(int)ParentID];
            if (!npc.active) Projectile.Kill();

            AICounter++;
            float progress = MathHelper.Clamp(EasingUtils.EaseOutBounce(AICounter / 20f), 0, 1f);
            Projectile.Center = npc.Hitbox.Top() + Vector2.Lerp(Vector2.UnitY * -10, Vector2.UnitY * -40f, progress);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Misc + Name).Value;
            Color textureColor = Color.White;
            float size = 1f;
            float alpha = 1f;

            //Projectile.rotation = MathHelper.Lerp(0, -MathHelper.PiOver4, EasingUtils.EaseOutBounce(Math.Clamp(MathHelper.Lerp(0, 1f, Projectile.timeLeft / 60f), 0, 1f)));
            if (AICounter >= 60) alpha = Math.Clamp(MathHelper.Lerp(1f, 0f, (AICounter - 60) / 60f), 0, 1f);

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, textureColor * alpha, Projectile.rotation, texture.Size() / 2f, size, SpriteEffects.None, 0);

            return false;
        }
    }
}