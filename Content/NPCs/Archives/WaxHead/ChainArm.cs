using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
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

        private Texture2D upperArmTexture;
        private Texture2D forearmTexture;

        int ballID = -1;
        public override void OnSpawn(IEntitySource source)
        {
            upperArmTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "BrassArm1", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            forearmTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "BrassArm2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            ballID = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ChainBall>(), 0, 0, Main.myPlayer, NPC.whoAmI);
        }

        private float _bendOffset;
        public float BendOffset
        {
            get => _bendOffset;
            set => _bendOffset = MathHelper.Clamp(value, -40f, 40f);
        }

        public Vector2 AnchorPoint { get; private set; }
        public Vector2 ElbowJoint { get; private set; }
        public Vector2 HandJoint { get; private set; }

        private Vector2 currentDirection = Vector2.UnitX;
        public override void AI()
        {
            if (ballID == -1) return;

            ChainBall chainBall = Main.projectile[ballID].ModProjectile as ChainBall;

            AnchorPoint = NPC.Center;

            if (chainBall.CurrentState == ChainBall.ChainState.Waiting)
            {
                Vector2 targetDirection = Vector2.Normalize(Main.LocalPlayer.Center - AnchorPoint);
                currentDirection = Vector2.Lerp(currentDirection, targetDirection, 0.05f);
                currentDirection = Vector2.Normalize(currentDirection);
            }

            // Always calculate arm positions using current direction and bend offset
            float armLength = 130f;
            HandJoint = AnchorPoint + currentDirection * armLength;

            Vector2 straightElbow = Vector2.Lerp(AnchorPoint, HandJoint, 0.45f);
            Vector2 perpendicular = new Vector2(-currentDirection.Y, currentDirection.X);
            Vector2 backwardDirection = -currentDirection;
            float backwardAmount = Math.Abs(BendOffset) * 0.5f;

            ElbowJoint = straightElbow + perpendicular * BendOffset + backwardDirection * backwardAmount;
            HandJoint = HandJoint + perpendicular * -BendOffset * 2;

            int shoulder = Dust.NewDust(AnchorPoint, 1, 1, DustID.Torch);
            Main.dust[shoulder].noGravity = true;

            int elbow = Dust.NewDust(ElbowJoint, 1, 1, DustID.RedTorch);
            Main.dust[elbow].noGravity = true;

            int hand = Dust.NewDust(HandJoint, 1, 1, DustID.BlueTorch);
            Main.dust[hand].noGravity = true;
        }

        public Vector2 GetHandPosition()
        {
            return HandJoint;
        }

        public float GetForearmAngle()
        {
            return (HandJoint - ElbowJoint).ToRotation();
        }

        public Vector2 GetArmDirection()
        {
            return Vector2.Normalize(Main.LocalPlayer.Center - AnchorPoint);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (upperArmTexture == null || forearmTexture == null) return false;

            Vector2 anchorScreen = AnchorPoint - Main.screenPosition;
            Vector2 elbowScreen = ElbowJoint - Main.screenPosition;

            float upperArmAngle = (ElbowJoint - AnchorPoint).ToRotation();
            float forearmAngle = (HandJoint - ElbowJoint).ToRotation();

            spriteBatch.Draw(upperArmTexture, anchorScreen, null, drawColor, upperArmAngle - MathHelper.PiOver2, new Vector2(upperArmTexture.Width / 2f, 0), 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(forearmTexture, elbowScreen, null, drawColor, forearmAngle - MathHelper.PiOver2, new Vector2(forearmTexture.Width / 2f, 0), 1f, SpriteEffects.None, 0f);

            return false;
        }
    }
}