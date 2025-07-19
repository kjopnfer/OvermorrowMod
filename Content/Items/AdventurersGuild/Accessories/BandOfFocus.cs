using OvermorrowMod.Common;
using OvermorrowMod.Common.Items;
using OvermorrowMod.Core.Items.Accessories;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.AdventurersGuild.Accessories
{
    public class BandOfFocus : OvermorrowAccessory
    {
        public override string Texture => AssetDirectory.GuildItems + Name;
        protected override void SafeSetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, gold: 1, silver: 50, copper: 0);
        }

        protected override void SetAccessoryEffects(AccessoryDefinition definition)
        {
            definition.AddVigorEffect(
                condition: (player) => true, // Vigor keyword already checks for 80%+ health
                effect: (player) =>
                {
                    player.GetCritChance(DamageClass.Magic) += 15;
                }
            );
        }
    }
}