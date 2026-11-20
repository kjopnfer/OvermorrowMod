using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Common.Waypoints;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.Archives
{
    /// <summary>
    /// Anchored at the multitile's bottom-left so it coexists with the ArchiveLight_TE at the top-left.
    /// </summary>
    public class Fireplace_TE : Waypoint_TE
    {
        public const float HealthFraction = 0.35f;

        public override Rectangle TileArea => new Rectangle(Position.X, Position.Y - 2, 6, 3);

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Framing.GetTileSafely(x, y);
            return tile.HasTile && tile.TileType == ModContent.TileType<Fireplace>();
        }

        public bool TryLight(Player player)
        {
            if (IsLit) return false;

            int hp = (int)(player.statLifeMax2 * HealthFraction);
            if (hp > 0) player.Heal(hp);

            int mp = player.statManaMax2 - player.statMana;
            if (mp > 0)
            {
                player.statMana = player.statManaMax2;
                player.ManaEffect(mp);
            }

            for (int b = 0; b < Player.MaxBuffs; b++)
            {
                int type = player.buffType[b];
                if (type > 0 && Main.debuff[type] && !Main.buffNoTimeDisplay[type])
                {
                    player.DelBuff(b);
                    b--;
                }
            }

            IsLit = true;
            return true;
        }

        public static Fireplace_TE GetOrCreate(int x, int y)
        {
            if (TileUtils.TryFindModTileEntity<Fireplace_TE>(x, y, out var existing))
                return existing;

            if (Main.netMode == NetmodeID.MultiplayerClient) return null;

            int id = ModContent.GetInstance<Fireplace_TE>().Place(x, y);
            return TileEntity.ByID.TryGetValue(id, out var placed) ? placed as Fireplace_TE : null;
        }

        public static bool IsLitAt(int anchorX, int anchorY)
        {
            return TileUtils.TryFindModTileEntity<Fireplace_TE>(anchorX, anchorY, out var te) && te.IsLit;
        }
    }
}
