using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.DrawLayers
{
    public class GuardianBarDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.Shield);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            Vector2 scale = new Vector2(6f, 3f);
            DrawData drawData = new DrawData(ModContent.Request<Texture2D>("Terraria/Images/Misc/Perlin").Value,
                drawPlayer.Center - Main.screenPosition + drawPlayer.Size * scale * 0.5f,
                new Rectangle(0, 0, drawPlayer.width, drawPlayer.height),
                Color.LightGreen,
                drawPlayer.bodyRotation,
                drawPlayer.Size,
                scale,
                SpriteEffects.None, 0);

            GameShaders.Misc["ForceField"].UseColor(Color.LightGreen);
            GameShaders.Misc["ForceField"].Apply(drawData);

            drawData.Draw(Main.spriteBatch);
            //Main.playerDrawData.Add(drawData);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
