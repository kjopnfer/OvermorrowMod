using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Items.Archives;
using OvermorrowMod.Core.UI.MonkeyStones;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Ledger = OvermorrowMod.Core.UI.DeathScreen.DeathScreen;

namespace OvermorrowMod.Core.UI.Upgrades
{
    public partial class UpgradeScreen
    {
        private const int WalletRowHeight = 34;
        private const int StatueColumns = 8;
        private const int StatueRows = 10;
        private const float StatueScale = 1.35f;

        private void DrawAltar(SpriteBatch spriteBatch, Rectangle area)
        {
            float alpha = Appear(0, 48);
            int rise = (int)((1f - alpha) * 14);
            Texture2D statue = ModContent.Request<Texture2D>(AssetDirectory.ArchiveTiles + "NormalWizardStatue").Value;
            int statueWidth = (int)(StatueColumns * 16 * StatueScale);
            int statueHeight = (int)(StatueRows * 16 * StatueScale);
            statueArea = new Rectangle(area.Center.X - statueWidth / 2, area.Y + 8 + rise, statueWidth, statueHeight);

            Texture2D glow = ModContent.Request<Texture2D>(AssetDirectory.Textures + "spotlight").Value;
            float breathe = 0.775f + 0.225f * MathF.Cos(openTicks / 240f * MathHelper.TwoPi);
            Vector2 glowScale = new Vector2(280f / glow.Width, 300f / glow.Height) * (0.94f + 0.06f * breathe);
            spriteBatch.Draw(glow, statueArea.Center.ToVector2() + new Vector2(0, 16), null, AuraColor with { A = 0 } * 0.45f * breathe * alpha, 0f, glow.Size() / 2f, glowScale, SpriteEffects.None, 0f);

            for (int x = 0; x < StatueColumns; x++)
            {
                for (int y = 0; y < StatueRows; y++)
                {
                    Rectangle source = new(x * 18, y * 18, 16, 16);
                    Vector2 position = new(statueArea.X + x * 16 * StatueScale, statueArea.Y + y * 16 * StatueScale);
                    spriteBatch.Draw(statue, position, source, Color.White * alpha, 0f, Vector2.Zero, StatueScale, SpriteEffects.None, 0f);
                }
            }

            motes.Draw(spriteBatch, new Vector2(statueArea.X, statueArea.Y));

            int nameY = statueArea.Bottom + 4;
            Ledger.DrawCentered(spriteBatch, Text("VendorName"), new Vector2(area.Center.X, nameY + 12), Ledger.GoldColor * alpha, 1.15f);

            int rowY = nameY + 36;
            for (int i = 0; i < Tiers.Length; i++)
            {
                MonkeyStoneTier tier = Tiers[i];
                int count = Wallet.Get(tier);
                Rectangle row = new(area.X, rowY + i * (WalletRowHeight + EntryGap), area.Width, WalletRowHeight);
                walletRows[i] = row;

                float rowAlpha = alpha * (count == 0 ? 0.45f : 1f);
                float flash = walletFlash[i] / 24f;
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, row, Ledger.SlotColor * alpha);
                if (flash > 0f) spriteBatch.Draw(TextureAssets.MagicPixel.Value, row, TierColor(tier) * 0.25f * flash * alpha);
                Ledger.DrawBorder(spriteBatch, row, Color.Lerp(Ledger.DarkSilverColor, TierColor(tier), flash) * alpha);

                DrawStone(spriteBatch, tier, new Vector2(row.X + 22, row.Center.Y), 22, Color.White * rowAlpha);
                Ledger.DrawTextCentered(spriteBatch, Lang.GetItemNameValue(MonkeyStoneDisplay.StoneType(tier)), row.X + 42, row.Center.Y, Ledger.SilverColor * rowAlpha, 0.9f);
                string amount = count.ToString();
                Ledger.DrawTextCentered(spriteBatch, amount, row.Right - 10 - Ledger.TextWidth(amount, 1.1f), row.Center.Y, Color.White * rowAlpha, 1.1f);
            }
        }
    }
}
