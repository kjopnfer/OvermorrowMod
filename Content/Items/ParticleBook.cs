using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items
{
    public class ParticleBook : ModItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.Book;
        public int currentType;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Particle Book");
            Tooltip.SetDefault("Right Click to change particle type");
        }

        public override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.staff[Item.type] = true;
            Item.consumable = false;
            Item.useAnimation = 5;
            Item.useTime = 5;
            Item.maxStack = 1;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                currentType = (currentType + 1) % Particle.ParticleTypes.Count;
                Main.NewText(currentType);
            }
            else
            {
                ParticleSystem.CreateParticle<Orb>(Main.MouseWorld, Vector2.Zero, Color.White, 1f);
                Particle.CreateParticle(currentType, Main.MouseWorld, Vector2.Zero, Color.White, 1, 1, 0, 1f);
            }

            return null;
        }
    }
}