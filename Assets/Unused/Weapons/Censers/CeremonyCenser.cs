using OvermorrowMod.Content.Projectiles.Censers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.WardenClass.Weapons.Censers
{
    public class CeremonyCenser : ModItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.CactusBathtub;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ceremonial Censer");
            Tooltip.SetDefault("el psy congroo");
        }

        public override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTurn = true;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.knockBack = 0f;
            Item.width = 36;
            Item.height = 48;
            Item.damage = 9;
            Item.shootSpeed = 18f;
            Item.shoot = ModContent.ProjectileType<CeremonyHead>();
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item71;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.noMelee = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
    }
}