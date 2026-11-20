using Microsoft.Xna.Framework;
using OvermorrowMod.Core.Globals;
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
        /// The waypoint the full map was opened from. While set, every other lit waypoint is a travel target.
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

        public static bool IsTarget(Waypoint_TE waypoint) => Source != null && waypoint != Source && waypoint.IsLit;

        public static void TravelTo(Waypoint_TE target)
        {
            Source = null;
            Main.mapFullscreen = false;

            Player player = Main.LocalPlayer;
            Vector2 destination = target.LandingPoint(player);
            player.Teleport(destination, -1);
            player.velocity = Vector2.Zero;
            player.fallStart = (int)(destination.Y / 16f);
            player.gfxOffY = 0f;
            player.oldPosition = destination;
            GlobalPlayer.RequestTeleport(destination);

            player.immune = true;
            player.immuneTime = 60;
            SoundEngine.PlaySound(SoundID.Item20, target.WorldCenter);
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
