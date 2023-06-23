using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Audio;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.UI.JobBoard;
using System.Collections.Generic;
using OvermorrowMod.Quests;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Content.Tiles.Town
{
    public class JobBoard : ModTile
    {
        public override bool CanExplode(int i, int j) => false;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(2, 3);

            TileObjectData.newTile.Width = 6;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };

            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.addTile(Type);

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Job Board");
            AddMapEntry(new Color(37, 37, 37), name);
        }

        bool isHovering = false;
        public override void MouseOver(int i, int j)
        {
            isHovering = true;
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
        }

        public static JobBoard_TE FindTE(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int left = i - tile.TileFrameX / 18;
            int top = j - tile.TileFrameY / 18;

            int index = ModContent.GetInstance<JobBoard_TE>().Find(left, top);
            if (index == -1) return null;

            JobBoard_TE entity = (JobBoard_TE)TileEntity.ByID[index];
            return entity;
        }

        public override bool RightClick(int i, int j)
        {
            SoundEngine.PlaySound(SoundID.MenuOpen, new Vector2(i * 16, j * 16));
            
            JobBoard_TE tileEntity = FindTE(i, j);
            if (tileEntity != null) UIJobBoardSystem.Instance.BoardState.OpenJobBoard(tileEntity);
            
            return true;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            isHovering = false;
            base.AnimateTile(ref frame, ref frameCounter);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {
                Texture2D texture = ModContent.Request<Texture2D>("OvermorrowMod/Content/Tiles/Town/JobBoard_Highlight").Value;
                Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

                Rectangle boardHitbox = new Rectangle(i * 16, j * 16, 16 * 7, 16 * 4);
                if (isHovering)
                {
                    Vector2 offset = new Vector2(-2, -2);
                    Main.spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero + offset, null, Color.PaleGoldenrod * 0.75f, 0f, default, 1f, SpriteEffects.None, 0f);
                }
            }

            return base.PreDraw(i, j, spriteBatch);
        }
    }

    public enum JobBoardID
    {
        Sojourn = -42069
    }

    public class JobBoard_TE : ModTileEntity
    {
        public HashSet<BaseQuest> jobQuests { get; private set; }
        private int boardID;

        public override void SaveData(TagCompound tag)
        {
            tag["boardID"] = boardID;
        }

        public override void LoadData(TagCompound tag)
        {
            boardID = tag.Get<int>("boardID");
        }

        // TODO: make button to clear quests for testing
        private void GetAvailableQuest(int id)
        {
            if (jobQuests.Count < 1) return;

            //jobQuests.Add()
        }

        // on interact, pass in the tile entity id to the ui state
        // on update, try to grab an available quest
        // save quest to a dictionary
        // when a player grabs a quest, remove it from available quests
        // prevent the quest from being assigned again while a player has the quest
        // alternatively, save the quest into another dictionary of active quests to reduce complexity of tracking
        public override void Update()
        {
            for (int i = 0; i < ByID.Count; i++)
            {
                ByID.TryGetValue(i, out TileEntity entity);

                // TODO: Make each town have a unique ID
                if (entity is JobBoard_TE)
                {
                    GetAvailableQuest(i);
                }
            }
            //int id = 
            //GetAvailableQuest(ByID[Position.X / 16, Position.Y / 16]);
            base.Update();
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile || tile.TileType != ModContent.TileType<JobBoard>())
                Kill(Position.X, Position.Y);

            return tile.HasTile && tile.TileType == ModContent.TileType<JobBoard>();
        }
    }
}