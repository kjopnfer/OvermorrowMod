/*using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs
{
    public class ChainBall : ModProjectile
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + "WaxheadFlail";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = ModUtils.SecondsToTicks(300);
            Projectile.penetrate = -1;
        }

        public int ParentID
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public ref float AICounter => ref Projectile.ai[1];

        public override void OnSpawn(IEntitySource source)
        {
            NPC npc = Main.npc[ParentID];
            if (!npc.active)
            {
                Projectile.Kill();
            }
        }

        public override void AI()
        {
            NPC npc = Main.npc[ParentID];
            if (!npc.active)
                Projectile.Kill();

            Projectile.timeLeft = 5;

            // Ball stays at the hand position
            if (npc.ModNPC is ChainArm arm)
            {
                Projectile.Center = arm.GetHandPosition();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            NPC npc = Main.npc[ParentID];
            if (npc.active && npc.ModNPC is ChainArm arm)
            {
                // Draw chain from shoulder to ball
                DrawChain(arm.GetShoulderPosition(), Projectile.Center);
            }

            // Draw the ball
            Texture2D ballTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "WaxheadFlail").Value;
            Vector2 ballScreenPos = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(ballTexture, ballScreenPos, null, Color.White, 0f, ballTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);

            return false;
        }

        private void DrawChain(Vector2 startPoint, Vector2 endPoint)
        {
            Texture2D chainTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "WaxheadChain").Value;

            Vector2 chainDirection = endPoint - startPoint;
            float chainDistance = chainDirection.Length();
            float chainRotation = chainDirection.ToRotation();

            int chainSegmentHeight = chainTexture.Height;
            int numSegments = (int)(chainDistance / chainSegmentHeight) + 1;

            for (int i = 0; i < numSegments; i++)
            {
                float progress = (float)i / numSegments;
                Vector2 segmentPosition = Vector2.Lerp(startPoint, endPoint, progress);
                Vector2 segmentScreenPos = segmentPosition - Main.screenPosition;

                Rectangle sourceRect = new Rectangle(0, 0, chainTexture.Width, chainSegmentHeight);
                Vector2 origin = new Vector2(chainTexture.Width / 2f, 0);

                Main.spriteBatch.Draw(chainTexture, segmentScreenPos, sourceRect, Color.White,
                    chainRotation + MathHelper.PiOver2, origin, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}*/