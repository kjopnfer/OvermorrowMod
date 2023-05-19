using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.WorldGeneration;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Town.Sojourn
{
    public abstract class TownGuard : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.width = 18;
            NPC.height = 38;
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.aiStyle = -1;
            NPC.damage = 30;
            NPC.defense = 30;
            NPC.lifeMax = 500;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.4f;
        }

        public override void AI()
        {
            foreach (Player player in Main.player)
            {
                if (!player.active) continue;

                if (player.talkNPC == NPC.whoAmI) return;
            }

            NPC.TargetClosest();
        }
    }

    [AutoloadHead]
    public class SojournGuard : TownGuard
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Town Guard");
            Main.npcFrameCount[NPC.type] = 1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float xOffset = NPC.direction == 1 ? 0 : 0;
            Vector2 drawOffset = new Vector2(xOffset, -10);

            spriteBatch.Draw(texture, NPC.Center + drawOffset - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            return false;
        }
    }

    [AutoloadHead]
    public class SojournGuard2 : TownGuard
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Town Guard");
            Main.npcFrameCount[NPC.type] = 1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float xOffset = NPC.direction == 1 ? 0 : 0;
            Vector2 drawOffset = new Vector2(xOffset, -10);

            spriteBatch.Draw(texture, NPC.Center + drawOffset - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            return false;
        }
    }

    [AutoloadHead]
    public class SojournGuard3 : TownGuard
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Town Guard");
            Main.npcFrameCount[NPC.type] = 1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float xOffset = NPC.direction == 1 ? 0 : 0;
            Vector2 drawOffset = new Vector2(xOffset, -10);

            spriteBatch.Draw(texture, NPC.Center + drawOffset - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            return false;
        }
    }

}