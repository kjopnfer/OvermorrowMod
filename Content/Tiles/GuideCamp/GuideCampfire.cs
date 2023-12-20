using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Quests;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.GuideCamp
{
    public class GuideCampfire : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Campfire");
            AddMapEntry(new Color(238, 145, 105), name);
        }

        public static GuideCampfire_TE FindTE(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int left = i - tile.TileFrameX / 18;
            int top = j - tile.TileFrameY / 18;

            int index = ModContent.GetInstance<GuideCampfire_TE>().Find(left, top);
            if (index == -1)
            {
                return null;
            }

            GuideCampfire_TE alter = (GuideCampfire_TE)TileEntity.ByID[index];

            return alter;
        }

        public override bool RightClick(int i, int j)
        {
            GuideCampfire_TE campfire = FindTE(i, j);
            Player player = Main.LocalPlayer;
            if (campfire != null && player.HeldItem.type == ItemID.Torch && player.HeldItem.stack > 0)
            {
                for (int _ = 0; _ < 10; _++)
                {
                    Dust dust = Dust.NewDustDirect(new Vector2(i, j) * 16, 1, 1, DustID.Torch);
                    dust.noGravity = true;
                }

                player.HeldItem.stack--;

                campfire.Interact();
                if (campfire.FireOn)
                { //player.GetModPlayer<DialoguePlayer>().AddNPCPopup(NPCID.Guide, )
                    QuestPlayer questPlayer = player.GetModPlayer<QuestPlayer>();
                    if (questPlayer.IsDoingQuest<Quests.ModQuests.GuideCampfire>())
                    {
                        questPlayer.CompleteMiscRequirement("campfire");
                    }
                }
            }

            return base.RightClick(i, j);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            GuideCampfire_TE campfire = FindTE(i, j);

            if (campfire != null && campfire.FireOn)
            {
                r = 0.9f;
                g = 0.7f;
                b = 0.6f;
            }
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.player[Main.myPlayer];
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ItemID.Torch;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            player.AddBuff(BuffID.Campfire, 10);
        }


        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(null, i * 16, j * 16, 16, 32, ItemID.Campfire);
        }

        int offsetCounter = 0;
        int markerFrame = 2;
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            offsetCounter = frameCounter;
            if (offsetCounter % 24 == 0) markerFrame = markerFrame == 2 ? 3 : 2;
        }


        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            QuestPlayer questPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();

            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {
                if (questPlayer.IsDoingQuest<Quests.ModQuests.GuideCampfire>() && questPlayer.showCampfireArrow)
                {
                    Vector2 offScreenRange = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                    Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + offScreenRange;
                    float yOffset = MathHelper.Lerp(8, 12, (float)Math.Sin(offsetCounter / 24f));


                    Texture2D texture = ModContent.Request<Texture2D>("OvermorrowMod/Quests/QuestAlert").Value;
                    Rectangle drawRectangle = new Rectangle(0, texture.Height / 6 * markerFrame, texture.Width, texture.Height / 6);

                    spriteBatch.Draw(
                        texture,
                        new Vector2(drawPos.X + 26, drawPos.Y - yOffset),
                        drawRectangle,
                        Color.White,
                        0f,
                        new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2),
                        1f,
                        SpriteEffects.None,
                        0f);
                }
            }

            return base.PreDraw(i, j, spriteBatch);
        }
    }

    public class GuideCampfire_TE : ModTileEntity
    {
        public bool FireOn = false;
        private int flameCounter = 0;

        public void Interact()
        {
            FireOn = !FireOn;
            flameCounter = 0;

            Main.LocalPlayer.GetModPlayer<QuestPlayer>().showCampfireArrow = false;
        }

        public override void Update()
        {
            var position = Position.ToWorldCoordinates(16, 16);
            //Dust dust = Dust.NewDustDirect(pos, 1, 1, DustID.AmberBolt);
            //dust.noGravity = true;

            if (FireOn)
            {
                if (flameCounter < 120) flameCounter++;

                Color color = Color.Black;
                float scale = MathHelper.Lerp(0, Main.rand.NextFloat(0.1f, 0.15f), Utils.Clamp(flameCounter, 0, 60) / 60f);
                float rotation = Main.rand.NextFloat(0, MathHelper.PiOver2);
                float maxTime = Main.rand.Next(7, 9) * 10;
                float alpha = Main.rand.NextFloat(0.1f, 0.25f);
                float speed = MathHelper.Lerp(0, Main.rand.Next(2, 4), Utils.Clamp(flameCounter, 0, 60) / 60f);

                float yOffset = MathHelper.Lerp(0, -16, Utils.Clamp(flameCounter, 0, 60) / 60f);
                if (Main.rand.NextBool(3))
                    Particle.CreateParticle(Particle.ParticleType<Smoke>(), position + new Vector2(8, yOffset), -Vector2.UnitY * speed, color, 1f, scale, rotation, scale, maxTime, alpha);

                color = Color.Orange;

                //Particle.CreateParticle(Particle.ParticleType<Smoke>(), position + new Vector2(10, 0), -Vector2.UnitY * Main.rand.Next(2, 4), Color.Red, 1f, scale * 1.1f, rotation, Main.rand.NextFloat(0.01f, 0.015f));


                if (flameCounter > 60)
                {
                    float flameScale = MathHelper.Lerp(0.0125f, Main.rand.NextFloat(0.025f, 0.08f), Utils.Clamp(flameCounter - 60, 0, 60f) / 60f);
                    float flameOffsetY = MathHelper.Lerp(4, 0, Utils.Clamp(flameCounter - 60, 0, 60) / 60f);

                    float scaleRate = 0.004f;
                    Particle.CreateParticle(Particle.ParticleType<Smoke>(), position + new Vector2(10, flameOffsetY), -Vector2.UnitY * Main.rand.Next(2, 4), color * 0.7f, 1f, scale, rotation, flameScale, 0f, 0f, scaleRate);
                }

                if (Main.rand.NextBool())
                {
                    float flameOffset = Main.rand.NextFloat(-2.5f, 2.5f) * 4;
                    //float scaleRate = Main.rand.NextFloat(0.01f, 0.011f);
                    //Particle.CreateParticle(Particle.ParticleType<Flames>(), position + new Vector2(10 + flameOffset, -4), -Vector2.UnitY * Main.rand.Next(5, 7), color, 1f, scale, rotation, scaleRate);
                }

                if (Main.rand.NextBool(15))
                {
                    float offset = Main.rand.Next(-4, 4) * 4;
                    Particle.CreateParticle(Particle.ParticleType<Ember>(), position + new Vector2(10 + offset, -2), -Vector2.UnitY * Main.rand.Next(2, 4), color, 1f, scale, rotation, Main.rand.NextFloat(0.01f, 0.015f), scale);
                }
            }

            base.Update();
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Dust.NewDustDirect(new Vector2(x, y) * 16, 1, 1, DustID.RedTorch);

            Tile tile = Main.tile[x, y];
            if (!tile.HasTile || tile.TileType != ModContent.TileType<GuideCampfire>())
            {
                Kill(Position.X, Position.Y);
            }

            return tile.HasTile && tile.TileType == ModContent.TileType<GuideCampfire>();
        }
    }
}