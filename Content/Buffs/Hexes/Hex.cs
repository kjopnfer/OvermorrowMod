using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs.Hexes
{
    public class Hex
    {
        public static Dictionary<Type, int> HexTypes;
        public static int HexType<T>() where T : ModHex => HexTypes[typeof(T)];
        public void Kill() { time = 0; }
        public int type;
        public int time;
        public ModHex modHex;
        public NPC npc;
    }
    public static class HexUtils
    {
        public static bool HasHex(this NPC npc, int type)
        {
            var modNpc = npc.GetGlobalNPC<HexNPC>();
            foreach (Hex hex in modNpc.Hexes)
            {
                if (hex.type == type) return true;
            }
            return false;
        }
        public static void AddHex(this NPC npc, int type, int time, bool sync = false)
        {
            ModHex modHex = ModHex.ModHexes[type];
            Hex hex = new Hex();
            hex.type = type;
            hex.modHex = (ModHex)Activator.CreateInstance(modHex.GetType());
            hex.modHex.hex = hex;
            hex.npc = npc;
            hex.time = time;
            var modNpc = npc.GetGlobalNPC<HexNPC>();
            // TODO: add multiplayer synchronization when adding hexes
            if (!npc.HasHex(type) && modHex.OnTryAdd())
                modNpc.Hexes.Add(hex);
        }
    }
    public class HexLoader
    {
        public static void Load(bool unload)
        {
            if (unload)
            {
                ModHex.ModHexes = null;
                Hex.HexTypes = null;
            }
            else
            {
                ModHex.ModHexes = new Dictionary<int, ModHex>();
                Hex.HexTypes = new Dictionary<Type, int>();
            }
        }
        public static void TryRegisteringHex(Type type)
        {
            Type baseType = typeof(ModHex);
            if (type != baseType && !type.IsAbstract && type.IsSubclassOf(baseType))
            {
                int id = Hex.HexTypes.Count;
                ModHex.ModHexes.Add(id, (ModHex)Activator.CreateInstance(type));
                Hex.HexTypes.Add(type, id);
            }
        }
    }
    public abstract class ModHex
    {
        public static Dictionary<int, ModHex> ModHexes;
        public Hex hex;
        public NPC npc { get { return hex.npc; } private set { } }
        /// <summary>Is ran every time a hex is added, return false to prevent the hex from being added</summary>
        public virtual bool OnTryAdd() { return true; }
        /// <summary>Is ran right before removing the hex</summary>
        public virtual void OnRemove() { }
        /// <summary>Is ran every time a npc is updated</summary>
        public virtual void Update() { }
        /// <summary>Is ran every time a npc is updated, more focused on life regeneration</summary>
        public virtual void UpdateLifeRegen(ref int damage) { }
        /// <summary>Used for drawing sprites or changing the npc's drawing color</summary>
        public virtual void Draw(SpriteBatch spriteBatch, Vector2 screenPos, ref Color drawColor) { }
        /// <summary>Used for making synergies between 2 different hexes</summary>
        public virtual void ApplySynergies(Hex otherHex) { }
    }
    public class HexNPC : GlobalNPC
    {
        public List<Hex> Hexes = new List<Hex>();
        public override bool InstancePerEntity => true;
        public override bool PreAI(NPC npc)
        {
            for (int i = 0; i < Hexes.Count; i++)
            {
                Hex hex = Hexes[i];
                hex.npc = npc;
                hex.modHex.Update();
                for (int j = 0; j < Hexes.Count; j++)
                {
                    Hex hex2 = Hexes[j];
                    hex.modHex.ApplySynergies(hex2);
                }
                hex.time--;
                if (hex.time < 0)
                {
                    hex.modHex.OnRemove();
                    Hexes.RemoveAt(i);
                    i--;
                }
            }
            return true;
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            for (int i = 0; i < Hexes.Count; i++)
            {
                Hex hex = Hexes[i];
                hex.npc = npc;
                hex.modHex.UpdateLifeRegen(ref damage);
            }
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            for (int i = 0; i < Hexes.Count; i++)
            {
                Hex hex = Hexes[i];
                hex.npc = npc;
                hex.modHex.Draw(spriteBatch, screenPos, ref drawColor);
            }
            return true;
        }
    }
}