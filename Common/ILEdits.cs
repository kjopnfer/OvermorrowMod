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
            //IL.Terraria.Projectile.AI_007_GrapplingHooks += GrappleCollision;
            //IL.Terraria.Liquid.Update += UpdateWater;
            //IL.Terraria.Liquid.QuickWater += QuickWater;
        }

        public static void Unload()
        {
            //IL.Terraria.Projectile.AI_007_GrapplingHooks -= GrappleCollision;

            //IL.Terraria.Liquid.Update -= UpdateWater;
            //IL.Terraria.Liquid.QuickWater -= QuickWater;
        }

        // This works in tandem with setting ai[0] = 2f in the AI in order to enter the pull check upon hitbox intersection
        public static void GrappleCollision(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            // this.ai[0] == 0f
            c.TryGotoNext(i => i.MatchLdfld<Projectile>("ai"), i => i.MatchLdcI4(0), i => i.MatchLdelemR4(), i => i.MatchLdcR4(0));
            // Call to tile.nactive()
            c.TryGotoNext(i => i.MatchCall<Tile>("nactive"));
            c.Index--;

            // This first section essentially adds HooksNPC(this) || ... to the start of the condition for checking whether
            // a tile is grappleable.
            // Inject some code that skips the condition checks for whether to hook onto a tile.
            var label = il.DefineLabel();
            // Load "this" on the stack.
            c.Emit(OpCodes.Ldarg_0);
            // Pop "this", then push the result of HooksNPC to the stack
            c.EmitDelegate(HooksNPC);
            // If the result is "true", goto label
            c.Emit(OpCodes.Brtrue, label);
            // Matches a later part of the if (Main.player[this.owner].grapCount < 10) line
            c.TryGotoNext(i => i.MatchLdfld<Player>("grapCount"));
            // Step back so that we are just inside the condition
            c.Index -= 4;
            // Set a label here.
            c.MarkLabel(label);

            // This bit lets us modify the value of the "flag" variable, and also modify "this" a bit each tick when ai[0] is 2 (attached)
            // Skip to last check of AI_007_GrapplingHooks_CanTileBeLatchedOnTo
            c.TryGotoNext(i => i.MatchCall<Projectile>("AI_007_GrapplingHooks_CanTileBeLatchedOnTo"));
            // Jump down a few instructions to get right after loading flag onto the stack
            c.Index += 5;
            // Load "this" on the stack
            c.Emit(OpCodes.Ldarg_0);
            // Pop "this", then push the result of ShouldReleaseHook. The previous instruction loads "flag" on the stack, and the next
            // jumps if it is false, so this effectively lets us replace "if (flag) {" with "if (ShouldReleaseHook(flag, this)) {"
            c.EmitDelegate(ShouldReleaseHook);
        }
        /// <summary>
        /// Return true from this method to override the check for grappling,
        /// return false for vanilla behavior.
        /// </summary>
        /// <param name="proj">Projectile to test</param>
        /// <returns>True if we should attach the grapple, independent of vanilla rules.</returns>
        private static bool HooksNPC(Projectile proj)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active && npc.ModNPC is CollideableNPC && npc.Hitbox.Intersects(proj.Hitbox))
                {
                    proj.velocity = Vector2.Zero;
                    proj.tileCollide = true;
                    ((CollideableNPC)npc.ModNPC).Grappled = true;
                    proj.ai[0] = 2f;
                    proj.ai[1] = i;
                    proj.position += npc.velocity;
                    proj.netUpdate = true;
                    //proj.position += npc.velocity;

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Return true from this method to release the hook, false to not release it.
        /// To not make a decision one way or another, return flag.
        /// </summary>
        /// <param name="flag">Current status of whether or not to test.</param>
        /// <param name="proj">Projectile to test for</param>
        /// <returns>True if we should release the grapple, false to keep it attached.</returns>
        private static bool ShouldReleaseHook(bool flag, Projectile proj)
        {
            if (proj.ai[1] >= 0 && proj.ai[1] < Main.maxNPCs)
            {
                var npc = Main.npc[(int)proj.ai[1]];
                if (!npc.active || npc.ModNPC is not CollideableNPC cnpc) return flag;
                proj.position += npc.velocity;
                proj.netUpdate = true;
                return false;
            }

            return flag;
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
                if (tile.WallType == ModContent.WallType<GlowWall>())
                {
                    Tile tile2 = Framing.GetTileSafely(Liquid.x, Liquid.y + 1);
                    if (tile2.WallType == ModContent.WallType<GlowWall>())
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
                if (tile.WallType == ModContent.WallType<GlowWall>())
                {
                    Tile tile2 = Framing.GetTileSafely(Liquid.x, Liquid.y + 1);
                    if (tile2.WallType == ModContent.WallType<GlowWall>())
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
            if (tile.WallType == WallID.LunarBrickWall) value = -value;
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