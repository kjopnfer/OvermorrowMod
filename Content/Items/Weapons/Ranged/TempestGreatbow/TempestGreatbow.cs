using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.TempestGreatbow
{
    public class TempestGreatbow : ModItem
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tempest Greatbow");
            Tooltip.SetDefault("Transforms wooden arrows into Stormbolts\n'Bring on The Thundah!'");
        }

        public override void SetDefaults()
        {
            //Item.autoReuse = true;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item5;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 46;
            Item.useAnimation = 50;
            Item.useTime = 50;
            Item.width = 40;
            Item.height = 66;
            //Item.shoot = ModContent.ProjectileType<StormBolt>();
            Item.shoot = ModContent.ProjectileType<StormBolt>();
            Item.shootSpeed = 30f;
            Item.knockBack = 10f;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(gold: 1);
            Item.useAmmo = AmmoID.Arrow;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
            {
                type = ModContent.ProjectileType<StormBolt>();
            }
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-8, 0);
        }
    }
}