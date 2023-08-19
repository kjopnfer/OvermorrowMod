using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.VanillaOverrides;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using OvermorrowMod.Common.VanillaOverrides.Melee;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace OvermorrowMod.Content.Items.Weapons.Melee
{
    public class Knife : ModDagger<Knife_Held, Knife_Thrown>
    //public class Knife : ModItem
    {
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0 && player.ownedProjectileCounts[ModContent.ProjectileType<Knife_Thrown>()] < Item.stack;

        public override void SafeSetDefaults()
        {
            Item.damage = 10;
            Item.knockBack = 2f;
            Item.useAnimation = 12;
            Item.useTime = 12;
            Item.width = 32;
            Item.height = 32;
            Item.crit = 20;
            Item.shootSpeed = 2.1f;

            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(0, 0, 0, 10);
        }


        public int attackIndex = 1;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            attackIndex++;
            if (attackIndex > 1) attackIndex = 0;

            attackIndex = 0;
            if (player.altFunctionUse == 2)
                Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, -1);
            else
            {
                bool dualWieldFlag = Item.stack == 2 && player.ownedProjectileCounts[ModContent.ProjectileType<Knife_Thrown>()] < 1;

                Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, attackIndex, 0f);

                float invertedAttackIndex = 0;
                //float invertedAttackIndex = attackIndex == 1 ? 0 : 1;
                if (dualWieldFlag)
                    Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, invertedAttackIndex, 1f);
            }

            return false;
        }
    }

    public class Knife_Held : HeldDagger
    {
        public override int ParentItem => ModContent.ItemType<Knife>();
        public override int ThrownProjectile => ModContent.ProjectileType<Knife_Thrown>();
        public override void SafeSetDefaults()
        {
            base.SafeSetDefaults();
        }
    }

    public class Knife_Thrown : ThrownDagger
    {
        public override int ParentItem => ModContent.ItemType<Knife>();        
    }
}