using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.PreHardmode.Ranged
{
    public class SandThrower : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sandthrower");
            Tooltip.SetDefault("Uses gel for ammo\n'I'd use sand for ammo but that's annoying'");
        }

        public override void SetDefaults()
        {
            item.width = 54;
            item.height = 20;
            item.useTime = 10;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.rare = ItemRarityID.Orange;
            item.damage = 17;
            item.ranged = true;
            item.shoot = ModContent.ProjectileType<SandThrowerProjectile>();
            item.shootSpeed = 4.5f;
            item.useAmmo = AmmoID.Gel;
            item.noMelee = true;
            item.UseSound = SoundID.Item34;
            item.value = Item.buyPrice(gold: 1, silver: 75);
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, 0f);
        }
    }
}