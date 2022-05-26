using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.Eye
{
    public class VortexEye : ModNPC
    {
        public override bool CheckActive() => false;
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eye");
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

        public ref float ParentID => ref NPC.ai[0];
        public ref float Rotation => ref NPC.ai[1];
        public override void AI()
        {
            NPC parent = Main.npc[(int)ParentID];

            if (parent == null || !parent.active)
            {
                Main.NewText("nope");
                NPC.active = false;
            }

            NPC.Center = ((TentacleBase)parent.ModNPC).LastPosition;
            NPC.rotation = Rotation;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D eye = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/EyeStalk").Value;
            spriteBatch.Draw(eye, NPC.Center - Main.screenPosition, null, drawColor, NPC.rotation + MathHelper.PiOver2, eye.Size() / 2, NPC.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void OnKill()
        {
            NPC parent = Main.npc[(int)ParentID];
            parent.active = false;
        }
    }

    public class TentacleBase : ModNPC
    {
        private NPC Child;
        private Segment Tentacle;
        public Vector2 LastPosition;

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
            Main.NewText("spawn rotation:" + rotation);

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

                LastPosition = nextSegment.EndPoint;
                nextSegment = nextSegment.Child;
            }

            var entitySource = NPC.GetSource_FromAI();
            int index = NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<VortexEye>(), 0, NPC.whoAmI, rotation);

            Child = Main.npc[index];

            if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
            {
                NetMessage.SendData(MessageID.SyncNPC, number: index);
            }

            base.OnSpawn(source);
        }

        public override void AI()
        {
            NPC npc = Main.npc[(int)NPC.ai[0]];

            if (!npc.active || npc == null || !Child.active) npc.active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
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

                if (!Main.gamePaused) nextSegment.Move(NPC.ai[2] == 1 ? 0.03f : -0.03f);

                nextSegment.Update();
                nextSegment.Draw(spriteBatch, texture);

                LastPosition = nextSegment.EndPoint;
                nextSegment = nextSegment.Child;

                counter++;
            }

            return false;
        }
    }

    public class DarkVortex : ModNPC
    {
        private List<int> Eyes;
        public override string Texture => AssetDirectory.Textures + "VortexCenter";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("???");
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
                Main.NewText("rotation: " + rotation);

                var entitySource = NPC.GetSource_FromAI();
                int index = NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y + 16, ModContent.NPCType<TentacleBase>(), 0, NPC.whoAmI, rotation, swayDirection);
                Eyes.Add(index);

                if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: index);
                }
            }
        }

        public override void AI()
        {
            NPC.rotation += 0.03f;

            bool stayAlive = false;
            foreach (int index in Eyes)
            {
                if (Main.npc[index].active)
                {
                    stayAlive = true;
                    break;
                }
            }

            if (!stayAlive) NPC.active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "VortexCenter").Value;
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, null, Color.Black, NPC.rotation, texture.Size() / 2, NPC.scale, SpriteEffects.None, 0);

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Vortex2").Value;
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, null, Color.Black, NPC.rotation * 0.5f, texture.Size() / 2, NPC.scale * 1.25f, SpriteEffects.None, 0);

            return false;
        }
    }
}
