using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Effects.Prim
{
    public enum DrawType
    {
        NPC,
        Projectile
    }
   
    public abstract class Trail
    {
        public static RenderTarget2D NPCTarget;
        public static RenderTarget2D ProjTarget;
        public static List<Trail> trails;
        public static void UpdateTrails()
        {
            for( int i = 0; i < trails.Count; i++)
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
            for( int i = 0; i < trails.Count; i++)
            {
                Trail trail = trails[i];
                if (trail.DrawType == type && trail.EntityID == id && !trail.Dying)
                {
                    trail.Dying = true;
                }
            }
        }
        public static int CreateNPCTrail(On.Terraria.NPC.orig_NewNPC orig, int X, int Y, int Type, int start, float ai0, float ai1, float ai2, float ai3, int target)
        {
            int a = orig(X, Y, Type, start, ai0, ai1, ai2, ai3, target);
            NPC npc = Main.npc[a];
            if (npc.modNPC != null && npc.modNPC is ITrailEntity)
            {
                ITrailEntity entity = npc.modNPC as ITrailEntity;
                Trail trail = (Trail)Activator.CreateInstance(entity.TrailType());
                trail.SetDefaults();
                trail.DrawType = DrawType.NPC;
                trail.Entity = npc;
                trail.EntityID = npc.whoAmI;
                trail.TrailEntity = entity;
                trails.Add(trail);
            }
            return a;
        }
        public static int CreateProjectileTrail(On.Terraria.Projectile.orig_NewProjectile_float_float_float_float_int_int_float_int_float_float orig, float X, float Y, float SpeedX, float SpeedY, int type, int damage, float Knockback, int owner, float ai0, float ai1)
        {
            int p = orig(X, Y, SpeedX, SpeedY, type, damage, Knockback, owner, ai0, ai1);
            Projectile projectile = Main.projectile[p];
            if (projectile.modProjectile != null && projectile.modProjectile is ITrailEntity)
            {
                ITrailEntity entity = projectile.modProjectile as ITrailEntity;
                Trail trail = (Trail)Activator.CreateInstance(entity.TrailType());
                trail.SetDefaults();
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
        public static void DrawPixelatedProjs(GameTime time)
        {
            if (Main.spriteBatch != null && ProjTarget != null && trails != null)
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                GraphicsDevice device = Main.graphics.GraphicsDevice;
                
                RenderTargetBinding[] bindings = device.GetRenderTargets();

                device.SetRenderTarget(ProjTarget);
                device.Clear(Color.Transparent);

                spriteBatch.Begin();
                foreach(Trail trail in trails)
                {
                    if (trail.Pixelated && trail.DrawType == DrawType.Projectile)
                    {
                        trail.PrepareTrail();
                        trail.Draw(spriteBatch);
                        trail.Vertices.Clear();
                    }
                }
                spriteBatch.End();

                device.SetRenderTargets(bindings);
            }
        }
        public static void DrawPixelatedNPCs(GameTime time)
        {
            if (Main.spriteBatch != null && NPCTarget != null && trails != null)
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                GraphicsDevice device = Main.graphics.GraphicsDevice;
                
                RenderTargetBinding[] bindings = device.GetRenderTargets();

                device.SetRenderTarget(NPCTarget);
                device.Clear(Color.Transparent);

                spriteBatch.Begin();
                foreach(Trail trail in trails)
                {
                    if (trail.Pixelated && trail.DrawType == DrawType.NPC)
                    {
                        trail.Draw(spriteBatch);
                        trail.Vertices.Clear();
                    }
                }
                spriteBatch.End();

                device.SetRenderTargets(bindings);
            }
        }
        public static void DrawNPCTrails(On.Terraria.Main.orig_DrawNPCs orig, Main self, bool behind)
        {
            foreach(Trail trail in trails)
            {
                if (trail.DrawType == DrawType.NPC && !trail.Pixelated)
                {
                    trail.PrepareTrail();
                    trail.Draw(Main.spriteBatch);
                    trail.Vertices.Clear();
                }
            }
            Main.spriteBatch.Draw(NPCTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
            orig(self, behind);
        }
        public static void DrawProjectileTrails(On.Terraria.Main.orig_DrawProjectiles orig, Main self)
        {
            foreach(Trail trail in trails)
            {
                if (trail.DrawType == DrawType.Projectile && !trail.Pixelated)
                {
                    trail.PrepareTrail();
                    trail.Draw(Main.spriteBatch);
                    trail.Vertices.Clear();
                }
            }
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, default, default);
            Main.spriteBatch.Draw(ProjTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
            Main.spriteBatch.End();
            orig(self);
        }
        public static void ChangeResolution(Vector2 reso)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            NPCTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth / 2, Main.screenHeight / 2);
            ProjTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth / 2, Main.screenHeight / 2);
        }

        public static void Load()
        {
            trails = new List<Trail>();
            On.Terraria.NPC.NewNPC += CreateNPCTrail;
            On.Terraria.Projectile.NewProjectile_float_float_float_float_int_int_float_int_float_float += CreateProjectileTrail;
            
            Main.OnResolutionChanged += ChangeResolution;

            NPCTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth / 2, Main.screenHeight / 2);
            ProjTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth / 2, Main.screenHeight / 2);

            On.Terraria.Main.DrawNPCs += DrawNPCTrails;
            On.Terraria.Main.DrawProjectiles += DrawProjectileTrails;

            On.Terraria.NPC.NPCLoot += NPCLoot;
            On.Terraria.Projectile.Kill += Kill;
            Main.OnPreDraw += DrawPixelatedProjs;
            Main.OnPreDraw += DrawPixelatedNPCs;
        }
        public static void Unload()
        {
            Main.OnPreDraw -= DrawPixelatedProjs;
            Main.OnPreDraw -= DrawPixelatedNPCs;
            On.Terraria.Projectile.Kill -= Kill;
            On.Terraria.NPC.NPCLoot -= NPCLoot;

            On.Terraria.Main.DrawProjectiles -= DrawProjectileTrails;
            On.Terraria.Main.DrawNPCs -= DrawNPCTrails;

            Main.OnResolutionChanged -= ChangeResolution;

            On.Terraria.Projectile.NewProjectile_float_float_float_float_int_int_float_int_float_float -= CreateProjectileTrail;
            On.Terraria.NPC.NewNPC -= CreateNPCTrail;
            trails = null;
            NPCTarget = null;
            ProjTarget = null;
        }
        public Effect Effect = OvermorrowModFile.Mod.TrailShader;
        public List<VertexPositionColorTexture> Vertices = new List<VertexPositionColorTexture>();
        public List<Vector2> Positions = new List<Vector2>();
        public int Length = 10;
        public bool Dying = false;
        public bool Dead = false;
        public int EntityID = -1;
        public Entity Entity;
        public ITrailEntity TrailEntity;
        public DrawType DrawType = DrawType.Projectile;
        public bool Pixelated = false;
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
        public void TrimToLength()
        {
            if (Positions.Count > Length)
            {
                Positions.RemoveAt(0);
            }
        }
        public virtual void SetDefaults()
        {

        }
        public virtual void Update()
        {

        }
        public virtual void PrepareTrail()
        {

        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
        public virtual void UpdateDead()
        {

        }
    }
}