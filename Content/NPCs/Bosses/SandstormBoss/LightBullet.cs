using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss
{
    public class LightBullet : ModNPC
    {
        private bool RunOnce = true;
        private NPC ParentNPC;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.LostSoulFriendly;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Light Warping Bullets");
        }

        public override void SetDefaults()
        {
            npc.width = npc.height = 16;
            npc.friendly = false;
            npc.timeLeft = 5;
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
                int RotationAngle = Main.rand.Next(6, 10);

                npc.ai[0] = RotationDirection;
                npc.ai[3] = RotationAngle;


                RunOnce = false;
            }

            if (ParentNPC.active)
            {
                npc.timeLeft = 5;
            }

            if (MiscCounter++ > 240)
            {
                Main.NewText("run");

                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];
                    if (player.active && npc.Distance(player.Center) < 2000)
                    {
                        Vector2 Target = npc.Center - player.Center;
                        npc.velocity = Vector2.Lerp(npc.velocity, Target.SafeNormalize(Vector2.UnitX) * -6, 0.1f);
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
            Particle.CreateParticle(Particle.ParticleType<Orb>(), npc.Center, RandomDirection, Color.Orange, 1, Main.rand.NextFloat(0.25f, 0.4f), 0, 25);

        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture = ModContent.GetTexture("OvermorrowMod/Content/NPCs/Bosses/SandstormBoss/PillarSpawner");
            //float mult = (0.55f + (float)Math.Sin(Main.GlobalTime) * 0.1f);
            //float scale = npc.scale * 2 * mult;

            Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, Color.Gold, 0, new Vector2(texture.Width, texture.Height) / 2, 1, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, Color.White, 0, new Vector2(texture.Width, texture.Height) / 2, 0.5f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }

}