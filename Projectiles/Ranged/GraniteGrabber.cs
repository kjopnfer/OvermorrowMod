using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Projectiles.Ranged
{
    public class GraniteGrabber : ModProjectile
    {

        private const string ChainTexturePath = "OvermorrowMod/NPCs/Biome/GranNPCChain";


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Grabber");
			Main.projFrames[base.projectile.type] = 3;
        }
        public override bool CanDamage() => true;



        private int SavedDMG = 0;
        private int timer = 0;
        private bool ComingBack = false;
        private bool StickingToNPC = false;
        private int flametimer = 0;


        private int ComingBackTimer = 0;



        readonly int Rota = 0;
        readonly int zimer = 0;
        float SpearTargetX = 0;
        float SpearTargetY = 0;
        readonly float Thank = 0;
        readonly float You = 0;
        int tf2 = 0;
		private int savedDMG = 0;
		int HasHit = 0;
		private int penet = 0;
		private int penet2 = 0;
		private int penet3 = 0;


		public override void SetDefaults()
        {
            projectile.width = 36;
            projectile.height = 36;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = 3;
            projectile.timeLeft = 2;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.timeLeft = 200;
            projectile.light = 1f;
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
			ComingBackTimer++;
            float BetweenBack = Vector2.Distance(Main.player[projectile.owner].Center, projectile.Center);
            if (!Main.player[projectile.owner].channel || BetweenBack > 375f || ComingBackTimer > 100 && !StickingToNPC)
			{
				ReturnBack();
			}




            if(ComingBack)
            {
                float BetweenKill = Vector2.Distance(Main.player[projectile.owner].Center, projectile.Center);
                projectile.tileCollide = false;
                Vector2 position = projectile.Center;
                Vector2 targetPosition = Main.player[projectile.owner].Center;
                Vector2 direction2 = targetPosition - position;
                direction2.Normalize();
                projectile.velocity = direction2 * 23;
                if(BetweenKill < 50f)
                {
				    projectile.Kill();    
                }
            }





            int direction = -1;

            if (projectile.Center.X < Main.player[projectile.owner].Center.X)
			{
                direction = 1;
			}

           	 	var player2 = Main.player[projectile.owner];

            	Vector2 mountedCenter2 = player2.Center;

            	var drawPosition2 = projectile.Center;
            	var remainingVectorToPlayer2 = mountedCenter2 - drawPosition2;

                int direction4 = -1;

                if (projectile.Center.X < mountedCenter2.X)
				{
                    direction4 = 1;
				}

                player2.itemRotation = (float)Math.Atan2(remainingVectorToPlayer2.Y * direction4, remainingVectorToPlayer2.X * direction4);

            projectile.timeLeft = 2;

			timer++;
			if (timer == 1)
			{
				savedDMG = projectile.damage;
			}


			if(HasHit > 0)
			{
				SpearTargetX = projectile.Center.X;
        		SpearTargetY = projectile.Center.Y;
			}


            if (++projectile.frameCounter >= 3)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
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
			if (HasHit == 0)
			{
				IsStickingToTarget = true; // we are sticking to a target
				TargetWhoAmI = target.whoAmI; // Set the target whoAmI
				projectile.velocity =
					(target.Center - projectile.Center) *
					0.75f; // Change velocity based on delta center of targets (difference between entity centers)
				projectile.netUpdate = true; // netUpdate this javelin
											 // It is recommended to split your code into separate methods to keep code clean and clear // Makes sure the sticking javelins do not deal damage anymore

				UpdateStickyJavelins(target);
			}
		}
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			projectile.tileCollide = false;
			projectile.damage = 0;
			HasHit++;
			penet++;
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
						&& currentProjectile.modProjectile is GraniteGrabber javelinProjectile // Use a pattern match cast so we can access the projectile like an ExampleJavelinProjectile
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
			if (projectile.alpha > 0) {
				projectile.alpha -= ALPHA_REDUCTION;
			}
			// If alpha gets lower than 0, set it to 0
			if (projectile.alpha < 0) {
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
			if (TargetWhoAmI >= MAX_TICKS) {
				// Change these multiplication factors to alter the javelin's movement change after reaching maxTicks
				const float velXmult = 0.98f; // x velocity factor, every AI update the x velocity will be 98% of the original speed
				const float velYmult = 0.98f; // y velocity factor, every AI update the y velocity will be be 0.35f bigger of the original speed, causing the javelin to drop to the ground
				TargetWhoAmI = MAX_TICKS; // set ai1 to maxTicks continuously
				projectile.velocity.X *= velXmult;
				projectile.velocity.Y *= velYmult;
			}

			// Make sure to set the rotation accordingly to the velocity, and add some to work around the sprite's rotation
			// Please notice the MathHelper usage, offset the rotation by 90 degrees (to radians because rotation uses radians) because the sprite's rotation is not aligned!
			if(!ComingBack)
			{
				projectile.rotation = (Main.player[projectile.owner].Center - projectile.Center).ToRotation() - MathHelper.ToRadians(90f);
			}


			// Spawn some random dusts as the javelin travels
		}
		private void StickyAI()
		{


				projectile.rotation = (Main.player[projectile.owner].Center - projectile.Center).ToRotation() - MathHelper.ToRadians(90f);


			if(!ComingBack)
			{
				StickingToNPC = true;
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
					ReturnBack();
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



				penet2++;
				Color Dcolor = Color.Blue;
				if (penet2 > 14)
				{
					penet = 0;
					penet2 = 0;
					Main.npc[projTargetIndex].StrikeNPC(savedDMG, 0, 0);
					penet3++;
				}

			}
				else
				{ // Otherwise, kill the projectile
					ReturnBack();
				}
			}

		}


		private void ReturnBack()
		{
			StickingToNPC = false;
			ComingBack = true;
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var player = Main.player[projectile.owner];

            Vector2 mountedCenter = player.Center;
            Texture2D chainTexture = ModContent.GetTexture(ChainTexturePath);

            var drawPosition = projectile.Center;
            var remainingVectorToPlayer = mountedCenter - drawPosition;

            float rotation = remainingVectorToPlayer.ToRotation() - MathHelper.PiOver2;

            if (projectile.alpha == 0)
            {
                int direction = -1;

                if (projectile.Center.X < mountedCenter.X)
                    direction = 1;

            }

            // This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
            while (true)
            {
                float length = remainingVectorToPlayer.Length();

                if (length < 25f || float.IsNaN(length))
                    break;

                // drawPosition is advanced along the vector back to the player by 12 pixels
                // 12 comes from the height of ExampleFlailProjectileChain.png and the spacing that we desired between links
                drawPosition += remainingVectorToPlayer * 28 / length;
                remainingVectorToPlayer = mountedCenter - drawPosition;

                // Finally, we draw the texture at the coordinates using the lighting information of the tile coordinates of the chain section
                Color color = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));
                spriteBatch.Draw(chainTexture, drawPosition - Main.screenPosition, null, color, (player.Center - projectile.Center).ToRotation(), chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            }

            return true;
        }
	}
}
