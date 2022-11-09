using Microsoft.Xna.Framework;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Common.TilePiles
{
    public class TileObjectSerializer : TagSerializer<TileObject, TagCompound>
    {
        public override TagCompound Serialize(TileObject value) => new TagCompound
        {
            ["identifier"] = value.identifier,
            ["xCoordinate"] = value.coordinates.X,
            ["yCoordinate"] = value.coordinates.Y,
            ["x"] = value.x,
            ["y"] = value.y,
            ["dependency"] = value.dependency,
            ["interactType"] = value.interactType
        };

        public override TileObject Deserialize(TagCompound tag) => new TileObject(
            tag.GetString("identifier"),
            new Vector2(tag.GetFloat("xCoordinate"), tag.GetFloat("yCoordinate")),
            tag.GetInt("x"),
            tag.GetInt("y"),
            tag.GetInt("dependency"),
            tag.GetInt("interactType"));
    }
}