﻿/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Content.NPCs.Carts
{
    public class Cart : ModNPC
    {
        public List<Item> shopItems = new List<Item>();
        public override bool CheckActive() => false;
        public override bool CanTownNPCSpawn(int numTownNPCs) => false;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Merchant Cart");
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 180;
            NPC.height = 150;
            NPC.friendly = true;
            NPC.dontTakeDamage = true;
            NPC.lifeMax = 100;
            NPC.aiStyle = -1;
            NPC.townNPC = true;
            TownNPCStayingHomeless = true;
        }
   
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            
        }

        public override string GetChat()
        {
            List<string> dialogue = new List<string>
            {
                "dialogue 1",
                "dialogue 2",
                "dialogue 3"
            };

            return Main.rand.Next(dialogue);
        }


        public override void AI()
        {
            NPC.homeless = true; // Make sure it stays homeless
        }

        public void SetupInventory()
        {
            var itemIDs = new List<int>();

            itemIDs.Add(ItemID.Banana);
            itemIDs.Add(ItemID.Banana);
            itemIDs.Add(ItemID.AcidDye);
            itemIDs.Add(ItemID.Banana);
            itemIDs.Add(ItemID.Banana);
            itemIDs.Add(ItemID.Banana);

            // convert to a list of items
            shopItems = new List<Item>();
            foreach (int itemID in itemIDs)
            {
                Item item = new Item();
                item.SetDefaults(itemID);
                shopItems.Add(item);
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag["itemIDs"] = shopItems;
        }

        public override void LoadData(TagCompound tag)
        {
            shopItems = tag.Get<List<Item>>("shopItems");
        }

        public override void AddShops()
        {
            var shop = new NPCShop(Type)
                .Add(ItemID.ChainKnife)
                .Add(ItemID.Banana)
                .Add(ItemID.AcidDye);
        }
    }
}
*/