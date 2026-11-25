using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Items.Archives;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.MetaUpgrades;
using OvermorrowMod.Core.UI.MonkeyStones;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Chat;
using Ledger = OvermorrowMod.Core.UI.DeathScreen.DeathScreen;

namespace OvermorrowMod.Core.UI.Upgrades
{
    public partial class UpgradeScreen : UIState
    {
        private const int PanelWidth = 1000;
        private const int HeaderHeight = 108;
        private const int BodyHeight = 572;
        private const int FooterHeight = 60;
        private const int ColumnGap = 18;
        private const int AltarWidth = 230;
        private const int ListWidth = 260;
        private const int DetailWidth = PanelWidth - AltarWidth - ListWidth - ColumnGap * 2;
        private const int EntryGap = 5;

        private static readonly float CloseDistance = ModUtils.TilesToPixels(12);
        private static readonly SoundStyle DenySound = SoundID.MenuTick with { Pitch = -0.7f };
        private static readonly MonkeyStoneTier[] Tiers = Enum.GetValues<MonkeyStoneTier>();

        public static bool Visible { get; private set; }

        private static Vector2 vendorPosition;
        private static int openTicks;
        private static MetaUpgrade selected = MetaUpgrade.All[0];

        private readonly int[] walletFlash = new int[Tiers.Length];

        private int pendingSocket = -1;
        private int landTicks;
        private string countFrom;
        private int countTicks = CountDuration;
        private int shakeTicks;

        private Rectangle panel;
        private Rectangle statueArea;
        private readonly Rectangle[] walletRows = new Rectangle[Tiers.Length];
        private readonly Rectangle[] sockets = new Rectangle[6];

        private static string Text(string key) => Language.GetTextValue(LocalizationPath.UI + "UpgradeScreen." + key);

        public static void Open(Vector2 position)
        {
            vendorPosition = position;
            openTicks = 0;
            Visible = true;
            Main.playerInventory = false;
            Main.LocalPlayer.SetTalkNPC(-1);
        }

        public static void Close() => Visible = false;

        private static Color TierColor(MonkeyStoneTier tier) => tier switch
        {
            MonkeyStoneTier.Green => new Color(91, 212, 91),
            MonkeyStoneTier.Blue => new Color(79, 155, 255),
            MonkeyStoneTier.Orange => new Color(255, 154, 60),
            _ => new Color(255, 90, 210),
        };

        private static MetaUpgradePlayer Upgrades => Main.LocalPlayer.GetModPlayer<MetaUpgradePlayer>();

        private static MonkeyStonePlayer Wallet => Main.LocalPlayer.GetModPlayer<MonkeyStonePlayer>();

        private bool Busy => flyers.Count > 0 || pendingSocket >= 0;

        private int ShownRank => pendingSocket >= 0 ? pendingSocket : Upgrades.Rank(selected);

        private static float Appear(float start, float length) => MathHelper.Clamp((openTicks - start) / length, 0f, 1f);

        private Color AuraColor => Upgrades.IsMaxed(selected) ? Ledger.GoldColor : TierColor(Upgrades.NextCost(selected)[0].Tier);

        public override void Update(GameTime gameTime)
        {
            Player player = Main.LocalPlayer;
            if (Main.playerInventory || player.dead || Vector2.Distance(player.Center, vendorPosition) > CloseDistance)
            {
                Main.playerInventory = false;
                Close();
                return;
            }

            if (openTicks == 0)
            {
                motes.Clear();
                sparks.Clear();
            }

            openTicks++;
            for (int i = 0; i < walletFlash.Length; i++)
                if (walletFlash[i] > 0) walletFlash[i]--;
            if (shakeTicks > 0) shakeTicks--;
            if (landTicks > 0) landTicks--;

            UpdateFlyers();
            UpdateCount();
            UpdateMotes();
            sparks.Update();
        }

