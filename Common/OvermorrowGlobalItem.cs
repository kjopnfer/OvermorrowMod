using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.Items.Consumable;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public class OvermorrowGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public int warningDelay = 0;
        public float reforgeAnimation = 0;

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ModContent.ItemType<ReforgeStone>())
            {
                for (int lines = 0; lines < tooltips.Count; lines++)
                {
                    if (tooltips[lines].Name == "Damage") tooltips.RemoveAt(lines);
                    if (tooltips[lines].Name == "CritChance") tooltips.RemoveAt(lines);
                    if (tooltips[lines].Name == "Speed") tooltips.RemoveAt(lines);
                    if (tooltips[lines].Name == "Knockback") tooltips.RemoveAt(lines);
                }
            }

            base.ModifyTooltips(item, tooltips);
        }

        public override bool ConsumeItem(Item item, Player player)
        {
            if (Main.mouseItem.type == ModContent.ItemType<ReforgeStone>())
            {
                return false;
            }

            return base.ConsumeItem(item, player);
        }

        public override void RightClick(Item item, Player player)
        {
            if (Main.mouseItem.type == ModContent.ItemType<ReforgeStone>())
            {
                Main.NewText("AHHHHHHH");
            }

            base.RightClick(item, player);
        }

        public override bool CanRightClick(Item item)
        {
            if (Main.mouseItem.type == ModContent.ItemType<ReforgeStone>())
            {
                // Check if the prefix is even valid for the item
                if (item.DamageType == DamageClass.Melee)
                {
                    // The item has a knockback prefix but it doesn't even have knockback
                    if (item.knockBack == 0 && Array.IndexOf(ReforgeStone.knockbackPrefixes, Main.mouseItem.prefix) > -1)
                    {
                        if (item.GetGlobalItem<OvermorrowGlobalItem>().warningDelay == 0)
                        {
                            Main.NewText("This item has no knockback to modify.", new Color(252, 86, 3));
                            item.GetGlobalItem<OvermorrowGlobalItem>().warningDelay = 120;
                        }
                    }
                    /*else if (item.crit == 0 && Array.IndexOf(ReforgeStone.critPrefixes, Main.mouseItem.prefix) > -1)  // The item has a crit prefix but it doesn't even have critical strike chance
                    {
                        if (item.GetGlobalItem<OvermorrowGlobalItem>().warningDelay == 0)
                        {
                            Main.NewText("This item has no critical strike chance to modify.", new Color(252, 86, 3));
                            item.GetGlobalItem<OvermorrowGlobalItem>().warningDelay = 120;
                        }
                    }*/
                    else
                    {
                        item.SetDefaults(item.type); // Reset the item to prevent stacking
                        item.Prefix(0);
                        item.Prefix(Main.mouseItem.prefix);
                        item.GetGlobalItem<OvermorrowGlobalItem>().reforgeAnimation = 30;

                        Main.mouseItem.TurnToAir();
                    }
                }
            }

            return base.CanRightClick(item);
        }

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (reforgeAnimation > 0)
            {
                Main.spriteBatch.Reload(SpriteSortMode.Immediate);

                Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
                float progress = Utils.Clamp(item.GetGlobalItem<OvermorrowGlobalItem>().reforgeAnimation, 0, 120f) / 120f;
                effect.Parameters["WhiteoutColor"].SetValue(Color.Orange.ToVector3());
                effect.Parameters["WhiteoutProgress"].SetValue(progress);
                effect.CurrentTechnique.Passes["Whiteout"].Apply();

                spriteBatch.Draw(TextureAssets.Item[item.type].Value, position, frame, drawColor, 0, origin, scale, SpriteEffects.None, 1);

                Main.spriteBatch.Reload(SpriteSortMode.Deferred);

                item.GetGlobalItem<OvermorrowGlobalItem>().reforgeAnimation -= 0.5f;

                return false;
            }

            return base.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }

        public override void UpdateInventory(Item item, Player player)
        {
            if (item.GetGlobalItem<OvermorrowGlobalItem>().warningDelay > 0)
            {
                item.GetGlobalItem<OvermorrowGlobalItem>().warningDelay--;
            }

            base.UpdateInventory(item, player);
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (player.GetModPlayer<OvermorrowModPlayer>().SerpentTooth)
            {
                damage.Flat += 5;
            }

            if (player.GetModPlayer<OvermorrowModPlayer>().PredatorTalisman)
            {
                damage.Flat += 3;
            }

            if (player.GetModPlayer<OvermorrowModPlayer>().EruditeDamage)
            {
                damage.Flat += 2;
            }

            if (player.GetModPlayer<OvermorrowModPlayer>().moonBuff)
            {
                damage.Flat += 4;
            }
        }

        public override void AddRecipes()
        {
            Recipe.Create(ItemID.MagicMirror)
                .AddIngredient(ItemID.RecallPotion, 7)
                .Register();
        }
    }
}