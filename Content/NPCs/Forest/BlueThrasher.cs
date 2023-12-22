using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.NPCs;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Forest
{
    public class BlueThrasher : OvermorrowNPC
    {
        private const int MAX_FRAMES = 9;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = MAX_FRAMES;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                // The small one
                //Position = new Vector2(16, 8),

                // The big image
                //PortraitPositionXOverride = 16,
                //PortraitPositionYOverride = 12,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SafeSetDefaults()
        {
            NPC.width = 42;
            NPC.height = 34;
            NPC.damage = 14;
            NPC.defense = 20;
            NPC.lifeMax = 240;
            NPC.noGravity = false;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath16;
            NPC.value = Item.buyPrice(silver: 1, copper: 30);

            // knockBackResist is the multiplier applied to the knockback the NPC receives when it takes damage
            NPC.knockBackResist = 0.1f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement("Slow but with a tough exoskeleton, Blue Thrashers utilize their heavy armor to bash and stun their prey.")
            });
        }

        public int idleDirection;
        public int idleHeight;
        public override void OnSpawn(IEntitySource source)
        {
            idleDirection = NPC.Center.X / 16 > Main.maxTilesX / 2 ? -1 : 1;
            idleHeight = Main.rand.Next(17, 24) * 10;
        }

        public enum AICase
        {
            Stunned = -1,
            Crawl = 0,
            Fly = 1,
            Wall = 2,
        }

        public override void AI()
        {
            float moveSpeed = 0.5f;
            float speedCap = 2f;
            switch (AIState)
            {
                #region Crawl
                case (int)AICase.Crawl:
                    if (target != null) idleDirection = target.Center.X > NPC.Center.X ? 1 : -1;

                    NPC.HorizontallyMove(NPC.Center + (Vector2.UnitX * 20 * idleDirection), moveSpeed, speedCap, 128, 128, true);
                    if (NPC.collideY) AICounter++;

                    /*if (NPC.velocity.X == 0 && !NPC.collideX && !NPC.collideY) NPC.velocity.X = moveSpeed;

                    if (NPC.justHit) Main.NewText("yes");

                    Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

                    if (NPC.collideX && NPC.collideY) NPC.velocity.Y = -4f;*/

                    // Check if there is enough space to fly
                    if (AICounter >= 120)
                    {
                        Main.NewText("switch to fly");
                        AICounter = 0;
                        AIState = (int)AICase.Fly;
                    }
                    break;
                #endregion
                #region Fly
                case (int)AICase.Fly:
                    break;
                #endregion
                default:
                    break;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 3;
            NPC.spriteDirection = NPC.direction;

            switch (AIState)
            {

                case (int)AICase.Fly:
                case (int)AICase.Crawl:
                    NPC.frame.X = 0;
                    if (NPC.collideY || NPC.CheckEntityBottomSlopeCollision() || NPC.IsABestiaryIconDummy) NPC.frameCounter++;
                    if (NPC.frameCounter % 6 == 0 && NPC.frameCounter != 0)
                    {
                        if (NPC.frame.Y < 7 * frameHeight)
                            NPC.frame.Y += frameHeight;
                        else
                            NPC.frame.Y = 0;
                    }
                    break;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture, NPC.Center + new Vector2(0, -6) - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return 0f;
            return spawnInfo.Player.ZonePurity && Main.dayTime ? 0.2f : 0f;
        }
    }
}