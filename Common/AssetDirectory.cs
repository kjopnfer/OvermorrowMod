namespace OvermorrowMod.Common
{
    public static class AssetDirectory
    {
        #region Base
        public const string Assets = "OvermorrowMod/Assets/";

        public const string Empty = Assets + "Empty";
        public const string Textures = Assets + "Textures/";

        public const string Backgrounds = Assets + "Backgrounds/";
        public const string Buffs = Assets + "Sprites/Buffs/";
        public const string Items = Assets + "Sprites/Items/";
        public const string NPCs = Assets + "Sprites/NPCs/";
        public const string Tiles = Assets + "Sprites/Tiles/";
        public const string Projectiles = Assets + "Sprites/Projectiles/";
        public const string Trails = Textures + "Trails/";


        // For TexGen mapping, not shader mapping
        public const string TextureMaps = Assets + "Maps/";
        #endregion

        public const string ArchiveTiles = Tiles + "Archives/";
        public const string ArchiveNPCs = NPCs + "Archives/";
        public const string ArchiveItems = Items + "Archives/";
        public const string ArchiveProjectiles = Projectiles + "Archives/";
    }
}