using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Dusts
{
    public class Blood : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.scale = 1.5f;
            dust.noGravity = false;
            dust.noLight = false;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.velocity.Y += 0.15f;

            Vector2 position = new Vector2(dust.position.X, dust.position.Y) / 16;
            if (Main.tile[(int)position.X, (int)position.Y].HasTile && Main.tile[(int)position.X, (int)position.Y].BlockType == Terraria.ID.BlockType.Solid && Main.tileSolid[Main.tile[(int)position.X, (int)position.Y].TileType])
            {
                dust.alpha += 5;
                dust.velocity *= -0.1f;
            }

            dust.rotation = dust.velocity.ToRotation();
            dust.scale *= 0.99f;

            if (dust.alpha > 240) dust.active = false;

            return false;
        }
    }
}