using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace OvermorrowMod.Common.Base
{
    public class BaseConstants
    {
        //------------------------------------------------------//
        //---------------BASE CONSTANTS CLASS-------------------//
        //------------------------------------------------------//
        // Contains various constants that could be useful in   //
        // modding.                                             //
        //------------------------------------------------------//
        //  Author(s): Grox the Great                           //
        //------------------------------------------------------//


        //---Alternating Variables---//


        //returns the name of the client's player. If the player is null or it's called on the server, it returns null.
        public static string NAME_MAINPLAYER { get { return Main.netMode == NetmodeID.Server || Main.player[Main.myPlayer] == null ? null : Main.player[Main.myPlayer].name; } }

        //returns the client player. If the player is null or it's called on the server, it returns null.
        public static Player MAINPLAYER { get { return Main.netMode == NetmodeID.Server ? null : Main.player[Main.myPlayer]; } }


        //----Drawing Constants----//


        //The 'bounding box' of a single player frame.
        public static readonly Rectangle FRAME_PLAYER = new Rectangle(0, 0, 40, 54);


        //----NetMessage.SendData Constants----//


        //these ids are more commonly used
        public const int NET_NPC_UPDATE = 23;
        public const int NET_NPC_HIT = 28;
        public const int NET_PROJ_UPDATE = 27;
        public const int NET_PLAYER_UPDATE = 13;
        public const int NET_TILE_UPDATE = 17;
        public const int NET_ITEM_UPDATE = 21;

        public const int NET_PLAYER_LIFE = 16;
        public const int NET_PLAYER_MANA = 42;
        public const int NET_PLAYER_ITEMROT_ITEMANIM = 41;
        public const int NET_PROJ_MANUALKILL = 29;


        //----Dust Constants----//


        public const int DUSTID_FIRE = 6;
        public const int DUSTID_WATERCANDLE = 29;
        public const int DUSTID_GLITTER = 43;
        public const int DUSTID_BLOOD = 5;
        public const int DUSTID_BONE = 26;
        public const int DUSTID_METAL = 30;
        public const int DUSTID_METALDUST = 31;
        public const int DUSTID_CURSEDFIRE = 75;
        public const int DUSTID_ICHOR = 170;
        public const int DUSTID_FROST = 185;

        public const int DUSTID_SOLAR = 6; //same as fire
        public const int DUSTID_NEBULA = 242;
        public const int DUSTID_STARDUST = 229;
        public const int DUSTID_VORTEX = 229;
        public const int DUSTID_LUNAR = 249; //???


        //----Item Constants----//


        public const int ITEMID_HEART = 58;
        public const int ITEMID_MANASTAR = 184;

        public const int ITEMID_HEALTHPOTION_LESSER = 28;
        public const int ITEMID_HEALTHPOTION = 188;
        public const int ITEMID_HEALTHPOTION_GREATER = 499;

        public const int ITEMID_MANAPOTION_LESSER = 110;
        public const int ITEMID_MANAPOTION = 189;
        public const int ITEMID_MANAPOTION_GREATER = 500;

        //an array of the vanilla gem types, in order: Amethyst, Topaz, Sapphire, Ruby, Emerald, Diamond, Amber.
        public static readonly int[] ITEMIDS_GEMS = new int[] { 181, 180, 177, 178, 179, 182, 999 };

        public static readonly int AMMOTYPE_ARROW = AmmoID.Arrow;
        public static readonly int AMMOTYPE_BULLET = AmmoID.Bullet;

        //----Tile Constants----//


        //these 4 arrays are made with conversion to corruption/hallow/normal biomes in mind. (stone, grass, sand, ice)
        public static readonly int[] TILEIDS_CONVERTCORRUPTION = new int[] { 25, 23, 112, 163, 398, 400 }; //Corruption versions of tiles that can be converted.
        public static readonly int[] TILEIDS_CONVERTHALLOW = new int[] { 117, 109, 116, 164, 402, 403 }; //Hallow versions of tiles that can be converted.
        public static readonly int[] TILEIDS_CONVERTCRIMSON = new int[] { 203, 199, 234, 200, 399, 401 }; //Crimson versions of tiles that can be converted.
        public static readonly int[] TILEIDS_CONVERTOVERWORLD = new int[] { 1, 2, 53, 161, 397, 396 }; //Normal versions of tiles that can be converted.
        public static readonly int[] TILEIDS_CONVERTALL = BaseUtility.CombineArrays(BaseUtility.CombineArrays(TILEIDS_CONVERTOVERWORLD, TILEIDS_CONVERTHALLOW), BaseUtility.CombineArrays(TILEIDS_CONVERTCORRUPTION, TILEIDS_CONVERTCRIMSON)); //combined array of all 4 of the above.

        //tiles found in the dungeon and dungeon-only tiles. (pre-1.2)
        public static readonly int[] TILEIDS_DUNGEON = new int[] { 10, 11, 12, 13, 19, 21, 28, 41, 42, 43, 44, 48, 49, 50 }; //all tiles found in the Dungeon
        public static readonly int[] TILEIDS_DUNGEONSTRICT = new int[] { 41, 42, 43, 44, 48, 49, 50 }; //Dungeon-only tiles

        //individual tile ids (type).
        public const int TILEID_DOORCLOSED = 10;
        public const int TILEID_CHESTS = 21;
        public const int TILEID_SKYISLANDBRICK = 202;

        //the 'style' of a chest.
        public const int CHESTSTYLE_WOOD = 0;
        public const int CHESTSTYLE_GOLD = 1;
        public const int CHESTSTYLE_GOLDLOCKED = 2;
        public const int CHESTSTYLE_SHADOW = 3;
        public const int CHESTSTYLE_SHADOWLOCKED = 4;
        public const int CHESTSTYLE_BARREL = 5;
        public const int CHESTSTYLE_TRASHCAN = 6;
        public const int CHESTSTYLE_EBONWOOD = 7;
        public const int CHESTSTYLE_MOHAGONY = 8;
        public const int CHESTSTYLE_HALLOWWOOD = 9;
        public const int CHESTSTYLE_JUNGLE = 10;
        public const int CHESTSTYLE_ICE = 11;
        public const int CHESTSTYLE_VINED = 12;
        public const int CHESTSTYLE_SKY = 13;
        public const int CHESTSTYLE_SHADEWOOD = 14;
        public const int CHESTSTYLE_WEBBED = 15;
        public const int CHESTSTYLE_LIHZAHRD = 16;
        public const int CHESTSTYLE_SEA = 17;
        public const int CHESTSTYLE_DUNGJUNGLE = 18;
        public const int CHESTSTYLE_DUNGCORRUPT = 19;
        public const int CHESTSTYLE_DUNGCRIMSON = 20;
        public const int CHESTSTYLE_DUNGHALLOWED = 21;
        public const int CHESTSTYLE_DUNGICE = 22;
        public const int CHESTSTYLE_DUNGJUNGLELOCKED = 23;
        public const int CHESTSTYLE_DUNGCORRUPTLOCKED = 24;
        public const int CHESTSTYLE_DUNGCRIMSONLOCKED = 25;
        public const int CHESTSTYLE_DUNGHALLOWEDLOCKED = 26;
        public const int CHESTSTYLE_DUNGICELOCKED = 27;

        //----Misc Constants----//

        //various chat colors used throughout the game.
        public static readonly Color CHATCOLOR_PURPLE = new Color(175, 75, 255);
        public static readonly Color CHATCOLOR_GREEN = new Color(50, 255, 130);
        public static readonly Color CHATCOLOR_RED = new Color(255, 25, 25);
        public static readonly Color CHATCOLOR_YELLOW = new Color(255, 240, 20);

        public static readonly Color NPCTEXTCOLOR_BUFF = new Color(255, 140, 40);

        public const int ARMORID_HEAD = 0;
        public const int ARMORID_BODY = 1;
        public const int ARMORID_LEGS = 2;
        public const int ARMORID_HEADVANITY = 10;
        public const int ARMORID_BODYVANITY = 11;
        public const int ARMORID_LEGSVANITY = 12;

        public const int TIME_DAWNDUSK = 0; //if Main.dayTime is true, this is dawn. Else, this is dusk.
        public const int TIME_MIDDAY = 27000;
        public const int TIME_MIDNIGHT = 16200;

        //various invasionTypes
        public const int INVASION_GOBLIN = 1;
        public const int INVASION_FROSTLEGION = 2;
        public const int INVASION_PIRATE = 3;
        public const int INVASION_MARTIAN = 4;

        //----------------------//
    }

    public class DuoObj
    {
        public object obj1, obj2;

        public DuoObj(object o1, object o2)
        {
            obj1 = o1; obj2 = o2;
        }
    }
}

