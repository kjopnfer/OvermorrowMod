using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using OvermorrowMod.Core;

namespace OvermorrowMod.Content.NPCs.Mercenary.Paladin
{
    public partial class Paladin : Mercenary
    {
        public class PaladinDrawHelper : MercenaryDrawHelper
        {
            public const int Paladin = 0;
            public override Texture2D SetTex { get => ModContent.Request<Texture2D>(AssetDirectory.NPC + "Mercenary/Paladin/PaladinSpriteSheet").Value; }
            public override Point Frame { get => new Point(38, 33); }
            public static PaladinDrawHelper Helper() => (PaladinDrawHelper)OvermorrowMod.Common.OvermorrowModFile.Instance.drawHelpers[Paladin];
        }

        /// <summary>
        /// Checks if the NPC's velocity is above 0 and the previous direction is the same as the current one, return true
        /// </summary>
        /// <returns>A boolean value determining if the NPC is moving the same direction</returns>
        private bool SameDirection() => NPC.velocity.X != 0 && NPC.direction == NPC.oldDirection;

        /// <summary>
        /// Shorthand for return the shorthand of the mod instance of the paladin draw helper
        /// </summary>
        /// <returns></returns>
        PaladinDrawHelper Helper() => PaladinDrawHelper.Helper();

        /// <summary>
        /// Returns the thrown hammer attack projectile
        /// </summary>
        /// <returns>Returns the hammer projectile if found, otherwise returns null</returns>
        private PaladinHammer HammerAlive()
        {
            foreach (Projectile p in Main.projectile)
            {
                if (p != null && p.active && p.type == ModContent.ProjectileType<PaladinHammer>())
                {
                    PaladinHammer hammer = p.ModProjectile as PaladinHammer;
                    if (hammer.owner == this) return hammer;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the hammer slam shockwave projectile
        /// </summary>
        /// <returns>Returns the hammer slam projectile if found, otherwise returns null</returns>
        private PaladinHammerHit Shockwave()
        {
            foreach (Projectile p in Main.projectile)
            {
                if (p != null && p.active && p.type == ModContent.ProjectileType<PaladinHammerHit>())
                {
                    PaladinHammerHit hammer = p.ModProjectile as PaladinHammerHit;
                    if (hammer.owner == this) return hammer;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the hammer spin projectile
        /// </summary>
        /// <returns>Returns the spinning projectile if found, otherwise returns null</returns>
        private PaladinHammerSpin Spinning()
        {
            foreach (Projectile p in Main.projectile)
            {
                if (p != null && p.active && p.type == ModContent.ProjectileType<PaladinHammerSpin>())
                {
                    PaladinHammerSpin hammer = p.ModProjectile as PaladinHammerSpin;
                    if (hammer.owner == this)
                        return hammer;
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if the paladin is ramming by detecting if the projectile is active
        /// </summary>
        /// <returns>Returns the ramming projectile if found, otherwise returns false</returns>
        private bool IsRamming()
        {
            foreach (Projectile p in Main.projectile)
            {
                if (p != null && p.active && p.type == ModContent.ProjectileType<PaladinRamHitbox>())
                {
                    PaladinRamHitbox hammer = p.ModProjectile as PaladinRamHitbox;
                    if (hammer.owner == this)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if there is a solid tile directly below the NPC
        /// </summary>
        /// <returns>Returns the instance of the tile if found, otherwise returns an empty tile</returns>
        private Tile OnSolidTile()
        {
            Point checktile = new Point(MathFunctions.AGF.Round(NPC.Center.X) / 16, MathFunctions.AGF.Round(NPC.Center.Y + 32) / 16);
            Tile tile = Main.tile[checktile.X, checktile.Y];
            if (!WorldGen.TileEmpty(checktile.X, checktile.Y) && (Main.tileSolidTop[tile.TileType] || Main.tileSolid[tile.TileType]))
                return tile;

            return new Tile();
        }
    }
}