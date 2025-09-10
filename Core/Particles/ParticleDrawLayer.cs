namespace OvermorrowMod.Core.Particles
{
    public enum ParticleDrawLayer
    {
        /// <summary>
        /// This doesnt work
        /// </summary>
        BehindTiles,

        /// <summary>
        /// This doesnt work
        /// </summary>
        AboveTiles,

        BehindNPCs,
        BehindProjectiles,

        /// <summary>
        /// This doesnt work
        /// </summary>
        BehindPlayers,

        /// <summary>
        /// Draws on the interface layer, gets hidden if interface is disabled.
        /// </summary>
        AboveAll,

        /// <summary>
        /// Draws on the interface layer, gets hidden if interface is disabled.
        /// </summary>
        Interface
    }
}