using Microsoft.Xna.Framework;

namespace OvermorrowMod.Core.Particles.Modular
{
    public enum EmitShape { Point, Circle, Ring, Cone }
    public enum EmitDirection { OutwardFromShape, FixedAngle, TowardCursor }
    public enum ParticleOrientation { FaceVelocity, Fixed, Spin }
    public enum ParticleEasing { Linear, EaseIn, EaseOut, Sine }

    /// <summary>
    /// The data definition for an effect: every behavior is a knob here, not a subclass.
    /// One spec is rolled into N particles by <see cref="ParticleEmitter"/> and advanced each
    /// frame by <see cref="ParticleSimStep"/>. Authored in code as a literal or tuned in the editor.
    /// </summary>
    public record class ParticleSpec
    {
        // Emission
        public EmitShape Shape { get; set; } = EmitShape.Point;
        public float ShapeRadius { get; set; } = 0f;
        public float ConeSpread { get; set; } = 30f;
        public int Count { get; set; } = 10;
        public float Rate { get; set; } = 0f;

        // Initial (ranges -> per-particle random)
        public float SpeedMin { get; set; } = 2f;
        public float SpeedMax { get; set; } = 4f;
        public EmitDirection DirectionMode { get; set; } = EmitDirection.OutwardFromShape;
        public float Angle { get; set; } = 0f;
        public float SpreadDeg { get; set; } = 360f;
        public int LifetimeMin { get; set; } = 30;
        public int LifetimeMax { get; set; } = 45;
        public float StartScaleMin { get; set; } = 0.3f;
        public float StartScaleMax { get; set; } = 0.5f;
        public float EndScale { get; set; } = 0f;
        public float StartRotationMin { get; set; } = 0f;
        public float StartRotationMax { get; set; } = 0f;

        // Over-life
        public ParticleEasing ScaleEasing { get; set; } = ParticleEasing.Linear;
        public Color StartColor { get; set; } = Color.White;
        public Color EndColor { get; set; } = Color.White;
        public float AlphaFadeInFrac { get; set; } = 0.1f;
        public float AlphaFadeOutFrac { get; set; } = 0.3f;
        public float Drag { get; set; } = 0f;
        public Vector2 Gravity { get; set; } = Vector2.Zero;
        public float AngularVelMin { get; set; } = 0f;
        public float AngularVelMax { get; set; } = 0f;
        public float Turbulence { get; set; } = 0f;

        // Render
        public string Texture { get; set; } = "trace_04";
        public bool Additive { get; set; } = true;
        public ParticleDrawLayer DrawLayer { get; set; } = ParticleDrawLayer.AboveAll;
        public ParticleOrientation Orientation { get; set; } = ParticleOrientation.FaceVelocity;
        public float RotationOffsetDeg { get; set; } = 0f;
        public bool FlipHorizontal { get; set; } = false;
        public bool FlipVertical { get; set; } = false;

        // Material (shader) - "None" for plain draw; otherwise a name from ParticleShaderRegistry.
        public string Shader { get; set; } = "None";
        public Color ShaderColor { get; set; } = Color.White;
        public float ShaderProgress { get; set; } = 1f;
        public bool ShaderProgressFromAge { get; set; } = true;
    }
}
