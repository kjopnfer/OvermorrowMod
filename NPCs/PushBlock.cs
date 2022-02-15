using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Buffs;
using OvermorrowMod.Particles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;


namespace OvermorrowMod.NPCs
{
    public class PushBlock : PushableNPC
    {
        bool RunOnce = true;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("A");
        }

        public override void SetDefaults()
        {
            npc.width = 57;
            npc.height = 64;
            npc.aiStyle = -1;
            npc.noGravity = false;
            npc.dontTakeDamage = true;
            npc.lifeMax = 100;
            npc.knockBackResist = 0f;
            npc.dontCountMe = true;
            npc.chaseable = false;
        }
    }
}
