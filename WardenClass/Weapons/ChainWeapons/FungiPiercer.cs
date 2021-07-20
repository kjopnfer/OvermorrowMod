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
    public class FungiPiercer : PiercingItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fungal Vine");
            Tooltip.SetDefault("Attacks have a chance to inflict Fungal Infection\n[c/00FF00:{ Imbuement }]\n" +
                "[c/800080:Right Click] to cause attacks to spawns spores at your cursor\nConsumes 2 Soul Essences" +
                "\n'Not toxic, but can still kill you'");
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
            item.shoot = mod.ProjectileType("FungiPiercerProjectile");
            item.rare = ItemRarityID.Green;
            item.UseSound = new LegacySoundStyle(SoundID.Grass, 0); // Grass
            item.noUseGraphic = true;

            soulGainChance = 5;
        }

        public override bool CanUseItem(Player player)
        {
            // Get the class info from the player
            var modPlayer = WardenDamagePlayer.ModPlayer(player);

            if (player.altFunctionUse == 2 && modPlayer.soulResourceCurrent >= 2 && player.GetModPlayer<WardenRunePlayer>().RuneID == WardenRunePlayer.Runes.None)
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
                player.AddBuff(ModContent.BuffType<FungiRune>(), 600);
            }
            else
            {
                item.useStyle = ItemUseStyleID.SwingThrow;
                item.useTurn = true;
                item.useAnimation = 14;
                item.useTime = 14;
                item.damage = 5;
                item.shootSpeed = 14f + modPlayer.modifyShootSpeed();
                item.shoot = mod.ProjectileType("FungiPiercerProjectile");
                item.UseSound = new LegacySoundStyle(SoundID.Grass, 0); // Grass
            }

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.GetModPlayer<WardenRunePlayer>().RuneID == WardenRunePlayer.Runes.MushroomRune && !player.GetModPlayer<WardenRunePlayer>().runeDeactivate)
            {
                for (int i = 0; i < Main.rand.Next(3, 5); i++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(Main.MouseWorld.X, Main.MouseWorld.Y, Main.rand.Next(-3, 3), Main.rand.Next(-5, -3), ModContent.ProjectileType<FungiSpore>(), damage, 3f, player.whoAmI);
                    }
                }
                return false;
            }   
            return true;
        }
    }
}