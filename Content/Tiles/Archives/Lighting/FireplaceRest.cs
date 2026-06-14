using OvermorrowMod.Common.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Content.Tiles.Archives
{
    /// <summary>
    /// One-time rest charge for a single Fireplace. Anchored at the multitile's
    /// bottom-left so it coexists with the ArchiveLight_TE at the top-left.
    /// </summary>
    public class FireplaceRest_TE : ModTileEntity
    {
        public const float HealthFraction = 0.35f;

        public bool HasRested;

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Framing.GetTileSafely(x, y);
            return tile.HasTile && tile.TileType == ModContent.TileType<Fireplace>();
        }

        public bool TryRest(Player player)
        {
            if (HasRested) return false;

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

            HasRested = true;
            return true;
        }

        public static FireplaceRest_TE GetOrCreate(int x, int y)
        {
            if (TileUtils.TryFindModTileEntity<FireplaceRest_TE>(x, y, out var existing))
                return existing;

            if (Main.netMode == NetmodeID.MultiplayerClient) return null;

            int id = ModContent.GetInstance<FireplaceRest_TE>().Place(x, y);
            return TileEntity.ByID.TryGetValue(id, out var placed) ? placed as FireplaceRest_TE : null;
        }

        public static bool IsRested(int anchorX, int anchorY)
        {
            return TileUtils.TryFindModTileEntity<FireplaceRest_TE>(anchorX, anchorY, out var te) && te.HasRested;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["HasRested"] = HasRested;
        }

        public override void LoadData(TagCompound tag)
        {
            HasRested = tag.GetBool("HasRested");
        }
    }
}
