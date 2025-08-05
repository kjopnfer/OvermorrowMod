using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Biomes;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public partial class Waxhead : ModNPC
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + Name;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void SetDefaults()
        {
            NPC.width = 80;
            NPC.height = 300;
            NPC.lifeMax = 30;
            NPC.aiStyle = -1;
            NPC.defense = 16;
            NPC.damage = 48;
            NPC.lifeMax = 3000;
            NPC.knockBackResist = 0f;
            NPC.noGravity = false;
            SpawnModBiomes = [ModContent.GetInstance<GrandArchives>().Type];
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            InitializeChainArm(source);
        }

        public override void AI()
        {
            NPC.Move(Main.LocalPlayer.Center, 0.1f, 2f, 1f);
            NPC.direction = NPC.GetDirectionFrom(Main.LocalPlayer);

            UpdateChainArm();
            DrawChainArmDebugDust();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // Draw main NPC
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 offset = new Vector2(0, -14);
            spriteBatch.Draw(texture, NPC.Center + offset - Main.screenPosition, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            // Draw chain arm
            DrawChainArm(spriteBatch, screenPos, drawColor);

            return false;
        }

        // Keep your existing frame code
        int xFrame = 0;
        int yFrame = 0;
        private void SetFrame()
        {
            xFrame = 1;
            if (NPC.frameCounter++ % 6 == 0)
            //if (NPC.frameCounter++ % 20 == 0)
            {
                yFrame++;
                if (yFrame >= 13) yFrame = 0;
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
    }
}