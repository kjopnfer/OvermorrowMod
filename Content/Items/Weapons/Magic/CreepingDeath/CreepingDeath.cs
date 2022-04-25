using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.CreepingDeath
{
    public class CreepingDeath : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Harvester of Sorrow");
            Tooltip.SetDefault("Shoots a blood ring around you.\nEnemies hit allow you to fires iron bolts\n'Language of the mad'");
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Orange;
            Item.mana = 6;
            Item.UseSound = SoundID.Item43;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 19;
            Item.useTurn = false;
            Item.useAnimation = 35;
            Item.useTime = 35;
            Item.width = 48;
            Item.height = 48;
            Item.shoot = ModContent.ProjectileType<IronBloodCircle>();
            Item.shootSpeed = 0f;
            Item.knockBack = 4.5f;
            Item.DamageType = DamageClass.Magic;
            Item.value = Item.sellPrice(gold: 1, silver: 75);
        }
    }
}