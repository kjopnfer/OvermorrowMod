using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace OvermorrowMod.Quests
{
    public interface IQuestReward
    {
        void Give(Player player);
        string Description { get; }
    }
}
