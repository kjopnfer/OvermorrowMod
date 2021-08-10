using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace OvermorrowMod.Items.Accessories 
{
    [AutoloadEquip(EquipType.Shield)]
    public class EnchantedAegis : ModItem {
        public override string Texture => "OvermorrowMod/Items/Accessories/StormShield"; //placeholder

        public override void SetStaticDefaults() {
            Tooltip.SetDefault(
                "placeholder"
                + "\nStores all Mana you consume in Damage"
            );
        }

        public override void SetDefaults() {
            item.width = 24;
            item.height = 28;
            item.value = 12500;
            item.rare = ItemRarityID.Green;
            item.accessory = true;
            item.defense = 3;
        }
        
        // how much damage is stored each time
        public float deltaDamage = 3;
        public float damageCap = 175;
        public bool reachedCap = false;

        public int dustCounter = 0;

        public override void UpdateAccessory(Player player, bool hideVisual) {
            // Creating beams of light
            if(player.HeldItem.magic && player.itemAnimation > 0 && player.itemAnimation % 7 == 0) {
                Projectile.NewProjectile(player.Center, new Vector2(0), ModContent.ProjectileType<OvermorrowMod.Projectiles.Accessory.StoredDamage>(), 0, 0f, player.whoAmI, Main.rand.Next(70, 95), 0f);

                if (player.GetModPlayer<OvermorrowModPlayer>().storedDamage < damageCap) {
                    player.GetModPlayer<OvermorrowModPlayer>().storedDamage += deltaDamage;

                    reachedCap = false;
                } 
                else if (!reachedCap) {
                    Color color = new Color(146, 227, 220);
                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y + 50, player.width, player.height), color, "Stored Damage Limit Reached", true, false);

                    reachedCap = true;
                }
            }

            // Spawning dust
            if (player.GetModPlayer<OvermorrowModPlayer>().storedDamage > 0 && dustCounter % 3 == 0) {
                Dust dust = Main.dust[Terraria.Dust.NewDust(player.Center + new Vector2(0, -100), 15, 15, 226, 0f, 0f, 0, default, 1.25f)];
                dust.noGravity = true;
            }

            if (dustCounter >= 3) 
                dustCounter = 0;
            
            dustCounter++;
        }
    }
}