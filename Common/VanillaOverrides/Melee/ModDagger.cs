using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.VanillaOverrides.Melee
{
    public enum DaggerAttack
    {
        Throw = -1,
        Slash = 0,
        Stab = 1,
    }

    public abstract class ModDagger<HeldProjectile, ThrownProjectile> : ModItem 
        where HeldProjectile : HeldDagger 
        where ThrownProjectile : ThrownDagger
    {
        public MeleeType MeleeType { get; } = MeleeType.Dagger;
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<HeldProjectile>()] <= 0 && player.ownedProjectileCounts[ModContent.ProjectileType<ThrownProjectile>()] < Item.stack;
        public override bool AltFunctionUse(Player player) => true;
        public sealed override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.autoReuse = true;
            Item.noUseGraphic = true; 
            Item.noMelee = true;
            Item.maxStack = 2;
            Item.shoot = ModContent.ProjectileType<HeldProjectile>();
            //Item.GetGlobalItem<GlobalGun>().MeleeType = MeleeType.Dagger;

            WeaponType.IsDagger[Type] = true;
            SafeSetDefaults();
        }

        public abstract List<DaggerAttack> AttackList { get; }
        public virtual void SafeSetDefaults() { }


        public int attackIndex = 0;
        bool isDualWielding => Item.stack == 2 && Main.LocalPlayer.ownedProjectileCounts[ModContent.ProjectileType<ThrownProjectile>()] < 1;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            attackIndex++;

            if (attackIndex > AttackList.Count - 1) attackIndex = 0;
            float attack = (int)AttackList[attackIndex];

            if (player.altFunctionUse == 2)
                Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, (int)DaggerAttack.Throw);
            else
            {
                Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, attack, 0f);

                if (isDualWielding)
                    Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, attack, 1f);
            }

            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            if (Item.stack == 2)
            {
                Color backKnifeColor = Main.LocalPlayer.ownedProjectileCounts[ModContent.ProjectileType<ThrownProjectile>()] == 2 ? Color.Black : drawColor;
                Color frontKnifeColor = Main.LocalPlayer.ownedProjectileCounts[ModContent.ProjectileType<ThrownProjectile>()] >= 1 ? Color.Black : drawColor;

                spriteBatch.Draw(texture, position, frame, backKnifeColor, 0f, origin, scale, SpriteEffects.FlipHorizontally, 1);
                spriteBatch.Draw(texture, position, frame, frontKnifeColor, 0f, origin, scale, SpriteEffects.None, 1);
            }
            else
            {
                Color color = Main.LocalPlayer.ownedProjectileCounts[ModContent.ProjectileType<ThrownProjectile>()] < 1 ? drawColor : Color.Black;
                spriteBatch.Draw(texture, position, frame, color, 0f, origin, scale, SpriteEffects.None, 1);
            }

            return false;
        }
    }
}