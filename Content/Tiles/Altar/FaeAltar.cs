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

namespace OvermorrowMod.Content.Tiles.Altar
{
    public class FaeAltar : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<FaeAltar_TE>().Hook_AfterPlacement, -1, 0, true);

            MinPick = 55;

            TileObjectData.addTile(Type);
        }
    }

    public class FaeAltar_TE : ModTileEntity
    {
        public Vector2 AltarPosition => Position.ToWorldCoordinates(16, 16);

        public override void Update()
        {
            int detectionRange = 6 * 16;
            foreach (Player player in Main.player)
            {
                if (!player.active) continue;

                if (Vector2.DistanceSquared(player.Center, AltarPosition) < detectionRange * detectionRange)
                {
                    player.GetModPlayer<AltarPlayer>().NearAltar = true;
                }
            }

            base.Update();
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            int id = Place(i, j);
            FaeAltar_TE te = TileEntity.ByID[id] as FaeAltar_TE;

            AltarWorld.AltarPosition = te.Position.ToWorldCoordinates(16, 16);

            return id;
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile || tile.TileType != ModContent.TileType<FaeAltar>())
            {
                Kill(Position.X, Position.Y);
            }

            return tile.HasTile && tile.TileType == ModContent.TileType<FaeAltar>();
        }
    }

    public class AltarWorld : ModSystem
    {
        public static Vector2 AltarPosition;
        public static int SacrificeBonus;
        public static int SacrificePoints;

        public static readonly int MAX_SACRIFICE = 100;

        public override void SaveWorldData(TagCompound tag)
        {
            tag["AltarPosition"] = AltarPosition;
            tag["SacrificeBonus"] = SacrificeBonus;
            tag["SacrificePoints"] = SacrificePoints;

        }

        public override void LoadWorldData(TagCompound tag)
        {
            AltarPosition = tag.Get<Vector2>("AltarPosition");
            SacrificeBonus = tag.Get<int>("SacrificeBonus");
            SacrificePoints = tag.Get<int>("SacrificePoints");
        }
    }

    public class AltarPlayer : ModPlayer
    {
        public bool NearAltar;

        public override void ResetEffects()
        {
            NearAltar = false;
        }
    }
}