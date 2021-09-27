using OvermorrowMod.Projectiles.Artifact;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.WardenClass.Weapons.Artifacts
{
    public class PillarArtifact : Artifact
    {
        public override string Texture => "OvermorrowMod/WardenClass/Weapons/Artifacts/SlimeArtifact";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pillar of Fortitude");
            Tooltip.SetDefault("[c/00FF00:{ Artifact of Wisdom }]\nConsume 2 Soul Essences to summon a Pillar\n" +
                "All players within range have their defense increased by 6 and become immune to knockback\n" +
                "'Whose masters do I need to awaken?'");
        }

        public override void SafeSetDefaults()
        {
            item.width = 42;
            item.height = 42;
            item.rare = ItemRarityID.Orange;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.noMelee = true;
            item.UseSound = SoundID.Item103;
            item.consumable = false;
            item.autoReuse = false;
            item.shoot = ModContent.ProjectileType<Pillar>();

            soulResourceCost = 2;
        }
    }
}