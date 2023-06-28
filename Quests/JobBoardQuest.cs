using Microsoft.Xna.Framework;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Quests.Requirements;
using OvermorrowMod.Quests.State;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Quests
{
    public abstract class JobBoardQuest : BaseQuest
    {
        public virtual int BoardID { get; }
    }
}