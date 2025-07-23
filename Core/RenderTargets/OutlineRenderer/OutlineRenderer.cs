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
        public static RenderTarget2D fillRenderTarget;
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

            InitializeRenderTargets(gD, RTwidth, RTheight);

            var validProjectiles = Main.projectile.Where(proj => proj.active && ShouldDrawOutline(proj)).ToList();
            var validNPCs = Main.npc.Where(npc => npc.active && ShouldDrawOutline(npc)).ToList();

            if (validProjectiles.Count == 0 && validNPCs.Count == 0)
            {
                ClearRenderTargets(gD);
                return;
            }

            gD.SetRenderTarget(processedRenderTarget);
            gD.Clear(Color.Transparent);

            var entityGroups = GroupEntitiesByOutlineGroup(validProjectiles, validNPCs);

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

        private List<EntityGroup> GroupEntitiesByOutlineGroup(List<Projectile> projectiles, List<NPC> npcs)
        {
            var groups = new List<EntityGroup>();
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

            // Group by entity type to allow outline merging for same entity types
            var groupedByEntityType = allEntities.GroupBy(x => new
            {
                EntityType = x.entity switch
                {
                    Projectile proj => $"Projectile_{proj.type}",
                    NPC npc => $"NPC_{npc.type}",
                    _ => "Unknown"
                },
                OutlineColor = x.outlineEntity.OutlineColor
            });

            foreach (var entityTypeGroup in groupedByEntityType)
            {
                var fillGroups = entityTypeGroup.GroupBy(x => new
                {
                    HasCustomDraw = x.outlineEntity.CustomDrawFunction != null,
                    HasFillTexture = x.outlineEntity.FillTexture != null,
                    HasFillColor = x.outlineEntity.FillColor.HasValue,
                    FillTexture = x.outlineEntity.FillTexture?.Name ?? "",
                    FillColor = x.outlineEntity.FillColor ?? Color.Transparent,
                    CustomDrawFunctionHash = x.outlineEntity.CustomDrawFunction?.GetHashCode() ?? 0
                });

                foreach (var fillGroup in fillGroups)
                {
                    groups.Add(new EntityGroup
                    {
                        Entities = fillGroup.ToList(),
                        OutlineColor = entityTypeGroup.Key.OutlineColor,
                        FillColor = fillGroup.Key.FillColor,
                        FillTexture = fillGroup.First().outlineEntity.FillTexture,
                        UseFillColor = fillGroup.Key.HasFillColor && !fillGroup.Key.HasFillTexture && !fillGroup.Key.HasCustomDraw,
                        CustomDrawFunction = fillGroup.First().outlineEntity.CustomDrawFunction
                    });
                }
            }

            return groups;
        }

        private void RenderEntityGroup(GraphicsDevice gD, EntityGroup group, int RTwidth, int RTheight)
        {
            gD.SetRenderTarget(outlineRenderTarget);
            gD.Clear(Color.Transparent);

            Main.spriteBatch.Begin(default, BlendState.AlphaBlend, Main.DefaultSamplerState, default, default, default);

            foreach (var (entity, outlineEntity) in group.Entities)
            {
                DrawEntityToTarget(entity);
            }

            Main.spriteBatch.End();

            gD.SetRenderTarget(fillRenderTarget);
            gD.Clear(Color.Transparent);

            if (group.CustomDrawFunction != null)
            {
                Main.spriteBatch.Begin(default, BlendState.AlphaBlend, Main.DefaultSamplerState, default, default, default);

                foreach (var (entity, outlineEntity) in group.Entities)
                {
                    group.CustomDrawFunction(Main.spriteBatch, gD, RTwidth, RTheight, entity);
                }

                Main.spriteBatch.End();
            }
            else if (group.FillTexture != null)
            {
                // For FillTexture, draw each entity individually with that texture
                Main.spriteBatch.Begin(default, BlendState.AlphaBlend, Main.DefaultSamplerState, default, default, default);
                foreach (var (entity, outlineEntity) in group.Entities)
                {
                    DrawEntityWithCustomTexture(entity, group.FillTexture);
                }
                Main.spriteBatch.End();
            }
            else if (group.UseFillColor)
            {
                gD.Clear(group.FillColor);
            }
            else
            {
                Main.spriteBatch.Begin(default, BlendState.AlphaBlend, Main.DefaultSamplerState, default, default, default);
                foreach (var (entity, outlineEntity) in group.Entities)
                {
                    DrawEntityToTarget(entity);
                }
                Main.spriteBatch.End();
            }

            gD.SetRenderTarget(processedRenderTarget);

            Effect outline = OvermorrowModFile.Instance.Outline.Value;
            if (outline != null)
            {
                outline.Parameters["PixelSize"].SetValue(new Vector2(1f / RTwidth, 1f / RTheight));
                outline.Parameters["OutlineColor"].SetValue(group.OutlineColor.ToVector4());
                outline.Parameters["FillColor"].SetValue(group.FillColor.ToVector4());
                outline.Parameters["UseFillColor"].SetValue(group.UseFillColor);

                Main.spriteBatch.Begin(default, BlendState.AlphaBlend, Main.DefaultSamplerState, default, RasterizerState.CullNone, outline);

                gD.Textures[0] = outlineRenderTarget;
                gD.Textures[1] = fillRenderTarget;

                Main.spriteBatch.Draw(outlineRenderTarget, Vector2.Zero, Color.White);
                Main.spriteBatch.End();
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
    }
}