using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Misc
{
    public class NineTails : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("The Cat-o-Nine Tails");
            /* Tooltip.SetDefault("Useable at the Dark Pantheon\n" +
                "Invokes the Curse of Caerea"); */
        }

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 40;
            Item.rare = ItemRarityID.Red;
            Item.maxStack = 1;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.consumable = false;
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
                Effect fx = OvermorrowModFile.Instance.TextShader.Value;
                Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Perlin").Value;
                fx.Parameters["uColor0"].SetValue(Color.DarkRed.ToVector3());
                //fx.Parameters["uColor1"].SetValue(Color.Black.ToVector3());
                //fx.Parameters["uColor0"].SetValue(new Color(140, 48, 85).ToVector3());
                //fx.Parameters["uColor1"].SetValue(new Color(176, 48, 62).ToVector3());
                fx.Parameters["uColor1"].SetValue(Color.Purple.ToVector3());
                fx.SafeSetParameter("uTime", Main.GlobalTimeWrappedHourly);
                //fx.CurrentTechnique.Passes["Noise"].Apply();
                fx.CurrentTechnique.Passes[2].Apply();

                if (Main.rand.NextBool(30))
                {
                    Utils.DrawBorderString(spriteBatch, line.Text, new Vector2(line.X, line.Y), Color.Lerp(Color.Red, Color.Transparent, 0.95f), 1f, Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, 0.5f));
                }
                //Utils.DrawBorderString(spriteBatch, line.text, new Vector2(line.X, line.Y), Color.Lerp(Color.Orange, Color.Transparent, 0.95f), 1f, Main.rand.NextBool(30) ? Main.rand.NextFloat(-0.5f, 0.5f) : 0, Main.rand.NextBool(30) ? Main.rand.NextFloat(-0.5f, 0.5f) : 0);
                Utils.DrawBorderString(spriteBatch, line.Text, new Vector2(line.X, line.Y), Color.Lerp(Color.Orange, Color.Transparent, 0.95f), 1f, Main.rand.NextBool(30) ? Main.rand.NextFloat(-0.5f, 0.5f) : 0, Main.rand.NextBool(30) ? Main.rand.NextFloat(-0.5f, 0.5f) : 0);
                Utils.DrawBorderString(spriteBatch, line.Text, new Vector2(line.X, line.Y), Color.White, 1f, Main.rand.NextBool(60) ? Main.rand.NextFloat(-0.45f, 0.45f) : 0);
                spriteBatch.Reload(SpriteSortMode.Deferred);
                return false;
            }
            return true;
        }
    }
}