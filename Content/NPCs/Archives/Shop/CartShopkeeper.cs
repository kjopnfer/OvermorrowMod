using OvermorrowMod.Common;
using OvermorrowMod.Core.Shop;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Archives.Shop
{
    public class CartShopkeeper : ModNPC
    {
        public const string ShopName = "Shop";

        public const int DialogueLineCount = 6;

        public override string Texture => AssetDirectory.NPCs + "ShopCart";

        /// <summary>
        /// One of the shopkeeper's localized greeting lines, chosen at random.
        /// </summary>
        public static string RandomLine()
        {
            return Language.GetTextValue(LocalizationPath.Dialogue + "CartShopkeeper.Line" + Main.rand.Next(DialogueLineCount));
        }

        /// <summary>
        /// Fixed horizontal facing set at spawn from the room's open side (-1 left, 1 right).
        /// </summary>
        public int Facing = 1;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 120;
            NPC.height = 120;
            NPC.friendly = true;
            NPC.townNPC = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.lifeMax = 250;
            NPC.defense = 30;
            NPC.aiStyle = -1;
            NPC.npcSlots = 0f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            NPC.GivenName = "Cart";
            ShopRun.GetStock(Main.LocalPlayer);
        }

        public override void AI()
        {
            NPC.velocity.X = 0f;
            NPC.direction = Facing;
            NPC.spriteDirection = Facing;
        }

        public override bool CanChat() => true;

        public override string GetChat() => RandomLine();

        public override void AddShops()
        {
            new NPCShop(Type, ShopName)
                .Add(ItemID.HealingPotion)
                .Register();
        }

        public override void ModifyActiveShop(string shopName, Item[] items) => FillShop(items);

        /// <summary>
        /// Replaces the shop slots with this run's rolled stock, skipping already-purchased items.
        /// </summary>
        public void FillShop(Item[] items)
        {
            var stock = ShopRun.GetStock(Main.LocalPlayer);

            int slot = 0;
            foreach (var (type, price) in stock)
            {
                if (ShopRun.Purchased.Contains(type)) continue;
                if (slot >= items.Length) break;
                items[slot] = new Item(type) { shopCustomPrice = price };
                slot++;
            }

            for (; slot < items.Length; slot++)
                items[slot] = new Item();
        }

        public bool WasPurchased(int type) => ShopRun.Purchased.Contains(type);

        public void MarkPurchased(int type) => ShopRun.Purchased.Add(type);
    }
}
