using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.Items.Collectibles;
using OvermorrowMod.Core.Loot;
using OvermorrowMod.Core.UI.RunStats;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Chat;

namespace OvermorrowMod.Core.UI
{
    /// <summary>
    /// Three-slot reward picker that pops up above an opened <see cref="IRewardSource"/>.
    /// </summary>
    public class RewardSelection : UIState
    {
        public static bool visible = false;

        /// <summary>
        /// True once the pop-out animation has fully settled and the picker is not fading out.
        /// </summary>
        public static bool IsInteractive { get; private set; }

        public IRewardSource source;
        public int[] itemIds;
        public int selectedIndex = -1;
        public int drawCounter = 0;

        public bool isClosing = false;
        private Vector2 lastWorldAnchor;
        private bool hasLastAnchor = false;

        private int popoutTimer = 0;
        private int slotTimer = 0;
        private int rerollFadeTimer = 0;
        private int[] pendingItemIds;

        private readonly bool[] launchSoundPlayed = new bool[3];

        private static readonly SoundStyle[] LaunchSounds =
        {
            new SoundStyle("OvermorrowMod/Sounds/LootReward") { PitchVariance = 0.2f, MaxInstances = 3 },
            new SoundStyle("OvermorrowMod/Sounds/LootReward2") { PitchVariance = 0.2f, MaxInstances = 3 },
        };

        public const int FadeTicks = 30;
        public const int SlotSize = 52;
        public const int SlotSpacing = 32;
        public const int VerticalOffsetAboveChest = 120;

        public const int PopoutFlightTicks = 30;
        public const int PopoutStaggerTicks = 28;
        
        public const float PopoutArcHeight = 100f;

        public const int ButtonSize = 32;
        public const int ButtonGap = 32;
        public const int ButtonRowGap = 24;
        public const int ButtonFadeInTicks = 20;
        public const int TitleGap = 18;
        public const int RerollFadeTicks = 15;

        public const int FlightTicks = 20;
        public const float FlightStartSize = 32f;
        public const float FlightEndSize = 12f;

        private Vector2 rowCenter;
        private float titleOpacity;

        private int flightType;
        private int flightTimer;
        private Vector2 flightStart;

        public float Opacity => titleOpacity;

        public bool Active => visible || flightType > 0;

        // Order slots launch in: left first, right second, middle third.
        private static readonly int[] PopOrder = { 0, 2, 1 };

        private RewardSlot[] slots = new RewardSlot[3];
        private RewardActionButton confirmButton;
        private RewardActionButton denyButton;
        private RewardActionButton rerollButton;

        // Tick by which the last slot has landed. Buttons start fading in here.
        private const int ButtonsAppearTick = (3 - 1) * PopoutStaggerTicks + PopoutFlightTicks; // 86

        // popoutTimer value once the slots have landed and the buttons have fully faded in.
        public const int OpenAnimationTicks = ButtonsAppearTick + ButtonFadeInTicks; // 106

        /// <summary>
        /// False while slots are flying out or fading for a reroll. The title and buttons stay visible through a reroll.
        /// </summary>
        public bool AcceptsInput => !isClosing && rerollFadeTimer == 0 && slotTimer >= ButtonsAppearTick;

        public override void OnInitialize()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = new RewardSlot { slotIndex = i };
                slots[i].Width.Set(SlotSize, 0);
                slots[i].Height.Set(SlotSize, 0);
                Append(slots[i]);
            }

            confirmButton = new RewardActionButton { type = RewardButtonType.Confirm };
            confirmButton.Width.Set(ButtonSize, 0);
            confirmButton.Height.Set(ButtonSize, 0);
            Append(confirmButton);

            denyButton = new RewardActionButton { type = RewardButtonType.Deny };
            denyButton.Width.Set(ButtonSize, 0);
            denyButton.Height.Set(ButtonSize, 0);
            Append(denyButton);

