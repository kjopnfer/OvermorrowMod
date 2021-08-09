using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Prefixes
{
    public class WardenPrefixes : ModPrefix 
    {
        private readonly float _power;

        public override bool CanRoll(Item item)
            => true;

        public override PrefixCategory Category
            => PrefixCategory.Custom;

        public WardenPrefixes() {
            
        }
        public WardenPrefixes(float power) {
            _power = power;
        }

        public override bool Autoload(ref string name) {
            if (!base.Autoload(ref name)) {
                return false;
            }

            mod.AddPrefix("Cursed", new WardenPrefixes(0.5f));
            mod.AddPrefix("Enchanted", new WardenPrefixes(1));
            mod.AddPrefix("Faithful", new WardenPrefixes(1.5f));
            mod.AddPrefix("Bound", new WardenPrefixes(2));
            return false;
        }

        public override void Apply(Item item) 
            => item.GetGlobalItem<OvermorrowMod.WardenClass.WardenGlobalItem>().soulGainChance = _power;

		public override void ModifyValue(ref float valueMult) {
			float multiplier = 1f + 0.05f * _power;
			valueMult *= multiplier;
		}
    }
}