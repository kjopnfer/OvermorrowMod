using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Magic;
using OvermorrowMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class MarbleBook : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arcum Arcanus");
        }

        public override void SetDefaults()
        {
            item.autoReuse = true;
            item.rare = ItemRarityID.Green;
            item.mana = 11;
            item.UseSound = SoundID.Item20;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 18;
            item.useTurn = false;
            item.useAnimation = 28;
            item.useTime = 28;
            item.width = 28;
            item.height = 30;
            item.shoot = ModContent.ProjectileType<GraniteSpike>();
            item.shootSpeed = 12f;
            item.knockBack = 3f;
            item.magic = true;
            item.value = Item.sellPrice(gold: 1);
        }
    }
}