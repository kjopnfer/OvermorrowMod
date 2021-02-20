using Microsoft.Xna.Framework;
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
            DisplayName.SetDefault("Mycelium Chains");
            Tooltip.SetDefault("[c/00FF00:{ Special Ability }]\n" +
                "[c/800080:Right Click] to launch a fungi bulb that spawns spores\nConsumes 1 Soul Essence");
        }

        public override void SafeSetDefaults()
        {
            item.autoReuse = false;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 14;
            item.useTime = 14;
            item.knockBack = 0f;
            item.width = 30;
            item.height = 10;
            item.damage = 12;
            item.shootSpeed = 14f;
            item.shoot = mod.ProjectileType("FungiPiercerProjectile");
            item.rare = ItemRarityID.Green;
            item.UseSound = new LegacySoundStyle(SoundID.Grass, 0); // Grass
            item.noUseGraphic = true;
        }

        public override bool CanUseItem(Player player)
        {
            // Get the class info from the player
            var modPlayer = WardenDamagePlayer.ModPlayer(player);

            if(player.altFunctionUse == 2 && modPlayer.soulResourceCurrent > 0)
            {
                item.useStyle = ItemUseStyleID.SwingThrow;
                item.useAnimation = 14;
                item.useTime = 14;
                item.knockBack = 0f;
                item.damage = 12;
                item.shootSpeed = 28f;
                item.shoot = mod.ProjectileType("FungiPiercerProjectileAlt");

                ConsumeSouls(1, player);
            }
            else
            {
                item.useStyle = ItemUseStyleID.SwingThrow;
                item.useTurn = true;
                item.useAnimation = 14;
                item.useTime = 14;
                item.damage = 12;
                item.shootSpeed = 14f + modPlayer.modifyShootSpeed();
                item.shoot = mod.ProjectileType("FungiPiercerProjectile");
            }

            return base.CanUseItem(player);
        }
    }
}