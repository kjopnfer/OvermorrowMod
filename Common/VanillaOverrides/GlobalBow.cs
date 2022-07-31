using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Overmorrow.Common;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using OvermorrowMod.Content.Buffs.Debuffs;
using OvermorrowMod.Content.UI;
using OvermorrowMod.Content.Projectiles.Misc;

namespace OvermorrowMod.Common.VanillaOverrides
{
    public class BowOverride
    {
        public int ItemID;
        public int ConvertAll;
        public int ConvertWood;
        public int ChargeTime;
        public int ArrowsFired;
        public int ConsumeChance;

        /// <summary>
        /// Japan's dumb code to make shit more READABLE
        /// </summary>
        /// <param name="ItemID">The bow ID to be overriden</param>
        /// <param name="ConvertAll">Converts all arrows to a specific type, enter 0 to not convert arrows. Enter 1 to leave as wooden.</param>
        /// <param name="ConvertWood">Converts all wooden arrows to this type of arrow</param>
        /// <param name="ChargeTime">The amount of time it takes for the bow to fully charge</param>
        /// <param name="ArrowsFired">The amount of arrows fired per shot</param>
        /// <param name="ConsumeChance">The probability of the ammo being consumed by percentage, entering 1 means 1% chance</param>
        public BowOverride(int ItemID, int ConvertAll, int ConvertWood, int ChargeTime, int ArrowsFired, int ConsumeChance)
        {
            this.ItemID = ItemID;
            this.ConvertAll = ConvertAll;
            this.ConvertWood = ConvertWood;
            this.ChargeTime = ChargeTime;
            this.ArrowsFired = ArrowsFired;
            this.ConsumeChance = ConsumeChance;
        }
    }

    /// <summary>
    /// the frank fire code
    /// </summary>
    public class GlobalBow : GlobalItem
    {
        public override bool InstancePerEntity => true;

        private bool channelCheck;
        private bool bowDrawCheck;
        private bool fullChargeCheck;
        private float chargeDamageScale;
        public static int convertWood;
        public static int convertAll;
        public static List<BowOverride> BowsToOverride = new List<BowOverride>();
        public static List<BowOverride> ModBowsToOverride = new List<BowOverride>();
        public List<int> ShittyAmmoConservationThing = new List<int>();

        public int DoShittyAmmoConservationThing()
        {
            Player player = Main.LocalPlayer;
            //thanks ilspy very cool
            return (player.magicQuiver ? 20 : 0) + (player.HasBuff(93) ? 20 : 0) + (player.HasBuff(112) ? 20 : 0) + ((player.armor[1].type == ItemID.ShroomiteBreastplate) ? 20 : 0) + ((player.armor[0].type == ItemID.ChlorophyteHelmet) ? 20 : 0) + ((player.armor[1].type == ItemID.HuntressAltShirt) ? 20 : 0) + ((player.armor[1].type == ItemID.VortexBreastplate) ? 25 : 0) + ((player.armor[1].type == ItemID.HuntressJerkin) ? 10 : 0) + ((player.setBonus == Language.GetTextValue("ArmorSetBonus.Fossil")) ? 20 : 0) + ((player.setBonus == Language.GetTextValue("ArmorSetBonus.CobaltRanged")) ? 20 : 0) + ((player.setBonus == Language.GetTextValue("ArmorSetBonus.MythrilRanged")) ? 20 : 0) + ((player.setBonus == Language.GetTextValue("ArmorSetBonus.AdamantiteRanged")) ? 25 : 0) + ((player.setBonus == Language.GetTextValue("ArmorSetBonus.Titanium") && player.armor[0].type == ItemID.TitaniumHelmet) ? 25 : 0);
        }

