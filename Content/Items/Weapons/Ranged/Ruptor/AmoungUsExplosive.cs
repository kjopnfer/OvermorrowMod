using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Ruptor
{
    public class AmoungUsExplosive : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ruptor");
            Main.projFrames[projectile.type] = 4;
        }
        public override bool CanDamage() => true;

        int HasHit = 0;
        int AniTimer = 20;
        public bool HasHitGround = false;

        bool DMGsave = false;
        int savedDMG = 0;

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 38;
            projectile.ranged = true;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 2;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.timeLeft = 200;

        }
        public bool IsStickingToTarget
        {
            get => projectile.ai[0] == 1f;
            set => projectile.ai[0] = value ? 1f : 0f;
        }
        // Index of the current target
        public int TargetWhoAmI
        {
            get => (int)projectile.ai[1];
            set => projectile.ai[1] = value;
        }
        private const int MAX_STICKY_JAVELINS = 6; // This is the max. amount of javelins being able to attach
        private readonly Point[] _stickingJavelins = new Point[MAX_STICKY_JAVELINS]; // The point array holding for sticking javelins
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            // For going through platforms and such, javelins use a tad smaller size
            width = height = 10; // notice we set the width to the height, the height to 10. so both are 10
            return true;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Inflate some target hitboxes if they are beyond 8,8 size
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            // Return if the hitboxes intersects, which means the javelin collides or not
            return projHitbox.Intersects(targetHitbox);
        }
        public override void AI()
        {

            if (!DMGsave)
            {
                savedDMG = projectile.damage;
                DMGsave = true;
            }

            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, DustID.AmberBolt, projectile.oldVelocity.X * 0.2f, projectile.oldVelocity.Y * 0.2f, 1, new Color(), 0.8f);
            }
            if (HasHit > 0 || HasHitGround)
            {
                projectile.scale = projectile.scale + 0.0017f;
                if (++projectile.frameCounter >= AniTimer)
                {
                    projectile.frameCounter = 0;
                    if (++projectile.frame >= Main.projFrames[projectile.type])
                    {
                        AniTimer /= 2;
                        projectile.frame = 0;
                    }
                    if (AniTimer < 4)
                    {
                        AniTimer = 4;
                    }
                }
            }



            UpdateAlpha();
            // Run either the Sticky AI or Normal AI
            // Separating into different methods helps keeps your AI clean
            if (IsStickingToTarget) StickyAI();
            else NormalAI();
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            IsStickingToTarget = true; // we are sticking to a target
            TargetWhoAmI = target.whoAmI; // Set the target whoAmI
            projectile.velocity =
                (target.Center - projectile.Center) *
                0.75f; // Change velocity based on delta center of targets (difference between entity centers)
            projectile.netUpdate = true; // netUpdate this javelin
                                         // It is recommended to split your code into separate methods to keep code clean and clear
            projectile.damage = 0; // Makes sure the sticking javelins do not deal damage anymore

            UpdateStickyJavelins(target);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.tileCollide = false;
            HasHit++;
            projectile.timeLeft = 150;
            target.immune[projectile.owner] = 0;
        }
        private void UpdateStickyJavelins(NPC target)
        {
            int currentJavelinIndex = 0; // The javelin index

            for (int i = 0; i < Main.maxProjectiles; i++) // Loop all projectiles
            {
                Projectile currentProjectile = Main.projectile[i];
                if (i != projectile.whoAmI // Make sure the looped projectile is not the current javelin
                    && currentProjectile.active // Make sure the projectile is active
                    && currentProjectile.owner == Main.myPlayer // Make sure the projectile's owner is the client's player
                    && currentProjectile.type == projectile.type // Make sure the projectile is of the same type as this javelin
                    && currentProjectile.modProjectile is AmoungUsExplosive javelinProjectile // Use a pattern match cast so we can access the projectile like an ExampleJavelinProjectile
                    && javelinProjectile.IsStickingToTarget // the previous pattern match allows us to use our properties
                    && javelinProjectile.TargetWhoAmI == target.whoAmI)
                {

                    _stickingJavelins[currentJavelinIndex++] = new Point(i, currentProjectile.timeLeft); // Add the current projectile's index and timeleft to the point array
                    if (currentJavelinIndex >= _stickingJavelins.Length)  // If the javelin's index is bigger than or equal to the point array's length, break
                        break;
                }
            }
            // Remove the oldest sticky javelin if we exceeded the maximum
            if (currentJavelinIndex >= MAX_STICKY_JAVELINS)
            {
                int oldJavelinIndex = 0;
                // Loop our point array
                for (int i = 1; i < MAX_STICKY_JAVELINS; i++)
                {
                    // Remove the already existing javelin if it's timeLeft value (which is the Y value in our point array) is smaller than the new javelin's timeLeft
                    if (_stickingJavelins[i].Y < _stickingJavelins[oldJavelinIndex].Y)
                    {
                        oldJavelinIndex = i; // Remember the index of the removed javelin
                    }
                }
                // Remember that the X value in our point array was equal to the index of that javelin, so it's used here to kill it.
                Main.projectile[_stickingJavelins[oldJavelinIndex].X].Kill();
            }
        }
        private void UpdateAlpha()
        {
            // Slowly remove alpha as it is present
            if (projectile.alpha > 0)
            {
                projectile.alpha -= ALPHA_REDUCTION;
            }
            // If alpha gets lower than 0, set it to 0
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
        }
        private const int MAX_TICKS = 45;
        // Change this number if you want to alter how the alpha changes
        private const int ALPHA_REDUCTION = 25;

        private void NormalAI()
        {
            TargetWhoAmI++;

            // For a little while, the javelin will travel with the same speed, but after this, the javelin drops velocity very quickly.
            if (TargetWhoAmI >= MAX_TICKS && !HasHitGround)
            {
                // Change these multiplication factors to alter the javelin's movement change after reaching maxTicks
                const float velXmult = 0.98f; // x velocity factor, every AI update the x velocity will be 98% of the original speed
                const float velYmult = 0.35f; // y velocity factor, every AI update the y velocity will be be 0.35f bigger of the original speed, causing the javelin to drop to the ground
                TargetWhoAmI = MAX_TICKS; // set ai1 to maxTicks continuously
                projectile.velocity.X *= velXmult;
                projectile.velocity.Y += velYmult;
            }

            // Make sure to set the rotation accordingly to the velocity, and add some to work around the sprite's rotation
            // Please notice the MathHelper usage, offset the rotation by 90 degrees (to radians because rotation uses radians) because the sprite's rotation is not aligned!

            projectile.rotation = projectile.Center.X;

            // Spawn some random dusts as the javelin travels
        }
        private void StickyAI()
        {
            // These 2 could probably be moved to the ModifyNPCHit hook, but in vanilla they are present in the AI
            projectile.ignoreWater = true; // Make sure the projectile ignores water
            projectile.tileCollide = false; // Make sure the projectile doesn't collide with tiles anymore
            const int aiFactor = 15; // Change this factor to change the 'lifetime' of this sticking javelin
            projectile.localAI[0] += 1f;

            // Every 30 ticks, the javelin will perform a hit effect
            bool hitEffect = projectile.localAI[0] % 30f == 0f;
            int projTargetIndex = TargetWhoAmI;
            if (projectile.localAI[0] >= 60 * aiFactor || projTargetIndex < 0 || projTargetIndex >= 200)
            { // If the index is past its limits, kill it
                projectile.Kill();
            }
            else if (Main.npc[projTargetIndex].active && !Main.npc[projTargetIndex].dontTakeDamage)
            { // If the target is active and can take damage
              // Set the projectile's position relative to the target's center
                projectile.Center = Main.npc[projTargetIndex].Center - projectile.velocity * 2f;
                projectile.gfxOffY = Main.npc[projTargetIndex].gfxOffY;
                if (hitEffect)
                { // Perform a hit effect here
                    Main.npc[projTargetIndex].HitEffect(0, 1.0);
                }
            }
            else
            { // Otherwise, kill the projectile
                projectile.Kill();
            }

        }

        public override void Kill(int timeLeft)
        {
            Vector2 eee = projectile.Center;
            Vector2 value1 = new Vector2(0f, 0f);
            Main.PlaySound(SoundID.Item64, (int)eee.X, (int)eee.Y);
            int explode = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value1.X, value1.Y, ProjectileID.Grenade, savedDMG * 3, 3f, projectile.owner, 0f);
            Main.projectile[explode].timeLeft = 0;
            {
                Dust.NewDust(eee, 5, 5, DustID.Fire, 0.0f, 0.0f, 120, new Color(), 0.8f);  //this is the dust that will spawn after the explosion
            }
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            {
                projectile.velocity.Y = 0;
                HasHitGround = true;
                projectile.velocity.X = 0;
            }
            return false;
        }
    }
}