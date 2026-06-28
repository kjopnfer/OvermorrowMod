using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Particles.Modular
{
    /// <summary>
    /// Immediate-mode debug panel that edits <see cref="ParticleEditorSystem.Spec"/> live: sliders,
    /// cyclers, toggles, color rows, a looping preview pane, and a Copy-as-C# export.
    /// </summary>
    public static class ParticleEditorUI
    {
        public static Rectangle Panel = new(16, 20, 380, 900);

        private static string dragging;
        private static string focused;
        private static string buffer = "";
        private static string hoverTip;
        private static bool prevLeft;
        private static bool prevRight;
        private static bool click;
        private static bool rclick;

        // Persisted HSL per color row so hue/sat survive at white/gray instead of snapping back.
        private static readonly Dictionary<string, Vector3> _hsl = new();
        private static readonly Dictionary<string, Color> _hslColor = new();

        private static bool texturePickerOpen;
        private static Rectangle pickerRect;

        public static void ClearFocus()
        {
            focused = null;
            texturePickerOpen = false;
        }

        /// <summary>True when the cursor is over the panel or the open texture picker (blocks world clicks).</summary>
        public static bool IsMouseOverUI()
        {
            Point m = Main.MouseScreen.ToPoint();
            return Panel.Contains(m) || (texturePickerOpen && pickerRect.Contains(m));
        }

        private static readonly string[] ShapeNames = Enum.GetNames(typeof(EmitShape));
        private static readonly string[] DirNames = Enum.GetNames(typeof(EmitDirection));
        private static readonly string[] OrientNames = Enum.GetNames(typeof(ParticleOrientation));
        private static readonly string[] EaseNames = Enum.GetNames(typeof(ParticleEasing));
        private static readonly string[] LayerNames = Enum.GetNames(typeof(ParticleDrawLayer));
        private static readonly string[] TextureOptions =
        {
            "circle_01", "circle_02", "circle_03", "circle_04", "circle_05",
            "spark_01", "spark_02", "spark_03", "spark_04",
            "trace_01", "trace_02", "trace_03", "trace_04", "trace_05", "trace_06", "trace_07",
            "star_01", "star_02", "star_03", "star_04", "star_05", "star_06", "star_07", "star_08", "star_09",
            "fire_01", "fire_02", "flame_01", "flame_02", "flame_03", "flame_04",
            "smoke_01", "smoke_02", "smoke_03", "smoke_04", "smoke_05", "smoke_06", "smoke_07", "smoke_08",
            "scorch_01", "scorch_02", "scorch_03",
            "magic_01", "magic_02", "magic_03", "magic_04", "magic_05",
            "magic_circle_01", "magic_circle_02",
            "light_01", "light_02", "light_03",
            "muzzle_01", "muzzle_02", "muzzle_03", "muzzle_04", "muzzle_05",
            "twirl_01", "twirl_02", "twirl_03",
            "slash_01", "slash_02", "slash_03", "slash_04",
            "pulse", "ray", "spotlight",
        };

        public static void Draw(SpriteBatch sb)
        {
            if (!Main.mouseLeft) dragging = null;
            click = Main.mouseLeft && !prevLeft;
            rclick = Main.mouseRight && !prevRight;
            hoverTip = null;

            Box(sb, Panel, new Color(18, 18, 26) * 0.93f);
            Outline(sb, Panel, new Color(80, 80, 110));

            var spec = ParticleEditorSystem.Spec;
            Vector2 p = new(Panel.X + 10, Panel.Y + 8);

            Text(sb, "PARTICLE EDITOR", p, Color.White, 0.85f);
            p.Y += 22;

            // Reserve the preview pane area; the pane itself is drawn last (additive) to avoid
            // disturbing the control layout's spritebatch state.
            Rectangle pane = new(Panel.X + 10, (int)p.Y, Panel.Width - 20, 80);
            p.Y += pane.Height + 8;

            // Emission
            spec.Count = (int)Slider(sb, "Count", spec.Count, 1, 60, ref p, "count", "Particles spawned per burst.");
            spec.Shape = (EmitShape)Cycler(sb, "Shape", (int)spec.Shape, ShapeNames, ref p, "shp", "Where particles spawn: point, circle, ring, or cone.");
            bool areaShape = spec.Shape == EmitShape.Circle || spec.Shape == EmitShape.Ring;
            if (areaShape)
                spec.ShapeRadius = Slider(sb, "Radius", spec.ShapeRadius, 0, 200, ref p, "rad", "Spawn radius for Circle/Ring shapes.");
            spec.DirectionMode = (EmitDirection)Cycler(sb, "Direction", (int)spec.DirectionMode, DirNames, ref p, "dir", "How launch direction is chosen.");
            bool angleUsed = spec.DirectionMode == EmitDirection.FixedAngle
                || (spec.DirectionMode == EmitDirection.OutwardFromShape && !areaShape);
            if (angleUsed)
                spec.Angle = Slider(sb, "Angle", spec.Angle, -180, 180, ref p, "ang", "Base launch direction in degrees (0 = right).");
            if (spec.Shape == EmitShape.Cone)
                spec.ConeSpread = Slider(sb, "Cone", spec.ConeSpread, 0, 180, ref p, "cone", "Cone fan width (degrees).");
            else
                spec.SpreadDeg = Slider(sb, "Spread", spec.SpreadDeg, 0, 360, ref p, "sprd", "Fan width around the launch direction. 360 = all directions.");

            // Motion
            spec.SpeedMin = Slider(sb, "Speed min", spec.SpeedMin, 0, 25, ref p, "spmn", "Lowest launch speed (px/frame).");
            spec.SpeedMax = Slider(sb, "Speed max", spec.SpeedMax, 0, 25, ref p, "spmx", "Highest launch speed (px/frame).");
            spec.Drag = Slider(sb, "Drag", spec.Drag, 0, 0.5f, ref p, "drag", "Velocity lost each frame. 0 = none, higher = stops faster.");
            spec.Gravity = new Vector2(spec.Gravity.X, Slider(sb, "Gravity Y", spec.Gravity.Y, -1, 1, ref p, "grav", "Per-frame accel. +down, -up."));
            spec.Turbulence = Slider(sb, "Turbulence", spec.Turbulence, 0, 3, ref p, "turb", "Random jitter added to velocity each frame.");

            // Orientation
            spec.Orientation = (ParticleOrientation)Cycler(sb, "Orient", (int)spec.Orientation, OrientNames, ref p, "ori", "FaceVelocity = face travel; Spin = curve the path each frame; Fixed = constant angle.");
            if (spec.Orientation == ParticleOrientation.Spin)
            {
                spec.AngularVelMin = Slider(sb, "Turn min", spec.AngularVelMin, -0.3f, 0.3f, ref p, "spnmn", "Lowest turn rate: velocity rotates this many radians/frame (curves the path).");
                spec.AngularVelMax = Slider(sb, "Turn max", spec.AngularVelMax, -0.3f, 0.3f, ref p, "spnmx", "Highest turn rate: velocity rotates this many radians/frame (curves the path).");
            }
            spec.RotationOffsetDeg = Slider(sb, "Rot offset", spec.RotationOffsetDeg, -180, 180, ref p, "rotoff", "Degrees added to the sprite's rotation (fix texture orientation).");
            spec.FlipHorizontal = Toggle(sb, "Flip X", spec.FlipHorizontal, ref p, "flx", "Mirror the sprite horizontally.");
            spec.FlipVertical = Toggle(sb, "Flip Y", spec.FlipVertical, ref p, "fly", "Mirror the sprite vertically.");

            // Life / size / color
            spec.LifetimeMin = (int)Slider(sb, "Life min", spec.LifetimeMin, 5, 180, ref p, "lfmn", "Shortest lifetime in ticks (60 = 1 second).");
            spec.LifetimeMax = (int)Slider(sb, "Life max", spec.LifetimeMax, 5, 180, ref p, "lfmx", "Longest lifetime in ticks (60 = 1 second).");
            spec.StartScaleMin = Slider(sb, "Scale min", spec.StartScaleMin, 0, 2.5f, ref p, "scmn", "Lowest starting size.");
            spec.StartScaleMax = Slider(sb, "Scale max", spec.StartScaleMax, 0, 2.5f, ref p, "scmx", "Highest starting size.");
            spec.EndScale = Slider(sb, "End scale", spec.EndScale, 0, 2.5f, ref p, "escl", "Size at end of life (interpolated over lifetime).");
            spec.ScaleEasing = (ParticleEasing)Cycler(sb, "Ease", (int)spec.ScaleEasing, EaseNames, ref p, "eas", "Curve for the scale-over-life transition.");
            spec.AlphaFadeInFrac = Slider(sb, "Fade in", spec.AlphaFadeInFrac, 0, 1, ref p, "fin", "Fraction of life spent fading in.");
            spec.AlphaFadeOutFrac = Slider(sb, "Fade out", spec.AlphaFadeOutFrac, 0, 1, ref p, "fout", "Fraction of life spent fading out.");
            spec.StartColor = ColorRow(sb, "Start col", spec.StartColor, ref p, "sc", "Color at spawn (lerps to End col over life).");
            spec.EndColor = ColorRow(sb, "End col", spec.EndColor, ref p, "ec", "Color at end of life.");

            // Render
            TextureRow(sb, spec, ref p);
            spec.DrawLayer = (ParticleDrawLayer)Cycler(sb, "Layer", (int)spec.DrawLayer, LayerNames, ref p, "lay", "Render/draw-order layer.");
            spec.Additive = Toggle(sb, "Additive", spec.Additive, ref p, "add", "Additive blending (bright/glowy) vs normal alpha.");
            spec.Shader = CyclerStr(sb, "Shader", spec.Shader, ParticleShaderRegistry.Names.ToArray(), ref p, "shd", "Per-pixel material shader (None for plain).");
            if (spec.Shader != ParticleShaderRegistry.None)
                spec.ShaderColor = ColorRow(sb, "Shader col", spec.ShaderColor, ref p, "shc", "Color fed to the shader.");

            if (Button(sb, "Copy as C#", ref p, "copy", "Copy this spec as a C# ParticleSpec to the clipboard.")) CopyCSharp(spec);
            if (Button(sb, "Copy JSON", ref p, "copyjson", "Copy this spec as JSON (re-importable with Paste JSON).")) CopyJson(spec);
            if (Button(sb, "Paste JSON", ref p, "pastejson", "Load a spec from JSON on the clipboard as a baseline to tweak.")) PasteJson();
            if (Button(sb, "Reset", ref p, "reset", "Reset all values to defaults.")) ParticleEditorSystem.Spec = new();

            // Preview pane, drawn last. Plain alpha draw to keep the UI spritebatch state intact
            // (additive glow is visible on world-spawned particles).
            Box(sb, pane, Color.Black * 0.7f);
            Outline(sb, pane, new Color(60, 60, 80));
            ParticleEditorSystem.Preview.Draw(sb, pane.Center.ToVector2(), useShader: false);

            if (texturePickerOpen) DrawTexturePicker(sb, spec);
            DrawTooltip(sb);
            prevLeft = Main.mouseLeft;
            prevRight = Main.mouseRight;
        }

        // ----- widgets -----

        private static float Slider(SpriteBatch sb, string label, float val, float min, float max, ref Vector2 pos, string id, string tip = null)
        {
            Hover(pos, tip);
            Text(sb, label, pos, Color.White, 0.8f);
            Rectangle track = new((int)pos.X + 108, (int)pos.Y + 6, 130, 5);
            Rectangle valBox = new(track.Right + 8, (int)pos.Y - 1, 62, 16);
            bool overVal = valBox.Contains(Main.MouseScreen.ToPoint());

            // Click the value box to type an exact number (typed values may exceed the slider range).
            if (overVal && click && focused != id)
            {
                focused = id;
                buffer = val.ToString("0.###");
                Main.clrInput();
            }

            if (focused == id)
            {
                PlayerInput.WritingText = true;
                Main.instance.HandleIME();
                buffer = Main.GetInputText(buffer);

                if (Main.inputTextEnter || (click && !overVal))
                {
                    if (float.TryParse(buffer, out float typed)) val = typed;
                    focused = null;
                }
                else if (Main.inputTextEscape)
                {
                    focused = null;
                }

                Box(sb, valBox, new Color(60, 80, 120));
                Text(sb, buffer + "|", new Vector2(valBox.X + 3, pos.Y), Color.White, 0.75f);
            }
            else
            {
                val = SliderRaw(track, val, min, max, id);
                Box(sb, valBox, new Color(35, 35, 48));
                Text(sb, val.ToString("0.###"), new Vector2(valBox.X + 3, pos.Y), new Color(200, 200, 160), 0.75f);
            }

            float t = max > min ? MathHelper.Clamp((val - min) / (max - min), 0f, 1f) : 0f;
            Box(sb, track, new Color(70, 70, 90));
            Box(sb, new Rectangle(track.X + (int)(t * track.Width) - 2, track.Y - 4, 5, 13), Color.White);
            pos.Y += 20;
            return val;
        }

        private static float SliderRaw(Rectangle track, float val, float min, float max, string id)
        {
            bool over = track.Contains(Main.MouseScreen.ToPoint());
            if (Main.mouseLeft && (dragging == id || (dragging == null && over)))
            {
                dragging = id;
                float frac = MathHelper.Clamp((Main.MouseScreen.X - track.X) / track.Width, 0f, 1f);
                val = MathHelper.Lerp(min, max, frac);
            }
            return val;
        }

        private static int Cycler(SpriteBatch sb, string label, int idx, string[] names, ref Vector2 pos, string id, string tip = null)
        {
            Hover(pos, tip);
            Text(sb, label, pos, Color.White, 0.8f);
            Rectangle box = new((int)pos.X + 108, (int)pos.Y, 156, 16);
            bool over = box.Contains(Main.MouseScreen.ToPoint());
            Box(sb, box, over ? new Color(70, 90, 120) : new Color(40, 40, 55));
            Text(sb, names[idx], new Vector2(box.X + 4, box.Y + 1), Color.White, 0.75f);
            if (over && click) idx = (idx + 1) % names.Length;
            else if (over && rclick) idx = (idx - 1 + names.Length) % names.Length;
            pos.Y += 20;
            return idx;
        }

        private static string CyclerStr(SpriteBatch sb, string label, string current, string[] options, ref Vector2 pos, string id, string tip = null)
        {
            int idx = Array.IndexOf(options, current);
            if (idx < 0) idx = 0;
            idx = Cycler(sb, label, idx, options, ref pos, id, tip);
            return options[idx];
        }

        private static bool Toggle(SpriteBatch sb, string label, bool val, ref Vector2 pos, string id, string tip = null)
        {
            Hover(pos, tip);
            Text(sb, label, pos, Color.White, 0.8f);
            Rectangle box = new((int)pos.X + 108, (int)pos.Y, 16, 16);
            bool over = box.Contains(Main.MouseScreen.ToPoint());
            Box(sb, box, val ? new Color(90, 170, 90) : new Color(60, 60, 70));
            if (over && click) val = !val;
            pos.Y += 20;
            return val;
        }

        private static Color ColorRow(SpriteBatch sb, string label, Color c, ref Vector2 pos, string id, string tip = null)
        {
            Hover(pos, tip);
            Text(sb, label, pos, Color.White, 0.8f);

            // Pull persisted HSL; resync only when the color was changed externally (paste/reset/hex).
            if (!_hsl.TryGetValue(id, out Vector3 state) || !_hslColor.TryGetValue(id, out Color lastC) || lastC != c)
            {
                state = Main.rgbToHsl(c);
                _hsl[id] = state;
                _hslColor[id] = c;
            }

            // Row 1: hex field + swatch.
            string hexId = id + "#";
            Rectangle hexBox = new((int)pos.X + 108, (int)pos.Y - 1, 78, 16);
            bool overHex = hexBox.Contains(Main.MouseScreen.ToPoint());
            if (overHex && click && focused != hexId)
            {
                focused = hexId;
                buffer = $"#{c.R:X2}{c.G:X2}{c.B:X2}";
                Main.clrInput();
            }

            bool hexCommitted = false;
            if (focused == hexId)
            {
                PlayerInput.WritingText = true;
                Main.instance.HandleIME();
                buffer = Main.GetInputText(buffer);
                if (Main.inputTextEnter || (click && !overHex))
                {
                    if (TryParseHex(buffer, out Color parsed))
                    {
                        c = parsed;
                        state = Main.rgbToHsl(c);
                        _hsl[id] = state;
                        _hslColor[id] = c;
                        hexCommitted = true;
                    }
                    focused = null;
                }
                else if (Main.inputTextEscape) focused = null;

                Box(sb, hexBox, new Color(60, 80, 120));
                Text(sb, buffer + "|", new Vector2(hexBox.X + 3, pos.Y), Color.White, 0.7f);
            }
            else
            {
                Box(sb, hexBox, new Color(35, 35, 48));
                Text(sb, $"#{c.R:X2}{c.G:X2}{c.B:X2}", new Vector2(hexBox.X + 3, pos.Y), new Color(200, 200, 160), 0.7f);
            }
            Box(sb, new Rectangle((int)pos.X + 192, (int)pos.Y, 18, 14), c);
            pos.Y += 20;

            // Rows 2-3: HSL sliders (driven by persisted state, not the round-tripped color).
            float h = state.X, s = state.Y, l = state.Z;

            Text(sb, "H", pos, new Color(180, 180, 180), 0.7f);
            Rectangle hTrack = new((int)pos.X + 28, (int)pos.Y + 6, 232, 6);
            DrawHueTrack(sb, hTrack);
            float nh = SliderRaw(hTrack, h, 0f, 1f, id + "h");
            Box(sb, new Rectangle(hTrack.X + (int)(nh * hTrack.Width) - 2, hTrack.Y - 3, 4, 12), Color.White);
            pos.Y += 20;

            float previewL = (l <= 0.02f || l >= 0.98f) ? 0.5f : l;
            Text(sb, "S", pos, new Color(180, 180, 180), 0.7f);
            Rectangle sTrack = new((int)pos.X + 28, (int)pos.Y + 6, 100, 6);
            DrawGradientTrack(sb, sTrack, f => Main.hslToRgb(h, f, previewL));
            float ns = SliderRaw(sTrack, s, 0f, 1f, id + "s");
            Box(sb, new Rectangle(sTrack.X + (int)(ns * sTrack.Width) - 2, sTrack.Y - 3, 4, 12), Color.White);

            Text(sb, "L", new Vector2(pos.X + 146, pos.Y), new Color(180, 180, 180), 0.7f);
            Rectangle lTrack = new((int)pos.X + 168, (int)pos.Y + 6, 92, 6);
            DrawGradientTrack(sb, lTrack, f => Main.hslToRgb(h, s, f));
            float nl = SliderRaw(lTrack, l, 0f, 1f, id + "l");
            Box(sb, new Rectangle(lTrack.X + (int)(nl * lTrack.Width) - 2, lTrack.Y - 3, 4, 12), Color.White);
            pos.Y += 20;

            // Only rebuild from HSL when a slider actually moved, so a typed hex stays exact.
            if (!hexCommitted && (nh != h || ns != s || nl != l))
            {
                // Dragging hue out of a white/gray color makes it vivid instead of staying colorless.
                if (nh != h)
                {
                    if (ns <= 0.001f) ns = 1f;
                    if (nl >= 0.999f || nl <= 0.001f) nl = 0.5f;
                }
                state = new Vector3(nh, ns, nl);
                c = Main.hslToRgb(nh, ns, nl);
                _hsl[id] = state;
                _hslColor[id] = c;
            }

            return c;
        }

        private static void DrawHueTrack(SpriteBatch sb, Rectangle track)
            => DrawGradientTrack(sb, track, f => Main.hslToRgb(f, 1f, 0.5f));

        private static void DrawGradientTrack(SpriteBatch sb, Rectangle track, Func<float, Color> colorAt)
        {
            const int seg = 16;
            for (int i = 0; i < seg; i++)
            {
                int x = track.X + track.Width * i / seg;
                int w = track.X + track.Width * (i + 1) / seg - x;
                Box(sb, new Rectangle(x, track.Y, w, track.Height), colorAt((i + 0.5f) / seg));
            }
        }

        private static bool TryParseHex(string text, out Color color)
        {
            color = Color.White;
            string hex = text.Trim().TrimStart('#');
            if (hex.Length != 6) return false;
            try
            {
                int r = Convert.ToInt32(hex.Substring(0, 2), 16);
                int g = Convert.ToInt32(hex.Substring(2, 2), 16);
                int b = Convert.ToInt32(hex.Substring(4, 2), 16);
                color = new Color(r, g, b);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool Button(SpriteBatch sb, string label, ref Vector2 pos, string id, string tip = null)
        {
            Rectangle box = new((int)pos.X, (int)pos.Y, 156, 20);
            bool over = box.Contains(Main.MouseScreen.ToPoint());
            if (tip != null && over) hoverTip = tip;
            Box(sb, box, over ? new Color(70, 110, 150) : new Color(45, 55, 70));
            Text(sb, label, new Vector2(box.X + 6, box.Y + 3), Color.White, 0.8f);
            pos.Y += 24;
            return over && click;
        }

        // ----- texture picker -----

        private static void TextureRow(SpriteBatch sb, ParticleSpec spec, ref Vector2 pos)
        {
            Hover(pos, "Particle sprite. Click to open the thumbnail picker.");
            Text(sb, "Texture", pos, Color.White, 0.8f);
            Rectangle box = new((int)pos.X + 108, (int)pos.Y, 156, 16);
            bool over = box.Contains(Main.MouseScreen.ToPoint());
            Box(sb, box, over || texturePickerOpen ? new Color(70, 90, 120) : new Color(40, 40, 55));
            Text(sb, spec.Texture, new Vector2(box.X + 4, box.Y + 1), Color.White, 0.75f);
            if (over && click) texturePickerOpen = !texturePickerOpen;
            pos.Y += 20;
        }

        private static void DrawTexturePicker(SpriteBatch sb, ParticleSpec spec)
        {
            const int cols = 6, cell = 46;
            int rows = (TextureOptions.Length + cols - 1) / cols;
            int w = cols * cell + 8;
            int h = rows * cell + 8;
            int x = Panel.Right + 8;
            int y = (int)MathHelper.Clamp(Panel.Y, 0, Math.Max(0, Main.screenHeight - h));
            pickerRect = new Rectangle(x, y, w, h);

            Box(sb, pickerRect, new Color(14, 14, 20) * 0.97f);
            Outline(sb, pickerRect, new Color(90, 90, 120));

            for (int i = 0; i < TextureOptions.Length; i++)
            {
                int cx = i % cols, cy = i / cols;
                Rectangle r = new(pickerRect.X + 4 + cx * cell, pickerRect.Y + 4 + cy * cell, cell - 4, cell - 4);
                bool over = r.Contains(Main.MouseScreen.ToPoint());
                bool selected = TextureOptions[i] == spec.Texture;
                Box(sb, r, selected ? new Color(70, 100, 140) : over ? new Color(50, 50, 70) : new Color(28, 28, 38));

                string path = AssetDirectory.Textures + TextureOptions[i];
                if (ModContent.HasAsset(path))
                {
                    Texture2D tex = ModContent.Request<Texture2D>(path).Value;
                    float fit = Math.Min(Math.Min((cell - 12f) / tex.Width, (cell - 12f) / tex.Height), 1f);
                    sb.Draw(tex, r.Center.ToVector2(), null, Color.White, 0f, tex.Size() / 2f, fit, SpriteEffects.None, 0f);
                }

                if (over)
                {
                    hoverTip = TextureOptions[i];
                    if (click)
                    {
                        spec.Texture = TextureOptions[i];
                        texturePickerOpen = false;
                    }
                }
            }
        }

        // ----- tooltips -----

        private static void Hover(Vector2 pos, string tip)
        {
            if (tip == null) return;
            Rectangle label = new((int)pos.X, (int)pos.Y - 1, 104, 17);
            if (label.Contains(Main.MouseScreen.ToPoint())) hoverTip = tip;
        }

        private static void DrawTooltip(SpriteBatch sb)
        {
            if (string.IsNullOrEmpty(hoverTip)) return;

            var font = FontAssets.MouseText.Value;
            const float scale = 0.8f;
            Vector2 size = font.MeasureString(hoverTip) * scale;
            Vector2 at = Main.MouseScreen + new Vector2(18, 18);
            if (at.X + size.X + 10 > Main.screenWidth) at.X = Main.screenWidth - size.X - 10;
            if (at.Y + size.Y + 8 > Main.screenHeight) at.Y = Main.screenHeight - size.Y - 8;

            Rectangle bg = new((int)at.X - 5, (int)at.Y - 3, (int)size.X + 10, (int)size.Y + 6);
            Box(sb, bg, new Color(10, 10, 16) * 0.96f);
            Outline(sb, bg, new Color(90, 90, 120));
            Text(sb, hoverTip, at, Color.White, scale);
        }

        // ----- primitives -----

        private static void Text(SpriteBatch sb, string text, Vector2 pos, Color color, float scale)
            => sb.DrawString(FontAssets.MouseText.Value, text, pos, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

        private static void Box(SpriteBatch sb, Rectangle r, Color c)
            => sb.Draw(TextureAssets.MagicPixel.Value, r, c);

        private static void Outline(SpriteBatch sb, Rectangle r, Color c)
        {
            Box(sb, new Rectangle(r.X, r.Y, r.Width, 1), c);
            Box(sb, new Rectangle(r.X, r.Bottom - 1, r.Width, 1), c);
            Box(sb, new Rectangle(r.X, r.Y, 1, r.Height), c);
            Box(sb, new Rectangle(r.Right - 1, r.Y, 1, r.Height), c);
        }

        private static void CopyCSharp(ParticleSpec s)
        {
            var sb = new StringBuilder();
            sb.AppendLine("new ParticleSpec {");
            sb.AppendLine($"    Shape = EmitShape.{s.Shape}, Count = {s.Count}, ShapeRadius = {F(s.ShapeRadius)}, ConeSpread = {F(s.ConeSpread)},");
            sb.AppendLine($"    DirectionMode = EmitDirection.{s.DirectionMode}, Angle = {F(s.Angle)}, SpreadDeg = {F(s.SpreadDeg)},");
            sb.AppendLine($"    SpeedMin = {F(s.SpeedMin)}, SpeedMax = {F(s.SpeedMax)}, Drag = {F(s.Drag)}, Gravity = new Vector2({F(s.Gravity.X)}, {F(s.Gravity.Y)}), Turbulence = {F(s.Turbulence)},");
            sb.AppendLine($"    AngularVelMin = {F(s.AngularVelMin)}, AngularVelMax = {F(s.AngularVelMax)}, Orientation = ParticleOrientation.{s.Orientation},");
            sb.AppendLine($"    LifetimeMin = {s.LifetimeMin}, LifetimeMax = {s.LifetimeMax},");
            sb.AppendLine($"    StartScaleMin = {F(s.StartScaleMin)}, StartScaleMax = {F(s.StartScaleMax)}, EndScale = {F(s.EndScale)}, ScaleEasing = ParticleEasing.{s.ScaleEasing},");
            sb.AppendLine($"    StartColor = new Color({s.StartColor.R}, {s.StartColor.G}, {s.StartColor.B}), EndColor = new Color({s.EndColor.R}, {s.EndColor.G}, {s.EndColor.B}),");
            sb.AppendLine($"    AlphaFadeInFrac = {F(s.AlphaFadeInFrac)}, AlphaFadeOutFrac = {F(s.AlphaFadeOutFrac)},");
            sb.AppendLine($"    Texture = \"{s.Texture}\", Additive = {s.Additive.ToString().ToLower()}, DrawLayer = ParticleDrawLayer.{s.DrawLayer},");
            sb.AppendLine($"    Orientation = ParticleOrientation.{s.Orientation}, RotationOffsetDeg = {F(s.RotationOffsetDeg)}, FlipHorizontal = {s.FlipHorizontal.ToString().ToLower()}, FlipVertical = {s.FlipVertical.ToString().ToLower()},");
            if (s.Shader != ParticleShaderRegistry.None)
                sb.AppendLine($"    Shader = \"{s.Shader}\", ShaderColor = new Color({s.ShaderColor.R}, {s.ShaderColor.G}, {s.ShaderColor.B}), ShaderProgressFromAge = {s.ShaderProgressFromAge.ToString().ToLower()},");
            sb.AppendLine("};");

            SetClipboard(sb.ToString());
            Main.NewText("Copied ParticleSpec as C# to clipboard.", Color.LightGreen);
        }

        private static void CopyJson(ParticleSpec spec)
        {
            SetClipboard(ParticleSpecSerializer.Serialize(spec));
            Main.NewText("Copied ParticleSpec as JSON to clipboard.", Color.LightGreen);
        }

        private static void PasteJson()
        {
            string text = GetClipboard();
            if (ParticleSpecSerializer.TryDeserialize(text, out ParticleSpec loaded))
            {
                ParticleEditorSystem.Spec = loaded;
                Main.NewText("Loaded ParticleSpec from clipboard JSON.", Color.LightGreen);
            }
            else
            {
                Main.NewText("Clipboard does not contain a valid ParticleSpec JSON.", Color.OrangeRed);
            }
        }

        private static void SetClipboard(string text)
        {
            try { ReLogic.OS.Platform.Get<ReLogic.OS.IClipboard>().Value = text; } catch { }
        }

        private static string GetClipboard()
        {
            try { return ReLogic.OS.Platform.Get<ReLogic.OS.IClipboard>().Value; } catch { return null; }
        }

        private static string F(float v) => v.ToString("0.###") + "f";
    }
}
