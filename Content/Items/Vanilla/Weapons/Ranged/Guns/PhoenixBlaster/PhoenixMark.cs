using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Vanilla.Weapons.Ranged
{
    public class PhoenixMark : ModProjectile
    {
        public override string Texture => AssetDirectory.Resprites + Name;
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

            //Main.spriteBatch.Draw(texture, Projectile.Center - new Vector2(0, targetNPC.height * 2f) - Main.screenPosition, null, Color.Red * alpha, 0f, texture.Size() / 2f, scale, SpriteEffects.None, 1);

            scale = MathHelper.Lerp(0.9f, 1.1f, (float)(Math.Sin(AICounter / 15f) / 2 + 0.5f));
            //Main.spriteBatch.Draw(texture, Projectile.Center - new Vector2(0, targetNPC.height * 2f) - Main.screenPosition, null, Color.White * 0.75f, 0f, texture.Size() / 2f, scale, SpriteEffects.None, 1);

           
            return false;
        }
    }

    public class PhoenixMarkBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Phoenix Mark");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = false;
            Main.buffNoSave[Type] = true;
        }
    }
}