using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.TextureMapping;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.RoomManager
{
    public abstract class Room
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        protected abstract Texture2D Tiles { get; }
        protected abstract Dictionary<Color, int> TileMapping { get; }

        protected abstract Texture2D Walls { get; }
        protected abstract Dictionary<Color, int> WallMapping { get; }
        protected abstract Texture2D Liquids { get; }
        protected abstract Texture2D Slopes { get; }

        protected abstract Texture2D Objects { get; }
        protected abstract Dictionary<Color, (int, int)> ObjectMapping { get; }


        public List<NPCSpawnPoint> SpawnPoints { get; private set; } = new();
        public Room() {
            Width = Tiles.Width;
            Height = Tiles.Height;
        }

        /// <summary>
        /// Called after the Texture Mapping has been run on the Main Thread.
        /// Used for when the objects don't generate from the object texture some apparent reason??
        /// </summary>
        public virtual void PostGenerate(int x, int y) { }

        public void Generate(Vector2 position)
        {
            SystemUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = TexGen.GetTexGenerator(Tiles, TileMapping, Walls, WallMapping, Liquids, Slopes, Objects, ObjectMapping);

                gen.Generate((int)position.X, (int)position.Y, true, true);
            });

            PostGenerate((int)position.X, (int)position.Y);
        }

        /// <summary>
        /// Create a new ModTileEntity that spawns a particular NPC at the position.
        /// </summary>
        /// <param name="position">The tile coordinates that the ModTileEntity is created.</param>
        /// <param name="npcType">The ID of the NPC that is spawned by the ModTileEntity</param>
        public void AddSpawnPoint(Vector2 position, int npcType)
        {
            int entityID = ModContent.GetInstance<NPCSpawnPoint>().Place((int)position.X, (int)position.Y);
            NPCSpawnPoint spawnPoint = TileEntity.ByID[entityID] as NPCSpawnPoint;
            spawnPoint.NPCType = npcType;

            SpawnPoints.Add(spawnPoint);
        }

        /// <summary>
        /// Calls all SpawnPoints and respawns their associated NPCs.
        /// </summary>
        public void RespawnNPCs()
        {
            if (HasRoomBeenCleared())
            {
                foreach (NPCSpawnPoint spawn in SpawnPoints)
                    spawn.SpawnNPC();
            }
        }

        /// <summary>
        /// Loops through all of the SpawnPoints and checks if their NPC is active.
        /// </summary>
        public bool HasRoomBeenCleared()
        {
            foreach (NPCSpawnPoint spawnPoint in SpawnPoints)
            {
                if (spawnPoint.ChildNPC.active)
                    return false;
            }

            return true;
        }
    }
}