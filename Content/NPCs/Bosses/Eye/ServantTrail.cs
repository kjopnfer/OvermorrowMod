using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.Eye
{
    /// <summary>
    /// The current primitive system doesn't support adding trails onto vanilla NPCs so this is a band-aid fix for that
    /// </summary>
    public class ServantTrail : ModProjectile, ITrailEntity
    {
        public Color TrailColor(float progress) => Color.Black;
        public float TrailSize(float progress) => 30;
        public Type TrailType() => typeof(LightningTrail);
        public override string Texture => AssetDirectory.Empty;
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 420;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override void AI()
        {
            NPC parent = Main.npc[(int)Projectile.ai[0]];

            if (parent.active && parent.type == NPCID.ServantofCthulhu)
            {
                Projectile.timeLeft = 5;
            }

            Projectile.Center = parent.Center;
            Projectile.velocity = parent.velocity * 3;
        }
    }
}