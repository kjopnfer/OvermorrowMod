using OvermorrowMod.Common.Detours;
using OvermorrowMod.Common.Primitives;
using System.Collections.Generic;

namespace OvermorrowMod.Core
{
    public static class DetourLoader
    {
        public static void Load()
        {
            Terraria.On_Main.DrawInterface += ParticleDetour.DrawParticles;

            #region Trails
            PrimitiveManager.trails = new List<Trail>();

            Terraria.On_NPC.NewNPC += PrimitiveManager.CreateNPCTrail;
            Terraria.On_Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float += PrimitiveManager.CreateProjectileTrail;

            Terraria.On_Main.DrawNPCs += PrimitiveManager.DrawNPCTrails;
            Terraria.On_Main.DrawProjectiles += PrimitiveManager.DrawProjectileTrails;

            Terraria.On_NPC.NPCLoot += PrimitiveManager.NPCLoot;
            Terraria.On_Projectile.Kill += PrimitiveManager.Kill;
            #endregion
        }

        public static void Unload()
        {
            Terraria.On_Main.DrawInterface -= ParticleDetour.DrawParticles;

            #region Trails
            Terraria.On_Projectile.Kill -= PrimitiveManager.Kill;
            Terraria.On_NPC.NPCLoot -= PrimitiveManager.NPCLoot;

            Terraria.On_Main.DrawProjectiles -= PrimitiveManager.DrawProjectileTrails;
            Terraria.On_Main.DrawNPCs -= PrimitiveManager.DrawNPCTrails;

            Terraria.On_Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float -= PrimitiveManager.CreateProjectileTrail;
            Terraria.On_NPC.NewNPC -= PrimitiveManager.CreateNPCTrail;

            PrimitiveManager.trails = null;
            #endregion
        }
    }
}
