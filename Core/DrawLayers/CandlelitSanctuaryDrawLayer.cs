using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Items;
using OvermorrowMod.Content.Items.Archives;
using OvermorrowMod.Content.Items.Archives.Accessories;
using OvermorrowMod.Core.Globals;
using ReLogic.Content;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.DrawLayers
{
    public class CandlelitSanctuaryDrawLayer : PlayerDrawLayer
    {
        private static Asset<Texture2D> CandleFlame;
        public override void Load()
        {
            CandleFlame = ModContent.Request<Texture2D>(AssetDirectory.ArchiveItems + "CandlelitSanctuaryFlame", ReLogic.Content.AssetRequestMode.ImmediateLoad);
        }

        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Shield);
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.drawPlayer.shield == EquipLoader.GetEquipSlot(Mod, nameof(CandlelitSanctuary), EquipType.Shield);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            //Color color = drawPlayer.GetImmuneAlphaPure(drawInfo.colorArmorHead, drawInfo.shadow);
            var candleInstance = OvermorrowAccessory.GetInstance<CandlelitSanctuary>(drawPlayer);
            if (candleInstance == null) return;

            int charges = candleInstance.CandleCharges;
            int direction = drawPlayer.direction;

            var shieldMask = CandleFlame;
            Vector2 Position = drawInfo.Position;
            Vector2 origin = new(CandleFlame.Value.Width * 0.5f, CandleFlame.Value.Height * 0.5f);
            Vector2 drawPos = new Vector2((int)(Position.X - drawPlayer.bodyFrame.Width / 2 + drawPlayer.width / 2), (int)(Position.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f)) + drawPlayer.bodyPosition + new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2);
            drawPos.X += drawPlayer.direction == 1 ? -2 : 2;
            drawPos.Y -= 44 * drawPlayer.gravDir;

            Rectangle bodyFrame = drawInfo.drawPlayer.bodyFrame;
            Vector2 bodyVect = drawInfo.bodyVect;

            if (bodyFrame.Width != shieldMask.Width())
            {
                bodyFrame.Width = shieldMask.Width();
                bodyVect.X += bodyFrame.Width - shieldMask.Width();

                if (drawInfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally))
                    bodyVect.X = bodyFrame.Width - bodyVect.X;
            }

            var movementFrame = bodyFrame.Y / bodyFrame.Height;

            int[] upFrames = [7, 8, 9, 14, 15, 16];

            float jumpOffset = movementFrame == 5 ? -8 * drawPlayer.gravDir : 0;

            float walkOffset = upFrames.Contains(movementFrame) ? -2 * drawPlayer.gravDir : 0;
            for (int i = 0; i < charges; i++)
            {
                //SpawnParticles(new Vector2(12 * direction, -14));

                Vector2 chargeOffset = Vector2.Zero;
                if (i == 1)
                {
                    chargeOffset = new Vector2(12, -4 * drawPlayer.gravDir);
                }
                else if (i == 2)
                {
                    chargeOffset = new Vector2(6, -6 * drawPlayer.gravDir);
                }

                Vector2 gravityOffset = drawPlayer.gravDir == -1 ? new Vector2(0, 42) : Vector2.Zero;
                Vector2 position = new Vector2((14 + chargeOffset.X) * direction, -10 + chargeOffset.Y + jumpOffset + walkOffset) + new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - bodyFrame.Width / 2 + drawInfo.drawPlayer.width / 2), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.bodyPosition + new Vector2(bodyFrame.Width / 2, drawInfo.drawPlayer.bodyFrame.Height / 2);
                var frameOffset = (int)MathHelper.Lerp(0, 5, ((Main.GlobalTimeWrappedHourly + (0.15f * i)) % 0.6f) / 0.6f);
                var frame = new Rectangle(0, (int)(shieldMask.Height() / 5 * frameOffset), shieldMask.Width(), shieldMask.Height() / 5);

                DrawData drawData = new(shieldMask.Value, position + gravityOffset, frame, Color.White, drawInfo.drawPlayer.bodyRotation, bodyVect, 0.7f, drawInfo.playerEffect, 1)
                {
                    shader = drawInfo.cShield
                };
                drawInfo.DrawDataCache.Add(drawData);

                if (drawInfo.drawPlayer.mount.Cart)
                    drawInfo.DrawDataCache.Reverse(drawInfo.DrawDataCache.Count - 2, 2);

                //Particle.CreateParticleDirect(Particle.ParticleType<Ember>(), position, velocity, Color.DarkBlue, 1f, scale, 0f, 0, scale);
            }

            /*Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveItems + "ScholarsHat_DrawLayer", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Vector2 Position = drawInfo.Position;
            Vector2 origin = new(texture.Width * 0.5f, texture.Height * 0.5f);
            Vector2 drawPos = new Vector2((int)(Position.X - drawPlayer.bodyFrame.Width / 2 + drawPlayer.width / 2), (int)(Position.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f)) + drawPlayer.bodyPosition + new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2);
            drawPos.X += drawPlayer.direction == 1 ? -2 : 2;
            drawPos.Y -= 14 * drawPlayer.gravDir;
            DrawData drawData = new(texture, drawPos + (Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height] * drawPlayer.gravDir) - Main.screenPosition, new Rectangle?(), color, drawInfo.drawPlayer.headRotation, origin, 1, drawInfo.playerEffect, 0)
            {
                shader = drawInfo.cHead
            };
            drawInfo.DrawDataCache.Add(drawData);*/
        }
    }
}