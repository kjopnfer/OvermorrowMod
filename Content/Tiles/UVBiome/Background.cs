using static Terraria.ModLoader.ModContent;
using System;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria.ID;
using Terraria.Graphics.Effects;
using OvermorrowMod.Common.Players;
using Terraria.Graphics.Shaders;

namespace OvermorrowMod.Content.Tiles.UVBiome
{
    public class Background : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.width = Main.screenWidth;
            NPC.height = Main.screenHeight;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            NPC.lifeMax = 20;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.friendly = true;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.npcSlots = 0f;
        }
        public override Color? GetAlpha(Color drawColor)
        {
            return Color.Transparent;
        }
        float ShaderAlpha = 0.5f;
        float ShaderRadius = 60f;
        float PerlinTimer = 150f;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, Request<Effect>("OvermorrowMod/Effects/UVBG").Value, Main.GameViewMatrix.ZoomMatrix);
            OvermorrowModPlayer player = Main.LocalPlayer.GetModPlayer<OvermorrowModPlayer>();
            MiscShaderData Thing = GameShaders.Misc["UVBG"];
            //Main.NewText(UVGogglesPplayer.RadiusArray4BG.Length);
            for (int i = 0; player.UVBubbles.Count > i; i++)
            {
                Thing.UseColor(1, player.UVBubbles[i].Radius, PerlinTimer);
                Texture2D Image = Request<Texture2D>(Texture).Value;
                Vector2 Pos = Vector2.One * 0 + player.UVBubbles[i].Position - Main.screenPosition - Image.Size() / 2;
                float xpoos = (1f / Main.rightWorld) * Main.screenPosition.X * 8;
                float ypoos = (1f / Main.bottomWorld) * Main.screenPosition.Y * 8;
                xpoos -= 0.5f;//Items.Testing.TestSpawnItem.BGPos.X;
                ypoos -= 0.5f;
                Thing.UseSecondaryColor(xpoos, ypoos, 0f);
                Thing.Apply();
                Main.spriteBatch.Draw(Image, Pos, Color.White);

                int MeInRealLife = (int)MathHelper.Clamp(i, 0, 5);
                Color UVColor = (Main.LocalPlayer.name.ToLower().StartsWith("frankfires") ? new Color(132, 194, 248) : new Color(0.7f, 0.25f, 0.95f));
                if (Main.netMode != NetmodeID.Server && !Filters.Scene[$"UVShader{MeInRealLife}"].IsActive())
                    Filters.Scene.Activate($"UVShader{MeInRealLife}", Main.MouseWorld).GetShader().UseColor(MathHelper.Clamp(ShaderAlpha, 0, 1), (float)player.UVBubbles[i].Radius, 0).UseTargetPosition(player.UVBubbles[i].Position).UseImage(Request<Texture2D>("OvermorrowMod/Effects/Noise1").Value).UseSecondaryColor(UVColor);
                if (Main.netMode != NetmodeID.Server && Filters.Scene[$"UVShader{MeInRealLife}"].IsActive())
                    Filters.Scene[$"UVShader{MeInRealLife}"].GetShader().UseColor(MathHelper.Clamp(ShaderAlpha, 0, 1), (float)player.UVBubbles[i].Radius, PerlinTimer).UseTargetPosition(player.UVBubbles[i].Position);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

        }
        public override void AI()
        {
            NPC.alpha = 0;
            NPC.Center = Main.MouseWorld;
            NPC.hide = true;
            NPC.behindTiles = false;
        }

        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsMoonMoon.Add(index);
        }
    }
}