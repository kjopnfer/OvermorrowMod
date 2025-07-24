using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Interfaces;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class Test : ModNPC, IOutlineEntity
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.AbigailMinion}";

        public bool ShouldDrawOutline => true;

        public Color OutlineColor => Color.Red;

        public Color? FillColor => Color.Black;

        public Texture2D FillTexture => null;

        public Action<SpriteBatch, GraphicsDevice, int, int> SharedGroupDrawFunction => null;

        public Action<SpriteBatch, GraphicsDevice, Entity> IndividualEntityDrawFunction => null;

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.Zombie);
            NPC.aiStyle = 3;

            AIType = NPCID.Zombie; // Use vanilla zombie's type when executing AI code. (This also means it will try to despawn during daytime)
            AnimationType = NPCID.Zombie; // Use vanilla zombie's type when executing animation code. Important to also match Main.npcFrameCount[NPC.type] in SetStaticDefaults.
        }

        public override void AI()
        {
            //NPC.AddBarrier(50, 600); // 50 BP, 10 seconds

            base.AI();
        }

        private void DrawSharedBackground(SpriteBatch spriteBatch, GraphicsDevice gD, int screenWidth, int screenHeight)
        {
            // Draw the continuous tiled background once for the entire group
            Texture2D backgroundTexture = ModContent.Request<Texture2D>(AssetDirectory.Backgrounds + "ArchiveBackground", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int tilesX = (screenWidth / backgroundTexture.Width) + 2;
            int tilesY = (screenHeight / backgroundTexture.Height) + 2;

            // Parallax factor - lower values = slower background movement
            float parallaxFactor = 0.5f; // Background moves at half camera speed

            Vector2 offset = new Vector2(
                (Main.screenPosition.X * parallaxFactor) % backgroundTexture.Width,
                (Main.screenPosition.Y * parallaxFactor) % backgroundTexture.Height
            );

            for (int x = -1; x < tilesX; x++)
            {
                for (int y = -1; y < tilesY; y++)
                {
                    Vector2 position = new Vector2(x * backgroundTexture.Width, y * backgroundTexture.Height) - offset;
                    spriteBatch.Draw(backgroundTexture, position, Color.White);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "ArchiveRat").Value;
            int yFrame = (int)((Main.GameUpdateCount / 6) % 9);
            var frame = new Rectangle(0, yFrame * (texture.Height / 9), texture.Width / 10, texture.Height / 9);

            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, frame, Color.White, 0f, frame.Size() / 2, 1f, SpriteEffects.None, 0);

            return false;
        }
    }
}