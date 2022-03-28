using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss
{
    public class LightningNode : ModNPC
    {
        public Color LinkColor = Color.Yellow;
        public Projectile ArenaCenter;

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.LostSoulFriendly;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Node");
        }

        public override void SetDefaults()
        {
            npc.width = npc.height = 70;
            npc.friendly = false;
            npc.timeLeft = 5;
            npc.lifeMax = 20;
            npc.aiStyle = -1;
            npc.knockBackResist = 0f;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.dontTakeDamage = true;
        }

        private ref float SelfID => ref npc.ai[0];
        private ref float ArenaID => ref npc.ai[1];
        private ref float AICounter => ref npc.ai[2];
        public override void AI()
        {
            ArenaCenter = Main.projectile[(int)ArenaID];

            // Read in the self ID
            // The links will be ID - 1, and ID + 1
            // If it is the first link, and the link before that is 0, link to the last link's ID
            // Same goes for the last link, where the next link will be the first link
            if (AICounter++ == 5)
            {

                switch (SelfID)
                {
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC NodeNPC = Main.npc[i];
                            if (NodeNPC.active && NodeNPC.ai[0] == SelfID + 1 && NodeNPC.modNPC is LightningNode)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<LightningLink>(), 35, 5f, Main.myPlayer, npc.whoAmI, NodeNPC.whoAmI);
                            }

                            if (NodeNPC.active && NodeNPC.ai[0] == SelfID - 1 && NodeNPC.modNPC is LightningNode)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<LightningLink>(), 35, 5f, Main.myPlayer, npc.whoAmI, NodeNPC.whoAmI);
                            }
                        }
                        break;
                    case 1:
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC NodeNPC = Main.npc[i];
                            if (NodeNPC.active && NodeNPC.ai[0] == SelfID + 1 && NodeNPC.modNPC is LightningNode)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<LightningLink>(), 35, 5f, Main.myPlayer, npc.whoAmI, NodeNPC.whoAmI);
                            }

                            if (NodeNPC.active && NodeNPC.ai[0] == 8 && NodeNPC.modNPC is LightningNode)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<LightningLink>(), 35, 5f, Main.myPlayer, npc.whoAmI, NodeNPC.whoAmI);
                            }
                        }
                        break;
                    case 8:
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC NodeNPC = Main.npc[i];
                            if (NodeNPC.active && NodeNPC.ai[0] == 1 && NodeNPC.modNPC is LightningNode)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<LightningLink>(), 35, 5f, Main.myPlayer, npc.whoAmI, NodeNPC.whoAmI);
                            }

                            if (NodeNPC.active && NodeNPC.ai[0] == SelfID - 1 && NodeNPC.modNPC is LightningNode)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<LightningLink>(), 35, 5f, Main.myPlayer, npc.whoAmI, NodeNPC.whoAmI);
                            }
                        }
                        break;
                }

                npc.velocity = ArenaCenter.DirectionFrom(npc.Center) * 0.66f;
            }

            if (npc.Hitbox.Intersects(ArenaCenter.Hitbox))
            {
                npc.active = false;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture = ModContent.GetTexture("OvermorrowMod/Content/NPCs/Bosses/SandstormBoss/PillarSpawner");
            //float mult = (0.55f + (float)Math.Sin(Main.GlobalTime) * 0.1f);
            //float scale = npc.scale * 2 * mult;

            Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, LinkColor, 0, new Vector2(texture.Width, texture.Height) / 2, 2, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, Color.White, 0, new Vector2(texture.Width, texture.Height) / 2, 1f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }

    public class LightningLink : Lightning
    {
        public float maxTime = 15;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Electricity");
        }

        public override void SafeSetDefaults()
        {
            projectile.width = 15;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = (int)maxTime;
            Length = 1f;
            Sine = true;
            Color1 = Color.LightYellow;
            Color2 = Color.Gold;
        }

        public override void AI()
        {
            if (Main.npc[(int)projectile.ai[0]].active && Main.npc[(int)projectile.ai[0]].modNPC is LightningNode &&
                Main.npc[(int)projectile.ai[1]].active && Main.npc[(int)projectile.ai[1]].modNPC is LightningNode)
            {
                projectile.timeLeft = 5;
            }

            Length = TRay.CastLength(projectile.Center, projectile.velocity, 2000f);
            float sway = 40f;
            float divider = 16f;
            Positions = CreateLightning(Main.npc[(int)projectile.ai[0]].Center, Main.npc[(int)projectile.ai[1]].Center, projectile.width, sway, divider);

            float progress = (maxTime - (float)projectile.timeLeft) / maxTime;
            float mult = (float)Math.Sin(progress * Math.PI);
            for (int i = 0; i < Positions.Count; i++)
            {
                Positions[i].Size = Positions[i].DefSize * mult;
            }
        }
    }
}