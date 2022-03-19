using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using OvermorrowMod.Projectiles.Melee;

namespace OvermorrowMod.Content.Items.Weapons.PreHardmode.Melee
{
    public class SearingSaber : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Searing Saber");
            Tooltip.SetDefault("he-he ho-ho ha-ha");
        }

        public override void SetDefaults()
        {
            item.damage = 38;
            item.melee = true; //Im on meth
            item.width = 44;
            item.height = 44;
            item.useTime = 10;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noUseGraphic = true;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.LightRed; //I bumped up the rarity bc rare drop, cope and seethe
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<YourMomIsHotter>();
            item.shootSpeed = 18f;
            item.channel = true;
        }
    }
}
