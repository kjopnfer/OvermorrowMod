using OvermorrowMod.Common.Detours;

namespace OvermorrowMod.Core
{
    public static class ModDetourLoader
    {
        public static void Load()
        {
            Terraria.On_Main.DrawInterface += ParticleDetour.DrawParticles;
        }

        public static void Unload()
        {
            Terraria.On_Main.DrawInterface -= ParticleDetour.DrawParticles;
        }
    }
}
