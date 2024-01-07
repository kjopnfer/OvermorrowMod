using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using OvermorrowMod.Common.Players;

namespace OvermorrowMod.Content.Items.Misc.StrangeSpore
{
    public class StrangeSpore : ModItem
    {
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Yellow;
            Item.width = 22;
            Item.height = 36;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item79;
            Item.mountType = ModContent.MountType<GlimsporeStomper>();
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
    public class GlimsporeStomper : ModMount
    {
        const int ExtraHitboxX = 6;
        int HitboxDir = 0;
        public override void SetMount(Player player, ref bool skipDust)
        {
            /*Main.player[player.whoAmI].dashTime = 0;
            Main.player[player.whoAmI].dash = 0;
            Main.player[player.whoAmI].velocity.X = 0;*/
            /*player.position.X = player.oldPosition.X;
            player.velocity.X = 0;
            player.dash = 0;*/
            HitboxDir = player.direction;
            player.Hitbox = new Rectangle(player.Hitbox.X /*- ExtraHitboxX*/, player.Hitbox.Y, player.Hitbox.Width + ExtraHitboxX, player.Hitbox.Height);
        }
        public override void SetStaticDefaults()
        {
            MountData.spawnDust = ModContent.DustType<Dusts.GlimsporeDust>();
            MountData.buff = ModContent.BuffType<Buffs.GlimsporeStomperBuff>();

            MountData.heightBoost = 60;
            MountData.fallDamage = 0.25f;
            MountData.dashSpeed = 0f;
            MountData.blockExtraJumps = true; //false
            List<int> MountedCenterPerFrame = new List<int>();
            for (int i = 0; MountData.heightBoost > i; i++)
                MountedCenterPerFrame.Add(55);
            int[] aaaa = { 64, 62, 64, 68, 68, 62, 64, 68, 68, 58, 68 };
            MountData.playerYOffsets = aaaa;
            MountData.xOffset = 16;
            MountData.yOffset = 10;
            MountData.bodyFrame = 3;

            MountData.playerHeadOffset = 64;
            MountData.flightTimeMax = 0;
            MountData.totalFrames = 11;


            MountData.standingFrameStart = 0;
            MountData.standingFrameCount = 1;
            MountData.standingFrameDelay = 12;

            MountData.runningFrameStart = 1;
            MountData.runningFrameCount = 8;
            MountData.runningFrameDelay = 16;

            MountData.flyingFrameCount = 0;
            MountData.flyingFrameDelay = 0;
            MountData.flyingFrameStart = 0;

            MountData.inAirFrameCount = 1;
            MountData.inAirFrameDelay = 12;
            MountData.inAirFrameStart = 10;

            MountData.idleFrameCount = 1;
            MountData.idleFrameDelay = 12;
            MountData.idleFrameStart = 0;

            MountData.swimFrameCount = 0;
            MountData.swimFrameDelay = 0;
            MountData.swimFrameStart = 0;
            MountData.frontTextureGlow = ModContent.Request<Texture2D>("OvermorrowMod/Content/Items/Misc/StrangeSpore/GlimsporeStomper_Front_Glow");

            MountData.runSpeed = 6.7f;
            MountData.acceleration = 0.1f;

            if (Main.netMode == NetmodeID.Server)
                return;

            MountData.textureWidth = MountData.frontTexture.Width();
            MountData.textureHeight = MountData.frontTexture.Height();


        }
        float JumpCharge = 0;
        bool DidStomp = false;
        int AirTime = 0;
        static bool JumpButtonFromStomp = false;
        static bool CanStomp = false;
        bool DontResetPos = false;
        int AttackDelay = 0;
        public override void UseAbility(Player player, Vector2 mousePosition, bool toggleOn)
        {
            //player.delayUseItem = true;
            if (player.controlUseItem && AttackDelay <= 0)
            {
                SoundEngine.PlaySound(SoundID.NPCHit25, player.MountedCenter);
                for (int i = 0; Main.rand.Next(5, 8) > i; i++)
                    Projectile.NewProjectile(null, player.Center, new Vector2((1.25f * player.velocity.X + Main.rand.Next(10, 16)) * player.direction, 0).RotatedByRandom(MathHelper.ToRadians(22.5f)), ModContent.ProjectileType<Projectiles.GlimsporeBomb>(), 30, 0, player.whoAmI);
                AttackDelay = 60;
            }
            AttackDelay--;
        }
        Vector2 GetMountCenter(Player player) => player.Center + new Vector2(16 * player.direction, 0) - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
        public override bool UpdateFrame(Player mountedPlayer, int state, Vector2 velocity)
        {
            UpdateHitbox(mountedPlayer);
            mountedPlayer.GetModPlayer<OvermorrowModPlayer>().ScreenPos = GetMountCenter(mountedPlayer);

            return base.UpdateFrame(mountedPlayer, state, velocity);
        }
        public override void UpdateEffects(Player player)
        {
            if (glow > 0f)
                glow -= 0.05f;
            if (!CanStomp && !player.controlDown)
                CanStomp = true;
            if (!player.controlDown)
                JumpButtonFromStomp = false;
            player.maxFallSpeed = 40;
            if (player.controlDown && player.velocity.Y == 0 && !JumpButtonFromStomp)
            {
                if (!DontResetPos)
                {
                    player.position.X = player.oldPosition.X;
                }
                player.velocity.X = 0;
                if (JumpCharge < 20)
                {
                    if (JumpCharge == 0)
                        SoundEngine.PlaySound(new SoundStyle($"OvermorrowMod/Sounds/GlimsporeSquish"), player.Center);
                    //Main.NewText(JumpCharge, Color.Red);
                    JumpCharge += 0.25f;
                    // deff do new effect
                    if (JumpCharge % 0.75f == 0)
                    {
                        float Radius = Main.rand.Next(20, 31) * 3;
                        float Rotation = MathHelper.ToRadians(Main.rand.Next(360));
                        Vector2 EffectPos = new Vector2((MountData.textureWidth / 2) + (player.direction == 1 ? 24 : 0) + player.Hitbox.X + ((float)Math.Cos(Rotation) * Radius), player.MountedCenter.Y + ((float)Math.Sin(Rotation) * Radius));
                        NPC.NewNPC(null, (int)EffectPos.X, (int)EffectPos.Y + 60, ModContent.NPCType<NPCs.ChargeEffect>());
                    }
                    if (JumpCharge == 20)
                    {
                        glow = 1f;
                        SoundEngine.PlaySound(SoundID.MaxMana, player.MountedCenter);
                        /*float Radius = 15f;
                        const int MaxCircles = 8;
                        for (int i = 0; MaxCircles > i; i++)
                        {
                            float Rotation = (MathHelper.TwoPi / MaxCircles) * i;
                            Vector2 EffectPos = new Vector2((MountData.textureWidth / 2) + 24 + player.Hitbox.X + ((float)Math.Cos(Rotation) * Radius), player.MountedCenter.Y + ((float)Math.Sin(Rotation) * Radius));
                            NPC.NewNPC(null, (int)EffectPos.X, (int)EffectPos.Y + 60, ModContent.NPCType<NPCs.ChargeEffect>(), ai1:1, ai2:Rotation);
                        }*/
                    }
                }
                if (player.velocity.Y == 0 && player.controlJump && JumpCharge > 0)
                {
                    CanStomp = false;
                    //SoulPlayer.DoScreenShake(7, new Point(2, 2));
                    AirTime = 0;
                    player.velocity.Y -= JumpCharge + 10;
                    SoundEngine.PlaySound(new SoundStyle($"OvermorrowMod/Sounds/WooshJump") { Volume = 1.25f, PitchVariance = 1.1f }, player.Center);
                    //Main.NewText(JumpCharge, Color.Green);
                    JumpCharge = 0;
                }
            }
            else
            {
                JumpCharge = 0;
                if (player.controlJump && player.velocity.Y == 0)
                {
                    player.velocity.Y -= JumpCharge + 10;
                    SoundEngine.PlaySound(new SoundStyle($"OvermorrowMod/Sounds/WooshJump") { Volume = 1.25f, PitchVariance = 1.1f }, player.Center);
                }
            }
            if (player.controlDown && player.velocity.Y != 0 && !DidStomp && !JumpButtonFromStomp && CanStomp)
            {
                player.velocity.Y = 60;
                JumpButtonFromStomp = true;
                DidStomp = true;
            }

            if (DidStomp)
            {
                player.GetModPlayer<OvermorrowModPlayer>().DashShadow = true;
                player.velocity.X = 0;
                AirTime++;
            }
            if (player.velocity.Y == 0)
            {
                if (DidStomp && AirTime > 5)
                {
                    player.GetModPlayer<OvermorrowModPlayer>().AddScreenShake(15, 6);
                    SoundEngine.PlaySound(SoundID.Item14, player.MountedCenter);
                    int[] DustGores = { GoreID.ChimneySmoke1, GoreID.ChimneySmoke2, GoreID.ChimneySmoke3, 220, 221, 222, 11, 12, 13, 61, 62, 63 };
                    Rectangle AoE = new Rectangle((int)player.Center.X - 150, (int)player.MountedCenter.Y, 300, 200);
                    for (int i = 0; Main.npc.Length > i; i++)
                    {
                        if (AoE.Contains(Main.npc[i].position.ToPoint()))
                        {
                            Main.npc[i].velocity.Y = -10;
                            if (!Main.npc[i].townNPC)
                                Main.npc[i].StrikeNPC(new NPC.HitInfo() { Damage = 100, Knockback = 20f, HitDirection = 0 });
                        }
                    }
                    for (int i = AoE.X; AoE.X + AoE.Width > i; i++)
                    {
                        if (i % 10 == 0)
                        {
                            for(int j = AoE.Y; AoE.Y + AoE.Height > j; j++)
                            {
                                Vector2 Pos = new Vector2(i, j);
                                Tile tile = Framing.GetTileSafely(Pos.ToTileCoordinates16());
                                if (tile.HasTile && Main.tileSolid[tile.TileType])
                                {
                                    //Gore.NewGore(Pos, new Vector2(0, -1), Main.rand.Next(825, 828), 1);
                                    //Dust.NewDustPerfect(Pos, ModContent.DustType<Dusts.Steam>());
                                    NPC.NewNPC(null, (int)Pos.X, (int)Pos.Y, ModContent.NPCType<NPCs.GamerDust>());
                                    /*for (int k = 0; 1 > k; k++)
                                        Collision.HitTiles(Pos, new Vector2(Main.rand.Next(-2, 3), -15), 50, 50);*/
                                    break;
                                }
                            }
                        }
                    }
                }
                DidStomp = false;
                player.eocDash = 0;
            }
        }
        void UpdateHitbox(Player player)
        {
            DontResetPos = false;
            if (player.direction != HitboxDir)
            {
                DontResetPos = true;
                if (player.direction > HitboxDir)
                {
                    HitboxDir = player.direction;
                    player.position.X -= 32;
                }
                else
                {
                    HitboxDir = player.direction;
                    player.position.X += 32;
                }
            }
        }
        public override void Dismount(Player player, ref bool skipDust)
        {
            UpdateHitbox(player);
            player.Hitbox = new Rectangle(player.Hitbox.X /*+ ExtraHitboxX*/, player.Hitbox.Y, player.Hitbox.Width - ExtraHitboxX, player.Hitbox.Height);
            player.eocDash = 0;
            player.GetModPlayer<OvermorrowModPlayer>().ScreenPos = null;
        }
        public static float glow = 0f;
        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            spriteEffects = SpriteEffects.None;
            if (drawPlayer.direction == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            if (drawPlayer.velocity.Y == 0 && drawPlayer.controlDown && !JumpButtonFromStomp)
            {
                frame = new Rectangle(0, 9 * 86, 64, 86);
            }
            frame.Height -= 2;
            //Main.PlayerRenderer.DrawPlayer(Main.Camera, drawPlayer, drawPlayer.position, 0f, Vector2.Zero);
            return true;
        }

