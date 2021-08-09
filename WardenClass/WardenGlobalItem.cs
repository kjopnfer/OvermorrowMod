using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Piercing;
using OvermorrowMod.Projectiles.Ranged;
using OvermorrowMod.Items.Weapons.PreHardmode.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Items.Other;

namespace OvermorrowMod.WardenClass
{
    public class WardenGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public WardenGlobalItem() {
            soulGainChance = 0;
        }
        public override GlobalItem Clone(Item item, Item itemClone)
		{
			WardenGlobalItem myClone = (WardenGlobalItem)base.Clone(item, itemClone);

            myClone.soulGainChance = soulGainChance;
            return myClone;
		}
        public float soulGainChance { get; set; }
    }
}