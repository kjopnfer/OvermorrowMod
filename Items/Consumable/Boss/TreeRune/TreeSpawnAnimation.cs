using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.NPCs.Bosses.TreeBoss;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace OvermorrowMod.Items.Consumable.Boss.TreeRune
{
    public class TreeSpawnAnimation : ModProjectile
    {
        public Player PlayerSummoner;
        public Vector2 InitialPosition;
        public Vector2 HoverPosition;

        public float MAX_RISE_TIME = 200f;
        public float MAX_ROTATE_TIME = 600f;

        public bool RunOnce = true;

        public enum ProjectileStates
        {
            Rising = 0,
            Hovering = 1,
            Rotating = 2
        }
        public int States = (int)ProjectileStates.Rising;

        public int RADIUS = 300;

        public override string Texture => "OvermorrowMod/Items/Consumable/Boss/TreeRune/TreeRune";
        public override bool? CanCutTiles() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iorich Spawner");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 25;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 28;
            projectile.tileCollide = false;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.timeLeft = 60 * 19;
            projectile.penetrate = -1;
        }
        public ref float ProjectileCounter => ref projectile.ai[0];
        public override void AI()
        {
            if (RunOnce)
            {
                InitialPosition = PlayerSummoner.Center;
                RunOnce = false;
            }

            ProjectileCounter++;

            switch (States)
            {
                case (int)ProjectileStates.Rising: // Rotate up into the air
                    projectile.rotation = MathHelper.Lerp(0, MathHelper.TwoPi * 4, ProjectileCounter / MAX_RISE_TIME);
                    projectile.Center = Vector2.Lerp(InitialPosition, InitialPosition - Vector2.UnitY * RADIUS, ProjectileCounter / MAX_RISE_TIME);

                    if (ProjectileCounter == MAX_RISE_TIME)
                    {
                        ProjectileCounter = 0;
                        States = (int)ProjectileStates.Hovering;
                    }
                    break;
                case (int)ProjectileStates.Hovering: // Pulsate and print dialogue, as well as hovering up and down
                    /*string[] Texts = new string[]
                    {
                        "I heed thy call.",
                        "Thou wishes to unlock the secrets of the Dryads?",
                        "Very well, I shall test thy resolve",
                    };*/

                    string[] Texts = new string[]
                    {
                        "u wanna hear a funny joke",
                        "im on a seafood diet",
                        "i see food i eat it",
                    };

                    projectile.localAI[0]++;

                    Vector2 HoverPosition = InitialPosition - Vector2.UnitY * RADIUS;
                    projectile.Center = Vector2.Lerp(HoverPosition, HoverPosition + Vector2.UnitY * 50, (float)Math.Sin(ProjectileCounter / 100f));

                    if (ProjectileCounter % 180 == 0)
                    {
                        Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<TreeRune_Pulse>(), 0, 0f, Main.myPlayer, projectile.whoAmI);

                        BossText(Texts[(int)projectile.ai[1]]);
                        projectile.ai[1]++;
                    }

                    if (ProjectileCounter > 540) projectile.rotation += 0.64f;

                    if (ProjectileCounter == MAX_ROTATE_TIME)
                    {
                        Main.PlaySound(SoundID.Item46, projectile.Center);

                        ProjectileCounter = 0;
                        projectile.ai[1] = 0;
                        States = (int)ProjectileStates.Rotating;
                    }
                    break;
                case (int)ProjectileStates.Rotating: // Rotate around the position that the player spawned, and also spawn Iorich
                    if (ProjectileCounter == 1)
                    {
                        NPC.NewNPC((int)InitialPosition.X, (int)(InitialPosition.Y - 50f), ModContent.NPCType<TreeBoss>(), 0, -1f, 0f, 0f, 0f, 255);
                    }

                    projectile.rotation += 0.64f;

                    projectile.ai[1] += MathHelper.Lerp(0, 0.065f, Utils.Clamp(projectile.ai[0], 0, 90f) / 90f);

                    projectile.Center = InitialPosition - new Vector2(0, RADIUS).RotatedBy(projectile.ai[1]);      
                    break;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/Items/Consumable/Boss/TreeRune/TreeRune_Trail");

            if (States == (int)ProjectileStates.Hovering)
            {
                // this gets the npc's frame
                int num178 = 60; // i think this controls the distance of the pulse, maybe color too, if we make it high: it is weaker
                int num179 = 60; // changing this value makes the pulsing effect rapid when lower, and slower when higher

                // default value
                int num177 = 6; // ok i think this controls the number of afterimage frames
                float num176 = 1f - (float)Math.Cos((projectile.localAI[0] - (float)num178) / (float)num179 * ((float)Math.PI * 2f));  // this controls pulsing effect
                num176 /= 3f;
                float scaleFactor10 = 10f; // Change scale factor of the pulsing effect and how far it draws outwards

                int num154 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
                int y2 = num154 * projectile.frame;
                Rectangle drawRectangle = new Rectangle(0, y2, Main.projectileTexture[projectile.type].Width, num154);

                Vector2 drawOrigin = drawRectangle.Size() / 2f;

                // ok this is the pulsing effect drawing
                for (int num164 = 1; num164 < num177; num164++)
                {
                    // these assign the color of the pulsing
                    Color spriteColor = Color.LightGreen;
                    //spriteColor = projectile.GetAlpha(spriteColor);
                    //spriteColor *= 1f - num176; // num176 is put in here to effect the pulsing

                    // num176 is used here too
                    Vector2 vector45 = projectile.Center + Utils.ToRotationVector2((float)num164 / (float)num177 * ((float)Math.PI * 2f) + projectile.rotation) * scaleFactor10 * num176 - Main.screenPosition;
                    vector45 -= new Vector2(texture.Width, texture.Height) * projectile.scale / 2f;
                    vector45 += drawOrigin * projectile.scale + new Vector2(0f, projectile.gfxOffY);

                    // the actual drawing of the pulsing effect
                    spriteBatch.Draw(texture, vector45, new Microsoft.Xna.Framework.Rectangle?(drawRectangle), spriteColor, projectile.rotation, drawOrigin, projectile.scale, projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
                }
            }

            if (States == (int)ProjectileStates.Rotating)
            {
                Color color = new Color(135, 255, 141);
                int num154 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
                int y2 = num154 * projectile.frame;
                Rectangle drawRectangle = new Rectangle(0, y2, Main.projectileTexture[projectile.type].Width, num154);

                Vector2 origin2 = drawRectangle.Size() / 2f;
                var off = new Vector2(projectile.width / 2f, projectile.height / 2f);


                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
                {
                    Color color2 = color;
                    color2 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                    Vector2 value4 = projectile.oldPos[i];
                    float num165 = projectile.oldRot[i];
                    Main.spriteBatch.Draw(texture, projectile.oldPos[i] - Main.screenPosition + off, new Microsoft.Xna.Framework.Rectangle?(drawRectangle), Color.Lerp(color2, Color.Transparent, Utils.Clamp(projectile.alpha, 0, 255) / 255f), num165, origin2, projectile.scale, SpriteEffects.None, 0f);
                }
            }

            return base.PreDraw(spriteBatch, lightColor);
        }

        public override void Kill(int timeLeft)
        {
            #region Dust Code
            Vector2 vector23 = projectile.Center + Vector2.One * -20f;
            int num137 = 40;
            int num138 = num137;
            for (int num139 = 0; num139 < 4; num139++)
            {
                int num140 = Dust.NewDust(vector23, num137, num138, 89, 0f, 0f, 100, default(Color), 0.25f);
                Main.dust[num140].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num137 / 2f;
            }

            for (int num141 = 0; num141 < 10; num141++)
            {
                int num142 = Dust.NewDust(vector23, num137, num138, 107, 0f, 0f, 200, default(Color), 0.7f);
                Main.dust[num142].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num137 / 2f;
                Main.dust[num142].noGravity = true;
                Main.dust[num142].noLight = true;
                Dust dust = Main.dust[num142];
                dust.velocity *= 3f;
                dust = Main.dust[num142];
                dust.velocity += projectile.DirectionTo(Main.dust[num142].position) * (2f + Main.rand.NextFloat() * 4f);
                num142 = Dust.NewDust(vector23, num137, num138, 107, 0f, 0f, 100, default(Color), 0.25f);
                Main.dust[num142].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num137 / 2f;
                dust = Main.dust[num142];
                dust.velocity *= 2f;
                Main.dust[num142].noGravity = true;
                Main.dust[num142].fadeIn = 1f;
                Main.dust[num142].color = Color.Crimson * 0.5f;
                Main.dust[num142].noLight = true;
                dust = Main.dust[num142];
                dust.velocity += projectile.DirectionTo(Main.dust[num142].position) * 8f;
            }

            for (int num143 = 0; num143 < 10; num143++)
            {
                int num144 = Dust.NewDust(vector23, num137, num138, 107, 0f, 0f, 0, default(Color), 0.7f);
                Main.dust[num144].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy(projectile.velocity.ToRotation()) * num137 / 2f;
                Main.dust[num144].noGravity = true;
                Main.dust[num144].noLight = true;
                Dust dust = Main.dust[num144];
                dust.velocity *= 3f;
                dust = Main.dust[num144];
                dust.velocity += projectile.DirectionTo(Main.dust[num144].position) * 2f;
            }

            for (int num145 = 0; num145 < 50; num145++)
            {
                int num146 = Dust.NewDust(vector23, num137, num138, 89, 0f, 0f, 0, default(Color), 0.25f);
                Main.dust[num146].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy(projectile.velocity.ToRotation()) * num137 / 2f;
                Main.dust[num146].noGravity = true;
                Dust dust = Main.dust[num146];
                dust.velocity *= 3f;
                dust = Main.dust[num146];
                dust.velocity += projectile.DirectionTo(Main.dust[num146].position) * 3f;
            }
            #endregion
            base.Kill(timeLeft);
        }

        private void BossText(string text) // boss messages
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                CombatText.NewText(projectile.getRect(), new Color(0, 255, 191), text, true);
                Main.NewText(text, new Color(0, 255, 191));
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                CombatText.NewText(projectile.getRect(), new Color(0, 255, 191), text, true);
                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(text), new Color(0, 255, 191));
            }
        }
    }
}
