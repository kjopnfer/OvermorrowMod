using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Primitives.Trails;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Interfaces;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.GraniteLauncher
{
    public class GraniteElectricity : Lightning
    {
        public float maxTime = 15;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Electricity");
        }

        public override void SafeSetDefaults()
        {
            Projectile.width = 7;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = (int)maxTime;
            Length = 1f;
            Sine = true;
            Color1 = new Color(122, 232, 246);
            Color2 = new Color(0, 137, 255);
        }

        public override void AI()
        {
            Projectile firstNode = Main.projectile[(int)Projectile.ai[0]];
            Projectile lastNode = Main.projectile[(int)Projectile.ai[1]];

            if (firstNode.active && firstNode.ModProjectile is GraniteShard && lastNode.active && lastNode.ModProjectile is GraniteShard)
            {
                Projectile.timeLeft = 5;
            }

            Length = TRay.CastLength(Projectile.Center, Projectile.velocity, 2000f);
            float sway = 10f;
            float divider = 5f;
            float thickness = 10;
            Positions = CreateLightning(firstNode.Center, lastNode.Center, thickness, sway, divider);

            float progress = (maxTime - (float)Projectile.timeLeft) / maxTime;
            float mult = (float)Math.Sin(progress * Math.PI);
            for (int i = 0; i < Positions.Count; i++)
            {
                Positions[i].Size = Positions[i].DefSize * mult;
            }
        }
    }
}