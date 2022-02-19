using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.PostRider
{
    public class JungleDiveBomber : ModNPC
    {

        readonly bool expert = Main.expertMode;
        private int experttimer = 0;
        int timer = 0;
        float savedX = 0;
        float savedY = 0;
        private const string ChainTexturePath = "OvermorrowMod/NPCs/PostRider/Gore68";
        int RandomAttPosX = Main.rand.Next(-75, 76);
        int RandomAttPosY = Main.rand.Next(-75, 76);
        Vector2 vinepos;

        public override void SetDefaults()
        {
            npc.damage = 75;
            npc.defense = 6;
            npc.lifeMax = 200;
            npc.aiStyle = 20;
            npc.width = 58;
            npc.height = 58;
            animationType = NPCID.ManEater;
            npc.noTileCollide = true;
            npc.noGravity = true;
        }

        int damage = 65;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 3;
        }

        public override void NPCLoot()
        {
            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("JungleEssence"));
        }
        public override void AI()
        {

            npc.TargetClosest(true);

            experttimer++;
            if (experttimer == 1)
            {
                savedX = npc.position.X;
                savedY = npc.position.Y;
                vinepos = npc.Center;
            }

            if (expert && experttimer == 1)
            {
                npc.life = 300;
                npc.lifeMax = 300;
                npc.damage = 50;
            }

            if (experttimer > 1)
            {
                if (npc.position.X > savedX + 555)
                {
                    npc.velocity.X *= -2f;
                }
                if (npc.position.X < savedX - 555)
                {
                    npc.velocity.X *= -2f;
                }

                if (npc.position.Y > savedY + 555)
                {
                    npc.velocity.Y *= -2f;
                }
                if (npc.position.Y < savedY - 555)
                {
                    npc.velocity.Y *= -2f;
                }
            }



            if (npc.velocity.X > 4f)
            {
                npc.velocity.X = 4f;
            }
            if (npc.velocity.X < -4f)
            {
                npc.velocity.X = -4f;
            }

            if (npc.velocity.Y > 4f)
            {
                npc.velocity.Y = 4f;
            }
            if (npc.velocity.Y < -4f)
            {
                npc.velocity.Y = -4f;
            }

            timer++;
            if (timer == 1)
            {
                RandomAttPosX = Main.rand.Next(-110, 111);
                RandomAttPosY = Main.rand.Next(-110, 111);
                Vector2 position = npc.Center;
                Vector2 targetPosition = Main.player[npc.target].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                int type = mod.ProjectileType("JungleBoom");
                Main.PlaySound(SoundID.Item62, npc.Center);
                if (!expert)
                {
                    damage = npc.damage * 2;
                }
                else
                {
                    damage = npc.damage + 10;
                }

                Projectile.NewProjectile(npc.Center.X + RandomAttPosX, npc.Center.Y + RandomAttPosY, 0, 0, type, damage, 0f, Main.myPlayer);
            }
            if (timer == 27)
            {
                timer = 0;
            }
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 mountedCenter = vinepos;
            Texture2D chainTexture = ModContent.GetTexture(ChainTexturePath);

            var drawPosition = npc.Center;
            var remainingVectorToPlayer = mountedCenter - drawPosition;

            float rotation = remainingVectorToPlayer.ToRotation() - MathHelper.PiOver2;

            // This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
            while (true)
            {
                float length = remainingVectorToPlayer.Length();

                if (length < 25f || float.IsNaN(length))
                    break;

                // drawPosition is advanced along the vector back to the player by 12 pixels
                // 12 comes from the height of ExampleFlailProjectileChain.png and the spacing that we desired between links
                drawPosition += remainingVectorToPlayer * 28 / length;
                remainingVectorToPlayer = mountedCenter - drawPosition;

                // Finally, we draw the texture at the coordinates using the lighting information of the tile coordinates of the chain section
                Color color = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));
                spriteBatch.Draw(chainTexture, drawPosition - Main.screenPosition, null, color, rotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            }

            return true;
        }


        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return Main.hardMode == true && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 && spawnInfo.player.ZoneJungle && spawnInfo.player.ZoneOverworldHeight ? 0.3f : 0f;
        }
    }
}