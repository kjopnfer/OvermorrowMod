using OvermorrowMod.Core.WorldGeneration;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Waypoints
{
    public class WaypointTravel : ModSystem
    {
        private const float MaxSourceDistance = 12 * 16;

        /// <summary>
        /// The waypoint the full map was opened from. Lit waypoints linked to it are travel targets while it is set.
        /// </summary>
        public static Waypoint_TE Source { get; private set; }

        public static void OpenMap(Waypoint_TE source)
        {
            Source = source;

            Main.playerInventory = false;
            Main.LocalPlayer.SetTalkNPC(-1);
            Main.npcChatCornerItem = 0;
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Main.mapFullscreenScale = 2.5f;
            Main.mapFullscreen = true;
            Main.resetMapFull = true;
        }

        public override void PostUpdatePlayers()
        {
            if (Source == null) return;

            bool exists = TileEntity.ByID.TryGetValue(Source.ID, out var te) && te == Source;
            if (!exists || !Main.mapFullscreen || SubworldTimer.IsExpired || Main.LocalPlayer.Distance(Source.WorldCenter) > MaxSourceDistance)
                Source = null;
        }

        public override void OnWorldUnload() => Source = null;
    }
}
