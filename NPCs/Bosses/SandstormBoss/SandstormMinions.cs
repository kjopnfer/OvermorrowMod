using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Buffs;
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

        public ref float ParentID => ref npc.ai[0];
        public ref float GlobalCounter => ref npc.ai[1];
        public ref float RotationCounter => ref npc.ai[2];
        public ref float AttackCounter => ref npc.ai[3];

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void AI()
        {
            if (RunOnce)
            {
                ParentNPC = Main.npc[(int)npc.ai[0]];
                RunOnce = false;
            }

            if (!ParentNPC.active)
            {
                npc.active = false;
            }

            IdlePosition = ParentNPC.Center + new Vector2(100, 0).RotatedBy(MathHelper.ToRadians(RotationCounter += 2f));

            // Code to rotate around the boss and allow grappling
            if (!IsDisabled)
            {
                npc.dontTakeDamage = false;

                // Run this before the grapple projetile check in the parent class lol
                if (GrappleProjectile != null)
                {
                    if (!GrappleProjectile.active)
                    {
                        ReturnIdle = true;
                        InitialPosition = npc.Center;
                    }
                }

                // Grappling handling
                base.AI();

                if (!Grappled && !ExecuteAttack && !ReturnIdle)
                {
                    npc.Center = IdlePosition;
                }

                if (ReturnIdle)
                {
                    CanBeGrappled = false;
                    npc.Center = Vector2.Lerp(InitialPosition, IdlePosition, Utils.Clamp(AttackCounter++, 0, 180) / 180f);

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
                    if (player.active && !PickedUp && npc.Hitbox.Intersects(player.Hitbox) && !player.HasBuff(ModContent.BuffType<Steal>()))
                    {
                        ParentPlayer = player;
                        player.AddBuff(ModContent.BuffType<Steal>(), 540);


                        npc.noTileCollide = true;
                        PickedUp = true;

                        Rectangle rectangle = new Rectangle((int)player.Center.X, player.Hitbox.Top - 5, 2, 2);
                        CombatText.NewText(rectangle, Color.Yellow, "Picked up Artifact!", true);
                        Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<PlayerCrosshair>(), 0, 0f, player.whoAmI, npc.whoAmI);
                    }
                }

                if (!FiredArtifact) // Don't run the idle code if the player has picked it up and fired
                {
                    if (PickedUp) // The object has been picked up and is orbiting the player
                    {
                        npc.velocity = Vector2.Zero;

                        npc.Center = ParentPlayer.Center + new Vector2(100, 0).RotatedBy(MathHelper.ToRadians(RotationCounter));

                        if (AttackCounter++ == 600)
                        {
                            AttackCounter = 0;
                            InitialPosition = npc.Center;
                            IsDisabled = false;
                            ReturnIdle = true;
                            PickedUp = false;
                            ParentPlayer = null;

                            npc.noTileCollide = true;
                        }
                    }
                    else // The object is on the ground and waiting to be picked up, returns after 5 seconds
                    {
                        if (AttackCounter < 60)
                        {
                            npc.velocity = -Vector2.UnitY;
                        }
                        else
                        {
                            npc.velocity = Vector2.UnitY * 2;
                        }

                        if (AttackCounter++ == 300)
                        {
                            npc.velocity = Vector2.Zero;

                            AttackCounter = 0;
                            InitialPosition = npc.Center;
                            IsDisabled = false;
                            ReturnIdle = true;

                            npc.noTileCollide = true;
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
                InitialPosition = npc.Center;

                npc.noTileCollide = false;
                IsDisabled = true;
            }

            AttackCounter = 0;

            npc.life = npc.lifeMax;
            npc.dontTakeDamage = true;
            npc.netUpdate = true;

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

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (IsDisabled && !PickedUp)
            {
                Texture2D texture = Main.npcTexture[npc.type];
                Color color = Color.Yellow;
                float mult = (0.55f + (float)Math.Sin(Main.GlobalTime * 2) * 0.1f);
                float scale = npc.scale * 2.5f * mult;

                spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, color, npc.rotation, new Vector2(texture.Width, texture.Height) / 2, scale, SpriteEffects.None, 0f);

            }

            return base.PreDraw(spriteBatch, drawColor);
        }
    }

    public class LaserMinion : DharuudMinion
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.Meteor3;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forbidden Laser Artifact");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            npc.width = npc.height = 24;
            npc.lifeMax = 100;
            npc.noTileCollide = true;
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
                        Vector2 RandomPosition = npc.Center + new Vector2(Main.rand.Next(125, 200), 0).RotatedByRandom(MathHelper.TwoPi);
                        Vector2 Direction = Vector2.Normalize(npc.Center - RandomPosition);

                        int DustSpeed = 10;

                        Particle.CreateParticle(Particle.ParticleType<Orb>(), RandomPosition, Direction * DustSpeed, Color.Orange, 1, Main.rand.NextFloat(0.25f, 0.4f), 0, 25);
                    }
                }
            }

            // Positions itself above the boss, before firing a beam in a wide arc
            if (ExecuteAttack)
            {
                CanBeGrappled = false;
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
                        InitialPosition = npc.Center;
                    }

                    npc.Center = Vector2.Lerp(InitialPosition, ParentPlayer.Center - new Vector2(0, 250), AttackCounter / 60f);
                }

                if (AttackCounter == 120)
                {

                    InitialPosition = npc.Center;
                    float RandomOffset = /*MathHelper.ToRadians(Main.rand.Next(-3, 3)) * 20*/MathHelper.ToRadians(20) * ParentPlayer.direction;
                    npc.netUpdate = true;

                    Projectile.NewProjectile(npc.Center, (npc.DirectionTo(ShootPosition).ToRotation() + RandomOffset).ToRotationVector2(), ModContent.ProjectileType<ForbiddenBeamFriendly>(), 60, 6f, Main.myPlayer, ShootPosition.X, ShootPosition.Y);
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

                    npc.Center = Vector2.Lerp(InitialPosition, IdlePosition, Utils.Clamp((AttackCounter - 360), 0, 120) / 120f);

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

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            if (!IsDisabled)
            {
                Vector2 scale = new Vector2(3 * 1.5f, 3 * 1f);

                Color color = Color.Yellow;
                if (ReturnIdle)
                {
                    color = Color.Lerp(Color.Orange, Color.Yellow, (float)Math.Sin(npc.localAI[0]++ / 15f));
                }

                DrawData drawData = new DrawData(ModContent.GetTexture("Terraria/Misc/Perlin"),
                    npc.Center - Main.screenPosition + npc.Size * scale * 0.5f,
                    new Rectangle(0, 0, npc.width, npc.height),
                    Color.Yellow,
                    npc.rotation,
                    npc.Size,
                    scale,
                    SpriteEffects.None, 0);

                GameShaders.Misc["ForceField"].UseColor(color);
                GameShaders.Misc["ForceField"].Apply(drawData);

                drawData.Draw(spriteBatch);
            }
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
            DisplayName.SetDefault("Forbidden Beam Artifact");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            npc.width = npc.height = 24;
            npc.lifeMax = 100;
            npc.noTileCollide = true;
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
                        Vector2 RandomPosition = npc.Center + new Vector2(Main.rand.Next(125, 200), 0).RotatedByRandom(MathHelper.TwoPi);
                        Vector2 Direction = Vector2.Normalize(npc.Center - RandomPosition);

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
                        InitialPosition = npc.Center;
                    }

                    npc.Center = Vector2.Lerp(InitialPosition, ShootPosition - new Vector2(0, 250), AttackCounter / 60f);
                }

                if (AttackCounter >= 120 && AttackCounter <= 240)
                {
                    if (AttackCounter == 120)
                    {
                        FiringBeam = true;
                        Projectile.NewProjectile(npc.Center + Vector2.UnitY * -750, Vector2.UnitY, ModContent.ProjectileType<GiantBeam>(), 60, 6f, Main.myPlayer, npc.whoAmI, 1);
                    }

                    InitialPosition = npc.Center;
                    npc.velocity.X = ShootPosition.X > npc.Center.X ? 2 : -2;
                }

                if (AttackCounter++ > 240 && AttackCounter <= 360)
                {
                    npc.velocity = Vector2.Zero;

                    FiringBeam = false;
                    npc.Center = Vector2.Lerp(InitialPosition, IdlePosition, Utils.Clamp((AttackCounter - 240), 0, 120) / 120f);

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

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            if (!IsDisabled)
            {
                Vector2 scale = new Vector2(3 * 1.5f, 3 * 1f);

                Color color = Color.Yellow;
                if (ReturnIdle)
                {
                    color = Color.Lerp(Color.Orange, Color.Yellow, (float)Math.Sin(npc.localAI[0]++ / 15f));
                }

                DrawData drawData = new DrawData(ModContent.GetTexture("Terraria/Misc/Perlin"),
                    npc.Center - Main.screenPosition + npc.Size * scale * 0.5f,
                    new Rectangle(0, 0, npc.width, npc.height),
                    Color.Yellow,
                    npc.rotation,
                    npc.Size,
                    scale,
                    SpriteEffects.None, 0);

                GameShaders.Misc["ForceField"].UseColor(color);
                GameShaders.Misc["ForceField"].Apply(drawData);

                drawData.Draw(spriteBatch);
            }

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
        private Vector2 RecoilPosition;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.Meteor3;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forbidden Blaster Artifact");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            npc.width = npc.height = 24;
            npc.lifeMax = 100;
            npc.noTileCollide = true;
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
                        Vector2 RandomPosition = npc.Center + new Vector2(Main.rand.Next(125, 200), 0).RotatedByRandom(MathHelper.TwoPi);
                        Vector2 Direction = Vector2.Normalize(npc.Center - RandomPosition);

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

                npc.TargetClosest(true);
                Player player = Main.player[npc.target];

                if (GlobalCounter <= 60f)
                {
                    if (GlobalCounter == 0)
                    {
                        InitialPosition = npc.Center;
                        npc.netUpdate = true;

                        npc.velocity = Vector2.Zero;
                    }

                    // Place crosshairs
                    if (GlobalCounter == 60)
                    {
                        InitialPosition = npc.Center;
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 Velocity = Vector2.Normalize(npc.DirectionTo(player.Center).RotatedByRandom(MathHelper.PiOver4));
                            Projectile.NewProjectile(npc.Center, Velocity, ModContent.ProjectileType<Crosshair>(), 60, 6f, Main.myPlayer, 0, 1200);
                        }
                    }

                    npc.Center = Vector2.Lerp(InitialPosition, ParentNPC.Center - new Vector2(0, 250), GlobalCounter / 60f);
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
                                        Main.player[ii].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 5;
                                    }
                                }

                                Vector2 ShootPosition = npc.DirectionTo(projectile.Center).ToRotation().ToRotationVector2();
                                Projectile.NewProjectile(npc.Center, npc.DirectionTo(projectile.Center).ToRotation().ToRotationVector2(), ModContent.ProjectileType<ForbiddenBurst>(), 60, 6f, Main.myPlayer, projectile.whoAmI);

                                if (Main.expertMode)
                                {
                                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<HeatCircle>(), 60, 0f, Main.myPlayer);
                                }

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
                        InitialPosition = npc.Center;
                        npc.netUpdate = true;

                        npc.velocity = Vector2.Zero;
                    }

                    // Place crosshairs
                    if (GlobalCounter == 60)
                    {
                        InitialPosition = npc.Center;
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 Velocity = Vector2.Normalize(npc.DirectionTo(ShootPosition).RotatedByRandom(MathHelper.ToRadians(15)));
                            Projectile.NewProjectile(npc.Center, Velocity, ModContent.ProjectileType<Crosshair>(), 60, 6f, Main.myPlayer, 0, 600);
                        }
                    }

                    npc.Center = Vector2.Lerp(InitialPosition, ParentPlayer.Center - new Vector2(0, 250), GlobalCounter / 60f);
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
                                        Main.player[ii].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 5;
                                    }
                                }

                                Vector2 ShootPosition = npc.DirectionTo(projectile.Center).ToRotation().ToRotationVector2();
                                Projectile.NewProjectile(npc.Center, npc.DirectionTo(projectile.Center).ToRotation().ToRotationVector2(), ModContent.ProjectileType<ForbiddenBurst>(), 60, 6f, Main.myPlayer, projectile.whoAmI, 1);

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

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            if (!IsDisabled)
            {
                Vector2 scale = new Vector2(3 * 1.5f, 3 * 1f);

                Color color = Color.Yellow;
                if (ReturnIdle)
                {
                    color = Color.Lerp(Color.Orange, Color.Yellow, (float)Math.Sin(npc.localAI[0]++ / 15f));
                }

                DrawData drawData = new DrawData(ModContent.GetTexture("Terraria/Misc/Perlin"),
                    npc.Center - Main.screenPosition + npc.Size * scale * 0.5f,
                    new Rectangle(0, 0, npc.width, npc.height),
                    Color.Yellow,
                    npc.rotation,
                    npc.Size,
                    scale,
                    SpriteEffects.None, 0);

                GameShaders.Misc["ForceField"].UseColor(color);
                GameShaders.Misc["ForceField"].Apply(drawData);

                drawData.Draw(spriteBatch);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}