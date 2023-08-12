using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace OvermorrowMod.Content.Items.Weapons.Melee
{
    public class Knife : ModItem
    {
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0 && player.ownedProjectileCounts[ModContent.ProjectileType<Knife_Thrown>()] <= 0;

        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("{Keyword:Alt}: Throw the knife. Hold down to increase {Keyword:Focus}\n" +
                "{Keyword:Focus}: Gain 20% increased critical strike chance"); */
        }

        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.knockBack = 2f;
            Item.useStyle = ItemUseStyleID.Thrust; // Makes the player do the proper arm motion
            Item.useAnimation = 12;
            Item.useTime = 12;
            Item.width = 32;
            Item.height = 32;
            Item.crit = 20;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.autoReuse = true;
            Item.noUseGraphic = true; // The sword is actually a "projectile", so the item should not be visible when used
            Item.noMelee = true; // The projectile will do the damage and not the item

            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(0, 0, 0, 10);

            Item.shoot = ModContent.ProjectileType<Knife_Held>(); // The projectile is what makes a shortsword work
            Item.shootSpeed = 2.1f; // This value bleeds into the behavior of the projectile as velocity, keep that in mind when tweaking values
        }

        // Draw knife counter in inventory
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Main.playerInventory) return;

            int knifeCount = 1 - Main.LocalPlayer.ownedProjectileCounts[ModContent.ProjectileType<Knife_Thrown>()];
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, knifeCount.ToString(), position + new Vector2(0f, 22f) * Main.inventoryScale, Color.White, 0f, Vector2.Zero, new Vector2(Main.inventoryScale), -1f, Main.inventoryScale);
        }

        public int attackIndex = 1;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            attackIndex++;
            if (attackIndex > 1) attackIndex = 0;

            if (player.altFunctionUse == 2)
                Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, -1);
            else
                Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, attackIndex);

            return false;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
    }

    public class Knife_Held : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override bool? CanHitNPC(NPC target) => !target.friendly && inSwingState;

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (HoldCounter >= heavySwingThreshold)
            {
                modifiers.SourceDamage *= 1.5f;
                modifiers.Knockback *= 2;
            }
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Knife");
        }

        public override void SetDefaults()
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
            Projectile.localNPCHitCooldown = 3;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
        }

        Player player => Main.player[Projectile.owner];

        public ref float ComboIndex => ref Projectile.ai[0];
        public ref float AICounter => ref Projectile.ai[1];


        // TODO: OnSpawn Hook with combo initializer
        public override void OnSpawn(IEntitySource source)
        {
            InitializeValues();
        }

        /// <summary>
        /// Initializes swing and damage values based on the combo index
        /// </summary>
        private void InitializeValues()
        {
            switch (ComboIndex)
            {
                default:
                    break;
            }
        }

        /// <summary>
        /// How long the player has gone without taking an action. Used to determine combo time.
        /// </summary>
        public float InactiveCounter = 0;
        public float InactiveLimit = 60;

        /// <summary>
        /// How long the player has held down the weapon, separate from the counter that handles actions
        /// </summary>
        public float HoldCounter = 0; // TODO: Sync this with ExtraAI

        private bool IsExecutingAction = false;
        public override void AI()
        {
            if (player.active && player.HeldItem.type == ModContent.ItemType<Knife>()) Projectile.timeLeft = 10;

            Projectile.width = Projectile.height = 40;
            player.heldProj = Projectile.whoAmI;

            HandleArmDrawing();
            HandleWeaponUse();
        }

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

        public int maxHoldCount = 60;
        /// <summary>
        /// Executed whenever the player holds down left mouse, used to draw the weapon moving back or any other prep effects.
        /// </summary>
        private void HandleWeaponHold()
        {
            InactiveCounter = 0;
            IsExecutingAction = true;

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
                        return 15;
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
                    case 2:
                        return 8;
                    default:
                        return 5;
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

                //if (HoldCounter < heavySwingThreshold) Main.NewText("light attack");
                //if (HoldCounter >= heavySwingThreshold) Main.NewText("heavy attack");
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
                        Projectile proj = Projectile.NewProjectileDirect(null, Projectile.Center, throwVelocity * 10, ModContent.ProjectileType<Knife_Thrown>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, Projectile.rotation);
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
                IsExecutingAction = false;
                InactiveCounter = 0;

                Projectile.Kill();
            }
        }

        int flashCounter = 0;
        Vector2 spriteOffset = Vector2.Zero;
        Vector2 spriteCenter = Vector2.Zero;
        private void HandleWeaponDrawing(Color lightColor)
        {
            Texture2D texture = TextureAssets.Item[ModContent.ItemType<Knife>()].Value;
            var spriteEffects = player.direction == 1 ? SpriteEffects.FlipVertically : SpriteEffects.None;

            Vector2 spritePositionOffset = Vector2.Zero;
            float rotationOffset = 0f;
            switch (ComboIndex)
            {
                case -1:
                    spritePositionOffset = new Vector2(6, 0).RotatedBy(Projectile.rotation);
                    rotationOffset = MathHelper.ToRadians(120 * player.direction);
                    break;
                case 0:
                    spritePositionOffset = new Vector2(-16, 12 * player.direction).RotatedBy(Projectile.rotation);
                    rotationOffset = MathHelper.ToRadians(45 * player.direction);
                    break;
                case 1:
                    spritePositionOffset = new Vector2(-8, 12 * player.direction).RotatedBy(Projectile.rotation);
                    rotationOffset = MathHelper.ToRadians(-45 * player.direction);
                    break;
                case 2:
                    spritePositionOffset = new Vector2(-2, 2 * player.direction).RotatedBy(Projectile.rotation);
                    rotationOffset = MathHelper.ToRadians(45 * player.direction);
                    break;
            }

            float flashProgress = Utils.Clamp((float)Math.Sin(flashCounter / 5f), 0, 1);

            Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
            effect.Parameters["WhiteoutColor"].SetValue(Color.White.ToVector3());
            effect.Parameters["WhiteoutProgress"].SetValue(flashProgress);
            effect.CurrentTechnique.Passes["Whiteout"].Apply();

            Color lerpColor = Color.Lerp(lightColor, Color.White, flashProgress);

            Main.spriteBatch.Draw(texture, spriteCenter + spritePositionOffset - Main.screenPosition, null, lerpColor, Projectile.rotation + rotationOffset, texture.Size() / 2f, Projectile.scale, spriteEffects, 1);
        }

        public float swingAngle = 0;
        private void HandleArmDrawing()
        {
            Vector2 mousePosition = justReleasedWeapon ? lastMousePosition : Main.MouseWorld;
            Projectile.spriteDirection = mousePosition.X < player.Center.X ? -1 : 1;
            player.direction = Projectile.spriteDirection;

            float staffRotation = player.Center.DirectionTo(mousePosition).ToRotation() + MathHelper.ToRadians(swingAngle) * -player.direction;
            Projectile.rotation = staffRotation;

            float backRotation = player.direction == -1 ? -150 : -30;

            switch (ComboIndex)
            {
                case 2:
                    if (AICounter < backTime + 2)
                    {
                        float progress = MathHelper.Lerp(0, 100, ModUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));
                        if (progress < 50)
                        {
                            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, Projectile.rotation + MathHelper.ToRadians(-90));
                        }
                        else
                        {
                            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, Projectile.rotation + MathHelper.ToRadians(-90));
                        }
                    }

                    if (justReleasedWeapon) // For some reason the arm resets while holding the staff back
                    {
                        if (AICounter > backTime && AICounter <= backTime + forwardTime)
                        {
                            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.ToRadians(-90));
                        }

                        if (AICounter > backTime + forwardTime && AICounter <= backTime + forwardTime + holdTime)
                        {
                            float progress = MathHelper.Lerp(0, 100, ModUtils.EaseOutQuint(Utils.Clamp(AICounter - (backTime + forwardTime), 0, holdTime) / holdTime));
                            if (progress < 50)
                            {
                                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, Projectile.rotation + MathHelper.ToRadians(-90));
                            }
                            else
                            {
                                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, Projectile.rotation + MathHelper.ToRadians(-90));
                            }
                        }
                    }

                    break;
                default:
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.ToRadians(-90));
                    break;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Width = 35;
            hitbox.Height = 35;

            Vector2 hitboxOffset;
            switch (ComboIndex)
            {
                case 2:
                    //hitbox.Height = 45;
                    hitboxOffset = positionOffset.RotatedBy(Projectile.rotation);

                    hitbox.X = (int)(player.Center.X - (hitbox.Width / 2f) + hitboxOffset.X);
                    hitbox.Y = (int)(player.Center.Y - (hitbox.Height / 2f) + hitboxOffset.Y);
                    break;
                default:
                    //hitbox.Width = 60;

                    hitboxOffset = new Vector2(25, -5 * player.direction).RotatedBy(Projectile.rotation);
                    hitbox.X = (int)(player.Center.X - (hitbox.Width / 2f) + hitboxOffset.X);
                    hitbox.Y = (int)(player.Center.Y - (hitbox.Height / 2f) + hitboxOffset.Y);
                    break;
            }

            spriteCenter = new Vector2(hitbox.X + (hitbox.Width / 2f), hitbox.Y + (hitbox.Height / 2f));

            base.ModifyDamageHitbox(ref hitbox);
        }

        public override void Kill(int timeLeft)
        {
            base.Kill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            HandleWeaponDrawing(lightColor);

            return false;
        }
    }

    public class Knife_Thrown : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;

        public override bool? CanDamage()
        {
            return !groundCollided;
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Knife");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2; // Make the hitbox small to prevent hitting the ground too early
            Projectile.DamageType = DamageClass.Melee;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 3;

        }

        public override void AI()
        {
            Tile bottomLeftTile = Main.tile[(int)Projectile.Hitbox.BottomLeft().X / 16, (int)Projectile.Hitbox.BottomLeft().Y / 16];
            Tile bottomRightTile = Main.tile[(int)Projectile.Hitbox.BottomRight().X / 16, (int)Projectile.Hitbox.BottomRight().Y / 16];

            // These are for weird slopes that don't trigger the collision code normally
            if ((bottomLeftTile.HasTile && Main.tileSolid[bottomLeftTile.TileType]) || 
                (bottomRightTile.HasTile && Main.tileSolid[bottomRightTile.TileType])) HandleCollisionBounce();

            if (Projectile.ai[0] == 0)
            {
                Projectile.rotation = Projectile.ai[1];
                Projectile.ai[1] = 0;
            }

            // Make the hitbox normal again after 1/6th of a second
            if (Projectile.ai[0] > 10)
                Projectile.width = Projectile.height = 32;


            if (!groundCollided)
            {
                Projectile.velocity.X *= 0.99f;
                Projectile.rotation += 0.48f * (Projectile.velocity.X > 0 ? 1 : -1);

                if (Projectile.ai[0]++ > 10)
                    Projectile.velocity.Y += 0.25f;
            }
            else
            {
                Projectile.velocity.X *= 0.97f;

                if (Projectile.ai[1] == 60f)
                {
                    Projectile.velocity.X *= 0.01f;
                    oldPosition = Projectile.Center;
                }

                float rotationFactor = MathHelper.Lerp(0.48f, 0f, Utils.Clamp(Projectile.ai[1]++, 0, 60f) / 60f);
                Projectile.rotation += rotationFactor * (Projectile.velocity.X > 0 ? 1 : -1);

                Projectile.velocity.Y *= 0.96f;

                if (Projectile.ai[1] > 60f)
                {
                    Projectile.tileCollide = false;
                    //Projectile.rotation = MathHelper.Lerp(oldRotation, MathHelper.TwoPi + oldRotation, Utils.Clamp(Projectile.ai[1], 0, 20f) / 20f);
                    Projectile.Center = Vector2.Lerp(oldPosition, oldPosition - Vector2.UnitY * -24, (float)Math.Sin((Projectile.ai[1] - 60f) / 40f));
                }
            }

            // Check for if the owner has picked up the knife after it has landed
            if (groundCollided)
            {
                foreach (Player player in Main.player)
                {
                    if (player.whoAmI != Projectile.owner) continue;
                    if (player.Hitbox.Intersects(Projectile.Hitbox)) Projectile.Kill();
                }
            }

            base.AI();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float activeAlpha = MathHelper.Lerp(0f, 1f, Utils.Clamp(Projectile.timeLeft, 0, 60f) / 60f);
            if (groundCollided)
            {
                float alpha = 0.65f * activeAlpha;
                Main.spriteBatch.Reload(BlendState.Additive);

                Texture2D outline = ModContent.Request<Texture2D>(AssetDirectory.Textures + "RingSolid").Value;
                Main.spriteBatch.Draw(outline, Projectile.Center - Main.screenPosition, null, Color.Orange * alpha, Projectile.rotation, outline.Size() / 2f, 1f * 0.1f, SpriteEffects.None, 1);

                outline = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_05").Value;
                Main.spriteBatch.Draw(outline, Projectile.Center - Main.screenPosition, null, Color.Orange * alpha, Projectile.rotation, outline.Size() / 2f, 1f * 0.5f, SpriteEffects.None, 1);

                Main.spriteBatch.Reload(BlendState.AlphaBlend);
            }

            Texture2D texture = TextureAssets.Item[ModContent.ItemType<Knife>()].Value;
            SpriteEffects spriteEffects = Projectile.velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Color color = groundCollided ? Color.White : lightColor;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color * activeAlpha, Projectile.rotation, texture.Size() / 2f, Projectile.scale, spriteEffects, 1);

            return base.PreDraw(ref lightColor);
        }

        bool groundCollided = false;
        float oldRotation;
        Vector2 oldPosition;
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!groundCollided && Projectile.velocity.Y > 0)
            {
                HandleCollisionBounce();
            }
            else
            {
                Projectile.velocity *= -0.5f;
            }
            /*if (Projectile.velocity != Vector2.Zero)
            {
                Projectile.timeLeft = 600;
                Projectile.velocity = Vector2.Zero;
                Projectile.rotation = MathHelper.ToRadians(45);

                oldPosition = Projectile.Center;
                oldRotation = Projectile.rotation;
            }*/

            return false;
        }

        private void HandleCollisionBounce()
        {
            if (groundCollided) return;

            groundCollided = true;
            Projectile.velocity.X *= 0.5f;
            Projectile.velocity.Y = Main.rand.NextFloat(-2.2f, -1f);
            Projectile.timeLeft = 600;
        }
    }
}