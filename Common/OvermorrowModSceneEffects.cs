using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Netcode;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Content.Buffs.Hexes;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Content.NPCs.Bosses.Eye;
using OvermorrowMod.Core;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public class RavensfellSkyScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override void SpecialVisuals(Player player, bool isActive)
        {
            //player.ManageSpecialBiomeVisuals()
            if (isActive)
                SkyManager.Instance.Activate("OM:RavensfellSky");
            else
                SkyManager.Instance.Deactivate("OM:RavensfellSky");
        }

        public override bool IsSceneEffectActive(Player player)
        {
            return player.ZoneForest;
        }
    }
}