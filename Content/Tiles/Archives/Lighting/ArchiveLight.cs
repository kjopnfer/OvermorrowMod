using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Content.Tiles.Archives
{
    /// <summary>
    /// Per-light brightness state for an Archive flame tile.
    /// </summary>
    public class ArchiveLight_TE : ModTileEntity
    {
        public float Current = 1f;
        public float Target = 1f;

        private float rampFrom = 1f;
        private int rampTimer;
        private int rampDuration;

        /// <summary>
        /// Multiplier applied on top of <see cref="Current"/> while the ghost is haunting this light.
        /// 1 is undisturbed; 0 is fully snuffed out.
        /// </summary>
        public float Haunt { get; private set; } = 1f;

        private float hauntPressure;
        private float pressureTarget;
        private int extinguishTimer;
        private int relightTimer;
        private bool extinguished;

        private const int ExtinguishDelay = 30;
        private const int RelightDelay = 60 * 20;
        private const float ExtinguishPressure = 0.85f;
        private const float FlickerPressure = 0.15f;

        /// <summary>
        /// Raises this light's haunt pressure for the frame; proximity is 0 at the edge of the
        /// ghost's reach and 1 right on top of it. Called every frame the ghost is in range.
        /// </summary>
        public void Disturb(float proximity)
        {
            if (proximity > pressureTarget) pressureTarget = proximity;
        }

        public void SetTarget(float value, int rampTicks, bool instant = false)
        {
            value = MathHelper.Clamp(value, 0f, 1f);

            if (instant || rampTicks <= 0)
            {
                Current = Target = value;
                rampTimer = rampDuration = 0;
                return;
            }

            rampFrom = Current;
            Target = value;
            rampDuration = rampTicks;
            rampTimer = 0;
        }

        public void Extinguish(int rampTicks) => SetTarget(0f, rampTicks);
        public void Ignite(int rampTicks) => SetTarget(1f, rampTicks);

        /// <summary>
        /// One-frame brightness jitter for a warning/disturbed effect.
        /// </summary>
        public void Flicker(float intensity = 0.6f)
        {
            if (rampTimer < rampDuration) return;
            Current = Target * (1f - Main.rand.NextFloat() * intensity);
        }

        public override void Update()
        {
            if (rampDuration > 0 && rampTimer < rampDuration)
            {
                rampTimer++;
                float progress = MathHelper.Clamp(rampTimer / (float)rampDuration, 0f, 1f);
                Current = MathHelper.Lerp(rampFrom, Target, EasingUtils.EaseInOutQuad(progress));
                if (rampTimer >= rampDuration) Current = Target;
            }

            UpdateHaunt();
        }

        /// <summary>
        /// Dims toward the ghost as it nears, snuffs out after sustained close contact, and stays dark
        /// for <see cref="RelightDelay"/> ticks before easing back on.
        /// </summary>
        private void UpdateHaunt()
        {
            if (extinguished)
            {
                Haunt = 0f;
                pressureTarget = 0f;
                hauntPressure = 0f;
                extinguishTimer = 0;
                if (--relightTimer <= 0) extinguished = false;
                return;
            }

            hauntPressure = MathHelper.Lerp(hauntPressure, pressureTarget, 0.25f);
            pressureTarget = 0f;

            float desired = 1f - hauntPressure;
            float rate = desired < Haunt ? 0.25f : 0.03f;
            Haunt = MathHelper.Lerp(Haunt, desired, rate);

            if (hauntPressure > FlickerPressure && hauntPressure < ExtinguishPressure && Main.rand.NextBool(2))
                Haunt *= 1f - Main.rand.NextFloat() * 0.5f * hauntPressure;

            if (hauntPressure >= ExtinguishPressure)
            {
                if (++extinguishTimer >= ExtinguishDelay)
                {
                    extinguished = true;
                    relightTimer = RelightDelay;
                    Haunt = 0f;
                }
            }
            else if (extinguishTimer > 0)
            {
                extinguishTimer--;
            }
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            return ArchiveLights.IsLightTile(Framing.GetTileSafely(x, y));
        }

        public override void SaveData(TagCompound tag)
        {
            tag["Current"] = Current;
            tag["Target"] = Target;
        }

        public override void LoadData(TagCompound tag)
        {
            Current = tag.GetFloat("Current");
            Target = tag.GetFloat("Target");
        }
    }

    /// <summary>
    /// Registry of Archive flame tile types plus the control API for dimming/reigniting them.
    /// </summary>
    public static class ArchiveLights
    {
        public static readonly HashSet<int> LightTileTypes = new();

        public const int CombatRoomHeight = 25;

        public static bool IsLightTile(Tile tile) => tile.HasTile && LightTileTypes.Contains(tile.TileType);

        /// <summary>
        /// Current brightness multiplier (0 - 1) for the light covering this tile, or 1 if uncontrolled.
        /// </summary>
        public static float GetBrightness(int i, int j)
        {
            Point topLeft = TileUtils.GetCornerOfMultiTile(i, j, TileUtils.CornerType.TopLeft);
            if (TileUtils.TryFindModTileEntity<ArchiveLight_TE>(topLeft.X, topLeft.Y, out var light))
                return light.Current * light.Haunt;

            return 1f;
        }

        public const float HauntOuterRadius = 90f;
        public const float HauntInnerRadius = 8f;

        /// <summary>
        /// Pressures every light within the ghost's reach, scaled by closeness, so it dims and
        /// eventually snuffs the lights nearest to it.
        /// </summary>
        public static void Disturb(Vector2 worldCenter)
        {
            int centerX = (int)(worldCenter.X / 16f);
            int centerY = (int)(worldCenter.Y / 16f);
            int radius = (int)System.Math.Ceiling(HauntOuterRadius);

            for (int x = centerX - radius; x <= centerX + radius; x++)
            {
                for (int y = centerY - radius; y <= centerY + radius; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (!IsLightTile(tile)) continue;

                    Point topLeft = TileUtils.GetCornerOfMultiTile(x, y, TileUtils.CornerType.TopLeft);
                    if (topLeft.X != x || topLeft.Y != y) continue;

                    float dx = x - centerX;
                    float dy = y - centerY;
                    float dist = (float)System.Math.Sqrt(dx * dx + dy * dy);
                    if (dist > HauntOuterRadius) continue;

                    float closeness = MathHelper.Clamp((HauntOuterRadius - dist) / (HauntOuterRadius - HauntInnerRadius), 0f, 1f);
                    if (closeness < 0.02f) continue;

                    float proximity = closeness * closeness;
                    GetOrCreate(x, y)?.Disturb(proximity);
                }
            }
        }

        private static ArchiveLight_TE GetOrCreate(int x, int y)
        {
            if (TileUtils.TryFindModTileEntity<ArchiveLight_TE>(x, y, out var existing))
                return existing;

            int id = ModContent.GetInstance<ArchiveLight_TE>().Place(x, y);
            return TileEntity.ByID.TryGetValue(id, out var placed) ? placed as ArchiveLight_TE : null;
        }

        /// <summary>
        /// Applies a brightness target to every light whose multitile origin falls inside the tile rectangle.
        /// </summary>
        public static void SetRegion(Rectangle tileRect, float target, int rampTicks, bool instant = false)
        {
            for (int x = tileRect.Left; x <= tileRect.Right; x++)
            {
                for (int y = tileRect.Top; y <= tileRect.Bottom; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (!IsLightTile(tile)) continue;

                    Point topLeft = TileUtils.GetCornerOfMultiTile(x, y, TileUtils.CornerType.TopLeft);
                    if (topLeft.X != x || topLeft.Y != y) continue;

                    GetOrCreate(x, y)?.SetTarget(target, rampTicks, instant);
                }
            }
        }

        public static void SetRadius(Vector2 worldCenter, float tileRadius, float target, int rampTicks, bool instant = false)
        {
            ForEachLightInRadius(worldCenter, tileRadius, light => light.SetTarget(target, rampTicks, instant));
        }

        public static void Flicker(Vector2 worldCenter, float tileRadius, float intensity = 0.6f)
        {
            ForEachLightInRadius(worldCenter, tileRadius, light => light.Flicker(intensity));
        }

        private static void ForEachLightInRadius(Vector2 worldCenter, float tileRadius, System.Action<ArchiveLight_TE> action)
        {
            int centerX = (int)(worldCenter.X / 16f);
            int centerY = (int)(worldCenter.Y / 16f);
            int radius = (int)System.Math.Ceiling(tileRadius);
            float radiusSquared = tileRadius * tileRadius;

            for (int x = centerX - radius; x <= centerX + radius; x++)
            {
                for (int y = centerY - radius; y <= centerY + radius; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (!IsLightTile(tile)) continue;

                    Point topLeft = TileUtils.GetCornerOfMultiTile(x, y, TileUtils.CornerType.TopLeft);
                    if (topLeft.X != x || topLeft.Y != y) continue;

                    float dx = x - centerX;
                    float dy = y - centerY;
                    if (dx * dx + dy * dy > radiusSquared) continue;

                    var light = GetOrCreate(x, y);
                    if (light != null) action(light);
                }
            }
        }
    }
}
