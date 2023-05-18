using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.NPCs;
using OvermorrowMod.Core;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace OvermorrowMod.Content.NPCs.Seer
{
    public abstract class Seer : Worm
    {
        public override void Init()
        {
            minLength = 7;
            maxLength = 7;
            headType = ModContent.NPCType<SeerHead>();
            bodyType = ModContent.NPCType<SeerBody>();
            tailType = ModContent.NPCType<SeerTail>();
            speed = 4.5f;
            turnSpeed = 0.1f;

            flies = true;
        }
    }

    public class SeerHead : Seer
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seer");

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = AssetDirectory.Bestiary + "Seer_Bestiary",
                Position = new Vector2(24, 0),
                PortraitPositionYOverride = -24f,
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.WanderingEye);
            NPC.damage = 30;
            NPC.defense = 20;
            NPC.lifeMax = 150;
            NPC.width = 32;
            NPC.height = 38;
            NPC.knockBackResist = 2f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCHit53;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

                new FlavorTextBestiaryInfoElement("'Juvenile' copies that have recently detached from an Eye of Cthulhu. As they mature, they shed their tails, becoming larger and more reclusive. If left unchecked, they will grow into Eyes of Cthulhu, capable of producing an endless amount of Demon Eyes.")
            });
        }

        public override void CustomBehavior()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                if (!Main.dayTime)
                {
                    NPC.TargetClosest();
                }
            }

            Player player = Main.player[NPC.target];
            if (player.dead || Main.dayTime)
            {
                NPC.TargetClosest(false);
                AIType = -1;
                NPC.aiStyle = -1;
                NPC.noTileCollide = true;

                // This despawn code is so jank that I have to force the NPC to complete stop and then fly up at mach 10

                NPC.velocity = Vector2.Zero;
                NPC.velocity.Y -= 10f;

                NPC.EncourageDespawn(10);
                return;
            }
        }

        public override void Init()
        {
            base.Init();
            head = true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return 0f;
            //return spawnInfo.Player.ZoneOverworldHeight && !Main.dayTime ? SpawnCondition.OverworldNightMonster.Chance * 0.17f : 0f;
        }
    }

    internal class SeerBody : Seer
    {
        public override string Texture => DistanceFromTail == 5 ? AssetDirectory.NPC + "Seer/EyeStemBulb" : AssetDirectory.NPC + "Seer/SeerBodyAlt";

        //public override string Texture => DistanceFromTail % 2 == 0 ? AssetDirectory.NPC + "Seer/EyeStemBulb" : AssetDirectory.NPC + "Seer/SeerBodyAlt";
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerBody);
            NPC.aiStyle = -1;
            NPC.width = 10;
            NPC.height = 14;
            NPC.damage = 30;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = DistanceFromTail % 2 == 0 ? ModContent.Request<Texture2D>(AssetDirectory.NPC + "Seer/SeerBody").Value : ModContent.Request<Texture2D>(AssetDirectory.NPC + "Seer/SeerBodyAlt").Value;
            texture = DistanceFromTail == 4 ? ModContent.Request<Texture2D>(AssetDirectory.NPC + "Seer/SeerStemBulb").Value : texture;
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, null, drawColor, NPC.rotation, texture.Size() / 2, NPC.scale, SpriteEffects.None, 0);

            return false;
        }
    }

    internal class SeerTail : Seer
    {
        public override string Texture => AssetDirectory.NPC + "Seer/SeerBodyAlt";
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerTail);
            NPC.width = 12;
            NPC.height = 14;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override void Init()
        {
            base.Init();
            tail = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return false;
        }
    }
}