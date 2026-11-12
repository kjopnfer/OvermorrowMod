namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid
{
    /// <summary>
    /// A door the layout can pair with another door by id.
    /// </summary>
    public interface IDungeonDoor
    {
        int DoorID { get; set; }
        int PairedDoor { get; set; }
    }
}
