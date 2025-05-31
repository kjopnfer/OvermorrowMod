using OvermorrowMod.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Armor
{

    [AutoloadEquip(EquipType.Head)]
    public class ScholarsHat : ModItem
    {
        public override string Texture => AssetDirectory.ArchiveItems + Name;
        public override void SetStaticDefaults()
        {
            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
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

    [AutoloadEquip(EquipType.Body)]
    public class ScholarsArmor : ModItem
    {
        public override string Texture => AssetDirectory.ArchiveItems + Name;
        public override bool IsArmorSet(Item head, Item body, Item legs) => head.type == ModContent.ItemType<ScholarsHat>() && legs.type == ModContent.ItemType<ScholarsRobes>();

        public override void SetStaticDefaults()
        {
            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
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