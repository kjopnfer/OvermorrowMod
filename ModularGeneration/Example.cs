using Microsoft.Xna.Framework;
using Terraria.ID;

namespace OvermorrowMod.ModularGeneration
{
    public class Example
    {
        public void generate(Vector2? entrance) //entrance CAN be null, if not specified.+
        {
            //world gen code goes here
            //always use ModularRoomGenerator.placeTile instead of WorldGen.placeTile
        }

        public void generate2(Vector2? entrance)
        {
            //different world gen code goes here
        }

        public void AnyWorldGenMethod()
        {

            Room.GenerateRoom code = new Room.GenerateRoom(generate); //this allows the ModularRoomGenerator to actually generate the rooms

            ModularRoomGenerator.AddRoom(new Room //AddRoom takes a Room struct, with all the parameters you see here
            {
                BlockType = TileID.BlueDungeonBrick,
                offset = 10,
                Entrance = Vector2.Zero,
                Exit = Vector2.One
            }, code); //AddRoom also takes a function reference, thats what the variable code is

            code = new Room.GenerateRoom(generate2);

            ModularRoomGenerator.AddRoom(new Room 
            {
                BlockType = TileID.PinkDungeonBrick,
                offset = 5,
                Entrance = Vector2.Zero,
                Exit = Vector2.One
            }, code);

            ModularRoomGenerator.Run(); //When all rooms are added, use this functions to begin generation
            ModularRoomGenerator.Clear(); //emptys all current room templatess
        }
    }
}