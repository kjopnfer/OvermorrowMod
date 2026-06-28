using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Particles.Modular
{
    /// <summary>
    /// Maps a shader name to its Effect + a parameter-apply delegate fed from the spec and the live
    /// particle state. New .fx shaders register here without touching the rest of the system.
    /// </summary>
    public static class ParticleShaderRegistry
    {
        public const string None = "None";

        public delegate void ApplyShader(Effect effect, ParticleSpec spec, ParticleState state);

        private static readonly Dictionary<string, (Func<Effect> Get, ApplyShader Apply, string Pass)> _shaders = new();

        public static void Register(string name, Func<Effect> get, ApplyShader apply, string pass)
            => _shaders[name] = (get, apply, pass);

        public static void Clear() => _shaders.Clear();

        public static IEnumerable<string> Names => new[] { None }.Concat(_shaders.Keys);

        public static bool TryGet(string name, out Effect effect, out ApplyShader apply, out string pass)
        {
            effect = null; apply = null; pass = null;
            if (string.IsNullOrEmpty(name) || name == None) return false;
            if (!_shaders.TryGetValue(name, out var entry)) return false;
            effect = entry.Get();
            apply = entry.Apply;
            pass = entry.Pass;
            return effect != null;
        }

        /// <summary>
        /// The one wired example: ColorFill tints the sprite toward ShaderColor by a progress value
        /// (driven by particle age by default). New shaders are added with more Register calls.
        /// </summary>
        public static void RegisterDefaults()
        {
            Register("ColorFill",
                () => OvermorrowModFile.Instance.ColorFill.Value,
                (fx, spec, state) =>
                {
                    fx.Parameters["ColorFillColor"].SetValue(spec.ShaderColor.ToVector3());
                    float progress = spec.ShaderProgressFromAge && state.Lifetime > 0f
                        ? MathHelper.Clamp(state.Age / state.Lifetime, 0f, 1f)
                        : spec.ShaderProgress;
                    fx.Parameters["ColorFillProgress"].SetValue(progress);
                },
                "ColorFill");
        }
    }

    public class ParticleShaderSystem : ModSystem
    {
        public override void PostSetupContent() => ParticleShaderRegistry.RegisterDefaults();
        public override void Unload() => ParticleShaderRegistry.Clear();
    }
}
