using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.Items.Consumable;
using OvermorrowMod.Core;
using System;
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
        public int resetDelay = 0;

        private bool drawInvalid = false;
        private TParticleSystem sys = new();
        private Color reforgeColor = Color.White;

        private float DPSCalculation(Item item)
        {
            return (float)(item.damage * 60f / item.useTime) * (1 + item.crit / 100f);
        }

        public override bool OnPickup(Item item, Player player)
        {
            DialoguePlayer dialoguePlayer = player.GetModPlayer<DialoguePlayer>();

            if (item.type == ItemID.Wood && !dialoguePlayer.pickupWood)
            {
                dialoguePlayer.pickupWood = true;

                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "Portraits/Guide/GuideSmug", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

                dialoguePlayer.AddPopup(texture, "Gathering [wood] I see, good thinking. Wood's useful for all sorts of things.", 90, 120, new Color(52, 201, 235), true, false);
                dialoguePlayer.AddPopup(texture, "If you need ideas just bring me some and I'll show you what you can make with it.", 90, 120, new Color(52, 201, 235), false, true);
            }

            return base.OnPickup(item, player);
        }

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

                if (Main.mouseItem.ModItem is MeleeReforge meleeStone && item.DamageType == DamageClass.Melee)
                {
                    //Main.NewText(meleeStone.DPSFloor + "/ " + meleeStone.DPSCeiling + ": " + DPSCalculation(item));

                    // The item has a knockback prefix but it doesn't even have knockback
                    if (item.knockBack == 0 && Array.IndexOf(ReforgeStone.knockbackPrefixesMelee, Main.mouseItem.prefix) > -1)
                    {
                        canApply = false;

                        if (globalItem.warningDelay == 0)
                        {
                            Main.NewText("This item has no knockback to modify.", new Color(252, 86, 3));
                            globalItem.warningDelay = 120;
                        }
                    }
                    else if (meleeStone.DPSCeiling < DPSCalculation(item) || meleeStone.DPSFloor > DPSCalculation(item))
                    {
                        canApply = false;

                        if (globalItem.warningDelay == 0)
                        {
                            Main.NewText("This item requires a higher tier of Reforge Stones.", new Color(252, 86, 3));
                            globalItem.warningDelay = 120;
                        }
                    }

                }
                else if (Main.mouseItem.ModItem is RangedReforge rangedStone && item.DamageType == DamageClass.Ranged)
                {
                    //Main.NewText(rangedStone.DPSFloor + "/ " + rangedStone.DPSCeiling + ": " + DPSCalculation(item));

                    // The item has a knockback prefix but it doesn't even have knockback
                    if (item.knockBack == 0 && Array.IndexOf(ReforgeStone.rangedPrefixes, Main.mouseItem.prefix) > -1)
                    {
                        canApply = false;

                        if (globalItem.warningDelay == 0)
                        {
                            Main.NewText("This item has no knockback to modify.", new Color(252, 86, 3));
                            globalItem.warningDelay = 120;
                        }
                    }
                    else if (rangedStone.DPSCeiling < DPSCalculation(item) || rangedStone.DPSFloor > DPSCalculation(item))
                    {
                        canApply = false;

                        if (globalItem.warningDelay == 0)
                        {
                            Main.NewText("This item requires a higher tier of Reforge Stones.", new Color(252, 86, 3));
                            globalItem.warningDelay = 120;
                        }
                    }

                }
                else if (Main.mouseItem.ModItem is MagicReforge magicStone && item.DamageType == DamageClass.Magic)
                {
                    //Main.NewText(magicStone.DPSFloor + "/ " + magicStone.DPSCeiling + ": " + DPSCalculation(item));

                    // The item has a knockback prefix but it doesn't even have knockback
                    if (item.mana >= 0 && Array.IndexOf(ReforgeStone.rangedPrefixes, Main.mouseItem.prefix) > -1)
                    {
                        canApply = false;

                        if (globalItem.warningDelay == 0)
                        {
                            Main.NewText("This item has no knockback to modify.", new Color(252, 86, 3));
                            globalItem.warningDelay = 120;
                        }
                    }
                    else if (magicStone.DPSCeiling < DPSCalculation(item) || magicStone.DPSFloor > DPSCalculation(item))
                    {
                        canApply = false;

                        if (globalItem.warningDelay == 0)
                        {
                            Main.NewText("This item requires a higher tier of Reforge Stones.", new Color(252, 86, 3));
                            globalItem.warningDelay = 120;
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
                    item.CloneDefaults(item.type);
                    item.Prefix(0);
                    item.Prefix(Main.mouseItem.prefix);

                    globalItem.reforgeAnimation = 40;

                    if (Main.mouseItem.ModItem is MeleeReforge) reforgeColor = Color.Orange;
                    else if (Main.mouseItem.ModItem is RangedReforge) reforgeColor = Color.Green;
                    else if (Main.mouseItem.ModItem is MagicReforge) reforgeColor = Color.Red;

                    Main.mouseItem.TurnToAir();
                }
            }

            return base.CanRightClick(item);
        }

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (reforgeAnimation > 0)
            {
                if (reforgeAnimation == 40) // Spawn the particles once, it's hard-coded assuming no other reforgeAnimations take longer than 1.5 secs LOL
                {
                    const int MAX_ITERATIONS = 6;
                    for (int i = 0; i < MAX_ITERATIONS; i++)
                    {
                        sys.CreateParticle(Main.rand.NextVector2FromRectangle(frame), Vector2.One.RotatedBy(MathHelper.TwoPi / MAX_ITERATIONS * i) * 0.5f, Main.hslToRgb(Main.rand.NextFloat(0, 1), 1f, 0.9f), (part) =>
                        {
                            if (part.ai[0] >= 40)
                            {
                                part.dead = true;
                            }
                            else
                            {
                                part.ai[0]++;
                                part.position += part.velocity;
                                part.alpha = (float)(Math.Sin((1f - part.ai[0] / 40) * Math.PI));
                                part.scale = (1f - part.ai[0] / 40) * part.ai[1];
                            }
                        }, ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.StardustTowerMark).Value, (part, batch) =>
                        {
                            batch.Draw(part.texture, position + part.position, null, part.color * part.alpha, 0f,
                                part.texture.Size() / 2, part.scale * 0.2f, SpriteEffects.None, 0f);
                            batch.Reload(BlendState.Additive);
                            Texture2D tex = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Spotlight").Value;
                            batch.Draw(tex, position + part.position, null, reforgeColor * part.alpha * 0.7f, 0f,
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
                effect.Parameters["WhiteoutColor"].SetValue(reforgeColor.ToVector3());
                effect.Parameters["WhiteoutProgress"].SetValue(progress);
                effect.CurrentTechnique.Passes["Whiteout"].Apply();

                spriteBatch.Draw(TextureAssets.Item[item.type].Value, position, frame, drawColor, 0, origin, scale, SpriteEffects.None, 1);

                Main.spriteBatch.Reload(SpriteSortMode.Deferred);

                reforgeAnimation -= 0.5f;

                return false;
            }

            return base.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }

        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Main.mouseItem.ModItem is ReforgeStone && reforgeAnimation == 0) // In case the item goes out of range after reforging don't just black it out immediately
            {
                drawInvalid = false;

                if (item.DamageType != Main.mouseItem.DamageType || item.damage == 0)
                {
                    drawInvalid = true;
                }

                if (Main.mouseItem.ModItem is MeleeReforge meleeStone)
                {
                    if (meleeStone.DPSCeiling < DPSCalculation(item) || meleeStone.DPSFloor > DPSCalculation(item))
                    {
                        drawInvalid = true;
                    }
                }
                else if (Main.mouseItem.ModItem is RangedReforge rangedStone)
                {
                    if (rangedStone.DPSCeiling < DPSCalculation(item) || rangedStone.DPSFloor > DPSCalculation(item))
                    {
                        drawInvalid = true;
                    }
                }
                else if (Main.mouseItem.ModItem is MagicReforge magicStone)
                {
                    if (magicStone.DPSCeiling < DPSCalculation(item) || magicStone.DPSFloor > DPSCalculation(item))
                    {
                        drawInvalid = true;
                    }
                }

                if (drawInvalid)
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
            if (item.GetGlobalItem<OvermorrowGlobalItem>().warningDelay > 0) item.GetGlobalItem<OvermorrowGlobalItem>().warningDelay--;

            base.UpdateInventory(item, player);
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (player.GetModPlayer<OvermorrowModPlayer>().SerpentTooth) damage.Flat += 5;

            if (player.GetModPlayer<OvermorrowModPlayer>().PredatorTalisman) damage.Flat += 3;

            if (player.GetModPlayer<OvermorrowModPlayer>().EruditeDamage) damage.Flat += 2;
        }

        public override void AddRecipes()
        {
            Recipe.Create(ItemID.MagicMirror)
                .AddIngredient(ItemID.RecallPotion, 7)
                .Register();
        }
    }
}