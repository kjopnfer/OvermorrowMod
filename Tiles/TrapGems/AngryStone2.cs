using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Tiles.TrapGems
{
    public class AngryStone2 : ModNPC
    {
        readonly bool expert = Main.expertMode;
        public override void SetDefaults()
        {
            npc.width = 32;
            npc.height = 26;
            npc.noTileCollide = false;
            npc.noGravity = false;
            npc.aiStyle = 3;
            npc.damage = 10;
            npc.defense = 9;
            npc.lifeMax = 50;
            npc.HitSound = SoundID.NPCHit4;
            npc.value = 12f;
            animationType = NPCID.Zombie;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(36, MethodHelper.SecondsToTicks(60));
        }
        public override void NPCLoot()
        {
            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("StoneFragment"));

            if(Main.hardMode == true && expert)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("StoneSoul"));
            }
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Angry Stone");
            Main.npcFrameCount[npc.type] = 3;
        }
    }
}

