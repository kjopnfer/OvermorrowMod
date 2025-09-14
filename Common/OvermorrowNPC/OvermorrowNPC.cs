using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.RoomManager;
using OvermorrowMod.Content.NPCs;
using OvermorrowMod.Core.Globals;
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
            AIStateMachine = new AIStateMachine(NPC.ModNPC as OvermorrowNPC, InitializeIdleStates(), InitializeMovementStates(), InitializeAttackStates());

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

            TargetingModule.Update();

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
    }
}