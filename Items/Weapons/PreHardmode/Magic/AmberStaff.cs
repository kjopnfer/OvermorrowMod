using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class AmberStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greater Amber Staff");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Orange;
            item.mana = 10;
            item.UseSound = SoundID.Item43;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 18;
            item.useTurn = false;
            item.useAnimation = 28;
            item.useTime = 14;
            item.width = 48;
            item.height = 48;
            item.shoot = ProjectileID.AmberBolt;
            item.shootSpeed = 14f;
            item.knockBack = 6f;
            item.magic = true;
            item.value = Item.sellPrice(gold: 1, silver: 75);
        }
    }
}