            rerollButton = new RewardActionButton { type = RewardButtonType.Reroll };
            rerollButton.Width.Set(ButtonSize, 0);
            rerollButton.Height.Set(ButtonSize, 0);
            Append(rerollButton);
        }

        public void Show(IRewardSource rewardSource, int[] items)
        {
            source = rewardSource;
            itemIds = items;
            selectedIndex = -1;
            drawCounter = FadeTicks;
            popoutTimer = 0;
            slotTimer = 0;
            rerollFadeTimer = 0;
            pendingItemIds = null;
            isClosing = false;

            for (int i = 0; i < slots.Length; i++)
            {
                int id = (items != null && i < items.Length) ? items[i] : 0;
                slots[i].SetItem(id);
                slots[i].selected = false;
                slots[i].animOpacity = 0f;
                launchSoundPlayed[i] = false;
            }

            if (confirmButton != null)
                confirmButton.animOpacity = 0f;

            if (denyButton != null)
                denyButton.animOpacity = 0f;

            if (rerollButton != null)
                rerollButton.animOpacity = 0f;

            IsInteractive = false;
            visible = true;
        }

        /// <summary>Begins the fade-out. Update finalizes the hide once drawCounter hits 0.</summary>
        public void Hide()
        {
            if (!visible) return;
            isClosing = true;
        }

        private void FinishHide()
        {
            IsInteractive = false;
            visible = false;
            source = null;
            isClosing = false;
            selectedIndex = -1;
            hasLastAnchor = false;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] != null)
                {
                    slots[i].selected = false;
                    slots[i].animOpacity = 0f;
                }
            }
            if (confirmButton != null) confirmButton.animOpacity = 0f;
            if (denyButton != null) denyButton.animOpacity = 0f;
            if (rerollButton != null) rerollButton.animOpacity = 0f;
            rerollFadeTimer = 0;
            pendingItemIds = null;
        }

        public void HandleAction(RewardButtonType kind)
        {
            if (kind == RewardButtonType.Reroll)
            {
                StartReroll();
                return;
            }

            if (source != null)
            {
                if (kind == RewardButtonType.Confirm)
                {
                    if (selectedIndex < 0) return;
                    int itemId = (itemIds != null && selectedIndex < itemIds.Length) ? itemIds[selectedIndex] : 0;
                    if (itemId > 0 && ContentSamples.ItemsByType[itemId].ModItem is CollectibleItem)
                        StartFlight(itemId);
                    source.ClaimReward(itemId);
                    SoundEngine.PlaySound(SoundID.Coins);
                }
                else // Deny
                {
                    source.DeclineReward();
                }
            }

            Hide();
        }

        private void StartReroll()
        {
            var rerolls = Main.LocalPlayer.GetModPlayer<RerollPlayer>();
            if (source == null || !rerolls.TrySpend()) return;

            int[] rerolled = source.RerollOffers();
            if (rerolled == null)
            {
                rerolls.Rerolls++;
                return;
            }

            pendingItemIds = rerolled;
            rerollFadeTimer = RerollFadeTicks;
            SelectSlot(-1);
        }

        private void SwapInRerolledItems()
        {
            itemIds = pendingItemIds;
            pendingItemIds = null;
            slotTimer = 0;
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].SetItem(i < itemIds.Length ? itemIds[i] : 0);
                launchSoundPlayed[i] = false;
            }
        }

        public void SelectSlot(int idx)
        {
            selectedIndex = idx;
            for (int i = 0; i < slots.Length; i++)
                slots[i].selected = (i == idx);
        }

        private void StartFlight(int itemId)
        {
            flightType = itemId;
            flightTimer = 0;
            flightStart = slots[selectedIndex].GetDimensions().Center();
            slots[selectedIndex].SetItem(0);
        }

        private void UpdateFlight()
        {
            if (flightType == 0 || ++flightTimer < FlightTicks) return;

            if (ContentSamples.ItemsByType[flightType].ModItem is CollectibleItem collectible)
                collectible.ShowPickup(Main.LocalPlayer);
            flightType = 0;
        }

        public override void Update(GameTime gameTime)
        {
            UpdateFlight();
            if (!visible) return;

            // Cache chest world position
            if (source != null && source.RewardSourceActive)
            {
                lastWorldAnchor = source.RewardAnchor;
                hasLastAnchor = true;
            }
            else if (!isClosing)
            {
                isClosing = true;
            }

            if (!hasLastAnchor)
            {
                FinishHide();
                return;
            }

            if (isClosing)
            {
                if (drawCounter > 0) 
                    drawCounter--;

                if (drawCounter <= 0)
                {
                    FinishHide();
                    return;
                }
            }
            else
            {
                popoutTimer++;
                if (rerollFadeTimer > 0)
                {
                    if (--rerollFadeTimer == 0)
                        SwapInRerolledItems();
                }
                else
                {
                    slotTimer++;
                }
            }

            IsInteractive = !isClosing && popoutTimer >= OpenAnimationTicks;

            // World-to-UI: chest center for the launch start, and chest +
            // vertical offset for the row of landing positions.
            Vector2 chestUI = WorldToUI(lastWorldAnchor);
            Vector2 rowCenterUI = WorldToUI(lastWorldAnchor + new Vector2(0, -VerticalOffsetAboveChest));
            int totalWidth = SlotSize * 3 + SlotSpacing * 2;
            float fadeOutMul = isClosing ? (drawCounter / (float)FadeTicks) : 1f;

            rowCenter = rowCenterUI;
            titleOpacity = MathHelper.Clamp(popoutTimer / (float)PopoutFlightTicks, 0f, 1f) * fadeOutMul;

            for (int i = 0; i < slots.Length; i++)
            {
                // Landing position in UI space for this slot.
                float finalX = rowCenterUI.X - totalWidth / 2f + SlotSize / 2f + i * (SlotSize + SlotSpacing);
                Vector2 finalPos = new Vector2(finalX, rowCenterUI.Y);

                // Translate slot index to launch order
                int popOrderIdx = System.Array.IndexOf(PopOrder, i);
                int slotAnimTimer = slotTimer - popOrderIdx * PopoutStaggerTicks;

                Vector2 slotPos;
                float popOpacity;

                if (slotAnimTimer < 0)
                {
                    // Not yet launched
                    slotPos = chestUI;
                    popOpacity = 0f;
                }
                else if (slotAnimTimer < PopoutFlightTicks)
                {
                    // Mid-flight
                    float t = slotAnimTimer / (float)PopoutFlightTicks;
                    Vector2 lerp = Vector2.Lerp(chestUI, finalPos, t);
                    float arc = -PopoutArcHeight * 4f * t * (1f - t);
                    slotPos = lerp + new Vector2(0, arc);
                    popOpacity = MathHelper.Clamp(t * 2f, 0f, 1f); // fade in over first half
                }
                else
                {
                    // Land
                    slotPos = finalPos;
                    popOpacity = 1f;
                }

                if (rerollFadeTimer > 0)
                    popOpacity *= rerollFadeTimer / (float)RerollFadeTicks;

                if (slotAnimTimer >= 0 && !launchSoundPlayed[i] && !isClosing && !slots[i].displayItem.IsAir)
                {
                    SoundEngine.PlaySound(Main.rand.Next(LaunchSounds));
                    launchSoundPlayed[i] = true;
                }

                slots[i].Left.Set(slotPos.X - SlotSize / 2f, 0);
                slots[i].Top.Set(slotPos.Y - SlotSize / 2f, 0);
                slots[i].animOpacity = popOpacity * fadeOutMul;
            }

            // Confirm, reroll, deny row, centered under the slot row.
            float buttonOpacity = MathHelper.Clamp((popoutTimer - ButtonsAppearTick) / (float)ButtonFadeInTicks, 0f, 1f) * fadeOutMul;
            float buttonRowY = rowCenterUI.Y + SlotSize / 2f + ButtonRowGap + ButtonSize / 2f;
            PlaceButton(confirmButton, rowCenterUI.X - (ButtonSize + ButtonGap), buttonRowY, buttonOpacity);
            PlaceButton(rerollButton, rowCenterUI.X, buttonRowY, buttonOpacity);
            PlaceButton(denyButton, rowCenterUI.X + (ButtonSize + ButtonGap), buttonRowY, buttonOpacity);

            Recalculate();
            base.Update(gameTime);
        }

        private static void PlaceButton(RewardActionButton button, float centerX, float centerY, float opacity)
        {
            button.Left.Set(centerX - ButtonSize / 2f, 0);
            button.Top.Set(centerY - ButtonSize / 2f, 0);
            button.animOpacity = opacity;
        }

        /// <summary>
        /// World position to UI-coordinate position. Applies game zoom
        /// around the screen center and divides by UI scale so the result
        /// lands in the same coord space the UIState uses for Left/Top.
        /// </summary>
        private static Vector2 WorldToUI(Vector2 worldPos)
        {
            Vector2 result = (worldPos - Main.screenPosition) * Main.GameViewMatrix.Zoom;
            result += new Vector2(Main.screenWidth, Main.screenHeight) / 2f * (Vector2.One - Main.GameViewMatrix.Zoom);
            result /= Main.UIScale;
            return result;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                base.Draw(spriteBatch);
                DrawTitle(spriteBatch);
            }

            DrawFlight(spriteBatch);
        }

        private void DrawFlight(SpriteBatch spriteBatch)
        {
            if (flightType == 0) return;

            float progress = EasingUtils.EaseInCubic(flightTimer / (float)FlightTicks);
            Vector2 position = Vector2.Lerp(flightStart, WorldToUI(Main.LocalPlayer.Center), progress);
            RunStatsPanel.DrawItemIcon(spriteBatch, flightType, position, MathHelper.Lerp(FlightStartSize, FlightEndSize, progress), Color.White);
        }

        private void DrawTitle(SpriteBatch spriteBatch)
        {
            if (titleOpacity <= 0f) return;

            string text = Language.GetTextValue(LocalizationPath.UI + "RewardSelection.Title");
            var font = FontAssets.MouseText.Value;
            Vector2 size = ChatManager.GetStringSize(font, text, Vector2.One);
            Vector2 pos = new Vector2(rowCenter.X - size.X / 2f, rowCenter.Y - SlotSize / 2f - TitleGap - size.Y);

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, text, pos, Color.White * titleOpacity, 0f, Vector2.Zero, Vector2.One);
        }
    }
}
