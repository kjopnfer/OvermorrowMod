using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla.Guns
{
    public class PhoenixMark : ModProjectile
    {
        //public override string Texture => AssetDirectory.Empty;
        public override bool? CanDamage() => false;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Phoenix Burst");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 360;
        }

        public ref float TargetNPC => ref Projectile.ai[0];
        public ref float AICounter => ref Projectile.ai[1];
        public override void AI()
        {
            NPC targetNPC = Main.npc[(int)TargetNPC];
            if (!targetNPC.active) Projectile.Kill();
            if (targetNPC.HasBuff<PhoenixMarkBuff>())
            {
                Projectile.timeLeft = 2;
            }

            Projectile.Center = targetNPC.Center;
            AICounter++;
            Projectile.scale = MathHelper.Lerp(0.5f, 0, Projectile.timeLeft / 120f);
            Projectile.rotation += 0.05f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            NPC targetNPC = Main.npc[(int)TargetNPC];

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            float scale = MathHelper.SmoothStep(0f, 1f, AICounter % 80f / 80f);
            float alpha = MathHelper.Lerp(2f, 0, AICounter % 80f / 80f);

            //if (Projectile.timeLeft <= 60) scale = MathHelper.Lerp(1f, 0f, Utils.Clamp(AICounter - 340, 0, 20f) / 20f);

            Main.spriteBatch.Draw(texture, Projectile.Center - new Vector2(0, targetNPC.height * 2f) - Main.screenPosition, null, Color.Red * alpha, 0f, texture.Size() / 2f, scale, SpriteEffects.None, 1);

            scale = MathHelper.Lerp(0.9f, 1.1f, (float)(Math.Sin(AICounter / 15f) / 2 + 0.5f));
            Main.spriteBatch.Draw(texture, Projectile.Center - new Vector2(0, targetNPC.height * 2f) - Main.screenPosition, null, Color.White * 0.75f, 0f, texture.Size() / 2f, scale, SpriteEffects.None, 1);

           
            return false;
            //Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01").Value;

            //float scale = MathHelper.Lerp(0, 0.25f, Utils.Clamp(AICounter, 0, 20f) / 20f);
            if (Projectile.timeLeft <= 60) scale = MathHelper.Lerp(0.25f, 0f, Utils.Clamp(AICounter - 340, 0, 20f) / 20f);

            Color color = Color.Lerp(Color.DarkOrange, Color.DarkRed, (float)(Math.Sin(AICounter / 10f) / 2 + 0.5f));
            for (int i = 0; i <= 3; i++)
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color * alpha, Projectile.rotation + MathHelper.PiOver2 * i, texture.Size() / 2f, scale, SpriteEffects.None, 1);

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Crosshair").Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color * alpha, Projectile.rotation, texture.Size() / 2f, scale, SpriteEffects.None, 1);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color * alpha, Projectile.rotation + MathHelper.PiOver4, texture.Size() / 2f, scale * 0.5f, SpriteEffects.None, 1);

            return false;
        }
    }

    public class PhoenixMarkBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Phoenix Mark");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = false;
            Main.buffNoSave[Type] = true;
        }
    }
}