using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.SandstormBoss
{
    public class LightBullet : ModNPC
    {
        private bool RunOnce = true;
        private NPC ParentNPC;
        private float RandomVelocity;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Light Warping Bullets");
            NPCID.Sets.TrailCacheLength[npc.type] = 20;
            NPCID.Sets.TrailingMode[npc.type] = 2;
        }

        public override void SetDefaults()
        {
            npc.width = npc.height = 16;
            npc.friendly = false;
            npc.timeLeft = 5;
            npc.lifeMax = 100;
            npc.aiStyle = -1;
            npc.noTileCollide = true;
        }

        private ref float Direction => ref npc.ai[0];
        private ref float InitialRotation => ref npc.ai[1];
        private ref float MiscCounter => ref npc.ai[2];
        private ref float Angle => ref npc.ai[3];

        public override void AI()
        {
            if (RunOnce)
            {
                ParentNPC = Main.npc[(int)npc.ai[0]];

                int RotationDirection = Main.rand.NextBool() ? 1 : -1;
                int RotationAngle = Main.rand.Next(4, 8);

                Direction = RotationDirection;
                Angle = RotationAngle;
                RandomVelocity = Main.rand.NextFloat(0.4f, 0.6f);


                RunOnce = false;
            }

            if (ParentNPC.active)
            {
                npc.timeLeft = 5;
            }

            if (MiscCounter++ > 240)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];
                    if (player.active && npc.Distance(player.Center) < 2000)
                    {
                        Vector2 Target = npc.Center - player.Center;
                        npc.velocity = Vector2.Lerp(npc.velocity, Target.SafeNormalize(Vector2.UnitX) * -6, RandomVelocity);
                        npc.velocity = npc.velocity.RotatedBy(MathHelper.ToRadians(Angle * Direction));

                        break;
                    }
                }
            }
            else
            {
                npc.Center = ParentNPC.Center + new Vector2(206, 0).RotatedBy(MathHelper.ToRadians(InitialRotation + ParentNPC.ai[1]));
            }

            Vector2 RandomDirection = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 2;
            //for (int i = 0; i < 3; i++)
            //    Particle.CreateParticle(Particle.ParticleType<Glow1>(), npc.Center, RandomDirection, Color.Yellow, 1, 1f, 0, Main.rand.NextFloat(0.7f, 1.3f), 0, Main.rand.Next(20, 30));
            //for (int i = 0; i < 3; i++)
            //    Particle.CreateParticle(Particle.ParticleType<Glow1>(), npc.position + new Vector2(npc.Size.X * Main.rand.NextFloat(), npc.Size.Y * Main.rand.NextFloat()), Vector2.Zero, new Color(0.4f, 0.2f, 0.01f), 1f, Main.rand.NextFloat(0.7f, 1.3f), 0, Main.rand.Next(20, 30));

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            //Main.spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = Main.npcTexture[npc.type];
            Vector2 drawOrigin = new Vector2(Main.npcTexture[npc.type].Width * 0.5f, npc.height * 0.5f);
            for (int k = 0; k < npc.oldPos.Length; k++)
            {
                // Adjust drawPos if the hitbox does not match sprite dimension
                Vector2 drawPos = npc.oldPos[k] - Main.screenPosition + drawOrigin;
                Color afterImageColor = Color.LightGreen;
                Color color = npc.GetAlpha(afterImageColor) * ((npc.oldPos.Length - k) / (float)npc.oldPos.Length);
                spriteBatch.Draw(texture, drawPos, npc.frame, color, npc.rotation, drawOrigin, npc.scale * (NPCID.Sets.TrailCacheLength[npc.type] - k) / NPCID.Sets.TrailCacheLength[npc.type], SpriteEffects.None, 0f);
            }

            for (float i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i += 0.2f)
            {
                Texture2D glow = Main.npcTexture[npc.type];
                Color color27 = Color.Yellow;
                color27 *= (float)(NPCID.Sets.TrailCacheLength[npc.type] - i) / NPCID.Sets.TrailCacheLength[npc.type];
                float scale = npc.scale;
                scale *= (float)(NPCID.Sets.TrailCacheLength[npc.type] - i) / NPCID.Sets.TrailCacheLength[npc.type];
                int max0 = (int)i - 1; 
                if (max0 < 0)
                    continue;
                Vector2 center = Vector2.Lerp(npc.oldPos[(int)i], npc.oldPos[max0], 1 - i % 1);
                float smoothtrail = i % 1 * MathHelper.Pi / 6.85f;

                center += npc.Size / 2;

                Main.spriteBatch.Draw(
                    glow,
                    center - Main.screenPosition + new Vector2(0, npc.gfxOffY),
                    null,
                    color27,
                    npc.rotation,
                    glow.Size() / 2,
                    scale,
                    SpriteEffects.None,
                    0f);
            }

            //Main.spriteBatch.Reload(BlendState.AlphaBlend);

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture = ModContent.GetTexture("OvermorrowMod/NPCs/Bosses/SandstormBoss/PillarSpawner");
            //float mult = (0.55f + (float)Math.Sin(Main.GlobalTime) * 0.1f);
            //float scale = npc.scale * 2 * mult;

            //Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, Color.Gold, 0, new Vector2(texture.Width, texture.Height) / 2, 1, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, Color.White, 0, new Vector2(texture.Width, texture.Height) / 2, 0.5f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }

}