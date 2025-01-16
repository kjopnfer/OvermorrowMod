# Style Guide

## Content Organization
- Utility functions and shared code (like base classes) should go in **Common**.
- Core functionality and mod initialization code goes in **Core**.
	- If the file does not handle something that the player can directly interact with it should be placed here.
- Projectiles associated with an NPC should be grouped within the same directory they are used in.

## Textures
- All textures should be stored in **Assets**, this is to make it easier to display the assets for non-coders.
	- Do not utilize TModLoader's texture autoloading, i.e., placing an NPC's texture within the same directory.
	- To load in a texture, manually specify it via this line: 
		- `public override string Texture => AssetDirectory.ArchiveNPCs + Name;`

## Coding Standards
- NPCs should inherit the base **OvermorrowNPC** class.
- Always utilize the **AssetDirectory** for loading textures. This allows for easier refactoring should anything get moved around.

- Avoid utilizing magic numbers if possible, especially if for a conditional.
	```
	private void MagicNumberFunction() 
	{
		if (npc.velocity.X > 128f) <-- Unsure what 128f signifies
		{
			// Do something
		}
	}
	
	private void NonMagicNumberFunction() 
	{
		float maxSpeed = 128f;
		if (npc.velocity.X > maxSpeed) <-- Easily understandable
		{
			// Do something
		}
	}
	```

