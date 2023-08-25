using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.VanillaOverrides.Melee;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee
{
    public class Windrunner : ModDagger<Windrunner_Held, Windrunner_Thrown>
    {
        public override List<DaggerAttack> AttackList => new List<DaggerAttack>() {
            DaggerAttack.Slash, DaggerAttack.Slash, DaggerAttack.Stab, DaggerAttack.Slash,
        };

        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Windrunner_Charge>()] < 1)
            {
                Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<Windrunner_Charge>(), 0, 0f, player.whoAmI);
            }
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 16;
            Item.knockBack = 2f;
            Item.useAnimation = 12;
            Item.useTime = 12;
            Item.width = 32;
            Item.height = 32;
            Item.crit = 20;
            Item.shootSpeed = 2.1f;

            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(0, 0, 0, 10);
        }
    }

    public class Windrunner_Held : HeldDagger
    {
        public override int ParentItem => ModContent.ItemType<Windrunner>();
        public override int ThrownProjectile => ModContent.ProjectileType<Windrunner_Thrown>();

        public override void SetWeaponDrawing(ref Vector2 spritePositionOffset, ref Vector2 dualWieldOffset, ref float rotationOffset, ref float scaleFactor)
        {
            scaleFactor = DualWieldFlag == 1 ? 0.8f : 1f;

            switch (ComboIndex)
            {
                case -1: // The throwing index
                    spritePositionOffset = new Vector2(6, 0).RotatedBy(Projectile.rotation);
                    rotationOffset = MathHelper.ToRadians(-145 * player.direction);
                    break;
                case 0:
                    dualWieldOffset = DualWieldFlag == 1 ? new Vector2(4, -4) : Vector2.Zero;
                    spritePositionOffset = new Vector2(-12 + dualWieldOffset.X, (22 + dualWieldOffset.Y) * player.direction).RotatedBy(Projectile.rotation);
                    rotationOffset = MathHelper.ToRadians(135 * player.direction);
                    break;
                case 1:
                    dualWieldOffset = DualWieldFlag == 1 ? new Vector2(4, -4) : Vector2.Zero;
                    spritePositionOffset = new Vector2(12 + dualWieldOffset.X, (2 + dualWieldOffset.Y) * player.direction).RotatedBy(Projectile.rotation);
                    rotationOffset = MathHelper.ToRadians(45 * player.direction);
                    break;
            }
        }

        Projectile stabProjectile = null;
        public override void OnDaggerStab(float stabCounter)
        {
            float rotation = Main.LocalPlayer.DirectionTo(Main.MouseWorld).ToRotation();
            float stabSpeed = isDualWielding ? 12f : 8f;

            if (stabCounter == 0 && DualWieldFlag != 1 && player.GetModPlayer<MeleePlayer>().WindrunnerCharge >= 5)
            {
                Main.LocalPlayer.velocity = new Vector2(2, 0).RotatedBy(rotation);

                float projectileTime = forwardTime;
                stabProjectile = Projectile.NewProjectileDirect(null, player.Center, Vector2.UnitX.RotatedBy(rotation) * stabSpeed, ModContent.ProjectileType<Windrunner_Stab>(), 0, 0f, Projectile.owner, projectileTime);

                player.GetModPlayer<MeleePlayer>().WindrunnerCharge = 0;
            }
        }

        public override void OnDaggerStabHit()
        {
            if (stabProjectile != null)
            {
                player.velocity *= -0.5f;
                stabProjectile.velocity *= -0.5f;
                stabProjectile.Kill();

                for (int i = 0; i < Main.rand.Next(12, 24); i++)
                {
                    Vector2 positionOffset = new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-5, 5)) * 4;
                    Vector2 RandomVelocity = Vector2.UnitX.RotatedBy(player.velocity.ToRotation()) * -Main.rand.Next(12, 16);
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center + positionOffset, 1, 1, DustID.Smoke, RandomVelocity.X, RandomVelocity.Y, 0, new Color(255, 255, 255), 1f)];
                    dust.noGravity = true;
                }
            }
            else
            {
                if (player.GetModPlayer<MeleePlayer>().WindrunnerCharge < 5) player.GetModPlayer<MeleePlayer>().WindrunnerCharge++;
            }
        }

        public override void OnDaggerSlashHit()
        {
            if (player.GetModPlayer<MeleePlayer>().WindrunnerCharge < 5) player.GetModPlayer<MeleePlayer>().WindrunnerCharge++;
        }

        public override void SetDamageHitbox(Vector2 positionOffset, ref Vector2 hitboxOffset, ref Rectangle hitbox)
        {
            hitbox.Width = 35;
            hitbox.Height = 35;

            switch (ComboIndex)
            {
                case 1:
                    hitboxOffset = positionOffset.RotatedBy(Projectile.rotation);

                    hitbox.X = (int)(player.Center.X - (hitbox.Width / 2f) + hitboxOffset.X);
                    hitbox.Y = (int)(player.Center.Y - (hitbox.Height / 2f) + hitboxOffset.Y);
                    break;
                default:
                    hitboxOffset = new Vector2(25, -5 * player.direction).RotatedBy(Projectile.rotation);
                    hitbox.X = (int)(player.Center.X - (hitbox.Width / 2f) + hitboxOffset.X);
                    hitbox.Y = (int)(player.Center.Y - (hitbox.Height / 2f) + hitboxOffset.Y);
                    break;
            }
        }
    }

    public class Windrunner_Thrown : ThrownDagger
    {
        public override int ParentItem => ModContent.ItemType<Windrunner>();
        public override void OnThrownDaggerHit()
        {
            Player player = Main.player[Projectile.owner];
            if (player.GetModPlayer<MeleePlayer>().WindrunnerCharge < 5) player.GetModPlayer<MeleePlayer>().WindrunnerCharge++;
        }
    }

    public class Windrunner_Stab : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override bool? CanDamage() => false;
        public override void SetDefaults()
        {
            Projectile.width = Main.player[Projectile.owner].width;
            Projectile.height = Main.player[Projectile.owner].height;

            Projectile.friendly = true;
            //Projectile.extraUpdates = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = (int)Projectile.ai[0];
            base.OnSpawn(source);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.armorEffectDrawShadow = true;
            player.armorEffectDrawShadowEOCShield = true;

            if (Main.rand.NextBool())
            {
                for (int i = 0; i < Main.rand.Next(1, 2); i++)
                {
                    Vector2 positionOffset = new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-5, 5)) * 4;
                    float randomScale = Main.rand.NextFloat(0.25f, 0.45f);
                    Vector2 RandomVelocity = -Vector2.Normalize(Projectile.velocity) * 8f;

                    Color color = Color.White;
                    Particle.CreateParticle(Particle.ParticleType<WhiteSpark>(), Projectile.Center + positionOffset, RandomVelocity, color, 1, randomScale);
                }
            }

            for (int i = 0; i < Main.rand.Next(1, 2); i++)
            {
                Vector2 positionOffset = new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-5, 5)) * 4;
                Vector2 RandomVelocity = -Vector2.Normalize(Projectile.velocity) * 12f;
                Dust dust = Main.dust[Dust.NewDust(Projectile.Center + positionOffset, 1, 1, DustID.Smoke, RandomVelocity.X, RandomVelocity.Y, 0, new Color(255, 255, 255), 1f)];
                dust.noGravity = true;
            }

            player.Center = Projectile.Center;
            player.velocity = Projectile.velocity;

            if (Projectile.ai[1]++ > Projectile.ai[0] * 0.5f) Projectile.velocity *= 0.95f;

            Vector2 center = Projectile.Center;
            float stepSpeed = 1f;
            Collision.StepUp(ref center, ref Projectile.velocity, Projectile.width, Projectile.height, ref stepSpeed, ref Projectile.gfxOffY);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            player.velocity = Projectile.velocity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Reload(BlendState.Additive);
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Trails + "Trail4").Value;
            float progress = (Projectile.timeLeft + Projectile.ai[0] / 3f) / Projectile.ai[0];
            float alpha = MathHelper.Lerp(0f, 0.55f, progress);

            if (Projectile.timeLeft > Projectile.ai[0] / 3f) alpha = 0.55f;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor * alpha, Projectile.velocity.ToRotation() + MathHelper.Pi, texture.Size() / 2f, new Vector2(0.5f, 0.35f), SpriteEffects.None, 1);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            return base.PreDraw(ref lightColor);
        }
    }

    public class Windrunner_Charge : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override bool? CanDamage() => false;
        public override void SetDefaults()
        {
            Projectile.width = Main.player[Projectile.owner].width;
            Projectile.height = Main.player[Projectile.owner].height;

            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }

        float rotationSpeed;
        float chargeRotation;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            MeleePlayer modPlayer = player.GetModPlayer<MeleePlayer>();

            if (player.HeldItem.type == ModContent.ItemType<Windrunner>()) Projectile.timeLeft = 5;
            Projectile.Center = player.Center;
            rotationSpeed = MathHelper.Lerp(1f, 3f, Utils.Clamp(modPlayer.WindrunnerCharge, 0, 5f) / 5f);
            chargeRotation += MathHelper.ToRadians(rotationSpeed);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            MeleePlayer modPlayer = player.GetModPlayer<MeleePlayer>();

            if (modPlayer.WindrunnerCharge > 0)
            {
                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "twirl_01").Value;
                float alpha = MathHelper.Lerp(0, 0.25f, Utils.Clamp(modPlayer.WindrunnerCharge, 0, 5) / 5f);
                if (modPlayer.WindrunnerCharge >= 5) alpha = 0.5f;

                DrawData textureLayer = new DrawData(texture, player.Center - Main.screenPosition, null, Color.White * alpha, chargeRotation, texture.Size() / 2f, 0.25f, SpriteEffects.FlipVertically, 1);
                textureLayer.Draw(Main.spriteBatch);
            }

            return base.PreDraw(ref lightColor);
        }
    }
}