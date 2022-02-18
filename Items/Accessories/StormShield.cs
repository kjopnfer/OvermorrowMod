using Microsoft.Xna.Framework;
using OvermorrowMod.Items.Accessories.Expert;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class StormShield : ModItem
    {
        public override string Texture => "OvermorrowMod/Items/Accessories/StormShield";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blitz Bulwark");
            Tooltip.SetDefault("Leaves a trail of sparks behind you\nAllows the player to quickly dash into the enemy\n" +
                "Dashing shoots out sparks from the player");
        }

        public override void SetDefaults()
        {
            item.melee = true;
            item.damage = 36;
            item.knockBack = 9f;
            item.shieldSlot = 5;
            item.width = 38;
            item.height = 48;
            item.value = Item.buyPrice(gold: 1);
            item.rare = ItemRarityID.Orange;
            item.accessory = true;
            item.defense = 3;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            OvermorrowModPlayer modPlayer = player.GetModPlayer<OvermorrowModPlayer>();

            modPlayer.StormScale = true;
            modPlayer.StormShield = true;

            if (!modPlayer.DashActive)
            {
                return;
            }

            modPlayer.DashType = 1;

            //This is where we set the afterimage effect.  You can replace these two lines with whatever you want to happen during the dash
            //Some examples include:  spawning dust where the player is, adding buffs, making the player immune, etc.
            //Here we take advantage of "player.eocDash" and "player.armorEffectDrawShadowEOCShield" to get the Shield of Cthulhu's afterimage effect
            player.eocDash = modPlayer.DashTimer;
            player.armorEffectDrawShadowEOCShield = true;

            //If the dash has just started, apply the dash velocity in whatever direction we wanted to dash towards
            if (modPlayer.DashTimer == OvermorrowModPlayer.MAX_DASH_TIMER)
            {
                Vector2 newVelocity = player.velocity;

                //Only apply the dash velocity if our current speed in the wanted direction is less than DashVelocity
                if ((modPlayer.DashDir == OvermorrowModPlayer.DashLeft && player.velocity.X > -modPlayer.DashVelocity) || (modPlayer.DashDir == OvermorrowModPlayer.DashRight && player.velocity.X < modPlayer.DashVelocity))
                {
                    //X-velocity is set here
                    int dashDirection = modPlayer.DashDir == OvermorrowModPlayer.DashRight ? 1 : -1;
                    newVelocity.X = dashDirection * modPlayer.DashVelocity;
                }
                player.velocity = newVelocity;
            }

            //Decrement the timers
            modPlayer.DashTimer--;
            modPlayer.DashDelay--;

            if (modPlayer.DashDelay == 0)
            {
                //The dash has ended.  Reset the fields
                modPlayer.DashDelay = OvermorrowModPlayer.MAX_DASH_DELAY;
                modPlayer.DashTimer = OvermorrowModPlayer.MAX_DASH_TIMER;
                modPlayer.DashActive = false;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<StormScale>(), 1);
            recipe.AddIngredient(ItemID.EoCShield, 1);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}