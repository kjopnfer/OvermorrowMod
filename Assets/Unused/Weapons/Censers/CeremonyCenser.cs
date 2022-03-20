using OvermorrowMod.Content.Projectiles.Censers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.WardenClass.Weapons.Censers
{
    public class CeremonyCenser : ModItem
    {        
        public override string Texture => "Terraria/Item_" + ItemID.CactusBathtub;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ceremonial Censer");
            Tooltip.SetDefault("el psy congroo");
        }

        public override void SetDefaults()
        {
            item.autoReuse = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTurn = true;
            item.useAnimation = 30;
            item.useTime = 30;
            item.knockBack = 0f;
            item.width = 36;
            item.height = 48;
            item.damage = 9;
            item.shootSpeed = 18f;
            item.shoot = ModContent.ProjectileType<CeremonyHead>();
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item71;
            item.noUseGraphic = true;
            item.channel = true;
            item.noMelee = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[item.shoot] < 1;
        }
    }
}