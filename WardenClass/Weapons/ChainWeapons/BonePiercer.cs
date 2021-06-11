using Microsoft.Xna.Framework;
using OvermorrowMod.Buffs.RuneBuffs;
using OvermorrowMod.Items.Materials;
using OvermorrowMod.Projectiles.Piercing;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Weapons.ChainWeapons
{
    public class BonePiercer : PiercingItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grave Hook");
            Tooltip.SetDefault("[c/00FF00:{ Imbuement }]\n" +
                            "[c/800080:Right Click] to cause all attacks to explode into homing souls on hit\nConsumes 1 Soul Essence" +
                            "\n'Just gotta put all the femurs together...'");
        }

        public override void SafeSetDefaults()
        {
            item.autoReuse = false;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 14;
            item.useTime = 14;
            item.knockBack = 0f;
            item.width = 38;
            item.height = 50;
            item.damage = 8;
            item.shootSpeed = 14f;
            item.shoot = mod.ProjectileType("BonePiercerProjectile");
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item1;
            item.noUseGraphic = true;

            soulGainChance = 3;
        }

        public override bool CanUseItem(Player player)
        {
            // Get the class info from the player
            var modPlayer = WardenDamagePlayer.ModPlayer(player);

            if (player.altFunctionUse == 2 && modPlayer.soulResourceCurrent > 0 && player.GetModPlayer<WardenRunePlayer>().RuneID == 0)
            {
                item.useStyle = ItemUseStyleID.HoldingUp;
                item.useAnimation = 45;
                item.useTime = 45;
                item.damage = 8;
                item.shootSpeed = 0f;
                item.shoot = ProjectileID.None;
                item.UseSound = SoundID.DD2_WitherBeastAuraPulse;

                ConsumeSouls(1, player);
                player.GetModPlayer<WardenRunePlayer>().ActiveRune = true;
                player.AddBuff(ModContent.BuffType<BoneRune>(), 600);
            }
            else
            {
                item.useStyle = ItemUseStyleID.SwingThrow;
                item.useTurn = true;
                item.useAnimation = 14;
                item.useTime = 14;
                item.damage = 8;
                item.shootSpeed = 14f + modPlayer.modifyShootSpeed();
                item.shoot = mod.ProjectileType("BonePiercerProjectile");
                item.UseSound = SoundID.Item1;
            }

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.GetModPlayer<WardenRunePlayer>().RuneID == 2 && !player.GetModPlayer<WardenRunePlayer>().runeDeactivate)
            {
                type = ModContent.ProjectileType<BonePiercerProjectileAlt>();
            }
            return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ChainKnife);
            recipe.AddIngredient(ModContent.ItemType<SoulFire>());
            recipe.AddIngredient(ItemID.Bone, 125);
            recipe.AddIngredient(ItemID.Cobweb, 40);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}