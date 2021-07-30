using Microsoft.Xna.Framework;
using OvermorrowMod.Buffs.RuneBuffs;
using OvermorrowMod.Projectiles.Piercing;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Weapons.ChainWeapons
{
    public class VinePiercer : PiercingItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stingvine");
            Tooltip.SetDefault("Attacks have a chance to inflict Poisoned\n" +
                            "[c/09DBB8:{ Imbuement }]\n" +
                            "[c/800080:Right Click] to empower your Warden Artifacts on use\n" +
                            "[c/DE3A28:{ Power }] Your Artifact summons spawn Spores on hit\n" +
                            "Consumes 1 Soul Essence\n" +
                            "'Float like a butterfly, sting like a vine'");
        }

        public override void SafeSetDefaults()
        {
            item.autoReuse = false;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 14;
            item.useTime = 14;
            item.knockBack = 0f;
            item.width = 36;
            item.height = 46;
            item.damage = 5;
            item.shootSpeed = 14f;
            item.shoot = mod.ProjectileType("VinePiercerProjectile");
            item.rare = ItemRarityID.Orange;
            item.UseSound = new LegacySoundStyle(SoundID.Grass, 0); // Grass
            item.noUseGraphic = true;

            soulGainChance = 5;
        }

        public override bool CanUseItem(Player player)
        {
            // Get the class info from the player
            var modPlayer = WardenDamagePlayer.ModPlayer(player);

            if (player.altFunctionUse == 2 && modPlayer.soulResourceCurrent >= 1 && player.GetModPlayer<WardenRunePlayer>().RuneID == WardenRunePlayer.Runes.None)
            {
                item.useStyle = ItemUseStyleID.HoldingUp;
                item.useAnimation = 45;
                item.useTime = 45;
                item.damage = 5;
                item.shootSpeed = 0f;
                item.shoot = ProjectileID.None;
                item.UseSound = SoundID.DD2_WitherBeastAuraPulse;

                ConsumeSouls(1, player);
                player.GetModPlayer<WardenRunePlayer>().ActiveRune = true;
                player.AddBuff(ModContent.BuffType<VineRune>(), 600);
            }
            else
            {
                item.useStyle = ItemUseStyleID.HoldingOut;
                item.useAnimation = 14;
                item.useTime = 14;
                if (player.GetModPlayer<WardenRunePlayer>().RuneID == WardenRunePlayer.Runes.JungleRune && !player.GetModPlayer<WardenRunePlayer>().runeDeactivate) 
                {
                    item.damage = 16;
                    item.useTurn = false;
                    item.shootSpeed = 6f;
                    item.autoReuse = true;
                    item.channel = true;
                }
                else
                {
                    item.damage = 5;
                    item.useTurn = true;
                    item.shootSpeed = 14f + modPlayer.modifyShootSpeed();
                    item.autoReuse = false;
                    item.channel = false;
                }
                item.shoot = mod.ProjectileType("VinePiercerProjectile");
                item.UseSound = new LegacySoundStyle(SoundID.Grass, 0); // Grass

            }

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.GetModPlayer<WardenRunePlayer>().RuneID == WardenRunePlayer.Runes.JungleRune && !player.GetModPlayer<WardenRunePlayer>().runeDeactivate)
            {
                type = ModContent.ProjectileType<JunglePiercer>();
            }
            return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ChainKnife);
            recipe.AddIngredient(ItemID.Stinger, 18);
            recipe.AddIngredient(ItemID.JungleSpores, 6);
            recipe.AddIngredient(ItemID.Vine, 12);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}