using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core.WorldGeneration;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.UI;

namespace OvermorrowMod.Common.Waypoints
{
    public class WaypointMapLayer : ModMapLayer
    {
        public override void Draw(ref MapOverlayDrawContext context, ref string text)
        {
            if (SubworldTimer.IsExpired) return;

            Texture2D icon = ModContent.Request<Texture2D>(AssetDirectory.UI + "WaypointIcon").Value;
            byte animatedRow = (byte)((int)(Main.GlobalTimeWrappedHourly * 4f) % 2);

            foreach (var te in TileEntity.ByID.Values)
            {
                if (te is not Waypoint_TE waypoint || !waypoint.IsLit) continue;

                bool isTarget = WaypointTravel.IsTarget(waypoint);
                var frame = new SpriteFrame(1, 2, 0, isTarget ? animatedRow : (byte)0) { PaddingY = 0 };
                Rectangle area = waypoint.TileArea;
                Vector2 position = new Vector2(area.X + area.Width / 2f, area.Y);

                bool hovered = context.Draw(icon, position, Color.White, frame, 1f, isTarget ? 1.25f : 1f, Alignment.Bottom).IsMouseOver;
                if (!hovered || !isTarget || !Main.mapFullscreen) continue;

                if (WaypointTravel.IsCombatLocked())
                {
                    text = "Can't travel during combat";
                    continue;
                }

                text = "Travel";
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    Main.mouseLeftRelease = false;
                    PlayerInput.LockGamepadButtons("MouseLeft");
                    WaypointTravel.TravelTo(waypoint);
                    return;
                }
            }
        }
    }
}
