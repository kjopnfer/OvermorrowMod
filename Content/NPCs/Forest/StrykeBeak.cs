using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.Biomes;
using OvermorrowMod.Content.Items.Accessories;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace OvermorrowMod.Content.NPCs.Forest
{
    public class StrykeBeak : ModNPC
    {
        private const int MAX_FRAMES = 8;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Red Strykebeak");
            Main.npcFrameCount[NPC.type] = MAX_FRAMES;
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 38;
            NPC.damage = 20;
            NPC.defense = 6;
            NPC.lifeMax = 360;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit28;
            NPC.DeathSound = SoundID.NPCDeath31;
            NPC.value = 60f;

            // knockBackResist is the multiplier applied to the knockback the NPC receives when it takes damage
            NPC.knockBackResist = 0.5f;

            //NPC.aiStyle = 5;
            //AIType = NPCID.EaterofSouls;
        }

        public enum AICase
        {
            Stunned = -2,
            Hit = -1,
            Idle = 0,
            Angry = 1
        }

        public ref float AIState => ref NPC.ai[0];
        Player player => Main.player[NPC.target];

        float flySpeedX = 2;
        float flySpeedY = 0;

        public override void AI()
        {
            frameTimer++;
            if (frameTimer % 5 == 0)
            {
                if (frame < 7)
                    frame++;
                else
                    frame = 0;
            }

            switch (AIState)
            {
                case (int)AICase.Angry:
                    if (NPC.Center.X <= NPC.Center.X + 30 && flySpeedX <= 2)
                    {
                        flySpeedX += 0.1f;
                        NPC.direction = 1;
                    }

                    //if (NPC.Center.X <= player.Center.X && flySpeedX <= 2)
                    //    flySpeedX += 0.1f;

                    /*if (NPC.Center.Y >= player.Center.Y - 45)
                    {
                        flySpeedY -= 0.1f;
                    }
                    else
                        if (flySpeedY <= 2) flySpeedY += 0.1f;*/
                    break;
                case (int)AICase.Idle:
                    NPC.TargetClosest();

                    if (NPC.Center.X >= player.Center.X && flySpeedX >= -2) // flies to players x position
                        flySpeedX -= 0.1f;

                    if (NPC.Center.X <= player.Center.X && flySpeedX <= 2)
                        flySpeedX += 0.1f;

                    if (NPC.Center.Y >= player.Center.Y - 45)
                    {
                        flySpeedY -= 0.1f;
                    }
                    else
                        if (flySpeedY <= 2) flySpeedY += 0.1f;

                    break;
            }

            if (AIState == (int)AICase.Angry) NPC.rotation = NPC.velocity.X * 0.075f;

            NPC.velocity.X = flySpeedX;
            NPC.velocity.Y = flySpeedY;
        }

        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
        {
            if (AIState == (int)AICase.Idle)
            {
                NPC.friendly = false;
                //AIState = (int)AICase.Angry;

                //flySpeedX += knockback;
                //flySpeedY += knockback;
            }
        }

        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            if (AIState == (int)AICase.Idle)
            {
                NPC.friendly = false;
                //AIState = (int)AICase.Angry;
            }

            flySpeedX += projectile.velocity.X * (projectile.knockBack * NPC.knockBackResist);
            flySpeedY += projectile.velocity.Y * (projectile.knockBack * NPC.knockBackResist);
        }

        private int frame = 0;
        private int frameTimer = 0;
        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            NPC.frame.Y = frameHeight * frame;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.OverworldDaySlime.Chance * 0.3f;
        }
    }
}