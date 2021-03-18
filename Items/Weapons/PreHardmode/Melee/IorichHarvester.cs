using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Melee
{
    public class IorichHarvester : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Harvester of Iorich");
            Tooltip.SetDefault("Throws a scythe that returns to your hand\n" +
                "'During The Great War of Cthulhu, it is said that Iorich alone repelled \n" +
                " an army of a thousand troops before they could lay siege to the towns'");
        }

        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.damage = 12;
            item.useTime = 20;
            item.useAnimation = 20;
            item.width = 58;
            item.height = 48;
            item.shoot = ModContent.ProjectileType<IorichHarvesterProjectile>();
            item.shootSpeed = 8f;
            item.knockBack = 3;
            item.melee = true;
            item.autoReuse = true;
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