using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Particles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.SandstormBoss
{
    public class DharuudMinion : PullableNPC
    {
        public NPC ParentNPC = null;
        public bool ExecuteAttack = false;
        protected Vector2 InitialPosition;
        protected bool RunOnce = true;

        public ref float ParentID => ref npc.ai[0];
        public ref float GlobalCounter => ref npc.ai[1];
        public ref float RotationCounter => ref npc.ai[2];
        public ref float AttackCounter => ref npc.ai[3];
    }

    public class LaserMinion : DharuudMinion
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.Meteor3;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forbidden Artifact");
        }

        public override void SetDefaults()
        {
            npc.width = npc.height = 24;
            npc.aiStyle = -1;
            npc.lifeMax = 100;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.friendly = false;
        }

        public override void AI()
        {
            base.AI();

            if (RunOnce)
            {
                ParentNPC = Main.npc[(int)npc.ai[0]];
                RunOnce = false;
            }

            Vector2 IdlePosition = ParentNPC.Center + new Vector2(100, 0).RotatedBy(MathHelper.ToRadians(npc.ai[2] += 2f));
            if (!Grappled && !ExecuteAttack)
            {
                npc.Center = IdlePosition;
            }

            // Positions itself above the boss, before firing a beam in a wide arc
            if (ExecuteAttack)
            {
                npc.TargetClosest(true);
                Player player = Main.player[npc.target];

                if (AttackCounter <= 60f)
                {
                    if (AttackCounter == 0)
                    {
                        InitialPosition = npc.Center;
                    }

                    npc.Center = Vector2.Lerp(InitialPosition, ParentNPC.Center - new Vector2(0, 250), AttackCounter / 60f);
                }

                if (AttackCounter == 120)
                {
                    InitialPosition = npc.Center;
                    float RandomOffset = /*MathHelper.ToRadians(Main.rand.Next(-3, 3)) * 20*/MathHelper.PiOver4 * ParentNPC.direction;
                    npc.netUpdate = true;

                    Projectile.NewProjectile(npc.Center, (npc.DirectionTo(player.Center).ToRotation() + RandomOffset).ToRotationVector2(), ModContent.ProjectileType<ForbiddenBeam>(), 60, 6f, Main.myPlayer, player.whoAmI);
                }

                if (AttackCounter++ > 360 && AttackCounter <= 480)
                {
                    npc.Center = Vector2.Lerp(InitialPosition, IdlePosition, Utils.Clamp((AttackCounter - 360), 0, 120) / 120f);

                    if (AttackCounter == 480)
                    {
                        AttackCounter = 0;
                        ExecuteAttack = false;
                    }
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            Vector2 scale = new Vector2(3 * 1.5f, 3 * 1f);
            DrawData drawData = new DrawData(ModContent.GetTexture("Terraria/Misc/Perlin"),
                npc.Center - Main.screenPosition + npc.Size * scale * 0.5f,
                new Rectangle(0, 0, npc.width, npc.height),
                Color.Yellow,
                npc.rotation,
                npc.Size,
                scale,
                SpriteEffects.None, 0);

            GameShaders.Misc["ForceField"].UseColor(Color.Yellow);
            GameShaders.Misc["ForceField"].Apply(drawData);

            drawData.Draw(spriteBatch);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }

    public class BeamMinion : DharuudMinion
    {
        private bool FiringBeam = false;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.Meteor3;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forbidden Artifact");
        }

        public override void SetDefaults()
        {
            npc.width = npc.height = 24;
            npc.aiStyle = -1;
            npc.lifeMax = 100;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.friendly = false;
        }

        public override void AI()
        {
            base.AI();

            if (RunOnce)
            {
                ParentNPC = Main.npc[(int)npc.ai[0]];
                RunOnce = false;
            }

            Vector2 IdlePosition = ParentNPC.Center + new Vector2(100, 0).RotatedBy(MathHelper.ToRadians(npc.ai[2] += 2f));
            if (!Grappled && !ExecuteAttack)
            {
                npc.Center = IdlePosition;
            }

            // Positions itself near the player, before glowing brightly
            // Afterwards, creates a giant wall of light and while moving slowly towards the player
            if (ExecuteAttack)
            {
                npc.TargetClosest(true);
                Player player = Main.player[npc.target];

                if (AttackCounter <= 60f)
                {
                    if (AttackCounter == 0)
                    {
                        InitialPosition = npc.Center;
                    }

                    npc.Center = Vector2.Lerp(InitialPosition, player.Center - new Vector2(0, 250), AttackCounter / 60f);
                }

                if (AttackCounter >= 120 && AttackCounter <= 240)
                {
                    if (AttackCounter == 120)
                    {
                        FiringBeam = true;
                        Projectile.NewProjectile(npc.Center + Vector2.UnitY * -750, Vector2.UnitY, ModContent.ProjectileType<GiantBeam>(), 60, 6f, Main.myPlayer, npc.whoAmI);
                    }

                    InitialPosition = npc.Center;
                    npc.velocity.X = player.Center.X > npc.Center.X ? 2 : -2;
                }

                if (AttackCounter++ > 240 && AttackCounter <= 360)
                {
                    npc.velocity = Vector2.Zero;

                    FiringBeam = false;
                    npc.Center = Vector2.Lerp(InitialPosition, IdlePosition, Utils.Clamp((AttackCounter - 240), 0, 120) / 120f);

                    if (AttackCounter == 360)
                    {
                        AttackCounter = 0;
                        ExecuteAttack = false;
                    }
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            Vector2 scale = new Vector2(3 * 1.5f, 3 * 1f);
            DrawData drawData = new DrawData(ModContent.GetTexture("Terraria/Misc/Perlin"),
                npc.Center - Main.screenPosition + npc.Size * scale * 0.5f,
                new Rectangle(0, 0, npc.width, npc.height),
                Color.Yellow,
                npc.rotation,
                npc.Size,
                scale,
                SpriteEffects.None, 0);

            GameShaders.Misc["ForceField"].UseColor(Color.Yellow);
            GameShaders.Misc["ForceField"].Apply(drawData);

            drawData.Draw(spriteBatch);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            /*if (FiringBeam)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                float mult = (0.55f + (float)Math.Sin(Main.GlobalTime) * 0.1f);
                float BeamScale = npc.scale * 4 * mult;
                Texture2D texture = ModContent.GetTexture("OvermorrowMod/Textures/PulseCircle");
                spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, Color.Yellow * 0.5f, 0, new Vector2(texture.Width, texture.Height) / 2, BeamScale * 0.5f, SpriteEffects.None, 0f);

                texture = ModContent.GetTexture("OvermorrowMod/Textures/Sunlight");
                spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, Color.White, 0, new Vector2(texture.Width, texture.Height) / 2, BeamScale * 0.25f, SpriteEffects.None, 0f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }*/
        }
    }

    public class BlasterMinion : DharuudMinion
    {
        private Vector2 ShootingPosition;
        private Vector2 RecoilPosition;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.Meteor3;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forbidden Artifact");
        }

        public override void SetDefaults()
        {
            npc.width = npc.height = 24;
            npc.aiStyle = -1;
            npc.lifeMax = 100;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.friendly = false;
        }

        public override void AI()
        {
            base.AI();

            if (RunOnce)
            {
                ParentNPC = Main.npc[(int)npc.ai[0]];
                RunOnce = false;
            }

            Vector2 IdlePosition = ParentNPC.Center + new Vector2(100, 0).RotatedBy(MathHelper.ToRadians(npc.ai[2] += 2f));
            if (!Grappled && !ExecuteAttack)
            {
                npc.Center = IdlePosition;
            }

            // Shoot three random invisible projectiles, when they collide with a tile they become visible
            // The NPC then fires beams at the projectiles, creating a small circle of light
            if (ExecuteAttack)
            {
                npc.TargetClosest(true);
                Player player = Main.player[npc.target];

                if (GlobalCounter <= 60f)
                {
                    if (GlobalCounter == 0)
                    {
                        InitialPosition = npc.Center;
                        ShootingPosition = new Vector2(Main.rand.Next(200, 250) * ParentNPC.direction, Main.rand.Next(-100, -50));
                        npc.netUpdate = true;

                        npc.velocity = Vector2.Zero;
                    }

                    npc.Center = Vector2.Lerp(InitialPosition, ParentNPC.Center - new Vector2(0, 250), GlobalCounter / 60f);
                }

                // Place crosshairs
                if (GlobalCounter == 60)
                {
                    InitialPosition = npc.Center;
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 Velocity = Vector2.Normalize(npc.DirectionTo(player.Center).RotatedByRandom(MathHelper.PiOver4));
                        Projectile.NewProjectile(npc.Center, Velocity, ModContent.ProjectileType<Crosshair>(), 60, 6f, Main.myPlayer);
                    }
                }

                // Fire at the cross hairs
                if (GlobalCounter++ >= 120 && GlobalCounter <= 190)
                {
                    if (AttackCounter++ == 20)
                    {
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            Projectile projectile = Main.projectile[i];
                            if (projectile.active && projectile.type == ModContent.ProjectileType<Crosshair>())
                            {
                                for (int ii = 0; ii < Main.maxPlayers; ii++)
                                {
                                    if (npc.Distance(Main.player[ii].Center) < 1000)
                                    {
                                        Main.player[ii].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 5;
                                    }
                                }

                                Vector2 ShootPosition = npc.DirectionTo(projectile.Center).ToRotation().ToRotationVector2();
                                Projectile.NewProjectile(npc.Center, npc.DirectionTo(projectile.Center).ToRotation().ToRotationVector2(), ModContent.ProjectileType<ForbiddenBurst>(), 60, 6f, Main.myPlayer, projectile.whoAmI);
                                npc.velocity = -Vector2.Normalize(ShootPosition) * 4;
                                AttackCounter = 0;

                                projectile.Kill();
                                break;
                            }
                        }
                    }

                    if (AttackCounter >= 10)
                    {
                        if (AttackCounter == 10)
                        {
                            npc.velocity = Vector2.Zero;
                            RecoilPosition = npc.Center;
                        }

                        npc.Center = Vector2.Lerp(RecoilPosition, InitialPosition, (AttackCounter - 10) / 10f);
                    }
                }


                if (GlobalCounter > 190 && GlobalCounter <= 310)
                {
                    npc.Center = Vector2.Lerp(InitialPosition, IdlePosition, Utils.Clamp(GlobalCounter - 190, 0, 120) / 120f);

                    if (GlobalCounter == 310)
                    {
                        GlobalCounter = 0;
                        AttackCounter = 0;
                        ExecuteAttack = false;
                    }
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            Vector2 scale = new Vector2(3 * 1.5f, 3 * 1f);
            DrawData drawData = new DrawData(ModContent.GetTexture("Terraria/Misc/Perlin"),
                npc.Center - Main.screenPosition + npc.Size * scale * 0.5f,
                new Rectangle(0, 0, npc.width, npc.height),
                Color.Yellow,
                npc.rotation,
                npc.Size,
                scale,
                SpriteEffects.None, 0);

            GameShaders.Misc["ForceField"].UseColor(Color.Yellow);
            GameShaders.Misc["ForceField"].Apply(drawData);

            drawData.Draw(spriteBatch);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}