using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Materials
{
    public class FakeGem : ModItem
    {
        int glowCounter;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Ore");
            Tooltip.SetDefault("Used to craft cursed items\n'Touching it grants visions of various cursed images from the Internet'");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.rare = ItemRarityID.Red;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine line in tooltips)
            {
                if (line.Name == "ItemName")
                {
                    string text = line.Text;
                    string text2 = "";
                    for (int ind = 0; ind < text.Length; ind++)
                    {
                        char ch = text[ind];
                        if (Main.rand.NextBool(60))
                        {
                            ch = (char)Main.rand.Next(35, 38);
                        }
                        text2 += ch;
                    }
                    line.Text = text2;
                }
            }
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            if (line.Name == "ItemName")
            {
                spriteBatch.Reload(SpriteSortMode.Immediate);
                Effect fx = OvermorrowModFile.Instance.TextShader;
                Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("Assets/Textures/Perlin").Value;
                fx.Parameters["uColor0"].SetValue(Color.DarkRed.ToVector3());
                //fx.Parameters["uColor1"].SetValue(Color.Black.ToVector3());
                //fx.Parameters["uColor0"].SetValue(new Color(140, 48, 85).ToVector3());
                //fx.Parameters["uColor1"].SetValue(new Color(176, 48, 62).ToVector3());
                fx.Parameters["uColor1"].SetValue(Color.Purple.ToVector3());
                fx.SafeSetParameter("uTime", Main.GlobalTimeWrappedHourly);
                //fx.CurrentTechnique.Passes["Noise"].Apply();
                fx.CurrentTechnique.Passes[2].Apply();

                /*if (Main.rand.NextBool(30))
                {
                    Utils.DrawBorderString(spriteBatch, line.text, new Vector2(line.X, line.Y), Color.Lerp(Color.Red, Color.Transparent, 0.95f), 1f, Main.rand.NextFloat(-0.3f, 0.3f), Main.rand.NextFloat(-0.2f, 0.2f));
                }*/
                Utils.DrawBorderString(spriteBatch, line.text, new Vector2(line.X, line.Y), Color.Lerp(Color.Orange, Color.Transparent, 0.95f), 1f, Main.rand.NextBool(30) ? Main.rand.NextFloat(-0.5f, 0.5f) : 0, Main.rand.NextBool(30) ? Main.rand.NextFloat(-0.5f, 0.5f) : 0);
                Utils.DrawBorderString(spriteBatch, line.text, new Vector2(line.X, line.Y), Color.Lerp(Color.Orange, Color.Transparent, 0.95f), 1f, Main.rand.NextBool(30) ? Main.rand.NextFloat(-0.5f, 0.5f) : 0, Main.rand.NextBool(30) ? Main.rand.NextFloat(-0.5f, 0.5f) : 0);
                Utils.DrawBorderString(spriteBatch, line.text, new Vector2(line.X, line.Y), Color.White, 1f, Main.rand.NextBool(60) ? Main.rand.NextFloat(-0.45f, 0.45f) : 0);
                spriteBatch.Reload(SpriteSortMode.Deferred);
                return false;
            }
            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            float glowScale = MathHelper.Lerp(1, 2, (float)Math.Sin(glowCounter / 20f));
            Color glowColor = Color.Lerp(new Color(94, 3, 3), new Color(255, 38, 38), (float)Math.Sin(glowCounter / 10f));

            glowCounter++;

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Glow").Value;
            Rectangle rect = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, new Rectangle?(rect), glowColor, rotation, drawOrigin, glowScale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


            return base.PreDrawInWorld(spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, 1f, 0f, 0f);
        }
    }
}