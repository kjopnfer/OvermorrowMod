using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.NPCs
{
    /// <summary>
    /// <para>This NPC handles opening an NPC's shop from the dialogue.</para>
    /// The player's ID is stored within the NPC's first AI index, and will die if the player is no longer within the shop.
    /// </summary>
    public class ShopHandler : ModNPC
    {
        public static Chest NPCShop = new Chest();

        public override string Texture => AssetDirectory.Empty;
        public override bool? CanBeHitByItem(Player player, Item item) => false;
        public override bool? CanBeHitByProjectile(Projectile projectile) => false;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.height = 1;
            NPC.width = 1;
            NPC.lifeMax = 1;
            NPC.trapImmune = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.behindTiles = true;
            NPC.alpha = 0;
            NPC.hide = true;
            NPC.aiStyle = -1;
        }

        public override void AI()
        {
            if (NPC.ai[0] == Main.myPlayer)
            {
                Player player = Main.player[Main.myPlayer];
                //if (player.talkNPC == NPC.whoAmI) Main.NewText("talking");

                // Kill the NPC if the player has left the shop
                if (!Main.playerInventory || player.talkNPC == -1)
                {
                    NPC.ai[0] = -1;
                    Main.playerInventory = false;
                    NPC.Kill();
                }
            }
        }

        public override void ModifyActiveShop(string shopName, Item[] items)
        {
            // I forgot what this does
            /*for (int k = 0; k < NPCShop.item.Length && k < shop.item.Length; k++)
            {
                shop.item[k] = NPCShop.item[k];
                nextSlot++;
            }*/
        }
    }
}