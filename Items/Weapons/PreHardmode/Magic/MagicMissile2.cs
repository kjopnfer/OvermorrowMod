using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.IO;
using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Magic;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class MagicMissile2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magic Missile II");
        }
        public override void SetDefaults()
        {
            item.damage = 25;
            item.magic = true;
            item.mana = 14;
            item.width = 26;
            item.height = 26;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.channel = true;
            item.knockBack = 8;
            item.value = Item.sellPrice(silver: 50);
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item9;
            item.shoot = ModContent.ProjectileType<MMProjectile>();
            item.shootSpeed = 10f;
        }
    }
}
