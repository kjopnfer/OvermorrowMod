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
    public class VortexEye : ModNPC
    {
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

            NPC.Center = Main.MouseWorld;
            float distance = Vector2.Distance(parent.Center, NPC.Center);

            int dust = Dust.NewDust(NPC.Center + new Vector2(0, distance / 2f).RotatedBy(NPC.DirectionTo(parent.Center).ToRotation() - MathHelper.PiOver2), 1, 1, DustID.Adamantite);
            Main.dust[dust].noGravity = true;

            dust = Dust.NewDust(parent.Center, 1, 1, DustID.Adamantite);
            Main.dust[dust].noGravity = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float CHAIN_LENGTH = 5;
            NPC parent = Main.npc[ParentIndex];
            
            Vector2 mountedCenter = parent.Center;
            float distance = Vector2.Distance(parent.Center, NPC.Center);

            Vector2 MidPoint = NPC.Center + new Vector2(0, distance / 2f).RotatedBy(NPC.DirectionTo(mountedCenter).ToRotation() - MathHelper.PiOver2);

            var drawPosition = NPC.Center;
            var remainingVectorToParent = MidPoint - drawPosition;
            float rotation = remainingVectorToParent.ToRotation() - MathHelper.PiOver2;

            distance = Vector2.Distance(MidPoint, NPC.Center);
            float iterations = distance / CHAIN_LENGTH;

            // Draw from NPC to MidPoint
            // X and Y are reversed, increasing X increases the height for some apparent reason
            Vector2 midPoint1 = MidPoint + new Vector2(-50, -50).RotatedBy(NPC.DirectionTo(MidPoint).ToRotation());
            Vector2 midPoint2 = NPC.Center + new Vector2(50, 50).RotatedBy(NPC.DirectionTo(MidPoint).ToRotation());

            int dust = Dust.NewDust(midPoint1, 1, 1, DustID.Orichalcum);
            Main.dust[dust].noGravity = true;

            dust = Dust.NewDust(midPoint2, 1, 1, DustID.Orichalcum);
            Main.dust[dust].noGravity = true;

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
            Vector2 midPoint3 = mountedCenter + new Vector2(-50, -50).RotatedBy(NPC.DirectionTo(MidPoint).ToRotation());
            Vector2 midPoint4 = MidPoint + new Vector2(50, 50).RotatedBy(NPC.DirectionTo(MidPoint).ToRotation());

            dust = Dust.NewDust(midPoint3, 1, 1, DustID.Palladium);
            Main.dust[dust].noGravity = true;

            dust = Dust.NewDust(midPoint4, 1, 1, DustID.Palladium);
            Main.dust[dust].noGravity = true;

            drawPosition = MidPoint;
            remainingVectorToParent = mountedCenter - drawPosition;
            rotation = remainingVectorToParent.ToRotation() - MathHelper.PiOver2;

            distance = Vector2.Distance(mountedCenter, MidPoint);
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

                Vector2 position = ModUtils.Bezier(mountedCenter, MidPoint, midPoint3, midPoint4, progress);
                Main.EntitySpriteDraw(chainTexture, position - Main.screenPosition, null, color, rotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            }

            return base.PreDraw(spriteBatch, screenPos, drawColor);
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
