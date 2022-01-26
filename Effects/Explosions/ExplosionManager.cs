using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Effects.Prim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Effects.Explosions
{
    public static class ExplosionManager
    {
        public static readonly List<Texture2D> CircularExplosionTextures = new List<Texture2D>();

        private static Effect circularEffect;
        private static readonly IDictionary<CircularExplosionGenerator, List<CircularExplosionInstance>> generators
            = new Dictionary<CircularExplosionGenerator, List<CircularExplosionInstance>>();
        public static void Load(OvermorrowModFile mod)
        {
            for (int i = 0; i < 2; i++)
            {
                CircularExplosionTextures.Add(mod.GetTexture("Effects/Explosions/Textures/Explosion" + i));
            }

            circularEffect = mod.GetEffect("Effects/Explosions/CircularExplosionShader");
            On.Terraria.Main.DrawDust += Draw;
        }

        private static void Draw(On.Terraria.Main.orig_DrawDust orig, Main self)
        {
            circularEffect.SafeSetParameter("WVP", PrimitiveHelper.GetMatrix());
            circularEffect.SafeSetParameter("screenPosition", Main.screenPosition);

            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;

            device.RasterizerState = rasterizerState;
            device.BlendState = BlendState.AlphaBlend;

            foreach (var kvp in generators)
            {
                var generator = kvp.Key;
                var instances = kvp.Value;

                if (!instances.Any()) continue;

                var tex = generator.Texture;
                device.Textures[0] = tex;
                circularEffect.SafeSetParameter("img0", tex);
                circularEffect.SafeSetParameter("tick", generator.CurrentTick);
                circularEffect.SafeSetParameter("radius", generator.Radius);
                circularEffect.SafeSetParameter("texWidth", tex.Width);
                circularEffect.SafeSetParameter("color", generator.Color);
                circularEffect.CurrentTechnique.Passes["Circular"].Apply();

                var vertices = instances.SelectMany(inst => inst.Vertices).ToArray();

                var indices = CircularExplosionGenerator.GetIndices(instances.Count);

                device.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3);
            }
            orig(self);
        }

        public static void Unload()
        {
            CircularExplosionTextures.Clear();
            circularEffect = null;
            generators.Clear();
            On.Terraria.Main.DrawDust -= Draw;
        }

        /// <summary>
        /// Add an explosion generator to the manager
        /// </summary>
        /// <param name="generator">Generator to add</param>
        public static void AddGenerator(CircularExplosionGenerator generator)
        {
            generators[generator] = new List<CircularExplosionInstance>();
        }

        /// <summary>
        /// Create explosion using given manager. It will be added to the list if it is not already there.
        /// </summary>
        /// <param name="generator">Generator to use</param>
        /// <param name="center">Center of explosion.</param>
        public static void CreateExplosion(CircularExplosionGenerator generator, Vector2 center)
        {
            if (!generators.TryGetValue(generator, out var instances))
            {
                generators[generator] = instances = new List<CircularExplosionInstance>();
            }
            instances.Add(new CircularExplosionInstance(generator, center));
        }

        /// <summary>
        /// Create a single explosion using a generator created on the fly.
        /// </summary>
        /// <param name="texture">Texture to use</param>
        /// <param name="radius">Radius in pixels</param>
        /// <param name="color">Multiplicative color</param>
        /// <param name="center">Center of explosion</param>
        public static void CreateExplosion(Texture2D texture, float radius, Color color, Vector2 center)
        {
            var generator = new CircularExplosionGenerator(texture, radius, color, 0);
            AddGenerator(generator);
            CreateExplosion(generator, center);
            generator.Finished = true;
        }
        /// <summary>
        /// Create a single explosion using a generator created on the fly.
        /// </summary>
        /// <param name="texture">Texture to use given by index in texture list</param>
        /// <param name="radius">Radius in pixels</param>
        /// <param name="color">Multiplicative color</param>
        /// <param name="center">Center of explosion</param>
        public static void CreateExplosion(int texture, float radius, Color color, Vector2 center)
        {
            var generator = new CircularExplosionGenerator(texture, radius, color, 0);
            AddGenerator(generator);
            CreateExplosion(generator, center);
            generator.Finished = true;
        }
        /// <summary>
        /// Called globally from OvermorrowModFile.
        /// </summary>
        public static void Update()
        {
            var toRemove = new List<CircularExplosionGenerator>();
            foreach (var kvp in generators)
            {
                kvp.Key.CurrentTick++;
                kvp.Value.RemoveAll(inst => kvp.Key.CurrentTick - inst.InitialTick >= kvp.Key.Duration);
                if (!kvp.Value.Any() && (kvp.Key.Finished || kvp.Key.CurrentTick > kvp.Key.MaxLifeTime))
                {
                    toRemove.Add(kvp.Key);
                }
            }
            foreach (var gen in toRemove)
            {
                generators.Remove(gen);
            }
        }
    }
}
