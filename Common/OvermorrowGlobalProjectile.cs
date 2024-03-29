using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Players;
using OvermorrowMod.Common.VanillaOverrides.Bow;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using OvermorrowMod.Content.Items.Ammo;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public class OvermorrowGlobalProjectile : GlobalProjectile
    {
        // Life is pain
        public override bool InstancePerEntity => true;
        public bool IsBullet { get; private set; } = false;
        public bool IsArrow { get; private set; } = false;
        public bool IsPowerShot = false;
        public bool IsPowerShotSniper = false;
        public bool IsMirageArrow = false;

        public bool slowedTime = false;

        public bool RetractSlow = false;
        private bool HasRebounded = false;
        private bool PracticeTargetHit = false;

        private bool Farlander = false;
        private bool FarlanderPowerShot = false;
        private bool FarlanderHit = false;

        private bool Undertaker = false;
        private int UndertakerCounter = 0;
        private bool WildEyeCrit = false;

        public WeaponID SourceGunType = WeaponID.None;

        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[projectile.type].Value;

            if (IsMirageArrow)
            {
                Main.spriteBatch.Reload(BlendState.Additive, SpriteSortMode.Immediate);

                int shaderID = GameShaders.Armor.GetShaderIdFromItemId(ItemID.MirageDye);

                DrawData newData = new DrawData(texture, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, texture.Size() / 2f, projectile.scale, 0, 0);
                GameShaders.Armor.Apply(shaderID, projectile, newData);
                newData.Draw(Main.spriteBatch);

                Main.spriteBatch.Reload(BlendState.AlphaBlend, SpriteSortMode.Deferred);

                return false;
            }

            return base.PreDraw(projectile, ref lightColor);
        }


        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[projectile.owner];
            BowPlayer bowPlayer = player.GetModPlayer<BowPlayer>();

            if (WildEyeCrit) modifiers.SetCrit();

            if (Undertaker)
            {
                float pointBlankBonus = 1 + MathHelper.Lerp(1.5f, 0, UndertakerCounter / 15f);
                modifiers.SourceDamage *= pointBlankBonus;
            }

            if (IsPowerShot)
            {
                modifiers.SourceDamage *= 1.25f;
            }

            if (projectile.type == ModContent.ProjectileType<ObsidianArrowProjectile>())
            {
                modifiers.CritDamage *= 1.25f;
            }

            #region Armor
            if (player.CheckArmorEquipped(ItemID.CowboyHat) && projectile.DamageType == DamageClass.Ranged)
            {
                modifiers.CritDamage *= 1.1f;
            }


            #endregion

            #region Accessories
            if (player.GetModPlayer<OvermorrowModPlayer>().SnakeBite && IsArrow && Main.rand.NextBool(5))
            {
                target.AddBuff(BuffID.Poisoned, 180);

                bool applyVenom = bowPlayer.ArrowArmorPenetration + projectile.ArmorPenetration > target.defense;

                if (!player.GetModPlayer<OvermorrowModPlayer>().SnakeBiteHide)
                {
                    float numSpawned = Main.rand.Next(4, 6);
                    for (int i = 0; i < numSpawned; i++)
                    {
                        float scale = Main.rand.NextFloat(0.7f, 1.25f);
                        float randomVelocity = Main.rand.Next(1, 2);
                        float velocityRotation = MathHelper.ToRadians(i * (360 / numSpawned) + Main.rand.Next(0, 4) * 15);
                        float randomTime = Main.rand.Next(3, 5) * 15;

                        //float cloudScale = Main.rand.NextFloat(0.21f, 0.22f);
                        //Particle.CreateParticle(Particle.ParticleType<Particles.VenomCloud>(), projectile.Center, Vector2.One.RotatedBy(velocityRotation) * randomVelocity, Color.LimeGreen, 1f, cloudScale, 0f, Main.rand.Next(7, 9) * 10);

                        if (applyVenom && !target.buffImmune[BuffID.Venom])
                            Particle.CreateParticle(Particle.ParticleType<VenomOrb>(), projectile.Center, Vector2.One.RotatedBy(velocityRotation) * randomVelocity, Color.LimeGreen, 1f, scale, 0f, randomTime);
                        else if (!target.buffImmune[BuffID.Poisoned])
                            Particle.CreateParticle(Particle.ParticleType<PoisonOrb>(), projectile.Center, Vector2.One.RotatedBy(velocityRotation) * randomVelocity, Color.LimeGreen, 1f, scale, 0f, randomTime);
                    }

                    numSpawned = Main.rand.Next(2, 4);
                    for (int i = 0; i < numSpawned; i++)
                    {
                        float velocityRotation = MathHelper.ToRadians(i * (360 / numSpawned) + Main.rand.Next(0, 4) * 15);
                        float randomVelocity = Main.rand.Next(0, 2);
                        float randomTime = Main.rand.Next(7, 9) * 10;

                        float cloudScale = Main.rand.NextFloat(0.21f, 0.22f);

                        if (applyVenom && !target.buffImmune[BuffID.Venom])
                            Particle.CreateParticle(Particle.ParticleType<Particles.VenomCloud>(), projectile.Center, Vector2.One.RotatedBy(velocityRotation) * randomVelocity, Color.LimeGreen, 1f, cloudScale, 0f, randomTime);
                        else if (!target.buffImmune[BuffID.Poisoned])
                            Particle.CreateParticle(Particle.ParticleType<Particles.Cloud>(), projectile.Center, Vector2.One.RotatedBy(velocityRotation) * randomVelocity, Color.LimeGreen, 1f, cloudScale, 0f, randomTime);
                    }
                }

                // Shitty way of handling armor penetration for this accessory, not sure if this even works
                projectile.ArmorPenetration += bowPlayer.ArrowArmorPenetration;

                if (applyVenom)
                {
                    for (int i = 0; i < target.buffType.Length; i++)
                    {
                        if (target.buffType[i] == BuffID.Poisoned) target.DelBuff(i);
                    }

                    target.AddBuff(BuffID.Venom, 120);
                }
            }
            #endregion
        }

        public Entity ownerEntity { get; private set; }
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_Parent parent)
            {
                ownerEntity = parent.Entity;
                /*if (parent.Entity is NPC npc)
                    Main.NewText("from npc " + npc.FullName);

                if (parent.Entity is Player player)
                    Main.NewText("from player " + player.name);*/
            }

            if (source is EntitySource_ItemUse_WithAmmo itemUse_WithAmmo && itemUse_WithAmmo.Item is Item item)
            {
                if (item.IsGun()) SourceGunType = item.GetWeaponTypeID();
            }


            if (source != null && source.Context != null)
            {
                string[] sourceInfo = source.Context.ToString().Split('_');
                string sourceType = sourceInfo[0];

                string sourceAction = "none";
                if (sourceInfo.Length > 1) sourceAction = sourceInfo[1];

                if (source is EntitySource_ItemUse_WithAmmo)
                {
                    if (sourceType == "HeldGun")
                    {
                        IsBullet = true;
                        SetHeldGunVariables(sourceAction);
                    }

                    if (sourceType == "HeldBow")
                    {
                        IsArrow = true;
                    }
                }
            }

            // Vanilla arrows default to a rotation of 0 when initially spawned, I don't know why
            if (IsArrow)
            {
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
        }

        private void SetHeldGunVariables(string sourceAction)
        {
            if (sourceAction == "none") return;
            switch (sourceAction)
            {
                case "WildEyeCrit":
                    WildEyeCrit = true;
                    break;
                case "Undertaker":
                    Undertaker = true;
                    break;
                case "Farlander":
                    Farlander = true;
                    break;
                case "FarlanderPowerShot":
                    Farlander = true;
                    FarlanderPowerShot = true;
                    break;
            }
        }

        public override bool PreAI(Projectile projectile)
        {
            if (slowedTime && !projectile.friendly)
            {
                projectile.position -= projectile.velocity * 0.95f;
            }

            if (Undertaker && UndertakerCounter < 15) UndertakerCounter++;

            return base.PreAI(projectile);
        }

        public override void SetDefaults(Projectile projectile)
        {
            if (projectile.type == 590)
            {
                projectile.timeLeft = 100;
            }
        }

        public override void OnKill(Projectile projectile, int timeLeft)
        {
            Player player = Main.player[projectile.owner];
            BowPlayer bowPlayer = player.GetModPlayer<BowPlayer>();
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();

            if (player.GetModPlayer<OvermorrowModPlayer>().PracticeTarget && IsArrow && !PracticeTargetHit)
            {
                SpawnPracticeTargetFail(player);
            }

            if (Farlander && !FarlanderHit)
            {
                gunPlayer.FarlanderSpeedBoost = 0;
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[projectile.owner];
            BowPlayer bowPlayer = player.GetModPlayer<BowPlayer>();
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();

            if (player.GetModPlayer<OvermorrowModPlayer>().PracticeTarget && IsArrow && !PracticeTargetHit)
            {
                SpawnPracticeTargetHit(player);
            }

            if (Farlander)
            {
                FarlanderHit = true;

                if (FarlanderPowerShot)
                {
                    if (gunPlayer.FarlanderSpeedBoost < 4) gunPlayer.FarlanderSpeedBoost++;
                }
            }

            if (player.GetModPlayer<GunPlayer>().CowBoySet && hit.Crit && SourceGunType == WeaponID.Revolver)
            {
                NPC closestNPC = projectile.FindClosestNPC(16 * 30f, target);
                if (closestNPC != null)
                {
                    projectile.usesLocalNPCImmunity = true;
                    projectile.localNPCHitCooldown = -1;
                    projectile.velocity = Vector2.Normalize(projectile.DirectionTo(closestNPC.Center)) * 12f;
                    if (!HasRebounded)
                    {
                        projectile.penetrate = 2;
                        HasRebounded = true;
                    }
                }
            }
        }

        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            Player player = Main.player[projectile.owner];
            BowPlayer bowPlayer = player.GetModPlayer<BowPlayer>();
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();

            if (Farlander && !FarlanderHit)
            {
                gunPlayer.FarlanderSpeedBoost = 0;
            }

            return base.OnTileCollide(projectile, oldVelocity);
        }

        private void SpawnPracticeTargetHit(Player player)
        {
            BowPlayer bowPlayer = player.GetModPlayer<BowPlayer>();

            if (bowPlayer.PracticeTargetCounter < 5)
            {
                bowPlayer.PracticeTargetCounter++;
                Projectile.NewProjectile(null, player.Center + new Vector2(-4, -32), Vector2.Zero, ModContent.ProjectileType<Content.Items.Accessories.PracticeTarget.PracticeTargetIcon>(), 0, 0f, player.whoAmI);
            }

            PracticeTargetHit = true;
        }

        private void SpawnPracticeTargetFail(Player player)
        {
            BowPlayer bowPlayer = player.GetModPlayer<BowPlayer>();

            int failCount = bowPlayer.PracticeTargetCounter == 0 ? -1 : bowPlayer.PracticeTargetCounter;

            Projectile.NewProjectile(null, player.Center + new Vector2(-4, -32), Vector2.Zero, ModContent.ProjectileType<Content.Items.Accessories.PracticeTarget.PracticeTargetIcon>(), 0, 0f, player.whoAmI, 0f, failCount);
            bowPlayer.PracticeTargetCounter = 0;
        }

        public override void GrappleRetreatSpeed(Projectile projectile, Player player, ref float speed)
        {
            if (RetractSlow)
            {
                speed = 4;
            }

            base.GrappleRetreatSpeed(projectile, player, ref speed);
        }

    }
}