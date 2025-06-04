using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.NPCs.Archives;
using OvermorrowMod.Core.NPCs;
using System;
using System.Diagnostics.Metrics;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs
{
    public class GrimoireSpellCast : BaseAttackState
    {
        public override int Weight => 1;
        public override bool CanExit => IsFinished;

        private int tileAttackDistance = 24;
        private int attackDelay = 60;
        public GrimoireSpellCast(OvermorrowNPC npc) : base(npc) { }

        public override bool CanExecute()
        {
            if (OvermorrowNPC is not LivingGrimoire)
                 return false;
         

            if (OvermorrowNPC.AIStateMachine.GetPreviousSubstates().FirstOrDefault() is not BasicFly)
            {
                Main.NewText("have not flown yet, preventin early attack", Color.Red);
                return false;
            }

            if (!OvermorrowNPC.TargetingModule.HasTarget())
                return false;

            if (attackDelay-- > 0)
                return false;

            return AttackCondition(OvermorrowNPC);
        }

        public override void Enter()
        {
            OvermorrowNPC.AICounter = 0;
            IsFinished = false;
            Main.NewText("entering cast spell");
        }

        public override void Exit()
        {
            OvermorrowNPC.AICounter = 0;
            attackDelay = 60;
            Main.NewText("exiting spell", Color.Red);
        }

        private int castTime = 120;
        float flySpeedX = 2;
        float flySpeedY = 0;
        int distanceFromGround = 180;
        public override void Update()
        {
            OvermorrowNPC.AICounter++;
            NPC.velocity.X /= 2f;

            BasicFly.HandleVerticalMovementToTarget(OvermorrowNPC, ref flySpeedY);
            BasicFly.HandleGroundProximity(OvermorrowNPC, ref flySpeedY, distanceFromGround);

            CastSpell(OvermorrowNPC);

            LivingGrimoire bookNPC = OvermorrowNPC as LivingGrimoire;
            //Main.NewText("spell : " + npc.AICounter);
            if (OvermorrowNPC.AICounter >= bookNPC.CastTime)
            {
                IsFinished = true;
            }
        }

        int chairProjectilesNeeded = 3;
        private void CastSpell(OvermorrowNPC npc)
        {
            switch (NPC.ModNPC)
            {
                case BarrierBook:
                    if (npc.AICounter == 10)
                    {
                        float radius = 500f;
                        var nearbyHostileEnemies = Main.npc
                            .Where(enemy => enemy.active && !enemy.friendly && Vector2.Distance(NPC.Center, enemy.Center) <= radius && enemy.whoAmI != NPC.whoAmI)
                            .ToList();

                        foreach (NPC enemy in nearbyHostileEnemies)
                        {
                            enemy.AddBarrier(50, 100);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), enemy.Center, Vector2.Zero, ModContent.ProjectileType<BarrierEffect>(), 1, 1f, Main.myPlayer, ai0: enemy.whoAmI);
                        }
                    }
                    break;
                case BlasterBook:
                    if (npc.AICounter == 20)
                    {
                        Vector2 directionToPlayer = (npc.TargetingModule.Target.Center - NPC.Center).SafeNormalize(Vector2.Zero); // Direction vector to the player

                        float angleSpread = MathHelper.ToRadians(25); // Spread angle for randomness
                        Vector2 projectileVelocity = directionToPlayer.RotatedByRandom(angleSpread) * 8; // Randomized rotation towards the player
                        Vector2 spawnPosition = NPC.Center + new Vector2(32, 0).RotatedBy(directionToPlayer.ToRotation());
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPosition, projectileVelocity, ModContent.ProjectileType<BlastRune>(), NPC.damage, 1f, Main.myPlayer, NPC.whoAmI);
                    }
                    break;
                case PlantBook:
                    if (npc.AICounter == 30)
                    {

                        float angle = MathHelper.ToRadians(75);
                        Vector2 projectileVelocity = new Vector2(100 * NPC.direction, 0).RotatedByRandom(angle) * 50;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), npc.TargetingModule.Target.Center, Vector2.Zero, ModContent.ProjectileType<PlantAura>(), NPC.damage, 1f, Main.myPlayer);
                    }
                    break;
                case ChairBook:
                    if (chairProjectilesNeeded == 0) return;

                    if (npc.AICounter % 10 == 0 && npc.AICounter < 40)
                    {
                        float angle = MathHelper.ToRadians(75);
                        float randomDirection = Main.rand.NextBool() ? 1 : -1;

                        // Make sure the x-offset is never zero
                        Vector2 projectileVelocity = new Vector2(Main.rand.Next(-3, 2) + 1, Main.rand.Next(8, 10)).RotatedByRandom(angle);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projectileVelocity, ModContent.ProjectileType<ChairBolt>(), 1, 1f, Main.myPlayer, 0f, NPC.whoAmI);

                        chairProjectilesNeeded--;
                    }
                    break;
                default:
                    return;
            }
        }

        private bool AttackCondition(OvermorrowNPC npc)
        {
            switch (NPC.ModNPC)
            {
                case ChairBook:
                    var chairSummons = Main.npc
                    .Where(npc => npc.active
                        && npc.ModNPC is ChairSummon chairSummon
                        && chairSummon.ParentID == NPC.whoAmI)
                    .Select(npc => npc.ModNPC as ChairSummon)
                    .ToList();

                    if (chairSummons.Count == 3) return false;

                    chairProjectilesNeeded = 3 - chairSummons.Count;
                    break;
            }

            float xDistance = Math.Abs(NPC.Center.X - npc.TargetingModule.Target.Center.X);
            float yDistance = Math.Abs(NPC.Center.Y - npc.TargetingModule.Target.Center.Y);

            bool isWithinXRange = xDistance <= npc.TargetingModule.Config.MaxAttackRange;
            bool isWithinYRange = yDistance < 200;
            bool hasLineOfSight = Collision.CanHitLine(npc.TargetingModule.Target.Center, 1, 1, NPC.Center, 1, 1);

            Main.NewText(isWithinYRange, Color.CornflowerBlue);
            Main.NewText(isWithinXRange , Color.Red);
            Main.NewText(hasLineOfSight, Color.LightGreen);

            return isWithinXRange && isWithinYRange && hasLineOfSight;
        }
    }
}