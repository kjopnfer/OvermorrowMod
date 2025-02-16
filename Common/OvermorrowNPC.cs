using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.RoomManager;
using OvermorrowMod.Core.Globals;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public abstract class OvermorrowNPC : ModNPC
    {
        public ref Player Player => ref Main.player[NPC.target];

        /// <summary>
        /// Saves the ID of the Spawner if the NPC was created by one.
        /// </summary>
        public int? SpawnerID { get; set; } = null;
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue(LocalizationPath.Bestiary + Name)),
            });
        }

        public sealed override void SetDefaults()
        {
            NPC.GetGlobalNPC<BarrierNPC>().MaxBarrierPoints = (int)(NPC.lifeMax * 0.25f);
            SafeSetDefaults();
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

        public sealed override void OnKill()
        {
            if (SpawnerID.HasValue && TileEntity.ByID.TryGetValue(SpawnerID.Value, out TileEntity entity) && entity is NPCSpawnPoint spawner)
            {
                spawner.HasBeenKilled = true;
            }
        }
    }
}