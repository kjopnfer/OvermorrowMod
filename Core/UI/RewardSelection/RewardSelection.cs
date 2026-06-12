using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Misc;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Chat;

namespace OvermorrowMod.Core.UI
{
    /// <summary>
    /// Three-slot reward picker that pops up above an opened reward chest.
    /// </summary>
    public class RewardSelection : UIState
    {
        public static bool visible = false;

        public NPC chestNPC;
        public int[] itemIds;
        public int selectedIndex = -1;
        public int drawCounter = 0;

        public bool isClosing = false;
        private Vector2 lastWorldAnchor;
        private bool hasLastAnchor = false;

        private int popoutTimer = 0;

        public const int FadeTicks = 30;
        public const int SlotSize = 52;
        public const int SlotSpacing = 32;
        public const int VerticalOffsetAboveChest = 120;

        public const int PopoutFlightTicks = 30;
        public const int PopoutStaggerTicks = 18;
        
        public const float PopoutArcHeight = 100f;

        public const int ButtonSize = 32;
        public const int ButtonGap = 32;
        public const int ButtonRowGap = 24;
        public const int ButtonFadeInTicks = 20;
        public const int TitleGap = 18;

        private Vector2 rowCenter;
        private float titleOpacity;

        // Order slots launch in: left first, right second, middle third.
        private static readonly int[] PopOrder = { 0, 2, 1 };

        private RewardSlot[] slots = new RewardSlot[3];
        private RewardActionButton confirmButton;
        private RewardActionButton denyButton;

        // Tick by which the last slot has landed. Buttons start fading in here.
        private const int ButtonsAppearTick = (3 - 1) * PopoutStaggerTicks + PopoutFlightTicks; // 66

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
        }

        public void Show(NPC chest, int[] items)
        {
            chestNPC = chest;
            itemIds = items;
            selectedIndex = -1;
            drawCounter = FadeTicks;
            popoutTimer = 0;
            isClosing = false;

            for (int i = 0; i < slots.Length; i++)
            {
                int id = (items != null && i < items.Length) ? items[i] : 0;
                slots[i].SetItem(id);
                slots[i].selected = false;
                slots[i].animOpacity = 0f;
            }

            if (confirmButton != null) 
                confirmButton.animOpacity = 0f;

            if (denyButton != null) 
                denyButton.animOpacity = 0f;

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
            visible = false;
            chestNPC = null;
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
        }

        public void HandleAction(RewardButtonType kind)
        {
            if (chestNPC?.ModNPC is CombatOrchestrator orch)
            {
                if (kind == RewardButtonType.Confirm)
                {
                    if (selectedIndex < 0) return;
                    int itemId = (itemIds != null && selectedIndex < itemIds.Length) ? itemIds[selectedIndex] : 0;
                    orch.ClaimReward(itemId);
                }
                else // Deny
                {
                    orch.isOpen = false;
                }
            }

            Hide();
        }

        public void SelectSlot(int idx)
        {
            selectedIndex = idx;
            for (int i = 0; i < slots.Length; i++)
                slots[i].selected = (i == idx);
        }

        public override void Update(GameTime gameTime)
        {
            if (!visible) return;

            // Cache chest world position
            if (chestNPC != null && chestNPC.active)
            {
                lastWorldAnchor = chestNPC.Center;
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
            }

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
                int slotAnimTimer = popoutTimer - popOrderIdx * PopoutStaggerTicks;

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

                slots[i].Left.Set(slotPos.X - SlotSize / 2f, 0);
                slots[i].Top.Set(slotPos.Y - SlotSize / 2f, 0);
                slots[i].animOpacity = popOpacity * fadeOutMul;
            }

            // Confirm/deny row, centered under the slot row.
            float buttonOpacity = MathHelper.Clamp((popoutTimer - ButtonsAppearTick) / (float)ButtonFadeInTicks, 0f, 1f);
            float buttonRowY = rowCenterUI.Y + SlotSize / 2f + ButtonRowGap + ButtonSize / 2f;
            float confirmCenterX = rowCenterUI.X - (ButtonSize + ButtonGap) / 2f;
            float denyCenterX    = rowCenterUI.X + (ButtonSize + ButtonGap) / 2f;

            confirmButton.Left.Set(confirmCenterX - ButtonSize / 2f, 0);
            confirmButton.Top.Set(buttonRowY - ButtonSize / 2f, 0);
            confirmButton.animOpacity = buttonOpacity * fadeOutMul;

            denyButton.Left.Set(denyCenterX - ButtonSize / 2f, 0);
            denyButton.Top.Set(buttonRowY - ButtonSize / 2f, 0);
            denyButton.animOpacity = buttonOpacity * fadeOutMul;

            Recalculate();
            base.Update(gameTime);
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
            if (!visible) return;
            base.Draw(spriteBatch);
            DrawTitle(spriteBatch);
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
