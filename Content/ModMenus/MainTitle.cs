using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using ReLogic.Content;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.ModMenus
{
    public class MainTitle : ModMenu
    {
        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            logoDrawCenter += new Vector2(0, 16);
            logoScale = 1f;
            return true;
        }

        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>(AssetDirectory.Menu + "ModTitleLogo");
        public override string DisplayName => "Overmorrow";
        public override int Music => MusicLoader.GetMusicSlot("OvermorrowMod/Sounds/Music/GrandArchivesPlaceholder");
        public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>(AssetDirectory.Empty);
        public override Asset<Texture2D> SunTexture => ModContent.Request<Texture2D>(AssetDirectory.Empty);
    }
}