using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using System.Diagnostics;

namespace OvermorrowMod.Content.Items.Weapons.Melee
{
	public class SmallUvShard : ModProjectile
	{
		public override string Texture => "OvermorrowMod/Content/Items/Weapons/Melee/UvDagger/SmallUvShard";
		public override void SetDefaults() 
		{
			Projectile.penetrate = 2;
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.alpha = 0;
			Projectile.friendly = true;
			Projectile.aiStyle = -1;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = true; //false
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 35;
			Projectile.scale = 1.3f;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;	
			
		}
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			Projectile.velocity = -oldVelocity;
			return false;
        }
        public override void AI() 
		{
			Projectile.rotation += 0.4f;
			Projectile.velocity *=   .90f;
			Projectile.scale -= .03f;
		}			
	}
}