using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System;
using Terraria.Audio;
using OvermorrowMod.Core;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Players;
using OvermorrowMod.Content.Items.Accessories.CapturedMirage;

namespace OvermorrowMod.Common.VanillaOverrides.Bow
{
    public abstract partial class HeldBow : ModProjectile
    {
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        /// <summary>
        /// Determines whether the bow consumes any ammo on use.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual bool CanConsumeAmmo(Player player) => true;

        /// <summary>
        /// The offsets for the top and bottom string endpoints, respectively.
        /// </summary>
        public virtual (Vector2, Vector2) StringPositions => (new Vector2(-5, 14), new Vector2(-5, -14));

        /// <summary>
        /// The offset for where the bow should be held. Defaults to <c>(13, 0)</c>.
        /// </summary>
        public virtual Vector2 PositionOffset => new Vector2(15, 0);

        /// <summary>
        /// The color of the bow's string.
        /// </summary>
        public virtual Color StringColor => Color.White;

        /// <summary>
        /// Determines whether the string of the bow ignores light.
        /// </summary>
        public virtual bool StringGlow => false;

        /// <summary>
        /// Determines how quickly the bow charges per tick
        /// </summary>
        public virtual float ChargeSpeed => 1;

        /// <summary>
        /// Determines how many ticks are required for the bow to be fully charged
        /// </summary>
        public virtual float MaxChargeTime => 60;

        /// <summary>
        /// Determines the delay inbetween the bow firing projectiles
        /// </summary>
        public virtual float ShootDelay => 30;

        /// <summary>
        /// The maximum velocity that the bow's projectiles can be fired as
        /// </summary>
        public virtual float MaxSpeed => 12;

        /// <summary>
        /// Determines if the bow fires a unique type of arrow. Uses Projectile ID instead of Item ID.
        /// </summary>
        public virtual int ArrowType => ProjectileID.None;

        /// <summary>
        /// Determines what arrow type is needed in order to convert the arrows to if ArrowType is given. Uses Item ID.
        /// </summary>
        public virtual int ConvertArrow => ItemID.None;

        public virtual SoundStyle DrawbackSound => new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/bowCharge");

        public virtual SoundStyle ShootSound => SoundID.Item5;
        public abstract int ParentItem { get; }

        public virtual void SafeSetDefaults() { }

        public sealed override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1;

            SafeSetDefaults();
        }

        public Projectile LoadedArrow { private set; get; }
        public int LoadedArrowType { private set; get; }
        public int LoadedArrowItemType { private set; get; }

        private int AmmoSlotID;

        public Player player => Main.player[Projectile.owner];
        public ref float drawCounter => ref Projectile.ai[0];
        public ref float delayCounter => ref Projectile.ai[1];
        public override void AI()
        {
            if (Main.myPlayer != player.whoAmI) return;
            if (player.HeldItem.type != ParentItem || !player.active || player.dead)
                Projectile.Kill();
            else
                Projectile.timeLeft = 5;

            player.heldProj = Projectile.whoAmI;

            HandlePlayerDrawing();
            HandleBowUse();
        }

        private float PracticeTargetModifier => MaxChargeTime * (0.05f * player.GetModPlayer<BowPlayer>().PracticeTargetCounter);
        private int ModifiedChargeTime => (int)Math.Ceiling(MaxChargeTime - PracticeTargetModifier < 6 ? 6 : MaxChargeTime - PracticeTargetModifier);

        /// <summary>
        /// Handles the drawing of the player's arm and the bow's rotation
        /// </summary>
        private void HandlePlayerDrawing()
        {
            float bowRotation = player.Center.DirectionTo(Main.MouseWorld).ToRotation();
            Projectile.rotation = bowRotation;
            Projectile.spriteDirection = bowRotation > MathHelper.PiOver2 || bowRotation < -MathHelper.PiOver2 ? -1 : 1;
            player.direction = Projectile.spriteDirection;

            Vector2 positionOffset = PositionOffset.RotatedBy(bowRotation);
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + positionOffset;

            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            Player.CompositeArmStretchAmount stretchAmount = drawCounter < Math.Round(ModifiedChargeTime * 0.33) ? Player.CompositeArmStretchAmount.Full : Player.CompositeArmStretchAmount.Quarter;
            if (drawCounter > Math.Round(ModifiedChargeTime * 0.66f)) stretchAmount = Player.CompositeArmStretchAmount.None;

            player.SetCompositeArmFront(true, stretchAmount, Projectile.rotation - MathHelper.PiOver2);

        }

