using OvermorrowMod.Items.Materials;
using OvermorrowMod.Projectiles.Magic.Gems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class ExplosionTestStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosion test Staff");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Orange;
            item.mana = 7;
            item.UseSound = SoundID.Item43;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 14;
            item.useTurn = false;
            item.useAnimation = 22;
            item.useTime = 22;
            item.width = 48;
            item.height = 48;
            item.shoot = ModContent.ProjectileType<ExplosionTestProj>();
            item.shootSpeed = 9.5f;
            item.knockBack = 4.5f;
            item.magic = true;
        }
    }
}