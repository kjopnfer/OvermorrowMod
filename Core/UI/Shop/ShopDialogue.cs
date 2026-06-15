using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.NPCs.Archives.Shop;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace OvermorrowMod.Core.UI.Shop
{
    /// <summary>
    /// Code-driven replacement for the vanilla chat box when talking to the Cart shopkeeper.
    /// Shows a greeting and buttons; the Shop button opens the registered NPCShop.
    /// </summary>
    public class ShopDialogue : UIState
    {
        /// <summary>
        /// True while the local player is talking to a Cart shopkeeper and not already in a menu.
        /// </summary>
        public static bool IsActive
        {
            get
            {
                int talk = Main.LocalPlayer.talkNPC;
                if (talk <= -1 || Main.playerInventory) return false;
                NPC npc = Main.npc[talk];
                return npc.active && npc.ModNPC is CartShopkeeper;
            }
        }

        private Rectangle shopButton;
        private Rectangle leaveButton;
        private bool prevMouseLeft;
        private bool openSoundPlayed;
        private string currentLine;

        /// <summary>
        /// Called when the dialogue is no longer showing so the open sound can replay next time.
        /// </summary>
        public void NotifyClosed() => openSoundPlayed = false;

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsActive) return;

            if (!openSoundPlayed)
            {
                SoundEngine.PlaySound(new SoundStyle("OvermorrowMod/Sounds/DialogueDraw") { Volume = 0.6f });
                currentLine = CartShopkeeper.RandomLine();
                openSoundPlayed = true;
            }

            NPC npc = Main.npc[Main.LocalPlayer.talkNPC];

            Texture2D panelTex = ModContent.Request<Texture2D>(AssetDirectory.UI + "DialoguePanel").Value;

            Vector2 panelCenter = new Vector2(Main.screenWidth / 2f, Main.screenHeight * 0.30f);
            spriteBatch.Draw(panelTex, panelCenter, null, Color.White * 0.92f, 0f, panelTex.Size() / 2f, new Vector2(1.15f, 1f), SpriteEffects.None, 0f);

            int innerW = 540;
            int innerH = 200;
            Rectangle inner = new Rectangle((int)(panelCenter.X - innerW / 2f), (int)(panelCenter.Y - innerH / 2f), innerW, innerH);

            Texture2D portrait = ModContent.Request<Texture2D>(AssetDirectory.NPCs + "ShopCart").Value;
            spriteBatch.Draw(portrait, new Vector2(inner.Left + 95, panelCenter.Y), null, Color.White, 0f, portrait.Size() / 2f, 0.8f, SpriteEffects.None, 0f);

            var font = FontAssets.MouseText.Value;
            int textX = inner.Left + 190;

            string name = string.IsNullOrEmpty(npc.GivenName) ? "Cart" : npc.GivenName;
            Utils.DrawBorderString(spriteBatch, name, new Vector2(textX, inner.Top + 20), new Color(255, 220, 130), 1.05f);

            string greeting = currentLine ?? string.Empty;
            string wrapped = font.CreateWrappedText(greeting, inner.Right - textX - 10);
            Utils.DrawBorderString(spriteBatch, wrapped, new Vector2(textX, inner.Top + 52), Color.White, 0.8f);

            int btnW = 130;
            int btnH = 42;
            int btnY = inner.Bottom - btnH - 8;
            shopButton = new Rectangle(textX, btnY, btnW, btnH);
            leaveButton = new Rectangle(textX + btnW + 14, btnY, btnW, btnH);

            DrawButton(spriteBatch, shopButton, "Shop");
            DrawButton(spriteBatch, leaveButton, "Leave");

            HandleClicks(npc);
        }

        private void HandleClicks(NPC npc)
        {
            Point mouse = new Point(Main.mouseX, Main.mouseY);
            if (shopButton.Contains(mouse) || leaveButton.Contains(mouse))
                Main.LocalPlayer.mouseInterface = true;

            bool click = Main.mouseLeft && !prevMouseLeft;
            prevMouseLeft = Main.mouseLeft;
            if (!click) return;

            if (shopButton.Contains(mouse))
                OpenShop(npc);
            else if (leaveButton.Contains(mouse))
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                Main.LocalPlayer.SetTalkNPC(-1);
            }
        }

        private static void OpenShop(NPC npc)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);

            string shopName = NPCShopDatabase.GetShopName(npc.type, CartShopkeeper.ShopName);
            Main.playerInventory = true;
            Main.npcChatText = "";
            Main.recBigList = false;
            Main.SetNPCShopIndex(1);
            Main.instance.shop[Main.npcShop].SetupShop(shopName, npc);

            if (npc.ModNPC is CartShopkeeper cart)
                cart.FillShop(Main.instance.shop[Main.npcShop].item);
        }

        private static void DrawButton(SpriteBatch sb, Rectangle r, string label)
        {
            bool hover = r.Contains(Main.mouseX, Main.mouseY);
            var font = FontAssets.MouseText.Value;
            float scale = hover ? 1.1f : 1f;
            Color color = hover ? Color.White : new Color(255, 224, 150);

            Vector2 size = font.MeasureString(label) * scale;
            Vector2 pos = new Vector2(r.Center.X - size.X / 2f, r.Center.Y - size.Y / 2f);
            Utils.DrawBorderString(sb, label, pos, color, scale);
        }
    }
}
