using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core.NPCs;
using System.Diagnostics.Metrics;
using System.Xml.Linq;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using static Terraria.GameContent.PlayerEyeHelper;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Biomes;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class Waxhead : ModNPC
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + Name;
        public override void SetDefaults()
        {
            NPC.width = 80;
            NPC.height = 300;
            NPC.lifeMax = 30;
            NPC.aiStyle = -1;
            NPC.defense = 16;
            NPC.knockBackResist = 0f;
            NPC.noGravity = false;
            //NPC.hide = true;

            SpawnModBiomes = [ModContent.GetInstance<GrandArchives>().Type];
        }

        public override void DrawBehind(int index)
        {
            //Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }

        public override void AI()
        {
            NPC.Move(Main.LocalPlayer.Center, 0.1f, 2f, 1f);
            NPC.direction = NPC.GetDirectionFrom(Main.LocalPlayer);
        }

        int xFrame = 0;
        int yFrame = 0;
        private void SetFrame()
        {
            //if (NPC.IsABestiaryIconDummy)
            {
                xFrame = 1;

                if (NPC.frameCounter++ % 6 == 0)
                {
                    yFrame++;
                    if (yFrame >= 13) yFrame = 0;
                }

                return;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            SetFrame();

            NPC.spriteDirection = NPC.direction;
            NPC.frame.Width = TextureAssets.Npc[NPC.type].Value.Width / 2;
            NPC.frame.Height = TextureAssets.Npc[NPC.type].Value.Height / 13;

            NPC.frame.X = NPC.frame.Width * xFrame;
            NPC.frame.Y = NPC.frame.Height * yFrame;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var maxFramesY = 13;
            var maxWalkFramesY = 7;

            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 offset = new Vector2(0, -14);
            spriteBatch.Draw(texture, NPC.Center + offset - Main.screenPosition, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            return false;
        }
    }
}