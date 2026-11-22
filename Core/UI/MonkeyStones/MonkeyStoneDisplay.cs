using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Items.Archives;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.UI.RunStats;
using OvermorrowMod.Core.WorldGeneration;
using ReLogic.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace OvermorrowMod.Core.UI.MonkeyStones
{
    public class MonkeyStoneDisplay : UIState
    {
        private const int HotbarStartX = 20;
        private const int HotbarSlotGap = 4;
        private const int HotbarCenterY = 47;
        private const int EntryGap = 12;
        private const int RowIconSize = 20;
        private const int IconTextGap = 4;
        private const int StackedRowHeight = 20;
        private const int TimerClearance = 50;
        private const float RowTextScale = 0.8f;

        private const int ColumnX = 577;
        private const int ColumnLabelX = 575;
        private const int ColumnLabelY = 84;
        private const float LabelScale = 0.75f;
        private const float InventoryScale = 0.6f;

        private const float EmptyAlpha = 0.4f;

        private static readonly MonkeyStoneTier[] Tiers = Enum.GetValues<MonkeyStoneTier>();

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!RunState.InRun) return;

            var wallet = Main.LocalPlayer.GetModPlayer<MonkeyStonePlayer>();
            if (Main.playerInventory) DrawColumn(spriteBatch, wallet);
            else DrawRow(spriteBatch, wallet);
        }

        private static void DrawColumn(SpriteBatch spriteBatch, MonkeyStonePlayer wallet)
        {
            Color labelColor = new(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor);
            spriteBatch.DrawString(FontAssets.MouseText.Value, Language.GetTextValue(LocalizationPath.UI + "MonkeyStones.Label"), new Vector2(ColumnLabelX, ColumnLabelY), labelColor, 0f, Vector2.Zero, LabelScale, SpriteEffects.None, 0f);

            Texture2D back = TextureAssets.InventoryBack.Value;
            foreach (MonkeyStoneTier tier in Tiers)
            {
                int count = wallet.Get(tier);
                Color color = Color.White * (count > 0 ? 1f : EmptyAlpha);
                Vector2 position = new(ColumnX, (int)(85f + (int)tier * 56 * InventoryScale + 20f));
                Rectangle box = new((int)position.X, (int)position.Y, (int)(back.Width * InventoryScale), (int)(back.Height * InventoryScale));

                spriteBatch.Draw(back, position, null, Main.inventoryBack, 0f, Vector2.Zero, InventoryScale, SpriteEffects.None, 0f);
                RunStatsPanel.DrawItemIcon(spriteBatch, StoneType(tier), box.Center.ToVector2(), 32 * InventoryScale, color);
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, FormatCount(count), position + new Vector2(10f, 26f) * InventoryScale, color, 0f, Vector2.Zero, new Vector2(InventoryScale), -1f, InventoryScale);
                Hover(box, tier, count);
            }
        }

        private static void DrawRow(SpriteBatch spriteBatch, MonkeyStonePlayer wallet)
        {
            float x = HotbarRight() + EntryGap;
            float[] widths = Tiers.Select(tier => EntryWidth(wallet.GetGainedThisRun(tier))).ToArray();
            bool stacked = x + widths.Sum() + EntryGap * (Tiers.Length - 1) > Main.screenWidth / 2f - TimerClearance;
            int columns = stacked ? 2 : Tiers.Length;
            int rows = Tiers.Length / columns;

            for (int column = 0; column < columns; column++)
            {
                float columnWidth = 0f;
                for (int row = 0; row < rows; row++)
                {
                    int index = row * columns + column;
                    float centerY = HotbarCenterY + (row - (rows - 1) / 2f) * StackedRowHeight;
                    DrawEntry(spriteBatch, Tiers[index], wallet.GetGainedThisRun(Tiers[index]), x, centerY, widths[index]);
                    columnWidth = Math.Max(columnWidth, widths[index]);
                }
                x += columnWidth + EntryGap;
            }
        }

        private static void DrawEntry(SpriteBatch spriteBatch, MonkeyStoneTier tier, int count, float x, float centerY, float width)
        {
            Color color = Color.White * (count > 0 ? 1f : EmptyAlpha);
            RunStatsPanel.DrawItemIcon(spriteBatch, StoneType(tier), new Vector2(x + RowIconSize / 2f, centerY), RowIconSize, color);

            string text = FormatCount(count);
            float textHeight = FontAssets.MouseText.Value.MeasureString(text).Y * RowTextScale;
            Vector2 textPosition = new(x + RowIconSize + IconTextGap, centerY - textHeight / 2f + 3f * RowTextScale);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, textPosition, color, 0f, Vector2.Zero, new Vector2(RowTextScale));

            Hover(new Rectangle((int)x, (int)(centerY - RowIconSize / 2f), (int)width, RowIconSize), tier, count);
        }

        private static void Hover(Rectangle area, MonkeyStoneTier tier, int count)
        {
            if (!area.Contains(Main.MouseScreen.ToPoint())) return;

            Main.LocalPlayer.mouseInterface = true;
            Main.hoverItemName = Lang.GetItemNameValue(StoneType(tier)) + " (" + count + ")";
        }

        private static int HotbarRight()
        {
            int x = HotbarStartX;
            for (int i = 0; i < InventoryPlayer.HotbarSlots; i++)
                x += (int)(TextureAssets.InventoryBack.Width() * Main.hotbarScale[i]) + HotbarSlotGap;
            return x - HotbarSlotGap;
        }

        private static float EntryWidth(int count) => RowIconSize + IconTextGap + FontAssets.MouseText.Value.MeasureString(FormatCount(count)).X * RowTextScale;

        private static string FormatCount(int count) => count < 10000 ? count.ToString() : (count / 1000f).ToString("0.#") + "k";

        internal static int StoneType(MonkeyStoneTier tier) => tier switch
        {
            MonkeyStoneTier.Blue => ModContent.ItemType<MonkeyStoneBlue>(),
            MonkeyStoneTier.Green => ModContent.ItemType<MonkeyStoneGreen>(),
            MonkeyStoneTier.Orange => ModContent.ItemType<MonkeyStoneOrange>(),
            _ => ModContent.ItemType<ChaosDiamond>(),
        };
    }
}
