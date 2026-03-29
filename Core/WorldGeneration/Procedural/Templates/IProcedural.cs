namespace OvermorrowMod.Core.WorldGeneration.Procedural.Templates
{
    public interface IProcedural
    {
        /// <summary>
        /// Build this piece from the given entry anchor.
        /// For connectors: returns the exit anchor where the next room should attach.
        /// For furniture: returns default SocketAnchor.
        /// </summary>
        SocketAnchor Build(SocketAnchor entry, int fillTileType, int liningTileType);
    }
}
