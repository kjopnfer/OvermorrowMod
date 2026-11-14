using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core.UI.RunStats;
using OvermorrowMod.Core.WorldGeneration;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Detours
{
    /// <summary>
    /// Adds a Stats icon left of the vanilla equipment page icons while in a run.
    /// Selecting it switches the equipment area to <see cref="RunStatsPanel.Page"/>.
    /// </summary>
    public class RunStatsPageIcon : ILoadable
    {
        private const float IconScale = 0.9f;

        public void Load(Mod mod)
        {
            if (Main.dedServ) return;
            Terraria.On_Main.DrawPageIcons += DrawRunStatsIcon;
        }

        public void Unload() { }

        private static int DrawRunStatsIcon(Terraria.On_Main.orig_DrawPageIcons orig, int yPos)
        {
            if (!RunState.InRun)
            {
                if (Main.EquipPageSelected == RunStatsPanel.Page)
                    Main.EquipPageSelected = 0;

                return orig(yPos);
            }

            int hovered = orig(yPos);
            RunStatsPanel.PageIconsY = yPos;

            bool selected = Main.EquipPageSelected == RunStatsPanel.Page;
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + (selected ? "StatsIcon_Selected" : "StatsIcon")).Value;
            Vector2 position = new(Main.screenWidth - 224, yPos);
            Vector2 size = texture.Size() * IconScale;
            bool hovering = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y).Contains(Main.mouseX, Main.mouseY) && Main.mouseItem.stack < 1;

            if (hovering)
                Main.spriteBatch.Draw(ModContent.Request<Texture2D>(AssetDirectory.UI + "StatsIcon_Hover").Value, position, null, Main.OurFavoriteColor, 0f, Vector2.Zero, IconScale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(texture, position, null, Color.White, 0f, Vector2.Zero, IconScale, SpriteEffects.None, 0f);

            if (hovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                Main.HoverItem = new Item();
                Main.hoverItemName = Language.GetTextValue(LocalizationPath.UI + "RunStats.Stats");

                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    Main.EquipPageSelected = selected ? 0 : RunStatsPanel.Page;
                    Main.EquipPage = Main.EquipPageSelected;
                    Main.mouseLeftRelease = false;
                    SoundEngine.PlaySound(SoundID.MenuOpen);
                }
            }

            return hovered;
        }
    }
}
