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
        protected Entity target = null;
        public ref float AIState => ref NPC.ai[0];
        public ref float AICounter => ref NPC.ai[1];

        /// <summary>
        /// Used to determine when the hitbox can damage the player. Also allows for extra damage frames inbetween AICases.
        /// </summary>
        protected int activeHitboxCount = 0;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return activeHitboxCount > 0;
        }

        public sealed override void SetDefaults()
        {
            NPC.aiStyle = -1;
            SafeSetDefaults();
        }

        public virtual void SafeSetDefaults() { }
        public void FaceTarget()
        {
            if (target == null) return;

            if (target.Center.X < NPC.Center.X) NPC.direction = -1;
            else NPC.direction = 1;
        }
    }
}