        public static void AddBowsToOverride()
        {
            BowsToOverride.Clear();

            BowsToOverride.Add(new BowOverride(ItemID.WoodenBow, 0, 0, 180, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.BorealWoodBow, 0, 0, 180, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.CopperBow, 0, 0, 180, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.PalmWoodBow, 0, 0, 180, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.RichMahoganyBow, 0, 0, 180, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.TinBow, 0, 0, 180, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.EbonwoodBow, 0, 0, 180, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.IronBow, 0, 0, 180, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.ShadewoodBow, 0, 0, 180, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.LeadBow, 0, 0, 165, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.PearlwoodBow, 0, 0, 165, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.SilverBow, 0, 0, 165, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.TungstenBow, 0, 0, 165, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.GoldBow, 0, 0, 150, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.PlatinumBow, 0, 0, 150, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.DemonBow, 0, 0, 150, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.TendonBow, 0, 0, 150, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.HellwingBow, 0, ProjectileID.Hellwing, 135, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.BeesKnees, 0, ProjectileID.BeeArrow, 135, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.MoltenFury, 0, ProjectileID.FireArrow, 135, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.DD2PhoenixBow, ProjectileID.DD2PhoenixBowShot, 0, 90, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.IceBow, ProjectileID.FrostArrow, 0, 120, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.ShadowFlameBow, ProjectileID.ShadowFlameArrow, 0, 120, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.Marrow, ProjectileID.BoneArrow, 0, 120, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.Phantasm, 0, ProjectileID.PhantasmArrow, 60, 7, 66));
            BowsToOverride.Add(new BowOverride(ItemID.Tsunami, 0, 0, 60, 5, 0));
            BowsToOverride.Add(new BowOverride(ItemID.DD2BetsyBow, ProjectileID.DD2BetsyArrow, 0, 60, 3, 0));
            BowsToOverride.Add(new BowOverride(ItemID.PulseBow, ProjectileID.PulseBolt, 0, 105, 1, 0));

            //if (RadiantShadows.shid)
            //{
            //int i = 0;
            //	while (OvermorrowModFile.ModBowsToOverride.Count > i)
            //	{
            //		BowsToOverride.Add(new BowOverride(OvermorrowModFile.ModBowsToOverride[i][0], OvermorrowModFile.ModBowsToOverride[i][1], OvermorrowModFile.ModBowsToOverride[i][2], OvermorrowModFile.ModBowsToOverride[i][3], OvermorrowModFile.ModBowsToOverride[i][4], OvermorrowModFile.ModBowsToOverride[i][5]));
            //		i++;
            //	}
            //}
        }
        /*public virtual void AddModBows()
		{
			BowsToOverride.Add(new int[] { ModContent.ItemType<Testing.testSemiCoolBow>(), 0, 0, 120, 3, 0 });
			BowsToOverride.Add(new int[] { ModContent.ItemType<Slingshot>(), ModContent.ProjectileType<RandBird>(), 0, 120, 1, 0 });
		}*/

        public static void LoadBows()
        {
            BowsToOverride = new List<BowOverride>();
            ModBowsToOverride = new List<BowOverride>();

            BowsToOverride.Add(new BowOverride(ItemID.WoodenBow, 0, 0, 180, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.BorealWoodBow, 0, 0, 180, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.CopperBow, 0, 0, 180, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.PalmWoodBow, 0, 0, 180, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.RichMahoganyBow, 0, 0, 180, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.TinBow, 0, 0, 180, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.EbonwoodBow, 0, 0, 180, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.IronBow, 0, 0, 180, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.ShadewoodBow, 0, 0, 180, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.LeadBow, 0, 0, 165, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.PearlwoodBow, 0, 0, 165, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.SilverBow, 0, 0, 165, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.TungstenBow, 0, 0, 165, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.GoldBow, 0, 0, 150, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.PlatinumBow, 0, 0, 150, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.DemonBow, 0, 0, 150, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.TendonBow, 0, 0, 150, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.HellwingBow, 0, ProjectileID.Hellwing, 135, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.BeesKnees, 0, ProjectileID.BeeArrow, 135, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.MoltenFury, 0, ProjectileID.FireArrow, 135, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.DD2PhoenixBow, ProjectileID.DD2PhoenixBowShot, 0, 90, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.IceBow, ProjectileID.FrostArrow, 0, 120, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.ShadowFlameBow, ProjectileID.ShadowFlameArrow, 0, 120, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.Marrow, ProjectileID.BoneArrow, 0, 120, 1, 0));
            BowsToOverride.Add(new BowOverride(ItemID.Phantasm, 0, ProjectileID.PhantasmArrow, 60, 7, 66));
            BowsToOverride.Add(new BowOverride(ItemID.Tsunami, 0, 0, 60, 5, 0));
            BowsToOverride.Add(new BowOverride(ItemID.DD2BetsyBow, ProjectileID.DD2BetsyArrow, 0, 60, 3, 0));
            BowsToOverride.Add(new BowOverride(ItemID.PulseBow, ProjectileID.PulseBolt, 0, 105, 1, 0));
        }

        public static void UnloadBows()
        {
            BowsToOverride = null;
            ModBowsToOverride = null;
        }

        public override void SetDefaults(Item item)
        {
            //AddBowsToOverride();
            //AddModBows();
            for (int i = 0; i < BowsToOverride.Count; i++)
            {
                if (item.type == BowsToOverride[i].ItemID)
                {
                    item.channel = true;
                    convertAll = BowsToOverride[i].ConvertAll;
                    convertWood = BowsToOverride[i].ConvertWood;
                }
            }
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            for (int i = 0; i < BowsToOverride.Count; i++)
            {
                if (item.type == BowsToOverride[i].ItemID && Config.improveBowsSend)
                {
                    damage.Flat = (item.damage * 7);
                }
            }
        }

