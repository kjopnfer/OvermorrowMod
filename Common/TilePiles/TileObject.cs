using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using System.Collections.Generic;
using System;
using Terraria.Audio;
using Terraria.ID;

namespace OvermorrowMod.Common.TilePiles
{
    public class TileObjects
    {
        public static Dictionary<Type, string> TileObjectTypes;
        public static Dictionary<string, Texture2D> TileObjectTextures;
        public static Dictionary<string, string> TileObjectNames;

        public static Texture2D GetTexture(string type) => TileObjectTextures[type];
        public static string ObjectType<T>() where T : TileObject => TileObjectTypes[typeof(T)];

        public static void Load()
        {
            TileObjectTypes = new Dictionary<Type, string>();
            TileObjectTextures = new Dictionary<string, Texture2D>();
            TileObjectNames = new Dictionary<string, string>();
            TileObject.TileObjects = new Dictionary<string, TileObject>();
        }

        public static void Unload()
        {
            TileObjectTypes = null;
            TileObjectTextures = null;
            TileObjectNames = null;
            TileObject.TileObjects = null;
        }

        public static void RegisterTileObject(Type type)
        {
            Type baseType = typeof(TileObject);
            if (type.IsSubclassOf(baseType) && !type.IsAbstract && type != baseType)
            {
                string id = type.Name;
                TileObjectTypes.Add(type, id);

                TileObject tileObject = (TileObject)Activator.CreateInstance(type);
                tileObject.SetDefaults();
                TileObject.TileObjects.Add(id, tileObject);

                var texture = ModContent.Request<Texture2D>(tileObject.Texture ?? type.FullName.Replace('.', '/'), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                TileObjectTextures.Add(id, texture);
                TileObjectNames.Add(id, type.Name);
            }
        }
    }

    public class TileObject
    {
        public static Dictionary<string, TileObject> TileObjects;

        public int Durability { get; set; }
        public SoundStyle HitSound { get; set; } = SoundID.Dig;
        public SoundStyle DeathSound { get; set; } = SoundID.Dig;
        public SoundStyle GrabSound { get; set; } = SoundID.Dig;
        public int TileDust { get; set; } = DustID.Dirt;
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; }
        public int ItemID { get; set; }
        public int MinStack { get; set; }
        public int MaxStack { get; set; }
        public bool CanWiggle { get; set; } = true;
        public bool CanHighlight { get; set; } = true;
        public static TileObject GetTileObject(string type) => TileObjects[type];
        public virtual string Texture { get { return null; } private set { } }
        public virtual void SetDefaults() {}
    }
}