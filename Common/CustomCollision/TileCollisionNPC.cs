using CollisionLib;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.CustomCollision
{
    public abstract class TileCollisionNPC : ModNPC
    {
        public Tile parentTile;
        public CollisionSurface[] colliders = null;
        public override bool CheckActive() => false;
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public virtual void SafeSetDefaults() { }
        public sealed override void SetDefaults()
        {
            NPC.width = 1;
            NPC.height = 1;
            NPC.lifeMax = 1000;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.ShowNameOnHover = false;

            SafeSetDefaults();
        }
    }
}