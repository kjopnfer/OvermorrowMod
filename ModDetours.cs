using Terraria;
using Microsoft.Xna.Framework;

namespace OvermorrowMod
{
    public static class ModDetours
    {
        public static void Load()
        {
            On.Terraria.Projectile.NewProjectile_float_float_float_float_int_int_float_int_float_float += NewProjectile;
            On.Terraria.Main.DrawProj += DrawTrailsProjectile;
            On.Terraria.NPC.NewNPC += NewNPC;
            On.Terraria.Main.DrawNPC += DrawTrailsNPC;
        }
        public static void Unload()
        {
            On.Terraria.Projectile.NewProjectile_float_float_float_float_int_int_float_int_float_float -= NewProjectile;
            On.Terraria.Main.DrawProj -= DrawTrailsProjectile;
            On.Terraria.NPC.NewNPC -= NewNPC;
            On.Terraria.Main.DrawNPC -= DrawTrailsNPC;
        }
        public static int NewProjectile(On.Terraria.Projectile.orig_NewProjectile_float_float_float_float_int_int_float_int_float_float orig, float X, float Y, float speedX, float speedY, int type, int damage, float kb, int owner, float ai0, float ai1)
        {
            int p = orig(X, Y, speedX, speedY, type, damage, kb, owner, ai0, ai1);
            OvermorrowModFile.Mod.TrailManager.CreateTrail(Main.projectile[p]);
            return p;
        }
        public static void DrawTrailsProjectile(On.Terraria.Main.orig_DrawProj orig, Main self, int i)
        {
            OvermorrowModFile.Mod.TrailManager.DrawTrailsProjectile(Main.spriteBatch);
            orig(self, i);
        }
        public static void DrawTrailsNPC(On.Terraria.Main.orig_DrawNPC orig, Main self, int index, bool behind)
        {
            OvermorrowModFile.Mod.TrailManager.DrawTrailsNPC(Main.spriteBatch);
            orig(self, index, behind);
        }
        public static int NewNPC(On.Terraria.NPC.orig_NewNPC orig, int X, int Y, int type, int start, float ai0, float ai1, float ai2, float ai3, int target)
        {
            int n = orig(X, Y, type, start, ai0, ai1, ai2, ai3, target);
            OvermorrowModFile.Mod.TrailManager.CreateTrail(Main.npc[n]);
            return n;
        }
    }
}