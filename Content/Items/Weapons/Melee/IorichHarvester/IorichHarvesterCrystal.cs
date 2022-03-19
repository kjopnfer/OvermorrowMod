using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Particles;
using System;
using Terraria;
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
            NPCID.Sets.TrailCacheLength[npc.type] = 10;    //The length of old position to be recorded
            NPCID.Sets.TrailingMode[npc.type] = 2;        //The recording mode, this tracks rotation
        }
        public override void SetDefaults()
        {
            npc.width = 100;
            npc.height = 168;
            npc.lifeMax = 300;
            npc.friendly = false;
            npc.aiStyle = -1;
            npc.timeLeft = 60 * 60;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.DD2_WitherBeastCrystalImpact;
        }

        public override void AI()
        {
            Lighting.AddLight(npc.Center, 0f, 1f, 0f);

            if (npc.ai[0]++ == 0)
            {
                Main.PlaySound(SoundID.Item46, npc.Center);

                for (int i = 0; i < 100; i++)
                {
                    Vector2 RandomVelocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(20, 30);
                    Dust.NewDust(npc.Center, 2, 2, DustID.TerraBlade, RandomVelocity.X, RandomVelocity.Y);
                }

                InitialPosition = npc.Center;
            }

            npc.Center = Vector2.Lerp(InitialPosition, InitialPosition + Vector2.UnitY * 20, (float)Math.Sin(npc.ai[0] / 60f));
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.type == ModContent.ProjectileType<IorichHarvesterProjectile>() && projectile.owner == npc.ai[1])
            {
                return true;
            }

            return false;
        }

        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            npc.position += Vector2.Normalize(projectile.velocity);

            Particle.CreateParticle(Particle.ParticleType<Shockwave>(), projectile.Center, Vector2.Zero, Color.LightGreen, 0.5f, 0.5f);

            for (int i = 0; i < Main.rand.Next(3, 6); i++)
            {
                Particle.CreateParticle(Particle.ParticleType<Spark>(), projectile.Center, projectile.velocity, Color.LightGreen, 1, 1f);
            }
        }

        public override void NPCLoot()
        {
            Main.PlaySound(SoundID.DD2_WitherBeastDeath, npc.Center);
        }

        /*public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Color color26 = Color.LightGreen;
            Texture2D texture2D16 = mod.GetTexture("npcs/Melee/IorichHarvesterCrystal");

            int num154 = Main.npcTexture[npc.type].Height / Main.projFrames[npc.type];
            int y2 = num154 * npc.frame;
            Rectangle drawRectangle = new Microsoft.Xna.Framework.Rectangle(0, y2, Main.npcTexture[npc.type].Width, num154);

            Vector2 origin2 = drawRectangle.Size() / 2f;

            for (int i = 0; i < npcID.Sets.TrailCacheLength[npc.type]; i++)
            {
                Color color27 = color26;
                color27 *= (float)(npcID.Sets.TrailCacheLength[npc.type] - i) / npcID.Sets.TrailCacheLength[npc.type];
                Vector2 value4 = npc.oldPos[i];
                float num165 = npc.oldRot[i];
                Main.spriteBatch.Draw(texture2D16, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(drawRectangle), color27, num165, origin2, npc.scale, SpriteEffects.None, 0f);
            }

            return base.PreDraw(spriteBatch, lightColor);
        }*/
    }

    // This exists to prevent the player from spawning more of the above NPC
    public class IorichHarvesterCrystalProjectile : ModProjectile
    {
        public override bool CanDamage() => false;
        public override string Texture => "Terraria/Item_" + ProjectileID.LostSoulFriendly;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nature Crystal");
        }

        public override void SetDefaults()
        {
            projectile.width = 100;
            projectile.height = 168;
            projectile.friendly = true;
            projectile.timeLeft = 60 * 60;
            projectile.tileCollide = false;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            if (Main.npc[(int)projectile.ai[0]].active)
            {
                projectile.timeLeft = 5;
            }
        }
    }
}