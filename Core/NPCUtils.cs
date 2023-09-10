using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Players;
using OvermorrowMod.Common.TilePiles;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Microsoft.Xna.Framework.Input;

namespace OvermorrowMod.Core
{
    public static partial class ModUtils
    {
        public static bool CheckEntityBottomSlopeCollision(this Entity entity)
        {
            Tile bottomLeftTile = Main.tile[(int)entity.Hitbox.BottomLeft().X / 16, (int)entity.Hitbox.BottomLeft().Y / 16];
            Tile bottomRightTile = Main.tile[(int)entity.Hitbox.BottomRight().X / 16, (int)entity.Hitbox.BottomRight().Y / 16];

            return (bottomLeftTile.HasTile && Main.tileSolid[bottomLeftTile.TileType]) || (bottomRightTile.HasTile && Main.tileSolid[bottomRightTile.TileType]);
        }

        public static OvermorrowGlobalProjectile GlobalProjectile(this Projectile projectile)
        {
            return projectile.GetGlobalProjectile<OvermorrowGlobalProjectile>();
        }
    }
}