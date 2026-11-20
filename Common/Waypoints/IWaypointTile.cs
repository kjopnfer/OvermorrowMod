namespace OvermorrowMod.Common.Waypoints
{
    public interface IWaypointTile
    {
        /// <summary>
        /// Returns the waypoint for the multitile containing (<paramref name="i"/>, <paramref name="j"/>), placing it if missing.
        /// </summary>
        Waypoint_TE GetOrCreateWaypoint(int i, int j);
    }
}
