using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.NPCs;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.Buffs;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss
{
    public class DharuudMinion : PullableNPC
    {
        public Projectile RotationCenter = null;
        public Player ParentPlayer = null;

        protected Vector2 InitialPosition;
        protected Vector2 IdlePosition;

        public bool ExecuteAttack = false;
        protected bool RunOnce = true;
        public bool ReturnIdle = false;

        public bool IsDisabled = false;
        public bool PickedUp = false;
        public bool FiredArtifact = false;
        public Vector2 ShootPosition;

        public ref float ParentID => ref NPC.ai[0];
        public ref float GlobalCounter => ref NPC.ai[1];
        public ref float RotationCounter => ref NPC.ai[2];
        public ref float AttackCounter => ref NPC.ai[3];

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void AI()
        {
            if (RunOnce)
            {
                //RotationCenter = Main.npc[(int)npc.ai[0]];
                RotationCenter = Main.projectile[(int)NPC.ai[0]];
                RunOnce = false;
            }

            if (!RotationCenter.active)
            {
                NPC.active = false;
            }

            IdlePosition = RotationCenter.Center + new Vector2(60, 0).RotatedBy(MathHelper.ToRadians(RotationCounter += 2f));

            // Code to rotate around the boss and allow grappling
            if (!IsDisabled)
            {
                NPC.dontTakeDamage = false;

                // Run this before the grapple projetile check in the parent class lol
                if (GrappleProjectile != null)
                {
                    if (!GrappleProjectile.active)
                    {
                        ReturnIdle = true;
                        InitialPosition = NPC.Center;
                    }
                }

                // Grappling handling
                base.AI();

                if (!Grappled && !ExecuteAttack && !ReturnIdle)
                {
                    NPC.Center = IdlePosition;
                }

                if (ReturnIdle)
                {
                    CanBeGrappled = false;
                    NPC.Center = Vector2.Lerp(InitialPosition, IdlePosition, Utils.Clamp(AttackCounter++, 0, 180) / 180f);

                    if (AttackCounter == 180)
                    {
                        AttackCounter = 0;
                        ReturnIdle = false;
                        CanBeGrappled = true;
                    }
                }
            }
            else // Artifact is not orbiting the boss
            {
                // Collision detection
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];
                    if (player.active && !PickedUp && NPC.Hitbox.Intersects(player.Hitbox) && !player.HasBuff(ModContent.BuffType<Steal>()))
                    {
                        ParentPlayer = player;
                        //player.AddBuff(ModContent.BuffType<Steal>(), 540);
                        player.AddBuff(ModContent.BuffType<Steal>(), 1200);


                        NPC.noTileCollide = true;
                        PickedUp = true;

                        Rectangle rectangle = new Rectangle((int)player.Center.X, player.Hitbox.Top - 5, 2, 2);
                        CombatText.NewText(rectangle, Color.Yellow, "Picked up Artifact!", true);
                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), player.Center, Vector2.Zero, ModContent.ProjectileType<PlayerCrosshair>(), 0, 0f, player.whoAmI, NPC.whoAmI);
                    }
                }

                if (!FiredArtifact) // Don't run the idle code if the player has picked it up and fired
                {
                    if (PickedUp) // The object has been picked up and is orbiting the player
                    {
                        NPC.velocity = Vector2.Zero;

                        NPC.Center = ParentPlayer.Center + new Vector2(100, 0).RotatedBy(MathHelper.ToRadians(RotationCounter));

                        if (AttackCounter++ == 600)
                        {
                            AttackCounter = 0;
                            InitialPosition = NPC.Center;
                            IsDisabled = false;
                            ReturnIdle = true;
                            PickedUp = false;
                            ParentPlayer = null;

                            NPC.noTileCollide = true;
                        }
                    }
                    else // The object is on the ground and waiting to be picked up, returns after 5 seconds
                    {
                        if (AttackCounter < 60)
                        {
                            NPC.velocity = -Vector2.UnitY;
                        }
                        else
                        {
                            NPC.velocity = Vector2.UnitY * 2;
                        }

                        if (AttackCounter++ == 300)
                        {
                            NPC.velocity = Vector2.Zero;

                            AttackCounter = 0;
                            InitialPosition = NPC.Center;
                            IsDisabled = false;
                            ReturnIdle = true;

                            NPC.noTileCollide = true;
                        }
                    }
                }
            }
        }

        public override bool CheckDead()
        {
            if (!IsDisabled)
            {
                Main.NewText("I AM DEAD AMEN");
                InitialPosition = NPC.Center;

                NPC.noTileCollide = false;
                IsDisabled = true;
            }

            AttackCounter = 0;

            NPC.life = NPC.lifeMax;
            NPC.dontTakeDamage = true;
            NPC.netUpdate = true;

            return false;
        }

        public override bool? CanBeHitByItem(Player player, Item item)
        {
            return ReturnIdle;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            return ReturnIdle && projectile.friendly;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (IsDisabled && !PickedUp)
            {
                Texture2D texture = TextureAssets.Npc[NPC.type].Value;
                Color color = new Color(186, 99, 45);
                float mult = (0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2) * 0.1f);
                float scale = NPC.scale * 2.5f * mult;

                spriteBatch.Draw(texture, NPC.Center - screenPos, null, color, NPC.rotation, new Vector2(texture.Width, texture.Height) / 2, scale, SpriteEffects.None, 0f);

            }

            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }

    public class LaserMinion : DharuudMinion
    {
        public override string Texture => AssetDirectory.Boss + "SandstormBoss/Hilt";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forbidden Laser Artifact");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.width = NPC.height = 24;
            NPC.lifeMax = 100;
            NPC.noTileCollide = true;
        }

        public override void AI()
        {
            base.AI();

            if (AttackCounter >= 60 && AttackCounter <= 90)
            {
                if (Main.rand.NextBool(3))
                {
                    for (int _ = 0; _ < 3; _++)
                    {
                        Vector2 RandomPosition = NPC.Center + new Vector2(Main.rand.Next(125, 200), 0).RotatedByRandom(MathHelper.TwoPi);
                        Vector2 Direction = Vector2.Normalize(NPC.Center - RandomPosition);

                        int DustSpeed = 10;

                        Particle.CreateParticle(Particle.ParticleType<Orb>(), RandomPosition, Direction * DustSpeed, Color.Orange, 1, Main.rand.NextFloat(0.25f, 0.4f), 0, 25);
                    }
                }
            }

            // Positions itself above the boss, before firing a beam in a wide arc
            if (ExecuteAttack)
            {
                CanBeGrappled = false;
                NPC.TargetClosest(true);
                Player player = Main.player[NPC.target];

                if (AttackCounter <= 60f)
                {
                    if (AttackCounter == 0)
                    {
                        InitialPosition = NPC.Center;
                    }

                    NPC.Center = Vector2.Lerp(InitialPosition, RotationCenter.Center - new Vector2(0, 250), AttackCounter / 60f);
                }

                if (AttackCounter == 120)
                {
                    InitialPosition = NPC.Center;
                    float RandomOffset = /*MathHelper.ToRadians(Main.rand.Next(-3, 3)) * 20*/MathHelper.PiOver4 * RotationCenter.direction;
                    NPC.netUpdate = true;

                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, (NPC.DirectionTo(player.Center).ToRotation() + RandomOffset).ToRotationVector2(), ModContent.ProjectileType<ForbiddenBeam>(), 60, 6f, Main.myPlayer, player.whoAmI);
                }

                if (AttackCounter++ > 360 && AttackCounter <= 480)
                {
                    NPC.Center = Vector2.Lerp(InitialPosition, IdlePosition, Utils.Clamp((AttackCounter - 360), 0, 120) / 120f);

                    if (AttackCounter == 480)
                    {
                        AttackCounter = 0;
                        ExecuteAttack = false;
                        CanBeGrappled = true;
                    }
                }
            }

            if (FiredArtifact)
            {
                CanBeGrappled = false;

                if (AttackCounter <= 60f)
                {
                    if (AttackCounter == 0)
                    {
                        InitialPosition = NPC.Center;
                    }

                    NPC.Center = Vector2.Lerp(InitialPosition, ParentPlayer.Center - new Vector2(0, 250), AttackCounter / 60f);
                }

                if (AttackCounter == 120)
                {

                    InitialPosition = NPC.Center;
                    float RandomOffset = /*MathHelper.ToRadians(Main.rand.Next(-3, 3)) * 20*/MathHelper.ToRadians(20) * ParentPlayer.direction;
                    NPC.netUpdate = true;

                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, (NPC.DirectionTo(ShootPosition).ToRotation() + RandomOffset).ToRotationVector2(), ModContent.ProjectileType<ForbiddenBeamFriendly>(), 60, 6f, Main.myPlayer, ShootPosition.X, ShootPosition.Y);
                }

                if (AttackCounter++ > 360 && AttackCounter <= 480)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile projectile = Main.projectile[i];
                        if (projectile.active && projectile.type == ModContent.ProjectileType<PlayerCrosshair>() && projectile.owner == ParentPlayer.whoAmI)
                        {
                            projectile.Kill();
                        }
                    }

                    NPC.Center = Vector2.Lerp(InitialPosition, IdlePosition, Utils.Clamp((AttackCounter - 360), 0, 120) / 120f);

                    if (AttackCounter == 480)
                    {
                        AttackCounter = 0;
                        CanBeGrappled = true;
                        FiredArtifact = false;
                        PickedUp = false;
                        ParentPlayer = null;
                        ReturnIdle = false;
                        IsDisabled = false;
                    }
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            if (!IsDisabled)
            {
                Vector2 scale = new Vector2(3 * 1.5f, 3 * 1f);

                Color color = Color.Yellow;
                if (ReturnIdle)
                {
                    color = Color.Lerp(Color.Orange, Color.Yellow, (float)Math.Sin(NPC.localAI[0]++ / 15f));
                }

                DrawData drawData = new DrawData(ModContent.Request<Texture2D>("Terraria/Misc/Perlin").Value,
                    NPC.Center - screenPos + NPC.Size * scale * 0.5f,
                    new Rectangle(0, 0, NPC.width, NPC.height),
                    Color.Yellow,
                    NPC.rotation,
                    NPC.Size,
                    scale,
                    SpriteEffects.None, 0);

                GameShaders.Misc["ForceField"].UseColor(color);
                GameShaders.Misc["ForceField"].Apply(drawData);

                drawData.Draw(spriteBatch);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }

    public class BeamMinion : DharuudMinion
    {
        private bool FiringBeam = false;
        //public override string Texture => "Terraria/Projectile_" + ProjectileID.Meteor3;
        public override string Texture => AssetDirectory.Boss + "SandstormBoss/Gem";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forbidden Beam Artifact");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.width = NPC.height = 24;
            NPC.lifeMax = 100;
            NPC.noTileCollide = true;
        }

        public override void AI()
        {
            base.AI();

            if (AttackCounter >= 60 && AttackCounter <= 90)
            {
                if (Main.rand.NextBool(3))
                {
                    for (int _ = 0; _ < 3; _++)
                    {
                        Vector2 RandomPosition = NPC.Center + new Vector2(Main.rand.Next(125, 200), 0).RotatedByRandom(MathHelper.TwoPi);
                        Vector2 Direction = Vector2.Normalize(NPC.Center - RandomPosition);

                        int DustSpeed = 10;

                        Particle.CreateParticle(Particle.ParticleType<Orb>(), RandomPosition, Direction * DustSpeed, Color.Orange, 1, Main.rand.NextFloat(0.25f, 0.4f), 0, 25);
                    }
                }
            }

            // Positions itself near the player, before glowing brightly
            // Afterwards, creates a giant wall of light and while moving slowly towards the player
            if (ExecuteAttack)
            {
                CanBeGrappled = false;
                NPC.TargetClosest(true);
                Player player = Main.player[NPC.target];

                if (AttackCounter <= 60f)
                {
                    if (AttackCounter == 0)
                    {
                        InitialPosition = NPC.Center;
                    }

                    NPC.Center = Vector2.Lerp(InitialPosition, player.Center - new Vector2(0, 250), AttackCounter / 60f);
                }

                if (AttackCounter >= 120 && AttackCounter <= 240)
                {
                    if (AttackCounter == 120)
                    {
                        FiringBeam = true;
                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center + Vector2.UnitY * -750, Vector2.UnitY, ModContent.ProjectileType<GiantBeam>(), 60, 6f, Main.myPlayer, NPC.whoAmI);
                    }

                    InitialPosition = NPC.Center;
                    NPC.velocity.X = player.Center.X > NPC.Center.X ? 2 : -2;
                }

                if (AttackCounter++ > 240 && AttackCounter <= 360)
                {
                    NPC.velocity = Vector2.Zero;

                    FiringBeam = false;
                    NPC.Center = Vector2.Lerp(InitialPosition, IdlePosition, Utils.Clamp((AttackCounter - 240), 0, 120) / 120f);

                    if (AttackCounter == 360)
                    {
                        AttackCounter = 0;
                        ExecuteAttack = false;
                        CanBeGrappled = true;
                    }
                }
            }

            if (FiredArtifact)
            {
                CanBeGrappled = false;

                if (AttackCounter <= 60f)
                {
                    if (AttackCounter == 0)
                    {
                        InitialPosition = NPC.Center;
                    }

                    NPC.Center = Vector2.Lerp(InitialPosition, ShootPosition - new Vector2(0, 250), AttackCounter / 60f);
                }

                if (AttackCounter >= 120 && AttackCounter <= 240)
                {
                    if (AttackCounter == 120)
                    {
                        FiringBeam = true;
                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center + Vector2.UnitY * -750, Vector2.UnitY, ModContent.ProjectileType<GiantBeam>(), 60, 6f, Main.myPlayer, NPC.whoAmI, 1);
                    }

                    InitialPosition = NPC.Center;
                    NPC.velocity.X = ShootPosition.X > NPC.Center.X ? 2 : -2;
                }

                if (AttackCounter++ > 240 && AttackCounter <= 360)
                {
                    NPC.velocity = Vector2.Zero;

                    FiringBeam = false;
                    NPC.Center = Vector2.Lerp(InitialPosition, IdlePosition, Utils.Clamp((AttackCounter - 240), 0, 120) / 120f);

                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile projectile = Main.projectile[i];
                        if (projectile.active && projectile.type == ModContent.ProjectileType<PlayerCrosshair>() && projectile.owner == ParentPlayer.whoAmI)
                        {
                            projectile.Kill();
                        }
                    }

                    if (AttackCounter == 360)
                    {
                        AttackCounter = 0;
                        CanBeGrappled = true;
                        FiredArtifact = false;
                        PickedUp = false;
                        ParentPlayer = null;
                        ReturnIdle = false;
                        IsDisabled = false;
                    }
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            if (!IsDisabled)
            {
                Vector2 scale = new Vector2(3 * 1.5f, 3 * 1f);

                Color color = Color.Yellow;
                if (ReturnIdle)
                {
                    color = Color.Lerp(Color.Orange, Color.Yellow, (float)Math.Sin(NPC.localAI[0]++ / 15f));
                }

                DrawData drawData = new DrawData(ModContent.Request<Texture2D>("Terraria/Misc/Perlin").Value,
                    NPC.Center - screenPos + NPC.Size * scale * 0.5f,
                    new Rectangle(0, 0, NPC.width, NPC.height),
                    Color.Yellow,
                    NPC.rotation,
                    NPC.Size,
                    scale,
                    SpriteEffects.None, 0);

                GameShaders.Misc["ForceField"].UseColor(color);
                GameShaders.Misc["ForceField"].Apply(drawData);

                drawData.Draw(spriteBatch);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

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
        private Vector2 RecoilPosition;
        public override string Texture => AssetDirectory.Boss + "SandstormBoss/Mask";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forbidden Blaster Artifact");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.width = NPC.height = 24;
            NPC.lifeMax = 100;
            NPC.noTileCollide = true;
        }

        public override void AI()
        {
            base.AI();

            if (GlobalCounter >= 60 && GlobalCounter <= 90)
            {
                if (Main.rand.NextBool(3))
                {
                    for (int _ = 0; _ < 3; _++)
                    {
                        Vector2 RandomPosition = NPC.Center + new Vector2(Main.rand.Next(125, 200), 0).RotatedByRandom(MathHelper.TwoPi);
                        Vector2 Direction = Vector2.Normalize(NPC.Center - RandomPosition);

                        int DustSpeed = 10;

                        Particle.CreateParticle(Particle.ParticleType<Orb>(), RandomPosition, Direction * DustSpeed, Color.Orange, 1, Main.rand.NextFloat(0.25f, 0.4f), 0, 25);
                    }
                }
            }

            // Shoot three random invisible projectiles, when they collide with a tile they become visible
            // The NPC then fires beams at the projectiles, creating a small circle of light
            if (ExecuteAttack)
            {
                CanBeGrappled = false;

                NPC.TargetClosest(true);
                Player player = Main.player[NPC.target];

                if (GlobalCounter <= 60f)
                {
                    if (GlobalCounter == 0)
                    {
                        InitialPosition = NPC.Center;
                        NPC.netUpdate = true;

                        NPC.velocity = Vector2.Zero;
                    }

                    // Place crosshairs
                    if (GlobalCounter == 60)
                    {
                        InitialPosition = NPC.Center;
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 Velocity = Vector2.Normalize(NPC.DirectionTo(player.Center).RotatedByRandom(MathHelper.PiOver4));
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Velocity, ModContent.ProjectileType<Crosshair>(), 60, 6f, Main.myPlayer, 0, 1200);
                        }
                    }

                    NPC.Center = Vector2.Lerp(InitialPosition, RotationCenter.Center - new Vector2(0, 250), GlobalCounter / 60f);
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
                                    if (projectile.Distance(Main.player[ii].Center) < 1000)
                                    {
                                        //Main.player[ii].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 5;
                                    }
                                }

                                Vector2 ShootPosition = NPC.DirectionTo(projectile.Center).ToRotation().ToRotationVector2();
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, NPC.DirectionTo(projectile.Center).ToRotation().ToRotationVector2(), ModContent.ProjectileType<ForbiddenBurst>(), 60, 6f, Main.myPlayer, projectile.whoAmI);

                                if (Main.expertMode)
                                {
                                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), projectile.Center, Vector2.Zero, ModContent.ProjectileType<HeatCircle>(), 60, 0f, Main.myPlayer);
                                }

                                NPC.velocity = -Vector2.Normalize(ShootPosition) * 4;
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
                            NPC.velocity = Vector2.Zero;
                            RecoilPosition = NPC.Center;
                        }

                        NPC.Center = Vector2.Lerp(RecoilPosition, InitialPosition, (AttackCounter - 10) / 10f);
                    }
                }


                if (GlobalCounter > 190 && GlobalCounter <= 310)
                {
                    NPC.Center = Vector2.Lerp(InitialPosition, IdlePosition, Utils.Clamp(GlobalCounter - 190, 0, 120) / 120f);

                    if (GlobalCounter == 310)
                    {
                        GlobalCounter = 0;
                        AttackCounter = 0;
                        ExecuteAttack = false;

                        CanBeGrappled = true;
                    }
                }
            }

            if (FiredArtifact)
            {
                CanBeGrappled = false;

                if (GlobalCounter <= 60f)
                {
                    if (GlobalCounter == 0)
                    {
                        InitialPosition = NPC.Center;
                        NPC.netUpdate = true;

                        NPC.velocity = Vector2.Zero;
                    }

                    // Place crosshairs
                    if (GlobalCounter == 60)
                    {
                        InitialPosition = NPC.Center;
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 Velocity = Vector2.Normalize(NPC.DirectionTo(ShootPosition).RotatedByRandom(MathHelper.ToRadians(15)));
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Velocity, ModContent.ProjectileType<Crosshair>(), 60, 6f, Main.myPlayer, 0, 600);
                        }
                    }

                    NPC.Center = Vector2.Lerp(InitialPosition, ParentPlayer.Center - new Vector2(0, 250), GlobalCounter / 60f);
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
                                    if (projectile.Distance(Main.player[ii].Center) < 1000)
                                    {
                                        //Main.player[ii].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 5;
                                    }
                                }

                                Vector2 ShootPosition = NPC.DirectionTo(projectile.Center).ToRotation().ToRotationVector2();
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, NPC.DirectionTo(projectile.Center).ToRotation().ToRotationVector2(), ModContent.ProjectileType<ForbiddenBurst>(), 60, 6f, Main.myPlayer, projectile.whoAmI, 1);

                                NPC.velocity = -Vector2.Normalize(ShootPosition) * 4;
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
                            NPC.velocity = Vector2.Zero;
                            RecoilPosition = NPC.Center;
                        }

                        NPC.Center = Vector2.Lerp(RecoilPosition, InitialPosition, (AttackCounter - 10) / 10f);
                    }
                }


                if (GlobalCounter > 190 && GlobalCounter <= 310)
                {
                    NPC.Center = Vector2.Lerp(InitialPosition, IdlePosition, Utils.Clamp(GlobalCounter - 190, 0, 120) / 120f);

                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile projectile = Main.projectile[i];
                        if (projectile.active && projectile.type == ModContent.ProjectileType<PlayerCrosshair>() && projectile.owner == ParentPlayer.whoAmI)
                        {
                            projectile.Kill();
                        }
                    }

                    if (GlobalCounter == 310)
                    {
                        GlobalCounter = 0;
                        AttackCounter = 0;
                        ExecuteAttack = false;

                        CanBeGrappled = true;
                        FiredArtifact = false;
                        PickedUp = false;
                        ParentPlayer = null;
                        ReturnIdle = false;
                        IsDisabled = false;
                    }
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            if (!IsDisabled)
            {
                Vector2 scale = new Vector2(3 * 1.5f, 3 * 1f);

                Color color = Color.Yellow;
                if (ReturnIdle)
                {
                    color = Color.Lerp(Color.Orange, Color.Yellow, (float)Math.Sin(NPC.localAI[0]++ / 15f));
                }

                DrawData drawData = new DrawData(ModContent.Request<Texture2D>("Terraria/Misc/Perlin").Value,
                    NPC.Center - screenPos + NPC.Size * scale * 0.5f,
                    new Rectangle(0, 0, NPC.width, NPC.height),
                    Color.Yellow,
                    NPC.rotation,
                    NPC.Size,
                    scale,
                    SpriteEffects.None, 0);

                GameShaders.Misc["ForceField"].UseColor(color);
                GameShaders.Misc["ForceField"].Apply(drawData);

                drawData.Draw(spriteBatch);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}