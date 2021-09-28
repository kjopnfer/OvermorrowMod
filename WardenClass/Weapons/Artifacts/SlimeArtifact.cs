using Terraria.ID;

namespace OvermorrowMod.WardenClass.Weapons.Artifacts
{
    public class SlimeArtifact : Artifact
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slime Chalice");
            Tooltip.SetDefault("[c/EBDE34:{ Artifact of Courage }]\n" +
                "Consume a Soul Essence to gain [c/00FF00:Gelatin Physique]\n" +
                "While active, every first jump spawns projectiles and restores 2 health\n" +
                "Additionally, grants increased jump height\n" +
                "All players on the same team gain the same buff for 3 minutes\n" +
                "'Consume the slime chalice'");
        }

        public override void SafeSetDefaults()
        {
            item.width = 42;
            item.height = 42;
            item.rare = ItemRarityID.Green;
            item.useAnimation = 60;
            item.useTime = 60;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.noMelee = true;
            item.UseSound = SoundID.Item103;
            item.consumable = false;
            item.autoReuse = false;

            soulResourceCost = 1;
            defBuffDuration = 3600; // 1 minute
        }
    }
}