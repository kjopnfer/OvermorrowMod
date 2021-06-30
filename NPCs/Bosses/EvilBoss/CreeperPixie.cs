using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Items.Materials;
using OvermorrowMod.Projectiles.NPCs.Hostile;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.EvilBoss
{
    public class CreeperPixie : ModNPC
    {
        readonly bool expert = Main.expertMode;
        int frame = 1;
        int spiderspritetimer = 0;
        int Rot = 0;
        public override void SetDefaults()
        {
            NPCID.Sets.TrailCacheLength[npc.type] = 17;
            NPCID.Sets.TrailingMode[npc.type] = 1;
            npc.width = 30;
            npc.height = 26;
            npc.damage = 30;
            npc.aiStyle = 0;
            npc.noTileCollide = true;
            npc.defense = 10;
            npc.lifeMax = 200;
            npc.alpha = 40;
            npc.alpha = 20;
            npc.noGravity = true;
            npc.lavaImmune = true;
        }
        int spiderexp = 0;
        
        public override void FindFrame(int frameHeight)
        {
            npc.frame.Y = frameHeight * frame;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.velocity.X = 6;
        }
        public override void AI()
        {

            spiderexp++;
            spiderspritetimer++;
            if(spiderspritetimer > 4)
            {
                frame++;
                spiderspritetimer = 0;
            }
            if(frame > 1)
            {
                frame = 0;
            }

            float Charge = Vector2.Distance(Main.player[npc.target].Center, npc.Center);

            if(spiderexp > 3)
            {
                if(Charge < 200)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = npc.Center + new Vector2(0, -40);
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed1 = 0.5f;
                    int damage = npc.damage;
                    npc.velocity.X = 0;
                    npc.velocity.Y = 0;
                    Vector2 perturbedSpeed1 = new Vector2(direction.X, direction.Y).RotatedBy(MathHelper.ToRadians(Rot));
                    Projectile.NewProjectile(position, perturbedSpeed1 * speed1, mod.ProjectileType("CreeperProj"), damage, 0f, Main.myPlayer);
                    Projectile.NewProjectile(position, -perturbedSpeed1 * speed1, mod.ProjectileType("CreeperProj"), damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item12, npc.position);
                    Rot += 10;
                    spiderexp = 0;
                }
            }

            if(Charge > 170)
            {
                int RandomAdd = Main.rand.Next(-1, 2);
                Vector2 position = npc.Center;
                Vector2 targetPosition = Main.player[npc.target].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                float speed = 3.3f + RandomAdd * 0.1f;
                npc.velocity += direction / speed;
                spiderexp = 0;
            }

            if(npc.velocity.X > 12)
            {
                npc.velocity.X = 12;
            }
            if(npc.velocity.X < -12)
            {
                npc.velocity.X = -12;
            }
            if(npc.velocity.Y > 12)
            {
                npc.velocity.Y = 12;
            }
            if(npc.velocity.Y < -12)
            {
                npc.velocity.Y = -12;
            }
        }

    

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 2;
            DisplayName.SetDefault("Creeper Pixie");
        }
    }
}