        private void Mount_Draw(Terraria.On_Mount.orig_Draw orig, Mount self, List<DrawData> playerDrawData, int drawType, Player drawPlayer, Vector2 Position, Color drawColor, SpriteEffects playerEffect, float shadow)
        {
            Main.spriteBatch.End();
            Effect effect = ModContent.Request<Effect>("OvermorrowMod/Effects/Flash").Value;
            effect.Parameters["uIntensity"].SetValue(GlimsporeStomper.glow);
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.Transform);
        }

        /*public override void JumpHeight(Player mountedPlayer, ref int jumpHeight, float xVelocity)
        {
            jumpHeight = 10;
            if (mountedPlayer.velocity.Y == 0)
                jumpHeight = 0;
        }
        public override void JumpSpeed(Player mountedPlayer, ref float jumpSeed, float xVelocity)
        {
            jumpSeed = 7f;
            if (mountedPlayer.velocity.Y == 0)
                jumpSeed = 0f;
        }*/
    }
    /*public class StomperGlow : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.HasBuff<Buffs.GlimsporeStomperBuff>();
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.MountBack);
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.Transform);
        }
    }*/
    /*public class StomperRemoveGlow : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.HasBuff<Buffs.GlimsporeStomperBuff>();
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.MountFront);
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.Transform);
        }
    }*/
}