using OvermorrowMod.Projectiles.Artifact;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.WardenClass.Weapons.Artifacts
{
    public class EarthCrystal : Artifact
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Earthen Crystal");
            Tooltip.SetDefault("[c/00FF00:{ Artifact of Wisdom }]\nConsume 2 Soul Essences to summon an ancient tree\n" +
                "All players within range have their health regen increased\n" +
                "'A crystal that once grew on the World Tree that had long since disappeared'");
        }

        public override void SafeSetDefaults()
        {
            item.width = 22;
            item.height = 40;
            item.rare = ItemRarityID.Orange;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.noMelee = true;
            item.UseSound = SoundID.Item25;
            item.consumable = false;
            item.autoReuse = false;
            item.shoot = ModContent.ProjectileType<WorldTree>();

            soulResourceCost = 2;
        }
    }
}
