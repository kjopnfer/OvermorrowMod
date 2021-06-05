using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class AmethystStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greater Amethyst Staff");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Orange;
            item.mana = 6;
            item.UseSound = SoundID.Item43;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 12;
            item.useTurn = false;
            item.useAnimation = 34;
            item.useTime = 17;
            item.width = 48;
            item.height = 44;
            item.shoot = ProjectileID.AmethystBolt;
            item.shootSpeed = 9f;
            item.knockBack = 5f;
            item.magic = true;
            item.value = Item.sellPrice(gold: 1, silver: 75);
        }
    }
}