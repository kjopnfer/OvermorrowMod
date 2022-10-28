namespace OvermorrowMod.Core
{
    public static class AssetDirectory
    {
        #region Assets
        public const string Assets = "OvermorrowMod/Assets/";

        public const string Empty = Assets + "Empty";

        public const string Textures = Assets + "Textures/";

        public const string Gores = Assets + "Gores/";

        public const string Chains = Textures + "Chains/";

        public const string UI = Textures + "UI/";

        public const string Bestiary = Textures + "Bestiary/";

        public const string Trails = Assets + "Textures/Trails/";
        public const string FullTrail = Assets + "Textures/Trails/"; // Uhh, this includes the ModFolder

        #endregion

        #region Content Textures
        public const string Content = "OvermorrowMod/Content/";

        public const string Magic = Content + "Items/Weapons/Magic/";
        public const string Melee = Content + "Items/Weapons/Melee/";
        public const string Ranged = Content + "Items/Weapons/Ranged/";
        public const string Summon = Content + "Items/Weapons/Summon/";

        public const string NPC = Content + "NPCs/";
        public const string Boss = Content + "NPCs/Bosses/";

        public const string Tiles = Content + "Tiles/";
        public const string WorldGen = Content + "WorldGeneration/";

        #endregion

        #region Other
        public const string Dialogue = "OvermorowMod/Common/Cutscenes/Dialogue/";
        #endregion

        public const string Popup = "Content/UI/Popups/";
    }
}