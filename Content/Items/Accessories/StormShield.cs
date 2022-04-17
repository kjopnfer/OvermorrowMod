using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Items.Accessories.Expert;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class StormShield : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blitz Bulwark");
            Tooltip.SetDefault("Leaves a trail of sparks behind you\nAllows the player to quickly dash into the enemy\n" +
                "Dashing shoots out sparks from the player");
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.damage = 36;
            Item.knockBack = 9f;
            Item.shieldSlot = 5;
            Item.width = 38;
            Item.height = 48;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
            Item.defense = 3;
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
            CreateRecipe()
                .AddIngredient<StormScale>()
                .AddIngredient(ItemID.EoCShield)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}