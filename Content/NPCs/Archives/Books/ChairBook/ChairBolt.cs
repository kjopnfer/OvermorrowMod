using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.NPCs.Archives;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs
{
    public class ChairBolt : ModProjectile, ITrailEntity
    {
        public IEnumerable<TrailConfig> TrailConfigurations()
        {
            return new List<TrailConfig>
            {
                new TrailConfig(
                    typeof(LaserTrail),
                    progress => Color.Lerp(Color.Purple, Color.Orange, progress) * MathHelper.SmoothStep(0, 1, progress),
                    progress => MathHelper.Lerp(30, 31, progress)
                ),
                new TrailConfig(
                    typeof(LaserTrail),
                    progress => DrawUtils.ColorLerp3(Color.HotPink, Color.HotPink, Color.Orange, progress) * 0.5f *  MathHelper.SmoothStep(0, 1, progress),
                    progress => MathHelper.Lerp(50, 61, progress)
                )
            };
        }

        public override string Texture => AssetDirectory.Empty;
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            //Projectile.hostile = true;
            Projectile.timeLeft = 300;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(randomDirection);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            randomDirection = reader.ReadInt32();
        }

        int randomDirection;
        public override void OnSpawn(IEntitySource source)
        {
            randomDirection = Main.rand.NextBool() ? 1 : -1;
            Projectile.tileCollide = false;
            Projectile.netUpdate = true;
        }

        public ref float ParentID => ref Projectile.ai[1];
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.5f, 0.5f, 0.5f));

            NPC parentNPC = Main.npc[(int)ParentID];
            if (!parentNPC.active) Projectile.Kill();

            if (Projectile.ai[0]++ < 60)
            {
                Projectile.velocity.Y -= 0.25f;
                Projectile.velocity.X *= 0.99f;
            }
            else if (Projectile.ai[0] > 75)
            {
                // Tries to avoid hitting the ceiling
                if (Projectile.Center.Y > parentNPC.Center.Y) Projectile.tileCollide = true;
                Projectile.velocity.Y += 0.75f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            int npcType = Main.rand.NextBool() ? ModContent.NPCType<AnimatedChair>() : ModContent.NPCType<AnimatedSofa>();
            int randomDirection = Main.rand.NextBool() ? -1 : 1;
            Projectile.netUpdate = true;

            Archives.ChairSummon npc = NPC.NewNPCDirect(Projectile.GetSource_FromAI(), (int)Projectile.Center.X, (int)Projectile.Center.Y, npcType).ModNPC as Archives.ChairSummon;
            npc.ParentID = ParentID;
            npc.NPC.direction = randomDirection;

            float baseSpeed = Main.rand.NextFloat(1f, 2f); // Base speed of the particles

            for (int repeat = 0; repeat < Main.rand.Next(3, 5); repeat++)
            {
                int numParticles = Main.rand.Next(8, 16); // Number of particles to spawn
                for (int i = 0; i < numParticles; i++)
                {
                    Color color = Color.Lerp(Color.Orange, Color.HotPink, Main.rand.NextFloat(0, 1f));

                    float angle = MathHelper.TwoPi / numParticles * i;
                    float scale = Main.rand.NextFloat(0.1f, 0.5f);

                    // Adjust the velocity to create a horizontal oval shape
                    Vector2 velocity = new Vector2((float)Math.Cos(angle) * 1.5f, (float)Math.Sin(angle)) * baseSpeed;

                    // Add a small random offset to the center
                    Vector2 offset = new Vector2(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f));

                    Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.StardustTowerMark).Value;
                    var lightOrb = new Circle(texture, 0f);
                    ParticleManager.CreateParticleDirect(lightOrb, Projectile.Bottom + offset, velocity, color, 1f, scale * 0.5f, 0f);

                }
            }
            return base.OnTileCollide(oldVelocity);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float particleScale = 0.05f;

            if (!Main.gamePaused)
            {
                if (Projectile.localAI[0]++ % Main.rand.Next(2, 6) == 0)
                {
                    int randomIterations = Main.rand.Next(2, 5);
                    Vector2 drawOffset = new Vector2(-4, -4).RotatedBy(Projectile.rotation);
                    Color color = Color.Lerp(Color.Purple, Color.DarkOrange, Main.rand.NextFloat(0, 1f));

                    Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.StardustTowerMark).Value;

                    for (int i = 0; i < randomIterations; i++)
                    {
                        var emberParticle = new Circle(texture, 0f, useSineFade: true); // Default max time, custom scale, sine fade
                        ParticleManager.CreateParticleDirect(emberParticle, Projectile.Center, -Projectile.velocity.RotatedByRandom(MathHelper.PiOver4) * 0.1f, color, 1f, particleScale, 0f);
                    }
                }
            }

            return false;
        }
    }
}