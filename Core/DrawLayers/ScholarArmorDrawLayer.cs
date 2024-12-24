using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Items.Armor.Kagenoi;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.DrawLayers
{
    public class ScholarArmorDrawLayer : PlayerDrawLayer
    {
        public override bool IsHeadLayer => true;
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.drawPlayer.head == EquipLoader.GetEquipSlot(Mod, nameof(ScholarsHat), EquipType.Head);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            Color color = drawPlayer.GetImmuneAlphaPure(drawInfo.colorArmorHead, drawInfo.shadow);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveItems + "ScholarsHat_DrawLayer", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Vector2 Position = drawInfo.Position;
            Vector2 origin = new(texture.Width * 0.5f, texture.Height * 0.5f);
            Vector2 drawPos = new Vector2((int)(Position.X - drawPlayer.bodyFrame.Width / 2 + drawPlayer.width / 2), (int)(Position.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f)) + drawPlayer.bodyPosition + new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2);
            drawPos.X += drawPlayer.direction == 1 ? -2 : 2;
            drawPos.Y -= 14 * drawPlayer.gravDir;
            DrawData drawData = new(texture, drawPos + (Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height] * drawPlayer.gravDir) - Main.screenPosition, new Rectangle?(), color, drawInfo.drawPlayer.headRotation, origin, 1, drawInfo.playerEffect, 0)
            {
                shader = drawInfo.cHead
            };
            drawInfo.DrawDataCache.Add(drawData);
        }
    }
}