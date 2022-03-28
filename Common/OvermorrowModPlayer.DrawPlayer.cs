using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.Buffs.Summon;
using OvermorrowMod.Content.Items.Weapons.Summoner.IorichWand;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public partial class OvermorrowModPlayer : ModPlayer
    {
        /*private void DrawGuardianBar(PlayerDrawInfo drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            OvermorrowModPlayer modPlayer = drawPlayer.GetModPlayer<OvermorrowModPlayer>();

            Main.spriteBatch.Reload(BlendState.Additive);
            //Texture2D ChargeContainer = ModContent.GetTexture(AssetDirectory.UI + "SoulContainerS");
            Texture2D ChargeContainer = ModContent.GetTexture(AssetDirectory.Textures + "TEST");
            Texture2D ChargeBar = ModContent.GetTexture(AssetDirectory.UI + "SoulMeterSFBar");

            const int textureHeight = 24;

            int frame = 5;
            var drawRectangleMeter = new Rectangle(0, textureHeight * frame, ChargeContainer.Width, textureHeight);

            Vector2 PlayerPosition = drawPlayer.position;
            //Vector2 ContainerPosition = new Vector2((int)(PlayerPosition.X - (double)Main.screenPosition.X - drawPlayer.bodyFrame.Width / 2 + drawPlayer.width / 2), (int)(PlayerPosition.Y - (double)Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4.0) + 153 + 60) + drawPlayer.bodyPosition + new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2);
            Vector2 ContainerPosition = new Vector2((int)(PlayerPosition.X - (double)Main.screenPosition.X - drawPlayer.bodyFrame.Width / 2 + drawPlayer.width / 2), (int)(PlayerPosition.Y - (double)Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 16.0)) + drawPlayer.bodyPosition + new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2);

            //DrawData drawData = new DrawData(ChargeContainer, ContainerPosition, drawRectangleMeter, Color.White, drawPlayer.bodyRotation, ChargeContainer.Size() / 2f, 1f, SpriteEffects.None, 0);
            DrawData drawData = new DrawData(ChargeContainer, ContainerPosition, null, Color.White, drawPlayer.bodyRotation, ChargeContainer.Size() / 2f, 1f, SpriteEffects.None, 0);
            //Main.playerDrawData.Add(drawData);

            drawData.Draw(Main.spriteBatch);

            Rectangle drawRectangleBar = new Rectangle(0, textureHeight * 5, (int)(ChargeBar.Width * MathHelper.Lerp(0, 1, modPlayer.IorichGuardianEnergy / 100f)), textureHeight);
            drawData = new DrawData(ChargeBar, ContainerPosition + new Vector2(19, 0), drawRectangleBar, Color.White, drawPlayer.bodyRotation, ChargeContainer.Size() / 2f, 1f, SpriteEffects.None, 0);
            //Main.playerDrawData.Add(drawData);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);

        }*/

        public int frameCounter;
        public int frame = 0;
        private void DrawGuardianBar(PlayerDrawInfo drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            DrawData drawData = new DrawData();
            Vector2 Position = drawPlayer.position;

            Vector2 position = new Vector2((int)(Position.X - (double)Main.screenPosition.X - drawPlayer.bodyFrame.Width / 2 + drawPlayer.width / 2), (int)(Position.Y - (double)Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4.0) + 3224) + drawPlayer.bodyPosition + new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2);

            Main.spriteBatch.Reload(BlendState.Additive);

            Texture2D ChargeContainer = ModContent.GetTexture(AssetDirectory.Textures + "TESTCIRCLE");

            frameCounter++;

            if (frameCounter % 3f == 2f) // Ticks per frame
            {
                frame += 1;
            }

            if (frame >= 26) // 6 is max # of frames
            {
                frame = 0; // Reset back to default
            }

            var drawRectangleMeter = new Rectangle(0, 256 * frame, ChargeContainer.Width, 256);
            drawData = new DrawData(ChargeContainer, position, drawRectangleMeter, Color.White, drawPlayer.bodyRotation, ChargeContainer.Size() / 2f, 1f, SpriteEffects.None, 0);
            drawData.Draw(Main.spriteBatch);

            Main.spriteBatch.Reload(BlendState.AlphaBlend);
        }
        private void DrawShield(PlayerDrawInfo drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            Vector2 scale = new Vector2(6f, 3f);
            DrawData drawData = new DrawData(ModContent.GetTexture("Terraria/Misc/Perlin"),
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
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            int body = layers.FindIndex(l => l == PlayerLayer.MiscEffectsFront);
            if (body < 0)
                return;

            if (player.HeldItem.type == ModContent.ItemType<IorichWand>() && player.HasBuff(ModContent.BuffType<IorichGuardian>())) 
                layers.Insert(body - 1, new PlayerLayer(mod.Name, "Body", DrawGuardianBar));

            if (player.GetModPlayer<OvermorrowModPlayer>().iorichGuardianShield)
                layers.Insert(body - 1, new PlayerLayer(mod.Name, "Body", DrawShield));
        }
    }
}