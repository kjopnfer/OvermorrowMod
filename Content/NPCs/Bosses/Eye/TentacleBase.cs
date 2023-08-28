using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using OvermorrowMod.Common;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Common.Players;

namespace OvermorrowMod.Content.NPCs.Bosses.Eye
{
    public class VortexEye : ModNPC
    {
        private float Rotation;
        public override bool CheckActive() => false;
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eye");
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 32;
            NPC.lifeMax = 640;
            NPC.aiStyle = -1;
            NPC.defense = 16;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.friendly = false;
            NPC.noTileCollide = true;
        }

        public override void DrawBehind(int index)
        {
            NPC.hide = true;
            Main.instance.DrawCacheNPCProjectiles.Add(index);
        }

        public enum AIStates
        {
            Death = -2,
            Panic = -1,
            Idle = 0
        }

        public ref float ParentID => ref NPC.ai[0];
        public ref float AICounter => ref NPC.ai[1];
        public ref float DeathFlag => ref NPC.ai[2];
        public ref float AICase => ref NPC.ai[3];

        public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
        {
            Rotation = NPC.ai[1];
            NPC.ai[1] = 0;

            base.OnSpawn(source);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            if (NPC.life <= 0 && AICase == (float)AIStates.Death)
            {
                var source = NPC.GetSource_Death();
                Gore.NewGore(source, NPC.Center, Vector2.Zero, Mod.Find<ModGore>("EyeStalk").Type, NPC.scale);
            }
        }

        public override void AI()
        {
            NPC parent = Main.npc[(int)ParentID];

            if (parent == null || !parent.active)
            {
                NPC.active = false;
            }

            NPC.Center = ((TentacleBase)parent.ModNPC).lastPosition;
            NPC.rotation = Rotation;

            switch (AICase)
            {
                case (float)AIStates.Panic:
                    if (AICounter++ == 240)
                    {
                        AICase = (float)AIStates.Death;
                        AICounter = 0;
                    }
                    break;
                case (float)AIStates.Death:
                    if (AICounter++ == 120)
                    {
                        NPC.life = 0;
                        NPC.HitEffect(0, 0);
                        DeathFlag = 1;
                        NPC.checkDead();
                    }
                    break;
                case (float)AIStates.Idle:
                    break;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            if (AICase == (float)AIStates.Death)
            {
                Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
                float progress = Utils.Clamp(AICounter, 0, 90) / 90f;
                effect.Parameters["WhiteoutColor"].SetValue(Color.Black.ToVector3());
                effect.Parameters["WhiteoutProgress"].SetValue(progress);
                effect.CurrentTechnique.Passes["Whiteout"].Apply();
            }

            Texture2D eye = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/EyeStalk").Value;
            Main.spriteBatch.Draw(eye, NPC.Center - Main.screenPosition, null, drawColor, NPC.rotation + MathHelper.PiOver2, eye.Size() / 2, NPC.scale, SpriteEffects.None, 0);


            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/EyeStalk_Glow").Value;
            spriteBatch.Draw(texture, NPC.Center - screenPos, null, Color.White, NPC.rotation + MathHelper.PiOver2, texture.Size() / 2, NPC.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.Transform);
        }

        public override bool CheckDead()
        {
            if (DeathFlag == 0)
            {
                AICase = (float)AIStates.Panic;

                NPC.life = NPC.lifeMax;
                NPC.dontTakeDamage = true;
                NPC.netUpdate = true;

                return false;
            }

            return true;
        }

        public override void OnKill()
        {
            NPC parent = Main.npc[(int)ParentID];
            parent.life = 0;
            parent.HitEffect(0, 0);
            parent.checkDead();
        }
    }

    public class TentacleBase : ModNPC
    {
        private NPC child;
        private Segment Tentacle;
        public Vector2 lastPosition;

        public override string Texture => AssetDirectory.Boss + "Eye/EyeOfCthulhu";
        public override void SetDefaults()
        {
            NPC.width = NPC.height = 32;
            NPC.lifeMax = 640;
            NPC.aiStyle = -1;
            NPC.defense = 16;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.friendly = false;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
        }

        public override void DrawBehind(int index)
        {
            NPC.hide = true;
            Main.instance.DrawCacheNPCProjectiles.Add(index);
        }

