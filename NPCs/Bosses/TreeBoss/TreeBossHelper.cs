using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.TreeBoss
{
    public partial class TreeBoss : ModNPC
    {
        private void BossText(string text) // boss messages
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                CombatText.NewText(npc.getRect(), new Color(0, 255, 191), text, true);
                Main.NewText(text, new Color(0, 255, 191));
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                CombatText.NewText(npc.getRect(), new Color(0, 255, 191), text, true);
                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(text), new Color(0, 255, 191));
            }
        }

        private static float FlowerDrawing(float theta)
        {
            return (float)(Math.Sin(theta) + Math.Sin(5 * theta / 2));
        }

        private Vector2 FlowerDrawing(Vector2 origin, float theta)
        {
            Vector2 output = new Vector2();

            output.X = (float)(FlowerDrawing(theta) * Math.Cos(theta));
            output.Y = (float)(FlowerDrawing(theta) * Math.Sin(theta));

            return output;
        }

        public void EyeFlare(SpriteBatch spriteBatch, int xOffset, int yOffset, Color LerpColor, bool DiscoColor = false)
        {
            Texture2D texture2 = ModContent.GetTexture("OvermorrowMod/Textures/test2");
            Rectangle rect2 = new Rectangle(0, 0, texture2.Width, texture2.Height);
            Vector2 drawOrigin2 = new Vector2(texture2.Width / 2, texture2.Height / 2);

            Color color = DiscoColor ? Main.DiscoColor : Color.Lerp(LerpColor, Color.White, (float)Math.Sin(npc.localAI[1] / 5f));

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            // Side and top flares
            spriteBatch.Draw(texture2, npc.Center + new Vector2(xOffset, yOffset) - Main.screenPosition, new Rectangle?(rect2), color, MathHelper.PiOver2, drawOrigin2, new Vector2(0.125f, MathHelper.Lerp(4, 5, (float)Math.Sin(npc.localAI[1] / 180f))), SpriteEffects.None, 0);
            spriteBatch.Draw(texture2, npc.Center + new Vector2(xOffset, yOffset) - Main.screenPosition, new Rectangle?(rect2), color, 0, drawOrigin2, new Vector2(0.125f, MathHelper.Lerp(0.75f, 1f, (float)Math.Sin(npc.localAI[1] / 180f))), SpriteEffects.None, 0);

            // The center circle
            spriteBatch.Draw(texture2, npc.Center + new Vector2(xOffset, yOffset) - Main.screenPosition, new Rectangle?(rect2), color, npc.rotation, drawOrigin2, 0.3f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
