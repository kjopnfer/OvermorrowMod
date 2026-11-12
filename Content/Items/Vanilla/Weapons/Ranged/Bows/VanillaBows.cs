using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using Terraria.ID;

namespace OvermorrowMod.Content.Items.Vanilla.Weapons.Ranged.Bows
{
    public class WoodenBow_Held : VanillaBow_Held
    {
        public override string Texture => AssetDirectory.Resprites + "WoodenBow";
        public override int ParentItem => ItemID.WoodenBow;
        protected override Color BowStringColor => new Color(83, 65, 50);
    }

    public class CopperBow_Held : VanillaBow_Held
    {
        public override string Texture => AssetDirectory.Resprites + "CopperBow";
        public override int ParentItem => ItemID.CopperBow;
        protected override Color BowStringColor => new Color(114, 81, 56);
    }

    public class TinBow_Held : VanillaBow_Held
    {
        public override string Texture => AssetDirectory.Resprites + "TinBow";
        public override int ParentItem => ItemID.TinBow;
        protected override Color BowStringColor => new Color(89, 51, 53);
    }

    public class IronBow_Held : VanillaBow_Held
    {
        public override string Texture => AssetDirectory.Resprites + "IronBow";
        public override int ParentItem => ItemID.IronBow;
        protected override Color BowStringColor => new Color(114, 81, 56);
    }

    public class LeadBow_Held : VanillaBow_Held
    {
        public override string Texture => AssetDirectory.Resprites + "LeadBow";
        public override int ParentItem => ItemID.LeadBow;
        protected override Color BowStringColor => new Color(89, 51, 53);
    }

    public class SilverBow_Held : VanillaBow_Held
    {
        public override string Texture => AssetDirectory.Resprites + "SilverBow";
        public override int ParentItem => ItemID.SilverBow;
        protected override Color BowStringColor => new Color(75, 86, 95);
    }

    public class TungstenBow_Held : VanillaBow_Held
    {
        public override string Texture => AssetDirectory.Resprites + "TungstenBow";
        public override int ParentItem => ItemID.TungstenBow;
        protected override Color BowStringColor => new Color(84, 75, 95);
    }

    public class GoldBow_Held : VanillaBow_Held
    {
        public override string Texture => AssetDirectory.Resprites + "GoldBow";
        public override int ParentItem => ItemID.GoldBow;
        protected override Color BowStringColor => new Color(75, 86, 95);
    }

    public class PlatinumBow_Held : VanillaBow_Held
    {
        public override string Texture => AssetDirectory.Resprites + "PlatinumBow";
        public override int ParentItem => ItemID.PlatinumBow;
        protected override Color BowStringColor => new Color(84, 75, 95);
    }

    public class DemonBow_Held : VanillaBow_Held
    {
        public override string Texture => AssetDirectory.Resprites + "DemonBow";
        public override int ParentItem => ItemID.DemonBow;
        protected override Color BowStringColor => new Color(65, 45, 85);
    }

    public class TendonBow_Held : VanillaBow_Held
    {
        public override string Texture => AssetDirectory.Resprites + "TendonBow";
        public override int ParentItem => ItemID.TendonBow;
        protected override Color BowStringColor => new Color(224, 12, 17);
    }

    public class BorealWoodBow_Held : VanillaBow_Held
    {
        public override string Texture => AssetDirectory.Resprites + "BorealWoodBow";
        public override int ParentItem => ItemID.BorealWoodBow;
        protected override Color BowStringColor => new Color(29, 24, 21);
    }

    public class PalmWoodBow_Held : VanillaBow_Held
    {
        public override string Texture => AssetDirectory.Resprites + "PalmWoodBow";
        public override int ParentItem => ItemID.PalmWoodBow;
        protected override Color BowStringColor => new Color(44, 34, 28);
    }

    public class EbonwoodBow_Held : VanillaBow_Held
    {
        public override string Texture => AssetDirectory.Resprites + "EbonwoodBow";
        public override int ParentItem => ItemID.EbonwoodBow;
        protected override Color BowStringColor => new Color(31, 29, 41);
    }

    public class ShadewoodBow_Held : VanillaBow_Held
    {
        public override string Texture => AssetDirectory.Resprites + "ShadewoodBow";
        public override int ParentItem => ItemID.ShadewoodBow;
        protected override Color BowStringColor => new Color(24, 31, 44);
    }

    public class RichMahoganyBow_Held : VanillaBow_Held
    {
        public override string Texture => AssetDirectory.Resprites + "RichMahoganyBow";
        public override int ParentItem => ItemID.RichMahoganyBow;
        protected override Color BowStringColor => new Color(50, 24, 26);
    }

    public class MoltenFury_Held : VanillaBow_Held
    {
        public override string Texture => AssetDirectory.Resprites + "MoltenFury";
        public override int ParentItem => ItemID.MoltenFury;
        protected override Color BowStringColor => new Color(238, 102, 70);
        protected override int ConvertArrowItem => ItemID.WoodenArrow;
        protected override int ForcedArrowProjectile => ProjectileID.FireArrow;
    }
}
