using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Items.Materials;
using OvermorrowMod.Projectiles.NPCs.Hostile;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.EvilBoss
{
    public class PixieBomb : ModNPC
    {
        readonly bool expert = Main.expertMode;
        int frame = 1;
        int spiderspritetimer = 0;

        public override void SetDefaults()
        {
            NPCID.Sets.TrailCacheLength[npc.type] = 17;
            NPCID.Sets.TrailingMode[npc.type] = 1;
            npc.width = 30;
            npc.height = 26;
            npc.damage = 30;
            npc.aiStyle = 0;
            npc.noTileCollide = true;
            npc.defense = 3;
            npc.lifeMax = 175;
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

            if(spiderexp > 15)
            {

                if(Charge < 200)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    int speed1 = Main.rand.Next(3, 6);
                    int speed2 = Main.rand.Next(2, 5);
                    int speed3 = Main.rand.Next(2, 5);
                    int damage = npc.damage;
                    npc.velocity.X = 0;
                    npc.velocity.Y = 0;
                    Vector2 perturbedSpeed1 = new Vector2(direction.X, direction.Y).RotatedByRandom(MathHelper.ToRadians(35f));
                    Vector2 perturbedSpeed2 = new Vector2(direction.X, direction.Y).RotatedByRandom(MathHelper.ToRadians(35f));
                    Vector2 perturbedSpeed3 = new Vector2(direction.X, direction.Y).RotatedByRandom(MathHelper.ToRadians(35f));
                    Projectile.NewProjectile(position, perturbedSpeed1 * speed1, mod.ProjectileType("ShadowPixie"), damage, 0f, Main.myPlayer);
                    Projectile.NewProjectile(position, perturbedSpeed2 * speed2, mod.ProjectileType("ShadowPixie"), damage, 0f, Main.myPlayer);
                    Projectile.NewProjectile(position, perturbedSpeed3 * speed3, mod.ProjectileType("ShadowPixie"), damage, 0f, Main.myPlayer);
                    Main.PlaySound(SoundID.Item103, npc.position);
                    spiderexp = 0;
                }
            }

            if(Charge > 170)
            {
                int RandomAdd = Main.rand.Next(-1, 4);
                int RandomTarget2 = Main.rand.Next(-75, 76);
                int RandomTarget1 = Main.rand.Next(-75, 76);
                Vector2 position = npc.Center;
                float targetPosition1 = Main.player[npc.target].Center.X + RandomTarget2;
                float targetPosition2 = Main.player[npc.target].Center.Y + RandomTarget1;
                Vector2 targetPosition = new Vector2(targetPosition1, targetPosition2);
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                float speed = 2.6f + RandomAdd * 0.1f;
                npc.velocity += direction / speed;
                spiderexp = 0;
            }


            if(npc.velocity.X > 8)
            {
                npc.velocity.X = 8;
            }
            if(npc.velocity.X < -8)
            {
                npc.velocity.X = -8;
            }
            if(npc.velocity.Y > 8)
            {
                npc.velocity.Y = 8;
            }
            if(npc.velocity.Y < -8)
            {
                npc.velocity.Y = -8;
            }

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Vector2 drawOrigin = new Vector2(npc.Center.X, npc.Center.Y);
            for (int k = 0; k < npc.oldPos.Length; k++)
            {
                // Adjust drawPos if the hitbox does not match sprite dimension
                Vector2 drawPos = npc.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(-14, -4);
                Color afterImageColor = npc.life <= npc.lifeMax * 0.5 ? Color.Green : Color.LightGreen;
                Color color = npc.GetAlpha(afterImageColor) * ((float)(npc.oldPos.Length - k) / (float)npc.oldPos.Length);
                spriteBatch.Draw(Main.npcTexture[npc.type], drawPos, npc.frame, color, npc.rotation, drawOrigin, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            }
            return base.PreDraw(spriteBatch, drawColor);
        }

    

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = mod.GetTexture("NPCs/Bosses/EvilBoss/PixieBomb_Glow");
            spriteBatch.Draw(texture, new Vector2(npc.Center.X - Main.screenPosition.X, npc.Center.Y - Main.screenPosition.Y), npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2f, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

    

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 2;
            DisplayName.SetDefault("Shadow Pixie");
        }
    }
}

