using OvermorrowMod.Core.Items.Accessories;
using OvermorrowMod.Core.Items.Guns;
using Terraria;
using Terraria.Audio;

namespace OvermorrowMod.Common.Items.Guns
{
    public abstract partial class HeldGun
    {
        private bool reloadFail = false;
        public bool reloadSuccess { get; private set; } = false;
        protected int reloadTime = 0;
        private int clickDelay = 0;
        public int reloadDelay { get; private set; } = 0;
        private int reloadBuffer = 10;

        /// <summary>
        /// Enters the reload minigame, resetting the firing cooldown so it begins immediately.
        /// </summary>
        private void EnterReload()
        {
            inReloadState = true;
            reloadTime = MaxReloadTime;
            reloadBuffer = 10;
            shootCounter = 0;
            Projectile.netUpdate = true;
        }

        private void HandleReloadAction()
        {
            if (reloadTime == MaxReloadTime)
            {
                OnReloadStart(player);
            }

            if (reloadTime > 0) reloadTime--;
            if (clickDelay > 0) clickDelay--;
            if (reloadBuffer > 0)
            {
                reloadBuffer--;
                return;
            }

            player.itemTime = 2;
            player.itemAnimation = 2;

            // Only process clicks if there's no click delay and reload hasn't failed
            if (player.controlUseItem && clickDelay == 0 && !reloadFail)
            {
                float clickPercentage = (1 - (float)reloadTime / MaxReloadTime) * 100f;
                clickDelay = 15;

                // Check if we clicked in any zone that hasn't been clicked yet
                bool hitValidZone = false;
                for (int i = 0; i < ClickZones.Count; i++)
                {
                    var zone = ClickZones[i];
                    bool inRange = clickPercentage >= zone.StartPercentage && clickPercentage <= zone.EndPercentage;
                    bool alreadyClicked = zone.HasClicked;

                    if (!alreadyClicked && inRange)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/ReloadingClick") with
                        {
                            MaxInstances = 0,
                            Volume = 1f
                        }, player.Center);

                        zone.HasClicked = true;
                        ReloadEventTrigger(player, i, GetClicksLeft());
                        hitValidZone = true;
                        break; // Only hit one zone per click
                    }
                }

                if (!hitValidZone)
                {
                    SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/youmissedthatone") with
                    {
                        Volume = 3f
                    }, player.Center);
                    reloadFail = true;
                }
            }

            if (reloadTime == 0)
            {
                bool wasSuccessful = CheckEventSuccess();
                reloadSuccess = wasSuccessful;

                // Reset state variables
                reloadFail = false;
                reloadDelay = 30;
                inReloadState = false;
                LoadMagazine();
                clickDelay = 0;

                if (wasSuccessful)
                {
                    OnReloadEventSuccess(player);
                }
                else
                {
                    OnReloadEventFail(player);
                }

                AccessoryKeywords.TriggerReload(player, wasSuccessful);

                // Refresh stats after success/fail events to pick up bonus changes
                RefreshStats();

                ReloadBulletDisplay();
                OnReloadEnd(player);

                // IMPORTANT: Reset zones LAST, after everything else is done
                ResetReloadZones();

                SoundEngine.PlaySound(ReloadFinishSound);
                Projectile.netUpdate = true;
            }
        }

        /// <summary>
        /// Handles reload completion with automatic modifier triggering.
        /// Override OnReloadComplete for gun-specific logic.
        /// </summary>
        public void OnReloadEnd(Player player)
        {
            // Always trigger modifier events
            GunModifierHandler.TriggerGunReload(this, player, reloadSuccess);

            if (reloadSuccess)
                GunModifierHandler.TriggerReloadSuccess(this, player, BulletDisplay);
            else
                GunModifierHandler.TriggerReloadFail(this, player, BulletDisplay);

            // Call overridable method for gun-specific logic
            OnReloadComplete(player, reloadSuccess);
        }

        /// <summary>
        /// Override this method to add gun-specific reload completion effects.
        /// Modifier events are automatically triggered before this method.
        /// </summary>
        protected virtual void OnReloadComplete(Player player, bool wasSuccessful) { }


        public virtual void OnReloadStart(Player player) { }

        /// <summary>
        /// Handles reload zone hits with automatic modifier triggering.
        /// Override OnReloadZoneHit for gun-specific logic.
        /// </summary>
        public void ReloadEventTrigger(Player player, int zoneIndex, int clicksLeft)
        {
            // Always trigger modifier events first
            GunModifierHandler.TriggerReloadZoneHit(this, player, BulletDisplay, zoneIndex, clicksLeft);

            // Call the overridable method for gun-specific logic
            OnReloadZoneHit(player, zoneIndex, clicksLeft);
        }

        /// <summary>
        /// Override this method to add gun-specific reload zone hit effects.
        /// Modifier events are automatically triggered before this method.
        /// </summary>
        protected virtual void OnReloadZoneHit(Player player, int zoneIndex, int clicksLeft) { }

        /// <summary>
        /// Handles reload success with automatic modifier triggering.
        /// Override OnReloadSuccessCore for gun-specific logic.
        /// </summary>
        public void OnReloadEventSuccess(Player player)
        {
            // Call overridable method for gun-specific logic
            OnReloadSuccessCore(player);
        }

        /// <summary>
        /// Override this method to add gun-specific reload success effects.
        /// </summary>
        protected virtual void OnReloadSuccessCore(Player player) { }

        /// <summary>
        /// Handles reload failure with automatic modifier triggering.
        /// Override OnReloadFailCore for gun-specific logic.
        /// </summary>
        public void OnReloadEventFail(Player player)
        {
            // Call overridable method for gun-specific logic
            OnReloadFailCore(player);
        }

        /// <summary>
        /// Override this method to add gun-specific reload failure effects.
        /// </summary>
        protected virtual void OnReloadFailCore(Player player) { }

        private bool CheckEventSuccess()
        {
            int clickedCount = 0;
            for (int i = 0; i < ClickZones.Count; i++)
            {
                bool clicked = ClickZones[i].HasClicked;
                if (clicked) clickedCount++;
            }

            bool success = clickedCount == ClickZones.Count;
            return success;
        }

        private int GetClicksLeft()
        {
            var numLeft = ClickZones.Count;
            foreach (ReloadZone clickZone in ClickZones)
            {
                if (clickZone.HasClicked) numLeft--;
            }

            return numLeft;
        }

        private void ResetReloadZones()
        {
            foreach (ReloadZone clickZone in ClickZones)
            {
                clickZone.HasClicked = false;
            }
        }
    }
}
