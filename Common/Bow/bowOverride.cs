using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Overmorrow.Common;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Buffs.Debuffs;
using OvermorrowMod.Content.UI;
using OvermorrowMod.Content.Projectiles.Misc;

namespace OvermorrowMod.Common.Bow
{
	public class bowOverride : GlobalItem
	{
		public override bool InstancePerEntity => true;

		private bool channelCheck;
		private bool bowDrawCheck;
		private bool fullChargeCheck;
		private int BowChargeTime = 180;
		private float chargeDamageScale;
		public static int convertWood2;
		public static int convertAll2;
		public static List<int[]> BowsToOverride = new List<int[]>();
		public static List<int[]> ModBowsToOverride = new List<int[]>();
		public List<int> ShittyAmmoConservationThing = new List<int>();

		public int DoShittyAmmoConservationThing()
		{
			Player player = Main.LocalPlayer;
			//thanks ilspy very cool
			return (player.magicQuiver ? 20 : 0) + (player.HasBuff(93) ? 20 : 0) + (player.HasBuff(112) ? 20 : 0) + ((player.armor[1].type == ItemID.ShroomiteBreastplate) ? 20 : 0) + ((player.armor[0].type == ItemID.ChlorophyteHelmet) ? 20 : 0) + ((player.armor[1].type == ItemID.HuntressAltShirt) ? 20 : 0) + ((player.armor[1].type == ItemID.VortexBreastplate) ? 25 : 0) + ((player.armor[1].type == ItemID.HuntressJerkin) ? 10 : 0) + ((player.setBonus == Language.GetTextValue("ArmorSetBonus.Fossil")) ? 20 : 0) + ((player.setBonus == Language.GetTextValue("ArmorSetBonus.CobaltRanged")) ? 20 : 0) + ((player.setBonus == Language.GetTextValue("ArmorSetBonus.MythrilRanged")) ? 20 : 0) + ((player.setBonus == Language.GetTextValue("ArmorSetBonus.AdamantiteRanged")) ? 25 : 0) + ((player.setBonus == Language.GetTextValue("ArmorSetBonus.Titanium") && player.armor[0].type == ItemID.TitaniumHelmet) ? 25 : 0);
		}

