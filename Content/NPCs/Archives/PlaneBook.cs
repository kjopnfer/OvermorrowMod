using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using Terraria.ID;
using Terraria.GameContent.Bestiary;
using Terraria.UI;
using Terraria.DataStructures;
using OvermorrowMod.Content.Buffs;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Biomes;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using Microsoft.VisualBasic;
using static Terraria.GameContent.PlayerEyeHelper;
using System.Xml.Linq;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class PlaneBook : ModNPC
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + Name;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Position = new Vector2(8, 8),
                PortraitPositionXOverride = 8,
                PortraitPositionYOverride = -6,
                Velocity = 1f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 44;
            NPC.lifeMax = 100;
            NPC.defense = 8;
            NPC.damage = 23;
            NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, silver: 2, copper: 20);

            SpawnModBiomes = [ModContent.GetInstance<GrandArchives>().Type, ModContent.GetInstance<Inkwell>().Type];
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue(LocalizatonPath.Bestiary + Name)),
            });
        }

        public ref float AIState => ref NPC.ai[0];
        public ref float AICounter => ref NPC.ai[1];
        public ref Player player => ref Main.player[NPC.target];
        public enum AICase
        {
            Fall = 0,
            Fly = 1,
            Cast = 2
        }

        public override void AI()
        {
            NPC.TargetClosest();

            AIState = (int)AICase.Fly;
            switch ((AICase)AIState)
            {
                case AICase.Fall:
                    break;
                case AICase.Fly:
                    break;
                case AICase.Cast:
                    break;
            }
            base.AI();
        }

        private void SetFrame()
        {
            if (NPC.IsABestiaryIconDummy) AIState = (int)AICase.Fly;

            switch ((AICase)AIState)
            {
                case AICase.Fall:
                    yFrame = 1;
                    break;
                case AICase.Fly:
                    yFrame = 8;

                    if (NPC.frameCounter++ % 3 == 0)
                    {
                        yFrameWing++;
                        if (yFrameWing >= 3) yFrameWing = 0;
                    }
                    break;
                case AICase.Cast:
                    yFrame = 0;
                    break;
            }
        }

        int xFrame = 0;
        int yFrame = 0;
        int yFrameWing = 0;
        public override void FindFrame(int frameHeight)
        {
            SetFrame();

            NPC.spriteDirection = NPC.direction;
            NPC.frame.Width = TextureAssets.Npc[NPC.type].Value.Width;
            NPC.frame.Height = TextureAssets.Npc[NPC.type].Value.Height / 11;

            NPC.frame.X = NPC.frame.Width * xFrame;
            NPC.frame.Y = NPC.frame.Height * yFrame;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D wingTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "BookWings").Value;

            int wingTextureHeight = (int)(wingTexture.Height / 3f);

            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(Type, out NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers))
            {
                drawModifiers.Position = new Vector2(8, 8);
                drawModifiers.PortraitPositionXOverride = 8;
                drawModifiers.PortraitPositionYOverride = -6;

                // Replace the existing NPCBestiaryDrawModifiers with our new one with an adjusted rotation
                NPCID.Sets.NPCBestiaryDrawOffset.Remove(Type);
                NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            }


            int xOffset = NPC.direction == -1 ? 4 : -10;
            Vector2 drawOffset = new Vector2(xOffset, -28);
            if (NPC.IsABestiaryIconDummy)
            {
                Vector2 bestiaryDrawOffset = new Vector2(-6, -28);
                spriteBatch.Draw(wingTexture, NPC.Center + bestiaryDrawOffset, new Rectangle(0, wingTextureHeight * yFrameWing, wingTexture.Width, wingTextureHeight), drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
                spriteBatch.Draw(texture, NPC.Center + new Vector2(-8, 0), NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
                return false;
            }

            spriteBatch.Draw(wingTexture, NPC.Center + drawOffset - Main.screenPosition, new Rectangle(0, wingTextureHeight * yFrameWing, wingTexture.Width, wingTextureHeight), drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            return false;
        }
    }
}