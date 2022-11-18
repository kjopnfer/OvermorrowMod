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
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.autoReuse = true;
            Item.maxStack = 1;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool? UseItem(Player player)
        {
            Item.useAnimation = 15;
            Item.useTime = 15;

            /*if (player.altFunctionUse == 2)
            {
                currentType = (currentType + 1) % Particle.ParticleTypes.Count;
                Main.NewText(currentType + " " + currentType.GetType().Name);
            }
            else
            {
                Particle.CreateParticle(currentType, Main.MouseWorld, Vector2.Zero, Color.White, 1, 1, 0, 1f);
            }*/

            for (int i = 0; i < 36; i++)
            {
                //Vector2 randomOffset = new Vector2(Main.rand.Next(-180, 180) * 4, Main.rand.Next(-10, 10) * 4);
                Vector2 randomOffset = new Vector2(Main.rand.Next(-10, 10) * 4, Main.rand.Next(-120, 120) * 4);

                //float scale = Main.rand.NextFloat(0, 1f);
                Particle.CreateParticle(Particle.ParticleType<DarkSmog>(), Main.MouseWorld + randomOffset, Vector2.Zero, Color.DarkSlateGray, 1, 0f);
            }

            return true;
        }
    }
}