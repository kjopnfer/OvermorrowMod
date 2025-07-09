using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    public class StellarCorona : ModProjectile
    {
        public override string Texture => AssetDirectory.ArchiveProjectiles + Name;
        public override bool? CanDamage()
        {
            return base.CanDamage();
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 114;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.tileCollide = false;
            Projectile.localNPCHitCooldown = ModUtils.SecondsToTicks(1);
            Projectile.timeLeft = ModUtils.SecondsToTicks(10);
            Projectile.DamageType = DamageClass.Magic;
            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }

        public ref float AICounter => ref Projectile.ai[0];
        public ref float TargetID => ref Projectile.ai[1];
        public override void AI()
        {
            NPC npc = Main.npc[(int)TargetID];
            if (!npc.active) Projectile.Kill();

            Projectile.rotation += 0.04f;
            Projectile.Center = npc.Center;

            AICounter++;
            if (AICounter > ModUtils.SecondsToTicks(8)) return;

            if (AICounter % 20 == 0)
            {
                float angle = Main.rand.NextFloat(0f, MathHelper.TwoPi); // Random angle in radians
                float speed = 6f; // Adjust the speed as desired
                Vector2 randomVelocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * speed;

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, randomVelocity, ModContent.ProjectileType<StellarProminence>(), Projectile.damage / 2, 0f, Projectile.owner, ai0: TargetID);
            }

            if (AICounter % 10 == 0)
            {
                Texture2D lightTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "ray").Value;

                for (int i = 0; i < 3; i++)
                {
                    var lightRay = new Light(lightTexture, ModUtils.SecondsToTicks(3), npc, Vector2.Zero);
                    float randomRotation = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                    float randomSize = Main.rand.NextFloat(0.1f, 0.3f);
                    ParticleManager.CreateParticleDirect(lightRay, npc.Center, Vector2.Zero, Color.White, 1f, randomSize, randomRotation, ParticleDrawLayer.BehindNPCs, useAdditiveBlending: true); // Scale of 3f becomes the max height
                }
            }
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            base.ModifyDamageHitbox(ref hitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            float sine = (float)Math.Sin(AICounter / 15f); // Adjust divisor to control frequency
            float alpha = MathHelper.Lerp(0.6f, 1f, (sine + 1f) / 2f); // Normalize sine to [0,1]

            float scale = MathHelper.Lerp(0f, 1f, Utils.Clamp(AICounter, 0f, 80f) / 80f);
            if (AICounter > ModUtils.SecondsToTicks(8))
                scale = MathHelper.Lerp(1f, 0f, Utils.Clamp(AICounter - ModUtils.SecondsToTicks(8), 0f, ModUtils.SecondsToTicks(2)) / ModUtils.SecondsToTicks(2));

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation, texture.Size() / 2f, scale, SpriteEffects.None, 1);

            return false;
        }
    }
}