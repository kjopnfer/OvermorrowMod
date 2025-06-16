using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Globals;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Misc
{
    public class DoorLock : ModNPC
    {
        public override bool CheckActive() => false;
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public virtual void SafeSetDefaults() { }
        public sealed override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 56;
            NPC.lifeMax = 1000;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.ShowNameOnHover = false;

            SafeSetDefaults();
        }

        public ModTileEntity tileEntity;
        public override void AI()
        {
            ArchiveDoor_TE instance = (ArchiveDoor_TE)TileEntity.ByID[tileEntity.ID];
            if (!instance.IsLocked) return;

            Lighting.AddLight(NPC.Center, 0f, 1f, 0.5f);
            NPC.ai[0]++;   
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D lockTexture = ModContent.Request<Texture2D>(AssetDirectory.Misc + Name).Value;
            Texture2D chainTexture = ModContent.Request<Texture2D>(AssetDirectory.Misc + "LockChain").Value;

            Vector2[] chainDirections = {
                Vector2.Normalize(new Vector2(-1, -1.05f)),
                Vector2.Normalize(new Vector2(1, -1.05f)),
                Vector2.Normalize(new Vector2(-1, 1f)),
                Vector2.Normalize(new Vector2(1, 1))
            };

            float max = 5;
            for (int direction = 0; direction < chainDirections.Length; direction++)
            {
                Vector2 chainDirection = chainDirections[direction];

                for (int i = 1; i <= max; i++)
                {
                    Vector2 chainPosition = NPC.Center + chainDirection * chainTexture.Height * i;

                    // Calculate rotation angle for the chain to match the direction
                    float rotation = (float)Math.Atan2(chainDirection.Y, chainDirection.X) + MathHelper.PiOver2;

                    // Calculate fade-out alpha based on distance from center
                    float fadeProgress = (float)i / max; // 3 is the max chain links
                    float alpha = MathHelper.Lerp(1f, 0.2f, fadeProgress); // Fade from 1.0 to 0.0

                    // Calculate pulsing shader progress
                    float timeOffset = (float)i * 25f; // Very fast chain-to-chain travel
                    float pulseSpeed = 0.05f; // Very fast pulse speed
                    float shaderProgress = MathF.Sin((NPC.ai[0] - timeOffset) * pulseSpeed) * 0.5f + 0.5f; // 0 to 1 sine wave
                    shaderProgress = MathHelper.Clamp(shaderProgress, 0f, 1f);

                    // Apply shader if progress is above threshold
                    if (shaderProgress > 0.1f)
                    {
                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                        Effect effect = OvermorrowModFile.Instance.ColorFill.Value;
                        effect.Parameters["ColorFillColor"].SetValue(Color.LightGreen.ToVector3());
                        effect.Parameters["ColorFillProgress"].SetValue(shaderProgress);
                        effect.CurrentTechnique.Passes["ColorFill"].Apply();
                    }

                    spriteBatch.Draw(
                        chainTexture,
                        chainPosition - screenPos,
                        new Rectangle(0, 0, chainTexture.Width, chainTexture.Height),
                        Color.White * alpha, // Apply fade-out alpha
                        rotation,
                        chainTexture.Size() / 2f,
                        1f,
                        SpriteEffects.None,
                        0
                    );

                    // Reset spritebatch if shader was applied
                    if (shaderProgress > 0.1f)
                    {
                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                    }
                }
            }

            spriteBatch.Draw(lockTexture, NPC.Center - screenPos, null, drawColor, 0f, lockTexture.Size() / 2f, 1f, SpriteEffects.None, 0);

            return false;
        }
    }
}