using OvermorrowMod.Common.Input;
using OvermorrowMod.Common.Items.Guns;
using OvermorrowMod.Core.Interfaces;
using System.Collections.Generic;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Globals
{
    public class GunPlayer : ModPlayer
    {
        /// <summary>
        /// Used to preserve data between guns whenever swapped to prevent reload skipping
        /// </summary>
        public Dictionary<int, HeldGunInfo> playerGunInfo = new Dictionary<int, HeldGunInfo>();
        public List<IGunModifier> ActiveModifiers { get; private set; } = new List<IGunModifier>();

        /// <summary>
        /// Set when the player presses the reload keybind; consumed by the held gun's AI to enter a reload on demand.
        /// </summary>
        public bool ReloadRequested;

        public int BulletArmorPenetration;

        public bool CowBoySet;
        public bool GraniteLauncher;

        public bool ChicagoBonusShots = false;
        public bool FarlanderPierce = false;
        public bool WildEyeCrit = false;

        public int GraniteEnergyCount = 0;
        public float FarlanderCharge = 0;
        public int FarlanderSpeedBoost = 0;
        public int MusketInaccuracy = 0;

        public override void ResetEffects()
        {
            BulletArmorPenetration = 0;

            CowBoySet = false;
            GraniteLauncher = false;

            ActiveModifiers.Clear();
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (OvermorrowKeybinds.ReloadKeybind?.JustPressed == true)
                ReloadRequested = true;
        }

        public void AddGunModifier(IGunModifier modifier)
        {
            ActiveModifiers.Add(modifier);
        }
    }
}