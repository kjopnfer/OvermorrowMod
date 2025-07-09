using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Items.Bows;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Weapons.Bows
{
    public abstract class HeldBow : ModProjectile
    {
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        protected BowStats currentStats;
        protected BowStats baseStats;

        public virtual BowStats GetBaseBowStats() => new BowStats();
        public virtual SoundStyle DrawbackSound => new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/BowCharge");
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

            baseStats = GetBaseBowStats();
            SafeSetDefaults();
        }

        public Projectile LoadedArrow { private set; get; }
        public int LoadedArrowType { private set; get; }
        public int LoadedArrowItemType { private set; get; }

        private int AmmoSlotID;
        private int flashCounter = 0;

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

            // Update stats every frame to account for changing modifiers
            currentStats = BowModifierHandler.GetModifiedStats(baseStats, player);

            HandlePlayerDrawing();
            HandleBowUse();
        }

        //private float PracticeTargetModifier => currentStats.MaxChargeTime * (0.05f * player.GetModPlayer<BowPlayer>().PracticeTargetCounter);
        private int ModifiedChargeTime => (int)Math.Ceiling(currentStats.MaxChargeTime < 6 ? 6 : currentStats.MaxChargeTime);

        private void HandlePlayerDrawing()
        {
            float bowRotation = player.Center.DirectionTo(Main.MouseWorld).ToRotation();
            Projectile.rotation = bowRotation;
            Projectile.spriteDirection = bowRotation > MathHelper.PiOver2 || bowRotation < -MathHelper.PiOver2 ? -1 : 1;
            player.direction = Projectile.spriteDirection;

            Vector2 positionOffset = currentStats.PositionOffset.RotatedBy(bowRotation);
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + positionOffset;

            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            Player.CompositeArmStretchAmount stretchAmount = drawCounter < Math.Round(ModifiedChargeTime * 0.33) ? Player.CompositeArmStretchAmount.Full : Player.CompositeArmStretchAmount.Quarter;
            if (drawCounter > Math.Round(ModifiedChargeTime * 0.66f)) stretchAmount = Player.CompositeArmStretchAmount.None;

            player.SetCompositeArmFront(true, stretchAmount, Projectile.rotation - MathHelper.PiOver2);
        }

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
                    drawCounter += currentStats.ChargeSpeed;

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

                    player.itemTime = 30;
                    player.itemAnimation = 30;

                    flashCounter = 0;
                }

                if (drawCounter < 0) drawCounter++;
            }
        }

        private void ShootArrow()
        {
            float progress = Utils.Clamp(drawCounter, 0, ModifiedChargeTime) / (float)ModifiedChargeTime;
            Vector2 arrowOffset = Vector2.Lerp(Vector2.UnitX * 20, Vector2.UnitX * 16, progress).RotatedBy(Projectile.rotation);
            Vector2 arrowPosition = player.Center + arrowOffset;

            Vector2 velocity = Vector2.Normalize(player.Center.DirectionTo(Main.MouseWorld));
            float speed = MathHelper.Lerp(1, currentStats.MaxSpeed, progress);

            SoundEngine.PlaySound(ShootSound);

            float damage = MathHelper.Lerp(0.25f, 1, Utils.Clamp(drawCounter, 0, ModifiedChargeTime) / (float)ModifiedChargeTime) * Projectile.damage * currentStats.DamageMultiplier;

            float speedBonus = IsPowerShot() ? 1.5f : 1f;
            int arrow = Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, LoadedArrowType, "HeldBow"), arrowPosition, velocity * speed * speedBonus, LoadedArrowType, (int)damage, Projectile.knockBack * currentStats.KnockbackMultiplier, player.whoAmI);

            Projectile arrowProjectile = Main.projectile[arrow];

            if (IsPowerShot())
            {
                BowModifierHandler.TriggerPowerShot(this, player);

                for (int i = 0; i < 4; i++)
                {
                    float randomScale = Main.rand.NextFloat(0.05f, 0.15f);
                    float angleOffset = Main.rand.Next(-3, 3) * 5;
                    Vector2 particleVelocity = -(velocity * Main.rand.Next(4, 8)).RotatedBy(MathHelper.ToRadians(-90 / 4 * i + 30 + angleOffset));
                    //Particle.CreateParticle(Particle.ParticleType<WhiteSpark>(), arrowPosition, particleVelocity, Color.White, randomScale, 0.5f, 0f, randomScale, 0f, 20f);
                }

                Main.projectile[arrow].GetGlobalProjectile<GlobalProjectiles>().IsPowerShot = true;

                //if (player.GetModPlayer<OvermorrowModPlayer>().CapturedMirage)
                //{
                //    MirageDummyProjectile mirage = Projectile.NewProjectileDirect(null, arrowPosition, velocity * speed * speedBonus, ModContent.ProjectileType<MirageDummyProjectile>(), (int)damage, Projectile.knockBack, player.whoAmI).ModProjectile as MirageDummyProjectile;
                //    mirage.mirageArrow = GetRandomArrow();
                //}
            }

            BowModifierHandler.TriggerArrowFired(this, player, arrowProjectile);
            ConsumeAmmo();

            delayCounter = currentStats.ShootDelay;
            LoadedArrowItemType = -1;
        }

        private void ConsumeAmmo()
        {
            if (!currentStats.CanConsumeAmmo) return;

            if (player.inventory[AmmoSlotID].type != ItemID.EndlessQuiver)
                player.inventory[AmmoSlotID].stack--;
        }

        private bool FindAmmo()
        {
            LoadedArrowItemType = -1;

            if (currentStats.ConvertArrow != ItemID.None)
            {
                for (int i = 0; i <= 3; i++)
                {
                    Item item = player.inventory[54 + i];
                    if (item.type == ItemID.None || item.ammo != AmmoID.Arrow) continue;

                    if (item.type == currentStats.ConvertArrow)
                    {
                        LoadedArrowType = currentStats.ArrowType;
                        LoadedArrowItemType = item.type;
                        AmmoSlotID = 54 + i;
                        return true;
                    }
                }
            }

            if (LoadedArrowItemType == -1)
            {
                for (int i = 0; i <= 3; i++)
                {
                    Item item = player.inventory[54 + i];
                    if (item.type == ItemID.None || item.ammo != AmmoID.Arrow) continue;

                    LoadedArrowType = item.shoot;
                    LoadedArrowItemType = item.type;
                    AmmoSlotID = 54 + i;
                    return true;
                }
            }

            return false;
        }

        private int GetRandomArrow()
        {
            for (int i = 0; i <= 3; i++)
            {
                Item item = player.inventory[54 + i];
                if (item.type == ItemID.None || item.ammo != AmmoID.Arrow) continue;

                if (item.shoot != LoadedArrowType)
                {
                    return item.shoot;
                }
            }

            return LoadedArrowType;
        }

        private bool IsPowerShot() => flashCounter >= 6 && flashCounter <= 36;

        public override bool PreDraw(ref Color lightColor)
        {
            DrawBow(lightColor);
            DrawArrow(lightColor);
            return false;
        }

        private void DrawBow(Color lightColor)
        {
            Vector2 topPosition = Projectile.Center + currentStats.StringPositions.Item1.RotatedBy(Projectile.rotation);
            Vector2 bottomPosition = Projectile.Center + currentStats.StringPositions.Item2.RotatedBy(Projectile.rotation);

            Vector2 restingPosition = Vector2.UnitX * (drawCounter < 0 ? 12 : 10);
            Vector2 armOffset = Vector2.Lerp(restingPosition, Vector2.UnitX * -1, Utils.Clamp(drawCounter, 0, ModifiedChargeTime) / (float)ModifiedChargeTime).RotatedBy(Projectile.rotation);
            Vector2 armPosition = player.MountedCenter + armOffset;

            Color color = currentStats.StringGlow ? currentStats.StringColor : currentStats.StringColor * Lighting.Brightness((int)player.Center.X / 16, (int)player.Center.Y / 16);

            Utils.DrawLine(Main.spriteBatch, topPosition, armPosition, color, currentStats.StringColor, 1.25f);
            Utils.DrawLine(Main.spriteBatch, bottomPosition, armPosition, color, currentStats.StringColor, 1.25f);

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 1);
        }

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

            Effect effect = OvermorrowModFile.Instance.ColorFill.Value;
            effect.Parameters["ColorFillColor"].SetValue(Color.White.ToVector3());
            effect.Parameters["ColorFillProgress"].SetValue(flashProgress);
            effect.CurrentTechnique.Passes["ColorFill"].Apply();

            Color lerpColor = Color.Lerp(color, Color.White, flashProgress);

            Main.instance.LoadProjectile(LoadedArrowType);
            Texture2D texture = TextureAssets.Projectile[LoadedArrowType].Value;

            Main.spriteBatch.Draw(texture, arrowPosition + new Vector2(0, Projectile.gfxOffY) - Main.screenPosition, null, lerpColor, Projectile.rotation + MathHelper.PiOver2, texture.Size() / 2f, 0.75f, SpriteEffects.None, 1);

            Main.spriteBatch.Reload(SpriteSortMode.Deferred);
        }
    }
}