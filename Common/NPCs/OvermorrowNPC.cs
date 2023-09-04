using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.NPCs
{
    public abstract class OvermorrowNPC : ModNPC
    {
        protected int frame = 0;
        protected int frameTimer = 0;

        protected Player player => Main.player[NPC.target];
        public ref float AIState => ref NPC.ai[0];
        public ref float AICounter => ref NPC.ai[1];
    }
}