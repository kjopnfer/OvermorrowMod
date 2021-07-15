using Microsoft.Xna.Framework;
using OvermorrowMod.Buffs.RuneBuffs;
using OvermorrowMod.Projectiles.Piercing.HellFire;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Weapons.ChainWeapons
{
    public class BlazePiercer : PiercingItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infernal Chains");
            Tooltip.SetDefault("Attacks have a chance to set enemies on fire\n[c/00FF00:{ Imbuement }]\n" +
                            "[c/800080:Right Click] to cause attacks to summon hellfire to consume your enemies\nConsumes 3 Soul Essences");
        }

        public override void SafeSetDefaults()
        {
            item.autoReuse = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 14;
            item.useTime = 14;
            item.knockBack = 0f;
            item.width = 36;
            item.height = 48;
            item.damage = 9;
            item.shootSpeed = 18f;
            item.shoot = mod.ProjectileType("BlazePiercerProjectile");
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item71;
            item.noUseGraphic = true;

            soulGainChance = 3;
        }

        public override bool CanUseItem(Player player)
        {
            // Get the class info from the player
            var modPlayer = WardenDamagePlayer.ModPlayer(player);

            if (player.altFunctionUse == 2 && modPlayer.soulResourceCurrent >= 3 && player.GetModPlayer<WardenRunePlayer>().RuneID == WardenRunePlayer.Runes.None)
            {
                item.useStyle = ItemUseStyleID.HoldingUp;
                item.useAnimation = 45;
                item.useTime = 45;
                item.damage = 12;
                item.shootSpeed = 0f;
                item.shoot = ProjectileID.None;
                item.UseSound = SoundID.DD2_WitherBeastAuraPulse;

                ConsumeSouls(3, player);
                player.GetModPlayer<WardenRunePlayer>().ActiveRune = true;
                player.AddBuff(ModContent.BuffType<BlazeRune>(), 600);
            }
            else
            {
                item.autoReuse = true;
                item.useStyle = ItemUseStyleID.HoldingOut;
                item.useTurn = true;
                if (player.GetModPlayer<WardenRunePlayer>().RuneID == WardenRunePlayer.Runes.HellRune && !player.GetModPlayer<WardenRunePlayer>().runeDeactivate)
                {
                    item.damage = 17;
                    item.useAnimation = 35;
                    item.useTime = 35;
                    item.UseSound = SoundID.Item45;
                }
                else
                {
                    item.damage = 9;
                    item.useAnimation = 14;
                    item.useTime = 14;
                    item.UseSound = SoundID.Item71;
                }
                item.shootSpeed = 18f + modPlayer.modifyShootSpeed();
                item.shoot = mod.ProjectileType("BlazePiercerProjectile");
            }

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.GetModPlayer<WardenRunePlayer>().RuneID == WardenRunePlayer.Runes.HellRune && !player.GetModPlayer<WardenRunePlayer>().runeDeactivate)
            {
                for (int i = 0; i < 200; i++)
                {

                    NPC npc = Main.npc[i];
                    if (Vector2.Distance(Main.MouseWorld, npc.Center) < 65)
                    {
                        if (npc.friendly == false && npc.life > 0)
                        {
                            Projectile.NewProjectile(npc.Center.X, npc.Center.Y - 165, 0, 0, ModContent.ProjectileType<HellFireDown>(), item.damage, 3, player.whoAmI);
                            Main.PlaySound(SoundID.Item45, Main.MouseWorld);
                        }
                    }
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ChainKnife);
            recipe.AddIngredient(ItemID.HellstoneBar, 14);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}