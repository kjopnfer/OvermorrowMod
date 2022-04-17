using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.GraniteBook
{
    public class GraniteBook : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Lance");
            Tooltip.SetDefault("Launches a Granite Spike that ignore 5 defense");
        }

        public override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Green;
            Item.mana = 11;
            Item.UseSound = SoundID.Item20;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 22;
            Item.useTurn = false;
            Item.useAnimation = 28;
            Item.useTime = 28;
            Item.width = 30;
            Item.height = 36;
            Item.shoot = ModContent.ProjectileType<GraniteSpike>();
            Item.shootSpeed = 12f;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Magic;
            Item.value = Item.sellPrice(gold: 1);
        }
    }
}