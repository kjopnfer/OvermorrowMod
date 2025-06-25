using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Effects
{
    public class AdditiveDrawing : ILoadable
    {
        public void Load(Mod mod)
        {
            if (Main.dedServ)
                return;

            On_Main.DrawDust += DrawAdditive;
        }

        public void Unload()
        {
        }

        private void DrawAdditive(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            Main.spriteBatch.Begin(default, BlendState.Additive, SamplerState.PointWrap, default, RasterizerState.CullNone, default, Main.GameViewMatrix.TransformationMatrix);

            for (int k = 0; k < Main.maxProjectiles; k++)
            {
                if (Main.projectile[k].active && Main.projectile[k].ModProjectile is IDrawAdditive)
                    (Main.projectile[k].ModProjectile as IDrawAdditive).DrawAdditive(Main.spriteBatch);
            }

            for (int k = 0; k < Main.maxNPCs; k++)
            {
                if (Main.npc[k].active && Main.npc[k].ModNPC is IDrawAdditive)
                    (Main.npc[k].ModNPC as IDrawAdditive).DrawAdditive(Main.spriteBatch);
            }

            ParticleManager.DrawAdditive(Main.spriteBatch);

            Main.spriteBatch.End();
        }
    }
}
