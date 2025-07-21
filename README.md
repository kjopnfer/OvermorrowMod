# Overmorrow

## Folder Structure
- **Assets**: Contains all the game assets like textures, sprites, and maps that are used.

- **Content**: Contains all elements that are directly interactable within the game, you can see or use it.
	- Use this folder for mod-specific implementations or game content like NPCs, projectiles, items, etc.
	- Example: Content-specific code that isn't broadly reusable.

- **Common**: Includes shared code that can be used by content and core. This includes utility functions and base classes.
	- Can be implemented by multiple types of entities (e.g., NPCs, projectiles, players) across the mod.
	- It doesn't rely on specific game content or assets.
	- Example: Interfaces, base classes, or utilities shared across many systems.

- **Core**: Contains the core mod logic, such as system handling, global overrides, and gameplay mechanics.
	- Should be tightly coupled with the infrastructure or base systems of the mod.
	- Example: Custom game frameworks, centralized logic, or systems that manage or define instances.

## Development Notes
- See the style guide to retain consistency.

## License
[All code here is licensed under the GPL v3 license.](LICENSE).

TL;DR: You can freely use, modify, and distribute this code, but any software you create that incorporates this GPL v3 
code must also be released under GPL v3 and you must provide the source code to anyone you distribute it to.