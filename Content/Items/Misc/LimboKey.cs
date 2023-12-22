using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Misc
{
    public class LimboKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Limbo Key");
            // Tooltip.SetDefault("'Opens a lock that doesn't quite exist'");
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 26;
            Item.rare = ItemRarityID.White;
            Item.maxStack = 1;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.consumable = false;
            Item.useStyle = ItemUseStyleID.Swing;
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            if (line.Name == "ItemName")
            {
                spriteBatch.Reload(SpriteSortMode.Immediate);
                Effect fx = OvermorrowModFile.Instance.TextShader.Value;
                Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("OvermorrowMod/Assets/Textures/Perlin").Value;
                fx.Parameters["uColor0"].SetValue(Color.Black.ToVector3());
                //fx.Parameters["uColor1"].SetValue(Color.Black.ToVector3());
                //fx.Parameters["uColor0"].SetValue(new Color(140, 48, 85).ToVector3());
                //fx.Parameters["uColor1"].SetValue(new Color(176, 48, 62).ToVector3());
                fx.Parameters["uColor1"].SetValue(Color.White.ToVector3());
                fx.SafeSetParameter("uTime", Main.GlobalTimeWrappedHourly);
                //fx.CurrentTechnique.Passes["Noise"].Apply();
                fx.CurrentTechnique.Passes[0].Apply();

                Utils.DrawBorderString(spriteBatch, line.Text, new Vector2(line.X, line.Y), Color.White, 1f);
                spriteBatch.Reload(SpriteSortMode.Deferred);
                return false;
            }
            return true;
        }
    }
}