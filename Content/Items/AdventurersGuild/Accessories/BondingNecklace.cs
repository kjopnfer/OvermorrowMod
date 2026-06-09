using OvermorrowMod.Common;
using OvermorrowMod.Common.Items;
using OvermorrowMod.Core.Items.Accessories;
using OvermorrowMod.Core.Loot;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.AdventurersGuild.Accessories
{
    [AutoloadEquip(EquipType.Neck)]
    [Loot(ItemType.Summon, Rarity.Rare)]
    public class BondingNecklace : OvermorrowAccessory
    {
        public override string Texture => AssetDirectory.GuildItems + Name;

        protected override void SafeSetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, gold: 1, silver: 50, copper: 0);
        }

        protected override void UpdateAccessoryEffects(Player player)
        {
            player.maxMinions += 1;

            int minionCount = 0;
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.owner == player.whoAmI && p.minion)
                    minionCount++;
            }
            player.statLifeMax2 += minionCount * 2;
        }

        protected override void SetAccessoryEffects(AccessoryDefinition definition) { }
    }
}
