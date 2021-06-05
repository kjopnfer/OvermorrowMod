using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class EmeraldStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greater Emerald Staff");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Orange;
            item.mana = 9;
            item.UseSound = SoundID.Item43;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 16;
            item.useTurn = false;
            item.useAnimation = 22;
            item.useTime = 11;
            item.width = 48;
            item.height = 48;
            item.shoot = ProjectileID.EmeraldBolt;
            item.shootSpeed = 12f;
            item.knockBack = 5f;
            item.magic = true;
            item.value = Item.sellPrice(gold: 1, silver: 75);
        }
    }
}