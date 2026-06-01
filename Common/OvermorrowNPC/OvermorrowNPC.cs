using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.RoomManager;
using OvermorrowMod.Content.NPCs;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Items;
using OvermorrowMod.Core.NPCs;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public abstract partial class OvermorrowNPC : ModNPC
    {
        public NPCTargetingModule TargetingModule { get; protected set; }

        public ref Player Player => ref Main.player[NPC.target];

        /// <summary>
        /// Defines a position in the world that the NPC can target.
        /// Thie position may be set to <see cref="Target"/>'s position,
        /// but it may also be used for idling behavior if no <see cref="Target"/> is defined.
        /// </summary>
        public Vector2? TargetPosition = null;

        /// <summary>
        /// Saves the ID of the Spawner if the NPC was created by one.
        /// </summary>
        public int? SpawnerID { get; set; } = null;

        /// <summary>
        /// World-space fallback anchor for idle behaviors when this NPC has no <see cref="NPCSpawnPoint"/>.
        /// Assigned by systems that create NPCs directly without a spawner.
        /// </summary>
        public Vector2? AnchorPosition { get; set; } = null;

        /// <summary>
        /// whoAmI of the CombatOrchestrator NPC that spawned this enemy as part of a combat wave.
        /// </summary>
        public int? CombatOrchestratorWhoAmI { get; set; } = null;

        /// <summary>
        /// Sound that plays when the NPC finds a target.
        /// </summary>
        public SoundStyle? AggroSound { get; set; } = null;

        /// <summary>
        /// Gets the associated NPCSpawnPoint if the NPC was created by a spawner.
        /// Returns null if no valid SpawnerID exists.
        /// </summary>
        public NPCSpawnPoint SpawnPoint => SpawnerID.HasValue && TileEntity.ByID.TryGetValue(SpawnerID.Value, out TileEntity entity)
            ? entity as NPCSpawnPoint
            : null;

        /// <summary>
        /// The world-space point that idle behaviors anchor to.
        /// Resolves to the spawn point when one exists, otherwise <see cref="AnchorPosition"/>.
        /// </summary>
        public Vector2? IdleAnchor => SpawnPoint != null ? SpawnPoint.Position.ToWorldCoordinates() : AnchorPosition;

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue(LocalizationPath.Bestiary + Name)),
            });
        }

        public virtual NPCTargetingConfig TargetingConfig() => new NPCTargetingConfig();
        public sealed override void SetDefaults()
        {
            TargetingModule = new NPCTargetingModule(NPC, TargetingConfig());
            AIStateMachine = new AIStateMachine(NPC.ModNPC as OvermorrowNPC, InitializeIdleStates(), InitializeMovementStates(), InitializeAttackStates(), InitializeDefenseStates());
            Personality = PersonalityRanges.Roll();

            SafeSetDefaults();

            NPC.GetGlobalNPC<BarrierNPC>().MaxBarrierPoints = (int)(NPC.lifeMax * 0.25f);
        }

        public virtual void SafeSetDefaults() { }

        protected virtual void DrawNPCBestiary(SpriteBatch spriteBatch, Color drawColor) { }

        /// <summary>
        /// Is called before <see cref="DrawOvermorrowNPC(SpriteBatch, Vector2, Color)"/>, which will always draw behind.
        /// The SpriteBatch calls here will not be captured by RenderTargets such as the NPCBarrierRenderer.
        /// </summary>
        public virtual void DrawBehindOvermorrowNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) { }

        /// <summary>
        /// The replacement for PreDraw. Everything drawn in here can be captured by a RenderTarget.
        /// </summary>
        public virtual bool DrawOvermorrowNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => true;
        public sealed override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                DrawNPCBestiary(spriteBatch, drawColor);
                return false;
            }

            DrawBehindOvermorrowNPC(spriteBatch, screenPos, drawColor);

            return DrawOvermorrowNPC(spriteBatch, screenPos, drawColor);
        }

        public sealed override bool PreAI()
        {
            // Prevent offscreen projectiles from killing the NPC.
            NPC.dontTakeDamage = !IsOnScreen();

            UpdateDropThrough();

            if (!DropThroughActive && CurrentSupportY.HasValue)
            {
                NPC.collideY = true;
            }

            TargetingModule.Update();
            UpdateCorneredState();

            return base.PreAI();
        }

        public sealed override void OnKill()
        {
            if (SpawnerID.HasValue && TileEntity.ByID.TryGetValue(SpawnerID.Value, out TileEntity entity) && entity is NPCSpawnPoint spawner)
            {
                spawner.SetSpawnerCleared();
            }
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            RecordDamage(damageDone);

            if (!TargetingModule.HasTarget())
            {
                // TODO: Probably create some standardized projectile class for the friendly NPCs to use in order to define them as an "owner"
                if (false)
                {

                }
                else // Otherwise, it is a player.
                {
                    Player player = Main.player[projectile.owner];
                    TargetingModule.SetTarget(player);
                }
            }

            base.OnHitByProjectile(projectile, hit, damageDone);
        }

        private const WeaponType StunningSwordFlags = WeaponType.Sword | WeaponType.Broadsword | WeaponType.Greatsword;
        private const int SwordStunTicks = 25;

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            RecordDamage(damageDone);

            if (!TargetingModule.HasTarget())
                TargetingModule.SetTarget(player);

            if ((item.GetWeaponType() & StunningSwordFlags) != 0)
            {
                Stun(SwordStunTicks);
            }

            base.OnHitByItem(player, item, hit, damageDone);
        }

        #region Threat sensing
        private readonly Queue<(int tick, int amount)> recentDamage = new Queue<(int tick, int amount)>();
        private const int DamageWindowTicks = 90;

        private int wallPressTicks;
        private int corneredFor;

        /// <summary>
        /// Whether this NPC is currently pressed against terrain while trying to approach its target.
        /// </summary>
        public bool IsCornered => corneredFor > 0;

        private void RecordDamage(int amount)
        {
            if (amount > 0) recentDamage.Enqueue(((int)Main.GameUpdateCount, amount));
        }

        /// <summary>
        /// Fraction of max life taken within the recent damage window, from any source.
        /// </summary>
        public float RecentDamageFraction()
        {
            int now = (int)Main.GameUpdateCount;
            while (recentDamage.Count > 0 && now - recentDamage.Peek().tick > DamageWindowTicks)
                recentDamage.Dequeue();

            int sum = 0;
            foreach (var entry in recentDamage) sum += entry.amount;

            return NPC.lifeMax > 0 ? sum / (float)NPC.lifeMax : 0f;
        }

        /// <summary>
        /// Whether recent sustained damage has crossed this NPC's damage threshold.
        /// </summary>
        public bool TookSustainedDamage() => RecentDamageFraction() >= MathHelper.Lerp(0.25f, 0.06f, Personality.Caution);

        /// <summary>
        /// Clears the recent damage window.
        /// </summary>
        public void ClearDamageWindow() => recentDamage.Clear();

        /// <summary>
        /// Finds the nearest hostile-to-NPC projectile that is heading toward this NPC.
        /// </summary>
        /// <param name="threat">The incoming projectile, when one is found.</param>
        public bool HasIncomingProjectile(out Projectile threat)
        {
            threat = null;
            float closest = float.MaxValue;
            foreach (Projectile p in Main.projectile)
            {
                if (!p.active || !p.friendly || p.hostile) continue;
                float dist = Vector2.Distance(p.Center, NPC.Center);
                if (dist > 160f || dist >= closest) continue;
                if (Vector2.Dot(NPC.Center - p.Center, p.velocity) <= 0) continue;
                threat = p;
                closest = dist;
            }

            return threat != null;
        }

        private void UpdateCorneredState()
        {
            if (corneredFor > 0) corneredFor--;

            bool approaching = TargetingModule.HasTarget() && AIStateMachine != null && AIStateMachine.GetCurrentState() is MovementState;
            if (approaching && NPC.collideX)
            {
                if (++wallPressTicks >= 45)
                {
                    corneredFor = 60;
                    wallPressTicks = 0;
                }
            }
            else
            {
                wallPressTicks = 0;
            }
        }

        /// <summary>
        /// Clears the cornered window.
        /// </summary>
        public void ClearCornered() => corneredFor = 0;
        #endregion
    }
}