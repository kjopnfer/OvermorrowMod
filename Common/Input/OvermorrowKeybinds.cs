using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Input
{
    public class OvermorrowKeybinds : ModSystem
    {
        public static ModKeybind ReloadKeybind;
        public static ModKeybind ParticleDebugKey;
        public static ModKeybind ParticleEditorKey;

        public override void Load()
        {
            ReloadKeybind = KeybindLoader.RegisterKeybind(Mod, "Reload", Keys.None);
            ParticleDebugKey = KeybindLoader.RegisterKeybind(Mod, "ParticleDebugSpawn", Keys.None);
            ParticleEditorKey = KeybindLoader.RegisterKeybind(Mod, "ParticleEditorToggle", Keys.None);
        }

        public override void Unload()
        {
            ReloadKeybind = null;
            ParticleDebugKey = null;
            ParticleEditorKey = null;
        }
    }
}
