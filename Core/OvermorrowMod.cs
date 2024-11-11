using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace OvermorrowMod.Core
{
	public class OvermorrowMod : Mod
	{
        public static OvermorrowMod Instance { get; set; }
        public OvermorrowMod() => Instance = this;
    }
}
