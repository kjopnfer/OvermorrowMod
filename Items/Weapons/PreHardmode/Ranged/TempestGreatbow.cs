using Microsoft.Xna.Framework;
using OvermorrowMod.NPCs.Bosses.StormDrake;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Ranged
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
            //item.autoReuse = true;
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item5;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 46;
            item.useAnimation = 50;
            item.useTime = 50;
            item.width = 40;
            item.height = 66;
            //item.shoot = ModContent.ProjectileType<StormBolt>();
            item.shoot = ModContent.ProjectileType<StormBolt2>();
            item.shootSpeed = 30f;
            item.knockBack = 10f;
            item.ranged = true;
            item.value = Item.sellPrice(gold: 1);
            item.useAmmo = AmmoID.Arrow;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
            {
                type = ModContent.ProjectileType<StormBolt2>();
            }
            return true;
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-8, 0);
        }
    }
}