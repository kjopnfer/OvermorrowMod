using OvermorrowMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.PreHardmode.Magic
{
    public class IorichStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iorich's Sorrow");
            Tooltip.SetDefault("Shoots a bolt infused with the energies of nature\n" +
                "'The veil of darkness did little to hide the inferno that devoured his people'");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.autoReuse = true;
            item.rare = ItemRarityID.Orange;
            item.mana = 12;
            item.UseSound = SoundID.Item8;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 21;
            item.useTurn = false;
            item.useAnimation = 32;
            item.useTime = 32;
            item.width = 56;
            item.height = 60;
            item.shoot = ModContent.ProjectileType<NatureBolt>();
            item.shootSpeed = 9f;
            item.knockBack = 3f;
            item.magic = true;
            item.value = Item.sellPrice(gold: 1, silver: 75);
        }
    }
}