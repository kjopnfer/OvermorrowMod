using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss
{
    public class Barrier : ModNPC
    {
        // TODO: Synchronize booleans whenever netUpdate is called
        // TODO: Go through other classes that use ((Class)NPC.modNPC).variable and add a netUpdate call to them
        public int BarrierID;

        public Vector2 RotationCenter;
        private bool RunOnce = true;

        public bool Rotate = false;
        public bool Shockwave = false;

        private float RotationOffset;
        private float InitialRadius;
        private float Radius;

        public override bool CheckActive() => false;
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

        public ref float AICounter => ref npc.ai[0];
        public ref float MiscCounter => ref npc.ai[1];
        public ref float MiscCounter2 => ref npc.ai[2];
        public ref float RotationCounter => ref npc.ai[3];

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
            if (AICounter > 30 && AICounter < 60)
            {
                Radius = MathHelper.SmoothStep(InitialRadius + 25, InitialRadius, Utils.Clamp(npc.ai[0] - 30f, 0, 30) / 30f);
            }

            // Code that runs after the projectiles have spawned in
            if (AICounter++ > 90)
            {
                // Arena projectile tells it when to rotate
                if (Rotate)
                {
                    if (Radius < InitialRadius + 275)
                    {
                        Radius += 5;
                    }

                    RotationCounter -= 0.01f;
                }

                if (Shockwave)
                {
                    /*if (MiscCounter2 == 0)
                    {
                        InitialRadius = Radius;
                    }

                    Main.NewText((float)Math.Sin(MiscCounter2 / 60f));
                    Radius = MathHelper.Lerp(InitialRadius, InitialRadius + 75, (float)Math.Sin(MiscCounter2 / 30f));*/

                    if (MiscCounter2++ == 120)
                    {
                        Particle.CreateParticle(Particle.ParticleType<Shockwave2>(), npc.Center, Vector2.Zero, Color.Yellow);
                        Projectile.NewProjectile(npc.Center, npc.DirectionTo(RotationCenter) * 2, ModContent.ProjectileType<BarrierWave>(), 50, 0f, Main.myPlayer);

                        Shockwave = false;
                        MiscCounter2 = 0;
                    }
                }

                // Counter for the glowmask
                MiscCounter++;
            }

            npc.Center = RotationCenter + new Vector2(Radius, 0).RotatedBy(RotationOffset + RotationCounter);
            npc.rotation = npc.DirectionTo(RotationCenter).ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (AICounter > 30 && !RunOnce)
            {
                spriteBatch.Reload(SpriteSortMode.Immediate);

                Effect effect = OvermorrowModFile.Mod.Whiteout;
                if (!Main.gamePaused) npc.localAI[0]++;

                float progress = Utils.Clamp(npc.localAI[0], 0, 15f) / 15f;

                if (progress < 1)
                {
                    effect.Parameters["WhiteoutColor"].SetValue(Color.Yellow.ToVector3());
                    effect.Parameters["WhiteoutProgress"].SetValue(1 - progress);
                    effect.CurrentTechnique.Passes["Whiteout"].Apply();
                }

                Texture2D texture = Main.npcTexture[npc.type];
                Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);

                Color color = Lighting.GetColor((int)npc.Center.X / 16, (int)(npc.Center.Y / 16f));

                spriteBatch.Draw(texture, npc.Center + new Vector2(0, 4) - Main.screenPosition, null, color, npc.rotation, origin, 1f, SpriteEffects.None, 0f);

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
                Main.spriteBatch.Draw(texture, npc.Center + new Vector2(0, 4) - Main.screenPosition, null, Color.Lerp(Color.Transparent, Color.White, MiscCounter / 60f), npc.rotation, origin, 1f, SpriteEffects.None, 0f);
            }
        }
    }

    public class Spin : ModNPC, ITrailEntity
    {
        public Color TrailColor(float progress) => Color.Lerp(Color.Yellow, Color.Orange, progress) * progress;
        public float TrailSize(float progress) => 60;
        public Type TrailType() => typeof(SpinTrail);

        public Vector2 RotationCenter;
        private bool RunOnce = true;

        public bool Rotate = false;

        private float RotationOffset;
        private float InitialRadius;
        private float Radius;

        public override string Texture => AssetDirectory.Melee + "SoulSaber/SoulSaber";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
        }

        public override void SetDefaults()
        {
            npc.width = 140;
            npc.height = 140;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.knockBackResist = 0f;
            npc.lifeMax = 100;
            npc.aiStyle = -1;
            npc.friendly = false;
            npc.dontTakeDamage = true;
            npc.alpha = 255;
        }

        public override void AI()
        {
            // Initialization step to save input into variables
            if (RunOnce)
            {
                RotationCenter = new Vector2(npc.ai[0], npc.ai[1]);
                RotationOffset = npc.ai[2];
                InitialRadius = npc.ai[3];
                Radius = InitialRadius; // Spawn offset from the circumference so that they "slide" inwards

                RunOnce = false;
                npc.ai[0] = 0;
                npc.ai[1] = 0;
                npc.ai[2] = 0;
                npc.ai[3] = 0;
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

                    npc.ai[2] -= 0.1f;
                }

                // Counter for the glowmask
                npc.ai[1]++;
            }

            npc.Center = RotationCenter + new Vector2(Radius, 0).RotatedBy(RotationOffset + npc.ai[2]);
            npc.rotation = npc.DirectionTo(RotationCenter).ToRotation() + MathHelper.PiOver4 + MathHelper.Pi;
        }
    }
}