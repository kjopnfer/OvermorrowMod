using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Items.Archives.Armor;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.DrawLayers
{
    public class QuillcrestHelmDrawLayer : PlayerDrawLayer
    {
        public override bool IsHeadLayer => true;
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.drawPlayer.head == EquipLoader.GetEquipSlot(Mod, nameof(QuillcrestHelm), EquipType.Head);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            Color color = drawPlayer.GetImmuneAlphaPure(drawInfo.colorArmorHead, drawInfo.shadow);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveItems + "QuillcrestHelm_DrawLayer", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            float baseVerticalOffset = -8f;
            float directionShift = 6f;
            float verticalLift = 14f;
            Vector2 fineOffset = new Vector2(0f, 0f);

            Vector2 position = drawInfo.Position;
            Vector2 origin = new(texture.Width * 0.5f, texture.Height * 0.5f);
            Vector2 drawPos = new Vector2((int)(position.X - drawPlayer.bodyFrame.Width / 2 + drawPlayer.width / 2), (int)(position.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + baseVerticalOffset)) + drawPlayer.bodyPosition + new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2);
            drawPos.X += drawPlayer.direction == 1 ? -directionShift : directionShift;
            drawPos.Y -= verticalLift * drawPlayer.gravDir;
            drawPos += fineOffset;

            DrawData drawData = new(texture, drawPos + (Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height] * drawPlayer.gravDir) - Main.screenPosition, new Rectangle?(), color, drawInfo.drawPlayer.headRotation, origin, 1f, drawInfo.playerEffect, 0)
            {
                shader = drawInfo.cHead
            };
            drawInfo.DrawDataCache.Add(drawData);
        }
    }
}
