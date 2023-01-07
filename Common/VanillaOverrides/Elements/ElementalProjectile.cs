using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.VanillaOverrides.Elements
{
    public class ElementalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public HashSet<Element> ElementTypes = new HashSet<Element>() { Element.None };

        public override void SetDefaults(Projectile projectile)
        {
            switch (projectile.type)
            {
                case ProjectileID.WandOfSparkingSpark:
                    ElementTypes = new HashSet<Element>() { Element.Fire };
                    break;
            }
        }
    }
}