		public static void AddBowsToOverride()
		{
			//ItemID, ConvertAll2, ConvertWood2, ChargeTime, AroowsFiredAtOnce, NoConsumeChance
			BowsToOverride.Clear();
			BowsToOverride.Add(new int[6] { ItemID.WoodenBow, 0, 0, 180, 1, 0 });
			BowsToOverride.Add(new int[6] { ItemID.BorealWoodBow, 0, 0, 180, 1, 0 });
			BowsToOverride.Add(new int[6] { ItemID.CopperBow, 0, 0, 180, 1, 0 });
			BowsToOverride.Add(new int[6] { ItemID.PalmWoodBow, 0, 0, 180, 1, 0 });
			BowsToOverride.Add(new int[6] { ItemID.RichMahoganyBow, 0, 0, 180, 1, 0 });
			BowsToOverride.Add(new int[6] { ItemID.TinBow, 0, 0, 180, 1, 0 });
			BowsToOverride.Add(new int[6] { ItemID.EbonwoodBow, 0, 0, 180, 1, 0 });
			BowsToOverride.Add(new int[6] { ItemID.IronBow, 0, 0, 180, 1, 0 });
			BowsToOverride.Add(new int[6] { ItemID.ShadewoodBow, 0, 0, 180, 1, 0 });
			BowsToOverride.Add(new int[6] { ItemID.LeadBow, 0, 0, 165, 1, 0 });
			BowsToOverride.Add(new int[6] { ItemID.PearlwoodBow, 0, 0, 165, 1, 0 });
			BowsToOverride.Add(new int[6] { ItemID.SilverBow, 0, 0, 165, 1, 0 });
			BowsToOverride.Add(new int[6] { ItemID.TungstenBow, 0, 0, 165, 1, 0 });
			BowsToOverride.Add(new int[6] { ItemID.GoldBow, 0, 0, 150, 1, 0 });
			BowsToOverride.Add(new int[6] { ItemID.PlatinumBow, 0, 0, 150, 1, 0 });
			BowsToOverride.Add(new int[6] { ItemID.DemonBow, 0, 0, 150, 1, 0 });
			BowsToOverride.Add(new int[6] { ItemID.TendonBow, 0, 0, 150, 1, 0 });
			BowsToOverride.Add(new int[6] { ItemID.HellwingBow, 0, ProjectileID.Hellwing, 135, 1, 0 });
			BowsToOverride.Add(new int[6] { ItemID.BeesKnees, 0, ProjectileID.BeeArrow, 135, 1, 0 });
			BowsToOverride.Add(new int[6] { ItemID.MoltenFury, 0, ProjectileID.FireArrow, 135, 1, 0 });
			BowsToOverride.Add(new int[6] { ItemID.DD2PhoenixBow, ProjectileID.DD2PhoenixBowShot, 0, 90, 1, 0 });
			BowsToOverride.Add(new int[6] { ItemID.IceBow, ProjectileID.FrostArrow, 0, 120, 1, 0 });
			BowsToOverride.Add(new int[6] { ItemID.ShadowFlameBow, ProjectileID.ShadowFlameArrow, 0, 120, 1, 0 });
			BowsToOverride.Add(new int[6] { ItemID.Marrow, ProjectileID.BoneArrow, 0, 120, 1, 0 });
			BowsToOverride.Add(new int[6] { ItemID.Phantasm, 0, ProjectileID.PhantasmArrow, 60, 7, 66 });
			BowsToOverride.Add(new int[6] { ItemID.Tsunami, 0, 0, 60, 5, 0 });
			BowsToOverride.Add(new int[6] { ItemID.DD2BetsyBow, ProjectileID.DD2BetsyArrow, 0, 60, 3, 0 });
			BowsToOverride.Add(new int[6] { ItemID.PulseBow, ProjectileID.PulseBolt, 0, 105, 1, 0 });
			//if (RadiantShadows.shid)
			//{
				int i = 0;
				while (OvermorrowModFile.ModBowsToOverride.Count > i)
				{
					BowsToOverride.Add(new int[] { OvermorrowModFile.ModBowsToOverride[i][0], OvermorrowModFile.ModBowsToOverride[i][1], OvermorrowModFile.ModBowsToOverride[i][2], OvermorrowModFile.ModBowsToOverride[i][3], OvermorrowModFile.ModBowsToOverride[i][4], OvermorrowModFile.ModBowsToOverride[i][5]});
					i++;
				}
			//}
		}
		/*public virtual void AddModBows()
		{
			BowsToOverride.Add(new int[] { ModContent.ItemType<Testing.testSemiCoolBow>(), 0, 0, 120, 3, 0 });
			BowsToOverride.Add(new int[] { ModContent.ItemType<Slingshot>(), ModContent.ProjectileType<RandBird>(), 0, 120, 1, 0 });
		}*/

		public override void SetDefaults(Item item)
		{
			AddBowsToOverride();
			//AddModBows();
			int g = 0;
			while (BowsToOverride.Count > g)
			{
				if (item.type == BowsToOverride[g][0])
				{
					item.channel = true;
					convertAll2 = BowsToOverride[g][1];
					convertWood2 = BowsToOverride[g][2];
				}
				g++;
			}
		}

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
			int g = 0;
			while (BowsToOverride.Count > g)
			{
				if (item.type == BowsToOverride[g][0] && Config.improveBowsSend)
				{
					damage.Flat += (item.damage * 7);
				}
				g++;
			}
		}