        public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
        {
            float time = Main.rand.NextFloat(1f);
            float offset = Main.rand.NextFloat(0.1f, 0.15f);
            float rotation = NPC.ai[1];

            Tentacle = new Segment(NPC.Center, 5, rotation, time);
            Segment current = Tentacle;
            for (int i = 0; i < 40; i++)
            {
                time += offset;

                Segment next = new Segment(current, 5, rotation, time);
                current.Child = next;
                current = next;
            }

            // Obtain the last position for the NPC
            Segment nextSegment = Tentacle;
            while (nextSegment != null)
            {
                nextSegment.Move(NPC.ai[2] == 1 ? 0.03f : -0.03f);
                nextSegment.Update();

                lastPosition = nextSegment.EndPoint;
                nextSegment = nextSegment.Child;
            }

            var entitySource = NPC.GetSource_FromAI();
            int index = NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<VortexEye>(), 0, NPC.whoAmI, rotation);

            child = Main.npc[index];

            if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
            {
                NetMessage.SendData(MessageID.SyncNPC, number: index);
            }

            base.OnSpawn(source);
        }

        public override void AI()
        {
            NPC npc = Main.npc[(int)NPC.ai[0]];

            if (!npc.active || npc == null) npc.active = false;

            if (!child.active)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 0);
                NPC.checkDead();
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            if (NPC.life <= 0)
            {
                int counter = 0;

                var source = NPC.GetSource_Death();
                Segment nextSegment = Tentacle;
                while (nextSegment != null)
                {
                    switch (counter % 4)
                    {
                        case 0:
                            Gore.NewGore(source, nextSegment.StartPoint, Vector2.Zero, Mod.Find<ModGore>("StalkBodyL1").Type, NPC.scale);
                            break;
                        case 1:
                            Gore.NewGore(source, nextSegment.StartPoint, Vector2.Zero, Mod.Find<ModGore>("StalkBodyL2").Type, NPC.scale);
                            break;
                        case 2:
                            Gore.NewGore(source, nextSegment.StartPoint, Vector2.Zero, Mod.Find<ModGore>("StalkBodyR1").Type, NPC.scale);
                            break;
                        case 3:
                            Gore.NewGore(source, nextSegment.StartPoint, Vector2.Zero, Mod.Find<ModGore>("StalkBodyR2").Type, NPC.scale);
                            break;
                    }

                    nextSegment = nextSegment.Child;
                    counter++;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            if (child.ai[3] == (float)VortexEye.AIStates.Death)
            {
                /*var deathShader = GameShaders.Misc["OvermorrowMod: DeathAnimation"];

                deathShader.UseOpacity(1f);
                if (child.ai[1] > 30f)
                {
                    deathShader.UseOpacity(1f - (child.ai[1] - 30f) / 90f);
                }

                deathShader.Apply(null);*/
                Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
                float progress = Utils.Clamp(child.ai[1], 0, 90) / 90f;
                effect.Parameters["WhiteoutColor"].SetValue(Color.Black.ToVector3());
                effect.Parameters["WhiteoutProgress"].SetValue(progress);
                effect.CurrentTechnique.Passes["Whiteout"].Apply();
            }

            int counter = 0;

            Segment nextSegment = Tentacle;
            while (nextSegment != null)
            {
                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/StalkBodyL1").Value;
                switch (counter % 4)
                {
                    case 0:
                        texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/StalkBodyL1").Value;
                        break;
                    case 1:
                        texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/StalkBodyL2").Value;
                        break;
                    case 2:
                        texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/StalkBodyR1").Value;
                        break;
                    case 3:
                        texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/StalkBodyR2").Value;
                        break;
                }

                float moveDirection = NPC.ai[2] == 1 ? 1 : -1;
                float rotationSpeed = child.ai[3] == (float)VortexEye.AIStates.Panic ? 0.09f : 0.03f;
                if (child.ai[3] == (float)VortexEye.AIStates.Death) rotationSpeed = MathHelper.Lerp(0.09f, 0, Utils.Clamp(child.ai[1], 0, 60) / 60f);

                if (!Main.gamePaused) nextSegment.Move(rotationSpeed * moveDirection);
                //Color color = counter < 5 ? Color.Transparent : Lighting.GetColor((int)(nextSegment.StartPoint.X / 16), (int)(nextSegment.StartPoint.Y / 16f));
                Color color = Lighting.GetColor((int)(nextSegment.StartPoint.X / 16), (int)(nextSegment.StartPoint.Y / 16f));

                nextSegment.Update();
                nextSegment.Draw(spriteBatch, texture, color);

                lastPosition = nextSegment.EndPoint;
                nextSegment = nextSegment.Child;

                counter++;
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.Transform);


            return false;
        }
    }

