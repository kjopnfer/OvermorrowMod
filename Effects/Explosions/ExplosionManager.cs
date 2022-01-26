using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Effects.Prim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Effects.Explosions
{
    public struct VertexPositionCenterTick : IVertexType
    {
        public Vector3 Position;
        public Vector2 Center;
        public int Tick;

        public VertexPositionCenterTick(Vector3 position, Vector2 center, int tick)
        {
            Position = position;
            Center = center;
            Tick = tick;
        }

        private readonly static VertexDeclaration declaration = new VertexDeclaration(new VertexElement[]
        {
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(Marshal.SizeOf(typeof(Vector3)), VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(Marshal.SizeOf(typeof(Vector3)) + Marshal.SizeOf(typeof(Vector2)),
                VertexElementFormat.Byte4, VertexElementUsage.TextureCoordinate, 1),
        });

        public VertexDeclaration VertexDeclaration => declaration;
    }

    public class CircularExplosionInstance
    {
        public int InitialTick { get; }
        public VertexPositionCenterTick[] Vertices { get; }
        public Vector2 Center { get; }
        public CircularExplosionGenerator Generator { get; }

        public CircularExplosionInstance(CircularExplosionGenerator generator, Vector2 center)
        {
            Vertices = generator.GenerateVertices(center);
            InitialTick = generator.CurrentTick;
            Center = center;
            Generator = generator;
        }
    }


    public class CircularExplosionGenerator
    {
        public bool Finished { get; set; }
        public int CurrentTick { get; set; }
        public int Texture { get; }
        public float Radius { get; }
        public Color Color { get; }
        public int MaxTick { get; set; }
        public CircularExplosionGenerator(int texture, float radius, Color color, int maxTick)
        {
            Texture = texture;
            Radius = radius;
            Color = color;
            CurrentTick = 0;
            MaxTick = maxTick;
        }

        public VertexPositionCenterTick[] GenerateVertices(Vector2 center)
        {
            var res = new VertexPositionCenterTick[4];
            res[0] = new VertexPositionCenterTick(
                new Vector3(center.X - Radius, center.Y + Radius, 0), center, CurrentTick);
            res[1] = new VertexPositionCenterTick(
                new Vector3(center.X - Radius, center.Y - Radius, 0), center, CurrentTick);
            res[2] = new VertexPositionCenterTick(
                new Vector3(center.X + Radius, center.Y + Radius, 0), center, CurrentTick);
            res[3] = new VertexPositionCenterTick(
                new Vector3(center.X + Radius, center.Y - Radius, 0), center, CurrentTick);
            return res;
        }

        public static int[] GetIndices(int count)
        {
            var indices = new int[count * 6];
            // Two triangles, need to do it this way instead of strips in order to batch sprites
            for (int i = 0; i < count; i++)
            {
                indices[i * 6 + 0] = i * 4 + 0;
                indices[i * 6 + 1] = i * 4 + 1;
                indices[i * 6 + 2] = i * 4 + 2;

                indices[i * 6 + 3] = i * 4 + 1;
                indices[i * 6 + 4] = i * 4 + 2;
                indices[i * 6 + 5] = i * 4 + 3;
            }
            return indices;
        }
    }


    public static class ExplosionManager
    {
        private static Effect circularEffect;
        private static List<Texture2D> circularExplosionTextures = new List<Texture2D>();

        private static readonly IDictionary<CircularExplosionGenerator, List<CircularExplosionInstance>> generators
            = new Dictionary<CircularExplosionGenerator, List<CircularExplosionInstance>>();
        public static void Load(OvermorrowModFile mod)
        {
            for (int i = 0; i < 2; i++)
            {
                circularExplosionTextures.Add(mod.GetTexture("Effects/Explosions/Textures/Explosion" + i));
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

                var tex = circularExplosionTextures[generator.Texture];
                device.Textures[0] = tex;
                circularEffect.SafeSetParameter("img0", tex);
                circularEffect.SafeSetParameter("tick", generator.CurrentTick);
                circularEffect.SafeSetParameter("radius", generator.Radius);
                circularEffect.SafeSetParameter("texWidth", tex.Width);
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
            circularExplosionTextures.Clear();
            circularEffect = null;
            generators.Clear();
            On.Terraria.Main.DrawDust -= Draw;
        }

        public static void AddGenerator(CircularExplosionGenerator generator)
        {
            generators[generator] = new List<CircularExplosionInstance>();
        }

        public static void CreateExplosion(CircularExplosionGenerator generator, Vector2 center)
        {
            generators[generator].Add(new CircularExplosionInstance(generator, center));
        }

        public static void Update()
        {
            var toRemove = new List<CircularExplosionGenerator>();
            foreach (var kvp in generators)
            {
                kvp.Key.CurrentTick++;
                kvp.Value.RemoveAll(inst => kvp.Key.CurrentTick - inst.InitialTick >= kvp.Key.MaxTick);
                if (!kvp.Value.Any() && kvp.Key.Finished)
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
