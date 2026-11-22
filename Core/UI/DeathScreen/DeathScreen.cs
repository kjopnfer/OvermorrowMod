using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        private const int PanelWidth = 580;
        private const int Padding = 32;
        private const int ScreenMargin = 30;
        private const int TitleHeight = 46;
        private const int MessageLineHeight = 24;
        private const int BiomeHeight = 22;
        private const int SectionGap = 24;
        private const int HeadingHeight = 24;
        private const int LineHeight = 26;
        private const int ColumnGap = 36;
        private const int SlotSize = 36;
        private const int SlotGap = 4;
        private const int ButtonWidth = 150;
        private const int ButtonHeight = 36;
        private const int BankedWidth = 110;
        private const float TitleScale = 0.75f;
        private const float MessageScale = 0.9f;
        private const float BiomeScale = 0.75f;
        private const float HeadingScale = 0.7f;
        private const float LineScale = 0.85f;
        private const float SmallScale = 0.7f;
        private const float HeadingSpacing = 3f;
        private const float LeftBehindOpacity = 0.55f;

        private static readonly Color BackdropColor = Color.Black * 0.7f;
        private static readonly Color PanelColor = new Color(0x10, 0x11, 0x16) * 0.94f;
        private static readonly Color BorderColor = new(0x2c, 0x2b, 0x30);
        private static readonly Color TitleColor = new(0xe6, 0xdc, 0xcb);
        private static readonly Color MessageColor = new(0xa8, 0x9f, 0x90);
        private static readonly Color MutedColor = new(0x8a, 0x84, 0x78);
        private static readonly Color LeaderColor = new(0x3a, 0x38, 0x33);
        private static readonly Color KeptColor = new(0x7f, 0xa0, 0x7a);
        private static readonly Color LostColor = new(0xb0, 0x6a, 0x62);
        private static readonly Color ButtonColor = new(0x6b, 0x64, 0x58);
        private static readonly Color[] RarityColors = [new(0xc4, 0xbc, 0xae), new(0x7f, 0xa9, 0xd9), new(0xb3, 0x91, 0xd6), new(0xd9, 0xa8, 0x5f)];

        private static readonly RasterizerState ClipRasterizer = new() { CullMode = CullMode.None, ScissorTestEnable = true };

        private enum Kind { Header, Ledger, Stone, NoStones, LeftBehind, Button }

        private class Entry
        {
            public Kind Kind;
            public string Label;
            public int Value;
            public bool Counts;
            public Func<int, string> Format = value => value.ToString();
            public Color ValueColor;
            public Color DiamondColor;
            public string Banked;
            public int Column;
            public string Heading;
            public Color HeadingColor;
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
        private readonly List<(Item Item, int Count)> leftBehind = new();
        private string coins;
        private float elapsed;
        private float timelineEnd;
        private float returnShownAt = -1f;
        private int tickCooldown;
        private float scrollOffset;
        private GamePadState oldGamePad;

        private static float TimelineStart => ShowDelay + BackdropFade;

        private static int ContentWidth => PanelWidth - Padding * 2;

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

            string theRun = Text("TheRun");
            entries.Add(Ledger(Text("Time"), record.RunTicks, 0, theRun, MutedColor, FormatTime));
            string[] killKeys = ["Enemies", "Elites", "Minibosses", "Bosses"];
            for (int i = 0; i < killKeys.Length; i++)
                entries.Add(Ledger(Text(killKeys[i]), record.KillsByTier[i], 0, null, MutedColor));
            entries.Add(Ledger(Text("RoomsCleared"), record.RoomsCleared, 0, null, MutedColor));

            foreach (Rarity rarity in Enum.GetValues<Rarity>())
            {
                Entry line = Ledger(Text("Rarity." + rarity), record.ItemsByRarity[(int)rarity], 1, rarity == Rarity.Common ? Text("ItemsFound") : null, MutedColor);
                line.ValueColor = RarityColors[(int)rarity];
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
                    ValueColor = KeptColor,
                    DiamondColor = StoneColor(tier),
                    Banked = Text("Banked", stones.Get(tier)),
                    Heading = entries.Any(entry => entry.Heading == kept) ? null : kept,
                    HeadingColor = KeptColor,
                });
            }
            if (!entries.Any(entry => entry.Heading == kept))
                entries.Add(new Entry { Kind = Kind.NoStones, Label = Text("NoStones"), Heading = kept, HeadingColor = KeptColor });

            leftBehind.Clear();
            int lockedType = ModContent.ItemType<LockedSlot>();
            foreach (Item item in gear)
            {
                if (item == null || item.IsAir || item.IsACoin || item.type == lockedType || RunCountedItem.IsKit(item)) continue;
                leftBehind.Add((item.Clone(), item.stack));
            }
            foreach (var (type, count) in player.GetModPlayer<CollectiblePlayer>().Collected)
                leftBehind.Add((ContentSamples.ItemsByType[type].Clone(), count));
            long coinValue = Utils.CoinsCount(out _, gear.Where(item => item != null && item.IsACoin).ToArray());
            coins = coinValue > 0 ? Main.ValueToCoins(coinValue) : null;
            entries.Add(new Entry { Kind = Kind.LeftBehind, Heading = Text("LeftBehind"), HeadingColor = LostColor });

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
            oldGamePad = GamePad.GetState(PlayerIndex.One);
            Visible = true;
        }

        private static Entry Ledger(string label, int value, int column, string heading, Color headingColor, Func<int, string> format = null) => new()
        {
            Kind = Kind.Ledger,
            Label = label,
            Value = value,
            Counts = true,
            Format = format ?? (number => number.ToString()),
            ValueColor = TitleColor,
            Column = column,
            Heading = heading,
            HeadingColor = headingColor,
        };

        private static string FormatTime(int ticks)
        {
            TimeSpan time = TimeSpan.FromSeconds(ticks / 60);
            return time.TotalHours >= 1 ? $"{(int)time.TotalHours}:{time.Minutes:00}:{time.Seconds:00}" : $"{time.Minutes}:{time.Seconds:00}";
        }

        private static Color StoneColor(MonkeyStoneTier tier) => tier switch
        {
            MonkeyStoneTier.Green => new Color(0x7f, 0xb0, 0x7a),
            MonkeyStoneTier.Blue => new Color(0x7f, 0xa9, 0xd9),
            MonkeyStoneTier.Orange => new Color(0xd9, 0xa8, 0x5f),
            _ => new Color(0xb3, 0x91, 0xd6),
        };

        private bool Finished => elapsed >= timelineEnd;

        public override void Update(GameTime gameTime)
        {
            if (!Visible) return;

            Main.LocalPlayer.mouseInterface = true;
            elapsed++;

            if (!Finished && elapsed > TimelineStart && SkipPressed())
            {
                elapsed = timelineEnd + FadeDuration;
                Main.mouseLeftRelease = false;
                SoundEngine.PlaySound(SkipSound);
            }

            if (Finished && returnShownAt < 0f)
                returnShownAt = elapsed;

            PlayCountTicks();
            oldGamePad = GamePad.GetState(PlayerIndex.One);
        }

        private bool SkipPressed()
        {
            if (Main.mouseLeft && Main.mouseLeftRelease) return true;
            if (Main.keyState.GetPressedKeys().Any(key => !Main.oldKeyState.IsKeyDown(key))) return true;
            if (!PlayerInput.UsingGamepad) return false;

            GamePadState pad = GamePad.GetState(PlayerIndex.One);
            foreach (Buttons button in Enum.GetValues<Buttons>())
            {
                if (pad.IsButtonDown(button) && !oldGamePad.IsButtonDown(button)) return true;
            }
            return false;
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

        private float Appear(Entry entry) => EaseOutCubic(MathHelper.Clamp((elapsed - entry.Start) / FadeDuration, 0f, 1f));

        private float Appear(Entry entry, float delay) => EaseOutCubic(MathHelper.Clamp((elapsed - entry.Start - delay) / FadeDuration, 0f, 1f));

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
                        entry.Height = LineHeight;
                        columnY[entry.Column] += LineHeight;
                        y = Math.Max(columnY[0], columnY[1]);
                        break;

                    case Kind.Stone:
                    case Kind.NoStones:
                        if (entry.Heading != null)
                        {
                            y += SectionGap;
                            entry.HeadingY = y;
                            y += HeadingHeight;
                        }
                        entry.Y = y;
                        entry.Height = LineHeight;
                        y += LineHeight;
                        break;

                    case Kind.LeftBehind:
                        y += SectionGap;
                        entry.HeadingY = y;
                        y += HeadingHeight;
                        entry.Y = y;
                        int columns = Math.Max(1, (ContentWidth + SlotGap) / (SlotSize + SlotGap));
                        int rows = (leftBehind.Count + columns - 1) / columns;
                        entry.Height = rows * (SlotSize + SlotGap) + (coins != null ? LineHeight : 0);
                        if (leftBehind.Count == 0 && coins == null) entry.Height = LineHeight;
                        y += entry.Height;
                        break;

                    case Kind.Button:
                        y += SectionGap;
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

            Texture2D pixel = TextureAssets.MagicPixel.Value;
            float backdrop = MathHelper.Clamp((elapsed - ShowDelay) / BackdropFade, 0f, 1f);
            if (backdrop <= 0f) return;

            spriteBatch.Draw(pixel, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), BackdropColor * backdrop);

            int contentHeight = Layout();
            int panelHeight = Math.Min(contentHeight + Padding * 2, Main.screenHeight - ScreenMargin * 2);
            Rectangle panel = new((Main.screenWidth - PanelWidth) / 2, (Main.screenHeight - panelHeight) / 2, PanelWidth, panelHeight);
            Rectangle clip = new(panel.X, panel.Y + Padding, PanelWidth, panelHeight - Padding * 2);

            spriteBatch.Draw(pixel, panel, PanelColor * backdrop);
            DrawBorder(spriteBatch, panel, BorderColor * backdrop);

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
                if (entry.Heading != null)
                    DrawHeading(spriteBatch, entry.Heading, x + (entry.Kind == Kind.Ledger ? entry.Column * ColumnWidth() + entry.Column * ColumnGap : 0), top + entry.HeadingY, entry.HeadingColor * alpha);

                switch (entry.Kind)
                {
                    case Kind.Header: DrawHeader(spriteBatch, panel.Center.X, x, y, alpha); break;
                    case Kind.Ledger: DrawLedger(spriteBatch, entry, x + entry.Column * (ColumnWidth() + ColumnGap), y, ColumnWidth(), alpha); break;
                    case Kind.Stone: DrawStone(spriteBatch, entry, x, y, alpha); break;
                    case Kind.NoStones: DrawText(spriteBatch, entry.Label, new Vector2(x, y), MutedColor * alpha, LineScale); break;
                    case Kind.LeftBehind: DrawLeftBehind(spriteBatch, x, y, alpha * LeftBehindOpacity, clip); break;
                }
            }

            Entry button = entries[^1];
            Rectangle buttonRect = new(panel.Center.X - ButtonWidth / 2, top + button.Y, ButtonWidth, ButtonHeight);

            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = previousScissor;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, previousRasterizer, null, Main.UIScaleMatrix);

            if (Finished) DrawReturn(spriteBatch, buttonRect, Appear(button), clip);
            else if (elapsed > TimelineStart) DrawCentered(spriteBatch, Text("ClickToSkip"), new Vector2(panel.Center.X, Math.Min(buttonRect.Center.Y, clip.Bottom - ButtonHeight / 2)), MutedColor * 0.8f * backdrop, SmallScale);
        }

        private static int ColumnWidth() => (ContentWidth - ColumnGap) / 2;

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
                scrollOffset -= PlayerInput.ScrollWheelDeltaForUI / 120f * LineHeight;
            }

            scrollOffset = MathHelper.Clamp(scrollOffset, 0f, maxScroll);
        }

        private void DrawHeader(SpriteBatch spriteBatch, int centerX, int x, int y, float alpha)
        {
            DynamicSpriteFont titleFont = FontAssets.DeathText.Value;
            Vector2 titleSize = titleFont.MeasureString(title) * TitleScale;
            int titleY = y + (int)((TitleHeight - titleSize.Y) / 2f);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, titleFont, title, new Vector2(centerX - titleSize.X / 2f, titleY), TitleColor * alpha, 0f, Vector2.Zero, new Vector2(TitleScale));

            int ruleY = y + TitleHeight / 2;
            int ruleGap = 18;
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            int leftEnd = (int)(centerX - titleSize.X / 2f) - ruleGap;
            int rightStart = (int)(centerX + titleSize.X / 2f) + ruleGap;
            spriteBatch.Draw(pixel, new Rectangle(x, ruleY, leftEnd - x, 1), BorderColor * 1.6f * alpha);
            spriteBatch.Draw(pixel, new Rectangle(rightStart, ruleY, x + ContentWidth - rightStart, 1), BorderColor * 1.6f * alpha);

            int lineY = y + TitleHeight;
            foreach (string line in messageLines)
            {
                DrawCentered(spriteBatch, line, new Vector2(centerX, lineY + MessageLineHeight / 2f), MessageColor * alpha, MessageScale);
                lineY += MessageLineHeight;
            }

            if (biome.Length > 0)
                DrawCentered(spriteBatch, biome, new Vector2(centerX, lineY + BiomeHeight / 2f), MutedColor * alpha, BiomeScale);
        }

        private void DrawLedger(SpriteBatch spriteBatch, Entry entry, int x, int y, int width, float alpha)
        {
            string value = entry.Format(Shown(entry));
            float valueWidth = TextWidth(value, LineScale);
            float labelWidth = TextWidth(entry.Label, LineScale);

            DrawText(spriteBatch, entry.Label, new Vector2(x, y), MessageColor * alpha, LineScale);
            DrawLeader(spriteBatch, x + labelWidth + 6, x + width - valueWidth - 6, y, alpha);
            DrawText(spriteBatch, value, new Vector2(x + width - valueWidth, y), entry.ValueColor * alpha, LineScale);
        }

        private void DrawStone(SpriteBatch spriteBatch, Entry entry, int x, int y, float alpha)
        {
            DrawDiamond(spriteBatch, new Vector2(x + 5, y + LineHeight / 2f - 3), 7, entry.DiamondColor * alpha);

            int labelX = x + 18;
            string value = entry.Format(Shown(entry));
            int valueRight = x + ContentWidth - BankedWidth;
            float valueWidth = TextWidth(value, LineScale);
            float labelWidth = TextWidth(entry.Label, LineScale);

            DrawText(spriteBatch, entry.Label, new Vector2(labelX, y), MessageColor * alpha, LineScale);
            DrawLeader(spriteBatch, labelX + labelWidth + 6, valueRight - valueWidth - 6, y, alpha);
            DrawText(spriteBatch, value, new Vector2(valueRight - valueWidth, y), entry.ValueColor * alpha, LineScale);

            float bankedAlpha = Appear(entry, entry.Length);
            if (bankedAlpha > 0f)
            {
                float bankedWidth = TextWidth(entry.Banked, SmallScale);
                DrawText(spriteBatch, entry.Banked, new Vector2(x + ContentWidth - bankedWidth, y + 3), MutedColor * bankedAlpha, SmallScale);
            }
        }

        private void DrawLeftBehind(SpriteBatch spriteBatch, int x, int y, float alpha, Rectangle clip)
        {
            if (leftBehind.Count == 0 && coins == null)
            {
                DrawText(spriteBatch, Text("NothingLeft"), new Vector2(x, y), MutedColor * alpha, LineScale);
                return;
            }

            int columns = Math.Max(1, (ContentWidth + SlotGap) / (SlotSize + SlotGap));
            bool canHover = Finished && clip.Contains(Main.MouseScreen.ToPoint());
            for (int i = 0; i < leftBehind.Count; i++)
            {
                var (item, count) = leftBehind[i];
                Rectangle slot = new(x + i % columns * (SlotSize + SlotGap), y + i / columns * (SlotSize + SlotGap), SlotSize, SlotSize);
                DrawBorder(spriteBatch, slot, MutedColor * alpha);
                RunStatsPanel.DrawItemIcon(spriteBatch, item.type, slot.Center.ToVector2(), SlotSize - 10, Color.White * alpha);

                if (count > 1)
                {
                    string amount = count.ToString();
                    DrawText(spriteBatch, amount, new Vector2(slot.Right - 3 - TextWidth(amount, SmallScale), slot.Bottom - 16), TitleColor * alpha, SmallScale);
                }

                if (canHover && slot.Contains(Main.MouseScreen.ToPoint()))
                {
                    Main.HoverItem = item.Clone();
                    Main.hoverItemName = item.Name;
                }
            }

            if (coins != null)
            {
                int rows = (leftBehind.Count + columns - 1) / columns;
                int coinY = y + rows * (SlotSize + SlotGap);
                string label = Text("Coins");
                float labelWidth = TextWidth(label, LineScale);
                float valueWidth = TextWidth(coins, LineScale);
                DrawText(spriteBatch, label, new Vector2(x, coinY), MessageColor * alpha, LineScale);
                DrawLeader(spriteBatch, x + labelWidth + 6, x + ContentWidth - valueWidth - 6, coinY, alpha);
                DrawText(spriteBatch, coins, new Vector2(x + ContentWidth - valueWidth, coinY), TitleColor * alpha, LineScale);
            }
        }

        private void DrawReturn(SpriteBatch spriteBatch, Rectangle bounds, float alpha, Rectangle clip)
        {
            bounds.Y = Math.Min(bounds.Y, clip.Bottom - bounds.Height);
            bool hovered = bounds.Contains(Main.MouseScreen.ToPoint());
            DrawBorder(spriteBatch, bounds, (hovered ? TitleColor : ButtonColor) * alpha);
            DrawCentered(spriteBatch, Text("Return"), bounds.Center.ToVector2(), (hovered ? TitleColor : MessageColor) * alpha, LineScale);

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

        private static void DrawHeading(SpriteBatch spriteBatch, string text, int x, int y, Color color)
        {
            float cursor = x;
            foreach (char character in text.ToUpperInvariant())
            {
                string glyph = character.ToString();
                DrawText(spriteBatch, glyph, new Vector2(cursor, y), color, HeadingScale);
                cursor += TextWidth(glyph, HeadingScale) + HeadingSpacing;
            }
        }

        private static void DrawLeader(SpriteBatch spriteBatch, float from, float to, int y, float alpha)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            int dotY = y + (int)(LineHeight * 0.55f);
            for (float dotX = from; dotX <= to - 2; dotX += 5f)
                spriteBatch.Draw(pixel, new Rectangle((int)dotX, dotY, 2, 2), LeaderColor * alpha);
        }

        private static void DrawDiamond(SpriteBatch spriteBatch, Vector2 center, int size, Color color)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            spriteBatch.Draw(pixel, new Rectangle((int)center.X, (int)center.Y, size, size), new Rectangle(0, 0, 1, 1), color, MathHelper.PiOver4, new Vector2(0.5f), SpriteEffects.None, 0f);
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
            Utils.DrawBorderString(spriteBatch, text, position, color, scale);
        }

        private static void DrawCentered(SpriteBatch spriteBatch, string text, Vector2 center, Color color, float scale)
        {
            Utils.DrawBorderString(spriteBatch, text, center, color, scale, 0.5f, 0.4f);
        }
    }
}
