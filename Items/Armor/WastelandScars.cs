
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;


namespace OvermorrowMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class WastelandScars : ModItem
    {

        int BradTimer = 0;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("3% increased demo damage \n'It feels painful'");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10000;
            item.rare = ItemRarityID.Orange;
            item.defense = 6;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<BradPoncho>() && legs.type == ItemType<WastelandPants>();
        }

        public override void UpdateEquip(Player player)
        {
            player.allDamage += 0.03f;
        }
        public override void UpdateArmorSet(Player player)
        {
            BradTimer++;
            if (BradTimer < 1250)
            {
                item.defense = 16;
                player.allDamage += 0.20f;
            }
            player.setBonus = "20% more damage, you suffer withdraw sometimes";
            if (BradTimer > 1250)
            {
                item.defense = 0;
                player.AddBuff(ModContent.BuffType<Withdraw>(), 10);
            }
            if (BradTimer == 2000)
            {
                BradTimer = 0;
            }
        }
    }
}