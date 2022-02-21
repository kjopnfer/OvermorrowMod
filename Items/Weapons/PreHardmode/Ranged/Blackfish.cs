using Terraria;
using System;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Ranged
{
    public class Blackfish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blackfish");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 22;
            item.ranged = true;
            item.autoReuse = true;
            item.useTime = 5;
            item.useAnimation = 5;
            item.useAmmo = AmmoID.Bullet;
            item.damage = 11;
            item.crit = 3;
            item.useStyle = 5;
            item.shootSpeed = 5f;
            item.noMelee = true;
            item.shoot = 10;
            item.useTurn = true;
            item.UseSound = SoundID.Item11;
        }

    }
}
