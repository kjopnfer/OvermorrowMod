using OvermorrowMod.Common.BackgroundObjects;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;

namespace OvermorrowMod.Common.Detours
{
    // Code adapted from the Starlight River repository
    public class BackgroundObjects
    { 
        public static void Main_DrawBackgroundObjects(On.Terraria.Main.orig_DrawBackground orig, Main self)
        {
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
            foreach (KeyValuePair<int, TileEntity> entity in TileEntity.ByID)
            {
                if (entity.Value is BaseBackgroundObject backgroundObject && backgroundObject.IsOnScreen() && backgroundObject.IsTileValidForEntity(entity.Value.Position.X, entity.Value.Position.Y))
                    backgroundObject.DrawObject(Main.spriteBatch);
            }

            //Main.spriteBatch.End();

            orig(self);
        }
    }
}