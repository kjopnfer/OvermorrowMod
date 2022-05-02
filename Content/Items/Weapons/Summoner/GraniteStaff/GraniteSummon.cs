using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.Buffs.Summon;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Summoner.GraniteStaff
{
    public class GraniteSummon : ModProjectile
    {
        int Random2 = Main.rand.Next(-15, 12);
        int Random = Main.rand.Next(1, 3);
        public override bool? CanDamage() => false;

        private int timer = 0;
        private int PosCheck = 0;
        private int PosPlay = 0;
        private int Pos = 0;
        private int NumProj = 0;
        private int movement2 = 0;
        float NPCtargetX = 0;
        float NPCtargetY = 0;
        int mrand = Main.rand.Next(-100, 101);
        int mrand2 = Main.rand.Next(-100, 101);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Elemental");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 38;
            Projectile.minionSlots = 1f;
            Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 200000;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            #region Active check
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<GraniteEleBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<GraniteEleBuff>()))
            {
                Projectile.timeLeft = 2;
            }
            #endregion


            NumProj = Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<GraniteSummon>()];
            PosCheck++;
            if (PosCheck == 2)
            {
                PosPlay = NumProj;
            }

            if (PosCheck == 5)
            {
                Pos = PosPlay * 30;
            }
            float distanceFromTarget = 500f;
            Vector2 targetCenter = Projectile.position;
            bool foundTarget = false;

            Projectile.tileCollide = false;
            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Main.player[Projectile.owner].Center);
                        bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;

                        if (((closest && inRange) || !foundTarget))
                        {
                            NPCtargetX = npc.Center.X;
                            NPCtargetY = npc.Center.Y;
                            Vector2 Rot = npc.Center;
                            //Projectile.rotation = (Rot - Projectile.Center).ToRotation();
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }


            if (foundTarget && Main.player[Projectile.owner].channel)
            {
                movement2++;

                if (movement2 == 70)
                {
                    mrand2 = Main.rand.Next(-170, 171);
                    mrand = Main.rand.Next(-170, 171);
                    movement2 = 0;
                }

                if (NPCtargetX > Projectile.Center.X)
                {
                    Projectile.spriteDirection = -1;
                }
                else
                {
                    Projectile.spriteDirection = 1;
                }

                if (NPCtargetX + mrand > Projectile.Center.X)
                {
                    Projectile.velocity.X += 0.9f;
                }

                if (NPCtargetX + mrand < Projectile.Center.X)
                {
                    Projectile.velocity.X -= 0.9f;
                }

                if (NPCtargetY + mrand2 > Projectile.Center.Y)
                {
                    Projectile.velocity.Y += 2f;
                }
                if (NPCtargetY + mrand2 < Projectile.Center.Y)
                {
                    Projectile.velocity.Y -= 2f;
                }


                if (Projectile.velocity.Y < -9f)
                {
                    Projectile.velocity.Y = -9f;
                }

                if (Projectile.velocity.Y > 9f)
                {
                    Projectile.velocity.Y = 9f;
                }


                if (Projectile.velocity.X < -9f)
                {
                    Projectile.velocity.X = -9f;
                }

                if (Projectile.velocity.X > 9f)
                {
                    Projectile.velocity.X = 9f;
                }

            }
            else
            {
                Projectile.spriteDirection = -Main.player[Projectile.owner].direction;


                if (Main.player[Projectile.owner].direction == -1)
                {
                    Projectile.position.X = Main.player[Projectile.owner].Center.X + Pos;
                    Projectile.position.Y = Main.player[Projectile.owner].Center.Y - 32;
                }

                if (Main.player[Projectile.owner].direction == 1)
                {
                    Projectile.position.X = Main.player[Projectile.owner].Center.X - Pos - 32;
                    Projectile.position.Y = Main.player[Projectile.owner].Center.Y - 32;
                }

                Projectile.velocity.Y = 0f;
                Projectile.velocity.X = 0f;

            }

            if (Main.player[Projectile.owner].channel && foundTarget)
            {
                timer++;
                if (timer == 45 + Random2)
                {
                    Random2 = Main.rand.Next(-15, 12);
                    Random = Main.rand.Next(-2, 3);
                    Vector2 position = Projectile.Center;
                    Vector2 targetPosition = Main.MouseWorld;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    Vector2 newpoint2 = new Vector2(direction.X, direction.Y).RotatedByRandom(MathHelper.ToRadians(1.5f));
                    float speed = Random + 20f;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, newpoint2.X * speed, newpoint2.Y * speed, ModContent.ProjectileType<GraniteLaser>(), Projectile.damage, 1f, Projectile.owner, 0f);
                    timer = 0;
                }
            }

            // Loop through the 4 animation frames, spending 5 ticks on each.
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }
        public override void PostDraw(Color lightColor)
        {
            //Texture2D texture = mod.GetTexture("Projectiles/Summon/StormWhelp_Glowmask");

            int num154 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type];
            int y2 = num154 * Projectile.frame;

            Texture2D texture = ModContent.Request<Texture2D>("OvermorrowMod/Content/Projectiles/Summon/GraniteSummon_Glow").Value;
            Rectangle drawRectangle = new Microsoft.Xna.Framework.Rectangle(0, y2, TextureAssets.Projectile[Projectile.type].Value.Width, num154);
            Main.EntitySpriteDraw
            (
                texture,
                new Vector2
                (
                    Projectile.position.X - Main.screenPosition.X + Projectile.width * 0.5f,
                    Projectile.position.Y - Main.screenPosition.Y + Projectile.height - drawRectangle.Height * 0.5f
                ),
                drawRectangle,
                Color.White,
                Projectile.rotation,
                new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2),
                Projectile.scale,
                Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0
            );
        }
    }
}
