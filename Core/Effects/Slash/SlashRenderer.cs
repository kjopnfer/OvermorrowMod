using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace OvermorrowMod.Core.Effects.Slash
{
    /// <summary>
    /// Manages and renders layered slash effects
    /// </summary>
    public class SlashRenderer
    {
        /// <summary>
        /// The path this slash follows
        /// </summary>
        public SlashPath Path { get; set; }

        /// <summary>
        /// Base width of the slash (before layer scaling)
        /// </summary>
        public float BaseWidth { get; set; }

        /// <summary>
        /// Number of segments to use for mesh generation
        /// </summary>
        public int Segments { get; set; }

        /// <summary>
        /// List of layers to render (order matters - later layers draw on top)
        /// </summary>
        public List<SlashLayer> Layers { get; private set; }

        /// <summary>
        /// Current fade progress for the slash (0 = no fade, 1 = full fade)
        /// </summary>
        public float FadeProgress { get; set; } = 0f;


        public SlashRenderer()
        {
            Layers = new List<SlashLayer>();
            BaseWidth = 20f;
            Segments = 30;
        }

        public SlashRenderer(SlashPath path, float baseWidth = 20f, int segments = 30)
        {
            Path = path;
            BaseWidth = baseWidth;
            Segments = segments;
            Layers = new List<SlashLayer>();
        }

        /// <summary>
        /// Adds a layer to the slash effect
        /// </summary>
        public void AddLayer(SlashLayer layer)
        {
            Layers.Add(layer);
        }

        /// <summary>
        /// Adds a simple layer with just texture and color
        /// </summary>
        public void AddLayer(Texture2D texture, Color color, float widthScale = 1f, float opacity = 1f)
        {
            Layers.Add(new SlashLayer(texture, color, widthScale, opacity));
        }

        /// <summary>
        /// Clears all layers
        /// </summary>
        public void ClearLayers()
        {
            Layers.Clear();
        }

        /// <summary>
        /// Renders all layers of the slash effect
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (Layers.Count == 0) return;

            // Draw each layer in order (first layer = bottom, last layer = top)
            foreach (var layer in Layers)
            {
                DrawLayer(spriteBatch, layer);
            }
        }

        private void DrawLayer(SpriteBatch spriteBatch, SlashLayer layer)
        {
            List<VertexPositionColorTexture> vertices;

            // Generate vertices based on whether the layer has a custom width curve
            if (layer.WidthCurve != null)
            {
                // Use the existing curve method (no tapering for custom curves)
                System.Func<float, float> combinedCurve = (t) => layer.GetWidth(BaseWidth, t);

                vertices = SlashMeshGenerator.GenerateSlashMeshWithCurve(
                    Path,
                    combinedCurve,
                    Segments,
                    layer.FinalColor,
                    layer.SpriteEffects
                );
            }
            else
            {
                float layerWidth = layer.GetWidth(BaseWidth);

                vertices = SlashMeshGenerator.GenerateSlashMesh(
                    Path,
                    layerWidth,
                    Segments,
                    layer.FinalColor * layer.Opacity,
                    layer.SpriteEffects,
                    layer.StartTaper,
                    layer.EndTaper,
                    layer.TaperLength,
                    layer.Offset,
                    FadeProgress
                );
            }

            if (vertices.Count > 0)
            {
                PrimitiveRenderer.Draw(spriteBatch, vertices, layer.Texture, layer.BlendState);
            }
        }

        public void UpdatePath(SlashPath newPath)
        {
            Path = newPath;
        }
    }
}