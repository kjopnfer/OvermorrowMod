using OvermorrowMod.Projectiles.Magic.CreepingD;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.PreHardmode.Magic
{
    public class CreepingDeath : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Harvester of Sorrow");
            Tooltip.SetDefault("Shoots a blood ring around you.\nEnemies hit allow you to fires iron bolts\n'Language of the mad'");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Orange;
            item.mana = 6;
            item.UseSound = SoundID.Item43;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 19;
            item.useTurn = false;
            item.useAnimation = 35;
            item.useTime = 35;
            item.width = 48;
            item.height = 48;
            item.shoot = ModContent.ProjectileType<IronBloodCircle>();
            item.shootSpeed = 0f;
            item.knockBack = 4.5f;
            item.magic = true;
            item.value = Item.sellPrice(gold: 1, silver: 75);
        }
    }
}