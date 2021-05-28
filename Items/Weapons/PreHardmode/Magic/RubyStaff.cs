using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class RubyStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greater Ruby Staff");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.autoReuse = true;
            item.rare = ItemRarityID.Orange;
            item.mana = 11;
            item.UseSound = SoundID.Item43;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 20;
            item.useTurn = false;
            item.useAnimation = 32;
            item.useTime = 16;
            item.width = 64;
            item.height = 60;
            item.shoot = ProjectileID.RubyBolt;
            item.shootSpeed = 9f;
            item.knockBack = 3f;
            item.magic = true;
            item.value = Item.sellPrice(gold: 1, silver: 75);
        }
    }
}