        private void Buy()
        {
            int rank = Upgrades.Rank(selected);
            StoneCost[] cost = selected.Costs[rank];
            string before = selected.ValueText(rank);

            if (!Upgrades.TryBuy(selected))
            {
                shakeTicks = ShakeDuration;
                SoundEngine.PlaySound(DenySound);
                return;
            }

            pendingSocket = rank;
            countFrom = before;
            countTicks = CountDuration;
            Vector2 target = sockets[rank].Center.ToVector2();

            int delay = 0;
            foreach (StoneCost stone in cost)
            {
                int index = Array.IndexOf(Tiers, stone.Tier);
                walletFlash[index] = 24;
                Vector2 from = walletRows[index].Center.ToVector2();
                for (int i = 0; i < Math.Clamp(stone.Amount, 1, MaxFlyersPerStone); i++)
                {
                    flyers.Add(new Flyer { Tier = stone.Tier, From = from + new Vector2(Main.rand.NextFloat(-30f, 30f), 0), To = target, Delay = delay });
                    delay += FlightStagger;
                }
            }

            SoundEngine.PlaySound(SoundID.Grab);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Main.LocalPlayer.mouseInterface = true;

            panel = new Rectangle(Main.screenWidth / 2 - PanelWidth / 2, Math.Max(10, (Main.screenHeight - HeaderHeight - BodyHeight - FooterHeight) / 2), PanelWidth, HeaderHeight + BodyHeight + FooterHeight);
            int bodyTop = panel.Y + HeaderHeight;

            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * 0.6f * Appear(0, 12));

            DrawHeader(spriteBatch);
            DrawAltar(spriteBatch, new Rectangle(panel.X, bodyTop, AltarWidth, BodyHeight));
            DrawList(spriteBatch, new Rectangle(panel.X + AltarWidth + ColumnGap, bodyTop, ListWidth, BodyHeight));
            DrawDetail(spriteBatch, new Rectangle(panel.Right - DetailWidth, bodyTop, DetailWidth, BodyHeight - 20));
            DrawClose(spriteBatch, new Rectangle(panel.Center.X - 90, panel.Bottom - FooterHeight + 10, 180, 42));
            DrawEffects(spriteBatch);
        }

        private void DrawHeader(SpriteBatch spriteBatch)
        {
            float alpha = Appear(0, 24);
            Ledger.DrawBand(spriteBatch, new Rectangle(panel.X, panel.Y + 18, panel.Width, HeaderHeight - 30), alpha);
            Ledger.DrawBanner(spriteBatch, panel.Center.X, panel.Y + 14, 250, alpha);

            DynamicSpriteFont titleFont = FontAssets.DeathText.Value;
            string title = Text("VendorName");
            Vector2 size = titleFont.MeasureString(title) * 0.9f;
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, titleFont, title, new Vector2(panel.Center.X - size.X / 2f, panel.Y + 28), Color.White * alpha, 0f, Vector2.Zero, new Vector2(0.9f));

            Ledger.DrawCentered(spriteBatch, Text("Message"), new Vector2(panel.Center.X, panel.Y + 90), Ledger.LightSilverColor * alpha, 1f);
        }

        private void DrawClose(SpriteBatch spriteBatch, Rectangle button)
        {
            float alpha = Appear(40, 20);
            bool hovered = button.Contains(Main.MouseScreen.ToPoint());
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, button, Ledger.SlotColor * alpha);
            Ledger.DrawBorder(spriteBatch, button, (hovered ? Ledger.LightSilverColor : Ledger.DarkSilverColor) * alpha);
            Ledger.DrawCentered(spriteBatch, Text("Close"), button.Center.ToVector2(), (hovered ? Color.White : Ledger.GoldColor) * alpha, 1f);

            if (hovered && Main.mouseLeft && Main.mouseLeftRelease)
            {
                Main.mouseLeftRelease = false;
                SoundEngine.PlaySound(SoundID.MenuClose);
                Close();
            }
        }

        private static void DrawStone(SpriteBatch spriteBatch, MonkeyStoneTier tier, Vector2 center, float size, Color color) => DrawIconFit(spriteBatch, MonkeyStoneDisplay.StoneType(tier), center, size, color);

        private static void DrawIconFit(SpriteBatch spriteBatch, int type, Vector2 center, float size, Color color)
        {
            Main.instance.LoadItem(type);
            Texture2D texture = TextureAssets.Item[type].Value;
            Rectangle frame = Main.itemAnimations[type] != null ? Main.itemAnimations[type].GetFrame(texture) : texture.Frame();
            float scale = size / Math.Max(frame.Width, frame.Height);
            spriteBatch.Draw(texture, center, frame, color, 0f, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
        }

        private static void DrawFrame(SpriteBatch spriteBatch, Rectangle rect, Color color, int thickness)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
            spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness), color);
            spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y + thickness, thickness, rect.Height - thickness * 2), color);
            spriteBatch.Draw(pixel, new Rectangle(rect.Right - thickness, rect.Y + thickness, thickness, rect.Height - thickness * 2), color);
        }
    }
}
