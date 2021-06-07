using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class MarbleBook : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Apollo Pythios");
            Tooltip.SetDefault("Summons three magical arrows that launch toward nearby enemies\n'No aiming required'");
        }

        public override void SetDefaults()
        {
            item.autoReuse = true;
            item.rare = ItemRarityID.Green;
            item.mana = 11;
            item.UseSound = SoundID.Item9;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 18;
            item.useTurn = false;
            item.useAnimation = 28;
            item.useTime = 28;
            item.width = 28;
            item.height = 30;
            item.shoot = ModContent.ProjectileType<MarbleArrow>();
            item.shootSpeed = 20f;
            item.knockBack = 3f;
            item.magic = true;
            item.value = Item.sellPrice(gold: 1);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for(int i = 0; i < 3; i++)
            {
                Vector2 randPos = new Vector2(player.Center.X + Main.rand.Next(-7, 7) * 10, player.Center.Y + Main.rand.Next(-7, 7) * 10);
                Projectile.NewProjectile(randPos, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}