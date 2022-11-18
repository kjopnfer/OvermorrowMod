using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Debuffs;
using OvermorrowMod.Content.Buffs.Hexes;
using OvermorrowMod.Content.Items.Accessories;
using OvermorrowMod.Content.NPCs;
using System;
using OvermorrowMod.Content.UI;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using OvermorrowMod.Content.Items.Consumable;
using System.Collections.Generic;
using OvermorrowMod.Common.Cutscenes;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using ReLogic.Content;
using Terraria.GameContent;

namespace OvermorrowMod.Common.NPCs.Mercenary
{
    public class MercenaryPlayer : ModPlayer
    {
        public Dictionary<int, BaseMercenary> Mercenaries = new Dictionary<int, BaseMercenary>();

        public void GetMercenary(int type)
        {

        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
        }

        public override void Load()
        {
            OvermorrowModFile mod = OvermorrowModFile.Instance;

            foreach (Type type in mod.Code.GetTypes())
            {
                if (type.IsSubclassOf(typeof(BaseMercenary)) && !type.IsAbstract && type != typeof(BaseMercenary))
                {
                    BaseMercenary mercenary = (BaseMercenary)Activator.CreateInstance(type);
                    //quest.SetDefaults();
                    Mercenaries.Add(mercenary.MercenaryID, mercenary);
                    //QuestTypes.Add(type, quest);
                }
            }
        }
    }
}