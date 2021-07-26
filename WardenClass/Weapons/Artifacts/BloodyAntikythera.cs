using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Artifact;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Weapons.Artifacts
{
    public class BloodyAntikythera : Artifact
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloody Antikythera");
            Tooltip.SetDefault("[c/00FF00:{ Artifact of Wisdom }]\nConsume 2 Soul Essences to summon a miniature Blood Moon\n" +
                "All players within range have their attack increased\n" +
                "'Blood spilled onto the Earth shall rain from the sky'");
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
            item.shoot = ModContent.ProjectileType<RedCloud>();

            soulResourceCost = 2;
        }
    }
}