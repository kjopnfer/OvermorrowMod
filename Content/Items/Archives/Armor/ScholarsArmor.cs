using OvermorrowMod.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Armor.Kagenoi
{

    [AutoloadEquip(EquipType.Body)]
    public class ScholarsArmor : ModItem
    {
        public override string Texture => AssetDirectory.ArchiveItems + Name;
        public override bool IsArmorSet(Item head, Item body, Item legs) => legs.type == ModContent.ItemType<ScholarsRobes>();

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 20;
        }

        public override void UpdateEquip(Player player)
        {
            //player.GetDamage(DamageClass.Melee) += 0.05f;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class ScholarsRobes : ModItem
    {
        public override string Texture => AssetDirectory.ArchiveItems + Name;

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 20;
        }

        public override void UpdateEquip(Player player)
        {
            //player.GetDamage(DamageClass.Melee) += 0.05f;
        }
    }
}