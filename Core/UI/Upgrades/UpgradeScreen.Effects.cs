using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Items.Archives;
using OvermorrowMod.Core.Particles.Modular;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace OvermorrowMod.Core.UI.Upgrades
{
    public partial class UpgradeScreen
    {
        private const int MoteCount = 34;
        private const int FlightDuration = 18;
        private const int FlightStagger = 2;
        private const int MaxFlyersPerStone = 6;
        private const int CountDuration = 26;
        private const int ShakeDuration = 18;

        private static readonly SoundStyle TickSound = SoundID.MenuTick with { Volume = 0.35f, MaxInstances = 3 };

        private static readonly ParticleSpec MoteSpec = new()
        {
            Count = 1,
            DirectionMode = EmitDirection.FixedAngle,
            Angle = -90f,
            SpreadDeg = 20f,
            SpeedMin = 0.2f,
            SpeedMax = 0.65f,
            LifetimeMin = 240,
            LifetimeMax = 480,
            StartScaleMin = 0.01f,
            StartScaleMax = 0.018f,
            EndScale = 0.006f,
            AlphaFadeInFrac = 0.2f,
            AlphaFadeOutFrac = 0.4f,
            Orientation = ParticleOrientation.Fixed,
            Texture = "circle_05",
        };

        private static readonly ParticleSpec RingSpec = new()
        {
            Count = 48,
            Shape = EmitShape.Ring,
            ShapeRadius = 10f,
            SpreadDeg = 0f,
            SpeedMin = 4f,
            SpeedMax = 4f,
            Drag = 0.1f,
            LifetimeMin = 36,
            LifetimeMax = 36,
            StartScaleMin = 0.012f,
            StartScaleMax = 0.012f,
            EndScale = 0.012f,
            AlphaFadeInFrac = 0f,
            AlphaFadeOutFrac = 1f,
            Orientation = ParticleOrientation.Fixed,
            Texture = "circle_05",
        };

        private static readonly ParticleSpec BurstSpec = new()
        {
            Count = 21,
            SpeedMin = 1.5f,
            SpeedMax = 5.5f,
            InitialVelocity = new Vector2(0f, -1.5f),
            Gravity = new Vector2(0f, 0.14f),
            Drag = 0.03f,
            LifetimeMin = 35,
            LifetimeMax = 50,
            StartScaleMin = 0.04f,
            StartScaleMax = 0.06f,
            AlphaFadeInFrac = 0f,
            AlphaFadeOutFrac = 0.5f,
            Texture = "trace_01",
            RotationOffsetDeg = -90f,
            FlipVertical = true,
        };

        private class Flyer
        {
            public MonkeyStoneTier Tier;
            public Vector2 From;
            public Vector2 To;
            public int Delay;
            public int Age;
        }

        private readonly ParticleSystem motes = new();
        private readonly ParticleSystem sparks = new();
        private readonly List<Flyer> flyers = new();

        private void UpdateFlyers()
        {
            for (int i = flyers.Count - 1; i >= 0; i--)
            {
                Flyer flyer = flyers[i];
                flyer.Age++;
                if (flyer.Age - flyer.Delay < FlightDuration) continue;

                flyers.RemoveAt(i);
                if (flyers.Count == 0) Land(flyer);
            }
        }

        private void Land(Flyer flyer)
        {
            Color color = TierColor(flyer.Tier) with { A = 0 };
            Color white = Color.White with { A = 0 };
            sparks.Emit(RingSpec with { StartColor = color, EndColor = color }, flyer.To);
            sparks.Emit(BurstSpec with { StartColor = color, EndColor = color }, flyer.To);
            sparks.Emit(BurstSpec with { Count = 7, StartColor = white, EndColor = white }, flyer.To);
            landTicks = 30;
            countTicks = 0;
            pendingSocket = -1;
            SoundEngine.PlaySound(SoundID.Item4 with { Pitch = -0.2f + (int)flyer.Tier * 0.2f, Volume = 0.6f });
        }

        private void UpdateCount()
        {
            if (countTicks >= CountDuration) return;

            countTicks++;
            if (countTicks % 3 == 0 && countTicks < CountDuration) SoundEngine.PlaySound(TickSound);
        }

        private void UpdateMotes()
        {
            motes.Update();
            if (statueArea.IsEmpty) return;

            Color color = AuraColor with { A = 0 } * 0.8f;
            while (motes.Count < MoteCount)
                motes.EmitOne(MoteSpec with { StartColor = color, EndColor = color }, new Vector2(Main.rand.NextFloat(statueArea.Width), statueArea.Height * Main.rand.NextFloat(0.3f, 1f)));
        }

        private void DrawEffects(SpriteBatch spriteBatch)
        {
            foreach (Flyer flyer in flyers)
            {
                float t = (flyer.Age - flyer.Delay) / (float)FlightDuration;
                if (t < 0f) continue;

                float eased = EasingUtils.EaseInOutCubic(t);
                Vector2 position = Vector2.Lerp(flyer.From, flyer.To, eased) - new Vector2(0, MathF.Sin(eased * MathHelper.Pi) * 80f);
                DrawStone(spriteBatch, flyer.Tier, position, 33, Color.White);
            }

            sparks.Draw(spriteBatch, Vector2.Zero);
        }
    }
}
