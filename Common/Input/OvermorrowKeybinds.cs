using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Input
{
    public class OvermorrowKeybinds : ModSystem
    {
        public static ModKeybind ReloadKeybind;

        public override void Load()
        {
            ReloadKeybind = KeybindLoader.RegisterKeybind(Mod, "Reload", Keys.None);
        }

        public override void Unload()
        {
            ReloadKeybind = null;
        }
    }
}
