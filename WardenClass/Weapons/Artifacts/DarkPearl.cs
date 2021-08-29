using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Artifact;
using OvermorrowMod.Projectiles.Artifact.DarkPortal;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Weapons.Artifacts
{
    public class DarkPearl : Artifact
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Pearl");
            Tooltip.SetDefault("[c/DE3A28:{ Artifact of Power }]\n" +
                               "Consume 1 Soul Essence to summon a Dark Portal\n" +
                               "The Dark Portal will summon Dark Serpents to fight for you\n" +
                               "'A blackened infection, a moon-lit reflection,\nBeheld the world, twisted to perfection.'");
        }

        public override void SafeSetDefaults()
        {
            item.width = 42;
            item.height = 42;
            item.rare = ItemRarityID.Orange;
            item.useAnimation = 60;
            item.useTime = 60;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.noMelee = true;
            item.UseSound = SoundID.Item103;
            item.consumable = false;
            item.autoReuse = false;
            item.shoot = ModContent.ProjectileType<DarkPortal>();
            item.damage = 24;

            soulResourceCost = 1;
        }
    }
}