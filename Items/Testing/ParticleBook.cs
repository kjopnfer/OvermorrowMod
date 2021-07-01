using OvermorrowMod.Particles;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;

namespace OvermorrowMod.Items.Testing
{
    public class ParticleBook : ModItem
    {
        public override string Texture => "Terraria/Item_" + ItemID.Book;
        public int currentType;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Particle Book");
            Tooltip.SetDefault("Right Click to change particle type");
        }

        public override void SetDefaults()
        {
            item.autoReuse = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            Item.staff[item.type] = true;
            item.consumable = false;
            item.useAnimation = 5;
            item.useTime = 5;
            item.maxStack = 1;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                currentType = (currentType + 1) % Particle.ParticleTypes.Count;
            }
            else
            {
                Particle.CreateParticle(currentType, Main.MouseWorld, Vector2.Zero, Color.White, 1, 1, 0, 1f);
            }
            return true;
        }
    }
}