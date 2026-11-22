using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Items.Collectibles;
using OvermorrowMod.Core.WorldGeneration;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace OvermorrowMod.Core.UI.RunStats
{
    /// <summary>
    /// Lists the player's stat bonuses from all sources, plus the additional effects and items granted by collectibles this run.
    /// Shows as a popup beside the reward picker, and in the equipment area on the Stats page.
    /// </summary>
    public class RunStatsPanel : UIState
    {
        /// <summary>
        /// Equipment page value for the Stats tab. Any value outside vanilla's 0 to 3 hides the armor and accessory slots.
        /// </summary>
        public const int Page = 10;

        private const int Width = 320;
        private const int RightMargin = 50;
        private const int BottomMargin = 20;
        private const int MinHeight = 80;
        private const int ScrollStep = 40;
        private const int ScrollbarWidth = 4;
        private const int MinThumbHeight = 16;
        private const int Padding = 14;
        private const int SectionGap = 12;
        private const int HeaderHeight = 20;
        private const int HeaderGap = 6;
        private const int Gap = 5;
        private const int StatHeight = 26;
        private const int CellPadding = 8;
        private const int EffectIconSize = 30;
        private const int EffectTitleHeight = 18;
        private const int EffectLineHeight = 17;
        private const int CollectedColumns = 6;
        private const int CollectedHeight = 44;

        private const float HeaderScale = 0.85f;
        private const float NameScale = 0.75f;
        private const float ValueScale = 0.9f;
        private const float EffectValueScale = 0.7f;

        private static readonly Color PanelColor = new(38, 50, 112);
        private static readonly Color CellColor = new Color(10, 16, 48) * 0.6f;
        private static readonly Color GainColor = new(138, 240, 110);
        private static readonly Color LossColor = new(255, 122, 122);
        private static readonly Color HeaderColor = new(245, 221, 133);
        private static readonly Color NameColor = new(223, 228, 255);
        private static readonly Color CountColor = new(200, 208, 245);

        /// <summary>
        /// Each stat's bonus over the player's base value, read from the final stats after all updates.
        /// Class-scaled stats use the held weapon's damage class.
        /// </summary>
        private static readonly (string Key, bool Percent, Func<Player, DamageClass, float> Bonus)[] StatBonuses =
        {
            ("MaxLife", false, (player, damageClass) => player.statLifeMax2 - SubworldPlayer.BaseMaxLife),
            ("MaxMana", false, (player, damageClass) => player.statManaMax2 - (20 + player.ConsumedManaCrystals * 20)),
            ("Defense", false, (player, damageClass) => (int)player.statDefense),
            ("DamagePercent", true, (player, damageClass) => (player.GetTotalDamage(damageClass).Additive * player.GetTotalDamage(damageClass).Multiplicative - 1f) * 100f),
            ("DamageFlat", false, (player, damageClass) => player.GetTotalDamage(damageClass).Flat),
            ("CritChance", true, (player, damageClass) => player.GetTotalCritChance(damageClass)),
            ("MoveSpeed", true, (player, damageClass) => (player.moveSpeed - 1f) * 100f),
            ("KnockbackPercent", true, (player, damageClass) => (player.GetTotalKnockback(damageClass).Additive * player.GetTotalKnockback(damageClass).Multiplicative - 1f) * 100f),
            ("KnockbackFlat", false, (player, damageClass) => player.GetTotalKnockback(damageClass).Flat),
            ("ArmorPenetration", false, (player, damageClass) => player.GetTotalArmorPenetration(damageClass)),
            ("Luck", false, (player, damageClass) => player.luck),
        };

        /// <summary>
        /// The y position vanilla drew the equipment page icons at this frame. The tab panel starts below it.
        /// </summary>
        internal static int PageIconsY;

        private readonly List<(string Name, string Value, Color Color)> stats = new();
        private readonly List<(int IconType, string Title, string[] ValueLines, Color Color)> effects = new();
        private readonly List<(int Type, int Count)> collected = new();
        private int collectedTotal;
        private int builtVersion = -1;
        private float scrollOffset;
        private bool draggingScrollbar;
        private float dragGrabOffset;

        private static readonly RasterizerState ClipRasterizer = new() { CullMode = CullMode.None, ScissorTestEnable = true };

        private static int ContentWidth => Width - Padding * 2;
        private static int CollectedWidth => (ContentWidth - Gap * (CollectedColumns - 1)) / CollectedColumns;

        public static bool PopupVisible => RunState.InRun && RewardSelection.visible && !Main.playerInventory;
        public static bool TabVisible => RunState.InRun && Main.playerInventory && Main.EquipPage == Page;

        public override void Draw(SpriteBatch spriteBatch)
        {
            bool tab = TabVisible;
            if (!tab && !PopupVisible)
                return;

            float opacity = tab ? 1f : RewardSelectionManager.Opacity;
            if (opacity <= 0f)
                return;

            var player = Main.LocalPlayer.GetModPlayer<CollectiblePlayer>();
            if (player.Version != builtVersion)
                Rebuild(player);

            RebuildStats(Main.LocalPlayer);

            int top = tab ? PageIconsY + 42 : PopupTop();
            int contentHeight = MeasureContentHeight();
            int height = Math.Max(MinHeight, Math.Min(contentHeight + Padding * 2, Main.screenHeight - BottomMargin - top));
            Rectangle bounds = new(Main.screenWidth - RightMargin - Width, top, Width, height);
            Rectangle clip = new(bounds.X, bounds.Y + Padding, Width, height - Padding * 2);
            int maxScroll = Math.Max(0, contentHeight - clip.Height);

            Rectangle track = new(bounds.Right - Padding, clip.Y, Padding, clip.Height);
            int thumbHeight = ThumbHeight(clip.Height, contentHeight);

            if (bounds.Contains(Main.MouseScreen.ToPoint()))
            {
                Main.LocalPlayer.mouseInterface = true;
                if (maxScroll > 0)
                {
                    PlayerInput.LockVanillaMouseScroll("OvermorrowMod/StatsPanel");
                    scrollOffset -= PlayerInput.ScrollWheelDeltaForUI / 120f * ScrollStep;
                }
            }

            if (maxScroll > 0)
                UpdateScrollbarDrag(track, thumbHeight, maxScroll);
            else
                draggingScrollbar = false;

            scrollOffset = MathHelper.Clamp(scrollOffset, 0f, maxScroll);

            Utils.DrawInvBG(spriteBatch, bounds, PanelColor * 0.94f * opacity);

            RasterizerState previousRasterizer = spriteBatch.GraphicsDevice.RasterizerState;
            Rectangle previousScissor = spriteBatch.GraphicsDevice.ScissorRectangle;
            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = ToScreenRectangle(clip, spriteBatch.GraphicsDevice.Viewport.Bounds);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, ClipRasterizer, null, Main.UIScaleMatrix);

            DrawContent(spriteBatch, bounds.X + Padding, clip.Y - (int)scrollOffset, clip, opacity);

            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = previousScissor;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, previousRasterizer, null, Main.UIScaleMatrix);

            if (maxScroll > 0)
                DrawScrollbar(spriteBatch, clip, bounds.Right - Padding / 2 - ScrollbarWidth / 2, thumbHeight, scrollOffset / maxScroll, opacity);
        }

        /// <summary>
        /// Pressing anywhere on the track grabs the thumb (jumping it under the cursor when pressed off the thumb), and holding the button drags it.
        /// </summary>
        private void UpdateScrollbarDrag(Rectangle track, int thumbHeight, int maxScroll)
        {
            if (!Main.mouseLeft)
            {
                draggingScrollbar = false;
                return;
            }

            int travel = track.Height - thumbHeight;
            if (!draggingScrollbar && Main.mouseLeftRelease && track.Contains(Main.MouseScreen.ToPoint()))
            {
                float thumbY = track.Y + travel * (scrollOffset / maxScroll);
                bool onThumb = Main.MouseScreen.Y >= thumbY && Main.MouseScreen.Y < thumbY + thumbHeight;
                dragGrabOffset = onThumb ? Main.MouseScreen.Y - thumbY : thumbHeight / 2f;
                draggingScrollbar = true;
                Main.mouseLeftRelease = false;
            }

            if (draggingScrollbar)
            {
                Main.LocalPlayer.mouseInterface = true;
                scrollOffset = (Main.MouseScreen.Y - dragGrabOffset - track.Y) / travel * maxScroll;
            }
        }

        private void DrawContent(SpriteBatch spriteBatch, int x, int y, Rectangle clip, float opacity)
        {
            bool first = true;
            bool mouseInClip = clip.Contains(Main.MouseScreen.ToPoint());

            if (stats.Count > 0)
            {
                y = DrawHeader(spriteBatch, "Stats", null, x, y, ref first, opacity);
                int columnWidth = (ContentWidth - Gap) / 2;
                for (int i = 0; i < stats.Count; i++)
                {
                    var (name, value, color) = stats[i];
                    Rectangle cell = new(x + i % 2 * (columnWidth + Gap), y + i / 2 * (StatHeight + Gap), columnWidth, StatHeight);
                    Utils.DrawInvBG(spriteBatch, cell, CellColor * opacity);
                    DrawTextCentered(spriteBatch, name, cell.X + CellPadding, cell.Center.Y, NameColor * opacity, NameScale);
                    float valueWidth = TextWidth(value, ValueScale);
                    DrawTextCentered(spriteBatch, value, cell.Right - CellPadding - valueWidth, cell.Center.Y, color * opacity, ValueScale);
                }
                y += Rows(stats.Count, 2) * (StatHeight + Gap) - Gap;
            }

            if (effects.Count > 0)
            {
                y = DrawHeader(spriteBatch, "Effects", null, x, y, ref first, opacity);
                foreach (var effect in effects)
                {
                    Rectangle row = new(x, y, ContentWidth, EffectHeight(effect.ValueLines.Length));
                    Utils.DrawInvBG(spriteBatch, row, CellColor * opacity);

                    if (effect.IconType > 0)
                        DrawItemIcon(spriteBatch, effect.IconType, new Vector2(row.X + CellPadding + EffectIconSize / 2f, row.Center.Y), EffectIconSize, Color.White * opacity);

                    int textX = row.X + CellPadding + EffectIconSize + CellPadding;
                    int textY = row.Center.Y - (EffectTitleHeight + effect.ValueLines.Length * EffectLineHeight) / 2;
                    DrawText(spriteBatch, effect.Title, new Vector2(textX, textY), Color.White * opacity, NameScale);
                    for (int i = 0; i < effect.ValueLines.Length; i++)
                        DrawText(spriteBatch, effect.ValueLines[i], new Vector2(textX, textY + EffectTitleHeight + i * EffectLineHeight), effect.Color * opacity, EffectValueScale);

                    y += row.Height + Gap;
                }
                y -= Gap;
            }

            if (collected.Count > 0)
            {
                string count = Language.GetTextValue(LocalizationPath.UI + "RunStats.ItemCount", collectedTotal);
                y = DrawHeader(spriteBatch, "Collected", count, x, y, ref first, opacity);
                for (int i = 0; i < collected.Count; i++)
                {
                    var (type, amount) = collected[i];
                    Rectangle cell = new(x + i % CollectedColumns * (CollectedWidth + Gap), y + i / CollectedColumns * (CollectedHeight + Gap), CollectedWidth, CollectedHeight);
                    spriteBatch.Draw(TextureAssets.InventoryBack.Value, cell, Color.White * 0.9f * opacity);
                    DrawItemIcon(spriteBatch, type, cell.Center.ToVector2(), 30, Color.White * opacity);

                    if (amount > 1)
                    {
                        string amountText = "x" + amount;
                        DrawText(spriteBatch, amountText, new Vector2(cell.Right - 3 - TextWidth(amountText, NameScale), cell.Bottom - 18), Color.White * opacity, NameScale);
                    }

                    if (opacity >= 1f && mouseInClip && cell.Contains(Main.MouseScreen.ToPoint()))
                    {
                        Main.HoverItem = ContentSamples.ItemsByType[type].Clone();
                        Main.hoverItemName = Main.HoverItem.Name;
                    }
                }
            }
        }

        private void Rebuild(CollectiblePlayer player)
        {
            builtVersion = player.Version;

            effects.Clear();
            foreach (CollectibleEffect effect in Enum.GetValues<CollectibleEffect>())
            {
                float magnitude = player.GetEffect(effect);
                if (magnitude == 0f)
                    continue;

                object[] args = EffectArgs(player, effect, magnitude);
                string key = LocalizationPath.UI + "RunStats.Effect." + effect;
                int iconType = SourceItem(player, effect);
                string title = Language.GetTextValue(key + ".Title", args);
                if (iconType > 0)
                    title = ContentSamples.ItemsByType[iconType].Name + ": " + title;

                int wrapWidth = (int)((ContentWidth - CellPadding * 3 - EffectIconSize) / EffectValueScale);
                string[] valueLines = Utils.WordwrapString(Language.GetTextValue(key + ".Value", args), FontAssets.MouseText.Value, wrapWidth, 10, out _).Where(line => !string.IsNullOrEmpty(line)).ToArray();
                effects.Add((iconType, title, valueLines, effect == CollectibleEffect.ShopMarkup ? LossColor : GainColor));
            }

            collected.Clear();
            foreach (var (type, count) in player.Collected)
                collected.Add((type, count));
            collectedTotal = collected.Sum(item => item.Count);
        }

        private void RebuildStats(Player player)
        {
            DamageClass damageClass = player.HeldItem.damage > 0 ? player.HeldItem.DamageType : DamageClass.Generic;

            stats.Clear();
            foreach (var (key, percent, bonus) in StatBonuses)
            {
                float value = MathF.Round(bonus(player, damageClass), 2);
                if (value == 0f)
                    continue;

                string text = (value > 0f ? "+" : "") + Format(value) + (percent ? "%" : "");
                stats.Add((Language.GetTextValue(LocalizationPath.UI + "RunStats.Stat." + key), text, value > 0f ? GainColor : LossColor));
            }
        }

        private int MeasureContentHeight()
        {
            int height = 0;
            int sections = 0;

            if (stats.Count > 0)
            {
                height += HeaderHeight + HeaderGap + Rows(stats.Count, 2) * (StatHeight + Gap) - Gap;
                sections++;
            }

            if (effects.Count > 0)
            {
                height += HeaderHeight + HeaderGap + effects.Sum(effect => EffectHeight(effect.ValueLines.Length) + Gap) - Gap;
                sections++;
            }

            if (collected.Count > 0)
            {
                height += HeaderHeight + HeaderGap + Rows(collected.Count, CollectedColumns) * (CollectedHeight + Gap) - Gap;
                sections++;
            }

            return height + Math.Max(0, sections - 1) * SectionGap;
        }

        private void DrawScrollbar(SpriteBatch spriteBatch, Rectangle clip, int x, int thumbHeight, float progress, float opacity)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            int thumbY = clip.Y + (int)((clip.Height - thumbHeight) * progress);
            spriteBatch.Draw(pixel, new Rectangle(x, clip.Y, ScrollbarWidth, clip.Height), CellColor * opacity);
            spriteBatch.Draw(pixel, new Rectangle(x, thumbY, ScrollbarWidth, thumbHeight), NameColor * (draggingScrollbar ? 1f : 0.8f) * opacity);
        }

        private static int ThumbHeight(int visibleHeight, int contentHeight) => Math.Max(MinThumbHeight, visibleHeight * visibleHeight / contentHeight);

        /// <summary>
        /// Converts a rectangle in UI coordinates to screen pixels for the scissor test, clamped to the viewport.
        /// </summary>
        internal static Rectangle ToScreenRectangle(Rectangle uiRectangle, Rectangle viewport)
        {
            Vector2 topLeft = Vector2.Transform(new Vector2(uiRectangle.X, uiRectangle.Y), Main.UIScaleMatrix);
            Vector2 bottomRight = Vector2.Transform(new Vector2(uiRectangle.Right, uiRectangle.Bottom), Main.UIScaleMatrix);
            return Rectangle.Intersect(new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)(bottomRight.X - topLeft.X), (int)(bottomRight.Y - topLeft.Y)), viewport);
        }

        private static int Rows(int count, int columns) => (count + columns - 1) / columns;

        private static int EffectHeight(int valueLines) => Math.Max(EffectIconSize, EffectTitleHeight + valueLines * EffectLineHeight) + Gap * 2;

        private static int PopupTop() => Main.mapEnabled && Main.mapStyle == 1 ? Main.miniMapY + Main.miniMapHeight + 30 : 120;

        private static object[] EffectArgs(CollectiblePlayer player, CollectibleEffect effect, float magnitude) => effect switch
        {
            CollectibleEffect.DecreaseSourceScaling => new object[] { player.DecreasingSourceCount, Format(player.DecreaseSourceDamageBonus), Format(player.DecreaseSourceCritBonus) },
            CollectibleEffect.ExtraCoinsOnHit or CollectibleEffect.ShopHealDiscount or CollectibleEffect.ShopMarkup => new object[] { Format(magnitude * 100f) },
            _ => new object[] { Format(magnitude) },
        };

        private static int SourceItem(CollectiblePlayer player, CollectibleEffect effect)
        {
            foreach (int type in player.Collected.Keys)
            {
                if (ContentSamples.ItemsByType[type].ModItem is CollectibleItem item && item.Describes(effect))
                    return type;
            }
            return 0;
        }

        private static string Format(float value) => value.ToString("0.##");

        private static int DrawHeader(SpriteBatch spriteBatch, string section, string rightText, int x, int y, ref bool first, float opacity)
        {
            if (!first)
                y += SectionGap;
            first = false;

            string header = Language.GetTextValue(LocalizationPath.UI + "RunStats." + section).ToUpperInvariant();
            DrawText(spriteBatch, header, new Vector2(x, y), HeaderColor * opacity, HeaderScale);

            if (rightText != null)
                DrawText(spriteBatch, rightText, new Vector2(x + ContentWidth - TextWidth(rightText, NameScale), y + 2), CountColor * opacity, NameScale);

            return y + HeaderHeight + HeaderGap;
        }

        private static float TextWidth(string text, float scale) => FontAssets.MouseText.Value.MeasureString(text).X * scale;

        private static void DrawTextCentered(SpriteBatch spriteBatch, string text, float x, float centerY, Color color, float scale)
        {
            float height = FontAssets.MouseText.Value.MeasureString(text).Y * scale;
            DrawText(spriteBatch, text, new Vector2(x, centerY - height / 2f + 3f * scale), color, scale);
        }

        private static void DrawText(SpriteBatch spriteBatch, string text, Vector2 position, Color color, float scale)
        {
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, position, color, 0f, Vector2.Zero, new Vector2(scale));
        }

        internal static void DrawItemIcon(SpriteBatch spriteBatch, int type, Vector2 center, float maxSize, Color color)
        {
            Main.instance.LoadItem(type);
            Texture2D texture = TextureAssets.Item[type].Value;
            Rectangle frame = Main.itemAnimations[type] != null ? Main.itemAnimations[type].GetFrame(texture) : texture.Frame();
            float scale = Math.Min(1f, maxSize / Math.Max(frame.Width, frame.Height));
            spriteBatch.Draw(texture, center, frame, color, 0f, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
        }
    }
}
