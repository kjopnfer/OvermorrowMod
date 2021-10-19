using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;

namespace OvermorrowMod.Effects.Prim
{
    public enum DrawType
    {
        Projectile,
        NPC
    }
    public class TrailManager
    {
        public List<Trail> activeTrails = new List<Trail>();
        public void CreateTrail(Projectile projectile)
        {
            if (projectile.modProjectile == null || !(projectile.modProjectile is ITrailEntity)) return;
            ITrailEntity trail = projectile.modProjectile as ITrailEntity;
            Type type = trail.TrailType();
            if (!type.IsSubclassOf(typeof(Trail)) || type == typeof(Trail)) return;
            Trail newTrail = (Trail)Activator.CreateInstance(trail.TrailType());
            newTrail.Entity = projectile;
            newTrail.Type = DrawType.Projectile;
            newTrail.SetDefaults();
            activeTrails.Add(newTrail);
        }
        public void CreateTrail(NPC npc)
        {
            if (npc.modNPC == null || !(npc.modNPC is ITrailEntity)) return;
            ITrailEntity trail = npc.modNPC as ITrailEntity;
            Type type = trail.TrailType();
            if (!type.IsSubclassOf(typeof(Trail)) || type == typeof(Trail)) return;
            Trail newTrail = (Trail)Activator.CreateInstance(trail.GetType());
            newTrail.Entity = npc;
            newTrail.Type = DrawType.NPC;
            newTrail.SetDefaults();
            Main.NewText("NPC Trail created");
            activeTrails.Add(newTrail);
        }
        public void UpdateTrails()
        {
            for (int i = 0; i < activeTrails.Count; i++)
            {
                if (activeTrails[i].Entity.active && !activeTrails[i].Dying)
                    activeTrails[i].Update();
                else
                {
                    activeTrails[i].Dying = true;
                    activeTrails[i].UpdateDead();
                }
                if (activeTrails[i].Dead)
                {
                    activeTrails.RemoveAt(i);
                    i--;
                }
            }
        }
        public void DrawTrailsProjectile(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < activeTrails.Count; i++)
            {
                if (activeTrails[i].Type == DrawType.Projectile)
                {
                    activeTrails[i].PrepareTrail();
                    activeTrails[i].PrepareEffect();
                    activeTrails[i].Draw(spriteBatch);
                    activeTrails[i].Vertices.Clear();
                }
            }
        }
        public void DrawTrailsNPC(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < activeTrails.Count; i++)
            {
                if (activeTrails[i].Type == DrawType.NPC)
                {
                    activeTrails[i].PrepareTrail();
                    activeTrails[i].PrepareEffect();
                    activeTrails[i].Draw(spriteBatch);
                    activeTrails[i].Vertices.Clear();
                }
            }
        }
    }
}