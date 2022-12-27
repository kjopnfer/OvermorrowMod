using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Overmorrow.Common;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using OvermorrowMod.Content.Buffs.Debuffs;
using OvermorrowMod.Content.UI;
using OvermorrowMod.Content.Projectiles.Misc;

namespace OvermorrowMod.Common.VanillaOverrides.Bow
{
    public class TestBow_Held : HeldBow
    {

    }

    public class TestBow : ModItem
    {
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<TestBow_Held>()] <= 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Test Held Bow");
        }

        public override void SetDefaults()
        {
            Item.damage = 89;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5;

            Item.UseSound = SoundID.Item5;
            Item.width = 32;
            Item.height = 74;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TestBow_Held>();
            Item.shootSpeed = 10f;
            Item.noUseGraphic = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = Item.shoot;
        }
    }
}