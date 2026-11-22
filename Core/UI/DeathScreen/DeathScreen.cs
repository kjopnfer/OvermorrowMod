using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Items;
using OvermorrowMod.Content.Items.Archives;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Items.Collectibles;
using OvermorrowMod.Core.Loot;
using OvermorrowMod.Core.Runs;
using OvermorrowMod.Core.UI.ClassSelection;
using OvermorrowMod.Core.UI.MonkeyStones;
using OvermorrowMod.Core.UI.RunStats;
using ReLogic.Graphics;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace OvermorrowMod.Core.UI.DeathScreen
{
    public class DeathScreen : UIState
    {
        private static readonly float ShowDelay = ModUtils.SecondsToTicks(0.6f);
        private static readonly float BackdropFade = ModUtils.SecondsToTicks(0.4f);
        private static readonly float FadeDuration = ModUtils.SecondsToTicks(0.22f);
        private static readonly float CountDuration = ModUtils.SecondsToTicks(0.42f);
        private static readonly float Beat = ModUtils.SecondsToTicks(0.09f);
        private static readonly float HeaderPause = ModUtils.SecondsToTicks(0.2f);
        private const int TickSoundInterval = 3;
        private const int ReturnClickDelay = 10;
        private const float RiseDistance = 6f;

        private static readonly SoundStyle CountSound = SoundID.MenuTick with { Volume = 0.35f, MaxInstances = 3 };
        private static readonly SoundStyle SkipSound = SoundID.MenuTick with { Volume = 0.7f };

        private const int PanelWidth = 760;
        private const int Padding = 28;
        private const int ScreenMargin = 30;
        private const int TitleHeight = 60;
        private const int MessageLineHeight = 28;
        private const int BiomeHeight = 26;
        private const int SectionGap = 18;
        private const int HeadingHeight = 26;
        private const int CellHeight = 34;
        private const int CellPadding = 10;
        private const int Gap = 5;
        private const int ColumnGap = 14;
        private const int StoneIconSize = 24;
        private const int SlotSize = 46;
        private const int ButtonWidth = 180;
        private const int ButtonHeight = 42;
        private const float TitleScale = 1f;
        private const float MessageScale = 1f;
        private const float BiomeScale = 0.85f;
        private const float HeadingScale = 0.95f;
        private const float NameScale = 0.9f;
        private const float ValueScale = 1f;
        private const float SmallScale = 0.8f;
        private const float LostOpacity = 0.8f;

        private const int BandStripWidth = 10;
        private const float BandReach = 1.15f;

        private static readonly Color BackdropColor = Color.Black * 0.6f;
        private static readonly Color SilverColor = new(163, 167, 194);
        private static readonly Color LightSilverColor = new(204, 208, 233);
        private static readonly Color DarkSilverColor = new(106, 118, 153);
        private static readonly Color GoldColor = new(214, 182, 84);
        private static readonly Color GainColor = new(80, 220, 90);
        private static readonly Color SlotColor = Color.Black * 0.45f;

        private static readonly RasterizerState ClipRasterizer = new() { CullMode = CullMode.None, ScissorTestEnable = true };

        private enum Kind { Header, Ledger, Stone, NoStones, Lost, Button }

        private class Entry
        {
            public Kind Kind;
            public string Label;
            public int Value;
            public bool Counts;
            public Func<int, string> Format = value => value.ToString();
            public Color ValueColor = Color.White;
            public int IconType;
            public string Banked;
            public int Column;
            public string Heading;
            public float Start;
            public float Length;
            public int Y;
            public int HeadingY;
            public int Height;
            public int LastShown;
        }

        public static bool Visible { get; private set; }

        private bool abandoned;
        private string title;
        private string[] messageLines = [];
        private string biome;
        private readonly List<Entry> entries = new();
        private readonly List<(Item Item, int Count)> lost = new();
        private string coins;
        private float elapsed;
        private float timelineEnd;
        private float returnShownAt = -1f;
        private int tickCooldown;
        private float scrollOffset;

        private static float TimelineStart => ShowDelay + BackdropFade;

        private static int ContentWidth => PanelWidth - Padding * 2;

        private static int ColumnWidth => (ContentWidth - ColumnGap) / 2;

        private static int SlotColumns => Math.Max(1, (ContentWidth + Gap) / (SlotSize + Gap));

        private static string Text(string key, params object[] args) => Language.GetTextValue(LocalizationPath.Localization + "DeathScreen." + key, args);

        public void ShowDeath(Player player)
        {
            var death = player.GetModPlayer<RunDeathPlayer>();
            Item[] gear = player.inventory.Concat(player.armor).Concat(player.miscEquips).Append(Main.mouseItem).ToArray();
            Build(player, false, death.DeathText, death.BiomeName, gear);
        }

        public void ShowAbandoned(Player player)
        {
            var stored = player.GetModPlayer<SubworldPlayer>();
            Item[] gear = (stored.subInventory ?? []).Concat(stored.subArmor ?? []).Concat(stored.subMisc ?? []).Where(item => item != null).ToArray();
            Build(player, true, null, player.GetModPlayer<RunSavePlayer>().BiomeName, gear);
        }

        public void Hide() => Visible = false;

        private void Build(Player player, bool abandon, string deathText, string biomeName, Item[] gear)
        {
            abandoned = abandon;
            title = Text(abandon ? "RunAbandoned" : "RunOver");
            biome = biomeName ?? "";
            int wrapWidth = (int)(ContentWidth / MessageScale);
            messageLines = string.IsNullOrEmpty(deathText) ? [] : Utils.WordwrapString(deathText, FontAssets.MouseText.Value, wrapWidth, 4, out _).Where(line => !string.IsNullOrEmpty(line)).ToArray();

            var record = player.GetModPlayer<RunRecordPlayer>();
            var stones = player.GetModPlayer<MonkeyStonePlayer>();

            entries.Clear();
            entries.Add(new Entry { Kind = Kind.Header });

            entries.Add(Ledger(Text("Time"), record.RunTicks, 0, Text("Summary"), FormatTime));
            string[] killKeys = ["Enemies", "Elites", "Minibosses", "Bosses"];
            for (int i = 0; i < killKeys.Length; i++)
                entries.Add(Ledger(Text(killKeys[i]), record.KillsByTier[i], 0, null));
            entries.Add(Ledger(Text("RoomsCleared"), record.RoomsCleared, 0, null));

            foreach (Rarity rarity in Enum.GetValues<Rarity>())
            {
                Entry line = Ledger(Text("Rarity." + rarity), record.ItemsByRarity[(int)rarity], 1, rarity == Rarity.Common ? Text("ItemsFound") : null);
                line.ValueColor = RarityColors.For(rarity);
                entries.Add(line);
            }

            string kept = Text("Kept");
            foreach (MonkeyStoneTier tier in Enum.GetValues<MonkeyStoneTier>())
            {
                int gained = stones.GetGainedThisRun(tier);
                if (gained <= 0) continue;

                entries.Add(new Entry
                {
                    Kind = Kind.Stone,
                    Label = Lang.GetItemNameValue(MonkeyStoneDisplay.StoneType(tier)),
                    Value = gained,
                    Counts = true,
                    Format = value => "+" + value,
                    ValueColor = GainColor,
                    IconType = MonkeyStoneDisplay.StoneType(tier),
                    Banked = Text("Banked", stones.Get(tier)),
                    Heading = entries.Any(entry => entry.Heading == kept) ? null : kept,
                });
            }
            if (!entries.Any(entry => entry.Heading == kept))
                entries.Add(new Entry { Kind = Kind.NoStones, Label = Text("NoStones"), Heading = kept });

            lost.Clear();
            int lockedType = ModContent.ItemType<LockedSlot>();
            foreach (Item item in gear)
            {
                if (item == null || item.IsAir || item.IsACoin || item.type == lockedType || RunCountedItem.IsKit(item)) continue;
                lost.Add((item.Clone(), item.stack));
            }
            foreach (var (type, count) in player.GetModPlayer<CollectiblePlayer>().Collected)
                lost.Add((ContentSamples.ItemsByType[type].Clone(), count));
            long coinValue = Utils.CoinsCount(out _, gear.Where(item => item != null && item.IsACoin).ToArray());
            coins = coinValue > 0 ? Main.ValueToCoins(coinValue) : null;
            entries.Add(new Entry { Kind = Kind.Lost, Heading = Text("Lost") });

            entries.Add(new Entry { Kind = Kind.Button });

            float time = TimelineStart;
            foreach (Entry entry in entries)
            {
                entry.Start = time;
                entry.Length = entry.Counts && entry.Value > 0 ? Math.Max(FadeDuration, CountDuration) : FadeDuration;
                time += entry.Length + Beat + (entry.Kind == Kind.Header ? HeaderPause : 0f);
            }
            timelineEnd = entries[^1].Start;

            elapsed = 0f;
            returnShownAt = -1f;
            tickCooldown = 0;
            scrollOffset = 0f;
            Visible = true;
        }

        private static Entry Ledger(string label, int value, int column, string heading, Func<int, string> format = null) => new()
        {
            Kind = Kind.Ledger,
            Label = label,
            Value = value,
            Counts = true,
            Format = format ?? (number => number.ToString()),
            Column = column,
            Heading = heading,
        };

        private static string FormatTime(int ticks)
        {
            TimeSpan time = TimeSpan.FromSeconds(ticks / 60);
            return time.TotalHours >= 1 ? $"{(int)time.TotalHours}:{time.Minutes:00}:{time.Seconds:00}" : $"{time.Minutes}:{time.Seconds:00}";
        }

        private bool Finished => elapsed >= timelineEnd;

        public override void Update(GameTime gameTime)
        {
            if (!Visible) return;

            Main.LocalPlayer.mouseInterface = true;
            elapsed++;

            if (!Finished && elapsed > TimelineStart && Main.mouseLeft && Main.mouseLeftRelease)
            {
                elapsed = timelineEnd + FadeDuration;
                Main.mouseLeftRelease = false;
                SoundEngine.PlaySound(SkipSound);
            }

            if (Finished && returnShownAt < 0f)
                returnShownAt = elapsed;

            PlayCountTicks();
        }

        private void PlayCountTicks()
        {
            if (tickCooldown > 0) tickCooldown--;

            foreach (Entry entry in entries)
            {
                if (!entry.Counts) continue;

                int shown = Shown(entry);
                if (shown == entry.LastShown) continue;

                entry.LastShown = shown;
                if (Finished || tickCooldown > 0) continue;

                SoundEngine.PlaySound(CountSound);
                tickCooldown = TickSoundInterval;
            }
        }

        private float Appear(Entry entry, float delay = 0f) => EaseOutCubic(MathHelper.Clamp((elapsed - entry.Start - delay) / FadeDuration, 0f, 1f));

        private int Shown(Entry entry)
        {
            if (!entry.Counts || entry.Value <= 0) return elapsed >= entry.Start ? entry.Value : 0;

            float progress = MathHelper.Clamp((elapsed - entry.Start) / CountDuration, 0f, 1f);
            return (int)MathF.Round(entry.Value * EaseOutCubic(progress));
        }

        private static float EaseOutCubic(float t) => 1f - MathF.Pow(1f - t, 3f);

        private int Layout()
        {
            int y = 0;
            int[] columnY = new int[2];
            int sectionTop = 0;

            foreach (Entry entry in entries)
            {
                switch (entry.Kind)
                {
                    case Kind.Header:
                        entry.Y = y;
                        entry.Height = TitleHeight + messageLines.Length * MessageLineHeight + (biome.Length > 0 ? BiomeHeight : 0);
                        y += entry.Height + SectionGap;
                        sectionTop = y;
                        columnY[0] = columnY[1] = y + HeadingHeight;
                        break;

                    case Kind.Ledger:
                        entry.HeadingY = sectionTop;
                        entry.Y = columnY[entry.Column];
                        entry.Height = CellHeight;
                        columnY[entry.Column] += CellHeight + Gap;
                        y = Math.Max(columnY[0], columnY[1]) - Gap;
                        break;

                    case Kind.Stone:
                    case Kind.NoStones:
                        if (entry.Heading != null)
                        {
                            y += SectionGap;
                            entry.HeadingY = y;
                            y += HeadingHeight;
                        }
                        else
                        {
                            y += Gap;
                        }
                        entry.Y = y;
                        entry.Height = CellHeight;
                        y += CellHeight;
                        break;

                    case Kind.Lost:
                        y += SectionGap;
                        entry.HeadingY = y;
                        y += HeadingHeight;
                        entry.Y = y;
                        int rows = (lost.Count + SlotColumns - 1) / SlotColumns;
                        entry.Height = (coins != null ? CellHeight + Gap : 0) + rows * (SlotSize + Gap) - (rows > 0 ? Gap : 0);
                        if (lost.Count == 0 && coins == null) entry.Height = CellHeight;
                        y += entry.Height;
                        break;

                    case Kind.Button:
                        y += SectionGap + 6;
                        entry.Y = y;
                        entry.Height = ButtonHeight;
                        y += ButtonHeight;
                        break;
                }
            }
            return y;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible) return;

            float backdrop = MathHelper.Clamp((elapsed - ShowDelay) / BackdropFade, 0f, 1f);
            if (backdrop <= 0f) return;

            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), BackdropColor * backdrop);

            int contentHeight = Layout();
            int panelHeight = Math.Min(contentHeight + Padding * 2, Main.screenHeight - ScreenMargin * 2);
            Rectangle panel = new((Main.screenWidth - PanelWidth) / 2, (Main.screenHeight - panelHeight) / 2, PanelWidth, panelHeight);
            Rectangle clip = new(panel.X, panel.Y + Padding, PanelWidth, panelHeight - Padding * 2);

            DrawBand(spriteBatch, panel, backdrop);
            DrawBanner(spriteBatch, panel.Center.X, panel.Y, PanelWidth / 2 - 40, backdrop);
            DrawBanner(spriteBatch, panel.Center.X, panel.Bottom, PanelWidth / 2 - 40, backdrop);

            UpdateScroll(contentHeight, clip);

            RasterizerState previousRasterizer = spriteBatch.GraphicsDevice.RasterizerState;
            Rectangle previousScissor = spriteBatch.GraphicsDevice.ScissorRectangle;
            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = RunStatsPanel.ToScreenRectangle(clip, spriteBatch.GraphicsDevice.Viewport.Bounds);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, ClipRasterizer, null, Main.UIScaleMatrix);

            int x = panel.X + Padding;
            int top = clip.Y - (int)scrollOffset;
            foreach (Entry entry in entries)
            {
                float alpha = Appear(entry);
                if (alpha <= 0f) continue;

                int y = top + entry.Y + (int)(RiseDistance * (1f - alpha));
                int columnX = x + entry.Column * (ColumnWidth + ColumnGap);
                if (entry.Heading != null)
                    DrawText(spriteBatch, entry.Heading.ToUpperInvariant(), new Vector2(columnX, top + entry.HeadingY), GoldColor * alpha, HeadingScale);

                switch (entry.Kind)
                {
                    case Kind.Header: DrawHeader(spriteBatch, panel.Center.X, y, alpha); break;
                    case Kind.Ledger: DrawLedger(spriteBatch, entry, new Rectangle(columnX, y, ColumnWidth, CellHeight), alpha); break;
                    case Kind.Stone: DrawStone(spriteBatch, entry, new Rectangle(x, y, ContentWidth, CellHeight), alpha); break;
                    case Kind.NoStones: DrawTextCentered(spriteBatch, entry.Label, x, y + CellHeight / 2f, SilverColor * alpha, NameScale); break;
                    case Kind.Lost: DrawLost(spriteBatch, x, y, alpha * LostOpacity, clip); break;
                }
            }

            Entry button = entries[^1];
            Rectangle buttonRect = new(panel.Center.X - ButtonWidth / 2, Math.Min(top + button.Y, clip.Bottom - ButtonHeight), ButtonWidth, ButtonHeight);

            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = previousScissor;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, previousRasterizer, null, Main.UIScaleMatrix);

            if (Finished) DrawReturn(spriteBatch, buttonRect, Appear(button));
            else if (elapsed > TimelineStart) DrawCentered(spriteBatch, Text("ClickToSkip"), buttonRect.Center.ToVector2(), DarkSilverColor * backdrop, SmallScale);
        }

        private void UpdateScroll(int contentHeight, Rectangle clip)
        {
            int maxScroll = Math.Max(0, contentHeight - clip.Height);
            if (maxScroll == 0)
            {
                scrollOffset = 0f;
                return;
            }

            if (!Finished)
            {
                Entry newest = entries.LastOrDefault(entry => elapsed >= entry.Start) ?? entries[0];
                scrollOffset = Math.Max(scrollOffset, newest.Y + newest.Height - clip.Height);
            }
            else
            {
                PlayerInput.LockVanillaMouseScroll("OvermorrowMod/DeathScreen");
                scrollOffset -= PlayerInput.ScrollWheelDeltaForUI / 120f * CellHeight;
            }

            scrollOffset = MathHelper.Clamp(scrollOffset, 0f, maxScroll);
        }

        private void DrawHeader(SpriteBatch spriteBatch, int centerX, int y, float alpha)
        {
            DynamicSpriteFont titleFont = FontAssets.DeathText.Value;
            Vector2 titleSize = titleFont.MeasureString(title) * TitleScale;
            Vector2 titlePosition = new(centerX - titleSize.X / 2f, y + (TitleHeight - titleSize.Y) / 2f);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, titleFont, title, titlePosition, Color.White * alpha, 0f, Vector2.Zero, new Vector2(TitleScale));

            int lineY = y + TitleHeight;
            foreach (string line in messageLines)
            {
                DrawCentered(spriteBatch, line, new Vector2(centerX, lineY + MessageLineHeight / 2f), LightSilverColor * alpha, MessageScale);
                lineY += MessageLineHeight;
            }

            if (biome.Length > 0)
                DrawCentered(spriteBatch, biome, new Vector2(centerX, lineY + BiomeHeight / 2f), GoldColor * alpha, BiomeScale);
        }

        private void DrawLedger(SpriteBatch spriteBatch, Entry entry, Rectangle cell, float alpha)
        {
            DrawTextCentered(spriteBatch, entry.Label, cell.X + CellPadding, cell.Center.Y, SilverColor * alpha, NameScale);

            string value = entry.Format(Shown(entry));
            DrawTextCentered(spriteBatch, value, cell.Right - CellPadding - TextWidth(value, ValueScale), cell.Center.Y, entry.ValueColor * alpha, ValueScale);
        }

        private void DrawStone(SpriteBatch spriteBatch, Entry entry, Rectangle cell, float alpha)
        {
            RunStatsPanel.DrawItemIcon(spriteBatch, entry.IconType, new Vector2(cell.X + CellPadding + StoneIconSize / 2f, cell.Center.Y), StoneIconSize, Color.White * alpha);
            DrawTextCentered(spriteBatch, entry.Label, cell.X + CellPadding * 2 + StoneIconSize, cell.Center.Y, SilverColor * alpha, NameScale);

            float bankedAlpha = Appear(entry, entry.Length);
            float bankedWidth = TextWidth(entry.Banked, SmallScale);
            if (bankedAlpha > 0f)
                DrawTextCentered(spriteBatch, entry.Banked, cell.Right - CellPadding - bankedWidth, cell.Center.Y, DarkSilverColor * bankedAlpha, SmallScale);

            string value = entry.Format(Shown(entry));
            float valueRight = cell.Right - CellPadding * 3 - bankedWidth;
            DrawTextCentered(spriteBatch, value, valueRight - TextWidth(value, ValueScale), cell.Center.Y, entry.ValueColor * alpha, ValueScale);
        }

        private void DrawLost(SpriteBatch spriteBatch, int x, int y, float alpha, Rectangle clip)
        {
            if (lost.Count == 0 && coins == null)
            {
                DrawTextCentered(spriteBatch, Text("NothingLost"), x, y + CellHeight / 2f, SilverColor * alpha, NameScale);
                return;
            }

            if (coins != null)
            {
                Rectangle cell = new(x, y, ContentWidth, CellHeight);
                DrawTextCentered(spriteBatch, Text("Coins"), cell.X + CellPadding, cell.Center.Y, SilverColor * alpha, NameScale);
                DrawTextCentered(spriteBatch, coins, cell.Right - CellPadding - TextWidth(coins, ValueScale), cell.Center.Y, Color.White * alpha, ValueScale);
                y += CellHeight + Gap;
            }

            bool canHover = Finished && clip.Contains(Main.MouseScreen.ToPoint());
            for (int i = 0; i < lost.Count; i++)
            {
                var (item, count) = lost[i];
                Rectangle slot = new(x + i % SlotColumns * (SlotSize + Gap), y + i / SlotColumns * (SlotSize + Gap), SlotSize, SlotSize);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, slot, SlotColor * alpha);
                DrawBorder(spriteBatch, slot, DarkSilverColor * alpha);
                RunStatsPanel.DrawItemIcon(spriteBatch, item.type, slot.Center.ToVector2(), 32, Color.White * alpha);

                if (count > 1)
                {
                    string amount = count.ToString();
                    DrawText(spriteBatch, amount, new Vector2(slot.Right - 4 - TextWidth(amount, SmallScale), slot.Bottom - 20), Color.White * alpha, SmallScale);
                }

                if (canHover && slot.Contains(Main.MouseScreen.ToPoint()))
                {
                    Main.HoverItem = item.Clone();
                    Main.hoverItemName = item.Name;
                }
            }
        }

        private void DrawReturn(SpriteBatch spriteBatch, Rectangle bounds, float alpha)
        {
            bool hovered = bounds.Contains(Main.MouseScreen.ToPoint());
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, bounds, SlotColor * alpha);
            DrawBorder(spriteBatch, bounds, (hovered ? LightSilverColor : DarkSilverColor) * alpha);
            DrawCentered(spriteBatch, Text("Return"), bounds.Center.ToVector2(), (hovered ? Color.White : GoldColor) * alpha, ValueScale);

            if (hovered && elapsed - returnShownAt >= ReturnClickDelay && Main.mouseLeft && Main.mouseLeftRelease)
            {
                Main.mouseLeftRelease = false;
                SoundEngine.PlaySound(SoundID.MenuClose);
                Return();
            }
        }

        private void Return()
        {
            Hide();
            if (abandoned)
            {
                RunReset.Reset(Main.LocalPlayer);
                ClassSelectionManager.Show();
            }
            else
            {
                SubworldSystem.Exit();
            }
        }

        private static void DrawBand(SpriteBatch spriteBatch, Rectangle panel, float alpha)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            int reach = (int)(panel.Width / 2f * BandReach);
            for (int x = 0; x < reach; x += BandStripWidth)
            {
                float stripAlpha = (1f - EasingUtils.EaseInCubic(x / (float)reach)) * alpha;
                spriteBatch.Draw(pixel, new Rectangle(panel.Center.X - x - BandStripWidth, panel.Y, BandStripWidth, panel.Height), Color.Black * stripAlpha);
                spriteBatch.Draw(pixel, new Rectangle(panel.Center.X + x, panel.Y, BandStripWidth, panel.Height), Color.Black * stripAlpha);
            }
        }

        private static void DrawBanner(SpriteBatch spriteBatch, int centerX, int y, int halfWidth, float alpha)
        {
            Texture2D left = ModContent.Request<Texture2D>(AssetDirectory.UI + "TitleCardLeftSegment").Value;
            Texture2D right = ModContent.Request<Texture2D>(AssetDirectory.UI + "TitleCardRightSegment").Value;
            Texture2D leftArrow = ModContent.Request<Texture2D>(AssetDirectory.UI + "TitleCardLeftArrow").Value;
            Texture2D rightArrow = ModContent.Request<Texture2D>(AssetDirectory.UI + "TitleCardRightArrow").Value;
            Texture2D middle = ModContent.Request<Texture2D>(AssetDirectory.UI + "TitleCardMiddle").Value;
            Color color = Color.White * alpha;

            for (int i = 0; i < halfWidth; i++)
            {
                spriteBatch.Draw(left, new Vector2(centerX - i, y), null, color, 0f, left.Size() / 2f, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(right, new Vector2(centerX + i, y), null, color, 0f, right.Size() / 2f, 1f, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(leftArrow, new Vector2(centerX - halfWidth - leftArrow.Width / 2f, y), null, color, 0f, leftArrow.Size() / 2f, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(rightArrow, new Vector2(centerX + halfWidth + rightArrow.Width / 2f, y), null, color, 0f, rightArrow.Size() / 2f, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(middle, new Vector2(centerX, y), null, color, 0f, middle.Size() / 2f, 1f, SpriteEffects.None, 0f);
        }

        private static void DrawBorder(SpriteBatch spriteBatch, Rectangle rect, Color color)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, 1), color);
            spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Bottom - 1, rect.Width, 1), color);
            spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, 1, rect.Height), color);
            spriteBatch.Draw(pixel, new Rectangle(rect.Right - 1, rect.Y, 1, rect.Height), color);
        }

        private static float TextWidth(string text, float scale) => FontAssets.MouseText.Value.MeasureString(text).X * scale;

        private static void DrawText(SpriteBatch spriteBatch, string text, Vector2 position, Color color, float scale)
        {
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, position, color, 0f, Vector2.Zero, new Vector2(scale));
        }

        private static void DrawTextCentered(SpriteBatch spriteBatch, string text, float x, float centerY, Color color, float scale)
        {
            float height = FontAssets.MouseText.Value.MeasureString(text).Y * scale;
            DrawText(spriteBatch, text, new Vector2(x, centerY - height / 2f + 3f * scale), color, scale);
        }

        private static void DrawCentered(SpriteBatch spriteBatch, string text, Vector2 center, Color color, float scale)
        {
            Vector2 size = FontAssets.MouseText.Value.MeasureString(text) * scale;
            DrawText(spriteBatch, text, new Vector2(center.X - size.X / 2f, center.Y - size.Y / 2f + 3f * scale), color, scale);
        }
    }
}
