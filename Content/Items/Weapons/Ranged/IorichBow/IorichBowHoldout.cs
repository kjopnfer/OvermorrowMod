using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Items.Consumable.Boss.TreeRune;
using OvermorrowMod.Core;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.IorichBow
{
    public class IorichBowHoldout : ModProjectile
    {
        public override string Texture => AssetDirectory.Ranged + "IorichBow/IorichBow";
        public override bool? CanDamage() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iorich Bow Holdout");
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 72;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public ref float EnergySpawnCounter => ref Projectile.ai[0];

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 RotationPoint = player.RotatedRelativePoint(player.MountedCenter, true);

            Projectile.Center = RotationPoint;
            Projectile.rotation = Main.MouseWorld.X < player.Center.X ? Projectile.velocity.ToRotation() + MathHelper.Pi : Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Main.MouseWorld.X < player.Center.X ? -1 : 1;

            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 AimDirection = Vector2.Normalize(Main.MouseWorld - RotationPoint);
                if (AimDirection.HasNaNs()) AimDirection = -Vector2.UnitY;

                AimDirection = Vector2.Normalize(Vector2.Lerp(Vector2.Normalize(Projectile.velocity), AimDirection, 0.75f));

                if (AimDirection != Projectile.velocity) Projectile.netUpdate = true;
                Projectile.velocity = AimDirection;

                if (player.channel && !player.noItems && !player.CCed && player.GetModPlayer<OvermorrowModPlayer>().BowEnergyCount < 6)
                {
                    if (Projectile.ai[1]++ % 20 == 0 && EnergySpawnCounter < 6)
                    {
                        float RandomRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
                        Vector2 RandomPosition = player.Center + new Vector2(200, 0).RotatedBy(-RandomRotation);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), RandomPosition, Vector2.Zero, ModContent.ProjectileType<IorichBowEnergy>(), 0, 0f, Main.myPlayer, player.whoAmI);
                        }

                        EnergySpawnCounter++;
                    }

                    if (EnergySpawnCounter == 6)
                    {
                        Projectile.ai[1] = 0;
                    }
                }
                else
                {
                    Vector2 ProjectileVelocity = Vector2.Normalize(Projectile.velocity);
                    if (ProjectileVelocity.HasNaNs())
                    {
                        ProjectileVelocity = -Vector2.UnitY;
                    }

                    if (player.GetModPlayer<OvermorrowModPlayer>().BowEnergyCount == 6)
                    {
                        if (Projectile.ai[1] == 0)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TreeRune_Pulse>(), 0, 0f, Main.myPlayer);

                            player.statLife += 5;
                            player.HealEffect(5);
                        }

                        if (Projectile.ai[1] % 5 == 0)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot, Projectile.Center);
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, ProjectileVelocity * 4, ModContent.ProjectileType<IorichBowArrow>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                    }
                    else
                    {
                        if (Projectile.ai[1] == 0)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, ProjectileVelocity * 4, player.HeldItem.ammo, Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                    }

                    Projectile.netUpdate = true;

                    if (Projectile.ai[1]++ > 15)
                    {
                        player.GetModPlayer<OvermorrowModPlayer>().BowEnergyCount = 0;
                        Projectile.Kill();
                    }
                }
            }

            Projectile.timeLeft = 5;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            Vector2 position = (Projectile.Center + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition).Floor();
            int offset = frameHeight * Projectile.frame;
            SpriteEffects effects = Main.player[Projectile.owner].direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;


            Main.EntitySpriteDraw(texture, position, new Rectangle?(new Rectangle(0, offset, texture.Width, frameHeight)), Color.White, Projectile.rotation, new Vector2(texture.Width / 2f, frameHeight / 2f), Projectile.scale, effects, 0);
            return false;
        }
    }
}