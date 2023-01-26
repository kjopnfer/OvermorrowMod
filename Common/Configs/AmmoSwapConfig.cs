using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla.Bows;
using Terraria.ModLoader.Config;
using System.ComponentModel;

namespace OvermorrowMod.Common.Configs
{
    public class AmmoSwapConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Ammo Quickswap Position")] // Headers are like titles in a config. You only need to declare a header on the item it should appear over, not every item in the category.
        [Label("Position UI on Mouse Cursor")] // A label is the text displayed next to the option. This should usually be a short description of what it does.
        [Tooltip("The UI is drawn on either the player or the mouse.\nIf disabled then it is drawn on the player instead.")] // A tooltip is a description showed when you hover your mouse over the option. It can be used as a more in-depth explanation of the option.
        
        [DefaultValue(true)] // This sets the configs default value.
        public bool MouseAnchor; // To see the implementation of this option, see ExampleWings.cs
    }
}