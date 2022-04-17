using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.SandThrower
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
            Item.width = 54;
            Item.height = 20;
            Item.useTime = 10;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Orange;
            Item.damage = 17;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ModContent.ProjectileType<SandThrowerProjectile>();
            Item.shootSpeed = 4.5f;
            Item.useAmmo = AmmoID.Gel;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item34;
            Item.value = Item.buyPrice(gold: 1, silver: 75);
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, 0f);
        }
    }
}