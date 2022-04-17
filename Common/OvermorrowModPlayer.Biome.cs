using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.Tiles.Underground;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    /* public partial class OvermorrowModPlayer : ModPlayer
    {
        // Biome
        public bool ZoneWaterCave = false;
        public bool ZoneMarble = false;
        public bool ZoneGranite = false;
        public bool ZoneLava = false;

        public override void UpdateBiomes()
        {
            ZoneWaterCave = OvermorrowWorld.floodedCaves > 50;
            ZoneMarble = OvermorrowWorld.marbleBiome > 10;
            ZoneGranite = OvermorrowWorld.graniteBiome > 10;
            ZoneLava = OvermorrowWorld.lavaBiome > 10;
        }

        public override void UpdateBiomeVisuals()
        {
            DrawHaze();
        }

        public override void CopyCustomBiomesTo(Player other)
        {
            OvermorrowModPlayer modOther = other.GetModPlayer<OvermorrowModPlayer>();
            modOther.ZoneWaterCave = ZoneWaterCave;
            modOther.ZoneLava = ZoneLava;
        }

        public override void SendCustomBiomes(BinaryWriter writer)
        {
            BitsByte flags = new BitsByte();
            flags[0] = ZoneWaterCave;
            flags[1] = ZoneMarble;
            flags[2] = ZoneGranite;
            flags[3] = ZoneLava;
            writer.Write(flags);
        }

        public override void ReceiveCustomBiomes(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            ZoneWaterCave = flags[0];
            ZoneMarble = flags[1];
            ZoneGranite = flags[2];
            ZoneLava = flags[3];
        }

        private void DrawHaze()
        {
            for (int k = (int)Math.Floor(player.position.X / 16) - 55; k < (int)Math.Floor(player.position.X / 16) + 55; k++)
            {
                for (int i = (int)Math.Floor(player.position.Y / 16) - 30; i < (int)Math.Floor(player.position.Y / 16) + 30; i++)
                {
                    if (!Main.tile[k, i - 1].active() && !Main.tile[k, i - 2].active() && Main.tile[k, i].active() && Main.tile[k, i].type == ModContent.TileType<CrunchyStone>())
                    {
                        if (Main.rand.NextBool(95))
                        {
                            //int Index = Dust.NewDust(new Vector2((k - 2) * 16, (i - 1) * 16), 5, 5, DustType<Steam>());
                            if (ZoneLava)
                            {
                                Particle.CreateParticle(Particle.ParticleType<Smoke2>(), new Vector2((k - 2) * 16, (i - 1) * 16), Vector2.Zero, Color.White);
                            }
                        }

                        /*if (Main.rand.NextBool(95))
                        {
                            int Index = Dust.NewDust(new Vector2((k - 2) * 16, (i - 1) * 16), 5, 5, ModContent.DustType<Steam>(), 0, 0, 0, default, Main.rand.NextFloat(0.5f, 1f));
                            if (ZoneLava)
                            {
                                Main.dust[Index].velocity.Y += 0.09f;
                            }
                        }
                    }
                }
            }
        }
    } */
}