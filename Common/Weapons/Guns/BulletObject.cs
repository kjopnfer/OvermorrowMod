using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Weapons.Guns
{
    /// <summary>
    /// Represents a bullet in the gun's visual display with flexible modification support.
    /// Accessories can modify appearance, add custom drawing, and store properties.
    /// </summary>
    public class BulletObject
    {
        public int DrawCounter = 0;
        public int DeathCounter = 0;
        public bool isActive = true;
        public bool startDeath = false;
        private string BulletTexture;

        // Flexible visual properties
        public Color BulletColor = Color.White;
        public Color GlowColor = Color.Transparent;
        public float GlowIntensity = 0f;
        public float ScaleMultiplier = 1f;
        public bool HasTrail = false;
        public Color TrailColor = Color.White;
        public bool Pulsing = false;
        public float PulseSpeed = 1f;
        public bool Spinning = false;
        public float SpinSpeed = 1f;
        public bool HasAura = false;
        public Color AuraColor = Color.White;
        public float AuraSize = 1f;
        public string CustomTexture = "";

        // Flexible property system for accessories
        public Dictionary<string, object> Properties = new Dictionary<string, object>();

        // Drawing delegates - accessories can register custom drawing methods
        public List<Action<SpriteBatch, Vector2, float, float>> CustomDrawMethods = new List<Action<SpriteBatch, Vector2, float, float>>();

        public BulletObject(string BulletTexture, int DrawCounter = 0)
        {
            this.BulletTexture = BulletTexture;
            this.DrawCounter = DrawCounter;
        }

        public void Update()
        {
            if (!isActive) return;

            if (startDeath)
            {
                DeathCounter++;
                if (DeathCounter == 15)
                {
                    isActive = false;
                }
            }

            DrawCounter++;
        }

        public void Deactivate()
        {
            startDeath = true;
        }

        public void Reset()
        {
            DeathCounter = 0;
            startDeath = false;
            isActive = true;
        }

        /// <summary>
        /// Clears all modifications but keeps the bullet active.
        /// Called when bullets are refreshed after reload.
        /// </summary>
        public void ClearModifications()
        {
            BulletColor = Color.White;
            GlowColor = Color.Transparent;
            GlowIntensity = 0f;
            ScaleMultiplier = 1f;
            HasTrail = false;
            TrailColor = Color.White;
            Pulsing = false;
            PulseSpeed = 1f;
            Spinning = false;
            SpinSpeed = 1f;
            HasAura = false;
            AuraColor = Color.White;
            AuraSize = 1f;
            CustomTexture = "";
            Properties.Clear();
            CustomDrawMethods.Clear();
        }

        /// <summary>
        /// Sets a property that accessories can use to store custom data.
        /// </summary>
        public void SetProperty(string key, object value)
        {
            Properties[key] = value;
        }

        /// <summary>
        /// Gets a property value, with optional default.
        /// </summary>
        public T GetProperty<T>(string key, T defaultValue = default(T))
        {
            if (Properties.ContainsKey(key) && Properties[key] is T)
            {
                return (T)Properties[key];
            }
            return defaultValue;
        }

        /// <summary>
        /// Adds a custom drawing method that will be called during bullet rendering.
        /// </summary>
        public void AddCustomDrawMethod(Action<SpriteBatch, Vector2, float, float> drawMethod)
        {
            CustomDrawMethods.Add(drawMethod);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            string textureToUse = string.IsNullOrEmpty(CustomTexture) ? BulletTexture : CustomTexture;
            Texture2D bulletTexture = ModContent.Request<Texture2D>(AssetDirectory.GunUI + textureToUse).Value;

            float baseScale = ScaleMultiplier;
            Color drawColor = BulletColor;

            // Calculate base position and rotation
            Vector2 positionOffset = Vector2.UnitY * MathHelper.Lerp(-1, 1, (float)Math.Sin(DrawCounter / 30f) * 0.5f + 0.5f);
            float baseRotation = MathHelper.Lerp(MathHelper.ToRadians(-8), MathHelper.ToRadians(8), (float)Math.Sin(DrawCounter / 40f) * 0.5f + 0.5f);

            // Apply death animation
            if (startDeath)
            {
                if (DeathCounter < 8)
                {
                    baseScale *= MathHelper.Lerp(1f, 1.5f, DeathCounter / 8f);
                }
                else
                {
                    baseScale *= MathHelper.Lerp(1.5f, 0, (DeathCounter - 8) / 7f);
                }
            }

            // Apply pulsing effect
            if (Pulsing)
            {
                float pulseMultiplier = 1f + (float)Math.Sin(DrawCounter * 0.1f * PulseSpeed) * 0.2f;
                baseScale *= pulseMultiplier;
            }

            // Apply spinning effect
            if (Spinning)
            {
                baseRotation += DrawCounter * 0.05f * SpinSpeed;
            }

            Vector2 finalPosition = position + positionOffset - Main.screenPosition;

            // Draw aura effect (behind bullet)
            if (HasAura)
            {
                float auraScale = baseScale * AuraSize;
                float auraAlpha = 0.3f + (float)Math.Sin(DrawCounter * 0.08f) * 0.2f;

                spriteBatch.Draw(bulletTexture, finalPosition, null,
                    AuraColor * auraAlpha, baseRotation, bulletTexture.Size() / 2f,
                    auraScale, SpriteEffects.None, 0f);
            }

            // Draw glow effect (behind bullet)
            if (GlowIntensity > 0)
            {
                float glowScale = baseScale * 1.3f;
                spriteBatch.Draw(bulletTexture, finalPosition, null,
                    GlowColor * GlowIntensity, baseRotation, bulletTexture.Size() / 2f,
                    glowScale, SpriteEffects.None, 0f);
            }

            // Draw trail effect (behind bullet)
            if (HasTrail)
            {
                for (int i = 1; i <= 3; i++)
                {
                    Vector2 trailOffset = -Vector2.UnitX * i * 4f;
                    float trailAlpha = 0.6f - (i * 0.15f);
                    float trailScale = baseScale * (1f - i * 0.1f);

                    spriteBatch.Draw(bulletTexture, finalPosition + trailOffset, null,
                        TrailColor * trailAlpha, baseRotation, bulletTexture.Size() / 2f,
                        trailScale, SpriteEffects.None, 0f);
                }
            }

            // Draw main bullet
            spriteBatch.Draw(bulletTexture, finalPosition, null, drawColor, baseRotation,
                bulletTexture.Size() / 2f, baseScale, SpriteEffects.None, 1f);

            // Draw custom effects from accessories
            foreach (var customDraw in CustomDrawMethods)
            {
                customDraw(spriteBatch, finalPosition, baseScale, baseRotation);
            }
        }
    }
}