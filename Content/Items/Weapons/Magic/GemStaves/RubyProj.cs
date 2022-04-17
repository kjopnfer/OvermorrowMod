using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.GemStaves
{
    public class RubyProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ruby");
        }
        public override bool? CanDamage() => true;

        float SpearTargetX = 0;
        float SpearTargetY = 0;
        int tf2 = 0;
        private int savedDMG = 0;
        int HasHit = 0;
        private int timer = 0;
        private int penet = 0;
        private int penet2 = 0;
        private int penet3 = 0;


        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 2;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 200;

        }
        public bool IsStickingToTarget
        {
            get => Projectile.ai[0] == 1f;
            set => Projectile.ai[0] = value ? 1f : 0f;
        }
        // Index of the current target
        public int TargetWhoAmI
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        private const int MAX_STICKY_JAVELINS = 6; // This is the max. amount of javelins being able to attach
        private readonly Point[] _stickingJavelins = new Point[MAX_STICKY_JAVELINS]; // The point array holding for sticking javelins

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
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

            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.HeartCrystal, Projectile.oldVelocity.X * 0, Projectile.oldVelocity.Y * 0, 1, new Color(), 1f);
            }

            if (penet3 > 2)
            {
                Projectile.Kill();
            }


            timer++;
            if (timer == 1)
            {
                savedDMG = Projectile.damage;
            }

            if (HasHit > 0)
            {
                SpearTargetX = Projectile.Center.X;
                SpearTargetY = Projectile.Center.Y;
            }
            UpdateAlpha();
            // Run either the Sticky AI or Normal AI
            // Separating into different methods helps keeps your AI clean


            if (IsStickingToTarget) StickyAI();
            else NormalAI();

        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (HasHit == 0)
            {
                IsStickingToTarget = true; // we are sticking to a target
                TargetWhoAmI = target.whoAmI; // Set the target whoAmI
                Projectile.velocity =
                    (target.Center - Projectile.Center) *
                    0.75f; // Change velocity based on delta center of targets (difference between entity centers)
                Projectile.netUpdate = true; // netUpdate this javelin
                                             // It is recommended to split your code into separate methods to keep code clean and clear // Makes sure the sticking javelins do not deal damage anymore

                UpdateStickyJavelins(target);
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.tileCollide = false;
            tf2 = target.defense;
            HasHit++;
            penet++;
        }
        private void UpdateStickyJavelins(NPC target)
        {
            int currentJavelinIndex = 0; // The javelin index

            for (int i = 0; i < Main.maxProjectiles; i++) // Loop all Projectiles
            {
                Projectile currentProjectile = Main.projectile[i];
                if (i != Projectile.whoAmI // Make sure the looped Projectile is not the current javelin
                    && currentProjectile.active // Make sure the Projectile is active
                    && currentProjectile.owner == Main.myPlayer // Make sure the Projectile's owner is the client's player
                    && currentProjectile.type == Projectile.type // Make sure the Projectile is of the same type as this javelin
                    && currentProjectile.ModProjectile is RubyProj javelinProjectile // Use a pattern match cast so we can access the Projectile like an ExampleJavelinProjectile
                    && javelinProjectile.IsStickingToTarget // the previous pattern match allows us to use our properties
                    && javelinProjectile.TargetWhoAmI == target.whoAmI)
                {

                    _stickingJavelins[currentJavelinIndex++] = new Point(i, currentProjectile.timeLeft); // Add the current Projectile's index and timeleft to the point array
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
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= ALPHA_REDUCTION;
            }
            // If alpha gets lower than 0, set it to 0
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
        }
        private const int MAX_TICKS = 45;
        // Change this number if you want to alter how the alpha changes
        private const int ALPHA_REDUCTION = 25;

        private void NormalAI()
        {
            TargetWhoAmI++;

            // For a little while, the javelin will travel with the same speed, but after this, the javelin drops velocity very quickly.
            if (TargetWhoAmI >= MAX_TICKS)
            {
                TargetWhoAmI = MAX_TICKS; // set ai1 to maxTicks continuously
            }

            // Make sure to set the rotation accordingly to the velocity, and add some to work around the sprite's rotation
            // Please notice the MathHelper usage, offset the rotation by 90 degrees (to radians because rotation uses radians) because the sprite's rotation is not aligned!
            Projectile.rotation =
                Projectile.velocity.ToRotation() +
                MathHelper.ToRadians(90f);

            // Spawn some random dusts as the javelin travels
        }
        private void StickyAI()
        {
            // These 2 could probably be moved to the ModifyNPCHit hook, but in vanilla they are present in the AI
            Projectile.ignoreWater = true; // Make sure the Projectile ignores water
            Projectile.tileCollide = false; // Make sure the Projectile doesn't collide with tiles anymore
            const int aiFactor = 15; // Change this factor to change the 'lifetime' of this sticking javelin
            Projectile.localAI[0] += 1f;


            // Every 30 ticks, the javelin will perform a hit effect
            bool hitEffect = Projectile.localAI[0] % 30f == 0f;
            int projTargetIndex = TargetWhoAmI;
            if (Projectile.localAI[0] >= 60 * aiFactor || projTargetIndex < 0 || projTargetIndex >= 200)
            { // If the index is past its limits, kill it
                Projectile.Kill();
            }
            else if (Main.npc[projTargetIndex].active && !Main.npc[projTargetIndex].dontTakeDamage)
            { // If the target is active and can take damage
              // Set the Projectile's position relative to the target's center
                Projectile.Center = Main.npc[projTargetIndex].Center - Projectile.velocity * 2f;
                Projectile.gfxOffY = Main.npc[projTargetIndex].gfxOffY;
                if (hitEffect)
                { // Perform a hit effect here
                    Main.npc[projTargetIndex].HitEffect(0, 1.0);
                }

                if (penet > 0)
                {
                    Projectile.damage = 0;
                    penet++;
                }

                if (Projectile.damage == 0)
                {
                    penet2++;
                }


                if (penet2 > 50)
                {
                    penet = 0;
                    penet2 = 0;
                    Main.npc[projTargetIndex].life -= savedDMG;
                    CombatText.NewText(Main.npc[projTargetIndex].getRect(), Color.Orange, savedDMG);
                    penet3++;
                }


            }
            else
            { // Otherwise, kill the Projectile
                Projectile.Kill();
            }
        }
    }
}
