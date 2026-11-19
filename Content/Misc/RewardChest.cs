using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Items.Collectibles;
using OvermorrowMod.Core.Loot;
using OvermorrowMod.Core.Loot.Pools;
using OvermorrowMod.Core.Particles;
using OvermorrowMod.Core.UI;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Misc
{
    /// <summary>
    /// A chest NPC that, once revealed and landed, opens on right click and offers a pick of rewards.
    /// </summary>
    public abstract class RewardChest : ModNPC
    {
        public bool isRevealed = false;
        public bool hasImpacted = false;
        public bool isOpen = false;

        public int openingBuildupTimer = 0;
        private const int OpeningBuildupDuration = 90;

        private bool wasOpen = false;

        private int[] offeredRewards;

        public bool isCleaningUp = false;
        public int cleanupTimer = 0;
        private const int CleanupDuration = 360;

        private const int RewardOfferCount = 3;

        private static readonly (ItemKind Kind, int Weight)[] RewardCategories =
        {
            (ItemKind.Consumable, 45),
            (ItemKind.Accessory, 30),
            (ItemKind.Weapon, 15),
            (ItemKind.Armor, 10),
        };

        public override string Texture => AssetDirectory.Misc + "RewardChest";

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;
            NPC.aiStyle = -1;
            NPC.lifeMax = 1;
            NPC.life = 1;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.immortal = true;
            NPC.friendly = true;
            NPC.dontCountMe = true;
            NPC.HitSound = null;
            NPC.DeathSound = null;
        }

        public override bool CheckActive() => false;

        /// <summary>
        /// The item ids offered when the chest first opens.
        /// </summary>
        protected abstract int[] RollRewards();

        protected virtual void OnRewardClaimed() { }

        public void ClaimReward(int itemId)
        {
            if (isCleaningUp) return;
            if (itemId > 0 && ContentSamples.ItemsByType[itemId].ModItem is CollectibleItem collectible)
            {
                collectible.Apply(Main.LocalPlayer, 1);
            }
            else if (itemId > 0)
            {
                Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_Loot(), itemId);
                LootRoller.GiveCompanionAmmo(NPC.GetSource_Loot(), Main.LocalPlayer.Center, itemId);
            }
            isCleaningUp = true;
            cleanupTimer = 0;
            OnRewardClaimed();
        }

        /// <summary>
        /// Advances the fade after a reward is claimed and despawns the chest when it ends.
        /// </summary>
        /// <returns>
        /// True while the chest is cleaning up.
        /// </returns>
        protected bool UpdateCleanup()
        {
            if (!isCleaningUp) return false;

            cleanupTimer++;
            if (cleanupTimer >= CleanupDuration)
                NPC.active = false;
            return true;
        }

        /// <summary>
        /// Lands the chest, emits its rays and light, and shows or hides the reward picker as the chest opens or closes.
        /// </summary>
        protected void UpdateChest()
        {
            if (isRevealed && !hasImpacted && NPC.collideY)
            {
                SpawnImpactParticles();
                hasImpacted = true;
            }

            EmitRays();
            if (isOpen && !wasOpen)
            {
                offeredRewards ??= RollRewards();
                if (offeredRewards.Length > 0)
                    RewardSelectionManager.ShowFor(NPC, offeredRewards);
            }
            else if (!isOpen && wasOpen)
            {
                RewardSelectionManager.Hide();
            }
            wasOpen = isOpen;

            if (isRevealed)
            {
                float intensity = 0.6f;
                if (openingBuildupTimer > 0)
                {
                    float progress = (OpeningBuildupDuration - openingBuildupTimer) / (float)OpeningBuildupDuration;
                    intensity = MathHelper.Lerp(0.6f, 1.2f, progress);
                }

                Lighting.AddLight(NPC.Center, Color.Gold.ToVector3() * intensity);
            }
        }

        /// <summary>
        /// Opens the landed chest on right click from a player in range and closes it when the player walks away.
        /// </summary>
        protected void UpdateOpening()
        {
            if (!hasImpacted) return;

            const float InteractRangeTiles = 7f;
            float interactRange = InteractRangeTiles * 16f;
            bool playerInRange = Main.LocalPlayer.active && Vector2.Distance(Main.LocalPlayer.Center, NPC.Center) <= interactRange;

            if (!playerInRange)
            {
                isOpen = false;
                openingBuildupTimer = 0;
                return;
            }

            if (openingBuildupTimer > 0)
            {
                openingBuildupTimer--;
                if (openingBuildupTimer == 0) isOpen = true;
                return;
            }

            if (!NPC.Hitbox.Contains((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y)) return;

            Main.LocalPlayer.cursorItemIconEnabled = true;
            Main.LocalPlayer.cursorItemIconID = ItemID.GoldChest;
            Main.LocalPlayer.noThrow = 2;

            if (Main.mouseRight && Main.mouseRightRelease && !isOpen)
                openingBuildupTimer = OpeningBuildupDuration;
        }

        /// <summary>
        /// Rolls <see cref="RewardOfferCount"/> distinct offers across the reward categories, gear from the active
        /// pool with <paramref name="gearModifier"/> and consumables from the collectibles pool with <paramref name="consumableModifier"/>.
        /// </summary>
        protected static int[] RollOffers(RarityModifier gearModifier, RarityModifier consumableModifier)
        {
            var gear = LootPoolRegistry.GetActive();
            if (gear == null) return System.Array.Empty<int>();
            var collectibles = LootPoolRegistry.Get<CollectiblesPool>();

            var offers = new List<int>(RewardOfferCount);
            var used = new HashSet<int>();

            for (int slot = 0; slot < RewardOfferCount; slot++)
            {
                int id = RollRewardSlot(gear, collectibles, used, gearModifier, consumableModifier);
                if (id == 0) continue;
                offers.Add(id);
                used.Add(id);
            }

            return offers.ToArray();
        }

        private static int RollRewardSlot(LootPool gear, LootPool collectibles, HashSet<int> used, RarityModifier gearModifier, RarityModifier consumableModifier)
        {
            ItemKind primary = PickRewardCategory();
            int id = RollCategory(primary, gear, collectibles, used, gearModifier, consumableModifier);
            if (id != 0) return id;

            foreach (var category in RewardCategories)
            {
                if (category.Kind == primary) continue;
                id = RollCategory(category.Kind, gear, collectibles, used, gearModifier, consumableModifier);
                if (id != 0) return id;
            }
            return 0;
        }

        private static int RollCategory(ItemKind kind, LootPool gear, LootPool collectibles, HashSet<int> used, RarityModifier gearModifier, RarityModifier consumableModifier)
        {
            if (kind == ItemKind.Consumable)
                return LootRoller.RollOne(collectibles, ItemKind.Consumable, consumableModifier, Main.LocalPlayer, used);

            return LootRoller.RollOne(gear, kind, gearModifier, Main.LocalPlayer, used);
        }

        private static ItemKind PickRewardCategory()
        {
            int total = 0;
            foreach (var category in RewardCategories) total += category.Weight;

            int roll = Main.rand.Next(total);
            foreach (var category in RewardCategories)
            {
                roll -= category.Weight;
                if (roll < 0) return category.Kind;
            }
            return RewardCategories[0].Kind;
        }

        private void SpawnImpactParticles()
        {
            Texture2D sparkTex = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01", AssetRequestMode.ImmediateLoad).Value;

            int count = Main.rand.Next(12, 19);
            for (int i = 0; i < count; i++)
            {
                float scale = Main.rand.NextFloat(2f, 7f);
                float angle = Main.rand.NextFloat(MathHelper.ToRadians(-15f), MathHelper.ToRadians(15f));
                if (Main.rand.NextBool()) angle += MathHelper.Pi;
                Vector2 vel = Vector2.UnitX.RotatedBy(angle) * Main.rand.Next(2, 12);

                var spark = new Spark(sparkTex, maxTime: Main.rand.Next(15, 30), true, 0f) { endColor = Color.Goldenrod };
                ParticleManager.CreateParticleDirect(spark, NPC.Bottom, vel, Color.Gold, 1f, scale, 0f, ParticleDrawLayer.BehindProjectiles, useAdditiveBlending: true);
            }
        }

        int frame = 0;
        int frameCount = 0;

        public int wobbleTimer = 0;
        public int wobbleActiveTicks = 0;
        public int wobbleRestTicks = 0;
        public float wobbleStrength = 0f;
        public int wobbleDirection = 1;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!isRevealed) return false;

            int targetFrame = isOpen ? 2 : 0;
            int frameInterval = (frame < targetFrame) ? 3 : 8;
            frameCount++;
            if (frameCount >= frameInterval)
            {
                frameCount = 0;
                if (frame < targetFrame) frame++;
                else if (frame > targetFrame) frame--;
            }

            // Buildup wobble
            if (openingBuildupTimer > 0)
            {
                int elapsed = OpeningBuildupDuration - openingBuildupTimer;
                float progress = elapsed / (float)OpeningBuildupDuration;
                float strength = MathHelper.Lerp(0.02f, 0.18f, progress);
                float freq = MathHelper.Lerp(0.3f, 1.0f, progress);
                NPC.rotation = (float)System.Math.Sin(elapsed * freq) * strength;
                wobbleTimer = 0;
            }
            else if (hasImpacted && !isOpen && frame == 0) // Idle wobble while closed
            {
                if (wobbleTimer == 0)
                {
                    wobbleActiveTicks = Main.rand.Next(18, 30);
                    wobbleRestTicks = Main.rand.Next(30, 120);
                    wobbleStrength = Main.rand.NextFloat(0.08f, 0.15f);
                    wobbleDirection = Main.rand.NextBool() ? 1 : -1;
                }

                if (wobbleTimer < wobbleActiveTicks)
                {
                    float damping = 1f - (wobbleTimer / (float)wobbleActiveTicks);
                    NPC.rotation = (float)System.Math.Sin(wobbleTimer * 0.5f) * wobbleStrength * damping * wobbleDirection;
                }
                else
                {
                    NPC.rotation = 0f;
                }

                wobbleTimer++;
                if (wobbleTimer >= wobbleActiveTicks + wobbleRestTicks)
                    wobbleTimer = 0;
            }
            else
            {
                NPC.rotation = 0f;
                wobbleTimer = 0;
            }

            float cleanupAlpha = isCleaningUp ? 1f - MathHelper.Clamp(cleanupTimer / (float)CleanupDuration, 0f, 1f) : 1f;

            Texture2D tex = ModContent.Request<Texture2D>(Texture, AssetRequestMode.ImmediateLoad).Value;
            var drawRectangle = new Rectangle(0, frame * 36, 32, 36);
            spriteBatch.Draw(tex, NPC.Center - screenPos, drawRectangle, drawColor * cleanupAlpha, NPC.rotation, drawRectangle.Size() / 2f, 1f, SpriteEffects.None, 0f);
            return false;
        }

        private void EmitRays()
        {
            if (Main.gamePaused) return;
            if (!hasImpacted) return;

            NPC.localAI[0]++;
            if (NPC.localAI[0] % 40 != 0) return;

            Texture2D rayTex = ModContent.Request<Texture2D>(AssetDirectory.Textures + "ray", AssetRequestMode.ImmediateLoad).Value;

            int count = Main.rand.Next(3, 5);
            for (int i = 0; i < count; i++)
            {
                var ray = new Light(rayTex, ModUtils.SecondsToTicks(5), NPC, Vector2.Zero);
                float rotation = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                float scale = Main.rand.NextFloat(0.05f, 0.065f);
                ParticleManager.CreateParticleDirect(ray, NPC.Center, Vector2.Zero, Color.Gold, 1f, scale, rotation, ParticleDrawLayer.BehindNPCs, useAdditiveBlending: true);
            }
        }
    }
}
