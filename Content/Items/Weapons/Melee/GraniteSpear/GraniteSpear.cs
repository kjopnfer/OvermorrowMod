using OvermorrowMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.GraniteSpear
{
    public class GraniteSpear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Pike");
            Tooltip.SetDefault("Striking enemies temporarily boosts your minion damage by 10%");
        }

        public override void SetDefaults()
        {
            //item.autoReuse = true;
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item1;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 17;
            item.scale = 1.1f;
            item.useAnimation = 20;
            item.useTime = 20;
            item.width = 58;
            item.height = 58;
            item.shoot = ModContent.ProjectileType<GraniteSpearProjectile>();
            item.shootSpeed = 4f;
            item.knockBack = 3.9f;
            item.melee = true;
            item.value = Item.sellPrice(gold: 1);
            item.noUseGraphic = true;
        }

        public override bool CanUseItem(Player player)
        {
            // Ensures no more than one spear can be thrown out, use this when using autoReuse
            return player.ownedProjectileCounts[item.shoot] < 1;
        }
    }
}