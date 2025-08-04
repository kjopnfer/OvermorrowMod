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
        private Vector2 anchorPoint;
        private Vector2 elbowJoint;
        private Vector2 handJoint;
        public override void OnSpawn(IEntitySource source)
        {
            upperArmTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "BrassArm1", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            forearmTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "BrassArm2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ChainBall>(), 0, 0, Main.myPlayer, NPC.whoAmI);
        }

        private float _bendOffset;
        public float BendOffset
        {
            get => _bendOffset;
            set => _bendOffset = MathHelper.Clamp(value, -40f, 40f);
        }
        public override void AI()
        {
            anchorPoint = NPC.Center;

            Vector2 directionToPlayer = Vector2.Normalize(Main.LocalPlayer.Center - anchorPoint);
            float armLength = 130f;
            handJoint = anchorPoint + directionToPlayer * armLength;

            Vector2 straightElbow = Vector2.Lerp(anchorPoint, handJoint, 0.45f);
            Vector2 perpendicular = new Vector2(-directionToPlayer.Y, directionToPlayer.X);
            //BendOffset = MathHelper.Lerp(0f, -40f, 0f);
            Vector2 backwardDirection = -directionToPlayer; // Toward shoulder
            float backwardAmount = Math.Abs(BendOffset) * 0.5f; // 30% of bend amount

            elbowJoint = straightElbow + perpendicular * BendOffset + backwardDirection * backwardAmount;
            handJoint = handJoint + perpendicular * -BendOffset * 2;

            int shoulder = Dust.NewDust(anchorPoint, 1, 1, DustID.Torch);
            Main.dust[shoulder].noGravity = true;

            int elbow = Dust.NewDust(elbowJoint, 1, 1, DustID.RedTorch);
            Main.dust[elbow].noGravity = true;

            int hand = Dust.NewDust(handJoint, 1, 1, DustID.BlueTorch);
            Main.dust[hand].noGravity = true;
        }

        public Vector2 GetHandPosition()
        {
            return handJoint;
        }

        public float GetForearmAngle()
        {
            return (handJoint - elbowJoint).ToRotation();
        }

        public Vector2 GetArmDirection()
        {
            return Vector2.Normalize(Main.LocalPlayer.Center - anchorPoint);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (upperArmTexture == null || forearmTexture == null) return false;

            Vector2 anchorScreen = anchorPoint - Main.screenPosition;
            Vector2 elbowScreen = elbowJoint - Main.screenPosition;

            float upperArmAngle = (elbowJoint - anchorPoint).ToRotation();
            float forearmAngle = (handJoint - elbowJoint).ToRotation();

            spriteBatch.Draw(upperArmTexture, anchorScreen, null, drawColor, upperArmAngle - MathHelper.PiOver2, new Vector2(upperArmTexture.Width / 2f, 0), 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(forearmTexture, elbowScreen, null, drawColor, forearmAngle - MathHelper.PiOver2, new Vector2(forearmTexture.Width / 2f, 0), 1f, SpriteEffects.None, 0f);

            return false;
        }
    }
}