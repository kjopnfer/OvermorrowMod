using Microsoft.Xna.Framework;
using OvermorrowMod.Buffs.RuneBuffs;
using OvermorrowMod.Items.Weapons.PreHardmode.Magic.Worm;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Weapons.ChainWeapons
{
    public class VilePiercer : PiercingItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vile Guillotine");
            Tooltip.SetDefault("[c/00FF00:{ Imbuement }]\n" +
                "[c/800080:Right Click] to cause attacks to launch a worm to devour your enemies\nConsumes 2 Soul Essences");
        }

        public override void SafeSetDefaults()
        {
            item.autoReuse = false;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 14;
            item.useTime = 14;
            item.knockBack = 0f;
            item.width = 42;
            item.height = 46;
            item.damage = 3;
            item.shootSpeed = 14f;
            item.shoot = mod.ProjectileType("VilePiercerProjectile");
            item.rare = ItemRarityID.Blue;
            item.UseSound = SoundID.Item71;
            item.noUseGraphic = true;

            soulGainChance = 2;
        }

        public override bool CanUseItem(Player player)
        {
            // Get the class info from the player
            var modPlayer = WardenDamagePlayer.ModPlayer(player);

            if(player.altFunctionUse == 2 && modPlayer.soulResourceCurrent >= 2 && player.GetModPlayer<WardenRunePlayer>().RuneID == 0)
            {
                item.useStyle = ItemUseStyleID.HoldingUp;
                item.useAnimation = 45;
                item.useTime = 45;
                item.knockBack = 0f;
                item.damage = 3;
                item.shootSpeed = 0f;
                item.shoot = ProjectileID.None;
                item.UseSound = SoundID.DD2_WitherBeastAuraPulse;

                ConsumeSouls(2, player);
                player.GetModPlayer<WardenRunePlayer>().ActiveRune = true;
                player.AddBuff(ModContent.BuffType<VileRune>(), 600);
            }
            else
            {
                item.useStyle = ItemUseStyleID.HoldingOut;
                item.useTurn = true;
                if (player.GetModPlayer<WardenRunePlayer>().RuneID == 6 && !player.GetModPlayer<WardenRunePlayer>().runeDeactivate)
                {
                    item.useAnimation = 28;
                    item.useTime = 28;
                    item.UseSound = mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Items/Hork");
                    item.damage = 12;
                }
                else
                {
                    item.useAnimation = 14;
                    item.useTime = 14;
                    item.UseSound = SoundID.Item71;
                    item.damage = 3;
                }
                item.shootSpeed = 14f + modPlayer.modifyShootSpeed();
                item.shoot = mod.ProjectileType("VilePiercerProjectile");
            }

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.GetModPlayer<WardenRunePlayer>().RuneID == 6 && !player.GetModPlayer<WardenRunePlayer>().runeDeactivate)
            {
                /*float numberProjectiles = 3; // This defines how many projectiles to shot
                float rotation = MathHelper.ToRadians(15);
                position += Vector2.Normalize(new Vector2(speedX, speedY)) * 45f; //this defines the distance of the projectiles form the player when the projectile spawns
                for (int i = 0; i < numberProjectiles; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .8f; // This defines the projectile roatation and speed. .8f == 80% of projectile speed
                    Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockBack, player.whoAmI);
                }
                return false;*/
                damage = 12;
                type = ModContent.ProjectileType<WormHead>();
            }

            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ChainKnife);
            recipe.AddIngredient(ItemID.DemoniteBar, 12);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}