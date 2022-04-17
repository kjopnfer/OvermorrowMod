using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.ThunderClap
{
    public class ThunderClap : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("ThunderClap");
            Tooltip.SetDefault("Throws a flail that shocks nearby enemies\n'Begone with the thunder clap'");
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.rare = ItemRarityID.Orange;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.knockBack = 4f;
            Item.damage = 24;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<ThunderClapHead>();
            Item.shootSpeed = 15.1f;
            Item.UseSound = SoundID.Item1;
            Item.DamageType = DamageClass.Melee;
            Item.crit = 9;
            Item.channel = true;
            Item.value = Item.sellPrice(silver: 5);
        }
    }
}