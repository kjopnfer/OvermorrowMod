# ModularRoomGenerator class

The **`ModularRoomGenerator`** class is a tool for managing a list of rooms and generating tunnels between them. It has the following features:

- Manages a list of rooms.
- Allows rooms to be added to the list using the **`AddRoom`** function.
- Runs the generation function for each room in the list using the **`Run`** function.
- Generates tunnels between rooms using the **`Tunnel`** class.
- Enable debugging by calling **`ModularRoomGenerator.SetDebugging(true)`**
- When debugging is on, you can remove rooms by simply calling **`ModularRoomGenerator.RemoveRoom`**
  
## Tunnel class

The **`Tunnel`** class is a helper class used by the **`ModularRoomGenerator`** to create tunnels between rooms. It has the following features:

- Contains two lists of vectors that define the tunnel path.
- Calculates the tunnel vectors given the start and end points and the offset distance using the **`Calculate`** function.
- Calls **`WorldGen.PlaceTile`** on every vector in the **`Tunnel1`** and **`Tunnel2`** lists using the **`PlaceTiles`** function.