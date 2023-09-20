using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Players
{
    public partial class OvermorrowModPlayer : ModPlayer
    {
        public enum BossID
        {
            Eye = 1
        }

        private int holdCounter;
        private Vector2 focusTo;
        private int holdCameraLength;
        private float towardsLength;
        private float returnLength;

        public int shakeTimer = 0;
        public bool FocusBoss;
        public bool MoveTowards = true;
        public bool ReturnBack = false;

        public bool LockCamera;
        private NPC focusNPC;

        private float CameraCounter = 0;
        private float TitleCounter = 0;

        public bool ShowText;
        public int TitleLength;
        public int TitleID;

        //sadness
        Vector2 screenPositionStore;

        // Controls the length of time the screen shakes
        public static int ScreenShake;
        // Controls how violently the screen shakes
        public static int ShakeOffset = 0;
        public void AddScreenShake(int Length, int Intensity = 1)
        {
            ScreenShake = Length;
            ShakeOffset = Intensity;
        }

        public void PlayerLockCamera(NPC focusNPC, int holdCameraLength, float towardsLength, float returnLength)
        {
            this.focusNPC = focusNPC;
            this.holdCameraLength = holdCameraLength;
            this.towardsLength = towardsLength;
            this.returnLength = returnLength;

            LockCamera = true;
            MoveTowards = true;
        }

        public void PlayerFocusCamera(Vector2 focusTo, int holdCameraLength, float towardsLength, float returnLength)
        {
            // The position to move to and from
            this.focusTo = focusTo;

            // How long the camera stays in place
            this.holdCameraLength = holdCameraLength;

            // How long it takes to travel to the position
            this.towardsLength = towardsLength;

            // How long it takes to return to the player
            this.returnLength = returnLength;

            // Finally, flag boolean to activate ModifyScreenPosition hook
            FocusBoss = true;
            MoveTowards = true;
        }

        public void ShowTitleCard(BossID id, int showLength)
        {
            TitleID = (int)id;
            TitleLength = showLength;
            ShowText = true;
        }

        public override void ModifyScreenPosition()
        {
            #region Camera Panning
            if (FocusBoss || LockCamera)
            {
                // Camera moves towards the desired position
                if (MoveTowards)
                {
                    if (!Main.gamePaused)
                    {
                        if (LockCamera)
                        {
                            screenPositionStore = new Vector2(MathHelper.Lerp(Player.Center.X - Main.screenWidth / 2, focusNPC.Center.X - Main.screenWidth / 2, CameraCounter), MathHelper.Lerp(Player.Center.Y - Main.screenHeight / 2, focusNPC.Center.Y - Main.screenHeight / 2, CameraCounter));
                        }
                        else
                        {
                            screenPositionStore = new Vector2(MathHelper.Lerp(Player.Center.X - Main.screenWidth / 2, focusTo.X - Main.screenWidth / 2, CameraCounter), MathHelper.Lerp(Player.Center.Y - Main.screenHeight / 2, focusTo.Y - Main.screenHeight / 2, CameraCounter));
                        }

                        CameraCounter += 1 / towardsLength;
                    }

                    Main.screenPosition = screenPositionStore;

                    if (CameraCounter >= 1f)
                    {
                        MoveTowards = false;

                        CameraCounter = 0;
                    }
                }
                else // Camera has moved to the desired position
                {
                    if (!ReturnBack)
                    {
                        if (FocusBoss)
                        {
                            Main.screenPosition = screenPositionStore;
                            holdCounter++;
                        }

                        if (LockCamera)
                        {
                            Main.screenPosition = focusNPC.Center - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
                            holdCounter++;
                        }

                        //ShowText = true;

                        if (holdCounter == holdCameraLength)
                        {
                            ReturnBack = true;
                            ShowText = false;

                            holdCounter = 0;
                        }
                    }
                    else
                    {
                        if (!Main.gamePaused)
                        {
                            if (LockCamera)
                            {
                                screenPositionStore = new Vector2(MathHelper.SmoothStep(focusNPC.Center.X - Main.screenWidth / 2, Player.Center.X - Main.screenWidth / 2, CameraCounter), MathHelper.SmoothStep(focusNPC.Center.Y - Main.screenHeight / 2, Player.Center.Y - Main.screenHeight / 2, CameraCounter));
                            }
                            else
                            {
                                screenPositionStore = new Vector2(MathHelper.SmoothStep(focusTo.X - Main.screenWidth / 2, Player.Center.X - Main.screenWidth / 2, CameraCounter), MathHelper.SmoothStep(focusTo.Y - Main.screenHeight / 2, Player.Center.Y - Main.screenHeight / 2, CameraCounter));
                            }

                            CameraCounter += 1 / returnLength;
                        }

                        Main.screenPosition = screenPositionStore;

                        if (CameraCounter >= 1f)
                        {
                            ReturnBack = false;
                            FocusBoss = false;
                            LockCamera = false;

                            MoveTowards = true;
                            ShowText = false;

                            CameraCounter = 0;
                        }
                    }
                }
            }
            #endregion

            #region Screenshake
            if (!Main.gamePaused)
            {
                if (ScreenShake > 0)
                {
                    Main.screenPosition += new Vector2(Main.rand.Next(-1 - ShakeOffset, 1 + ShakeOffset), Main.rand.Next(-1, 1));
                    ScreenShake--;
                }
            }
            #endregion

            #region Boss Title
            if (TitleCounter == TitleLength)
            {
                TitleCounter = 0;
                ShowText = false;
            }
            else
            {
                if (!Main.gamePaused) TitleCounter++;
            }
            #endregion
        }
    }
}