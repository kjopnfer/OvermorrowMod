using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using OvermorrowMod.Common.NPCs;
using OvermorrowMod.Content.Tiles.WaterCave;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public static class ILEdits
    {
        public static void Load()
        {
            IL.Terraria.Main.UpdateAudio += TitleMusic;
            IL.Terraria.Main.UpdateAudio += TitleDisable;
            IL.Terraria.Projectile.VanillaAI += GrappleCollision;
            //IL.Terraria.Liquid.Update += UpdateWater;
            //IL.Terraria.Liquid.QuickWater += QuickWater;
        }

        public static void Unload()
        {
            IL.Terraria.Main.UpdateAudio -= TitleMusic;
            IL.Terraria.Main.UpdateAudio -= TitleDisable;
            IL.Terraria.Projectile.VanillaAI -= GrappleCollision;
            //IL.Terraria.Liquid.Update -= UpdateWater;
            //IL.Terraria.Liquid.QuickWater -= QuickWater;
        }

        private static void TitleMusic(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(instr => instr.MatchLdcI4(6) && instr.Next.MatchStfld(typeof(Main).GetField("newMusic"))))
            {
                throw new Exception("Title music patch failed lol");
            }
            c.Index++;
            c.EmitDelegate<Func<int, int>>(TitleMusicDelegate);
        }

        public static int TitleMusicDelegate(int oldMusic)
        {
            // Can also add mod checks for music here and return oldMusic if not active
            return OvermorrowModFile.Instance.GetSoundSlot((SoundType)51, "Sounds/Music/SandstormBoss");
        }

        // If you don't include the following two methods, the game slaps your volume down to zero
        public static int TitleDisableDelegate(int oldValue) => 0;
        public static void TitleDisable(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(instr => instr.MatchLdsfld<Main>("musicError") && instr.Next.MatchLdcI4(100)))
            {
                throw new Exception("Title music disable failed");
            }
            c.Index++;
            c.EmitDelegate<Func<int, int>>(TitleDisableDelegate);
        }

        // This works in tandem with setting ai[0] = 2f in the AI in order to enter the pull check upon hitbox intersection
        public static void GrappleCollision(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.TryGotoNext(i => i.MatchLdfld<Projectile>("aiStyle"), i => i.MatchLdcI4(7));
            c.TryGotoNext(i => i.MatchLdfld<Projectile>("ai"), i => i.MatchLdcI4(0), i => i.MatchLdelemR4(), i => i.MatchLdcR4(2));
            c.TryGotoNext(i => i.MatchLdloc(143)); //flag2 in source code
            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<GrappleDelegate>(EmitGrappleDelegate);
            c.TryGotoNext(i => i.MatchStfld<Player>("grapCount"));
            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<UngrappleDelegate>(EmitUngrappleDelegate);
        }

        private delegate bool GrappleDelegate(bool fail, Projectile proj);
        private static bool EmitGrappleDelegate(bool fail, Projectile proj)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.modNPC is CollideableNPC && npc.Hitbox.Intersects(proj.Hitbox))
                {
                    //Main.NewText("test");

                    proj.velocity = Vector2.Zero;
                    proj.tileCollide = true;
                    //proj.position += npc.velocity;

                    return false;
                }
            }

            return fail;
        }

        private delegate void UngrappleDelegate(Projectile proj);
        private static void EmitUngrappleDelegate(Projectile proj)
        {
            Player player = Main.player[proj.owner];
            int numHooks = 3;

            switch (proj.type)
            {
                case 165:
                    numHooks = 8;
                    break;
                case 256:
                case 372:
                    numHooks = 2;
                    break;
                case 652:
                    numHooks = 1;
                    break;
                case 646:
                case 647:
                case 648:
                case 649:
                    numHooks = 4;
                    break;
            }

            ProjectileLoader.NumGrappleHooks(proj, player, ref numHooks);
            if (player.grapCount > numHooks) Main.projectile[player.grappling.OrderBy(n => (Main.projectile[n].active ? 0 : 999999) + Main.projectile[n].timeLeft).ToArray()[0]].Kill();
        }

        public static void UpdateWater(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            for (int j = 0; j < 4; j++)
            {
                if (!c.TryGotoNext(i => i.MatchLdcI4(1)))
                {
                    OvermorrowModFile.Instance.Logger.Error("Failed to patch update water shit");
                    return;
                }
            }

            c.Index++;

            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<int, Liquid, int>>((value, Liquid) =>
            {
                Tile tile = Main.tile[Liquid.x, Liquid.y];
                if (tile.wall == ModContent.WallType<GlowWall>())
                {
                    Tile tile2 = Framing.GetTileSafely(Liquid.x, Liquid.y + 1);
                    if (tile2.wall == ModContent.WallType<GlowWall>())
                    {
                        value = -value;
                    }
                    else
                    {
                        value = 0;
                    }
                }
                return value;
            });

            if (!c.TryGotoNext(i => i.MatchLdcI4(1)))
            {
                OvermorrowModFile.Instance.Logger.Error("Failed to patch update water shit");
                return;
            }

            c.Index++;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<int, Liquid, int>>((value, Liquid) =>
            {
                Tile tile = Main.tile[Liquid.x, Liquid.y];
                if (tile.wall == ModContent.WallType<GlowWall>())
                {
                    Tile tile2 = Framing.GetTileSafely(Liquid.x, Liquid.y + 1);
                    if (tile2.wall == ModContent.WallType<GlowWall>())
                    {
                        value = -value;
                    }
                    else
                    {
                        value = 0;
                    }
                }
                return value;
            });
        }

        public static int Inverse(int value, int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (tile.wall == WallID.LunarBrickWall) value = -value;
            return value;
        }

        public static void QuickWater(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            // 11 ones before the one we want
            // 10 == b
            for (int j = 0; j < 10; j++)
            {
                if (!c.TryGotoNext(i => i.MatchLdcI4(1)))
                {
                    OvermorrowModFile.Instance.Logger.Error("Failed to patch quick water shit");
                    return;
                }
            }

            for (int j = 0; j < 9; j++)
            {
                if (!c.TryGotoNext(i => i.MatchLdcI4(1)))
                {
                    OvermorrowModFile.Instance.Logger.Error("Failed to patch quick water shit");
                    return;
                }
                c.Index++;
                // right after inputting one
                c.Emit(OpCodes.Ldloc_S, 13);
                c.Emit(OpCodes.Ldloc_S, 14);
                // takes the existing value, the x and the y
                c.EmitDelegate<Func<int, int, int, int>>(Inverse);
            }
            // 9 times
        }
    }
}