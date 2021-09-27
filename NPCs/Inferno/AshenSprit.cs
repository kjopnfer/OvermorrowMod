using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.NPCs.Inferno
{
    class AshenSprit : ModNPC
    {

        readonly bool expert = Main.expertMode;
        private int experttimer = 0;
        private int wormtimer = 0;
        private int bombtimer = 0;
        private int arrowtimer = 0;
        private int axetimer = 0;
        private int targettimer = 0;
        private int pandoratimer = 0;
        private int zerotimer = 0;
        private int pufftimer = 0;
        int spritetimer = 0;
        private int look = 0;
        int frame = 1;
        int RandomAtt = Main.rand.Next(-2, 5);
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 6;
            DisplayName.SetDefault("Ashen Soul");
        }
        public override void SetDefaults()
        {
            npc.width = 30;
            npc.height = 60;
            npc.lifeMax = 200;
            npc.damage = 25;
            npc.defense = 5;
            npc.knockBackResist = 0.1f;
            npc.lavaImmune = true;
            npc.dontTakeDamage = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frame.Y = frameHeight * frame;
            npc.spriteDirection = look;
        }
        
        public override void AI()
        {

                experttimer++;
                if(expert && experttimer == 1)
                {
                    npc.damage = npc.damage / 2;
                }



            if(Main.player[npc.target].position.X < 24500)
            {
                npc.position.X = Main.player[npc.target].position.X;
                npc.position.Y = Main.player[npc.target].position.Y + 200;
                npc.alpha = 255;
                pufftimer = 0;
            }
            else
            {
            npc.alpha = 75;
            pufftimer++;

            if(RandomAtt < 4 || RandomAtt > 4)
            {
                npc.position.X = Main.player[npc.target].position.X;
                npc.position.Y = Main.player[npc.target].position.Y - 200;
            }

            if(npc.alpha == 75 && pufftimer == 3)
            {
                Vector2 position = npc.Center;
                Main.PlaySound(SoundID.Item8, (int)position.X, (int)position.Y);
                int radius = 8;     //this is the explosion radius, the highter is the value the bigger is the explosion

                    for (int x = -radius; x <= radius; x++)
                    {
                        for (int y = -radius; y <= radius; y++)
                        {

                            if (Math.Sqrt(x * x + y * y) <= radius + 0.5)   //this make so the explosion radius is a circle
                            {
                                Dust.NewDust(position, 5, 5, DustID.Pixie, 0.0f, 0.0f, 120, new Color(), 1f);  //this is the dust that will spawn after the explosion
                            }
                        }
                    }
            }
            int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Fire, 0.0f, 0.0f, 300, new Color(), 1.1f);
            if(RandomAtt == -2)
            {
                targettimer++;
                spritetimer++;
                if(spritetimer > 4)
                {
                    frame++;
                    spritetimer = 0;
                }
                if(frame > 2 && targettimer < 201)
                {
                    frame = 0;
                }

                if(frame == 6 && targettimer > 201)
                {
                    frame = 3;
                }

                if(targettimer == 1)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 0f;
                    npc.velocity.X = 0f;
                    int type = mod.ProjectileType("TargetWarning");
                    int damage = npc.damage + 7;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                    CombatText.NewText(npc.getRect(), Color.Orange, "Death From Above");
                }

                if(targettimer == 201)
                {
                    Vector2 value1 = new Vector2(0f, 0f);
                    Projectile.NewProjectile(npc.Center.X + Main.rand.Next(-600, 601), Main.player[npc.target].Center.Y, value1.X, value1.Y, mod.ProjectileType("ExplosiveTarget"), npc.damage, 1f);
                    Projectile.NewProjectile(npc.Center.X + Main.rand.Next(-600, 601), Main.player[npc.target].Center.Y, value1.X, value1.Y, mod.ProjectileType("ExplosiveTarget"), npc.damage, 1f);
                    Projectile.NewProjectile(npc.Center.X + Main.rand.Next(-600, 601), Main.player[npc.target].Center.Y, value1.X, value1.Y, mod.ProjectileType("ExplosiveTarget"), npc.damage, 1f);
                    Projectile.NewProjectile(npc.Center.X + Main.rand.Next(-600, 601), Main.player[npc.target].Center.Y, value1.X, value1.Y, mod.ProjectileType("ExplosiveTarget"), npc.damage, 1f);
                    Projectile.NewProjectile(npc.Center.X + Main.rand.Next(-600, 601), Main.player[npc.target].Center.Y, value1.X, value1.Y, mod.ProjectileType("ExplosiveTarget"), npc.damage, 1f);
                    Projectile.NewProjectile(npc.Center.X + Main.rand.Next(-600, 601), Main.player[npc.target].Center.Y, value1.X, value1.Y, mod.ProjectileType("ExplosiveTarget"), npc.damage, 1f);
                    Projectile.NewProjectile(npc.Center.X + Main.rand.Next(-600, 601), Main.player[npc.target].Center.Y, value1.X, value1.Y, mod.ProjectileType("ExplosiveTarget"), npc.damage, 1f);
                    Projectile.NewProjectile(npc.Center.X, Main.player[npc.target].Center.Y, value1.X, value1.Y, mod.ProjectileType("ExplosiveTarget"), npc.damage, 1f);
                }

                if(targettimer == 300)
                {
                    RandomAtt = Main.rand.Next(-1, 5);
                    targettimer = 0;
                }
            
            }

            if(RandomAtt == -1)
            {
                pandoratimer++;
                spritetimer++;
                if(spritetimer > 4)
                {
                    frame++;
                    spritetimer = 0;
                }
                if(frame > 2 && pandoratimer < 201)
                {
                    frame = 0;
                }

                if(frame == 6 && pandoratimer > 201)
                {
                    frame = 3;
                }

                if(pandoratimer == 1)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 0f;
                    npc.velocity.X = 0f;
                    int type = mod.ProjectileType("PandorasWarning");
                    int damage = npc.damage + 7;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                    CombatText.NewText(npc.getRect(), Color.Orange, "Pandora's Box");
                }

                if(pandoratimer == 201)
                {
                    Vector2 value1 = new Vector2(0f, 0f);
                    Projectile.NewProjectile(npc.Center.X - 322, Main.player[npc.target].Center.Y, value1.X, value1.Y + 3f, mod.ProjectileType("PandorasBox"), npc.damage, 1f);
                    Projectile.NewProjectile(npc.Center.X + 322, Main.player[npc.target].Center.Y, value1.X, value1.Y + 3f, mod.ProjectileType("PandorasBox"), npc.damage, 1f);
                }

                if(pandoratimer == 300)
                {
                    RandomAtt = Main.rand.Next(-2, 5);
                    pandoratimer = 0;
                }
            }
            if(RandomAtt == 0)
            {
                zerotimer++;
                spritetimer++;
                if(spritetimer > 4)
                {
                    frame++;
                    spritetimer = 0;
                }
                if(frame > 2 && zerotimer < 201)
                {
                    frame = 0;
                }

                if(frame == 6 && zerotimer > 201)
                {
                    frame = 3;
                }
                if(zerotimer == 1)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 0f;
                    npc.velocity.X = 0f;
                    int type = mod.ProjectileType("HolyWarning");
                    int damage = npc.damage + 7;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                    CombatText.NewText(npc.getRect(), Color.Orange, "Igneous Boulder");
                }
                if(zerotimer == 200)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 0f;
                    npc.velocity.X = 0f;
                    int type = mod.ProjectileType("FlamingLight");
                    int damage = npc.damage + 7;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                }

                if(zerotimer == 240)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 0f;
                    npc.velocity.X = 0f;
                    int type = mod.ProjectileType("FlamingLight");
                    int damage = npc.damage + 7;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                }

                if(zerotimer == 280)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 0f;
                    npc.velocity.X = 0f;
                    int type = mod.ProjectileType("FlamingLight");
                    int damage = npc.damage + 7;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                }

                if(zerotimer == 400)
                {
                    RandomAtt = Main.rand.Next(-2, 5);
                    zerotimer = 0;
                }
            }
            if(RandomAtt == 1)
            {
                wormtimer++;
                spritetimer++;
                if(spritetimer > 4)
                {
                    frame++;
                    spritetimer = 0;
                }
                if(frame > 2 && wormtimer < 201)
                {
                    frame = 0;
                }

                if(frame == 6 && wormtimer > 201)
                {
                    frame = 3;
                }

                if(wormtimer == 1)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 0f;
                    npc.velocity.X = 0f;
                    int type = mod.ProjectileType("WormWarning");
                    int damage = npc.damage + 7;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                    CombatText.NewText(npc.getRect(), Color.Orange, "Mysterious Creatures");
                }

                if(wormtimer == 201)
                {
                    NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y - 40, mod.NPCType("AshenSeeker"), 0, npc.whoAmI, npc.Center.X, npc.Center.Y, 0.0f, byte.MaxValue);
                    NPC.NewNPC((int)npc.Center.X - 40, (int)npc.Center.Y, mod.NPCType("ServantHead"));
                    NPC.NewNPC((int)npc.Center.X + 40, (int)npc.Center.Y, mod.NPCType("ServantHead"));
                }

                if(wormtimer == 300)
                {
                    RandomAtt = Main.rand.Next(-2, 5);
                    wormtimer = 0;
                }
            
            }
            if(RandomAtt == 2)
            {
                bombtimer++;
                spritetimer++;
                if(spritetimer > 4)
                {
                    frame++;
                    spritetimer = 0;
                }
                if(frame > 2 && bombtimer < 201)
                {
                    frame = 0;
                }

                if(frame == 6 && bombtimer > 201)
                {
                    frame = 3;
                }
                if(bombtimer == 1)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 0f;
                    npc.velocity.X = 0f;
                    int type = mod.ProjectileType("WaveWarning");
                    int damage = npc.damage + 7;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                    CombatText.NewText(npc.getRect(), Color.Orange, "Fire Bolts");
                }

                if(bombtimer == 201)
                {
                    Vector2 value1 = new Vector2(0f, 0f);
                    Projectile.NewProjectile(Main.player[npc.target].Center.X - 222, Main.player[npc.target].Center.Y - 222, value1.X + 3f, value1.Y + 3f, mod.ProjectileType("Firewave"), npc.damage - 10, 1f);
                    Projectile.NewProjectile(Main.player[npc.target].Center.X + 222, Main.player[npc.target].Center.Y - 222, value1.X - 3f, value1.Y + 3f, mod.ProjectileType("Firewave"), npc.damage - 10, 1f);
                    Projectile.NewProjectile(Main.player[npc.target].Center.X - 222, Main.player[npc.target].Center.Y + 222, value1.X + 3f, value1.Y - 3f, mod.ProjectileType("Firewave"), npc.damage - 10, 1f);
                    Projectile.NewProjectile(Main.player[npc.target].Center.X + 222, Main.player[npc.target].Center.Y + 222, value1.X - 3f, value1.Y - 3f, mod.ProjectileType("Firewave"), npc.damage - 10, 1f);

                    Projectile.NewProjectile(Main.player[npc.target].Center.X - 222, Main.player[npc.target].Center.Y, value1.X + 3f, value1.Y, mod.ProjectileType("Firewave"), npc.damage - 10, 1f);
                    Projectile.NewProjectile(Main.player[npc.target].Center.X, Main.player[npc.target].Center.Y - 222, value1.X, value1.Y + 3, mod.ProjectileType("Firewave"), npc.damage - 10, 1f);
                    Projectile.NewProjectile(Main.player[npc.target].Center.X, Main.player[npc.target].Center.Y + 222, value1.X, value1.Y - 3, mod.ProjectileType("Firewave"), npc.damage - 10, 1f);
                    Projectile.NewProjectile(Main.player[npc.target].Center.X + 222, Main.player[npc.target].Center.Y, value1.X - 3f, value1.Y, mod.ProjectileType("Firewave"), npc.damage - 10, 1f);
                }

                if(bombtimer == 350)
                {
                    RandomAtt = Main.rand.Next(-2, 5);
                    bombtimer = 0;
                }
            }
            if(RandomAtt == 3)
            {
                arrowtimer++;
                spritetimer++;
                if(spritetimer > 4)
                {
                    frame++;
                    spritetimer = 0;
                }
                if(frame > 2 && arrowtimer < 201)
                {
                    frame = 0;
                }

                if(frame > 5 && arrowtimer > 201)
                {
                    frame = 3;
                }
                if(arrowtimer == 1)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 0f;
                    npc.velocity.X = 0f;
                    int type = mod.ProjectileType("BombWarning");
                    int damage = npc.damage + 7;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                    CombatText.NewText(npc.getRect(), Color.Orange, "Skull Boomer");
                }
                if(arrowtimer == 201)
                {
                    Vector2 value1 = new Vector2(0f, 0f);
                    Projectile.NewProjectile(Main.player[npc.target].Center.X - 820, Main.player[npc.target].Center.Y, value1.X + 13f, value1.Y + 0f, mod.ProjectileType("SidewaysBomb"), npc.damage + 20, 1f);
                }

                if(arrowtimer == 400)
                {
                    RandomAtt = Main.rand.Next(-2, 5);
                    arrowtimer = 0;
                }
            }
            if(RandomAtt == 4)
            {
                axetimer++;
                spritetimer++;
                if(spritetimer > 4)
                {
                    frame++;
                    spritetimer = 0;
                }
                if(frame > 2 && axetimer < 301)
                {
                    frame = 0;
                }

                if(frame > 5 && axetimer > 301)
                {
                    frame = 3;
                }
                if(axetimer == 1)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 0f;
                    npc.velocity.X = 0f;
                    int type = mod.ProjectileType("AxeWarning");
                    int damage = npc.damage + 7;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                    CombatText.NewText(npc.getRect(), Color.Orange, "Lava Laser");
                }

                if(axetimer < 351)
                {
                    npc.position.X = Main.player[npc.target].position.X;
                    npc.position.Y = Main.player[npc.target].position.Y - 200;
                }

                if(axetimer > 351)
                {

                    npc.TargetClosest(true);
                    if (Main.player[npc.target].position.X < npc.position.X)
                    {
                        npc.velocity.X -= 0.15f; // accelerate to the left
                    }
                    if (Main.player[npc.target].position.X > npc.position.X)
                    {
                        npc.velocity.X += 0.15f; // accelerate to the right
                    }
                    if (Main.player[npc.target].position.Y < npc.position.Y)
                    {
                        npc.velocity.Y -= 0.15f; // accelerate to the down
                    }
                    if (Main.player[npc.target].position.Y > npc.position.Y)
                    {
                        npc.velocity.Y += 0.15f; // accelerate to the up
                    }

                    if (npc.velocity.Y < -2.5f)
                    {
                        npc.velocity.Y = -2.5f;
                    }
                    if (npc.velocity.Y > 2.5f)
                    {
                        npc.velocity.Y = 2.5f;
                    }
                    if (npc.velocity.X < -6)
                    {
                        npc.velocity.X = -6;
                    }
                    if (npc.velocity.X > 6)
                    {
                        npc.velocity.X = 6;
                    }

                }

                if(axetimer == 351)
                {
                    Vector2 pos = npc.Center;
                    int damage = 30;
                    if (Main.LocalPlayer.ownedProjectileCounts[ProjectileType<FireRay>()] < 1)
                    {
                        Projectile.NewProjectile(pos.X, pos.Y, 0f, 0f, ProjectileType<FireRay>(), damage, 0f, Main.myPlayer, npc.whoAmI, Main.myPlayer);
                    }
                    if (Main.LocalPlayer.ownedProjectileCounts[ProjectileType<FireRay2>()] < 1)
                    {
                        Projectile.NewProjectile(pos.X, pos.Y, 0f, 0f, ProjectileType<FireRay2>(), damage, 0f, Main.myPlayer, npc.whoAmI, Main.myPlayer);
                    }
                }

                if(axetimer == 800)
                {
                    RandomAtt = Main.rand.Next(-2, 4);
            Vector2 position = npc.Center;
            Main.PlaySound(SoundID.Item8, (int)position.X, (int)position.Y);
            int radius = 8;     //this is the explosion radius, the highter is the value the bigger is the explosion

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {

                    if (Math.Sqrt(x * x + y * y) <= radius + 0.5)   //this make so the explosion radius is a circle
                    {
                        Dust.NewDust(position, 5, 5, DustID.Pixie, 0.0f, 0.0f, 120, new Color(), 1f);  //this is the dust that will spawn after the explosion
                    }
                }
            }
                    axetimer = 0;
                }
            }
            }
        }
    }
}

