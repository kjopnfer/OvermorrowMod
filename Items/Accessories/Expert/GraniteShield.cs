using OvermorrowMod.Buffs.Summon;
using OvermorrowMod.Projectiles.Accessory;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Accessories.Expert
{
    public class GraniteShield : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Shield");
            Tooltip.SetDefault("The Shield reflects projectiles");
        }
        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 34;
            item.value = 10000;
            item.rare = ItemRarityID.Expert;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddBuff(ModContent.BuffType<SpiderWebBuff>(), 2);
            if ((int)player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Accessory.GraniteShield>()] >= 1)
            {
                return;
            }
            Projectile.NewProjectile(player.position.X, player.position.Y, 0f, 0f, ModContent.ProjectileType<Projectiles.Accessory.GraniteShield>(), 20, 1.25f, player.whoAmI, 0f, 0f);
        }
    }
}