		public override void UpdateInventory(Item item, Player player)
		{
			Main.LocalPlayer.GetModPlayer<TrajectoryPlayer>();
			int g = 0;
			while (BowsToOverride.Count > g)
			{
				if (item.type == BowsToOverride[g][0])
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
						item.CloneDefaults(BowsToOverride[g][0]);
					}
				}
				g++;
			}
		}

		public override bool CanUseItem(Item item, Player player)
		{
			int g = 0;
			while (BowsToOverride.Count > g)
			{
				if (Config.improveBowsSend && item.type == BowsToOverride[g][0])
				{
					return !player.HasBuff(ModContent.BuffType<BowDryfire>()) && !player.dead && ((player.channel && !player.dead) || (!bowChargeUI.mouseHoverCharge && !bowChargeUI.dragging));
				}
				g++;
			}

			return true;
		}

		public override void HoldItem(Item item, Player player)
		{
			int f = 0;

			while (BowsToOverride.Count > f)
			{
				if (item.type == BowsToOverride[f][0] && Config.improveBowsSend)
				{
					SetDefaults(item);
					Vector2 playerPos = new Vector2(player.Center.X, player.Center.Y);
					TrajectoryPlayer trajectoryPlayer = Main.LocalPlayer.GetModPlayer<TrajectoryPlayer>();
					trajectoryPlayer.drawChargeBar = true;
					trajectoryPlayer.bowTimingReduce = (player.magicQuiver ? 15 : 0);
					trajectoryPlayer.bowTimingMax = BowsToOverride[f][3] - trajectoryPlayer.bowTimingReduce;
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
							player.direction = ((Main.MouseWorld.X > Main.screenWidth / 2) ? -1 : 1);
							player.itemRotation = (float)Math.Atan2((distanceY * -1f * player.direction), (distanceX * -1f * player.direction));
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
								int g = 0;
								while (player.inventory.Length > g)
								{
									if (player.inventory[g].ammo == AmmoID.Arrow)
									{
										toShoot = player.inventory[g].shoot;
										int damage = player.inventory[g].damage;
										if (!player.inventory[g].consumable)
										{
											break;
										}
										DontConsumeChance += DoShittyAmmoConservationThing() + BowsToOverride[f][5];
										if (Main.rand.Next(1, 101) > DontConsumeChance)
										{
											player.inventory[g].stack--;
											break;
										}
										break;
									}
									else
									{
										g++;
									}
								}
							}
							else
							{
								int g2 = 54;
								while (58 > g2)
								{
									if (player.inventory[g2].ammo == AmmoID.Arrow)
									{
										toShoot = player.inventory[g2].shoot;
										int damage2 = player.inventory[g2].damage;
										if (!player.inventory[g2].consumable)
										{
											break;
										}
										DontConsumeChance += DoShittyAmmoConservationThing() + BowsToOverride[f][5];
										if (Main.rand.Next(1, 101) > DontConsumeChance)
										{
											player.inventory[g2].stack--;
											break;
										}
										break;
									}
									else
									{
										g2++;
									}
								}
							}

							if ((int)this.chargeDamageScale != 0)
							{
								if (convertWood2 != 0 && toShoot == 1)
								{
									toShoot = convertWood2;
								}

								if (convertAll2 != 0)
								{
									toShoot = convertAll2;
								}

								if (BowsToOverride[f][4] != 0)
								{
									Vector2 position = player.RotatedRelativePoint(player.MountedCenter, true);
									float num7 = 0.31415927f;
									int num8 = BowsToOverride[f][4];
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

				f++;
			}
		}
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			int f = 0;
			while (BowsToOverride.Count > f)
			{
				if (item.type == BowsToOverride[f][0] && Config.improveBowsSend)
				{
					TrajectoryPlayer trajectoryPlayer = Main.LocalPlayer.GetModPlayer<TrajectoryPlayer>();
					int i = 0;
					while (tooltips.Count > i)
					{
						if (tooltips[i].Name == "Speed")
						{
							tooltips.RemoveAt(i);
							tooltips.Insert(i, new TooltipLine(Mod, "Charge", string.Format("Takes {0} {1} to fully charge", (BowsToOverride[f][3] - (float)trajectoryPlayer.bowTimingReduce) / 60f, ((float)BowsToOverride[f][3] / 60f == 1f) ? "second" : "seconds")));
						}
						i++;
					}
				}
				f++;
			}

			if (item.type == ItemID.MagicQuiver && Config.improveBowsSend)
			{
				tooltips.Insert(tooltips.Count, new TooltipLine(Mod, "ChargeReduce", "Reduces bow charge time by 0.5 seconds"));
			}
		}
	}
}
