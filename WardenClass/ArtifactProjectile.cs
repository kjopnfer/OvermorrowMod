using Microsoft.Xna.Framework;
using OvermorrowMod.Buffs;
using OvermorrowMod.Projectiles.Artifact;
using OvermorrowMod.Projectiles.Magic;
using OvermorrowMod.Projectiles.Piercing;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.WardenClass
{
    public abstract class ArtifactProjectile : ModProjectile
    {
        public WardenRunePlayer.Runes RuneID;
        public int AuraRadius = 0;
        private bool isActive = false;
        private bool PillarLoop = false;
        public virtual void SafeSetDefaults()
        {

        }

        public sealed override void SetDefaults()
        {
            SafeSetDefaults();
            projectile.melee = false;
            projectile.ranged = false;
            projectile.magic = false;
            projectile.thrown = false;
            projectile.minion = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(AuraRadius);
            writer.Write((byte)RuneID);
            writer.Write((bool)PillarLoop);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            AuraRadius = reader.ReadInt32();
            RuneID = (WardenRunePlayer.Runes)reader.ReadByte();
            PillarLoop = reader.ReadBoolean();
        }

        // Default AI will be for Support Artifacts, if its an Attack Artifact this will naturally be overrided
        private int DustType;
        private float DustScale;
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (player.dead || !player.active)
            {
                return;
            }

            // Whatever spawning shenanigans
            if (projectile.type == ModContent.ProjectileType<WorldTree>() || projectile.type == ModContent.ProjectileType<Pillar>())
            {
                // Get the ground beneath the projectile
                Vector2 projectilePos = new Vector2(projectile.position.X / 16, (projectile.position.Y + projectile.height) / 16);
                Tile tile = Framing.GetTileSafely((int)projectilePos.X, (int)projectilePos.Y);
                while (!tile.active() || tile.type == TileID.Trees)
                {
                    projectilePos.Y += 1;
                    tile = Framing.GetTileSafely((int)projectilePos.X, (int)projectilePos.Y);
                }

                projectile.position = projectilePos * 16 - new Vector2(0, projectile.height);
            }

            if (projectile.type == ModContent.ProjectileType<RedCloud>())
            {
                Lighting.AddLight(projectile.Center, 1.2f, 0f, 0f);
            }
            else if (projectile.type == ModContent.ProjectileType<WorldTree>())
            {
                Lighting.AddLight(projectile.Center, 0f, 0.66f, 0f);
            }

            #region Aura
            // Generate the Aura
            if (projectile.type == ModContent.ProjectileType<RedCloud>() || projectile.type == ModContent.ProjectileType<WorldTree>() || projectile.type == ModContent.ProjectileType<Pillar>())
            {
                projectile.ai[0]++;

                if (projectile.ai[1] < AuraRadius) // The radius
                {
                    projectile.ai[1] += 15;
                }
                else
                {
                    isActive = true;
                }

                if (projectile.type == ModContent.ProjectileType<RedCloud>())
                {
                    DustType = 60;
                    DustScale = 2.04f;
                }
                else if (projectile.type == ModContent.ProjectileType<WorldTree>())
                {
                    DustType = 107;
                    DustScale = 1f;
                }
                                
                if (projectile.type == ModContent.ProjectileType<Pillar>())
                {
                    for (int i = 0; i < 18; i++)
                    {
                        Vector2 dustPos = projectile.Center + new Vector2(projectile.ai[1], 0).RotatedBy(MathHelper.ToRadians(i * 20 + projectile.ai[0]));
                        Dust dust = Dust.NewDustPerfect(dustPos, 64, Vector2.Zero, 0, new Color(255, 255, 255), 2.04f);
                        dust.noGravity = true;
                    }
                }
                else
                {
                    for (int i = 0; i < 36; i++)
                    {
                        Vector2 dustPos = (projectile.Center - new Vector2(0, 68)) + new Vector2(projectile.ai[1], 0).RotatedBy(MathHelper.ToRadians(i * 10 + projectile.ai[0]));
                        Dust dust = Main.dust[Terraria.Dust.NewDust(dustPos, 15, 15, DustType, 0f, 0f, 0, default, DustScale)];
                        dust.noGravity = true;
                    }
                }

                // Apply buffs
                if (isActive)
                {
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        float distance = Vector2.Distance((projectile.Center - new Vector2(0, 68)), Main.player[i].Center);
                        if (distance <= AuraRadius)
                        {
                            if (projectile.type == ModContent.ProjectileType<WorldTree>())
                            {
                                Main.player[i].AddBuff(ModContent.BuffType<TreeBuff>(), 60);
                            }

                            if (projectile.type == ModContent.ProjectileType<RedCloud>())
                            {
                                Main.player[i].AddBuff(ModContent.BuffType<MoonBuff>(), 60);
                            }

                            if (projectile.type == ModContent.ProjectileType<Pillar>())
                            {
                                Main.player[i].AddBuff(ModContent.BuffType<PillarBuff>(), 60);
                            }

                            if (RuneID == WardenRunePlayer.Runes.SkyRune)
                            {
                                Main.player[i].AddBuff(ModContent.BuffType<GoldWind>(), 60);
                            }
                        }
                    }

                    // Do stuff against enemy NPCs
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        // Make it so it doesn't affect friendly NPCs
                        if (!Main.npc[i].friendly)
                        {
                            float distance = Vector2.Distance((projectile.Center - new Vector2(0, 68)), Main.npc[i].Center);
                            if (distance <= AuraRadius)
                            {
                                if (RuneID == WardenRunePlayer.Runes.CorruptionRune)
                                {
                                    Main.npc[i].AddBuff(BuffID.CursedInferno, 120);
                                }

                                if (RuneID == WardenRunePlayer.Runes.CrimsonRune)
                                {
                                    if (projectile.ai[0] % 180 == 0 && Main.npc[i].active)
                                    {
                                        Vector2 origin = Main.npc[i].Center;
                                        float radius = 15;
                                        int numLocations = 30;
                                        for (int j = 0; j < 30; j++)
                                        {
                                            Vector2 dustPosition = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * j)) * radius;
                                            Vector2 dustvelocity = new Vector2(0f, 10f).RotatedBy(MathHelper.ToRadians(360f / numLocations * j));
                                            int dust = Dust.NewDust(dustPosition, 2, 2, 90, dustvelocity.X, dustvelocity.Y, 0, default, 1.25f);
                                            Main.dust[dust].noGravity = true;
                                        }

                                        int randRotation = Main.rand.Next(24) * 15; // Uhhh, random degrees in increments of 15
                                        for (int j = 0; j < 6; j++)
                                        {
                                            Projectile.NewProjectile(Main.npc[i].Center, new Vector2(6).RotatedBy(MathHelper.ToRadians((360 / 6) * j + randRotation)), ModContent.ProjectileType<RedThornHead>(), 23, 6f, projectile.owner);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region Animation
            if (projectile.type == ModContent.ProjectileType<WorldTree>())
            {
                if (++projectile.frameCounter >= 4)
                {
                    projectile.frameCounter = 0;
                    if (++projectile.frame >= Main.projFrames[projectile.type])
                    {
                        projectile.frame = 0;
                    }
                }
            }

            if (projectile.type == ModContent.ProjectileType<RedCloud>())
            {
                // Loop through the 10 animation frames, spending 12 ticks on each.
                if (++projectile.frameCounter >= 12)
                {
                    projectile.frameCounter = 0;
                    if (++projectile.frame >= Main.projFrames[projectile.type])
                    {
                        projectile.frame = 0;
                    }
                }
            }

            if (projectile.type == ModContent.ProjectileType<Pillar>())
            {
                // Loop through the first 10 animation frames, spending 5 ticks on each.
                if (!PillarLoop)
                {
                    if (++projectile.frameCounter >= 5)
                    {
                        projectile.frameCounter = 0;
                        if (++projectile.frame >= 10)
                        {
                            PillarLoop = true;
                            projectile.netUpdate = true;
                        }
                    }
                }
                else
                {
                    if (++projectile.frameCounter >= 5)
                    {
                        projectile.frameCounter = 0;
                        if (++projectile.frame >= 19)
                        {
                            PillarLoop = true;
                            projectile.frame = 10;
                        }
                    }
                }
            }
            #endregion
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int randNum = Main.rand.Next(3, 5);
            projectile.netUpdate = true;

            switch (RuneID)
            {
                case WardenRunePlayer.Runes.CorruptionRune:
                    target.AddBuff(BuffID.CursedInferno, 480);
                    break;
                case WardenRunePlayer.Runes.CrimsonRune:
                    target.AddBuff(BuffID.Ichor, 480);
                    break;
                case WardenRunePlayer.Runes.JungleRune:
                    for (int i = 0; i < randNum; i++)
                    {
                        if (Main.rand.Next(3) == 0)
                        {
                            Projectile.NewProjectile(projectile.Center, Vector2.One.RotatedByRandom(Math.PI) * 4, ModContent.ProjectileType<Spores>(), damage, 3f, projectile.owner);
                        }
                    }
                    break;
                case WardenRunePlayer.Runes.MushroomRune:
                    for (int i = 0; i < randNum; i++)
                    {
                        if (Main.rand.Next(3) == 0)
                        {
                            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, Main.rand.Next(-3, 3), Main.rand.Next(-5, -3), ModContent.ProjectileType<FungiSpore2>(), damage, 3f, projectile.owner);
                        }
                    }
                    break;
                case WardenRunePlayer.Runes.HellRune:
                    if (Main.rand.Next(2) == 0)
                    {
                        Vector2 randPosition = new Vector2(target.Center.X - Main.rand.Next(-200, 200), target.Center.Y + 800);
                        Vector2 moveTo = target.Center - randPosition;
                        float magnitude = (float)Math.Sqrt(moveTo.X * moveTo.X + moveTo.Y * moveTo.Y);
                        Projectile.NewProjectile(randPosition, moveTo / magnitude, ModContent.ProjectileType<DemonClaw>(), 36, 6f, projectile.owner);
                    }
                    break;
            }
        }
    }
}