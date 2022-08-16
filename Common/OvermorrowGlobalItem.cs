using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
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

        public override bool ConsumeItem(Item item, Player player)
        {
            if (Main.mouseItem.type == ModContent.ItemType<ReforgeStone>())
            {
                return false;
            }

            return base.ConsumeItem(item, player);
        }

        public override bool CanRightClick(Item item)
        {
            if (Main.mouseItem.ModItem is ReforgeStone)
            {
                OvermorrowGlobalItem globalItem = item.GetGlobalItem<OvermorrowGlobalItem>();
                bool canApply = true;

                if (Main.mouseItem.ModItem is MeleeReforge meleeStone)
                {
                    // Check if the prefix is even valid for the item
                    if (item.DamageType == DamageClass.Melee)
                    {
                        // The item has a knockback prefix but it doesn't even have knockback
                        if (item.knockBack == 0 && Array.IndexOf(ReforgeStone.knockbackPrefixesMelee, Main.mouseItem.prefix) > -1)
                        {
                            if (globalItem.warningDelay == 0)
                            {
                                Main.NewText("This item has no knockback to modify.", new Color(252, 86, 3));
                                canApply = false;
                                globalItem.warningDelay = 120;
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
                        else if (meleeStone.DPSCeiling >= DPSCalculation(item) && meleeStone.DPSFloor < DPSCalculation(item))
                        {
                            if (globalItem.warningDelay == 0)
                            {
                                Main.NewText("This item requires a higher tier of Reforge Stones.", new Color(252, 86, 3));
                                canApply = false;
                                globalItem.warningDelay = 120;
                            }
                        }
                    }
                }
                else if (Main.mouseItem.ModItem is RangedReforge rangedStone)
                {
                    // Check if the prefix is even valid for the item
                    if (item.DamageType == DamageClass.Magic)
                    {
                        // The item has a knockback prefix but it doesn't even have knockback
                        if (item.knockBack == 0 && Array.IndexOf(ReforgeStone.rangedPrefixes, Main.mouseItem.prefix) > -1)
                        {
                            if (globalItem.warningDelay == 0)
                            {
                                Main.NewText("This item has no knockback to modify.", new Color(252, 86, 3));
                                canApply = false;
                                globalItem.warningDelay = 120;
                            }
                        }
                        else if (rangedStone.DPSCeiling >= DPSCalculation(item) && rangedStone.DPSFloor < DPSCalculation(item))
                        {
                            if (globalItem.warningDelay == 0)
                            {
                                Main.NewText("This item requires a higher tier of Reforge Stones.", new Color(252, 86, 3));
                                canApply = false;
                                globalItem.warningDelay = 120;
                            }
                        }
                    }
                }

                if (item.DamageType != Main.mouseItem.DamageType)
                {
                    canApply = false;

                    if (globalItem.warningDelay == 0)
                    {
                        Main.NewText("This item is unable to be used with this stone.", new Color(252, 86, 3));
                        globalItem.warningDelay = 120;
                    }
                }

                if (canApply)
                {
                    item.SetDefaults(item.type); // Reset the item to prevent stacking
                    item.Prefix(0);
                    item.Prefix(Main.mouseItem.prefix);
                    globalItem.reforgeAnimation = 40;

                    Main.mouseItem.TurnToAir();

                    Main.NewText(DPSCalculation(item));
                }
            }

            return base.CanRightClick(item);
        }

        TParticleSystem sys = new();
        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (reforgeAnimation > 0)
            {
                // Spawn the particles once
                if (reforgeAnimation == 40)
                {
                    int maxIterations = 6;
                    for (int i = 0; i < maxIterations; i++)
                    {
                        sys.CreateParticle(Main.rand.NextVector2FromRectangle(frame), Vector2.One.RotatedBy(MathHelper.TwoPi / maxIterations * i) * 0.5f, Main.hslToRgb(Main.rand.NextFloat(0, 1), 1f, 0.9f), (part) =>
                        {
                            if (part.ai[0] >= 40)
                            {
                                part.dead = true;
                            }
                            else
                            {
                                part.ai[0]++;
                                part.position += part.velocity;
                                //part.position += part.velocity.RotatedBy(Math.PI / 2) * (float)Math.Sin(part.ai[0] * Math.PI / 10);
                                part.alpha = (float)(Math.Sin((1f - part.ai[0] / 40) * Math.PI));
                                part.scale = (1f - part.ai[0] / 40) * part.ai[1];
                            }
                        }, ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.StardustTowerMark).Value, (part, batch) =>
                        {
                            batch.Draw(part.texture, position + part.position, null, part.color * part.alpha, 0f,
                                part.texture.Size() / 2, part.scale * 0.2f, SpriteEffects.None, 0f);
                            batch.Reload(BlendState.Additive);
                            Texture2D tex = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Spotlight").Value;
                            batch.Draw(tex, position + part.position, null, Color.Orange * part.alpha * 0.7f, 0f,
                                tex.Size() / 2, part.scale * 2.5f, SpriteEffects.None, 0f);
                            batch.Draw(tex, position + part.position, null, part.color * part.alpha * 0.4f, 0f,
                                tex.Size() / 2, part.scale * 5f, SpriteEffects.None, 0f);
                            batch.Reload(BlendState.AlphaBlend);
                        }, (part) =>
                        {
                            part.alpha = 1f;
                            part.scale = 1f;
                            part.ai[1] = Main.rand.NextFloat(0.2f, 0.3f);
                        });
                    }
                }

                sys.UpdateParticles();
                sys.DrawParticles();

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

        private float DPSCalculation(Item item)
        {
            return (float)(item.damage * 60f / item.useTime) * (1 + item.crit / 100f);
        }

        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Main.mouseItem.ModItem is ReforgeStone)
            {
                if (item.DamageType != Main.mouseItem.DamageType || item.damage == 0)
                {
                    Main.spriteBatch.Reload(SpriteSortMode.Immediate);

                    Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
                    float progress = 30f / 120f;
                    effect.Parameters["WhiteoutColor"].SetValue(Color.Black.ToVector3());
                    effect.Parameters["WhiteoutProgress"].SetValue(1 - progress);
                    effect.CurrentTechnique.Passes["Whiteout"].Apply();

                    spriteBatch.Draw(TextureAssets.Item[item.type].Value, position, frame, drawColor, 0, origin, scale, SpriteEffects.None, 1);

                    Main.spriteBatch.Reload(SpriteSortMode.Deferred);
                }
            }

            base.PostDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
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