using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.WorldGeneration.TestSubworld;
using ReLogic.Graphics;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Chat;

namespace OvermorrowMod.Core.UI.ClassSelection
{
    public enum CharacterClass { Warrior, Mage, Ranger, Summoner }

    public class ClassSelection : UIState
    {
        public static bool visible = false;

        private const int PanelWidth = 460;
        private const int PanelHeight = 250;
        private const int OptionSize = 88;
        private const int OptionGap = 18;
        private const int ButtonWidth = 120;
        private const int ButtonHeight = 32;
        private const int ButtonGap = 24;

        private static readonly CharacterClass[] Classes = { CharacterClass.Warrior, CharacterClass.Mage, CharacterClass.Ranger, CharacterClass.Summoner };

        private static readonly Dictionary<CharacterClass, (int type, int stack)[]> Loadouts = new()
        {
            [CharacterClass.Warrior] = new (int, int)[] { (ItemID.GoldBroadsword, 1), (ItemID.EnchantedBoomerang, 1) },
            [CharacterClass.Mage] = new (int, int)[] { (ItemID.DiamondStaff, 1), (ItemID.WandofSparking, 1) },
            [CharacterClass.Ranger] = new (int, int)[] { (ItemID.GoldBow, 1), (ItemID.Musket, 1), (ItemID.WoodenArrow, 150), (ItemID.MusketBall, 150) },
            [CharacterClass.Summoner] = new (int, int)[] { (ItemID.SlimeStaff, 1), (ItemID.BlandWhip, 1) },
        };

        public static int MainWeaponType(CharacterClass c) => Loadouts[c][0].type;

        public CharacterClass selectedClass = CharacterClass.Warrior;

        private ClassOption[] options;
        private ClassSelectionButton actionButton;
        private ClassSelectionButton closeButton;

        public override void OnInitialize()
        {
            options = new ClassOption[Classes.Length];
            for (int i = 0; i < Classes.Length; i++)
            {
                options[i] = new ClassOption { characterClass = Classes[i] };
                options[i].Width.Set(OptionSize, 0);
                options[i].Height.Set(OptionSize, 0);
                Append(options[i]);
            }

            actionButton = new ClassSelectionButton();
            actionButton.Width.Set(ButtonWidth, 0);
            actionButton.Height.Set(ButtonHeight, 0);
            actionButton.onClick = OnActionClicked;
            Append(actionButton);

            closeButton = new ClassSelectionButton { kind = ClassButtonKind.Close };
            closeButton.Width.Set(ButtonWidth, 0);
            closeButton.Height.Set(ButtonHeight, 0);
            closeButton.onClick = Hide;
            Append(closeButton);
        }

        public void Show()
        {
            visible = true;
            actionButton.kind = SubworldSystem.IsActive<TestSubworld>() ? ClassButtonKind.Regenerate : ClassButtonKind.Enter;
            SelectClass(selectedClass);
        }

        public void Hide() => visible = false;

        public void SelectClass(CharacterClass c)
        {
            selectedClass = c;
            for (int i = 0; i < options.Length; i++)
                options[i].selected = options[i].characterClass == c;
        }

        private void OnActionClicked()
        {
            var sp = Main.LocalPlayer.GetModPlayer<SubworldPlayer>();
            sp.pendingClassItems = new List<Item>();
            foreach (var (type, stack) in Loadouts[selectedClass])
            {
                var item = new Item();
                item.SetDefaults(type);
                item.stack = stack;
                sp.pendingClassItems.Add(item);
            }

            bool regenerating = SubworldSystem.IsActive<TestSubworld>();
            if (regenerating)
            {
                for (int i = 0; i < Main.LocalPlayer.inventory.Length; i++) Main.LocalPlayer.inventory[i] = new Item();
                for (int i = 0; i < Main.LocalPlayer.armor.Length; i++) Main.LocalPlayer.armor[i] = new Item();
                // Enter<T>() while already inside T no-ops in SubworldLibrary.
                // Exiting first queues a transition back through the main
                // world so the second Enter takes effect.
                SubworldSystem.Exit();
            }

            Hide();
            SubworldSystem.Enter<TestSubworld>();
        }

        public override void Update(GameTime gameTime)
        {
            if (!visible) return;

            float panelX = (Main.screenWidth - PanelWidth) / 2f;
            float panelY = (Main.screenHeight - PanelHeight) / 2f;

            float rowWidth = OptionSize * options.Length + OptionGap * (options.Length - 1);
            float rowStartX = panelX + (PanelWidth - rowWidth) / 2f;
            float rowY = panelY + 64f;

            for (int i = 0; i < options.Length; i++)
            {
                options[i].Left.Set(rowStartX + i * (OptionSize + OptionGap), 0);
                options[i].Top.Set(rowY, 0);
            }

            float buttonsY = panelY + PanelHeight - ButtonHeight - 16f;
            float buttonsTotal = ButtonWidth * 2 + ButtonGap;
            float buttonsStartX = panelX + (PanelWidth - buttonsTotal) / 2f;

            actionButton.Left.Set(buttonsStartX, 0);
            actionButton.Top.Set(buttonsY, 0);

            closeButton.Left.Set(buttonsStartX + ButtonWidth + ButtonGap, 0);
            closeButton.Top.Set(buttonsY, 0);

            Recalculate();
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!visible) return;

            float panelX = (Main.screenWidth - PanelWidth) / 2f;
            float panelY = (Main.screenHeight - PanelHeight) / 2f;
            Rectangle panelRect = new Rectangle((int)panelX, (int)panelY, PanelWidth, PanelHeight);

            Texture2D pixel = TextureAssets.MagicPixel.Value;
            spriteBatch.Draw(pixel, panelRect, new Color(20, 20, 30, 240));
            DrawBorder(spriteBatch, panelRect, new Color(160, 140, 90), 2);

            DynamicSpriteFont titleFont = FontAssets.DeathText.Value;
            string titleText = "Choose Your Character";
            Vector2 titleSize = titleFont.MeasureString(titleText);
            Vector2 titlePos = new Vector2(panelX + PanelWidth / 2f, panelY + 12f);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, titleFont, titleText, titlePos, new Color(220, 200, 130), 0f, new Vector2(titleSize.X / 2f, 0f), new Vector2(0.6f));

            if (panelRect.Contains(Main.MouseScreen.ToPoint()))
                Main.LocalPlayer.mouseInterface = true;

            base.Draw(spriteBatch);
        }

        private static void DrawBorder(SpriteBatch sb, Rectangle r, Color c, int t)
        {
            Texture2D px = TextureAssets.MagicPixel.Value;
            sb.Draw(px, new Rectangle(r.X, r.Y, r.Width, t), c);
            sb.Draw(px, new Rectangle(r.X, r.Bottom - t, r.Width, t), c);
            sb.Draw(px, new Rectangle(r.X, r.Y, t, r.Height), c);
            sb.Draw(px, new Rectangle(r.Right - t, r.Y, t, r.Height), c);
        }
    }
}
