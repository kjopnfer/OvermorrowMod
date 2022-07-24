using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.NPCs.Bosses.Eye;
using OvermorrowMod.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.DrawLayers
{
    public class MiniServantDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.FrontAccFront);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;

            foreach (NPC npc in Main.npc)
            {
                if (!(npc.ModNPC is MiniServant servant)) continue;

                if (servant.latchPlayer == drawPlayer)
                {
                    Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/MiniServant").Value;
                    Texture2D glow = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/MiniServant_Glow").Value;

                    Color color = Lighting.GetColor((int)npc.Center.X, (int)npc.Center.Y);


                    DrawData textureLayer = new DrawData(texture, npc.Center - Main.screenPosition, npc.frame, color, npc.rotation, npc.frame.Size() / 2, npc.scale, SpriteEffects.None, 1);
                    DrawData glowLayer = new DrawData(glow, npc.Center - Main.screenPosition, npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2, npc.scale, SpriteEffects.None, 1);

                    textureLayer.Draw(Main.spriteBatch);
                    glowLayer.Draw(Main.spriteBatch);
                }
            }
        }
    }
}
