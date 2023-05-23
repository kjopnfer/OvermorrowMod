using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs
{
	public class ChargeEffect : ModNPC
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
        }
        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.width = 100;
            NPC.height = 100;
            NPC.noGravity = true;
            NPC.behindTiles = true;
            NPC.noTileCollide = true;
            NPC.lifeMax = 1;
            NPC.immortal = true;
            NPC.aiStyle = -1;
            NPC.hide = true; // needed for drawing infront of players
            NPC.dontTakeDamageFromHostiles = true;
        }
        float AlphaGetReal = 1f;
        /*public override Color? GetAlpha(Color lightColor)
        {
            return new Color(0.75f, 0.75f, 0.75f, 1f) * AlphaGetReal;
        }*/
        float Stretch = 1f;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 Scale = new Vector2(MathHelper.Clamp(1f / Stretch, 0.05f, int.MaxValue), Stretch) / 4f;
            Vector2 Center = NPC.position + ((NPC.Size / 2) * Scale);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.Transform);
            spriteBatch.Draw(ModContent.Request<Texture2D>("OvermorrowMod/Content/NPCs/ChargeEffect").Value, Center - Main.screenPosition, null, Color.Cyan * AlphaGetReal, (Main.LocalPlayer.MountedCenter - Center).ToRotation() - MathHelper.ToRadians(-90), (NPC.Size / 2) * Scale, Scale, SpriteEffects.None, 0f);
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.Transform);
        }
        public override void AI()
        {
            bool AltAI = NPC.ai[1] == 1;
            if (NPC.ai[0] == 0)
            {
                Stretch = AltAI ? 2 : 1;
            }
            Player player = Main.LocalPlayer;
            NPC.velocity += AltAI ? NPC.DirectionTo(NPC.Center + new Vector2(0, 100).RotatedBy(NPC.ai[2])) * 0.5f : NPC.DirectionTo(player.MountedCenter) * 0.1f;
            //npc.scale -= 0.005f;
            AlphaGetReal -= 0.05f;
            Stretch += AltAI ? -0.05f : 0.05f;
            if (AlphaGetReal <= 0)
                NPC.active = false;
            NPC.ai[0]++;
        }
        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsOverPlayers.Add(index);
        }

    }
}