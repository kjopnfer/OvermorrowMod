using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;


namespace OvermorrowMod.NPCs.SpiderBoss
{
    public class SpiderBoss : ModNPC
    {

        readonly bool expert = Main.expertMode;
        private int experttimer = 0;
        private int spiderweb = 0;
        private int spidertimer = 0;
        private int stickywebtimer = 0;
        private int zerotimer = 0;
        private int lasertimer = 0;
        private int eggtimer = 0;
        private int webround = 0;
        private int eggcooldown = 0;
        private int webcooldown = 0;
        private float expertfire = 1;
        int RandomSpiVelo2 = Main.rand.Next(-4, 5);
        int RandomSpiVelo = Main.rand.Next(0, 2);
        int RandomAtt = Main.rand.Next(0, 7);
        int RandomSpiderTarget = Main.rand.Next(-100, 101);
        int RandomSpiPos = Main.rand.Next(-5, 6);
        int RandomSpiPos2 = Main.rand.Next(-2, 3);
        int spritetimer = 0;
        int frame = 1;


        public override void SetDefaults()
        {
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/DanceoftheAshenSerpent");
            npc.aiStyle = 0;
            npc.width = 62;
            npc.height = 62;
            npc.scale = 1.2f;
            npc.knockBackResist = 0f;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.damage = 30;
            npc.defense = 7;
            npc.lifeMax = 10000;
            npc.lavaImmune = true;
            npc.HitSound = SoundID.NPCHit25;
            npc.DeathSound = SoundID.NPCDeath54;
            npc.value = 12f;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frame.Y = frameHeight * frame;
            if (Main.player[npc.target].position.X > npc.position.X && eggtimer == 0)
            {
                npc.spriteDirection = -1;
            }
        }

        public override void AI()
        {


            experttimer++;
            if (expert && experttimer == 1)
            {
                npc.life = 13500;
                npc.lifeMax = 13500;
                npc.damage = npc.damage / 2 + 7;
            }



            eggcooldown--;
            if (eggcooldown > 1 && RandomAtt == 4)
            {
                RandomAtt = 1;
                eggcooldown = -10;
            }

            webcooldown--;
            if (webcooldown > 1 && RandomAtt == 5)
            {
                RandomAtt = 3;
                webcooldown = -10;
            }

            if (expert)
            {
                expertfire = 1.5f;
            }



            spritetimer++;
            if (spritetimer > 4 && eggtimer == 0)
            {
                frame++;
                spritetimer = 0;
            }
            if (frame > 3 && eggtimer == 0)
            {
                frame = 0;
            }


            if (spiderweb == 0 && eggtimer == 0)
            {
                npc.rotation = (Main.player[npc.target].Center - npc.Center).ToRotation();
            }

            if (spiderweb > 0)
            {
                npc.rotation = 67.5f;
            }

            if (eggtimer > 0)
            {
                npc.rotation = 0f;
            }


            if (Main.player[npc.target].position.X < npc.position.X && zerotimer == 0 && spiderweb == 0 && lasertimer == 0 && eggtimer == 0)
            {
                npc.velocity.X -= 0.2f; // accelerate to the left
            }
            if (Main.player[npc.target].position.X > npc.position.X && zerotimer == 0 && spiderweb == 0 && lasertimer == 0 && eggtimer == 0)
            {
                npc.velocity.X += 0.2f; // accelerate to the right
            }
            if (Main.player[npc.target].position.Y < npc.position.Y && zerotimer == 0 && lasertimer == 0 && eggtimer == 0)
            {
                npc.velocity.Y -= 0.1f; // accelerate to the down
            }
            if (Main.player[npc.target].position.Y > npc.position.Y && zerotimer == 0 && lasertimer == 0 && eggtimer == 0)
            {
                npc.velocity.Y += 0.1f; // accelerate to the up
            }

            if (npc.velocity.Y > 7f)
            {
                npc.velocity.Y = 7f;
            }
            if (npc.velocity.Y < -7f)
            {
                npc.velocity.Y = -7f;
            }


            if (npc.velocity.X > 7f)
            {
                npc.velocity.X = 7f;
            }
            if (npc.velocity.X < -7f)
            {
                npc.velocity.X = -7f;
            }



            if (RandomAtt == 1)
            {
                spidertimer++;
                if (spidertimer > 160)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 4f * expertfire;
                    int type = mod.ProjectileType("SpiderBossFire");
                    int damage = 20;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item34, npc.position);
                }

                if (spidertimer == 200)
                {
                    RandomAtt = Main.rand.Next(0, 7);
                    spidertimer = 0;
                }
            }
            if (RandomAtt == 0)
            {
                zerotimer++;
                npc.velocity.Y = 0f;
                npc.velocity.X = 0f;

                if (zerotimer > 1 && Main.player[npc.target].Center.X < npc.Center.X)
                {
                    RandomSpiPos2 = Main.rand.Next(-2, 0);
                    RandomSpiPos = Main.rand.Next(-5, 0);
                }
                if (zerotimer > 1 && Main.player[npc.target].Center.X > npc.Center.X)
                {
                    RandomSpiPos2 = Main.rand.Next(0, 3);
                    RandomSpiPos = Main.rand.Next(0, 6);
                }

                if (zerotimer == 100)
                {
                    npc.defense = 13;
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 value1 = new Vector2(0f, -15f);
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, RandomSpiPos + direction.X / 40f, value1.Y + RandomSpiPos2, mod.ProjectileType("SpiderBossEgg"), npc.damage, 1f);
                    Main.PlaySound(SoundID.Item2, npc.position);
                }


