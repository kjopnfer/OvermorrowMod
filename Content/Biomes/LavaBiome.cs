using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.Tiles.Underground;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Biomes
{
    public class LavaBiome : ModBiome
    {
        public override bool IsBiomeActive(Player player)
        {
            return ModContent.GetInstance<BiomeTileCounts>().LavaBiome > 10;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            //for (int k = (int)Math.Floor(player.position.X / 16) - 55; k < (int)Math.Floor(player.position.X / 16) + 55; k++)
            //{
            //    for (int i = (int)Math.Floor(player.position.Y / 16) - 30; i < (int)Math.Floor(player.position.Y / 16) + 30; i++)
            //    {
            //        if (!Main.tile[k, i - 1].HasTile
            //            && !Main.tile[k, i - 2].HasTile
            //            && Main.tile[k, i].HasTile
            //            && Main.tile[k, i].TileType == ModContent.TileType<CrunchyStone>()
            //            && Main.rand.NextBool(95))
            //        {
            //            Particle.CreateParticle(Particle.ParticleType<Smoke2>(), new Vector2((k - 2) * 16, (i - 1) * 16), Vector2.Zero, Color.White);
            //
            //            /*if (Main.rand.NextBool(95))
            //            {
            //                int Index = Dust.NewDust(new Vector2((k - 2) * 16, (i - 1) * 16), 5, 5, ModContent.DustType<Steam>(), 0, 0, 0, default, Main.rand.NextFloat(0.5f, 1f));
            //                if (ZoneLava)
            //                {
            //                    Main.dust[Index].velocity.Y += 0.09f;
            //                }
            //            }*/
            //        }
            //    }
            //}
        }
    }
}
