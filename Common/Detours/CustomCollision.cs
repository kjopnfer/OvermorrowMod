using Microsoft.Xna.Framework;
using OvermorrowMod.Common.NPCs;
using OvermorrowMod.Common.Players;
using OvermorrowMod.Content.NPCs.Bosses.SandstormBoss;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Detours
{
    public class CustomCollision
    {
        public static void Player_UpdateNPCCollision(On.Terraria.Player.orig_Update_NPCCollision orig, Player self)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                // We don't want to include Moving Platforms for collision because this detour will block the FallThrough needed
                if (npc.ModNPC is CollideableNPC && npc.active && npc.ModNPC != null && !(npc.ModNPC is MovingPlatform))
                {
                    Rectangle PlayerBottom = new Rectangle((int)self.position.X, (int)self.position.Y + self.height, self.width, 1);
                    Rectangle NPCTop = new Rectangle((int)npc.position.X, (int)npc.position.Y - (int)npc.velocity.Y, npc.width, 8 + (int)Math.Max(self.velocity.Y, 0));

                    if (PlayerBottom.Intersects(NPCTop))
                    {
                        if (self.position.Y <= npc.position.Y && !self.justJumped && self.velocity.Y >= 0)
                        {
                            self.gfxOffY = npc.gfxOffY;
                            self.position.Y = npc.position.Y - self.height + 4;
                            self.velocity.Y = 0;

                            self.position += npc.velocity;
                            self.fallStart = (int)(self.position.Y / 16f);

                            if (self == Main.LocalPlayer)
                                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, Main.LocalPlayer.whoAmI);

                            if (npc.type == ModContent.NPCType<Pillar>())
                            {
                                self.Hurt(PlayerDeathReason.ByCustomReason(self.name + " had an obelisk stuck up their ass"), 20, -1);
                            }

                            orig(self);
                        }
                    }


                    Rectangle PlayerLeft = new Rectangle((int)self.position.X, (int)self.position.Y, 1, self.height);
                    Rectangle PlayerRight = new Rectangle((int)self.position.X + self.width, (int)self.position.Y, 1, self.height);

                    Rectangle NPCRight = new Rectangle((int)npc.position.X + npc.width, (int)npc.position.Y, 8 + (int)Math.Max(self.velocity.X, 0), npc.height);
                    Rectangle NPCLeft = new Rectangle((int)npc.position.X, (int)npc.position.Y, 10 - (int)Math.Max(self.velocity.X, 0), npc.height);

                    if (PlayerLeft.Intersects(NPCRight))
                    {
                        if (self.position.X >= npc.position.X + npc.width && self.velocity.X <= 0)
                        {
                            int offset = npc.ModNPC is PushableNPC ? 7 : 2;
                            self.velocity.X = 0;
                            self.position.X = npc.position.X + npc.width + offset;

                            if (self == Main.LocalPlayer)
                                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, Main.LocalPlayer.whoAmI);

                            //orig(self);
                        }

                        if (npc.ModNPC is PushableNPC)
                        {
                            npc.position.X -= 1;
                        }
                    }

                    if (PlayerRight.Intersects(NPCLeft))
                    {
                        if (npc.ModNPC is PushableNPC)
                        {
                            npc.position.X += 1;
                        }

                        if (self.position.X <= npc.position.X && self.velocity.X >= 0)
                        {
                            int offset = npc.ModNPC is PushableNPC ? 2 : 0;

                            self.velocity.X = 0;
                            self.position.X = npc.position.X - self.width - offset;

                            if (self == Main.LocalPlayer)
                                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, Main.LocalPlayer.whoAmI);

                            //orig(self);
                        }
                    }

                    Rectangle PlayerTop = new Rectangle((int)self.position.X, (int)self.position.Y, self.width, 1);
                    Rectangle NPCBottom = new Rectangle((int)npc.position.X, (int)npc.position.Y + npc.height + (int)npc.velocity.Y, npc.width, 8 + (int)Math.Max(self.velocity.Y, 0));
                    if (PlayerTop.Intersects(NPCBottom))
                    {
                        if (self.position.Y <= npc.position.X && self.velocity.Y <= 0)
                        {
                            self.gfxOffY = npc.gfxOffY;
                            self.position.Y = npc.position.Y + npc.height + 4;
                            self.position += npc.velocity;
                            self.velocity.Y = 0;

                            if (self == Main.LocalPlayer)
                                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, Main.LocalPlayer.whoAmI);
                        }
                    }

                }
            }

            orig(self);
        }

        // Detour for moving platforms
        public static void Player_PlatformCollision(On.Terraria.Player.orig_SlopingCollision orig, Player self, bool fallThrough, bool ignorePlats)
        {
            if (self.GetModPlayer<OvermorrowModPlayer>().PlatformTimer > 0)
            {
                orig(self, fallThrough, ignorePlats);
                return;
            }

            if (self.GoingDownWithGrapple)
            {
                if (self.grapCount == 1)
                {
                    foreach (int PlayerGrappleIndex in self.grappling)
                    {
                        if (PlayerGrappleIndex < 0 || PlayerGrappleIndex > Main.maxProjectiles)
                            continue;

                        Projectile GrappleHook = Main.projectile[PlayerGrappleIndex];

                        foreach (NPC npc in Main.npc)
                        {
                            if (!npc.active || npc.ModNPC == null || !(npc.ModNPC is MovingPlatform))
                                continue;

                            if (GrappleHook.active && npc.Hitbox.Intersects(GrappleHook.Hitbox) && self.Hitbox.Intersects(GrappleHook.Hitbox))
                            {
                                self.position = GrappleHook.position + new Vector2(GrappleHook.width / 2 - self.width / 2, GrappleHook.height / 2 - self.height / 2);
                                self.position += npc.velocity;

                                self.velocity.Y = 0;
                                self.jump = 0;

                                self.fallStart = (int)(self.position.Y / 16f);
                            }
                        }
                    }
                }

                orig(self, fallThrough, ignorePlats);
                return;
            }

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active || npc.ModNPC == null || !(npc.ModNPC is MovingPlatform))
                    continue;

                Rectangle PlayerRect = new Rectangle((int)self.position.X, (int)self.position.Y + (self.height), self.width, 1);
                Rectangle NPCRect = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, 8 + (self.velocity.Y > 0 ? (int)self.velocity.Y : 0));

                if (self.grapCount == 1 && npc.velocity.Y != 0)
                {
                    //if the player is using a single grappling hook we can check if they are colliding with it and its embedded in the moving platform, while its changing Y position so we can give the player their jump back
                    foreach (int eachGrappleIndex in self.grappling)
                    {
                        if (eachGrappleIndex < 0 || eachGrappleIndex > Main.maxProjectiles)//somehow this can be invalid at this point?
                            continue;

                        Projectile grappleHookProj = Main.projectile[eachGrappleIndex];
                        if (grappleHookProj.active && npc.Hitbox.Intersects(grappleHookProj.Hitbox) && self.Hitbox.Intersects(grappleHookProj.Hitbox))
                        {
                            self.position = grappleHookProj.position + new Vector2(grappleHookProj.width / 2 - self.width / 2, grappleHookProj.height / 2 - self.height / 2);
                            self.position += npc.velocity;

                            self.velocity.Y = 0;
                            self.jump = 0;

                            self.fallStart = (int)(self.position.Y / 16f);
                        }
                    }
                }
                else if (PlayerRect.Intersects(NPCRect) && self.position.Y <= npc.position.Y)
                {
                    if (!self.justJumped && self.velocity.Y >= 0)
                    {
                        if (fallThrough) self.GetModPlayer<OvermorrowModPlayer>().PlatformTimer = 10;

                        self.position.Y = npc.position.Y - self.height + 4;
                        self.position += npc.velocity;

                        self.fallStart = (int)(self.position.Y / 16f);

                        self.velocity.Y = 0;
                        self.jump = 0;
                        self.gfxOffY = npc.gfxOffY;

                    }
                }
            }

            orig(self, fallThrough, ignorePlats);
        }
    }
}