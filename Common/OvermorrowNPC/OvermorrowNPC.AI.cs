using Microsoft.Xna.Framework;
using OvermorrowMod.Content.NPCs;
using OvermorrowMod.Core.NPCs;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public abstract partial class OvermorrowNPC : ModNPC
    {
        public ref float AICounter => ref NPC.ai[0];
        public ref float IdleCounter => ref NPC.ai[1];

        public AIStateMachine AIStateMachine = null;

        /// <summary>
        /// This NPC's per-instance behavioral bias, rolled once at spawn.
        /// </summary>
        public Personality Personality { get; private set; }

        /// <summary>
        /// Per-type roll ranges for this NPC's personality axes.
        /// </summary>
        protected virtual PersonalityProfile PersonalityRanges => new PersonalityProfile();

        // TODO: Make these abstract instead.
        public virtual List<BaseIdleState> InitializeIdleStates() => new List<BaseIdleState> {
                new Wander(this)
        };

        public virtual List<BaseAttackState> InitializeAttackStates() => new List<BaseAttackState> {
                new GroundDashAttack(this)
        };

        public virtual List<BaseMovementState> InitializeMovementStates() => new List<BaseMovementState> {
                new MeleeWalk(this)
        };

        public virtual List<BaseDefenseState> InitializeDefenseStates() => new List<BaseDefenseState>();

        /// <summary>
        /// Whether this NPC should be supported by custom collider surfaces.
        /// </summary>
        public virtual bool UsesCustomGrounding => !NPC.noGravity;

        /// <summary>
        /// Whether this NPC can be put into hit-stun.
        /// </summary>
        public virtual bool CanBeStunned => true;

        /// <summary>
        /// True while the FSM is in StunnedState.
        /// </summary>
        public bool IsStunned => AIStateMachine?.GetCurrentState() is StunnedState;

        /// <summary>
        /// Applies a hit-stun for the given tick window. Calling again refreshes the duration.
        /// </summary>
        public void Stun(int ticks)
        {
            if (!CanBeStunned) return;

            AICounter = ticks;
            IdleCounter = 0;
            NPC.velocity.X = 0;
            AIStateMachine?.ForceChangeState(AIStateType.Stunned, this);
        }

        /// <summary>
        /// Y of the collider surface supporting this NPC, or null when unsupported.
        /// </summary>
        public float? CurrentSupportY { get; private set; }

        internal void SetCurrentSupportY(float? y) => CurrentSupportY = y;

        private bool dropThroughActive;
        private float dropFromY;
        private int dropThroughTicks;
        private bool dropThroughOriginalNoTileCollide;
        private int nextDropAllowedTick;

        internal bool DropThroughActive => dropThroughActive;

        /// <summary>
        /// Drops the NPC through the one-way surface it is standing on. Rate-limited.
        /// </summary>
        public void RequestDropThrough()
        {
            if (dropThroughActive) return;
            if (Main.GameUpdateCount < (uint)nextDropAllowedTick) return;

            float dropY;
            if (CurrentSupportY.HasValue)
            {
                dropY = CurrentSupportY.Value;
            }
            else
            {
                Point feetTile = (NPC.Bottom + new Vector2(0, 1)).ToTileCoordinates();
                Tile tile = Framing.GetTileSafely(feetTile.X, feetTile.Y);
                if (!tile.HasTile || !Main.tileSolidTop[tile.TileType]) return;
                dropY = NPC.Bottom.Y;
            }

            dropFromY = dropY;
            dropThroughActive = true;
            dropThroughTicks = 3;
            dropThroughOriginalNoTileCollide = NPC.noTileCollide;
        }

        /// <summary>
        /// Advances drop-through state by one tick.
        /// </summary>
        internal void UpdateDropThrough()
        {
            if (!dropThroughActive) return;

            NPC.noTileCollide = true;
            dropThroughTicks--;
            if (dropThroughTicks <= 0 || NPC.Bottom.Y > dropFromY + 4f)
            {
                dropThroughActive = false;
                NPC.noTileCollide = dropThroughOriginalNoTileCollide;
                nextDropAllowedTick = (int)Main.GameUpdateCount + 30;
            }
        }
    }
}