        public override void UpdateInventory(Item item, Player player)
        {
            Main.LocalPlayer.GetModPlayer<TrajectoryPlayer>();

            for (int i = 0; i < BowsToOverride.Count; i++)
            {
                if (item.type == BowsToOverride[i].ItemID)
                {
                    if (Config.improveBowsSend)
                    {
                        item.shoot = ModContent.ProjectileType<ghostProjectile>();
                        item.useStyle = ItemUseStyleID.Shoot;
                        item.useTime = 1;
                        item.useAnimation = 5;
                        item.reuseDelay = 0;
                        item.channel = true;
                        item.autoReuse = true;
                        item.noMelee = true;
                        item.UseSound = null;
                        item.noUseGraphic = false;
                    }
                    else
                    {
                        item.CloneDefaults(BowsToOverride[i].ItemID);
                    }
                }
            }
        }

        public override bool CanUseItem(Item item, Player player)
        {
            for (int i = 0; i < BowsToOverride.Count; i++)
            {
                if (Config.improveBowsSend && item.type == BowsToOverride[i].ItemID)
                {
                    return !player.HasBuff(ModContent.BuffType<BowDryfire>()) && !player.dead && ((player.channel && !player.dead) || (!bowChargeUI.mouseHoverCharge && !bowChargeUI.dragging));
                }
            }

            return true;
        }

