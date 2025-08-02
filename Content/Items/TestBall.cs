using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.InverseKinematics;
using OvermorrowMod.Common.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Tools
{
    public class ChainBall : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = ModUtils.SecondsToTicks(20);
            Projectile.penetrate = -1;
        }

        private InverseKinematicLimb armLimb;
        private Vector2 shoulderPosition;
        private bool armInitialized = false;

        public override void OnSpawn(IEntitySource source)
        {
            shoulderPosition = Projectile.Center;
        }

        private void InitializeArm()
        {
            try
            {
                Texture2D upperArmTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "BrassArm1", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Texture2D forearmTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "BrassArm2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

                float[] segmentLengths = new float[] { upperArmTexture.Height - 16, forearmTexture.Height };
                Texture2D[] segmentTextures = new Texture2D[] { upperArmTexture, forearmTexture };
                Vector2[] origins = new Vector2[]
                {
                    new Vector2(upperArmTexture.Width / 2f, 0),    // Left side, middle for upper arm
                    new Vector2(forearmTexture.Width / 2f, 0)      // Left side, middle for forearm
                };

                armLimb = new InverseKinematicLimb(shoulderPosition.X, shoulderPosition.Y, 2, segmentLengths, 0f, segmentTextures, origins);


                armInitialized = true;
            }
            catch (Exception ex)
            {
                Main.NewText("Error initializing arm: " + ex.Message);
            }
        }

        public ref float AICounter => ref Projectile.ai[0];

        public override void AI()
        {
            AICounter++;

            // Initialize arm on first frame after spawn
            if (!armInitialized && AICounter == 1)
            {
                InitializeArm();
            }

            if (armInitialized && armLimb != null)
            {
                // Update arm to point toward mouse
                Vector2 targetPosition = Main.MouseWorld;
                armLimb.Update(targetPosition);

           
                armLimb.Segments[0].MinAngle = MathHelper.ToRadians(0f);
                armLimb.Segments[0].MaxAngle = MathHelper.ToRadians(360f);

                // Set angle constraints for the second segment (lower limb/forearm)
                // Prevent it from rotating fully backwards when facing backwards
                armLimb.Segments[1].MinAngle = armLimb.Segments[0].Angle;
                armLimb.Segments[1].MaxAngle = MathHelper.ToRadians(180f);
                if (armLimb.Segments[1].Angle >= MathHelper.ToRadians(355))
                    armLimb.Segments[1].MaxAngle = MathHelper.ToRadians(360f);


                float upperArmAngle = MathHelper.ToDegrees(armLimb.Segments[0].Angle);
                float forearmAngle = MathHelper.ToDegrees(armLimb.Segments[1].Angle);
                float forearmMinAngle = MathHelper.ToDegrees(armLimb.Segments[1].MinAngle);
                float forearmMaxAngle = MathHelper.ToDegrees(armLimb.Segments[1].MaxAngle);

                Main.NewText($"Upper Arm: {upperArmAngle:F1}°");
                Main.NewText($"Forearm: {forearmAngle:F1}° (Range: {forearmMinAngle:F1}° to {forearmMaxAngle:F1}°)");

                // Keep projectile center at the end of the arm
                Projectile.Center = armLimb.GetEndPosition();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //  debug circle at projectile center
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Vector2 screenPos = Projectile.Center - Main.screenPosition;
            Rectangle rect = new Rectangle((int)screenPos.X - 10, (int)screenPos.Y - 10, 20, 20);
            Main.spriteBatch.Draw(pixel, rect, Color.Red);

            if (armInitialized && armLimb != null)
            {
                try
                {
                    armLimb.Draw(Main.spriteBatch, lightColor);

                    Vector2 endPos = armLimb.GetEndPosition() - Main.screenPosition;
                    Rectangle endRect = new Rectangle((int)endPos.X - 5, (int)endPos.Y - 5, 10, 10);
                    Main.spriteBatch.Draw(pixel, endRect, Color.Blue);
                }
                catch (Exception ex)
                {
                    Main.NewText("Error drawing arm: " + ex.Message);
                }
            }

            return false;
        }
    }
}