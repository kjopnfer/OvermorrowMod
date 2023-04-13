using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories.CapturedMirage
{
    public class MirageDummyProjectile : ModProjectile
    {
        public int mirageArrow;
        public override string Texture => AssetDirectory.Empty;
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;
        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 10;
        }

        public override void Kill(int timeLeft)
        {
            Projectile mirage = Projectile.NewProjectileDirect(null, Projectile.Center, Projectile.velocity, mirageArrow, Projectile.damage, Projectile.knockBack, Projectile.owner);
            mirage.GetGlobalProjectile<OvermorrowGlobalProjectile>().IsMirageArrow = true;
        }
    }
}