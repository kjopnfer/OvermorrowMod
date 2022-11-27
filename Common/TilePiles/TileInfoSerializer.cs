using Microsoft.Xna.Framework;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Common.TilePiles
{
    public class TileInfoSerializer : TagSerializer<TileInfo, TagCompound>
    {
        public override TagCompound Serialize(TileInfo value) => new TagCompound
        {
            ["identifier"] = value.identifier,
            ["xCoordinate"] = value.coordinates.X,
            ["yCoordinate"] = value.coordinates.Y,
            ["x"] = value.x,
            ["y"] = value.y,
            ["dependency"] = value.dependency,
            ["interactType"] = value.interactType
        };

        public override TileInfo Deserialize(TagCompound tag) => new TileInfo(
            tag.GetString("identifier"),
            new Vector2(tag.GetFloat("xCoordinate"), tag.GetFloat("yCoordinate")),
            tag.GetInt("x"),
            tag.GetInt("y"),
            tag.GetInt("dependency"),
            tag.GetInt("interactType"));
    }
}