using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Items.Consumable.Boss.TreeRune;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.IorichBow
{
    public class IorichBowHoldout : ModProjectile
    {
        public override string Texture => AssetDirectory.Ranged + "IorichBow/IorichBow";
        public override bool CanDamage() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iorich Bow Holdout");
            ProjectileID.Sets.NeedsUUID[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 72;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public ref float EnergySpawnCounter => ref projectile.ai[0];

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            Vector2 RotationPoint = player.RotatedRelativePoint(player.MountedCenter, true);

            projectile.Center = RotationPoint;
            projectile.rotation = Main.MouseWorld.X < player.Center.X ? projectile.velocity.ToRotation() + MathHelper.Pi : projectile.velocity.ToRotation();
            projectile.spriteDirection = Main.MouseWorld.X < player.Center.X ? -1 : 1;

            player.ChangeDir(projectile.direction);
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            if (projectile.owner == Main.myPlayer)
            {
                Vector2 AimDirection = Vector2.Normalize(Main.MouseWorld - RotationPoint);
                if (AimDirection.HasNaNs()) AimDirection = -Vector2.UnitY;

                AimDirection = Vector2.Normalize(Vector2.Lerp(Vector2.Normalize(projectile.velocity), AimDirection, 0.75f));

                if (AimDirection != projectile.velocity) projectile.netUpdate = true;
                projectile.velocity = AimDirection;

                if (player.channel && !player.noItems && !player.CCed && player.GetModPlayer<OvermorrowModPlayer>().BowEnergyCount < 6)
                {
                    if (projectile.ai[1]++ % 20 == 0 && EnergySpawnCounter < 6)
                    {
                        float RandomRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
                        Vector2 RandomPosition = player.Center + new Vector2(200, 0).RotatedBy(-RandomRotation);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(RandomPosition, Vector2.Zero, ModContent.ProjectileType<IorichBowEnergy>(), 0, 0f, Main.myPlayer, player.whoAmI);
                        }

                        EnergySpawnCounter++;
                    }

                    if (EnergySpawnCounter == 6)
                    {
                        projectile.ai[1] = 0;
                    }
                }
                else
                {
                    Vector2 ProjectileVelocity = Vector2.Normalize(projectile.velocity);
                    if (ProjectileVelocity.HasNaNs())
                    {
                        ProjectileVelocity = -Vector2.UnitY;
                    }

                    if (player.GetModPlayer<OvermorrowModPlayer>().BowEnergyCount == 6)
                    {
                        if (projectile.ai[1] == 0)
                        {
                            Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<TreeRune_Pulse>(), 0, 0f, Main.myPlayer);

                            player.statLife += 5;
                            player.HealEffect(5);
                        }

                        if (projectile.ai[1] % 5 == 0)
                        {
                            Main.PlaySound(SoundID.DD2_PhantomPhoenixShot, projectile.Center);
                            Projectile.NewProjectile(projectile.Center, ProjectileVelocity * 4, ModContent.ProjectileType<IorichBowArrow>(), projectile.damage, projectile.knockBack, projectile.owner);
                        }
                    }
                    else
                    {
                        if (projectile.ai[1] == 0)
                        {
                            Projectile.NewProjectile(projectile.Center, ProjectileVelocity * 4, player.HeldItem.ammo, projectile.damage, projectile.knockBack, projectile.owner);
                        }
                    }

                    projectile.netUpdate = true;

                    if (projectile.ai[1]++ > 15)
                    {
                        player.GetModPlayer<OvermorrowModPlayer>().BowEnergyCount = 0;
                        projectile.Kill();
                    }
                }
            }

            projectile.timeLeft = 5;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            Vector2 position = (projectile.Center + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition).Floor();
            int offset = frameHeight * projectile.frame;
            SpriteEffects effects = Main.player[projectile.owner].direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;


            spriteBatch.Draw(texture, position, new Rectangle?(new Rectangle(0, offset, texture.Width, frameHeight)), Color.White, projectile.rotation, new Vector2(texture.Width / 2f, frameHeight / 2f), projectile.scale, effects, 0f);
            return false;
        }
    }
}