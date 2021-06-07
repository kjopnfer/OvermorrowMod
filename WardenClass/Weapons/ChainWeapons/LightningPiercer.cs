using Microsoft.Xna.Framework;
using OvermorrowMod.Buffs.RuneBuffs;
using OvermorrowMod.Projectiles.Piercing;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Weapons.ChainWeapons
{
    public class LightningPiercer : PiercingItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heaven's Rope");
            Tooltip.SetDefault("Attacks have a chance to Electrified\n[c/00FF00:{ Imbuement }]\n" +
                            "[c/800080:Right Click] to cause all attacks to launch an electrically charged chain\nConsumes 1 Soul Essence");
        }

        public override void SafeSetDefaults()
        {
            item.autoReuse = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 14;
            item.useTime = 14;
            item.knockBack = 0f;
            item.width = 38;
            item.height = 48;
            item.damage = 6;
            item.shootSpeed = 18f;
            item.shoot = mod.ProjectileType("LightningPiercerProjectile");
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item71;
            item.noUseGraphic = true;

            soulGainChance = 2;
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
                item.damage = 6;
                item.shootSpeed = 0f;
                item.shoot = ProjectileID.None;
                item.UseSound = SoundID.DD2_WitherBeastAuraPulse;

                ConsumeSouls(1, player);
                player.GetModPlayer<WardenRunePlayer>().ActiveRune = true;
                player.AddBuff(ModContent.BuffType<LightningRune>(), 600);
            }
            else
            {
                item.autoReuse = true;
                item.useStyle = ItemUseStyleID.SwingThrow;
                item.useTurn = true;
                item.useAnimation = 14;
                item.useTime = 14;
                item.damage = 6;
                item.shootSpeed = 18f + modPlayer.modifyShootSpeed();
                item.shoot = mod.ProjectileType("LightningPiercerProjectile");
                item.UseSound = SoundID.Item71;
            }

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.GetModPlayer<WardenRunePlayer>().RuneID == 3 && !player.GetModPlayer<WardenRunePlayer>().runeDeactivate)
            {
                type = ModContent.ProjectileType<LightningPiercerProjectileAlt>();
            }
            return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HellstoneBar, 24);
            recipe.AddIngredient(ItemID.Chain, 12);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}