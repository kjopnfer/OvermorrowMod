using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs
{
	public class GamerDust : ModNPC
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
        }
        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.width = 34;
            NPC.height = 36;
            NPC.noGravity = true;
            NPC.behindTiles = true;
            NPC.noTileCollide = true;
            NPC.lifeMax = 1;
            NPC.immortal = true;
            NPC.aiStyle = -1;
            NPC.hide = true; // needed for drawing infront of players
        }
        float AlphaGetReal = 1f;
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(0.75f, 0.75f, 0.75f, 1f) * AlphaGetReal;
        }
        float Rotation = 0f;
        public override void AI()
        {
            if (NPC.ai[0] == 0)
            {
                Rotation = MathHelper.ToRadians(Main.rand.Next(-3, 4));
                NPC.velocity = new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-15, 0) / 10f).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-1500, 1500) / 100f));
            }
            if (NPC.velocity.X != 0)
                NPC.velocity.X += (NPC.velocity.X < 0 ? 0.05f : -0.05f);
            NPC.rotation += Rotation + 0.05f;
            NPC.scale -= 0.005f;
            AlphaGetReal -= 0.01f;
            if (NPC.scale <= 0)
                NPC.active = false;
            NPC.ai[0]++;
        }
        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsOverPlayers.Add(index);
        }

    }
}