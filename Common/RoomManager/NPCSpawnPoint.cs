using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Common.RoomManager
{
    public class NPCSpawnPoint : ModTileEntity
    {
        /// <summary>
        /// The room that this spawn point is associated with.
        /// </summary>
        public Room Room { get; set; }

        /// <summary>
        /// The NPC that this spawn point should create.
        /// </summary>
        public int NPCType { get; set; }

        /// <summary>
        /// The NPC that this spawner is keeping track of.
        /// </summary>
        public NPC ChildNPC { get; private set; } = null;

        /// <summary>
        /// This is set to true if the associated NPC has been killed.
        /// </summary>
        public bool HasBeenCleared { get; private set; } = false;

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
        }

        public void SpawnNPC()
        {
            ChildNPC = NPC.NewNPCDirect(null, Position.ToWorldCoordinates(), NPCType);
        }

        public override void Update()
        {
            if (ChildNPC != null)
            {
                if (ChildNPC.active)
                {
                    Main.NewText("active");
                }
                else
                {
                    ChildNPC = null;
                    HasBeenCleared = true;
                }
            }

            base.Update();
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            return true;
        }
    }
}