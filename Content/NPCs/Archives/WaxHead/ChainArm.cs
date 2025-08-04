using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.InverseKinematics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs
{
    public class ChainArm : ModNPC
    {
        public override string Texture => AssetDirectory.Empty;

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 60;
            NPC.lifeMax = 1000;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
            NPC.immortal = true;
        }

        public InverseKinematicLimb armLimb { get; private set; }
        private Vector2 shoulderPosition;
        private bool armInitialized = false;

        public override void OnSpawn(IEntitySource source)
        {
            shoulderPosition = NPC.Center;
        }

        private int chainBallProjectileIndex = -1;
        private void SpawnChainBall()
        {
            if (Main.netMode != Terraria.ID.NetmodeID.MultiplayerClient)
            {
                Vector2 armEndPos = armLimb.GetEndPosition();
                int projectileIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ChainBall>(), 0, Main.myPlayer, 0, NPC.whoAmI);

                if (projectileIndex >= 0 && projectileIndex < Main.maxProjectiles)
                {
                    chainBallProjectileIndex = projectileIndex;
                }
            }
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

                SpawnChainBall();
            }
            catch (Exception ex)
            {
                Main.NewText("Error initializing arm: " + ex.Message);
            }
        }

        public ref float AICounter => ref NPC.ai[0];

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

                if (AICounter % 10 == 0) // Every 10 frames
                {
                    Vector2 armTip = armLimb.GetEndPosition();
                    Vector2 mousePos = Main.MouseWorld;
                    float armToMouse = Vector2.Distance(armTip, mousePos);

                    Core.OvermorrowModFile.Instance.Logger.Debug($"ArmLag: {armToMouse:F1}");
                }
            }
        }

        public Vector2 GetArmEndPosition()
        {
            if (armInitialized && armLimb != null)
            {
                return armLimb.GetEndPosition();
            }
            return NPC.Center;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // Debug circle at NPC center
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Vector2 npcScreenPos = NPC.Center - Main.screenPosition;
            Rectangle rect = new Rectangle((int)npcScreenPos.X - 10, (int)npcScreenPos.Y - 10, 20, 20);
            spriteBatch.Draw(pixel, rect, Color.Red);

            if (armInitialized && armLimb != null)
            {
                try
                {
                    armLimb.Draw(spriteBatch, drawColor);

                    Vector2 armEndPos = armLimb.GetEndPosition() - Main.screenPosition;
                    Rectangle endRect = new Rectangle((int)armEndPos.X - 5, (int)armEndPos.Y - 5, 10, 10);
                    //spriteBatch.Draw(pixel, endRect, Color.Blue);
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