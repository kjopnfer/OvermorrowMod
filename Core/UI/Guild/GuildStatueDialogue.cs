using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.UI.Shop;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace OvermorrowMod.Core.UI.Guild
{
    public class GuildStatueDialogue : UIState
    {
        private const string Line = "MONKEY";

        private static readonly float CloseDistance = ModUtils.TilesToPixels(12);

        private static Vector2? statuePosition;

        public static bool IsActive => statuePosition is Vector2 position && !Main.playerInventory && Main.LocalPlayer.talkNPC == -1 && Vector2.Distance(Main.LocalPlayer.Center, position) <= CloseDistance;

        public static void Open(Vector2 position)
        {
            Main.LocalPlayer.SetTalkNPC(-1);
            statuePosition = position;
            SoundEngine.PlaySound(new SoundStyle("OvermorrowMod/Sounds/DialogueDraw") { Volume = 0.6f });
        }

        public static void Close() => statuePosition = null;

        private Rectangle leaveButton;
        private bool prevMouseLeft;

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D panelTex = ModContent.Request<Texture2D>(AssetDirectory.UI + "DialoguePanel").Value;

            Vector2 panelCenter = new Vector2(Main.screenWidth / 2f, Main.screenHeight * 0.30f);
            spriteBatch.Draw(panelTex, panelCenter, null, Color.White * 0.92f, 0f, panelTex.Size() / 2f, new Vector2(1.15f, 1f), SpriteEffects.None, 0f);

            Utils.DrawBorderStringBig(spriteBatch, Line, panelCenter - new Vector2(0, 30), Color.White, 1f, 0.5f, 0.5f);

            int btnW = 130;
            int btnH = 42;
            leaveButton = new Rectangle((int)(panelCenter.X - btnW / 2f), (int)(panelCenter.Y + 40), btnW, btnH);
            ShopDialogue.DrawButton(spriteBatch, leaveButton, "Leave");

            Point mouse = new Point(Main.mouseX, Main.mouseY);
            if (leaveButton.Contains(mouse)) Main.LocalPlayer.mouseInterface = true;

            bool click = Main.mouseLeft && !prevMouseLeft;
            prevMouseLeft = Main.mouseLeft;
            if (click && leaveButton.Contains(mouse))
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                Close();
            }
        }
    }
}
