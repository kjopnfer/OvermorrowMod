using Microsoft.Xna.Framework;
using OvermorrowMod.Core.WorldGeneration;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Globals
{
    public class DashPlayer : ModPlayer
    {
        private const int DashTicks = 10;
        private const float DashSpeed = 14f;
        private const float SlideDecay = 0.75f;
        private const int MaxCharges = 2;
        private const int RechargeTicks = 45;

        /// <summary>
        /// Index order of <see cref="Player.doubleTapCardinalTimer"/>.
        /// </summary>
        private enum Cardinal { Down, Up, Right, Left }

        private int dashTicksLeft;
        private bool sliding;
        private int dashDirection;
        private int charges = MaxCharges;
        private int rechargeTicksLeft;
        private int tapDirection;

        private bool Dashing => dashTicksLeft > 0 || sliding;

        public override void OnEnterWorld() => ResetDash();

        public override void UpdateDead() => ResetDash();

        public override void PostUpdateEquips()
        {
            if (RunState.InRun) Player.dashType = 0;
        }

        public override void SetControls()
        {
            if (ConsumeDoubleTap(Player.controlRight && Player.releaseRight, Cardinal.Right)) tapDirection = 1;
            else if (ConsumeDoubleTap(Player.controlLeft && Player.releaseLeft, Cardinal.Left)) tapDirection = -1;
        }

        public override void PreUpdateMovement()
        {
            if (dashTicksLeft == 0 && tapDirection != 0)
            {
                TryStartDash();
                tapDirection = 0;
            }

            if (dashTicksLeft > 0) UpdateBurst();
            else if (sliding) UpdateSlide();
        }

        public override void PostUpdate()
        {
            if (Dashing && Player.velocity.X == 0f) EndDash();

            if (!Dashing && rechargeTicksLeft > 0 && --rechargeTicksLeft == 0) charges = MaxCharges;
        }

        private void TryStartDash()
        {
            if (charges == 0 || Player.dead || Player.CCed || Player.mount.Active || Player.pulley || Player.tongued || Player.grappling[0] >= 0) return;

            charges--;
            dashDirection = tapDirection;
            dashTicksLeft = DashTicks;
            sliding = false;
            Player.timeSinceLastDashStarted = 0;
            SpawnStartDust();
        }

        private bool ConsumeDoubleTap(bool pressed, Cardinal direction)
        {
            int timer = Player.doubleTapCardinalTimer[(int)direction];
            if (!pressed || timer == 0 || timer == 15) return false;

            Player.doubleTapCardinalTimer[(int)direction] = 0;
            return true;
        }

        private void UpdateBurst()
        {
            Player.velocity.X = DashSpeed * dashDirection;
            Player.dashDelay = -1;
            SpawnTrailDust();

            if (--dashTicksLeft == 0) sliding = true;
        }

        private void UpdateSlide()
        {
            float runSpeed = Math.Max(Player.accRunSpeed, Player.maxRunSpeed);
            if (Math.Abs(Player.velocity.X) <= runSpeed)
            {
                EndDash();
                return;
            }

            Player.velocity.X *= SlideDecay;
            Player.dashDelay = -1;
            SpawnTrailDust();
        }

        private void EndDash()
        {
            dashTicksLeft = 0;
            sliding = false;
            rechargeTicksLeft = RechargeTicks;
        }

        private void ResetDash()
        {
            dashTicksLeft = 0;
            sliding = false;
            charges = MaxCharges;
            rechargeTicksLeft = 0;
            tapDirection = 0;
        }

        private void SpawnStartDust()
        {
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                dust.position += new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, 6));
                dust.velocity *= 0.2f;
                dust.scale *= 1f + Main.rand.Next(20) * 0.01f;
            }

            for (int i = 0; i < 2; i++)
            {
                Vector2 position = new Vector2(Player.Center.X - 24f, Player.Center.Y - 34f + i * 20f);
                Gore gore = Gore.NewGoreDirect(Player.GetSource_FromThis(), position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
                gore.velocity = new Vector2(Main.rand.Next(-50, 51), Main.rand.Next(-50, 51)) * 0.01f * 0.4f;
            }
        }

        private void SpawnTrailDust()
        {
            for (int i = 0; i < 2; i++)
            {
                Dust dust = Player.velocity.Y != 0f ? Dust.NewDustDirect(new Vector2(Player.position.X, Player.Center.Y - 8f), Player.width, 16, DustID.Smoke, 0f, 0f, 100, default, 1.4f) : Dust.NewDustDirect(new Vector2(Player.position.X, Player.position.Y + Player.height - 4f), Player.width, 8, DustID.Smoke, 0f, 0f, 100, default, 1.4f);
                dust.velocity *= 0.1f;
                dust.scale *= 1f + Main.rand.Next(20) * 0.01f;
            }
        }
    }
}
