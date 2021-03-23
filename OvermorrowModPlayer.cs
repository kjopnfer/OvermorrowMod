using Microsoft.Xna.Framework;
using OvermorrowMod.Buffs.Debuffs;
using OvermorrowMod.Items.Accessories;
using OvermorrowMod.Projectiles.Accessory;
using OvermorrowMod.Projectiles.Boss;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod
{
    public class OvermorrowModPlayer : ModPlayer
    {
        // Accessories
        public bool BloodyHeart;
        public bool BloodyTeeth;
        public bool DripplerEye;
        public bool ShatteredOrb;
        public bool StormScale;
        public bool StormShield;
        public bool TreeNecklace;

        // Shield Variables
        public int DashType;

        public static readonly int DashRight = 2;
        public static readonly int DashLeft = 3;

        // The direction the player is currently dashing towards.  Defaults to -1 if no dash is ocurring.
        public int DashDir = -1;

        //The fields related to the dash accessory
        public bool DashActive = false;
        public int DashDelay = MAX_DASH_DELAY;
        public int DashTimer = MAX_DASH_TIMER;

        // The initial velocity.  10 velocity is about 37.5 tiles/second or 50 mph
        public readonly float DashVelocity = 10f;
        // These two fields are the max values for the delay between dashes and the length of the dash in that order
        // The time is measured in frames
        public static readonly int MAX_DASH_DELAY = 50;
        public static readonly int MAX_DASH_TIMER = 35;

        // Accessory Counters
        public int dripplerStack;
        private int sparkCounter;
        private int treeCounter;
        private int treeDefenseStack;

        // Buffs
        public bool mirrorBuff;
        public bool moonBuff;
        public bool treeBuff;

        public override void ResetEffects()
        {
            BloodyHeart = false;
            BloodyTeeth = false;
            DripplerEye = false;
            ShatteredOrb = false;
            StormScale = false;
            StormShield = false;
            TreeNecklace = false;

            mirrorBuff = false;
            moonBuff = false;
            treeBuff = false;

            bool dashAccessoryEquipped = false;

            //This is the loop used in vanilla to update/check the not-vanity accessories
            for (int i = 3; i < 8 + player.extraAccessorySlots; i++)
            {
                Item item = player.armor[i];

                //Set the flag for the ExampleDashAccessory being equipped if we have it equipped OR immediately return if any of the accessories are
                // one of the higher-priority ones
                if (item.type == ModContent.ItemType<StormShield>())
                    dashAccessoryEquipped = true;
                else if (item.type == ItemID.EoCShield || item.type == ItemID.MasterNinjaGear || item.type == ItemID.Tabi)
                    return;
            }

            //If we don't have the ExampleDashAccessory equipped or the player has the Solor armor set equipped, return immediately
            //Also return if the player is currently on a mount, since dashes on a mount look weird, or if the dash was already activated
            if (!dashAccessoryEquipped || player.setSolar || player.mount.Active || DashActive)
                return;

            //When a directional key is pressed and released, vanilla starts a 15 tick (1/4 second) timer during which a second press activates a dash
            //If the timers are set to 15, then this is the first press just processed by the vanilla logic.  Otherwise, it's a double-tap
            if (player.controlRight && player.releaseRight && player.doubleTapCardinalTimer[DashRight] < 15)
                DashDir = DashRight;
            else if (player.controlLeft && player.releaseLeft && player.doubleTapCardinalTimer[DashLeft] < 15)
                DashDir = DashLeft;
            else
                return;  //No dash was activated, return

            DashActive = true;
            DashType = 0;
        }

        // In MP, other clients need accurate information about your player or else bugs happen.
        // clientClone, SyncPlayer, and SendClientChanges, ensure that information is correct.
        // We only need to do this for data that is changed by code not executed by all clients, 
        // or data that needs to be shared while joining a world.
        // For example, examplePet doesn't need to be synced because all clients know that the player is wearing the ExamplePet item in an equipment slot. 
        // The examplePet bool is set for that player on every clients computer independently (via the Buff.Update), keeping that data in sync.
        // ExampleLifeFruits, however might be out of sync. For example, when joining a server, we need to share the exampleLifeFruits variable with all other clients.
        // In addition, in ExampleUI we have a button that toggles "Non-Stop Party". We need to sync this whenever it changes.
        public override void clientClone(ModPlayer clientClone)
        {
            OvermorrowModPlayer clone = clientClone as OvermorrowModPlayer;
            // Here we would make a backup clone of values that are only correct on the local players Player instance.
            // Some examples would be RPG stats from a GUI, Hotkey states, and Extra Item Slots
        }


        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (BloodyTeeth)
            {
                int bleedChance = Main.rand.Next(4);
                if (bleedChance == 0 && item.melee)
                {
                    target.AddBuff(ModContent.BuffType<Bleeding>(), 360);
                }
            }
        }

        public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            if (BloodyHeart)
            {
                int projectiles = 3;
                if (Main.netMode != NetmodeID.MultiplayerClient && Main.myPlayer == player.whoAmI)
                {
                    for (int i = 0; i < projectiles; i++)
                    {
                        Projectile.NewProjectile(player.Center, new Vector2(7).RotatedBy(MathHelper.ToRadians((360 / projectiles) * i + i)), ModContent.ProjectileType<BouncingBlood>(), 19, 2, player.whoAmI);
                    }
                }
            }
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            if (mirrorBuff)
            {
                damage /= 2;
            }
            base.ModifyHitByNPC(npc, ref damage, ref crit);
        }

        public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff)
        {
            if (DripplerEye)
            {
                player.rangedCrit += dripplerStack;
            }

            if (StormScale) // Create sparks while moving, increase defense if health is below 50%
            {
                if (player.dashDelay > 0)
                {
                    Main.NewText("player is dashing");
                }

                if (player.statLife <= player.statLifeMax2 * 0.5f)
                {
                    player.statDefense += 5;
                }

                if (player.velocity.X != 0 || player.velocity.Y != 0)
                {
                    if (sparkCounter % 30 == 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient && Main.myPlayer == player.whoAmI)
                        {
                            for (int i = 0; i < Main.rand.Next(1, 3); i++)
                            {
                                Projectile.NewProjectile(player.Center.X, player.Center.Y + Main.rand.Next(-15, 18), 0, 0, ModContent.ProjectileType<ElectricSparksFriendly>(), 20, 1, player.whoAmI, 0, 0);
                            }
                        }
                    }
                    sparkCounter++;
                }
            }

            if (TreeNecklace)
            {
                Lighting.AddLight(player.Center, 0f, 0.75f, 0f);

                // The player is standing still
                if (player.velocity == Vector2.Zero)
                {
                    treeCounter++;
                    if (treeCounter % 60 == 0 && treeDefenseStack <= 15)
                    {
                        treeDefenseStack++;
                    }
                }
                else // Reset the counter
                {
                    treeCounter = 0;
                    treeDefenseStack = 0;
                }

                player.statDefense += treeDefenseStack;
            }

            if (moonBuff)
            {
                player.allDamage += .25f;
            }

            base.UpdateEquips(ref wallSpeedBuff, ref tileSpeedBuff, ref tileRangeBuff);
        }

        public override void UpdateLifeRegen()
        {
            if (treeBuff)
            {
                // lifeRegen is measured in 1/2 life per second. Therefore, this effect causes 2 life gained per second.
                player.lifeRegen += 4;
            }
        }

        public override void PostUpdateRunSpeeds()
        {
            if (player.pulley && DashType > 0)
            {
                DashMovement();
            }
        }

        public override void PostUpdateEquips()
        {
            if(player.mount.Active || player.mount.Cart)
            {
                player.dashDelay = 10;
                DashType = 0;
            }
        }

        public void DashMovement()
        {
            int cShoe = 0;
            if (DashType == 1 && player.eocDash > 0)
            {
                if (player.eocHit < 0)
                {
                    Rectangle dashHitbox = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - 4.0), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
                    for (int i = 0; i < 200; i++)
                    {
                        if (!(Main.npc[i]).active || Main.npc[i].dontTakeDamage || Main.npc[i].friendly || Main.npc[i].immune[(player).whoAmI] > 0)
                        {
                            continue;
                        }

                        NPC npc = Main.npc[i];
                        Rectangle npcHitbox = npc.getRect();
                        if (dashHitbox.Intersects(npcHitbox) && (npc.noTileCollide || player.CanHit(npc)))
                        {
                            float damage = 36f * player.meleeDamage;
                            float knockback = 9f;
                            bool crit = false;

                            if (player.kbGlove)
                            {
                                knockback *= 2f;
                            }

                            if (player.kbBuff)
                            {
                                knockback *= 1.5f;
                            }

                            if (Main.rand.Next(100) < player.meleeCrit)
                            {
                                crit = true;
                            }

                            int direction = player.direction;

                            if (player.velocity.X < 0f)
                            {
                                direction = -1;
                            }
                            if (player.velocity.X > 0f)
                            {
                                direction = 1;
                            }

                            if (player.whoAmI == Main.myPlayer)
                            {
                                player.ApplyDamageToNPC(npc, (int)damage, knockback, direction, crit);
                            }

                            player.eocDash = 10;
                            player.dashDelay = 30;
                            player.velocity.X = (0f - (float)direction) * 9f;
                            player.velocity.Y = -4f;
                            player.immune = true;
                            player.immuneNoBlink = true;
                            player.immuneTime = 4;
                            player.eocHit = i;
                        }
                    }
                }
                else if ((!player.controlLeft || player.velocity.X >= 0f) && (!player.controlRight || player.velocity.X <= 0f))
                {
                    player.velocity.X *= 0.95f;
                }
            }

            if (player.dashDelay > 0)
            {
                if (player.eocDash > 0)
                {
                    player.eocDash--;
                }
                if (player.eocDash == 0)
                {
                    player.eocHit = -1;
                }
                player.dashDelay--;
            }
            else if (player.dashDelay < 0)
            {
                float num47 = 12f;
                float num46 = 0.992f;
                float num45 = Math.Max(player.accRunSpeed, player.maxRunSpeed);
                float num44 = 0.96f;
                int num43 = 20;
                if (player.dash == 1)
                {
                    for (int n = 0; n < 2; n++)
                    {
                        int num42 = (player.velocity.Y != 0f) ? Dust.NewDust(new Vector2(player.position.X, player.position.Y + (float)(player.height / 2) - 8f), player.width, 16, 31, 0f, 0f, 100, default(Color), 1.4f) : Dust.NewDust(new Vector2(player.position.X, player.position.Y + (float)player.height - 4f), player.width, 8, 31, 0f, 0f, 100, default(Color), 1.4f);
                        Main.dust[num42].velocity *= 0.1f;
                        Main.dust[num42].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                        Main.dust[num42].shader = GameShaders.Armor.GetSecondaryShader(cShoe, player);
                    }
                }

                if (player.dash <= 0)
                {
                    return;
                }

                if (player.velocity.X > num47 || player.velocity.X < 0f - num47)
                {
                    player.velocity.X *= num46;
                    return;
                }
                if (player.velocity.X > num45 || player.velocity.X < 0f - num45)
                {
                    player.velocity.X *= num44;
                    return;
                }
                player.dashDelay = num43;
                if (player.velocity.X < 0f)
                {
                    player.velocity.X = 0f - num45;
                }
                else if (player.velocity.X > 0f)
                {
                    player.velocity.X = num45;
                }
            }
            else
            {
                if (player.dash <= 0 || player.mount.Active)
                {
                    return;
                }
                if (player.dash == 1)
                {
                    int num36 = 0;
                    bool flag5 = false;
                    if (player.dashTime > 0)
                    {
                        player.dashTime--;
                    }
                    if (player.dashTime < 0)
                    {
                        player.dashTime++;
                    }
                    if (player.controlRight && player.releaseRight)
                    {
                        if (player.dashTime > 0)
                        {
                            num36 = 1;
                            flag5 = true;
                            player.dashTime = 0;
                        }
                        else
                        {
                            player.dashTime = 15;
                        }
                    }
                    else if (player.controlLeft && player.releaseLeft)
                    {
                        if (player.dashTime < 0)
                        {
                            num36 = -1;
                            flag5 = true;
                            player.dashTime = 0;
                        }
                        else
                        {
                            player.dashTime = -15;
                        }
                    }
                    if (flag5)
                    {
                        player.velocity.X = 16.9f * (float)num36;
                        Point point11 = (player.Center + new Vector2(num36 * player.width / 2 + 2, player.gravDir * (0f - (float)player.height) / 2f + player.gravDir * 2f)).ToTileCoordinates();
                        Point point10 = (player.Center + new Vector2(num36 * player.width / 2 + 2, 0f)).ToTileCoordinates();
                        if (WorldGen.SolidOrSlopedTile(point11.X, point11.Y) || WorldGen.SolidOrSlopedTile(point10.X, point10.Y))
                        {
                            player.velocity.X /= 2f;
                        }
                        player.dashDelay = -1;
                        for (int num35 = 0; num35 < 20; num35++)
                        {
                            int num31 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 31, 0f, 0f, 100, default(Color), 2f);
                            Dust expr_CDB_cp_0 = Main.dust[num31];
                            expr_CDB_cp_0.position.X = expr_CDB_cp_0.position.X + (float)Main.rand.Next(-5, 6);
                            Dust expr_D02_cp_0 = Main.dust[num31];
                            expr_D02_cp_0.position.Y = expr_D02_cp_0.position.Y + (float)Main.rand.Next(-5, 6);
                            Main.dust[num31].velocity *= 0.2f;
                            Main.dust[num31].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                            Main.dust[num31].shader = GameShaders.Armor.GetSecondaryShader(cShoe, player);
                        }
                        int num33 = Gore.NewGore(new Vector2(player.position.X + (float)(player.width / 2) - 24f, player.position.Y + (float)(player.height / 2) - 34f), default(Vector2), Main.rand.Next(61, 64));
                        Main.gore[num33].velocity.X = (float)Main.rand.Next(-50, 51) * 0.01f;
                        Main.gore[num33].velocity.Y = (float)Main.rand.Next(-50, 51) * 0.01f;
                        Main.gore[num33].velocity *= 0.4f;
                        num33 = Gore.NewGore(new Vector2(player.position.X + (float)(player.width / 2) - 24f, player.position.Y + (float)(player.height / 2) - 14f), default(Vector2), Main.rand.Next(61, 64));
                        Main.gore[num33].velocity.X = (float)Main.rand.Next(-50, 51) * 0.01f;
                        Main.gore[num33].velocity.Y = (float)Main.rand.Next(-50, 51) * 0.01f;
                        Main.gore[num33].velocity *= 0.4f;
                    }
                }/*
                else if (dash == 2)
                {
                    int num30 = 0;
                    bool flag4 = false;
                    if (dashTime > 0)
                    {
                        dashTime--;
                    }
                    if (dashTime < 0)
                    {
                        dashTime++;
                    }
                    if (controlRight && releaseRight)
                    {
                        if (dashTime > 0)
                        {
                            num30 = 1;
                            flag4 = true;
                            dashTime = 0;
                        }
                        else
                        {
                            dashTime = 15;
                        }
                    }
                    else if (controlLeft && releaseLeft)
                    {
                        if (dashTime < 0)
                        {
                            num30 = -1;
                            flag4 = true;
                            dashTime = 0;
                        }
                        else
                        {
                            dashTime = -15;
                        }
                    }
                    if (flag4)
                    {
                        velocity.X = 14.5f * (float)num30;
                        Point point9 = (base.Center + new Vector2(num30 * width / 2 + 2, gravDir * (0f - (float)height) / 2f + gravDir * 2f)).ToTileCoordinates();
                        Point point8 = (base.Center + new Vector2(num30 * width / 2 + 2, 0f)).ToTileCoordinates();
                        if (WorldGen.SolidOrSlopedTile(point9.X, point9.Y) || WorldGen.SolidOrSlopedTile(point8.X, point8.Y))
                        {
                            velocity.X /= 2f;
                        }
                        dashDelay = -1;
                        eocDash = 15;
                        for (int num29 = 0; num29 < 0; num29++)
                        {
                            int num28 = Dust.NewDust(new Vector2(position.X, position.Y), width, height, 31, 0f, 0f, 100, default(Color), 2f);
                            Dust expr_1106_cp_0 = Main.dust[num28];
                            expr_1106_cp_0.position.X = expr_1106_cp_0.position.X + (float)Main.rand.Next(-5, 6);
                            Dust expr_112D_cp_0 = Main.dust[num28];
                            expr_112D_cp_0.position.Y = expr_112D_cp_0.position.Y + (float)Main.rand.Next(-5, 6);
                            Main.dust[num28].velocity *= 0.2f;
                            Main.dust[num28].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
                            Main.dust[num28].shader = GameShaders.Armor.GetSecondaryShader(cShield, this);
                        }
                    }
                }*/
            }
        }


        // Synchronization Code
        /*public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
		{
			ModPacket packet = mod.GetPacket();
			packet.Write((byte)ExampleModMessageType.ExamplePlayerSyncPlayer);
			packet.Write((byte)player.whoAmI);
			packet.Write(exampleLifeFruits);
			packet.Write(nonStopParty); // While we sync nonStopParty in SendClientChanges, we still need to send it here as well so newly joining players will receive the correct value.
			packet.Send(toWho, fromWho);
		}

		public override void SendClientChanges(ModPlayer clientPlayer)
		{
			// Here we would sync something like an RPG stat whenever the player changes it.
			ExamplePlayer clone = clientPlayer as ExamplePlayer;
			if (clone.nonStopParty != nonStopParty)
			{
				// Send a Mod Packet with the changes.
				var packet = mod.GetPacket();
				packet.Write((byte)ExampleModMessageType.NonStopPartyChanged);
				packet.Write((byte)player.whoAmI);
				packet.Write(nonStopParty);
				packet.Send();
			}
		}*/


    }
}