                if (zerotimer == 105 && expert)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 value1 = new Vector2(0f, -15f);
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, RandomSpiPos + direction.X / 40f, value1.Y + RandomSpiPos2, mod.ProjectileType("SpiderBossEgg"), npc.damage, 1f);
                    Main.PlaySound(SoundID.Item2, npc.position);
                }

                if (zerotimer == 110)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 value1 = new Vector2(0f, -15f);
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, RandomSpiPos + direction.X / 40f, value1.Y + RandomSpiPos2, mod.ProjectileType("SpiderBossEgg"), npc.damage, 1f);
                    Main.PlaySound(SoundID.Item2, npc.position);
                }

                if (zerotimer == 115 && expert)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 value1 = new Vector2(0f, -15f);
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, RandomSpiPos + direction.X / 40f, value1.Y + RandomSpiPos2, mod.ProjectileType("SpiderBossEgg"), npc.damage, 1f);
                    Main.PlaySound(SoundID.Item2, npc.position);
                }

                if (zerotimer == 120)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 value1 = new Vector2(0f, -15f);
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, RandomSpiPos + direction.X / 40f, value1.Y + RandomSpiPos2, mod.ProjectileType("SpiderBossEgg"), npc.damage, 1f);
                    Main.PlaySound(SoundID.Item2, npc.position);
                }

                if (zerotimer == 125 && expert)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 value1 = new Vector2(0f, -15f);
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, RandomSpiPos + direction.X / 40f, value1.Y + RandomSpiPos2, mod.ProjectileType("SpiderBossEgg"), npc.damage, 1f);
                    Main.PlaySound(SoundID.Item2, npc.position);
                }

                if (zerotimer == 130)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 value1 = new Vector2(0f, -15f);
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, RandomSpiPos + direction.X / 40f, value1.Y + RandomSpiPos2, mod.ProjectileType("SpiderBossEgg"), npc.damage, 1f);
                    Main.PlaySound(SoundID.Item2, npc.position);
                }

                if (zerotimer == 135 && expert)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 value1 = new Vector2(0f, -15f);
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, RandomSpiPos + direction.X / 40f, value1.Y + RandomSpiPos2, mod.ProjectileType("SpiderBossEgg"), npc.damage, 1f);
                    Main.PlaySound(SoundID.Item2, npc.position);
                }

                if (zerotimer == 140)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 value1 = new Vector2(0f, -15f);
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, RandomSpiPos + direction.X / 40f, value1.Y + RandomSpiPos2, mod.ProjectileType("SpiderBossEgg"), npc.damage, 1f);
                    Main.PlaySound(SoundID.Item2, npc.position);
                }

                if (zerotimer == 145 && expert)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 value1 = new Vector2(0f, -15f);
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, RandomSpiPos + direction.X / 40f, value1.Y + RandomSpiPos2, mod.ProjectileType("SpiderBossEgg"), npc.damage, 1f);
                    Main.PlaySound(SoundID.Item2, npc.position);
                }

                if (zerotimer == 150)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 value1 = new Vector2(0f, -15f);
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, RandomSpiPos + direction.X / 40f, value1.Y + RandomSpiPos2, mod.ProjectileType("SpiderBossEgg"), npc.damage, 1f);
                    Main.PlaySound(SoundID.Item2, npc.position);
                }

                if (zerotimer == 155 && expert)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 value1 = new Vector2(0f, -15f);
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, RandomSpiPos + direction.X / 40f, value1.Y + RandomSpiPos2, mod.ProjectileType("SpiderBossEgg"), npc.damage, 1f);
                    Main.PlaySound(SoundID.Item2, npc.position);
                }
                if (zerotimer == 400)
                {
                    RandomAtt = Main.rand.Next(0, 7);
                    zerotimer = 0;
                    npc.defense = 7;
                }
            }

            if (RandomAtt == 2)
            {
                spiderweb++;
                if (spiderweb == 20)
                {
                    npc.velocity.Y = -2f;
                    Vector2 value1 = new Vector2(0f, 0f);
                    Projectile.NewProjectile(npc.Center.X - 7, npc.Center.Y - 750f, value1.X, value1.Y, mod.ProjectileType("SpiderWebDraw"), npc.damage - npc.damage, 1f);
                }

                if (npc.velocity.Y > 3f && spiderweb > 1)
                {
                    npc.velocity.Y = 3f;
                }
                if (npc.velocity.Y < -3f && spiderweb > 1)
                {
                    npc.velocity.Y = -3f;
                }


                if (spiderweb == 50)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 0.3f;
                    int type = mod.ProjectileType("SpiderFire2");
                    int damage = 30;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item74, npc.position);
                }

                if (spiderweb == 100 && RandomSpiVelo == 0)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 newpoint1 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(14));
                    Vector2 newpoint2 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(-14));
                    float speed = 0.4f;
                    int type = mod.ProjectileType("SpiderFire2");
                    int damage = 30;
                    Projectile.NewProjectile(position, newpoint1 * speed, type, damage, 0f, Main.myPlayer);
                    Projectile.NewProjectile(position, newpoint2 * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item74, npc.position);
                }


                if (expert && spiderweb == 100)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 newpoint1 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(25));
                    Vector2 newpoint2 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(-25));
                    float speed = 0.4f;
                    int type = mod.ProjectileType("SpiderFire2");
                    int damage = 30;
                    Projectile.NewProjectile(position, newpoint1 * speed, type, damage, 0f, Main.myPlayer);
                    Projectile.NewProjectile(position, newpoint2 * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item74, npc.position);
                }

                if (spiderweb == 100 && RandomSpiVelo == 1)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 1.3f;
                    int type = mod.ProjectileType("SpiderFire2");
                    int damage = 30;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item74, npc.position);
                }


                if (spiderweb == 150)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 1.3f;
                    int type = mod.ProjectileType("SpiderFire2");
                    int damage = 30;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item74, npc.position);
                }

                if (spiderweb == 200 && RandomSpiVelo == 1)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 newpoint1 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(14));
                    Vector2 newpoint2 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(-14));
                    float speed = 0.4f;
                    int type = mod.ProjectileType("SpiderFire2");
                    int damage = 30;
                    Projectile.NewProjectile(position, newpoint1 * speed, type, damage, 0f, Main.myPlayer);
                    Projectile.NewProjectile(position, newpoint2 * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item74, npc.position);
                }

                if (spiderweb == 200 && RandomSpiVelo == 0)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 1.3f;
                    int type = mod.ProjectileType("SpiderFire2");
                    int damage = 30;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item74, npc.position);
                }


                if (spiderweb == 250 && RandomSpiVelo == 0)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 1.3f;
                    int type = mod.ProjectileType("SpiderFire2");
                    int damage = 30;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item74, npc.position);
                }

                if (expert && spiderweb == 250)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 newpoint1 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(25));
                    Vector2 newpoint2 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(-25));
                    float speed = 0.4f;
                    int type = mod.ProjectileType("SpiderFire2");
                    int damage = 30;
                    Projectile.NewProjectile(position, newpoint1 * speed, type, damage, 0f, Main.myPlayer);
                    Projectile.NewProjectile(position, newpoint2 * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item74, npc.position);
                }

                if (spiderweb == 250 && RandomSpiVelo == 1)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 1.3f;
                    int type = mod.ProjectileType("SpiderFire2");
                    int damage = 30;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item74, npc.position);
                }

                if (spiderweb == 300 && RandomSpiVelo == 1)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 1.3f;
                    int type = mod.ProjectileType("SpiderFire2");
                    int damage = 30;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item74, npc.position);
                }


                if (spiderweb == 300 && RandomSpiVelo == 0)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 newpoint1 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(14));
                    Vector2 newpoint2 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(-14));
                    float speed = 0.4f;
                    int type = mod.ProjectileType("SpiderFire");
                    int damage = 30;
                    Projectile.NewProjectile(position, newpoint1 * speed, type, damage, 0f, Main.myPlayer);
                    Projectile.NewProjectile(position, newpoint2 * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item74, npc.position);
                }


                if (expert && spiderweb == 350)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 newpoint1 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(25));
                    Vector2 newpoint2 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(-25));
                    float speed = 0.4f;
                    int type = mod.ProjectileType("SpiderFire2");
                    int damage = 30;
                    Projectile.NewProjectile(position, newpoint1 * speed, type, damage, 0f, Main.myPlayer);
                    Projectile.NewProjectile(position, newpoint2 * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item74, npc.position);
                }


                if (spiderweb == 350 && RandomSpiVelo == 0)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 newpoint1 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(14));
                    Vector2 newpoint2 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(-14));
                    float speed = 0.4f;
                    int type = mod.ProjectileType("SpiderFire");
                    int damage = 30;
                    Projectile.NewProjectile(position, newpoint1 * speed, type, damage, 0f, Main.myPlayer);
                    Projectile.NewProjectile(position, newpoint2 * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item74, npc.position);
                }

                if (spiderweb == 350 && RandomSpiVelo == 1)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 1.3f;
                    int type = mod.ProjectileType("SpiderFire2");
                    int damage = 30;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item74, npc.position);
                }

                if (spiderweb == 400)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 newpoint1 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(14));
                    Vector2 newpoint2 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(-14));
                    float speed = 0.4f;
                    int type = mod.ProjectileType("SpiderFire");
                    int damage = 30;
                    Projectile.NewProjectile(position, newpoint1 * speed, type, damage, 0f, Main.myPlayer);
                    Projectile.NewProjectile(position, newpoint2 * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item74, npc.position);
                }

                if (expert && spiderweb == 400)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 newpoint1 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(25));
                    Vector2 newpoint2 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(-25));
                    float speed = 0.4f;
                    int type = mod.ProjectileType("SpiderFire");
                    int damage = 40;
                    Projectile.NewProjectile(position, newpoint1 * speed, type, damage, 0f, Main.myPlayer);
                    Projectile.NewProjectile(position, newpoint2 * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item74, npc.position);
                }

                if (spiderweb == 450)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 0.4f;
                    int type = mod.ProjectileType("SpiderFire");
                    int damage = 40;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item74, npc.position);
                }

                if (expert && spiderweb == 500)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 newpoint1 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(25));
                    Vector2 newpoint2 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(-25));
                    Vector2 newpoint3 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(10));
                    Vector2 newpoint4 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(-10));
                    float speed = 0.4f;
                    int type = mod.ProjectileType("SpiderFire");
                    int damage = 40;
                    Projectile.NewProjectile(position, newpoint1 * speed, type, damage, 0f, Main.myPlayer);
                    Projectile.NewProjectile(position, newpoint2 * speed, type, damage, 0f, Main.myPlayer);
                    Projectile.NewProjectile(position, newpoint3 * speed, type, damage, 0f, Main.myPlayer);
                    Projectile.NewProjectile(position, newpoint4 * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item74, npc.position);
                }

                if (spiderweb == 550 && RandomSpiVelo == 1)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 1.3f;
                    int type = mod.ProjectileType("SpiderFire");
                    int damage = 40;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item74, npc.position);
                }

                if (spiderweb == 550 && RandomSpiVelo == 0)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 newpoint1 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(14));
                    Vector2 newpoint2 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(-14));
                    float speed = 0.4f;
                    int type = mod.ProjectileType("SpiderFire");
                    int damage = 40;
                    Projectile.NewProjectile(position, newpoint1 * speed, type, damage, 0f, Main.myPlayer);
                    Projectile.NewProjectile(position, newpoint2 * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item74, npc.position);
                }

                if (spiderweb == 580 && RandomSpiVelo == 1)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 1.3f;
                    int type = mod.ProjectileType("SpiderFire");
                    int damage = 40;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item74, npc.position);
                }

                if (spiderweb == 580 && RandomSpiVelo == 0)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 newpoint1 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(14));
                    Vector2 newpoint2 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(-14));
                    float speed = 0.4f;
                    int type = mod.ProjectileType("SpiderFire");
                    int damage = 40;
                    Projectile.NewProjectile(position, newpoint1 * speed, type, damage, 0f, Main.myPlayer);
                    Projectile.NewProjectile(position, newpoint2 * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item74, npc.position);
                }



                if (spiderweb == 620 && RandomSpiVelo == 1)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 1.3f;
                    int type = mod.ProjectileType("SpiderFire");
                    int damage = 40;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item74, npc.position);
                }

                if (spiderweb == 620 && RandomSpiVelo == 0)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 newpoint1 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(14));
                    Vector2 newpoint2 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(-14));
                    float speed = 0.4f;
                    int type = mod.ProjectileType("SpiderFire");
                    int damage = 30;
                    Projectile.NewProjectile(position, newpoint1 * speed, type, damage, 0f, Main.myPlayer);
                    Projectile.NewProjectile(position, newpoint2 * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item74, npc.position);
                }


                if (spiderweb == 700)
                {
                    RandomAtt = Main.rand.Next(0, 7);
                    spiderweb = 0;
                }

            }

            if (RandomAtt == 3)
            {
                lasertimer++;
                if (lasertimer == 50)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 pos = npc.Center;
                    int damage = 20;
                    if (Main.LocalPlayer.ownedProjectileCounts[ProjectileType<SpiderRay>()] < 1)
                    {
                        Projectile.NewProjectile(pos.X, pos.Y, direction.X / 2f, direction.Y / 2f, ProjectileType<SpiderRay>(), damage, 0f, Main.myPlayer, npc.whoAmI, Main.myPlayer);
                    }
                }

                if (lasertimer == 100)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 pos = npc.Center;
                    int damage = 20;
                    if (Main.LocalPlayer.ownedProjectileCounts[ProjectileType<SpiderRay2>()] < 1)
                    {
                        Projectile.NewProjectile(pos.X, pos.Y, direction.X / 2f, direction.Y / 2f, ProjectileType<SpiderRay2>(), damage, 0f, Main.myPlayer, npc.whoAmI, Main.myPlayer);
                    }
                }

                if (lasertimer == 150)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 pos = npc.Center;
                    int damage = 20;
                    if (Main.LocalPlayer.ownedProjectileCounts[ProjectileType<SpiderRay3>()] < 1)
                    {
                        Projectile.NewProjectile(pos.X, pos.Y, direction.X / 2f, direction.Y / 2f, ProjectileType<SpiderRay3>(), damage, 0f, Main.myPlayer, npc.whoAmI, Main.myPlayer);
                    }
                }

                if (lasertimer == 200 && expert)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 pos = npc.Center;
                    int damage = 20;
                    if (Main.LocalPlayer.ownedProjectileCounts[ProjectileType<SpiderRay4>()] < 1)
                    {
                        Projectile.NewProjectile(pos.X, pos.Y, direction.X / 2f, direction.Y / 2f, ProjectileType<SpiderRay4>(), damage, 0f, Main.myPlayer, npc.whoAmI, Main.myPlayer);
                    }
                }

                if (lasertimer == 250 && expert)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 pos = npc.Center;
                    int damage = 20;
                    if (Main.LocalPlayer.ownedProjectileCounts[ProjectileType<SpiderRay5>()] < 1)
                    {
                        Projectile.NewProjectile(pos.X, pos.Y, direction.X / 2f, direction.Y / 2f, ProjectileType<SpiderRay5>(), damage, 0f, Main.myPlayer, npc.whoAmI, Main.myPlayer);
                    }
                }

                if (Main.player[npc.target].position.X + RandomSpiderTarget < npc.position.X && lasertimer > 1)
                {
                    npc.velocity.X -= 0.2f;
                    RandomSpiderTarget = Main.rand.Next(-100, 101);
                }
                if (Main.player[npc.target].position.X + RandomSpiderTarget > npc.position.X && lasertimer > 1)
                {
                    npc.velocity.X += 0.2f;
                    RandomSpiderTarget = Main.rand.Next(-100, 101);
                }
                if (Main.player[npc.target].position.Y + RandomSpiderTarget < npc.position.Y && lasertimer > 1)
                {
                    npc.velocity.Y -= 0.1f;
                    RandomSpiderTarget = Main.rand.Next(-100, 101);
                }
                if (Main.player[npc.target].position.Y + RandomSpiderTarget > npc.position.Y && lasertimer > 1)
                {
                    npc.velocity.Y += 0.1f;
                    RandomSpiderTarget = Main.rand.Next(-100, 101);
                }

                if (npc.velocity.Y > 6f && lasertimer > 1)
                {
                    npc.velocity.Y = 6f;
                }

                if (npc.velocity.Y < -6f && lasertimer > 1)
                {
                    npc.velocity.Y = -6f;
                }

                if (npc.velocity.X > 6f && lasertimer > 1)
                {
                    npc.velocity.X = 6f;
                }

                if (npc.velocity.X < -6f && lasertimer > 1)
                {
                    npc.velocity.X = -6f;
                }



                if (lasertimer == 600)
                {
                    RandomAtt = Main.rand.Next(0, 7);
                    lasertimer = 0;
                }
            }

            if (RandomAtt == 4)
            {

                if (spritetimer > 3 && eggtimer > 0)
                {
                    frame++;
                    spritetimer = 0;
                }
                if (frame > 8 && eggtimer > 0)
                {
                    frame = 4;
                }
                if (Main.player[npc.target].position.X < npc.position.X && eggtimer > 0)
                {
                    npc.spriteDirection = -1;
                }

                if (Main.player[npc.target].position.X > npc.position.X && eggtimer > 0)
                {
                    npc.spriteDirection = 1;
                }

                eggtimer++;
                if (eggtimer > 1)
                {
                    npc.noTileCollide = false;
                    npc.noGravity = false;
                    npc.aiStyle = 3;
                    aiType = NPCID.CrimsonPenguin;
                }

                if (eggtimer == 1)
                {
                    npc.position.Y = Main.player[npc.target].Center.Y - 100;
                }
                if (eggtimer == 120)
                {
                    npc.defense = 13;
                    NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y + 30, mod.NPCType("CreeperEgg"));
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/NPC/Blorch"), npc.position);
                }
                if (eggtimer == 240)
                {
                    NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y + 30, mod.NPCType("CreeperEgg"));
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/NPC/Blorch"), npc.position);
                }
                if (eggtimer == 360)
                {
                    NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y + 30, mod.NPCType("CreeperEgg"));
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/NPC/Blorch"), npc.position);
                }
                if (eggtimer == 480)
                {
                    NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y + 30, mod.NPCType("CreeperEgg"));
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/NPC/Blorch"), npc.position);
                }
                if (eggtimer == 540)
                {
                    NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y + 30, mod.NPCType("CreeperEgg"));
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/NPC/Blorch"), npc.position);
                }


                if (eggtimer == 600)
                {
                    NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y + 30, mod.NPCType("MushroomEgg"));
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/NPC/Blorch"), npc.position);
                }

                if (eggtimer == 630 && expert)
                {
                    NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y + 30, mod.NPCType("MushroomEgg"));
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/NPC/Blorch"), npc.position);
                }
                if (eggtimer == 660 && expert)
                {
                    NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y + 30, mod.NPCType("MushroomEgg"));
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/NPC/Blorch"), npc.position);
                }

                if (eggtimer == 720)
                {
                    npc.noTileCollide = true;
                    npc.noGravity = true;
                    npc.aiStyle = 0;
                    aiType = 0;
                    RandomAtt = Main.rand.Next(0, 7);
                    eggtimer = 0;
                    eggcooldown = 10;
                    npc.defense = 7;
                }
            }

            if (RandomAtt == 5)
            {
                RandomSpiVelo2 = Main.rand.Next(-4, 5);
                stickywebtimer++;
                if (stickywebtimer == 40)
                {
                    float positionX = npc.Center.X;
                    float positionY = npc.Center.Y;
                    float targetPositionX = Main.player[npc.target].Center.X;
                    float targetPositionY = Main.player[npc.target].Center.Y;
                    float directionX = targetPositionX - positionX;
                    float directionY = targetPositionY - positionY;
                    float speed = 0.04f;
                    int type = mod.ProjectileType("StickyWeb");
                    int damage = 28;
                    Projectile.NewProjectile(positionX, positionY, directionX * speed, directionY * speed, type, damage, 0f, Main.myPlayer);
                    RandomSpiVelo2 = Main.rand.Next(-2, 3);
                    Main.PlaySound(SoundID.Item5, npc.position);
                }

                if (stickywebtimer == 100)
                {
                    float positionX = npc.Center.X;
                    float positionY = npc.Center.Y;
                    float targetPositionX = Main.player[npc.target].Center.X;
                    float targetPositionY = Main.player[npc.target].Center.Y;
                    float directionX = targetPositionX - positionX;
                    float directionY = targetPositionY - positionY;
                    float speed = 0.04f;
                    int type = mod.ProjectileType("StickyWeb");
                    int damage = 28;
                    Projectile.NewProjectile(positionX, positionY, directionX * speed, directionY * speed, type, damage, 0f, Main.myPlayer);
                    RandomSpiVelo2 = Main.rand.Next(-2, 3);
                    Main.PlaySound(SoundID.Item5, npc.position);
                }
                if (stickywebtimer == 110)
                {
                    float positionX = npc.Center.X;
                    float positionY = npc.Center.Y;
                    float targetPositionX = Main.player[npc.target].Center.X;
                    float targetPositionY = Main.player[npc.target].Center.Y;
                    float directionX = targetPositionX - positionX;
                    float directionY = targetPositionY - positionY;
                    float speed = 0.04f;
                    int type = mod.ProjectileType("StickyWeb");
                    int damage = 28;
                    Projectile.NewProjectile(positionX, positionY, directionX * speed, directionY * speed, type, damage, 0f, Main.myPlayer);
                    RandomSpiVelo2 = Main.rand.Next(-2, 3);
                    Main.PlaySound(SoundID.Item5, npc.position);
                }


                if (stickywebtimer == 130 && expert)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    Vector2 newpoint1 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(14));
                    Vector2 newpoint2 = new Vector2(direction.X / 42f, direction.Y / 42f).RotatedBy(MathHelper.ToRadians(-14));
                    float speed = 0.04f;
                    int type = mod.ProjectileType("StickyWeb");
                    int damage = 40;
                    Projectile.NewProjectile(position, newpoint1 * speed, type, damage, 0f, Main.myPlayer);
                    Projectile.NewProjectile(position, newpoint2 * speed, type, damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item5, npc.position);
                }

                if (stickywebtimer == 135 && expert)
                {
                    float positionX = npc.Center.X;
                    float positionY = npc.Center.Y;
                    float targetPositionX = Main.player[npc.target].Center.X;
                    float targetPositionY = Main.player[npc.target].Center.Y;
                    float directionX = targetPositionX - positionX;
                    float directionY = targetPositionY - positionY;
                    float speed = 0.04f;
                    int type = mod.ProjectileType("StickyWeb");
                    int damage = 28;
                    Projectile.NewProjectile(positionX, positionY, directionX * speed, directionY * speed, type, damage, 0f, Main.myPlayer);
                    RandomSpiVelo2 = Main.rand.Next(-2, 3);
                    Main.PlaySound(SoundID.Item5, npc.position);
                }

                if (stickywebtimer == 140 && expert)
                {
                    float positionX = npc.Center.X;
                    float positionY = npc.Center.Y;
                    float targetPositionX = Main.player[npc.target].Center.X;
                    float targetPositionY = Main.player[npc.target].Center.Y;
                    float directionX = targetPositionX - positionX;
                    float directionY = targetPositionY - positionY;
                    float speed = 0.04f;
                    int type = mod.ProjectileType("StickyWeb");
                    int damage = 28;
                    Projectile.NewProjectile(positionX, positionY, directionX * speed, directionY * speed, type, damage, 0f, Main.myPlayer);
                    RandomSpiVelo2 = Main.rand.Next(-2, 3);
                    Main.PlaySound(SoundID.Item5, npc.position);
                }


                if (stickywebtimer == 160)
                {
                    float positionX = npc.Center.X;
                    float positionY = npc.Center.Y;
                    float targetPositionX = Main.player[npc.target].Center.X;
                    float targetPositionY = Main.player[npc.target].Center.Y;
                    float directionX = targetPositionX - positionX;
                    float directionY = targetPositionY - positionY;
                    float speed = 0.04f;
                    int type = mod.ProjectileType("StickyWeb");
                    int damage = 28;
                    Projectile.NewProjectile(positionX, positionY, directionX * speed, directionY * speed, type, damage, 0f, Main.myPlayer);
                    RandomSpiVelo2 = Main.rand.Next(-2, 3);
                    Main.PlaySound(SoundID.Item5, npc.position);
                }
                if (stickywebtimer == 170)
                {
                    float positionX = npc.Center.X;
                    float positionY = npc.Center.Y;
                    float targetPositionX = Main.player[npc.target].Center.X;
                    float targetPositionY = Main.player[npc.target].Center.Y;
                    float directionX = targetPositionX - positionX;
                    float directionY = targetPositionY - positionY;
                    float speed = 0.04f;
                    int type = mod.ProjectileType("StickyWeb");
                    int damage = 28;
                    Projectile.NewProjectile(positionX, positionY, directionX * speed, directionY * speed, type, damage, 0f, Main.myPlayer);
                    RandomSpiVelo2 = Main.rand.Next(-2, 3);
                    Main.PlaySound(SoundID.Item5, npc.position);
                }
                if (stickywebtimer == 180)
                {
                    float positionX = npc.Center.X;
                    float positionY = npc.Center.Y;
                    float targetPositionX = Main.player[npc.target].Center.X;
                    float targetPositionY = Main.player[npc.target].Center.Y;
                    float directionX = targetPositionX - positionX;
                    float directionY = targetPositionY - positionY;
                    float speed = 0.04f;
                    int type = mod.ProjectileType("StickyWeb");
                    int damage = 28;
                    Projectile.NewProjectile(positionX, positionY, directionX * speed, directionY * speed, type, damage, 0f, Main.myPlayer);
                    RandomSpiVelo2 = Main.rand.Next(-2, 3);
                    Main.PlaySound(SoundID.Item5, npc.position);
                }


                if (stickywebtimer == 200 && expert)
                {
                    float positionX = npc.Center.X;
                    float positionY = npc.Center.Y;
                    float targetPositionX = Main.player[npc.target].Center.X;
                    float targetPositionY = Main.player[npc.target].Center.Y;
                    float directionX = targetPositionX - positionX;
                    float directionY = targetPositionY - positionY;
                    float speed = 0.04f;
                    int type = mod.ProjectileType("StickyWeb");
                    int damage = 28;
                    Projectile.NewProjectile(positionX, positionY, directionX * speed, directionY * speed, type, damage, 0f, Main.myPlayer);
                    RandomSpiVelo2 = Main.rand.Next(-2, 3);
                    Main.PlaySound(SoundID.Item5, npc.position);
                }

                if (stickywebtimer == 230)
                {
                    float positionX = npc.Center.X;
                    float positionY = npc.Center.Y;
                    float targetPositionX = Main.player[npc.target].Center.X;
                    float targetPositionY = Main.player[npc.target].Center.Y;
                    float directionX = targetPositionX - positionX;
                    float directionY = targetPositionY - positionY;
                    float speed = 0.04f;
                    int type = mod.ProjectileType("StickyWeb");
                    int damage = 28;
                    Projectile.NewProjectile(positionX, positionY, directionX * speed, directionY * speed, type, damage, 0f, Main.myPlayer);
                    RandomSpiVelo2 = Main.rand.Next(-2, 3);
                    Main.PlaySound(SoundID.Item5, npc.position);
                }
                if (stickywebtimer == 235)
                {
                    float positionX = npc.Center.X;
                    float positionY = npc.Center.Y;
                    float targetPositionX = Main.player[npc.target].Center.X;
                    float targetPositionY = Main.player[npc.target].Center.Y;
                    float directionX = targetPositionX - positionX;
                    float directionY = targetPositionY - positionY;
                    float speed = 0.04f;
                    int type = mod.ProjectileType("StickyWeb");
                    int damage = 28;
                    Projectile.NewProjectile(positionX, positionY, directionX * speed, directionY * speed, type, damage, 0f, Main.myPlayer);
                    RandomSpiVelo2 = Main.rand.Next(-2, 3);
                    Main.PlaySound(SoundID.Item5, npc.position);
                }

                if (stickywebtimer == 265)
                {
                    float positionX = npc.Center.X;
                    float positionY = npc.Center.Y;
                    float targetPositionX = Main.player[npc.target].Center.X;
                    float targetPositionY = Main.player[npc.target].Center.Y;
                    float directionX = targetPositionX - positionX;
                    float directionY = targetPositionY - positionY;
                    float speed = 0.04f;
                    int type = mod.ProjectileType("StickyWeb");
                    int damage = 28;
                    Projectile.NewProjectile(positionX, positionY, directionX * speed, directionY * speed, type, damage, 0f, Main.myPlayer);
                    RandomSpiVelo2 = Main.rand.Next(-2, 3);
                    Main.PlaySound(SoundID.Item5, npc.position);
                }
                if (stickywebtimer == 275)
                {
                    float positionX = npc.Center.X;
                    float positionY = npc.Center.Y;
                    float targetPositionX = Main.player[npc.target].Center.X;
                    float targetPositionY = Main.player[npc.target].Center.Y;
                    float directionX = targetPositionX - positionX;
                    float directionY = targetPositionY - positionY;
                    float speed = 0.04f;
                    int type = mod.ProjectileType("StickyWeb");
                    int damage = 28;
                    Projectile.NewProjectile(positionX, positionY, directionX * speed, directionY * speed, type, damage, 0f, Main.myPlayer);
                    RandomSpiVelo2 = Main.rand.Next(-2, 3);
                    Main.PlaySound(SoundID.Item5, npc.position);
                }


                if (stickywebtimer == 300 && expert)
                {
                    float positionX = npc.Center.X;
                    float positionY = npc.Center.Y;
                    float targetPositionX = Main.player[npc.target].Center.X;
                    float targetPositionY = Main.player[npc.target].Center.Y;
                    float directionX = targetPositionX - positionX;
                    float directionY = targetPositionY - positionY;
                    float speed = 0.04f;
                    int type = mod.ProjectileType("StickyWeb");
                    int damage = 28;
                    Projectile.NewProjectile(positionX, positionY, directionX * speed, directionY * speed, type, damage, 0f, Main.myPlayer);
                    RandomSpiVelo2 = Main.rand.Next(-2, 3);
                    Main.PlaySound(SoundID.Item5, npc.position);
                }


                if (stickywebtimer == 340)
                {
                    float positionX = npc.Center.X;
                    float positionY = npc.Center.Y;
                    float targetPositionX = Main.player[npc.target].Center.X;
                    float targetPositionY = Main.player[npc.target].Center.Y;
                    float directionX = targetPositionX - positionX;
                    float directionY = targetPositionY - positionY;
                    float speed = 0.04f;
                    int type = mod.ProjectileType("StickyWeb");
                    int damage = 28;
                    Projectile.NewProjectile(positionX, positionY, directionX * speed, directionY * speed, type, damage, 0f, Main.myPlayer);
                    RandomSpiVelo2 = Main.rand.Next(-2, 3);
                    Main.PlaySound(SoundID.Item5, npc.position);
                }

                if (stickywebtimer == 350 && expert)
                {
                    float positionX = npc.Center.X;
                    float positionY = npc.Center.Y;
                    float targetPositionX = Main.player[npc.target].Center.X;
                    float targetPositionY = Main.player[npc.target].Center.Y;
                    float directionX = targetPositionX - positionX;
                    float directionY = targetPositionY - positionY;
                    float speed = 0.04f;
                    int type = mod.ProjectileType("StickyWeb");
                    int damage = 28;
                    Projectile.NewProjectile(positionX, positionY, directionX * speed, directionY * speed, type, damage, 0f, Main.myPlayer);
                    RandomSpiVelo2 = Main.rand.Next(-2, 3);
                    Main.PlaySound(SoundID.Item5, npc.position);
                }


                if (stickywebtimer == 360 && expert)
                {
                    float positionX = npc.Center.X;
                    float positionY = npc.Center.Y;
                    float targetPositionX = Main.player[npc.target].Center.X;
                    float targetPositionY = Main.player[npc.target].Center.Y;
                    float directionX = targetPositionX - positionX;
                    float directionY = targetPositionY - positionY;
                    float speed = 0.04f;
                    int type = mod.ProjectileType("StickyWeb");
                    int damage = 28;
                    Projectile.NewProjectile(positionX, positionY, directionX * speed, directionY * speed, type, damage, 0f, Main.myPlayer);
                    RandomSpiVelo2 = Main.rand.Next(-2, 3);
                    Main.PlaySound(SoundID.Item5, npc.position);
                }

                if (stickywebtimer == 370 && expert)
                {
                    float positionX = npc.Center.X;
                    float positionY = npc.Center.Y;
                    float targetPositionX = Main.player[npc.target].Center.X;
                    float targetPositionY = Main.player[npc.target].Center.Y;
                    float directionX = targetPositionX - positionX;
                    float directionY = targetPositionY - positionY;
                    float speed = 0.04f;
                    int type = mod.ProjectileType("StickyWeb");
                    int damage = 28;
                    Projectile.NewProjectile(positionX, positionY, directionX * speed, directionY * speed, type, damage, 0f, Main.myPlayer);
                    RandomSpiVelo2 = Main.rand.Next(-2, 3);
                    Main.PlaySound(SoundID.Item5, npc.position);
                }


                if (stickywebtimer == 380 && expert)
                {
                    float positionX = npc.Center.X;
                    float positionY = npc.Center.Y;
                    float targetPositionX = Main.player[npc.target].Center.X;
                    float targetPositionY = Main.player[npc.target].Center.Y;
                    float directionX = targetPositionX - positionX;
                    float directionY = targetPositionY - positionY;
                    float speed = 0.04f;
                    int type = mod.ProjectileType("StickyWeb");
                    int damage = 28;
                    Projectile.NewProjectile(positionX, positionY, directionX * speed, directionY * speed, type, damage, 0f, Main.myPlayer);
                    RandomSpiVelo2 = Main.rand.Next(-2, 3);
                    Main.PlaySound(SoundID.Item5, npc.position);
                }


                if (stickywebtimer == 390 && expert)
                {
                    float positionX = npc.Center.X;
                    float positionY = npc.Center.Y;
                    float targetPositionX = Main.player[npc.target].Center.X;
                    float targetPositionY = Main.player[npc.target].Center.Y;
                    float directionX = targetPositionX - positionX;
                    float directionY = targetPositionY - positionY;
                    float speed = 0.04f;
                    int type = mod.ProjectileType("StickyWeb");
                    int damage = 28;
                    Projectile.NewProjectile(positionX, positionY, directionX * speed, directionY * speed, type, damage, 0f, Main.myPlayer);
                    RandomSpiVelo2 = Main.rand.Next(-2, 3);
                    Main.PlaySound(SoundID.Item5, npc.position);
                }

                if (stickywebtimer == 430)
                {
                    float positionX = npc.Center.X;
                    float positionY = npc.Center.Y;
                    float targetPositionX = Main.player[npc.target].Center.X;
                    float targetPositionY = Main.player[npc.target].Center.Y;
                    float directionX = targetPositionX - positionX;
                    float directionY = targetPositionY - positionY;
                    float speed = 0.04f;
                    int type = mod.ProjectileType("StickyWeb");
                    int damage = 28;
                    Projectile.NewProjectile(positionX, positionY, directionX * speed, directionY * speed, type, damage, 0f, Main.myPlayer);
                    RandomSpiVelo2 = Main.rand.Next(-2, 3);
                    Main.PlaySound(SoundID.Item5, npc.position);
                }
                if (stickywebtimer == 450)
                {
                    float positionX = npc.Center.X;
                    float positionY = npc.Center.Y;
                    float targetPositionX = Main.player[npc.target].Center.X;
                    float targetPositionY = Main.player[npc.target].Center.Y;
                    float directionX = targetPositionX - positionX;
                    float directionY = targetPositionY - positionY;
                    float speed = 0.04f;
                    int type = mod.ProjectileType("StickyWeb");
                    int damage = 28;
                    Projectile.NewProjectile(positionX, positionY, directionX * speed, directionY * speed, type, damage, 0f, Main.myPlayer);
                    RandomSpiVelo2 = Main.rand.Next(-2, 3);
                    Main.PlaySound(SoundID.Item5, npc.position);
                }
                if (stickywebtimer == 460)
                {
                    float positionX = npc.Center.X;
                    float positionY = npc.Center.Y;
                    float targetPositionX = Main.player[npc.target].Center.X;
                    float targetPositionY = Main.player[npc.target].Center.Y;
                    float directionX = targetPositionX - positionX;
                    float directionY = targetPositionY - positionY;
                    float speed = 0.04f;
                    int type = mod.ProjectileType("StickyWeb");
                    int damage = 28;
                    Projectile.NewProjectile(positionX, positionY, directionX * speed, directionY * speed, type, damage, 0f, Main.myPlayer);
                    RandomSpiVelo2 = Main.rand.Next(-4, 5);
                    Main.PlaySound(SoundID.Item5, npc.position);
                }


                if (stickywebtimer == 720)
                {
                    RandomAtt = Main.rand.Next(0, 7);
                    stickywebtimer = 0;
                    webcooldown = 10;
                }
            }


            if (RandomAtt == 6)
            {
                webround++;
                if (webround == 10)
                {
                    Vector2 value1 = new Vector2(0f, 0f);
                    Projectile.NewProjectile(npc.Center.X - 222, npc.Center.Y - 222, value1.X + 3f, value1.Y + 3f, mod.ProjectileType("StickyWeb2"), npc.damage - 10, 1f);
                }

                if (webround == 20)
                {
                    Vector2 value1 = new Vector2(0f, 0f);
                    Projectile.NewProjectile(npc.Center.X + 222, npc.Center.Y - 222, value1.X - 3f, value1.Y + 3f, mod.ProjectileType("StickyWeb2"), npc.damage - 10, 1f);
                }

                if (webround == 30)
                {
                    Vector2 value1 = new Vector2(0f, 0f);
                    Projectile.NewProjectile(npc.Center.X - 222, npc.Center.Y + 222, value1.X + 3f, value1.Y - 3f, mod.ProjectileType("StickyWeb2"), npc.damage - 10, 1f);
                }

                if (webround == 40)
                {
                    Vector2 value1 = new Vector2(0f, 0f);
                    Projectile.NewProjectile(npc.Center.X + 222, npc.Center.Y + 222, value1.X - 3f, value1.Y - 3f, mod.ProjectileType("StickyWeb2"), npc.damage - 10, 1f);
                }

                if (webround == 50)
                {
                    Vector2 value1 = new Vector2(0f, 0f);
                    Projectile.NewProjectile(npc.Center.X - 222, npc.Center.Y, value1.X + 3f, value1.Y, mod.ProjectileType("StickyWeb2"), npc.damage - 10, 1f);
                }

                if (webround == 60)
                {
                    Vector2 value1 = new Vector2(0f, 0f);
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y - 222, value1.X, value1.Y + 3, mod.ProjectileType("StickyWeb2"), npc.damage - 10, 1f);
                }

                if (webround == 70)
                {
                    Vector2 value1 = new Vector2(0f, 0f);
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y + 222, value1.X, value1.Y - 3, mod.ProjectileType("StickyWeb2"), npc.damage - 10, 1f);
                }

                if (webround == 80)
                {
                    Vector2 value1 = new Vector2(0f, 0f);
                    Projectile.NewProjectile(npc.Center.X + 222, npc.Center.Y, value1.X - 3f, value1.Y, mod.ProjectileType("StickyWeb2"), npc.damage - 10, 1f);
                }


                if (webround == 140)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 3f * expertfire;
                    int type = 472;
                    int damage = 40;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                }

                if (webround == 200)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 4f * expertfire;
                    int type = 472;
                    int damage = 40;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                }

                if (webround == 280)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 5f * expertfire;
                    int type = 472;
                    int damage = 40;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                }

                if (webround == 340)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 6f * expertfire;
                    int type = 472;
                    int damage = 40;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                }

                if (webround == 400)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 7f * expertfire;
                    int type = 472;
                    int damage = 40;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                }


                if (webround == 450 && expert)
                {
                    RandomSpiVelo = Main.rand.Next(0, 2);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 10f * expertfire;
                    int damage = 40;
                    Vector2 newpoint1 = new Vector2(direction.X, direction.Y).RotatedBy(MathHelper.ToRadians(10));
                    Vector2 newpoint2 = new Vector2(direction.X, direction.Y).RotatedBy(MathHelper.ToRadians(-10));
                    Projectile.NewProjectile(position, direction * speed, ProjectileID.WebSpit, damage, 0f, Main.myPlayer);
                    Projectile.NewProjectile(position, newpoint1 * speed, ProjectileID.WebSpit, damage, 0f, Main.myPlayer);
                    Projectile.NewProjectile(position, newpoint2 * speed, ProjectileID.WebSpit, damage, 0f, Main.myPlayer);
                }


                if (webround == 480)
                {
                    RandomAtt = Main.rand.Next(0, 7);
                    webround = 0;
                }
            }
        }



        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ashen Arachnid");
            Main.npcFrameCount[npc.type] = 9;
        }
    }
}
