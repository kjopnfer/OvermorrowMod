using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;

namespace OvermorrowMod.Common.Primitives
{
    public enum DrawType
    {
        NPC,
        Projectile
    }

    public abstract class Trail
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
        public static int CreateNPCTrail(On.Terraria.NPC.orig_NewNPC orig, IEntitySource source, int X, int Y, int Type, int start, float ai0, float ai1, float ai2, float ai3, int target)
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
        public static int CreateProjectileTrail(On.Terraria.Projectile.orig_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float orig, IEntitySource source, float X, float Y, float SpeedX, float SpeedY, int type, int damage, float Knockback, int owner, float ai0, float ai1)
        {
            int p = orig(source, X, Y, SpeedX, SpeedY, type, damage, Knockback, owner, ai0, ai1);
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
        public static void NPCLoot(On.Terraria.NPC.orig_NPCLoot orig, NPC self)
        {
            orig(self);
            KillByID(self.whoAmI, DrawType.NPC);
        }
        public static void Kill(On.Terraria.Projectile.orig_Kill orig, Projectile self)
        {
            orig(self);
            KillByID(self.whoAmI);
        }
        public static void DrawNPCTrails(On.Terraria.Main.orig_DrawNPCs orig, Main self, bool behind)
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
        public static void DrawProjectileTrails(On.Terraria.Main.orig_DrawProjectiles orig, Main self)
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


        public static void Load()
        {
            trails = new List<Trail>();
            On.Terraria.NPC.NewNPC += CreateNPCTrail;
            On.Terraria.Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float += CreateProjectileTrail;

            On.Terraria.Main.DrawNPCs += DrawNPCTrails;
            On.Terraria.Main.DrawProjectiles += DrawProjectileTrails;

            On.Terraria.NPC.NPCLoot += NPCLoot;
            On.Terraria.Projectile.Kill += Kill;
        }
        public static void Unload()
        {
            On.Terraria.Projectile.Kill -= Kill;
            On.Terraria.NPC.NPCLoot -= NPCLoot;

            On.Terraria.Main.DrawProjectiles -= DrawProjectileTrails;
            On.Terraria.Main.DrawNPCs -= DrawNPCTrails;

            On.Terraria.Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float -= CreateProjectileTrail;
            On.Terraria.NPC.NewNPC -= CreateNPCTrail;
            trails = null;
        }
        protected Effect Effect { get; }
        protected List<VertexPositionColorTexture> Vertices { get; } = new List<VertexPositionColorTexture>();
        protected Texture2D Texture { get; }
        protected TrailPositionBuffer Positions { get; }
        public bool Dying = false;
        public bool Dead = false;
        public int EntityID = -1;
        public Entity Entity;
        public ITrailEntity TrailEntity;
        public DrawType DrawType = DrawType.Projectile;
        public bool Pixelated = false;

        protected Trail(int length, Texture2D texture, Effect effect = null)
        {
            Positions = new TrailPositionBuffer(length);
            Texture = texture;
            Effect = effect ?? OvermorrowModFile.Instance.TrailShader;
        }

        public void AddVertex(Vector2 pos, Color color, Vector2 texCoord)
        {
            if (!Pixelated)
            {
                pos -= Main.screenPosition;
            }
            else
            {
                pos = (pos - Main.screenPosition) / 2;
            }
            Vertices.Add(new VertexPositionColorTexture(new Vector3(pos.X, pos.Y, 0), color, texCoord));
        }

        public abstract void Update();
        public abstract void PrepareTrail();
        public abstract void Draw();
        public abstract void UpdateDead();
    }
}