using System;
using Terraria;

namespace OvermorrowMod.Core.Items.Accessories
{
    public class AccessoryEffect
    {
        public Func<Player, object[], bool> Condition { get; set; }
        public Action<Player, object[]> Effect { get; set; }

        public AccessoryEffect(Func<Player, object[], bool> condition, Action<Player, object[]> effect)
        {
            Condition = condition;
            Effect = effect;
        }

        public void TryExecute(Player player, params object[] args)
        {
            if (Condition(player, args))
            {
                Effect(player, args);
            }
        }
    }
}