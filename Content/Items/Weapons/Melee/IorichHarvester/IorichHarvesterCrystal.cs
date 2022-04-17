using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.IorichHarvester
{
    public class IorichHarvesterCrystal : ModNPC
    {
        Vector2 InitialPosition;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nature Crystal");

            // Afterimage effect
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;    //The length of old position to be recorded
            NPCID.Sets.TrailingMode[NPC.type] = 2;        //The recording mode, this tracks rotation
        }
        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 168;
            NPC.lifeMax = 300;
            NPC.friendly = false;
            NPC.aiStyle = -1;
            NPC.timeLeft = 60 * 60;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.DD2_WitherBeastCrystalImpact;
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, 0f, 1f, 0f);

            if (NPC.ai[0]++ == 0)
            {
                SoundEngine.PlaySound(SoundID.Item46, NPC.Center);

                for (int i = 0; i < 100; i++)
                {
                    Vector2 RandomVelocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(20, 30);
                    Dust.NewDust(NPC.Center, 2, 2, DustID.TerraBlade, RandomVelocity.X, RandomVelocity.Y);
                }

                InitialPosition = NPC.Center;
            }

            NPC.Center = Vector2.Lerp(InitialPosition, InitialPosition + Vector2.UnitY * 20, (float)Math.Sin(NPC.ai[0] / 60f));
        }

        public override bool? CanBeHitByProjectile(Projectile Projectile)
        {
            if (Projectile.type == ModContent.ProjectileType<IorichHarvesterProjectile>() && Projectile.owner == NPC.ai[1])
            {
                return true;
            }

            return false;
        }

        public override void OnHitByProjectile(Projectile Projectile, int damage, float knockback, bool crit)
        {
            NPC.position += Vector2.Normalize(Projectile.velocity);

            Particle.CreateParticle(Particle.ParticleType<Pulse>(), Projectile.Center, Vector2.Zero, Color.LightGreen, 0.5f, 0.5f);

            for (int i = 0; i < Main.rand.Next(3, 6); i++)
            {
                Particle.CreateParticle(Particle.ParticleType<Spark>(), Projectile.Center, Projectile.velocity, Color.LightGreen, 1, 1f);
            }
        }

        public override void OnKill()
        {
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, NPC.Center);
        }

        /*public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Color color26 = Color.LightGreen;
            Texture2D texture2D16 = mod.GetTexture("NPCs/Melee/IorichHarvesterCrystal");

            int num154 = Main.NPCTexture[NPC.type].Height / Main.projFrames[NPC.type];
            int y2 = num154 * NPC.frame;
            Rectangle drawRectangle = new Microsoft.Xna.Framework.Rectangle(0, y2, Main.NPCTexture[NPC.type].Width, num154);

            Vector2 origin2 = drawRectangle.Size() / 2f;

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Color color27 = color26;
                color27 *= (float)(NPCID.Sets.TrailCacheLength[NPC.type] - i) / NPCID.Sets.TrailCacheLength[NPC.type];
                Vector2 value4 = NPC.oldPos[i];
                float num165 = NPC.oldRot[i];
                Main.spriteBatch.Draw(texture2D16, value4 + NPC.Size / 2f - Main.screenPosition + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(drawRectangle), color27, num165, origin2, NPC.scale, SpriteEffects.None, 0f);
            }

            return base.PreDraw(spriteBatch, lightColor);
        }*/
    }

    // This exists to prevent the player from spawning more of the above NPC
    public class IorichHarvesterCrystalProjectile : ModProjectile
    {
        public override bool? CanDamage() => false;
        public override string Texture => "Terraria/Images/Item_" + ProjectileID.LostSoulFriendly;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nature Crystal");
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 168;
            Projectile.friendly = true;
            Projectile.timeLeft = 60 * 60;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            if (Main.npc[(int)Projectile.ai[0]].active)
            {
                Projectile.timeLeft = 5;
            }
        }
    }
}