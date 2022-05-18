using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.Buffs.Debuffs;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.Eye
{
    public partial class EyeOfCthulhu : GlobalNPC
    {
        public void Transition(NPC npc, Player player)
        {
            if (TransitionPhase)
            {
                if (++npc.ai[1] % 15 == 0 && npc.ai[1] < 540)
                {

                    Vector2 RandomPosition = player.Center + new Vector2(Main.rand.Next(-9, 7) + 1, Main.rand.Next(-7, 5) + 1) * 75;
                    Projectile.NewProjectile(npc.GetSource_FromAI(), RandomPosition, Vector2.Zero, ModContent.ProjectileType<DarkEye>(), npc.damage, 0f, Main.myPlayer);

                    if (Main.rand.NextBool(4))
                    {
                        Projectile.NewProjectile(npc.GetSource_FromAI(), player.position, Vector2.Zero, ModContent.ProjectileType<DarkEye>(), npc.damage, 0f, Main.myPlayer);
                    }
                }

                if (npc.ai[1] >= /*870*/750)
                {

                    if (npc.ai[3] > 0)
                    {
                        npc.ai[3] -= 0.05f;
                    }
                    else
                    {
                        npc.dontTakeDamage = false;

                        npc.ai[0] = (float)AIStates.Selector;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                    }
                }
            }
        }
    }
}