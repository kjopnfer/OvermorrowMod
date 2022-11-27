using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Core;
using System.Collections.Generic;
using System;

namespace OvermorrowMod.Common.TilePiles
{
    public class TileObjects
    {
        public static Dictionary<Type, int> TileObjectTypes;
        public static Dictionary<int, Texture2D> TileObjectTextures;
        public static Dictionary<int, string> TileObjectNames;

        public static Texture2D GetTexture(int type) => TileObjectTextures[type];
        public static int ObjectType<T>() where T : TileObject => TileObjectTypes[typeof(T)];

        public static void Load()
        {
            TileObjectTypes = new Dictionary<Type, int>();
            TileObjectTextures = new Dictionary<int, Texture2D>();
            TileObjectNames = new Dictionary<int, string>();
        }

        public static void Unload()
        {
            TileObjectTypes = null;
            TileObjectTextures = null;
            TileObjectNames = null;
        }

        public static void RegisterTileObject(Type type)
        {
            Type baseType = typeof(TileObject);
            if (type.IsSubclassOf(baseType) && !type.IsAbstract && type != baseType)
            {
                int id = TileObjectTypes.Count;
                TileObjectTypes.Add(type, id);

                TileObject tileObject = (TileObject)Activator.CreateInstance(type);
                tileObject.SetDefaults();
                TileObject.TileObjects.Add(id, tileObject);

                var texture = ModContent.Request<Texture2D>(tileObject.Texture ?? type.FullName.Replace('.', '/')).Value;
                TileObjectTextures.Add(id, texture);
                TileObjectNames.Add(id, type.Name);
            }
        }
    }

    public class TileObject
    {
        public static Dictionary<int, TileObject> TileObjects;

        private int _width;
        private int _height;
        private string _name;
        private int _itemID;
        private int _minStack;
        private int _maxStack;

        public int width
        {
            get => _width;
            set => _width = value;
        }

        public int height
        {
            get => _height;
            set => _height = value;
        }

        public string name
        {
            get => _name;
            set => _name = value;
        }

        public int itemID
        {
            get => _itemID;
            set => _itemID = value;
        }
        public int minStack
        {
            get => _minStack;
            set => _minStack = value;
        }

        public int maxStack
        {
            get => _maxStack;
            set => _maxStack = value;
        }

        public virtual string Texture { get { return null; } private set { } }
        public virtual void SetDefaults() {}
    }
}