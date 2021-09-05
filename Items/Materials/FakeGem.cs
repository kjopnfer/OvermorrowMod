using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Tiles.Block;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Materials
{
    public class FakeGem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Ore");
            Tooltip.SetDefault("Used to craft cursed items\n'Mimicking common ores, they are the bane of the unwary'");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 28;
            item.rare = ItemRarityID.Blue;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            if (line.Name == "ItemName")
            {
                spriteBatch.Reload(SpriteSortMode.Immediate);
                Effect fx = OvermorrowModFile.Mod.TextShader;
                Main.graphics.GraphicsDevice.Textures[0] = mod.GetTexture("Textures/Perlin");
                fx.Parameters["uColor0"].SetValue(Color.Red.ToVector3());
                //fx.Parameters["uColor1"].SetValue(Color.Black.ToVector3());
                //fx.Parameters["uColor0"].SetValue(new Color(140, 48, 85).ToVector3());
                //fx.Parameters["uColor1"].SetValue(new Color(176, 48, 62).ToVector3());
                fx.Parameters["uColor1"].SetValue(Color.Purple.ToVector3());
                fx.SafeSetParameter("uTime", Main.GlobalTime);
                //fx.CurrentTechnique.Passes["Noise"].Apply();
                fx.CurrentTechnique.Passes[0].Apply();
                Utils.DrawBorderString(spriteBatch, line.text, new Vector2(line.X, line.Y), Color.Lerp(Color.Red, Color.Transparent, 0.95f), 1f, Main.rand.NextFloat(-0.3f, 0.3f), Main.rand.NextFloat(-0.2f, 0.2f));
                Utils.DrawBorderString(spriteBatch, line.text, new Vector2(line.X, line.Y), Color.Lerp(Color.Orange, Color.Transparent, 0.95f), 1f, Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(-0.4f, 0.4f));
                Utils.DrawBorderString(spriteBatch, line.text, new Vector2(line.X, line.Y), Color.White, 1f);
                spriteBatch.Reload(SpriteSortMode.Deferred);
                return false;
            }
            return true;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(item.Center, 1f, 0f, 0f);
        }
    }
}