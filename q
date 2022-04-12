warning: LF will be replaced by CRLF in Content/NPCs/Bosses/SandstormBoss/DharuudArena.cs.
The file will have its original line endings in your working directory
[1mdiff --git a/Content/NPCs/Bosses/SandstormBoss/DharuudArena.cs b/Content/NPCs/Bosses/SandstormBoss/DharuudArena.cs[m
[1mindex 44315de..3bf7ce8 100644[m
[1m--- a/Content/NPCs/Bosses/SandstormBoss/DharuudArena.cs[m
[1m+++ b/Content/NPCs/Bosses/SandstormBoss/DharuudArena.cs[m
[36m@@ -32,15 +32,16 @@[m [mnamespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss[m
             projectile.hide = true;[m
             projectile.alpha = 255;[m
         }[m
[31m-[m
[32m+[m[32m        public ref float AICounter => ref projectile.ai[0];[m
         public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)[m
         {[m
[32m+[m[32m            drawCacheProjsBehindNPCs.Add(index);[m
             drawCacheProjsBehindProjectiles.Add(index);[m
         }[m
 [m
         public override void AI()[m
         {[m
[31m-            if (projectile.ai[0]++ == 0)[m
[32m+[m[32m            if (AICounter++ == 0)[m
             {[m
                 if (!Sandstorm.Happening)[m
                 {[m
[36m@@ -79,7 +80,7 @@[m [mnamespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss[m
                 //Main.projectile[proj].hostile = false;[m
             }[m
 [m
[31m-            if (projectile.ai[0] > 120)[m
[32m+[m[32m            if (AICounter > 120)[m
             {[m
                 if (projectile.ai[1]++ % 10 == 0 && BarrierCounter < 8)[m
                 {[m
[36m@@ -104,7 +105,7 @@[m [mnamespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss[m
                 }[m
             }[m
 [m
[31m-            if (projectile.ai[0] == 360)[m
[32m+[m[32m            if (AICounter == 360)[m
             {[m
                 //Player player = Main.player[projectile.owner];[m
                 //player.GetModPlayer<OvermorrowModPlayer>().PlayerFocusCamera(projectile.Center, 90, 120f, 60f);[m
[36m@@ -139,9 +140,9 @@[m [mnamespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss[m
                 }[m
             }[m
 [m
[31m-            if (projectile.ai[0] > 360)[m
[32m+[m[32m            if (AICounter > 360)[m
             {[m
[31m-                if (projectile.ai[0] == 460)[m
[32m+[m[32m                if (AICounter == 460)[m
                 {[m
                     //DisablePlatforms();[m
 [m
[36m@@ -198,11 +199,11 @@[m [mnamespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss[m
             Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, color, MathHelper.ToRadians(projectile.localAI[0]), origin, scale, SpriteEffects.None, 0f);[m
 [m
 [m
[31m-            if (projectile.ai[0] > 460)[m
[32m+[m[32m            if (AICounter > 460)[m
             {[m
                 texture = ModContent.GetTexture(AssetDirectory.Textures + "PulseCircle");[m
                 color = Color.Orange;[m
[31m-                float circleScale = MathHelper.Lerp(0, 2.5f, Utils.Clamp(projectile.ai[0] - 360, 0, 100) / 100f);[m
[32m+[m[32m                float circleScale = MathHelper.Lerp(0, 2.5f, Utils.Clamp(AICounter - 360, 0, 100) / 100f);[m
 [m
                 if (!Main.gamePaused) projectile.localAI[0]++;[m
 [m
[1mdiff --git a/Content/NPCs/Bosses/SandstormBoss/Fragment.cs b/Content/NPCs/Bosses/SandstormBoss/Fragment.cs[m
[1mindex d79a8c9..511b3fd 100644[m
[1m--- a/Content/NPCs/Bosses/SandstormBoss/Fragment.cs[m
[1m+++ b/Content/NPCs/Bosses/SandstormBoss/Fragment.cs[m
[36m@@ -136,7 +136,7 @@[m [mnamespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss[m
     public class HorizontalFragment : ModProjectile, ITrailEntity[m
     {[m
         public Color TrailColor(float progress) => Color.Lerp(new Color(253, 254, 255), new Color(244, 188, 91), progress) * progress;[m
[31m-        public float TrailSize(float progress) => 48;[m
[32m+[m[32m        public float TrailSize(float progress) => 12;[m
         public Type TrailType() => typeof(TorchTrail);[m
 [m
         public override string Texture => "OvermorrowMod/Content/NPCs/Bosses/SandstormBoss/Fragment";[m
[1mdiff --git a/Content/NPCs/Bosses/SandstormBoss/LightBullet.cs b/Content/NPCs/Bosses/SandstormBoss/LightBullet.cs[m
[1mdeleted file mode 100644[m
[1mindex 1746858..0000000[m
[1m--- a/Content/NPCs/Bosses/SandstormBoss/LightBullet.cs[m
[1m+++ /dev/null[m
[36m@@ -1,99 +0,0 @@[m
[31m-using Microsoft.Xna.Framework;[m
[31m-using Microsoft.Xna.Framework.Graphics;[m
[31m-using OvermorrowMod.Common.Particles;[m
[31m-using Terraria;[m
[31m-using Terraria.ID;[m
[31m-using Terraria.ModLoader;[m
[31m-[m
[31m-namespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss[m
[31m-{[m
[31m-    public class LightBullet : ModNPC[m
[31m-    {[m
[31m-        private bool RunOnce = true;[m
[31m-        private NPC ParentNPC;[m
[31m-        public override string Texture => "Terraria/Projectile_" + ProjectileID.LostSoulFriendly;[m
[31m-        public override void SetStaticDefaults()[m
[31m-        {[m
[31m-            DisplayName.SetDefault("Light Warping Bullets");[m
[31m-        }[m
[31m-[m
[31m-        public override void SetDefaults()[m
[31m-        {[m
[31m-            npc.width = npc.height = 16;[m
[31m-            npc.friendly = false;[m
[31m-            npc.timeLeft = 5;[m
[31m-            npc.aiStyle = -1;[m
[31m-            npc.noTileCollide = true;[m
[31m-        }[m
[31m-[m
[31m-        private ref float Direction => ref npc.ai[0];[m
[31m-        private ref float InitialRotation => ref npc.ai[1];[m
[31m-        private ref float MiscCounter => ref npc.ai[2];[m
[31m-        private ref float Angle => ref npc.ai[3];[m
[31m-[m
[31m-        public override void AI()[m
[31m-        {[m
[31m-            if (RunOnce)[m
[31m-            {[m
[31m-                ParentNPC = Main.npc[(int)npc.ai[0]];[m
[31m-[m
[31m-                int RotationDirection = Main.rand.NextBool() ? 1 : -1;[m
[31m-                int RotationAngle = Main.rand.Next(6, 10);[m
[31m-[m
[31m-                npc.ai[0] = RotationDirection;[m
[31m-                npc.ai[3] = RotationAngle;[m
[31m-[m
[31m-[m
[31m-                RunOnce = false;[m
[31m-            }[m
[31m-[m
[31m-            if (ParentNPC.active)[m
[31m-            {[m
[31m-                npc.timeLeft = 5;[m
[31m-            }[m
[31m-[m
[31m-            if (MiscCounter++ > 240)[m
[31m-            {[m
[31m-                Main.NewText("run");[m
[31m-[m
[31m-                for (int i = 0; i < Main.maxPlayers; i++)[m
[31m-                {[m
[31m-                    Player player = Main.player[i];[m
[31m-                    if (player.active && npc.Distance(player.Center) < 2000)[m
[31m-                    {[m
[31m-                        Vector2 Target = npc.Center - player.Center;[m
[31m-                        npc.velocity = Vector2.Lerp(npc.velocity, Target.SafeNormalize(Vector2.UnitX) * -6, 0.1f);[m
[31m-                        npc.velocity = npc.velocity.RotatedBy(MathHelper.ToRadians(Angle * Direction));[m
[31m-[m
[31m-                        break;[m
[31m-                    }[m
[31m-                }[m
[31m-            }[m
[31m-            else[m
[31m-            {[m
[31m-                npc.Center = ParentNPC.Center + new Vector2(206, 0).RotatedBy(MathHelper.ToRadians(InitialRotation + ParentNPC.ai[1]));[m
[31m-            }[m
[31m-[m
[31m-            Vector2 RandomDirection = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 2;[m
[31m-            Particle.CreateParticle(Particle.ParticleType<Orb>(), npc.Center, RandomDirection, Color.Orange, 1, Main.rand.NextFloat(0.25f, 0.4f), 0, 25);[m
[31m-[m
[31m-        }[m
[31m-[m
[31m-        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)[m
[31m-        {[m
[31m-            Main.spriteBatch.End();[m
[31m-            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);[m
[31m-[m
[31m-            Texture2D texture = ModContent.GetTexture("OvermorrowMod/Content/NPCs/Bosses/SandstormBoss/PillarSpawner");[m
[31m-            //float mult = (0.55f + (float)Math.Sin(Main.GlobalTime) * 0.1f);[m
[31m-            //float scale = npc.scale * 2 * mult;[m
[31m-[m
[31m-            Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, Color.Gold, 0, new Vector2(texture.Width, texture.Height) / 2, 1, SpriteEffects.None, 0f);[m
[31m-            Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, Color.White, 0, new Vector2(texture.Width, texture.Height) / 2, 0.5f, SpriteEffects.None, 0f);[m
[31m-[m
[31m-            Main.spriteBatch.End();[m
[31m-            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);[m
[31m-        }[m
[31m-    }[m
[31m-[m
[31m-}[m
\ No newline at end of file[m
[1mdiff --git a/Content/NPCs/Bosses/SandstormBoss/Ruins.cs b/Content/NPCs/Bosses/SandstormBoss/Ruins.cs[m
[1mindex 4d3805d..67132a0 100644[m
[1m--- a/Content/NPCs/Bosses/SandstormBoss/Ruins.cs[m
[1m+++ b/Content/NPCs/Bosses/SandstormBoss/Ruins.cs[m
[36m@@ -23,7 +23,7 @@[m [mnamespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss[m
         public override bool CanHitPlayer(Player target, ref int cooldownSlot) => CanFall;[m
         public override void SetStaticDefaults()[m
         {[m
[31m-            DisplayName.SetDefault("Buried Ruin");[m
[32m+[m[32m            DisplayName.SetDefault("");[m
         }[m
 [m
         public override void SetDefaults()[m
[36m@@ -72,22 +72,6 @@[m [mnamespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss[m
                 InitialPosition = npc.Center;[m
             }[m
 [m
[31m-            /*if (!Collision.CanHit(npc.Center, npc.width, npc.height, npc.Center + Vector2.UnitY, 2, 2))[m
[31m-            {[m
[31m-                if (Main.rand.NextBool(3))[m
[31m-                {[m
[31m-                    Tile tile = Framing.GetTileSafely((int)(SpawnPosition.X + (Main.rand.Next(-2, 2) * 15)) / 16, (int)SpawnPosition.Y / 16);[m
[31m-[m
[31m-                    while (!tile.active() || tile.collisionType != 1)[m
[31m-                    {[m
[31m-                        SpawnPosition.Y += 1;[m
[31m-                        tile = Framing.GetTileSafely((int)npc.Center.X / 16, (int)npc.Center.Y / 16);[m
[31m-                    }[m
[31m-[m
[31m-                    Particle.CreateParticle(Particle.ParticleType<Smoke2>(), SpawnPosition * 16, Vector2.One.RotatedByRandom(MathHelper.TwoPi), new Color(182, 128, 70));[m
[31m-                }[m
[31m-            }*/[m
[31m-[m
             if (!CanFall)[m
             {[m
                 if (AICounter++ <= 1280)[m
[36m@@ -141,7 +125,7 @@[m [mnamespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss[m
                     {[m
                         Vector2 RandomPosition = npc.Center + new Vector2(Main.rand.Next(-10, 10), 5);[m
                         Vector2 RandomVelocity = -Vector2.One.RotatedByRandom(MathHelper.Pi) * Main.rand.Next(1, 3);[m
[31m-                        Particle.CreateParticle(Particle.ParticleType<Smoke2>(), RandomPosition, RandomVelocity, new Color(182, 128, 70), Main.rand.NextFloat(0.25f, 0.55f));[m
[32m+[m[32m                        Particle.CreateParticle(Particle.ParticleType<Smoke2>(), RandomPosition, RandomVelocity, new Color(182, 128, 70), Main.rand.NextFloat(0.15f, 0.35f));[m
                     }[m
 [m
                     npc.life = 0;[m
[1mdiff --git a/Content/NPCs/Bosses/SandstormBoss/SandstormBoss.cs b/Content/NPCs/Bosses/SandstormBoss/SandstormBoss.cs[m
[1mindex ced05df..577d756 100644[m
[1m--- a/Content/NPCs/Bosses/SandstormBoss/SandstormBoss.cs[m
[1m+++ b/Content/NPCs/Bosses/SandstormBoss/SandstormBoss.cs[m
[36m@@ -78,7 +78,7 @@[m [mnamespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss[m
             PhaseTransition = -2,[m
             Intro = -1,[m
             Selector = 0,[m
[31m-            //Shards = 1,[m
[32m+[m[32m            Orbs = 1,[m
             Ruins = 2,[m
             Vortex = 3,[m
             Shards = 4,[m
[36m@@ -164,6 +164,10 @@[m [mnamespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss[m
                         MiscCounter = 0;[m
                     }[m
 [m
[32m+[m[32m                    break;[m
[32m+[m[32m                case (int)AIStates.Orbs:[m
[32m+[m
[32m+[m
                     break;[m
                 case (int)AIStates.Ruins:[m
                     if (MiscCounter++ % 15 == 0 && MiscCounter < 280)[m
[36m@@ -182,6 +186,23 @@[m [mnamespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss[m
                         }[m
                     }[m
 [m
[32m+[m[32m                    if (MiscCounter % 90 == 0)[m
[32m+[m[32m                    {[m
[32m+[m[32m                        for (int i = 0; i < 8; i++)[m
[32m+[m[32m                        {[m
[32m+[m[32m                            Vector2 SpawnPosition = npc.Center + new Vector2(28, 0).RotatedBy(MathHelper.ToRadians(360 / 8 * i));[m
[32m+[m
[32m+[m
[32m+[m[32m                            Vector2 RandomVelocity = Vector2.Normalize(npc.DirectionTo(player.Center)).RotatedBy(MathHelper.ToRadians(Main.rand.Next(45, 90))) * Main.rand.Next(3, 5);[m
[32m+[m[32m                            Projectile.NewProjectile(SpawnPosition, RandomVelocity, ModContent.ProjectileType<LightStar>(), 30, 6f, Main.myPlayer, 0, -1);[m
[32m+[m
[32m+[m[32m                            RandomVelocity = Vector2.Normalize(npc.DirectionTo(player.Center)).RotatedBy(MathHelper.ToRadians(-Main.rand.Next(45, 90))) * Main.rand.Next(3, 5);[m
[32m+[m[32m                            Projectile.NewProjectile(SpawnPosition, RandomVelocity, ModContent.ProjectileType<LightStar>(), 30, 6f, Main.myPlayer, 0, 1);[m
[32m+[m[32m                            //NPC.NewNPC((int)SpawnPosition.X, (int)SpawnPosition.Y, ModContent.NPCType<LightBullet>(), 0, npc.whoAmI, 360 / 8 * i);[m
[32m+[m
[32m+[m[32m                        }[m
[32m+[m[32m                    }[m
[32m+[m
                     if (MiscCounter == 860)[m
                     {[m
                         Main.NewText("DROP");[m
[36m@@ -235,7 +256,7 @@[m [mnamespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss[m
 [m
                         MiscCounter = 0;[m
                     }[m
[31m-                    break;          [m
[32m+[m[32m                    break;[m
                 case (int)AIStates.Shards:[m
                     if (MiscCounter == 0)[m
                     {[m
[36m@@ -487,15 +508,6 @@[m [mnamespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss[m
             //    Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, Color.Yellow, MathHelper.ToRadians(npc.localAI[0] += 0.5f) + MathHelper.ToRadians(i * (360 / 9)), new Vector2(texture.Width / 2, texture.Height) / 2, new Vector2(3f, 1f), SpriteEffects.None, 0f);[m
             //}[m
 [m
[31m-            if (npc.localAI[0]++ == 0)[m
[31m-            {[m
[31m-                for (int i = 0; i < 8; i++)[m
[31m-                {[m
[31m-                    Vector2 SpawnPosition = npc.Center + new Vector2(206, 0).RotatedBy(MathHelper.ToRadians(360 / 8 * i));[m
[31m-                    NPC.NewNPC((int)SpawnPosition.X, (int)SpawnPosition.Y, ModContent.NPCType<LightBullet>(), 0, npc.whoAmI, 360 / 8 * i);[m
[31m-                }[m
[31m-            }[m
[31m-[m
             // Main.windSpeedSet lets the wind speed gradually increase[m
             // Main.windSpeed is instantaneous[m
             // windSpeed affects how strong the sandstorm will push the player[m
