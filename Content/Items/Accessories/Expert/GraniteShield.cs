using OvermorrowMod.Content.Buffs.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories.Expert
{
    public class GraniteShield : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Shield");
            Tooltip.SetDefault("Conjures a Granite Shield to protect you\nThe shield reflects projectiles below a damage threshold\n" +
                "The shield breaks after taking enough damage\nDamage capacity upgrades throughout the game");
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
            player.AddBuff(ModContent.BuffType<GraniteShieldBuff>(), 2);
            if ((int)player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Accessory.GraniteShield>()] >= 1)
            {
                return;
            }
            Projectile.NewProjectile(player.position.X, player.position.Y, 0f, 0f, ModContent.ProjectileType<Projectiles.Accessory.GraniteShield>(), 20, 1.25f, player.whoAmI, 0f, 0f);
        }
    }
}