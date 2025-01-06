using OvermorrowMod.Common.Primitives;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;

namespace OvermorrowMod.Common.Detours
{
    public enum DrawType
    {
        NPC,
        Projectile
    }

    public class PrimitiveManager
    {
        public static List<Trail> trails;

        public static void UpdateTrails()
        {
            for (int i = 0; i < trails.Count; i++)
            {
                Trail trail = trails[i];
                if (!trail.Entity.active)
                {
                    trail.Dying = true;
                }
                if (trail.Dead)
                {
                    trails.RemoveAt(i);
                    i--;
                    continue;
                }
                if (!trail.Dying)
                    trail.Update();
                else
                {
                    trail.UpdateDead();
                }
            }
        }

        public static void KillByID(int id, DrawType type = DrawType.Projectile)
        {
            for (int i = 0; i < trails.Count; i++)
            {
                Trail trail = trails[i];
                if (trail.DrawType == type && trail.EntityID == id && !trail.Dying)
                {
                    trail.Dying = true;
                }
            }
        }

        public static int CreateNPCTrail(Terraria.On_NPC.orig_NewNPC orig, IEntitySource source, int X, int Y, int Type, int start, float ai0, float ai1, float ai2, float ai3, int target)
        {
            int a = orig(source, X, Y, Type, start, ai0, ai1, ai2, ai3, target);
            NPC npc = Main.npc[a];
            if (npc.ModNPC != null && npc.ModNPC is ITrailEntity)
            {
                ITrailEntity entity = npc.ModNPC as ITrailEntity;
                Trail trail = (Trail)Activator.CreateInstance(entity.TrailType());
                trail.DrawType = DrawType.NPC;
                trail.Entity = npc;
                trail.EntityID = npc.whoAmI;
                trail.TrailEntity = entity;
                trails.Add(trail);
            }
            return a;
        }

        public static int CreateProjectileTrail(Terraria.On_Projectile.orig_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float orig, IEntitySource source, float X, float Y, float SpeedX, float SpeedY, int type, int damage, float Knockback, int owner, float ai0, float ai1, float ai2)
        {
            int p = orig(source, X, Y, SpeedX, SpeedY, type, damage, Knockback, owner, ai0, ai1, ai2);
            Projectile projectile = Main.projectile[p];
            if (projectile.ModProjectile is ITrailEntity entity)
            {
                Trail trail = (Trail)Activator.CreateInstance(entity.TrailType());
                trail.DrawType = DrawType.Projectile;
                trail.Entity = projectile;
                trail.EntityID = projectile.whoAmI;
                trail.TrailEntity = entity;
                trails.Add(trail);
            }
            return p;
        }

        public static void NPCLoot(Terraria.On_NPC.orig_NPCLoot orig, NPC self)
        {
            orig(self);
            KillByID(self.whoAmI, DrawType.NPC);
        }

        public static void Kill(Terraria.On_Projectile.orig_Kill orig, Projectile self)
        {
            orig(self);
            KillByID(self.whoAmI);
        }

        public static void DrawNPCTrails(Terraria.On_Main.orig_DrawNPCs orig, Main self, bool behind)
        {
            foreach (Trail trail in trails)
            {
                if (trail.DrawType == DrawType.NPC && !trail.Pixelated)
                {
                    trail.PrepareTrail();
                    trail.Draw();
                    trail.Vertices.Clear();
                }
            }
            orig(self, behind);
        }

        public static void DrawProjectileTrails(Terraria.On_Main.orig_DrawProjectiles orig, Main self)
        {
            foreach (Trail trail in trails)
            {
                if (trail.DrawType == DrawType.Projectile && !trail.Pixelated)
                {
                    trail.PrepareTrail();
                    trail.Draw();
                    trail.Vertices.Clear();
                }
            }
            orig(self);
        }
    }
}