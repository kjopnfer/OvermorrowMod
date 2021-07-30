using Microsoft.Xna.Framework;
using OvermorrowMod.Buffs.RuneBuffs;
using OvermorrowMod.Projectiles.Piercing;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Weapons.ChainWeapons
{
    public class CrimsonPiercer : PiercingItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tendon Guillotine");
            Tooltip.SetDefault("[c/09DBB8:{ Imbuement }]\n" +
                               "[c/800080:Right Click] to empower your Warden Artifacts on use\n" +
                               "[c/DE3A28:{ Power }] Your Artifact summons inflict Ichor\n" +
                               "[c/00FF00:{ Wisdom }] Spawns Bloody Thorns to damage enemies within range\n" +
                               "Consumes 1 Soul Essence\n" +
                               "'How does the Crimson work!?'");
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
            item.damage = 3;
            item.shootSpeed = 14f;
            item.shoot = mod.ProjectileType("CrimsonPiercerProjectile");
            item.rare = ItemRarityID.Blue;
            item.UseSound = SoundID.Item71;
            item.noUseGraphic = true;

            soulGainChance = 4;
        }

        public override bool CanUseItem(Player player)
        {
            // Get the class info from the player
            var modPlayer = WardenDamagePlayer.ModPlayer(player);

            if(player.altFunctionUse == 2 && modPlayer.soulResourceCurrent >= 1 && player.GetModPlayer<WardenRunePlayer>().RuneID == WardenRunePlayer.Runes.None)
            {
                item.useStyle = ItemUseStyleID.HoldingUp;
                item.useAnimation = 45;
                item.useTime = 45;
                item.damage = 3;
                item.shootSpeed = 0f;
                item.shoot = ProjectileID.None;
                item.UseSound = SoundID.DD2_WitherBeastAuraPulse;

                ConsumeSouls(1, player);
                player.GetModPlayer<WardenRunePlayer>().ActiveRune = true;
                player.AddBuff(ModContent.BuffType<SanguineRune>(), 300);
            }
            else
            {
                item.useStyle = ItemUseStyleID.SwingThrow;
                item.useTurn = true;
                
                if (player.GetModPlayer<WardenRunePlayer>().RuneID == WardenRunePlayer.Runes.CrimsonRune && !player.GetModPlayer<WardenRunePlayer>().runeDeactivate)
                {
                    item.useAnimation = 20;
                    item.useTime = 20;
                    item.UseSound = null;
                    item.shootSpeed = 6f;
                    item.damage = 15;
                }
                else
                {
                    item.useAnimation = 14;
                    item.useTime = 14;
                    item.UseSound = SoundID.Item71;
                    item.shootSpeed = 14f + modPlayer.modifyShootSpeed();                
                    item.damage = 4;
                }
                item.shoot = mod.ProjectileType("CrimsonPiercerProjectile");
            }

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.GetModPlayer<WardenRunePlayer>().RuneID == WardenRunePlayer.Runes.CrimsonRune && !player.GetModPlayer<WardenRunePlayer>().runeDeactivate)
            {
                Vector2 origin = Main.MouseWorld;
                float radius = 15;
                int numLocations = 30;
                for (int i = 0; i < 30; i++)
                {
                    Vector2 dustPosition = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                    Vector2 dustvelocity = new Vector2(0f, 10f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                    int dust = Dust.NewDust(dustPosition, 2, 2, 90, dustvelocity.X, dustvelocity.Y, 0, default, 1.25f);
                    Main.dust[dust].noGravity = true;
                }

                type = ModContent.ProjectileType<RedThornHead>();
                int randRotation = Main.rand.Next(24) * 15; // Uhhh, random degrees in increments of 15
                for (int i = 0; i < 6; i++)
                {
                    Projectile.NewProjectile(Main.MouseWorld, new Vector2(6).RotatedBy(MathHelper.ToRadians((360 / 6) * i + randRotation)), type, damage, knockBack, player.whoAmI);
                }
                return false;
            }

            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ChainKnife);
            recipe.AddIngredient(ItemID.CrimtaneBar, 12);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}