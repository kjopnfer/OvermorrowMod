using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss
{
    public class Barrier : ModNPC
    {
        public Vector2 RotationCenter;
        private bool RunOnce = true;

        public bool Rotate = false;

        private float RotationOffset;
        private float InitialRadius;
        private float Radius;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forbidden Barrier");
        }

        public override void SetDefaults()
        {
            npc.width = 186;
            npc.height = 186;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.knockBackResist = 0f;
            npc.lifeMax = 100;
            npc.aiStyle = -1;
            npc.friendly = false;
            npc.dontTakeDamage = true;
        }

        public override void AI()
        {
            // Initialization step to save input into variables
            if (RunOnce)
            {
                RotationCenter = new Vector2(npc.ai[0], npc.ai[1]);
                RotationOffset = npc.ai[2];
                InitialRadius = npc.ai[3];
                Radius = InitialRadius + 25; // Spawn offset from the circumference so that they "slide" inwards

                RunOnce = false;
                npc.ai[0] = 0;
                npc.ai[1] = 0;
                npc.ai[2] = 0;
                npc.ai[3] = 0;

                float radius = 60;
                int numLocations = 10;
                for (int i = 0; i < numLocations; i++)
                {
                    Vector2 position = npc.Center + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                    Vector2 dustvelocity = new Vector2(0f, 5f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                    int dust = Dust.NewDust(position, 2, 2, DustID.Enchanted_Gold, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                    Main.dust[dust].noGravity = true;
                }
            }

            // Code for the barriers to shift inwards
            if (npc.ai[0] > 30 && npc.ai[0] < 60)
            {
                Radius = MathHelper.SmoothStep(InitialRadius + 25, InitialRadius, Utils.Clamp(npc.ai[0] - 30f, 0, 30) / 30f);
            }

            // Code that runs after the projectiles have spawned in
            if (npc.ai[0]++ > 90)
            {
                // Arena projectile tells it when to rotate
                if (Rotate)
                {
                    if (Radius < InitialRadius + 275)
                    {
                        Radius += 5;
                    }

                    npc.ai[2] += 0.01f;
                }

                // Counter for the glowmask
                npc.ai[1]++;
            }

            npc.Center = RotationCenter + new Vector2(Radius, 0).RotatedBy(RotationOffset + npc.ai[2]);
            npc.rotation = npc.DirectionTo(RotationCenter).ToRotation() + MathHelper.PiOver2;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (npc.ai[0] > 30 && !RunOnce)
            {
                spriteBatch.Reload(SpriteSortMode.Immediate);

                Effect effect = OvermorrowModFile.Mod.Whiteout;
                if (!Main.gamePaused) npc.localAI[0]++;

                float progress = Utils.Clamp(npc.localAI[0], 0, 15f) / 15f;
                effect.Parameters["WhiteoutColor"].SetValue(Color.Yellow.ToVector3());
                effect.Parameters["WhiteoutProgress"].SetValue(1 - progress);
                effect.CurrentTechnique.Passes["Whiteout"].Apply();

                Texture2D texture = Main.npcTexture[npc.type];
                Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);

                spriteBatch.Draw(texture, npc.Center + new Vector2(0, 4) - Main.screenPosition, null, Color.White, npc.rotation, origin, 1f, SpriteEffects.None, 0f);

                spriteBatch.Reload(SpriteSortMode.Deferred);
            }

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = ModContent.GetTexture(AssetDirectory.Boss + "SandstormBoss/Barrier_Lines");
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);

            if (npc.ai[0] > 90 && !RunOnce)
            {
                Main.spriteBatch.Draw(texture, npc.Center + new Vector2(0, 4) - Main.screenPosition, null, Color.Lerp(Color.Transparent, Color.White, npc.ai[1] / 60f), npc.rotation, origin, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}