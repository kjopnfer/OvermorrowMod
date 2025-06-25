using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Primitives.Trails;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common;
using System.Collections.Generic;
using Terraria.ModLoader;
using OvermorrowMod.Common.Utilities;
using Terraria;
using OvermorrowMod.Content.Particles;
using System;
using OvermorrowMod.Core.Particles;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.DataStructures;
using OvermorrowMod.Core.Interfaces;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    public class StellarProminence : ModProjectile, ITrailEntity, IDrawAdditive
    {
        public IEnumerable<TrailConfig> TrailConfigurations()
        {
            return new List<TrailConfig>
            {
                new TrailConfig(
                    typeof(LaserTrail),
                    progress => Color.Lerp(Color.White, Color.White, progress) * MathHelper.SmoothStep(0, 1, progress),
                    progress => 10
                ),
                new TrailConfig(
                    typeof(FireTrail),
                    progress => DrawUtils.ColorLerp3(Color.White, Color.White, Color.White, progress) *  MathHelper.SmoothStep(0, 1, progress),
                    progress => 20
                )
            };
        }

        public override string Texture => AssetDirectory.Empty;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = ModUtils.SecondsToTicks(2);
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1; 
            Projectile.extraUpdates = 1;
            Projectile.DamageType = DamageClass.Magic;
        }

        public ref float AICounter => ref Projectile.ai[1];
        public ref float TargetID => ref Projectile.ai[0];

        public override void OnSpawn(IEntitySource source)
        {
            // Randomized turn config
            Projectile.localAI[0] = MathHelper.ToRadians(Main.rand.NextFloat(6f, 10f)); // maxTurn
            Projectile.localAI[1] = Main.rand.NextFloat(0.1f, 0.2f); // turnLerp
        }

        public override void AI()
        {
            NPC target = Main.npc[(int)TargetID];
            if (!target.active || target.friendly || target.life <= 0)
            {
                Projectile.Kill();
                return;
            }

            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0.25f);

            if (AICounter++ > 30)
            {
                Vector2 toTarget = target.Center - Projectile.Center;
                float desiredAngle = toTarget.ToRotation();
                float currentAngle = Projectile.velocity.ToRotation();

                float maxTurn = MathHelper.ToRadians(Projectile.localAI[0]); // turning speed
                float newAngle = currentAngle.AngleLerp(desiredAngle, Projectile.localAI[1]);

                float speed = Projectile.velocity.Length();
                Projectile.velocity = speed * newAngle.ToRotationVector2();

                if (Vector2.Distance(Projectile.Center, target.Center) < 16)
                {
                    Projectile.Kill();
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            /*float particleScale = 0.1f;
            Color color = Color.White;

            Texture2D circle = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_01").Value;
            //float alpha = MathHelper.Lerp(0.5f, 0f, Utils.Clamp(AICounter, 0f, 80f) / 80f);
            //float scale = MathHelper.Lerp(0f, 6f, Utils.Clamp(AICounter, 0f, 80f) / 80f);

            Main.spriteBatch.Reload(BlendState.Additive);

            float alpha = 1f;
            float scale = 0.25f;

            Texture2D tex2 = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_05").Value;
            //float alpha = MathHelper.Lerp(0.5f, 0f, Utils.Clamp(AICounter, 0f, 80f) / 80f);
            //float scale = MathHelper.Lerp(0f, 6f, Utils.Clamp(AICounter, 0f, 80f) / 80f);
            scale = 0.5f;
            Main.spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, color * 0.5f, Projectile.velocity.ToRotation(), tex2.Size() / 2f, new Vector2(0.7f, 0.25f), SpriteEffects.None, 1);
            Main.spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, color * 0.3f, Projectile.velocity.ToRotation(), tex2.Size() / 2f, new Vector2(1f, 0.25f), SpriteEffects.None, 1);

            Main.spriteBatch.Draw(circle, Projectile.Center - Main.screenPosition, null, Color.White * 0.7f, MathHelper.PiOver2, circle.Size() / 2f, 0.2f, SpriteEffects.None, 1);
            Main.spriteBatch.Draw(circle, Projectile.Center - Main.screenPosition, null, color * 0.4f, MathHelper.PiOver2, circle.Size() / 2f, 0.5f, SpriteEffects.None, 1);

            //Main.spriteBatch.Draw(circle, Projectile.Center - Main.screenPosition, null, color * 0.7f, MathHelper.PiOver4, circle.Size() / 2f, scale, SpriteEffects.None, 1);

            Main.spriteBatch.Reload(BlendState.AlphaBlend);*/


            return base.PreDraw(ref lightColor);
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            Color color = Color.White;

            Texture2D circle = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_01").Value;
            Texture2D tex2 = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_05").Value;

            spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, color * 0.5f, Projectile.velocity.ToRotation(), tex2.Size() / 2f, new Vector2(0.7f, 0.25f), SpriteEffects.None, 1);
            spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, color * 0.3f, Projectile.velocity.ToRotation(), tex2.Size() / 2f, new Vector2(1f, 0.25f), SpriteEffects.None, 1);

            spriteBatch.Draw(circle, Projectile.Center - Main.screenPosition, null, Color.White * 0.7f, MathHelper.PiOver2, circle.Size() / 2f, 0.2f, SpriteEffects.None, 1);
            spriteBatch.Draw(circle, Projectile.Center - Main.screenPosition, null, color * 0.4f, MathHelper.PiOver2, circle.Size() / 2f, 0.5f, SpriteEffects.None, 1);
        }
    }
}