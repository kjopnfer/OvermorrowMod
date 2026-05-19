using CollisionLib;
using Microsoft.Xna.Framework;
using OvermorrowMod.Common.CustomCollision;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class WoodenLadderCollision : TileCollisionNPC
    {
        public override bool PreAI()
        {
            if (colliders == null)
            {
                // Single platform across the top rung. Stacked ladders each
                // spawn their own collider so the only walkable surface on a
                // chain is whichever ladder is topmost.
                colliders = new CollisionSurface[]
                {
                    new CollisionSurface(
                        NPC.TopLeft + new Vector2(0, 2),
                        NPC.TopRight + new Vector2(0, 2),
                        new int[] { CollisionID.Platform, 0, 0, 0 },
                        true),
                };
            }
            return true;
        }

        public override void AI()
        {
            if (colliders != null && colliders.Length > 0)
            {
                foreach (var collider in colliders)
                    collider.Update();
            }
        }

        public override void PostAI()
        {
            if (colliders != null)
            {
                foreach (CollisionSurface collider in colliders)
                    collider.PostUpdate();
            }
        }
    }
}
