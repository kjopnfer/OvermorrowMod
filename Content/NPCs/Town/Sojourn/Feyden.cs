using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Common.Pathfinding;
using OvermorrowMod.Content.Projectiles;
using OvermorrowMod.Content.UI.SpeechBubble;
using OvermorrowMod.Content.WorldGeneration;
using OvermorrowMod.Core;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Content.NPCs.Town.Sojourn
{
    public partial class Feyden : ModNPC
    {
        private WalkPathFinder _pf = new WalkPathFinder(SharedAIState.State1x2, new WalkPathFinderProperties
        {
            Acceleration = 1.0f,
            JumpSpeeds = new[] { 7f / 16f, 5.5f / 16f },
            MoveSpeed = 2f / 16f,
            MaxFallDepth = 50,
            NumPermutationSteps = 2,
            Timeout = 300,
            MaxDivergence = 100,
        });

        public override bool CanTownNPCSpawn(int numTownNPCs) => false;
        public override bool NeedSaving() => true;
        public override bool UsesPartyHat() => false;
        public override bool CheckActive() => false;
        public override bool CanChat() => true;

        // Legends never die
        public override bool CheckDead() => false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 25;
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true; // Sets NPC to be a Town NPC
            NPC.friendly = true; // NPC Will not attack player
            /*NPC.width = 32;
            NPC.height = 32;
            NPC.aiStyle = 7;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;*/
            NPC.width = 16;
            NPC.height = 32;
            NPC.aiStyle = 7;
            NPC.defense = 30;
            NPC.damage = 0;
            NPC.lifeMax = 150;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            AnimationType = NPCID.Guide;
        }

        public override void OnSpawn(IEntitySource source)
        {
            foreach (var npc in Main.npc)
            {
                if (npc.active && npc.type == Type && npc != NPC) npc.life = 0;
            }

            if (!OvermorrowWorld.savedFeyden) AIState = (int)AICase.Idle;
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
        }

        public enum AICase
        {
            Idle = 0,
            //Default = 0,
            Following = 1,
            Approach = 2,
            Fighting = 3,
            Dodge = 4,
        }

        public bool canFight = true; // TODO: Make this togglable through a UI
        public Player followPlayer = null;
        public NPC targetNPC = null;

        private Projectile swingAttack = null;

        private int AIState;
        private int AICounter = 0;
        private int AttackDelay = 0;

        private int prevTextIndex = -1;
        public override void AI()
        {
            DialoguePlayer dialoguePlayer = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();

            NPC.dontTakeDamage = false;

            // TODO: The NPC should check if their following player is active or exists, otherwise get reassigned to host
            if (followPlayer != null)
            {
                if (followPlayer.active) AIState = (int)AICase.Following;
                else ReassignToHost();
            }
            else
            {
                //if (OvermorrowWorld.savedFeyden) AIState = (int)AICase.Default;
            }

            if (targetNPC != null)
            {
                if (!targetNPC.active) targetNPC = null;
                else
                {
                    NPC.direction = targetNPC.Center.X > NPC.Center.X ? 1 : -1;
                }
            }

            TrySpawnSlimeCaveHandler();

            switch (AIState)
            {
                case (int)AICase.Idle:
                    SpawnIdleSlimes();

                    // Feyden will constantly search for targets to fight before they are "rescued"
                    if (!OvermorrowWorld.savedFeyden || targetNPC == null)
                    {
                        NPC.aiStyle = 0;
                        targetNPC = NPC.FindClosestNPC(45 * 16f);

                        if (AttackDelay > 0) AttackDelay--;

                        if (targetNPC != null && AttackDelay <= 0)
                        {
                            AIState = (int)AICase.Approach;
                        }
                    }
                    break;
                /*case (int)AICase.Default: // Behave like a normal town NPC
                    NPC.aiStyle = 7;
                    break;*/
                case (int)AICase.Following:
                    _pf.SetTarget(followPlayer.position);
                    _pf.GetVelocity(ref NPC.position, ref NPC.velocity);

                    NPC.aiStyle = 0;

                    if (canFight)
                    {
                        if (targetNPC == null)
                        {
                            targetNPC = NPC.FindClosestNPC(400f);
                        }
                    }

                    // The Pathfinder should reset every few seconds for any environmental changes
                    // TODO: Maybe make this more efficient by detecting environmental change to reset instead?
                    if (++AICounter % 600 == 0)
                    {
                        AICounter = 0;
                        SharedAIState.State1x2.Reset();
                    }
                    break;
                case (int)AICase.Approach:
                    // Move towards the NPC until they are 2 tiles away
                    if (targetNPC != null)
                    {
                        _pf.SetTarget(targetNPC.position);
                        _pf.GetVelocity(ref NPC.position, ref NPC.velocity);

                        if (NPC.Distance(targetNPC.position) <= 32)
                        {
                            Vector2 position = NPC.Center + new Vector2(32 * NPC.direction, -8);
                            swingAttack = Projectile.NewProjectileDirect(NPC.GetSource_FromAI("FriendyAttack"), position, Vector2.Zero, ModContent.ProjectileType<FeydenAttack>(), 10, 2f, Main.myPlayer, NPC.whoAmI);
                            AIState = (int)AICase.Fighting;

                            if (!OvermorrowWorld.savedFeyden && dialoguePlayer.Player.Distance(NPC.Center) < 32 * 16)
                            {
                                
                                /*if (Main.rand.NextBool(5) && !dialoguePlayer.CheckPopupAlreadyActive(ModContent.NPCType<Feyden>()))
                                {
                                    int attackID = Main.rand.Next(1, 9);
                                    dialoguePlayer.AddNPCPopup(ModContent.NPCType<Feyden>(), ModUtils.GetXML(AssetDirectory.Popups + "FeydenCave.xml"), "ATTACK_" + attackID);
                                }*/


                            }
                        }
                    }
                    else
                    {
                        if (!OvermorrowWorld.savedFeyden) AIState = (int)AICase.Idle;
                        //else AIState = (int)AICase.Default;
                    }
                    break;
                case (int)AICase.Fighting:
                    AttackDelay = 15;

                    if (swingAttack != null)
                    {
                        if (swingAttack.active)
                        {
                            NPC.knockBackResist *= 0;
                            return;
                        }
                    }

                    if (targetNPC != null)
                    {
                        // The NPC is dead, look for a new one
                        if (!OvermorrowWorld.savedFeyden) AIState = (int)AICase.Idle;
                        //else AIState = (int)AICase.Default;
                    }
                    else AIState = (int)AICase.Idle;
                    break;
                case (int)AICase.Dodge:
                    NPC.dontTakeDamage = true;

                    // Check left and right side and move to the more open area
                    if (AICounter == 0)
                    {
                        var left = TRay.CastLength(NPC.Center, -Vector2.UnitX, 16 * 6);
                        var right = TRay.CastLength(NPC.Center, Vector2.UnitX, 16 * 6);

                        int rollDirection = NPC.direction;
                        if (left > right) rollDirection = -1;
                        else if (right > left) rollDirection = 1;

                        NPC.velocity.X = 6 * rollDirection;
                    }

                    Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

                    if (++AICounter % 20 == 0)
                    {
                        AICounter = 0;
                        if (!OvermorrowWorld.savedFeyden) AIState = (int)AICase.Idle;
                    }
                    break;
            }
        }

        public override bool? CanFallThroughPlatforms()
        {
            return _pf.ShouldFallThroughPlatforms(NPC.velocity, NPC.position);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return 0f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            // Do hurt animation
            base.HitEffect(hit);
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (Main.rand.Next(1, 100) <= 69)
            {
                if (AIState != (int)AICase.Fighting)
                {
                    // Dodging can't completely ignore the damage this is the lowest that can be set
                    modifiers.ModifyHitInfo += (ref NPC.HitInfo hitInfo) =>
                    {
                        hitInfo.Damage = 1;
                    };

                    AICounter = 0;
                    AIState = (int)AICase.Dodge;
                }
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(AIState);
            writer.Write(AICounter);
            writer.Write(AttackDelay);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            AIState = reader.ReadInt16();
            AICounter = reader.ReadInt16();
            AttackDelay = reader.ReadInt16();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTextureHandler(out Texture2D texture, out int yFrameCount);

            Rectangle drawRectangle = new Rectangle(0, texture.Height / yFrameCount * yFrame, texture.Width, texture.Height / yFrameCount);
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.Draw(texture, NPC.Center - screenPos, drawRectangle, drawColor, NPC.rotation, drawRectangle.Size() / 2f, NPC.scale, spriteEffects, 0);

            return false;
        }
    }

    public class FeydenAttack : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 32;
            Projectile.friendly = true;

            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.tileCollide = false;
            Projectile.localNPCHitCooldown = -1; // 1 hit per npc max

            Projectile.timeLeft = 30;
        }

        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active) Projectile.Kill();

            Projectile.Center = npc.Center + new Vector2(16 * npc.direction, -8);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
    }
}