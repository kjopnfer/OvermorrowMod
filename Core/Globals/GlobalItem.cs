using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Items.Archives.Accessories;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Particles;
using System.Diagnostics.Metrics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;

namespace OvermorrowMod.Core.Globals
{
    public class GlobalItems : GlobalItem
    {
        public override bool InstancePerEntity => true;

        /// <summary>
        /// If set to true, prevents this item from being picked up.
        /// </summary>
        public bool PreventPickup = false;

        private float GlowCounter = 0;
        public bool DisplayMode = false;
        public override bool CanPickup(Item item, Player player)
        {
            return !PreventPickup;
        }

        public override bool OnPickup(Item item, Player player)
        {
            if (!PreventPickup)
            {
                item.active = false;
            }

            return !PreventPickup;
        }

        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (DisplayMode)
            {
                if (!Main.gamePaused)
                {
                    GlowCounter++;
                    if (GlowCounter % 20 == 0)
                    {
                        Texture2D lightTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "ray").Value;
                        Color color = Color.Green;

                        for (int i = 0; i < Main.rand.Next(2, 5); i++)
                        {
                            var lightRay = new Light(lightTexture, ModUtils.SecondsToTicks(4), item, Vector2.Zero);
                            float randomRotation = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                            float randomSize = Main.rand.NextFloat(0.05f, 0.085f);
                            ParticleManager.CreateParticleDirect(lightRay, item.Center, Vector2.Zero, color, 1f, randomSize, randomRotation, ParticleDrawLayer.BehindNPCs, useAdditiveBlending: true);
                        }
                    }
                }


                Texture2D texture = TextureAssets.Item[item.type].Value;
                spriteBatch.Draw(texture, item.Center - Main.screenPosition, null, Color.White, 0f, texture.Size() / 2, item.scale, SpriteEffects.None, 0);

                return false;
            }

            return base.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (item.ModItem is IBowModifier bowModifier)
            {
                player.GetModPlayer<BowPlayer>().AddBowModifier(bowModifier);
            }

            if (item.ModItem is IBowDrawEffects drawEffects)
            {
                player.GetModPlayer<BowPlayer>().AddBowDrawEffect(drawEffects);
            }

            if (item.ModItem is IGunModifier gunModifier)
            {
                player.GetModPlayer<GunPlayer>().AddGunModifier(gunModifier);
            }
        }
    }
}