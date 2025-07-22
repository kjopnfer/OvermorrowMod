using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.Interfaces;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    public class ChaliceHealth : ModProjectile, IOutlineEntity
    {
        public override string Texture => AssetDirectory.ArchiveProjectiles + Name;

        public bool ShouldDrawOutline => true;
        public Color OutlineColor => Color.Teal;
        public Color FillColor => Color.Black;
        public bool UseFillColor => true;

        public override void SetStaticDefaults()
        {
            Projectile.width = Projectile.height = 28;
            Projectile.timeLeft = ModUtils.SecondsToTicks(5);
            Projectile.tileCollide = false;
        }

        private float baseScale;
        public override void OnSpawn(IEntitySource source)
        {
            baseScale = Main.rand.NextFloat(0.2f, 2f);
            Projectile.scale = baseScale;
        }

        public override void AI()
        {
            float pulsateSpeed = 0.08f;
            float pulsateAmount = 0.3f;

            float pulsate = 1f + (float)Math.Sin(Projectile.timeLeft * pulsateSpeed) * pulsateAmount;
            Projectile.scale = baseScale * pulsate;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}