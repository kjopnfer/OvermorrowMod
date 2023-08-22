using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria.DataStructures;
using System.IO;
using System;

namespace OvermorrowMod.Common.VanillaOverrides.Melee
{
    public abstract partial class HeldDagger : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;

        // I don't know if there is a different way to do this
        public abstract int ParentItem { get; }
        public abstract int ThrownProjectile { get; }

        public override bool? CanHitNPC(NPC target) => !target.friendly && inSwingState;

        public sealed override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            SafeSetDefaults();
        }

        public virtual void SafeSetDefaults() { }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (HoldCounter >= heavySwingThreshold)
            {
                modifiers.SourceDamage *= 1.5f;
                modifiers.Knockback *= 2;
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (DualWieldFlag == 1)
            {
                Projectile.hide = true;
                behindProjectiles.Add(index);
            }

            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }

        /// <summary>
        /// How long the player has held down the weapon, separate from the counter that handles actions
        /// </summary>
        public float HoldCounter = 0; // TODO: Sync this with ExtraAI
        protected Player player => Main.player[Projectile.owner];
        public ref float ComboIndex => ref Projectile.ai[0];
        public ref float DualWieldFlag => ref Projectile.ai[1];
        public ref float AICounter => ref Projectile.ai[2];

        public sealed override void AI()
        {
            if (player.active && player.HeldItem.type == ParentItem) Projectile.timeLeft = 10;

            Projectile.width = Projectile.height = 40;

            if (DualWieldFlag != 1)
                player.heldProj = Projectile.whoAmI;

            HandleArmDrawing();
            HandleWeaponUse();
        }

        public int maxHoldCount = 60;
        /// <summary>
        /// Executed whenever the player holds down left mouse, used to draw the weapon moving back or any other prep effects.
        /// </summary>
        private void HandleWeaponHold()
        {
            if (HoldCounter < maxHoldCount) HoldCounter++;
            if (HoldCounter > heavySwingThreshold && flashCounter <= 15) flashCounter++;

            if (AICounter <= backTime)
            {
                AICounter++;
                switch (ComboIndex)
                {
                    case 0:
                        positionOffset = (player.direction == -1 ? new Vector2(14, -4) : new Vector2(10, 6)).RotatedBy(Projectile.rotation);
                        swingAngle = MathHelper.Lerp(0, 100, ModUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));
                        break;
                    case 2:
                        float xOffset = MathHelper.Lerp(20, 6, ModUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));
                        positionOffset = new Vector2(xOffset, 0);
                        break;
                    case -1:
                        swingAngle = MathHelper.Lerp(0, 105, ModUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));
                        break;
                    default:
                        positionOffset = (player.direction == -1 ? new Vector2(14, -4) : new Vector2(10, 6)).RotatedBy(Projectile.rotation);
                        swingAngle = MathHelper.Lerp(0, -135, ModUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));
                        break;
                }
            }
        }


        float backTime
        {
            get
            {
                switch (ComboIndex)
                {
                    case 1:
                        return 10;
                    default:
                        return DualWieldFlag == 1 ? 5 : 15;
                }
            }
        }

        float forwardTime
        {
            get
            {
                switch (ComboIndex)
                {
                    case -1:
                        return 4;
                    default:
                        return DualWieldFlag == 1 ? 10 : 5;
                }
            }
        }

        float holdTime
        {
            get
            {
                return ComboIndex == -1 ? 4 : 10;
            }
        }

        int heavySwingThreshold = 15;

        private bool justReleasedWeapon = false;
        private bool inSwingState = false;
        private Vector2 lastMousePosition;
        private void HandleWeaponRelease()
        {
            // On weapon release is when we execute the attack animation
            if (!justReleasedWeapon)
            {
                flashCounter = 0;
                justReleasedWeapon = true;
                lastMousePosition = Main.MouseWorld;
            }

            // Position
            switch (ComboIndex)
            {
                case 2:
                    float xOffset = 0f;
                    if (AICounter <= backTime)
                        xOffset = MathHelper.Lerp(20, 6, ModUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));

                    if (AICounter > backTime && AICounter <= backTime + forwardTime)
                    {
                        if (!inSwingState) SoundEngine.PlaySound(SoundID.Item1, player.Center);

                        inSwingState = true;
                        xOffset = MathHelper.Lerp(6, 32, ModUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));
                    }

                    if (AICounter > backTime + forwardTime && AICounter <= backTime + forwardTime + holdTime)
                        xOffset = MathHelper.Lerp(32, 12, ModUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));

                    positionOffset = new Vector2(xOffset, 0);
                    break;
                default:
                    positionOffset = (player.direction == -1 ? new Vector2(14, -4) : new Vector2(10, 6)).RotatedBy(Projectile.rotation);
                    break;
            }

            AICounter++;

            // Angle
            switch (ComboIndex)
            {
                case -1:
                    if (AICounter <= backTime)
                        swingAngle = MathHelper.Lerp(0, 105, ModUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));

                    if (AICounter > backTime && AICounter <= backTime + forwardTime)
                    {
                        if (!inSwingState) SoundEngine.PlaySound(SoundID.Item1, player.Center);

                        inSwingState = true;
                        swingAngle = MathHelper.Lerp(105, 15, ModUtils.EaseInCubic(Utils.Clamp(AICounter - backTime, 0, forwardTime) / forwardTime));
                    }

                    if (AICounter > backTime + forwardTime && AICounter <= backTime + forwardTime + holdTime)
                    {
                        inSwingState = false;
                        swingAngle = MathHelper.Lerp(15, -45, ModUtils.EaseInQuart(Utils.Clamp(AICounter - (backTime + forwardTime), 0, holdTime) / holdTime));
                    }

                    if (AICounter >= backTime + forwardTime + holdTime)
                    {
                        Vector2 throwVelocity = Vector2.Normalize(Projectile.DirectionTo(Main.MouseWorld));
                        Projectile proj = Projectile.NewProjectileDirect(null, Projectile.Center, throwVelocity * 10, ThrownProjectile, Projectile.damage, Projectile.knockBack, Projectile.owner, 0, Projectile.rotation);
                        proj.CritChance = Projectile.CritChance;
                        if (HoldCounter >= heavySwingThreshold) proj.CritChance += 20;

                        Projectile.Kill();
                    }
                    break;
                case 0:
                    if (AICounter <= backTime)
                        swingAngle = MathHelper.Lerp(-45, 95, ModUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));

                    if (AICounter > backTime && AICounter <= backTime + forwardTime)
                    {
                        if (!inSwingState) SoundEngine.PlaySound(SoundID.Item1, player.Center);

                        inSwingState = true;
                        swingAngle = MathHelper.Lerp(95, -75, ModUtils.EaseInCubic(Utils.Clamp(AICounter - backTime, 0, forwardTime) / forwardTime));
                    }

                    if (AICounter > backTime + forwardTime && AICounter <= backTime + forwardTime + holdTime)
                    {
                        inSwingState = false;
                        swingAngle = MathHelper.Lerp(-75, -25, ModUtils.EaseInQuart(Utils.Clamp(AICounter - (backTime + forwardTime), 0, holdTime) / holdTime));
                    }
                    break;

                default:
                    if (AICounter <= backTime)
                        swingAngle = MathHelper.Lerp(-45, -105, ModUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));

                    if (AICounter > backTime && AICounter <= backTime + forwardTime)
                    {
                        if (!inSwingState) SoundEngine.PlaySound(SoundID.Item1, player.Center);

                        inSwingState = true;
                        swingAngle = MathHelper.Lerp(-105, 35, ModUtils.EaseInCubic(Utils.Clamp(AICounter - backTime, 0, forwardTime) / forwardTime));
                    }

                    if (AICounter > backTime + forwardTime && AICounter <= backTime + forwardTime + holdTime)
                    {
                        inSwingState = false;
                        swingAngle = MathHelper.Lerp(35, -45, ModUtils.EaseInQuart(Utils.Clamp(AICounter - (backTime + forwardTime), 0, holdTime) / holdTime));
                    }
                    break;
            }

            // HoldCounter gets reset to zero at the end of the attack animation
            if (AICounter >= backTime + forwardTime + holdTime)
            {
                AICounter = 0;
                HoldCounter = 0;
                swingAngle = 0;

                justReleasedWeapon = false;
                Projectile.Kill();
            }
        }

        int flashCounter = 0;
        Vector2 spriteOffset = Vector2.Zero;
        Vector2 spriteCenter = Vector2.Zero;
        private void HandleWeaponDrawing(Color lightColor)
        {
            Texture2D texture = TextureAssets.Item[ParentItem].Value;
            var spriteEffects = player.direction == 1 ? SpriteEffects.FlipVertically : SpriteEffects.None;

            Vector2 spritePositionOffset = Vector2.Zero;
            Vector2 dualWieldOffset = Vector2.Zero;
            float rotationOffset = 0f;
            float scaleFactor = DualWieldFlag == 1 ? 0.9f : 1f;

            SetWeaponDrawing(ref spritePositionOffset, ref dualWieldOffset, ref rotationOffset, ref scaleFactor);

            float flashProgress = Utils.Clamp((float)Math.Sin(flashCounter / 5f), 0, 1);

            Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
            effect.Parameters["WhiteoutColor"].SetValue(Color.White.ToVector3());
            effect.Parameters["WhiteoutProgress"].SetValue(flashProgress);
            effect.CurrentTechnique.Passes["Whiteout"].Apply();

            Color lerpColor = Color.Lerp(lightColor, Color.White, flashProgress);

            Main.spriteBatch.Draw(texture, spriteCenter + spritePositionOffset - Main.screenPosition, null, lerpColor, Projectile.rotation + rotationOffset, texture.Size() / 2f, Projectile.scale * scaleFactor, spriteEffects, 1);
        }

        public virtual void SetWeaponDrawing(ref Vector2 spritePositionOffset, ref Vector2 dualWieldOffset, ref float rotationOffset, ref float scaleFactor) { }

        public float swingAngle = 0;
        private void HandleArmDrawing()
        {
            Vector2 mousePosition = justReleasedWeapon ? lastMousePosition : Main.MouseWorld;
            Projectile.spriteDirection = mousePosition.X < player.Center.X ? -1 : 1;
            player.direction = Projectile.spriteDirection;

            float staffRotation = player.Center.DirectionTo(mousePosition).ToRotation() + MathHelper.ToRadians(swingAngle) * -player.direction;
            Projectile.rotation = staffRotation;

            switch (ComboIndex)
            {
                default:
                    if (DualWieldFlag == 1)
                        player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.ToRadians(-90));
                    else
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.ToRadians(-90));
                    break;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
        }

        public sealed override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            Vector2 hitboxOffset = Vector2.Zero;
            SetDamageHitbox(positionOffset, ref hitboxOffset, ref hitbox);

            spriteCenter = new Vector2(hitbox.X + (hitbox.Width / 2f), hitbox.Y + (hitbox.Height / 2f));
        }

        public virtual void SetDamageHitbox(Vector2 positionOffset, ref Vector2 hitboxOffset, ref Rectangle hitbox) { }

        Vector2 positionOffset;
        private void HandleWeaponUse()
        {
            // Throwing knife behavior
            if (ComboIndex == -1)
            {
                if (Main.mouseRight && !justReleasedWeapon) HandleWeaponHold();
                else if (HoldCounter > 0)
                {
                    HandleWeaponRelease();
                }
            }
            else
                HandleWeaponRelease();

            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter);
        }

        public sealed override bool PreDraw(ref Color lightColor)
        {
            HandleWeaponDrawing(lightColor);

            return false;
        }
    }
}