    public class DarkVortex : ModNPC
    {
        private List<int> Eyes;
        public override string Texture => AssetDirectory.Textures + "VortexCenter";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("???");
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 256;
            NPC.lifeMax = 640;
            NPC.aiStyle = -1;
            NPC.defense = 16;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.friendly = false;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
        }

        public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
        {
            Eyes = new List<int>();

            for (int i = 0; i < 3; i++)
            {
                float rotation = MathHelper.ToRadians(120f * i);
                float swayDirection = Main.rand.NextBool() ? 1 : -1;

                var entitySource = NPC.GetSource_FromAI();
                Vector2 offset = new Vector2(20, 0).RotatedBy(rotation);
                int index = NPC.NewNPC(entitySource, (int)(NPC.Center.X + offset.X), (int)(NPC.Center.Y + offset.Y) + 16, ModContent.NPCType<TentacleBase>(), 0, NPC.whoAmI, rotation, swayDirection);
                Eyes.Add(index);

                if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: index);
                }
            }
        }

        public ref float AICounter => ref NPC.ai[0];
        public ref float WaveCounter => ref NPC.ai[1];
        public ref float DeathCounter => ref NPC.ai[2];
        public ref float SpookyEye => ref NPC.ai[3];
        public override void AI()
        {
            NPC.rotation += 0.03f;

            bool stayAlive = false;
            foreach (int index in Eyes)
            {
                if (Main.npc[index].active && Main.npc[index].type == ModContent.NPCType<TentacleBase>())
                {
                    stayAlive = true;
                    break;
                }
            }

            foreach (Player player in Main.player)
            {
                if (player.active)
                {
                    if (NPC.Distance(player.Center) < 600)
                    {
                        Filters.Scene.Activate("EyeVortex", player.position);
                    }
                    else
                    {
                        Filters.Scene.Deactivate("EyeVortex", player.position);
                    }
                }
            }

            if (WaveCounter >= 300 && WaveCounter <= 540)
            {
                if (WaveCounter >= 540) WaveCounter = 0;

                if (AICounter++ % 60 == 0 && stayAlive)
                {
                    var entitySource = NPC.GetSource_FromAI();
                    int eye = NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.ServantofCthulhu, 0, -1);

                    Main.npc[eye].velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 5;

                    if (Main.netMode == NetmodeID.Server && eye < Main.maxNPCs)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: eye);
                    }
                }
            }

            if (!stayAlive)
            {
                if (DeathCounter == 0)
                {
                    foreach (Player player in Main.player)
                    {
                        if (NPC.Distance(player.Center) < 900)
                        {
                            player.GetModPlayer<OvermorrowModPlayer>().PlayerFocusCamera(NPC.Center, 120, 300, 120);
                        }
                    }
                }

                if (DeathCounter++ >= 360)
                {
                    if (NPC.scale <= 0)
                    {
                        foreach (Player player in Main.player)
                        {
                            if (NPC.Distance(player.Center) < 600) Filters.Scene.Deactivate("EyeVortex", player.position);
                        }

                        NPC.active = false;
                    }

                    NPC.scale -= 0.005f;
                    if (SpookyEye > 0) SpookyEye--;
                }
                else
                {
                    if (DeathCounter >= 240)
                    {
                        if (SpookyEye < 30) SpookyEye++;
                    }
                }
            }

            WaveCounter++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float increase = MathHelper.Lerp(1.35f, 2.25f, Utils.Clamp(DeathCounter, 0, 240) / 240f);
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Vortex2").Value;
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, null, new Color(60, 3, 79), NPC.rotation * 0.5f, texture.Size() / 2, NPC.scale * increase, SpriteEffects.None, 0);

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "VortexCenter").Value;
            increase = MathHelper.Lerp(1.1f, 2f, Utils.Clamp(DeathCounter, 0, 240) / 240f);
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, null, Color.Black, NPC.rotation, texture.Size() / 2, NPC.scale * increase, SpriteEffects.None, 0);

            if (DeathCounter >= 240)
            {
                texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/Iris").Value;

                Color color = Color.Lerp(Color.Transparent, Color.White, Utils.Clamp(SpookyEye, 0, 30) / 30f);
                spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, null, color, 0f, texture.Size() / 2, 1f, SpriteEffects.None, 0);
            }

            return false;
        }
    }
}
