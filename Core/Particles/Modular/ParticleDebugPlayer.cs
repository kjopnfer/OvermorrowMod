using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Input;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Particles.Modular
{
    /// <summary>
    /// Temporary Phase 1 verification: the ParticleDebugSpawn keybind fires a demo spec at the cursor
    /// that exercises drag + gravity + spin + color/scale-over-life at once. Replaced by the editor.
    /// </summary>
    public class ParticleDebugPlayer : ModPlayer
    {
        private static readonly ParticleSpec Demo = new()
        {
            Shape = EmitShape.Point,
            Count = 20,
            DirectionMode = EmitDirection.OutwardFromShape,
            SpeedMin = 8f,
            SpeedMax = 13f,
            Drag = 0.10f,
            Gravity = new Vector2(0f, 0.15f),
            AngularVelMin = 0.04f,
            AngularVelMax = 0.09f,
            Orientation = ParticleOrientation.Spin,
            LifetimeMin = 40,
            LifetimeMax = 70,
            StartScaleMin = 0.4f,
            StartScaleMax = 0.8f,
            EndScale = 0.05f,
            ScaleEasing = ParticleEasing.EaseOut,
            StartColor = new Color(255, 240, 185),
            EndColor = new Color(120, 22, 0),
            Additive = true,
            Texture = "trace_04",
        };

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (OvermorrowKeybinds.ParticleDebugKey?.JustPressed == true)
                ParticleEmitter.EmitWorld(Demo, Main.MouseWorld);

            if (OvermorrowKeybinds.ParticleEditorKey?.JustPressed == true)
            {
                ParticleEditorSystem.Active = !ParticleEditorSystem.Active;
                if (!ParticleEditorSystem.Active) ParticleEditorUI.ClearFocus();
            }
        }
    }
}
