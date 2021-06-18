using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Items.Weapons.Testing
{
    public class StableReprogrammedDroidSpawner : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reprogrammed Probe Spawner");
            Tooltip.SetDefault("Summons Probes that fight for you \nTo aim the laser use a summon stick \nWill not activate unless enemies are arround and you are using the summon stick");
        }
        public override void SetDefaults()
        {

            item.width = 32;
            item.height = 32;
            item.damage = 55;
            item.summon = true;
            item.noMelee = true;
            item.useTime = 44;
            item.useAnimation = 44;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.knockBack = 0;
            item.shoot = mod.ProjectileType("ProbePROJ");
            item.shootSpeed = 11f;
        }
    }
}