        public override void HoldItem(Item item, Player player)
        {
            for (int i = 0; i < BowsToOverride.Count; i++)
            {
                if (item.type == BowsToOverride[i].ItemID && Config.improveBowsSend)
                {
                    SetDefaults(item);
                    Vector2 playerPos = new Vector2(player.Center.X, player.Center.Y);
                    TrajectoryPlayer trajectoryPlayer = Main.LocalPlayer.GetModPlayer<TrajectoryPlayer>();

                    trajectoryPlayer.drawChargeBar = true;
                    trajectoryPlayer.bowTimingReduce = (player.magicQuiver ? 15 : 0);
                    trajectoryPlayer.bowTimingMax = BowsToOverride[i].ChargeTime - trajectoryPlayer.bowTimingReduce;

                    Vector2 dir = (Main.MouseWorld - player.Center) / 30f;
                    if (player.channel)
                    {
                        trajectoryPlayer.drawTrajectory = true;
                        float distanceX = Main.MouseWorld.X - trajectoryPlayer.trajPointX;
                        float distanceY = Main.MouseWorld.Y - trajectoryPlayer.trajPointY;
                        chargeDamageScale = player.GetWeaponDamage(player.HeldItem) / (float)trajectoryPlayer.bowTimingMax * trajectoryPlayer.bowTiming;
                        player.itemTime = 5;

                        if (bowDrawCheck)
                        {
                            player.direction = ((Main.MouseWorld.X > player.Center.X) ? -1 : 1);
                            player.itemRotation = (float)Math.Atan2((distanceY * -1f * player.direction), (distanceX * -1f * player.direction));
                            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.DirectionTo(Main.MouseWorld).ToRotation() + MathHelper.PiOver2);
                        }

                        if (!channelCheck)
                        {
                            //Cursor.Position = new System.Drawing.Point(Main.screenWidth / 2, Main.screenHeight / 2);
                            channelCheck = true;
                        }

                        if (!fullChargeCheck && trajectoryPlayer.bowTimingMax == trajectoryPlayer.bowTiming)
                        {
                            SoundEngine.PlaySound(SoundID.MaxMana, player.Center);
                            fullChargeCheck = true;
                        }

                        if (!bowDrawCheck)
                        {
                            if (Main.MouseWorld != new Vector2((Main.screenWidth / 2), (Main.screenHeight / 2)))
                            {
                                SoundStyle sound = new SoundStyle("OvermorrowMod/Sounds/bowCharge");
                                SoundEngine.PlaySound(sound, player.Center);
                                Mouse.SetPosition(Main.screenWidth / 2, Main.screenHeight / 2);
                                bowDrawCheck = true;
                            }
                        }
                        else if (trajectoryPlayer.bowTiming < trajectoryPlayer.bowTimingMax)
                        {
                            trajectoryPlayer.bowTiming++;
                        }
                    }
                    else
                    {
                        bowDrawCheck = false;
                        channelCheck = false;
                        fullChargeCheck = false;

                        if (trajectoryPlayer.bowTiming != 0)
                        {
                            int toShoot = 0;
                            int DontConsumeChance = 0;
                            if (player.inventory[54].type == ItemID.None && player.inventory[55].type == ItemID.None && player.inventory[56].type == ItemID.None && player.inventory[57].type == ItemID.None)
                            {
                                for (int j = 0; j < player.inventory.Length; j++)
                                {
                                    if (player.inventory[j].ammo == AmmoID.Arrow)
                                    {
                                        toShoot = player.inventory[j].shoot;
                                        int damage = player.inventory[j].damage;
                                        if (!player.inventory[j].consumable)
                                        {
                                            break;
                                        }
                                        DontConsumeChance += DoShittyAmmoConservationThing() + BowsToOverride[i].ConsumeChance;
                                        if (Main.rand.Next(1, 101) > DontConsumeChance)
                                        {
                                            player.inventory[j].stack--;
                                            break;
                                        }
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                // I think this is the ammo slots or something
                                for (int j = 54; j < 58; j++)
                                {
                                    if (player.inventory[j].ammo == AmmoID.Arrow)
                                    {
                                        toShoot = player.inventory[j].shoot;
                                        int damage2 = player.inventory[j].damage;
                                        if (!player.inventory[j].consumable)
                                        {
                                            break;
                                        }
                                        DontConsumeChance += DoShittyAmmoConservationThing() + BowsToOverride[i].ConsumeChance;
                                        if (Main.rand.Next(1, 101) > DontConsumeChance)
                                        {
                                            player.inventory[j].stack--;
                                            break;
                                        }
                                        break;
                                    }
                                }
                            }

                            if ((int)chargeDamageScale != 0)
                            {
                                if (convertWood != 0 && toShoot == 1)
                                {
                                    toShoot = convertWood;
                                }

                                if (convertAll != 0)
                                {
                                    toShoot = convertAll;
                                }

                                // This smells like vanilla code and when I tried cleaning it up things broke so LOL
                                if (BowsToOverride[i].ArrowsFired != 0)
                                {
                                    Vector2 position = player.RotatedRelativePoint(player.MountedCenter, true);
                                    float num7 = 0.31415927f;
                                    int num8 = BowsToOverride[i].ArrowsFired;
                                    Vector2 vector2_4 = dir * -1f / trajectoryPlayer.chargeVelocityDivide;
                                    vector2_4.Normalize();
                                    Vector2 spinningpoint = vector2_4 * 40f;
                                    bool flag5 = Collision.CanHit(position, 0, 0, position + spinningpoint, 0, 0);
                                    SoundEngine.PlaySound(new SoundStyle("OvermorrowMod/Sounds/bowShoot"), player.Center);
                                    for (int index = 0; index < num8; index++)
                                    {
                                        float num9 = index - (float)((num8 - 1.0) / 2.0);
                                        Vector2 vector2_5 = spinningpoint.RotatedBy(num7 * num9, default);
                                        if (!flag5)
                                        {
                                            vector2_5 -= spinningpoint;
                                        }
                                        int index2 = Projectile.NewProjectile(null, position.X + vector2_5.X, position.Y + vector2_5.Y, dir.X * -1f / trajectoryPlayer.chargeVelocityDivide, dir.Y * -1f / trajectoryPlayer.chargeVelocityDivide, toShoot, (int)chargeDamageScale, item.knockBack, player.whoAmI, 0f, 0f);
                                        Main.projectile[index2].noDropItem = true;
                                    }
                                }
                                else
                                {
                                    SoundEngine.PlaySound(new SoundStyle("OvermorrowMod/Sounds/bowShoot"), player.Center);
                                    Projectile.NewProjectile(null, playerPos, dir * -1f / trajectoryPlayer.chargeVelocityDivide, toShoot, (int)chargeDamageScale, item.knockBack, player.whoAmI, 0f, 0f);
                                }
                            }
                            else
                            {
                                SoundEngine.PlaySound(new SoundStyle("OvermorrowMod/Sounds/bowDryfire"), player.Center);
                                player.AddBuff(ModContent.BuffType<BowDryfire>(), 120, true);
                            }
                        }

                        trajectoryPlayer.bowTiming = 0;
                    }
                }
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            for (int i = 0; i < BowsToOverride.Count; i++)
            {
                if (item.type == BowsToOverride[i].ItemID && Config.improveBowsSend)
                {
                    TrajectoryPlayer trajectoryPlayer = Main.LocalPlayer.GetModPlayer<TrajectoryPlayer>();
                    for (int lines = 0; lines < tooltips.Count; lines++)
                    {
                        if (tooltips[lines].Name == "Speed")
                        {
                            tooltips.RemoveAt(lines);
                            tooltips.Insert(lines, new TooltipLine(Mod, "Charge",
                                string.Format("Takes {0} {1} to fully charge",
                                (BowsToOverride[i].ChargeTime - (float)trajectoryPlayer.bowTimingReduce) / 60f,
                                ((float)BowsToOverride[i].ChargeTime / 60f == 1f) ? "second" : "seconds")));
                        }
                    }
                }
            }

            if (item.type == ItemID.MagicQuiver && Config.improveBowsSend)
            {
                tooltips.Insert(tooltips.Count, new TooltipLine(Mod, "ChargeReduce", "Reduces bow charge time by 0.5 seconds"));
            }
        }
    }
}
