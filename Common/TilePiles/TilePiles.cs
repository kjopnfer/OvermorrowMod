using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Terraria.ModLoader;
using OvermorrowMod.Core;
using Terraria.ModLoader.IO;
using Terraria.GameContent.Metadata;
using OvermorrowMod.Content.Tiles.TilePiles;
using Terraria.Audio;
using OvermorrowMod.Content.Items.Misc;

namespace OvermorrowMod.Common.TilePiles
{
    // Base template for the physical tile piles, would probably be inherited by classes like ModTilePile_3x3 or ModTilePile_2x2 or something
    public abstract class ModTilePile<TEntity> : ModTile where TEntity : BaseTilePile
    {
        public override bool CanExplode(int i, int j) => false;
        public override bool CreateDust(int i, int j, ref int type) => false;
        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;
        public override bool KillSound(int i, int j, bool fail) => false;
        public override void SetStaticDefaults()
        {
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3); // Probably should be changeable within the child
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<BasicLoot>().Hook_AfterPlacement, -1, 0, true); // FOR TESTING ONLY

            MinPick = 55; // debugging
            TileObjectData.addTile(Type);
        }

        // Used for debugging, set to true to draw the tile matrix for the associated tile
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) => false;

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            bool activeObjects = false;

            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX != 0 || tile.TileFrameY != 0) return;

            TEntity pile = FindTE(i, j);

            if (pile == null) return;

            foreach (TileInfo tileObject in pile.PileContents)
            {
                if (tileObject.dependency >= 0)
                {
                    if (!pile.PileContents[tileObject.dependency].active)
                    {
                        if (tileObject.active)
                        {
                            tileObject.active = false;
                            Item.NewItem(new EntitySource_Misc("TilePileLoot"), tileObject.rectangle, tileObject.ID, tileObject.GetRandomStack());

                            SoundEngine.PlaySound(tileObject.deathSound);
                        }
                    }
                }

                if (tileObject.breakCount >= tileObject.tileDurability)
                {
                    if (tileObject.active)
                    {
                        tileObject.active = false;
                        Item.NewItem(new EntitySource_Misc("TilePileLoot"), tileObject.rectangle, tileObject.ID, tileObject.GetRandomStack());

                        SoundEngine.PlaySound(tileObject.deathSound);
                    }
                }

                if (!tileObject.active) continue;

                Rectangle rect = tileObject.rectangle;
                Vector2 pos = new Vector2(rect.X, rect.Y) - Main.screenPosition;
                Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
                Vector2 wiggleOffset = Vector2.Zero;
                float wiggleRotation = 0f;

                if (tileObject.selected)
                {
                    if (tileObject.canWiggle) tileObject.wiggleTimer++;
                    if (tileObject.hitDelay > 0) tileObject.hitDelay--;

                    if (tileObject.wiggleTimer < 20 && tileObject.wiggleTimer > 3)
                    {
                        wiggleOffset = new Vector2((float)Math.Sin(tileObject.wiggleTimer * 4), -1f);
                        wiggleRotation = (float)Math.Sin(tileObject.wiggleTimer * 2) / 10f;
                    }

                    if (tileObject.wiggleTimer > 60) tileObject.wiggleTimer = 0;

                    Player player = Main.LocalPlayer;
                    if (player.itemTime == 0)
                    {
                        player.ApplyItemTime(player.HeldItem);
                    }

                    if (player.HeldItem.pick > 0 && player.itemAnimation > 0 && tileObject.interactType == (int)TileInfo.InteractionType.Mine)
                    {
                        if (player.itemTime <= player.itemTimeMax / 3 && tileObject.hitDelay <= 0)
                        {
                            SoundEngine.PlaySound(tileObject.hitSound);
                            tileObject.breakCount += player.HeldItem.pick;
                            tileObject.hitDelay = player.HeldItem.useTime / 2;

                            for (int _ = 0; _ < 5; _++)
                            {
                                int d = Dust.NewDust(new Vector2(tileObject.rectangle.X, tileObject.rectangle.Y), rect.Width, rect.Height, tileObject.tileDust, 0f, 0f);
                                Main.dust[d].velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi);
                            }
                        }
                    }

                    if (player.HeldItem.axe > 0 && player.itemAnimation > 0 && tileObject.interactType == (int)TileInfo.InteractionType.Chop)
                    {
                        if (player.itemTime <= player.itemTimeMax / 3 && tileObject.hitDelay <= 0)
                        {
                            //Main.NewText(player.itemTime + " / " + player.itemTimeMax);
                            SoundEngine.PlaySound(tileObject.hitSound);
                            tileObject.breakCount += player.HeldItem.axe;
                            tileObject.hitDelay = player.HeldItem.useTime / 2;

                            for (int _ = 0; _ < 5; _++)
                            {
                                int d = Dust.NewDust(new Vector2(tileObject.rectangle.X, tileObject.rectangle.Y), rect.Width, rect.Height, tileObject.tileDust, 0f, 0f);
                                Main.dust[d].velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi);
                            }
                        }
                    }
                }
                else
                {
                    if (tileObject.canWiggle) tileObject.wiggleTimer += 2;
                }

                rect.X = (int)(pos.X + zero.X + wiggleOffset.X) + rect.Width / 2;
                rect.Y = (int)(pos.Y + zero.Y + wiggleOffset.Y) + rect.Height / 2;

                Vector2 offScreenRange = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + offScreenRange;
                Color hoverColor = tileObject.selected ? Color.Yellow : Lighting.GetColor(i, j);

                spriteBatch.Draw(tileObject.texture, pos + offScreenRange, null, hoverColor, wiggleRotation, Vector2.Zero, 1f, SpriteEffects.None, 0);

                if (Main.rand.NextBool(180))
                {
                    int d = Dust.NewDust(new Vector2(tileObject.rectangle.X, tileObject.rectangle.Y), rect.Width, rect.Height, DustID.TintableDustLighted, 0f, 0f, 254, Color.White, 0.5f);
                    Main.dust[d].velocity *= 0f;
                }

                tileObject.selected = false;
                activeObjects = true;
            }

            if (!activeObjects) WorldGen.KillTile(i, j);
        }

        public override void MouseOver(int i, int j)
        {
            TEntity pile = FindTE(i, j);

            if (pile == null) return;

            foreach (TileInfo tileObject in pile.PileContents)
            {
                if (!tileObject.active) continue;

                if (Main.MouseWorld.Between(tileObject.rectangle.TopLeft(), tileObject.rectangle.BottomRight()))
                {
                    tileObject.selected = true;

                    Player player = Main.LocalPlayer;
                    player.cursorItemIconEnabled = true;

                    switch (tileObject.interactType)
                    {
                        case (int)TileInfo.InteractionType.Click:
                            player.cursorItemIconID = ModContent.ItemType<EmptyItem>();
                            break;
                        case (int)TileInfo.InteractionType.Chop:
                            player.cursorItemIconID = ItemID.IronAxe;
                            break;
                        case (int)TileInfo.InteractionType.Mine:
                            player.cursorItemIconID = ItemID.IronPickaxe;
                            break;
                    }
                }
            }
        }

        public override bool RightClick(int i, int j)
        {
            TEntity pile = FindTE(i, j);

            if (pile != null)
            {
                foreach (TileInfo tileObject in pile.PileContents)
                {
                    if (Main.MouseWorld.Between(tileObject.rectangle.TopLeft(), tileObject.rectangle.BottomRight()) && tileObject.active)
                    {
                        tileObject.active = false;
                        Item.NewItem(new EntitySource_Misc("TilePileLoot"), tileObject.rectangle, tileObject.ID, tileObject.GetRandomStack());

                        SoundEngine.PlaySound(tileObject.grabSound);
                        break;
                    }
                }
            }

            return true;
        }

        public static TEntity FindTE(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int left = i - tile.TileFrameX / 18;
            int top = j - tile.TileFrameY / 18;

            int index = ModContent.GetInstance<TEntity>().Find(left, top);
            if (index == -1)
            {
                return null;
            }

            TEntity entity = (TEntity)TileEntity.ByID[index];
            return entity;

        }
    }


}
