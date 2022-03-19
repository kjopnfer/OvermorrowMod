using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            DisplayName.SetDefault("Limbo Key");
            Tooltip.SetDefault("'Opens a lock that doesn't quite exist'");
        }

        public override void SetDefaults()
        {
            item.width = 34;
            item.height = 26;
            item.rare = ItemRarityID.White;
            item.maxStack = 1;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 15;
            item.consumable = false;
            item.useStyle = ItemUseStyleID.SwingThrow;
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            if (line.Name == "ItemName")
            {
                spriteBatch.Reload(SpriteSortMode.Immediate);
                Effect fx = OvermorrowModFile.Mod.TextShader;
                Main.graphics.GraphicsDevice.Textures[0] = mod.GetTexture("Assets/Textures/Perlin");
                fx.Parameters["uColor0"].SetValue(Color.Black.ToVector3());
                //fx.Parameters["uColor1"].SetValue(Color.Black.ToVector3());
                //fx.Parameters["uColor0"].SetValue(new Color(140, 48, 85).ToVector3());
                //fx.Parameters["uColor1"].SetValue(new Color(176, 48, 62).ToVector3());
                fx.Parameters["uColor1"].SetValue(Color.White.ToVector3());
                fx.SafeSetParameter("uTime", Main.GlobalTime);
                //fx.CurrentTechnique.Passes["Noise"].Apply();
                fx.CurrentTechnique.Passes[0].Apply();

                Utils.DrawBorderString(spriteBatch, line.text, new Vector2(line.X, line.Y), Color.White, 1f);
                spriteBatch.Reload(SpriteSortMode.Deferred);
                return false;
            }
            return true;
        }
    }
}