        /// <summary>
        /// Handles ammo usage, counters, and arrow firing
        /// </summary>
        private void HandleBowUse()
        {
            if (delayCounter > 0) delayCounter--;

            if (player.controlUseItem && drawCounter >= 0)
            {
                Projectile.timeLeft = 120;

                if (delayCounter != 0) return;

                if (drawCounter == 0)
                {
                    if (FindAmmo()) SoundEngine.PlaySound(DrawbackSound);
                    ModUtils.AutofillAmmoSlots(player, AmmoID.Arrow);
                }

                if (FindAmmo())
                {
                    if (drawCounter > ModifiedChargeTime) drawCounter = ModifiedChargeTime;
                    drawCounter += ChargeSpeed;

                    // Prevent the player from switching items if they are drawing the bow
                    player.itemTime = 2;
                    player.itemAnimation = 2;
                }
            }
            else
            {
                if (drawCounter > 0)
                {
                    ShootArrow();
                    drawCounter = -6;

                    // Prevent the player from switching items if they have shot the bow
                    player.itemTime = 30;
                    player.itemAnimation = 30;

                    flashCounter = 0;
                }

                if (drawCounter < 0) drawCounter++;
            }
        }

        /// <summary>
        /// Allows for any additional bonuses and effects to happen when a focus shot is executed
        /// </summary>
        public virtual void OnPowerShot() { }

