using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.RenderTargets
{
    class OutlineRenderer : ILoadable
    {
        public static RenderTarget2D outlineRenderTarget;
        public static RenderTarget2D fillRenderTarget; // New: for custom fill textures
        public static RenderTarget2D processedRenderTarget;

        public float Priority => 1.1f;

        public void Load(Mod mod)
        {
            if (Main.dedServ)
                return;

            Terraria.On_Main.DrawNPCs += DrawOutlines;
            Terraria.On_Main.CheckMonoliths += BuildOutlineTargets;
        }

        public void Unload()
        {
            outlineRenderTarget?.Dispose();
            fillRenderTarget?.Dispose();
            processedRenderTarget?.Dispose();
            outlineRenderTarget = null;
            fillRenderTarget = null;
            processedRenderTarget = null;
        }

        private void DrawOutlines(Terraria.On_Main.orig_DrawNPCs orig, Main self, bool behindTiles = false)
        {
            orig(self, behindTiles);

            if (!behindTiles && processedRenderTarget != null)
            {
                DrawFinalOutlines();
            }
        }

        private void DrawFinalOutlines()
        {
            if (Main.dedServ || Main.spriteBatch == null || processedRenderTarget == null)
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone,
                null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(processedRenderTarget, Vector2.Zero, Color.White);

            spriteBatch.End();
            spriteBatch.Begin(default, default, Main.DefaultSamplerState, default, default, default, Main.GameViewMatrix.TransformationMatrix);
        }

        private void BuildOutlineTargets(Terraria.On_Main.orig_CheckMonoliths orig)
        {
            orig();

            if (Main.gameMenu || Main.spriteBatch == null || Main.graphics.GraphicsDevice == null)
                return;

            var gD = Main.graphics.GraphicsDevice;
            int RTwidth = Main.screenWidth;
            int RTheight = Main.screenHeight;

            // Initialize render targets
            InitializeRenderTargets(gD, RTwidth, RTheight);

            var validProjectiles = Main.projectile.Where(proj => proj.active && ShouldDrawOutline(proj)).ToList();
            var validNPCs = Main.npc.Where(npc => npc.active && ShouldDrawOutline(npc)).ToList();

            if (validProjectiles.Count == 0 && validNPCs.Count == 0)
            {
                ClearRenderTargets(gD);
                return;
            }

            // Clear the processed render target once at the start
            gD.SetRenderTarget(processedRenderTarget);
            gD.Clear(Color.Transparent);

            // Group entities by their fill type for optimal rendering
            var entityGroups = GroupEntitiesByFillType(validProjectiles, validNPCs);

            // Render each group
            foreach (var group in entityGroups)
            {
                RenderEntityGroup(gD, group, RTwidth, RTheight);
            }

            gD.SetRenderTarget(null);
        }

        private void InitializeRenderTargets(GraphicsDevice gD, int width, int height)
        {
            var targetSize = new Vector2(width, height);

            if (outlineRenderTarget?.Size() != targetSize)
            {
                outlineRenderTarget?.Dispose();
                outlineRenderTarget = new RenderTarget2D(gD, width, height, default, default, default, default, RenderTargetUsage.PreserveContents);
            }

            if (fillRenderTarget?.Size() != targetSize)
            {
                fillRenderTarget?.Dispose();
                fillRenderTarget = new RenderTarget2D(gD, width, height, default, default, default, default, RenderTargetUsage.PreserveContents);
            }

            if (processedRenderTarget?.Size() != targetSize)
            {
                processedRenderTarget?.Dispose();
                processedRenderTarget = new RenderTarget2D(gD, width, height, default, default, default, default, RenderTargetUsage.PreserveContents);
            }
        }

        private void ClearRenderTargets(GraphicsDevice gD)
        {
            gD.SetRenderTarget(processedRenderTarget);
            gD.Clear(Color.Transparent);
        }

        private List<EntityGroup> GroupEntitiesByFillType(List<Projectile> projectiles, List<NPC> npcs)
        {
            var groups = new List<EntityGroup>();

            // Group all entities by their fill configuration
            var allEntities = new List<(Entity entity, IOutlineEntity outlineEntity)>();

            foreach (var proj in projectiles)
            {
                if (proj.ModProjectile is IOutlineEntity outlineEntity)
                    allEntities.Add((proj, outlineEntity));
            }

            foreach (var npc in npcs)
            {
                if (npc.ModNPC is IOutlineEntity outlineEntity)
                    allEntities.Add((npc, outlineEntity));
            }

            // Group by fill type and properties
            var grouped = allEntities.GroupBy(x => new
            {
                HasFillTexture = x.outlineEntity.FillTexture != null,
                HasFillColor = x.outlineEntity.FillColor.HasValue,
                FillTexture = x.outlineEntity.FillTexture?.Name ?? "",
                FillColor = x.outlineEntity.FillColor ?? Color.Transparent,
                OutlineColor = x.outlineEntity.OutlineColor
            });

            foreach (var group in grouped)
            {
                groups.Add(new EntityGroup
                {
                    Entities = group.ToList(),
                    OutlineColor = group.Key.OutlineColor,
                    FillColor = group.Key.FillColor,
                    FillTexture = group.First().outlineEntity.FillTexture,
                    UseFillColor = group.Key.HasFillColor && !group.Key.HasFillTexture
                });
            }

            return groups;
        }

        private void RenderEntityGroup(GraphicsDevice gD, EntityGroup group, int RTwidth, int RTheight)
        {
            // Render outline shapes to outline target
            gD.SetRenderTarget(outlineRenderTarget);
            gD.Clear(Color.Transparent);

            Main.spriteBatch.Begin(default, BlendState.AlphaBlend, Main.DefaultSamplerState, default, default, default);

            foreach (var (entity, outlineEntity) in group.Entities)
            {
                DrawEntityToTarget(entity);
            }

            Main.spriteBatch.End();

            // Render fill content to fill target
            gD.SetRenderTarget(fillRenderTarget);
            gD.Clear(Color.Transparent);

            if (group.FillTexture != null)
            {
                // For custom textures, draw the FULL texture as a continuous background
                // This creates the "window" effect where entities reveal parts of a larger texture
                DrawContinuousBackground(gD, group.FillTexture, RTwidth, RTheight);
            }
            else if (group.UseFillColor)
            {
                // For solid colors, fill the entire render target
                gD.Clear(group.FillColor);
            }
            else
            {
                // For original textures, draw each entity individually
                Main.spriteBatch.Begin(default, BlendState.AlphaBlend, Main.DefaultSamplerState, default, default, default);
                foreach (var (entity, outlineEntity) in group.Entities)
                {
                    DrawEntityToTarget(entity);
                }
                Main.spriteBatch.End();
            }

            // Apply shader to combine outline and fill (don't clear processed target here)
            gD.SetRenderTarget(processedRenderTarget);

            Effect outline = OvermorrowModFile.Instance.Outline.Value;
            if (outline != null)
            {
                outline.Parameters["PixelSize"].SetValue(new Vector2(1f / RTwidth, 1f / RTheight));
                outline.Parameters["OutlineColor"].SetValue(group.OutlineColor.ToVector4());
                outline.Parameters["FillColor"].SetValue(group.FillColor.ToVector4());
                outline.Parameters["UseFillColor"].SetValue(group.UseFillColor);

                Main.spriteBatch.Begin(default, BlendState.AlphaBlend, Main.DefaultSamplerState, default, RasterizerState.CullNone, outline);

                // Set both textures for the shader
                gD.Textures[0] = outlineRenderTarget;
                gD.Textures[1] = fillRenderTarget;

                Main.spriteBatch.Draw(outlineRenderTarget, Vector2.Zero, Color.White);
                Main.spriteBatch.End();
            }
        }

        /// <summary>
        /// Draws a continuous background texture that can be revealed through entity shapes.
        /// This creates a "portal" or "window" effect where the background appears continuous across all entities.
        /// </summary>
        private void DrawContinuousBackground(GraphicsDevice gD, Texture2D backgroundTexture, int screenWidth, int screenHeight)
        {
            Main.spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.LinearWrap, default, default, default);

            DrawStaticBackground(backgroundTexture, screenWidth, screenHeight);

            Main.spriteBatch.End();
        }

        /// <summary>
        /// Draws a static background that doesn't move with the camera.
        /// Creates a "window" effect where moving entities reveal different parts of the background.
        /// </summary>
        private void DrawStaticBackground(Texture2D texture, int screenWidth, int screenHeight)
        {
            // Calculate how many tiles we need to cover the screen
            int tilesX = (screenWidth / texture.Width) + 2;
            int tilesY = (screenHeight / texture.Height) + 2;

            // Use screen position to offset the background pattern
            Vector2 offset = new Vector2(
                Main.screenPosition.X % texture.Width,
                Main.screenPosition.Y % texture.Height
            );

            for (int x = -1; x < tilesX; x++)
            {
                for (int y = -1; y < tilesY; y++)
                {
                    Vector2 position = new Vector2(x * texture.Width, y * texture.Height) - offset;
                    Main.spriteBatch.Draw(texture, position, Color.White);
                }
            }
        }

        private void DrawEntityToTarget(Entity entity)
        {
            Texture2D texture = GetEntityTexture(entity);
            if (texture == null) return;

            Vector2 position = entity.Center - Main.screenPosition;
            Vector2 origin = texture.Size() / 2f;
            float rotation = entity switch
            {
                NPC npc => npc.rotation,
                Projectile proj => proj.rotation,
                _ => 0f
            };
            float scale = entity switch
            {
                NPC npc => npc.scale,
                Projectile proj => proj.scale,
                _ => 1f
            };

            Main.spriteBatch.Draw(texture, position, null, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);
        }

        private void DrawEntityWithCustomTexture(Entity entity, Texture2D customTexture)
        {
            Vector2 position = entity.Center - Main.screenPosition;
            Vector2 origin = customTexture.Size() / 2f;
            float rotation = entity switch
            {
                NPC npc => npc.rotation,
                Projectile proj => proj.rotation,
                _ => 0f
            };
            float scale = entity switch
            {
                NPC npc => npc.scale,
                Projectile proj => proj.scale,
                _ => 1f
            };

            Main.spriteBatch.Draw(customTexture, position, null, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);
        }

        private void DrawEntityWithSolidColor(Entity entity, Color color)
        {
            Texture2D pixelTexture = TextureAssets.MagicPixel.Value;
            Vector2 position = entity.Center - Main.screenPosition;
            float scale = entity switch
            {
                NPC npc => npc.scale,
                Projectile proj => proj.scale,
                _ => 1f
            };

            // Draw a colored rectangle matching the entity's size
            Rectangle bounds = entity.Hitbox;
            Vector2 size = new Vector2(bounds.Width, bounds.Height) * scale;
            Vector2 origin = size / 2f;

            Main.spriteBatch.Draw(pixelTexture, position, null, color, 0f, Vector2.Zero, size, SpriteEffects.None, 0f);
        }

        private Texture2D GetEntityTexture(Entity entity)
        {
            return entity switch
            {
                NPC npc when npc.type > 0 && npc.type < Terraria.ID.NPCID.Count => TextureAssets.Npc[npc.type].Value,
                Projectile proj when proj.ModProjectile != null => ModContent.Request<Texture2D>(proj.ModProjectile.Texture).Value,
                _ => null
            };
        }

        private bool ShouldDrawOutline(Entity entity)
        {
            return entity switch
            {
                NPC npc when npc.ModNPC is IOutlineEntity outline => outline.ShouldDrawOutline,
                Projectile proj when proj.ModProjectile is IOutlineEntity outline => outline.ShouldDrawOutline,
                _ => false
            };
        }

        private class EntityGroup
        {
            public List<(Entity entity, IOutlineEntity outlineEntity)> Entities { get; set; }
            public Color OutlineColor { get; set; }
            public Color FillColor { get; set; }
            public Texture2D FillTexture { get; set; }
            public bool UseFillColor { get; set; }
        }
    }
}