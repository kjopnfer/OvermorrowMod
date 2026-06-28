using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace OvermorrowMod.Core.Particles.Modular
{
    /// <summary>
    /// Drives the in-game particle editor: holds the working spec, runs the preview loop, blocks world
    /// interaction over the panel, and spawns the spec at the cursor (left-click = burst, right-hold = stream).
    /// </summary>
    public class ParticleEditorSystem : ModSystem
    {
        public static bool Active;
        public static ParticleSpec Spec = new();
        public static readonly ParticleSystem Preview = new();

        private static int previewTick;
        private static bool prevLeft;

        public override void UpdateUI(GameTime gameTime)
        {
            if (!Active || Main.gameMenu) return;

            // Block world interaction while the cursor is over the panel or the texture picker.
            bool overPanel = ParticleEditorUI.IsMouseOverUI();
            if (overPanel) Main.LocalPlayer.mouseInterface = true;

            // Looping preview burst inside the panel's preview pane.
            if (++previewTick >= 45)
            {
                previewTick = 0;
                Preview.Emit(Spec, Vector2.Zero);
            }
            Preview.Update();

            // World spawn when not interacting with the panel.
            if (!overPanel)
            {
                bool leftPressed = Main.mouseLeft && !prevLeft;
                if (leftPressed) ParticleEmitter.EmitWorld(Spec, Main.MouseWorld);     // single burst
                if (Main.mouseRight) ParticleEmitter.EmitWorldOne(Spec, Main.MouseWorld); // hold to stream
            }

            prevLeft = Main.mouseLeft;
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (index == -1) return;

            layers.Insert(index, new LegacyGameInterfaceLayer("OvermorrowMod: Particle Editor",
                () =>
                {
                    if (Active) ParticleEditorUI.Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.None));
        }
    }
}