        /// <summary>
        /// Handles the code for projectile firing
        /// </summary>
        private void ShootArrow()
        {
            float progress = Utils.Clamp(drawCounter, 0, ModifiedChargeTime) / (float)ModifiedChargeTime;
            Vector2 arrowOffset = Vector2.Lerp(Vector2.UnitX * 20, Vector2.UnitX * 16, progress).RotatedBy(Projectile.rotation);
            Vector2 arrowPosition = player.Center + arrowOffset;

            Vector2 velocity = Vector2.Normalize(player.Center.DirectionTo(Main.MouseWorld));
            float speed = MathHelper.Lerp(1, MaxSpeed, progress);

            SoundEngine.PlaySound(ShootSound);

            float damage = MathHelper.Lerp(0.25f, 1, Utils.Clamp(drawCounter, 0, ModifiedChargeTime) / (float)ModifiedChargeTime) * Projectile.damage;

            float speedBonus = IsPowerShot() ? 1.5f : 1f;
            int arrow = Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, LoadedArrowType, "HeldBow"), arrowPosition, velocity * speed * speedBonus, LoadedArrowType, (int)damage, Projectile.knockBack, player.whoAmI);

            if (IsPowerShot())
            {
                OnPowerShot();

                for (int i = 0; i < 4; i++)
                {
                    float randomScale = Main.rand.NextFloat(0.05f, 0.15f);
                    float angleOffset = Main.rand.Next(-3, 3) * 5;
                    Vector2 particleVelocity = -(velocity * Main.rand.Next(4, 8)).RotatedBy(MathHelper.ToRadians(-90 / 4 * i + 30 + angleOffset));
                    Particle.CreateParticle(Particle.ParticleType<WhiteSpark>(), arrowPosition, particleVelocity, Color.White, randomScale, 0.5f, 0f, randomScale, 0f, 20f);
                }

                Main.projectile[arrow].GetGlobalProjectile<OvermorrowGlobalProjectile>().IsPowerShot = true;

                if (player.GetModPlayer<OvermorrowModPlayer>().CapturedMirage)
                {
                    MirageDummyProjectile mirage = Projectile.NewProjectileDirect(null, arrowPosition, velocity * speed * speedBonus, ModContent.ProjectileType<MirageDummyProjectile>(), (int)damage, Projectile.knockBack, player.whoAmI).ModProjectile as MirageDummyProjectile;
                    mirage.mirageArrow = GetRandomArrow();
                }
            }

            ConsumeAmmo();

            delayCounter = ShootDelay;
            LoadedArrowItemType = -1;
        }

        private int flashCounter = 0;

        /// <summary>
        /// Handles the ingame drawing for the projectile on the held bow
        /// </summary>
        /// <param name="lightColor"></param>
        private void DrawArrow(Color lightColor)
        {
            if (drawCounter >= ModifiedChargeTime)
            {
                if (flashCounter == 0) SoundEngine.PlaySound(SoundID.MaxMana);
                if (flashCounter < 48 && !Main.gamePaused) flashCounter++;
            }

            Vector2 arrowOffset = Vector2.Lerp(Vector2.UnitX * 20, Vector2.UnitX * 16, Utils.Clamp(drawCounter, 0, 40f) / 40f).RotatedBy(Projectile.rotation);
            Vector2 arrowPosition = player.MountedCenter + arrowOffset;

            if (LoadedArrowItemType == -1) return;

            if (LoadedArrowType == ProjectileID.FireArrow) Lighting.AddLight(arrowPosition, 1f, 0.647f, 0);
            if (LoadedArrowType == ProjectileID.FrostburnArrow) Lighting.AddLight(arrowPosition, 0f, 0.75f, 0.75f);
            if (LoadedArrowType == ProjectileID.CursedArrow) Lighting.AddLight(arrowPosition, 0.647f, 1f, 0f);

            Color color = LoadedArrowType == ProjectileID.JestersArrow ? Color.White : lightColor;

            Main.spriteBatch.Reload(SpriteSortMode.Immediate);

            float flashProgress = Utils.Clamp((float)Math.Sin(flashCounter / 12f), 0, 1);

            Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
            effect.Parameters["WhiteoutColor"].SetValue(Color.White.ToVector3());
            effect.Parameters["WhiteoutProgress"].SetValue(flashProgress);
            effect.CurrentTechnique.Passes["Whiteout"].Apply();

            Color lerpColor = Color.Lerp(color, Color.White, flashProgress);

            Main.instance.LoadProjectile(LoadedArrowType);
            Texture2D texture = TextureAssets.Projectile[LoadedArrowType].Value;

            Main.spriteBatch.Draw(texture, arrowPosition + new Vector2(0, Projectile.gfxOffY) - Main.screenPosition, null, lerpColor, Projectile.rotation + MathHelper.PiOver2, texture.Size() / 2f, 0.75f, SpriteEffects.None, 1);

            Main.spriteBatch.Reload(SpriteSortMode.Deferred);
        }

        /// <summary>
        /// Handles the ingame drawing for the bow's sprite
        /// </summary>
        /// <param name="lightColor"></param>
        private void DrawBow(Color lightColor)
        {
            Vector2 topPosition = Projectile.Center + StringPositions.Item1.RotatedBy(Projectile.rotation);
            Vector2 bottomPosition = Projectile.Center + StringPositions.Item2.RotatedBy(Projectile.rotation);

            Vector2 restingPosition = Vector2.UnitX * (drawCounter < 0 ? 12 : 10);
            Vector2 armOffset = Vector2.Lerp(restingPosition, Vector2.UnitX * -1, Utils.Clamp(drawCounter, 0, ModifiedChargeTime) / (float)ModifiedChargeTime).RotatedBy(Projectile.rotation);
            Vector2 armPosition = player.MountedCenter + armOffset;

            Color color = StringGlow ? StringColor : StringColor * Lighting.Brightness((int)player.Center.X / 16, (int)player.Center.Y / 16);

            Utils.DrawLine(Main.spriteBatch, topPosition, armPosition, color, StringColor, 1.25f);
            Utils.DrawLine(Main.spriteBatch, bottomPosition, armPosition, color, StringColor, 1.25f);

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 1);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawBow(lightColor);
            DrawArrow(lightColor);

            return false;
        }
    }
}