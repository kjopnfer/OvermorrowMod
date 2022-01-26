using MonoMod.Cil;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod
{
    public static class ILEdits
    {
        public static void Load()
        {
            IL.Terraria.Main.UpdateAudio += TitleMusic;
            IL.Terraria.Main.UpdateAudio += TitleDisable;
        }

        public static void Unload()
        {
            IL.Terraria.Main.UpdateAudio -= TitleMusic;
            IL.Terraria.Main.UpdateAudio -= TitleDisable;
        }

        private static void TitleMusic(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(instr => instr.MatchLdcI4(6) && instr.Next.MatchStfld(typeof(Main).GetField("newMusic"))))
            {
                throw new Exception("Title music patch failed lol");
            }
            c.Index++;
            c.EmitDelegate<Func<int, int>>(TitleMusicDelegate);
        }

        public static int TitleMusicDelegate(int oldMusic)
        {
            // Can also add mod checks for music here and return oldMusic if not active
            return OvermorrowModFile.Mod.GetSoundSlot((SoundType)51, "Sounds/Music/SandstormBoss");
        }

        // If you don't include the following two methods, the game slaps your volume down to zero
        public static int TitleDisableDelegate(int oldValue) => 0;
        public static void TitleDisable(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(instr => instr.MatchLdsfld<Main>("musicError") && instr.Next.MatchLdcI4(100)))
            {
                throw new Exception("Title music disable failed");
            }
            c.Index++;
            c.EmitDelegate<Func<int, int>>(TitleDisableDelegate);
        }
    }
}