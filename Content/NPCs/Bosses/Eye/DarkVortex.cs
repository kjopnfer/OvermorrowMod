using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.Eye
{
    public class TestEye : ModNPC
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("test");
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 32;
            NPC.lifeMax = 640;
            NPC.aiStyle = -1;
            NPC.defense = 16;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.friendly = false;
            NPC.noTileCollide = true;
        }

        public override void DrawBehind(int index)
        {
            NPC.hide = true;
            Main.instance.DrawCacheNPCProjectiles.Add(index);
        }

        public override bool CheckActive() => false;

        public ref float ParentID => ref NPC.ai[0];
        public ref float Rotation => ref NPC.ai[1];
        public override void AI()
        {
            NPC parent = Main.npc[(int)ParentID];

            Main.NewText("active");

            if (parent == null || !parent.active)
            {
                Main.NewText("nope"); 
                NPC.active = false;
            }

            //NPC.Center = Main.LocalPlayer.Center; <---- this keeps it active
            NPC.Center = ((TestThing)parent.ModNPC).LastPosition;
            //NPC.rotation = NPC.DirectionTo(parent.Center).ToRotation();
            NPC.rotation = Rotation;
            //NPC.rotation = NPC.DirectionTo(((TestThing)parent.ModNPC).LastPosition).ToRotation();
            //Main.LocalPlayer.Center = NPC.Center;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D eye = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/EyeStalk").Value;
            spriteBatch.Draw(eye, NPC.Center - Main.screenPosition, null, drawColor, NPC.rotation + MathHelper.PiOver2, eye.Size() / 2, NPC.scale, SpriteEffects.None, 0);

            return false;
        }
    }

    public class TestThing : ModNPC
    {
        private Segment tentacle;

        public Vector2 LastPosition;
        public override string Texture => AssetDirectory.Boss + "Eye/EyeOfCthulhu";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("???");
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 32;
            NPC.lifeMax = 640;
            NPC.aiStyle = -1;
            NPC.defense = 16;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.friendly = false;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
        }
    
        public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
        {
            float time = Main.rand.NextFloat(0.5f);
            float offset = Main.rand.NextFloat(0.1f, 0.15f);
            float rotation = MathHelper.ToRadians(Main.rand.Next(36) * 10);

            tentacle = new Segment(NPC.Center, 5, rotation, time);
            Segment current = tentacle;
            for (int i = 0; i < 40; i++)
            {
                time += offset;

                Segment next = new Segment(current, 5, rotation, time);
                current.Child = next;
                current = next;
            }

            // Obtain the last position for the NPC
            Segment nextSegment = tentacle;
            while (nextSegment != null)
            {
                nextSegment.Move();
                nextSegment.Update();

                LastPosition = nextSegment.EndPoint;
                nextSegment = nextSegment.Child;
            }

            Main.NewText("spawned");

            var entitySource = NPC.GetSource_FromAI();
            int index = NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<TestEye>(), 0, NPC.whoAmI, rotation);

            if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
            {
                NetMessage.SendData(MessageID.SyncNPC, number: index);
            }

            base.OnSpawn(source);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            int counter = 0;

            Segment nextSegment = tentacle;
            while (nextSegment != null)
            {
                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/StalkBodyL1").Value;
                switch (counter % 4)
                {
                    case 0:
                        texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/StalkBodyL1").Value;
                        break;
                    case 1:
                        texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/StalkBodyL2").Value;
                        break;
                    case 2:
                        texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/StalkBodyR1").Value;
                        break;
                    case 3:
                        texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/StalkBodyR2").Value;
                        break;
                }

                if (!Main.gamePaused) nextSegment.Move();

                nextSegment.Update();
                nextSegment.Draw(spriteBatch, texture);

                LastPosition = nextSegment.EndPoint;
                nextSegment = nextSegment.Child;

                counter++;
            }

            return false;
        }
    }

    public class VortexEye : ModNPC
    {
        Vector2 InitialPosition;

        public float IdleDistance;
        public float LerpTime;

        public int StalkID;
        public int ParentIndex;

        private float InitialRotation;

        public override string Texture => AssetDirectory.Boss + "Eye/EyeStalk";
        public override void SetDefaults()
        {
            NPC.width = NPC.height = 32;
            NPC.lifeMax = 640;
            NPC.aiStyle = -1;
            NPC.defense = 16;
            NPC.knockBackResist = 1.25f;
            NPC.friendly = false;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
        }

        public override void DrawBehind(int index)
        {
            // In front of all normal NPC
            NPC.hide = true;
            Main.instance.DrawCacheNPCProjectiles.Add(index);
        }

        public ref float AICase => ref NPC.ai[0];
        public ref float AICounter => ref NPC.ai[1];
        public ref float MiscCounter => ref NPC.ai[2];

        public override void AI()
        {
            NPC parent = Main.npc[ParentIndex];

            if (AICounter == 0)
            {
                InitialPosition = NPC.Center;
            }

            AICounter += 3;

            NPC.Center = InitialPosition + new Vector2(MathHelper.Lerp(-65, 65, (float)Math.Sin((AICounter) / 60f) / 2 + 0.5f), 0).RotatedBy(NPC.DirectionTo(parent.Center).ToRotation() - MathHelper.PiOver2);
            //AICounter++;

            float distance = Vector2.Distance(parent.Center, NPC.Center);

            if (Main.mouseRight) NPC.Center = Main.MouseWorld;

            //NPC.rotation = NPC.DirectionTo(Main.MouseWorld).ToRotation();

            //int dust = Dust.NewDust(NPC.Center + new Vector2(0, distance / 2f).RotatedBy(NPC.DirectionTo(parent.Center).ToRotation() - MathHelper.PiOver2), 1, 1, DustID.Adamantite);
            //Main.dust[dust].noGravity = true;

            //int dust = Dust.NewDust(parent.Center, 1, 1, DustID.Adamantite);
            //Main.dust[dust].noGravity = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float CHAIN_LENGTH = 5;
            NPC parent = Main.npc[ParentIndex];

            Vector2 StaticParentCenter = parent.Center;
            Vector2 ParentCenter = StaticParentCenter + new Vector2(MathHelper.Lerp(15, -15, (float)Math.Sin((AICounter + 30f) / 60f) / 2 + 0.5f), 0);

            float distance = Vector2.Distance(parent.Center, NPC.Center);

            Vector2 MidPoint = NPC.Center + new Vector2(MathHelper.Lerp(-60, 60, (float)Math.Sin((AICounter + 30f) / 60f) / 2 + 0.5f), distance / 2f).RotatedBy(NPC.DirectionTo(StaticParentCenter).ToRotation() - MathHelper.PiOver2);
            Vector2 StaticMidPoint = NPC.Center + new Vector2(0, distance / 2f).RotatedBy(NPC.DirectionTo(StaticParentCenter).ToRotation() - MathHelper.PiOver2);

            //int dust = Dust.NewDust(MidPoint, 1, 1, DustID.Adamantite);
            //Main.dust[dust].noGravity = true;

            var drawPosition = NPC.Center;
            var remainingVectorToParent = MidPoint - drawPosition;
            float rotation = remainingVectorToParent.ToRotation() - MathHelper.PiOver2;

            distance = Vector2.Distance(MidPoint, NPC.Center);
            float iterations = distance / CHAIN_LENGTH;

            // Draw from NPC to MidPoint
            // X and Y are reversed, increasing X increases the height for some apparent reason
            // Draw influenced by NPC rotation
            //Vector2 midPoint2 = NPC.Center + new Vector2(50, 50).RotatedBy(NPC.DirectionTo(MidPoint).ToRotation() + NPC.rotation);
            Vector2 midPoint2 = NPC.Center + new Vector2(25, MathHelper.Lerp(50, -50, (float)Math.Sin((AICounter) / 60f) / 2 + 0.5f)).RotatedBy(NPC.DirectionTo(StaticMidPoint).ToRotation());
            Vector2 midPoint1 = StaticMidPoint + new Vector2(-30, MathHelper.Lerp(60, -60, (float)Math.Sin((AICounter + 30f) / 60f) / 2 + 0.5f)).RotatedBy(NPC.DirectionTo(StaticMidPoint).ToRotation());

            NPC.rotation = NPC.DirectionTo(midPoint2).ToRotation();
            // green
            //dust = Dust.NewDust(midPoint2, 1, 1, 157);
            //Main.dust[dust].noGravity = true;

            // yellow
            //dust = Dust.NewDust(midPoint1, 1, 1, 204);
            //Main.dust[dust].noGravity = true;

            for (int i = 0; i < iterations; i++)
            {
                Texture2D chainTexture = i % 2 == 0 ? ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/StalkBody").Value : ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/StalkBodyAlt").Value;
                if (i == iterations - 5)
                {
                    chainTexture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/StalkBodyBulb").Value;
                }

                float progress = i / iterations;
                Color color = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));

                Vector2 position = ModUtils.Bezier(MidPoint, NPC.Center, midPoint1, midPoint2, progress);
                Main.EntitySpriteDraw(chainTexture, position - Main.screenPosition, null, color, rotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            }

            // Draw from MidPoint to Parent
            Vector2 midPoint4 = StaticMidPoint + new Vector2(30, MathHelper.Lerp(60, -60, (float)Math.Sin((AICounter + 45f) / 60f) / 2 + 0.5f)).RotatedBy(NPC.DirectionTo(StaticMidPoint).ToRotation());
            Vector2 midPoint3 = ParentCenter + new Vector2(-25, MathHelper.Lerp(50, -50, (float)Math.Sin((AICounter + 60f) / 60f) / 2 + 0.5f)).RotatedBy(NPC.DirectionTo(StaticMidPoint).ToRotation());

            // blue
            //dust = Dust.NewDust(midPoint3, 1, 1, 68);
            //Main.dust[dust].noGravity = true;

            // purple
            //dust = Dust.NewDust(midPoint4, 1, 1, 70);
            //Main.dust[dust].noGravity = true;

            drawPosition = MidPoint;
            remainingVectorToParent = ParentCenter - drawPosition;
            rotation = remainingVectorToParent.ToRotation() - MathHelper.PiOver2;

            distance = Vector2.Distance(ParentCenter, MidPoint);
            iterations = distance / CHAIN_LENGTH;

            for (int i = 0; i < iterations; i++)
            {
                Texture2D chainTexture = i % 2 == 0 ? ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/StalkBody").Value : ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/StalkBodyAlt").Value;
                if (i == iterations - 5)
                {
                    chainTexture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/StalkBodyBulb").Value;
                }

                float progress = i / iterations;
                Color color = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));

                Vector2 position = ModUtils.Bezier(ParentCenter, MidPoint, midPoint3, midPoint4, progress);
                Main.EntitySpriteDraw(chainTexture, position - Main.screenPosition, null, color, rotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            }

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/EyeStalk").Value;
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, null, drawColor, NPC.rotation - MathHelper.PiOver2, texture.Size() / 2, NPC.scale, SpriteEffects.None, 0);


            return false;
        }
    }

    public class DarkVortex : ModNPC
    {
        public override string Texture => AssetDirectory.Boss + "Eye/EyeStalk";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("???");
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 32;
            NPC.lifeMax = 640;
            NPC.aiStyle = -1;
            NPC.defense = 16;
            NPC.knockBackResist = 1.25f;
            NPC.noGravity = true;
            NPC.friendly = false;
            NPC.noTileCollide = true;
        }

        public ref float AICase => ref NPC.ai[0];
        public ref float AICounter => ref NPC.ai[1];
        public ref float MiscCounter => ref NPC.ai[2];

        public override void AI()
        {
            if (AICounter++ == 0)
            {
                var entitySource = NPC.GetSource_FromAI();
                int index = NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y - 200, ModContent.NPCType<VortexEye>(), NPC.whoAmI);
                NPC minionNPC = Main.npc[index];
                if (minionNPC.ModNPC is VortexEye minion)
                {
                    minion.ParentIndex = NPC.whoAmI;
                }

                if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: index);
                }
            }

            base.AI();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/VortexShadow").Value;

            float mult = (0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.1f);
            float scale = NPC.scale * 1.55f * mult;
            Color color = Color.White;

            //Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition, null, color, NPC.rotation, texture.Size() / 2, scale, SpriteEffects.None, 0);

            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            return false;
        }
    }

    /*public class DarkVortex : ModProjectile
    {
        public override bool? CanDamage() => true;
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Vortex");
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 720;
            Projectile.penetrate = -1;
            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override void AI()
        {
            if (Projectile.timeLeft <= 180)
            {
                Projectile.scale = MathHelper.Lerp(0, 1, MathHelper.Clamp(Projectile.timeLeft, 0, 180) / 180f);
            }
            else
            {
                Projectile.scale = MathHelper.Lerp(0, 1, MathHelper.Clamp(Projectile.ai[0]++, 0, 180) / 180f);
            }

            Projectile.rotation += 0.06f;
        }

        public static void DrawAll(SpriteBatch spriteBatch)
        {
            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.type == ModContent.ProjectileType<DarkVortex>() && projectile.active)
                {
                    Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/DarkVortex").Value;

                    float mult = (0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.1f);
                    float scale = projectile.scale * 1.55f * mult;
                    Color color = Color.White;

                    spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, color, projectile.rotation, texture.Size() / 2, scale, SpriteEffects.None, 0);

                    texture = ModContent.Request<Texture2D>("OvermorrowMod/Content/NPCs/Bosses/SandstormBoss/VortexCenter").Value;
                    scale = projectile.scale * 1.55f * mult;
                    spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, color, projectile.rotation, texture.Size() / 2, scale, SpriteEffects.None, 0);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/VortexShadow").Value;

            float mult = (0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.1f);
            float scale = Projectile.scale * 1.55f * mult;
            Color color = Color.White;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, texture.Size() / 2, scale, SpriteEffects.None, 0);

            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            return false;
        }
    }*/
}
