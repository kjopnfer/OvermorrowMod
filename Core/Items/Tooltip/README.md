# Tooltip System Documentation

## Overview

The tooltip system provides an enhanced information display for items, including keyword explanations, buff/debuff descriptions, projectile details, and set bonuses. Players can hold **Shift** to view detailed information about any item with enhanced tooltips.

## System Architecture

```
TooltipSystem (ModSystem)
├── Manages global tooltip state and key detection
└── Coordinates between tooltip components

ItemTooltips (GlobalItem)
├── Processes item tooltips and adds keyword parsing
├── Integrates with ITooltipEntities interface
└── Handles localization and text formatting

TooltipRenderer (Static)
├── Handles all tooltip drawing and positioning
├── Processes keyword highlighting and parsing
└── Renders tooltip entities with proper formatting

TooltipParser (Static)
├── Extracts keywords from tooltip text
└── Handles text parsing and validation
```

## Creating Enhanced Tooltips

### 1. Basic Item Tooltips with Keywords

Items automatically support keyword parsing when their tooltips are defined in the localization file:

```hjson
Items: {
    MyWeapon: {
        DisplayName: My Weapon
        Tooltip:
            '''
            {Keyword:Strike}: Your attacks gain special effects
            {Keyword:Reload}: Weapon recharges with bonus damage
            Deals increased damage to armored enemies
            '''
        Flavor: Forged in the depths of forgotten archives
    }
}
```

**Keywords** are automatically detected and highlighted when wrapped in `{Keyword:}` tags. Available keywords are defined in the localization file under the `Keywords` section.

### 2. Advanced Tooltips with ITooltipEntities

For items that need detailed tooltip information (buffs, projectiles, set bonuses), implement `ITooltipEntities`:

```csharp
public class MyAccessory : OvermorrowAccessory, ITooltipEntities
{
    public List<TooltipEntity> TooltipObjects()
    {
        return new List<TooltipEntity>
        {
            // Buff tooltip
            new BuffTooltip(
                texture: ModContent.Request<Texture2D>(AssetDirectory.Tooltips + "MyBuff").Value,
                title: "My Buff Name",
                description: new[] { "Line 1 of description", "Line 2 of description" },
                priority: 10,
                type: BuffTooltipType.Buff
            ),
            
            // Projectile tooltip
            new ProjectileTooltip(
                texture: ModContent.Request<Texture2D>(AssetDirectory.Tooltips + "MyProjectile").Value,
                title: "My Projectile",
                description: new[] { "Projectile behavior description" },
                damage: 25f,
                type: ProjectileTooltipType.Projectile,
                damageClass: DamageClass.Magic
            )
        };
    }
}
```

### 3. Set Bonus Tooltips

*Set bonus tooltips are currently work in progress and not yet implemented.*

## Localization Integration

### Tooltip Text Localization

The system automatically looks for localized text using these paths:

- **Item Tooltips**: `Mods.OvermorrowMod.Items.{ItemName}.Tooltip`
- **Item Flavor Text**: `Mods.OvermorrowMod.Items.{ItemName}.Flavor`
- **Tooltip Entities**: `Mods.OvermorrowMod.TooltipEntities.{EntityName}.DisplayName` and `Description.Line0`, `Description.Line1`, etc.

### Localization File Structure

In `en-US_Mods.OvermorrowMod.hjson`:

```hjson
Items: {
    MyWeapon: {
        Tooltip: "Deals {Keyword:Strike} damage with special effects"
        Flavor: "A weapon forged in the depths of the archives"
    }
}

TooltipEntities: {
    MyBuff: {
        DisplayName: "Power Surge"
        Description: {
            Line0: "+ Increases damage by 25%"
            Line1: "+ Regenerates mana faster"
        }
    }
}

Keywords: {
    Strike: {
        DisplayName: "Strike"
        Description: "Triggers on hitting an enemy"
    }
    
    Focus: {
        DisplayName: "Focus"
        Description: "Triggers when firing during white flash window"
    }
    
    Reload: {
        DisplayName: "Reload"
        Description: "Triggers on successful reload skill check"
    }
}
```

## Available Tooltip Types

### BuffTooltip
- **Purpose**: Display buff/debuff information
- **Properties**: Texture, title, description lines, priority, type (Buff/Debuff)
- **Usage**: Status effects, temporary bonuses/penalties

### ProjectileTooltip
- **Purpose**: Display projectile/summon information  
- **Properties**: Texture, title, description, damage value, projectile type, damage class
- **Usage**: Summoned creatures, special projectiles, weapons that create projectiles

### SetBonusTooltip
- **Status**: Work in progress, not yet implemented
- **Purpose**: Will display armor set bonuses
- **Usage**: Future feature for armor sets and item collections

## Keywords System

### Available Keywords (from localization)

- **Aerial**: Activates while airborne
- **Ambush**: Triggers when attacking unaware enemies  
- **Awakened**: Activates bonuses while inside the "dungeon"
- **Deaths Door**: Activates while below 20% health
- **Execute**: Triggers after killing an enemy
- **Focus**: Triggers when firing during weapons white flash window
- **Misfire**: Triggers on failed reload skill check
- **Quickslot**: Activates while item is in hotbar
- **Reload**: Triggers on successful reload skill check
- **Retaliate**: Triggers when hit by enemy or projectile
- **Secondary**: Triggers when holding right-click
- **Strike**: Triggers on hitting an enemy
- **Vigor**: Activates while above 80% health

### Using Keywords in Tooltips

Keywords are automatically detected when wrapped in `{Keyword:}` tags in your localization file:

```hjson
Items: {
    MyAccessory: {
        DisplayName: My Accessory
        Tooltip:
            '''
            {Keyword:Strike}: Each hit increases damage by 5%
            {Keyword:Reload}: Your next clip has increased firing speed
            {Keyword:Vigor}: Gain 15% increased damage while above 80% health
            {Keyword:DeathsDoor}: Activate emergency shield when below 20% health
            '''
    }
    
    MyWeapon: {
        DisplayName: My Weapon
        Tooltip:
            '''
            {Keyword:Focus}: Triggers during white flash window for bonus damage
            {Keyword:Secondary}: Hold right-click to charge up a powerful shot
            Replaces your arrows with <Projectile:Special Arrows>
            '''
    }
}
```

## Examples

### Complete Accessory with Tooltips

```csharp
public class WarriorsEpic : OvermorrowAccessory, ITooltipEntities
{
    public List<TooltipEntity> TooltipObjects()
    {
        // Get localized text
        var title = Language.GetTextValue(LocalizationPath.Buffs + "WarriorsResolve.DisplayName");
        var line1 = Language.GetTextValue(LocalizationPath.TooltipEntities + "WarriorsResolve.Description.Line0");
        var line2 = Language.GetTextValue(LocalizationPath.TooltipEntities + "WarriorsResolve.Description.Line1");
        var line3 = Language.GetTextValue(LocalizationPath.TooltipEntities + "WarriorsResolve.Description.Line2");

        return new List<TooltipEntity>
        {
            new BuffTooltip(
                ModContent.Request<Texture2D>(AssetDirectory.Tooltips + "WarriorsResolve").Value,
                title,
                new[] { line1, line2, line3 },
                10,
                BuffTooltipType.Buff
            )
        };
    }
    
    protected override void SafeSetDefaults()
    {
        Item.width = 38;
        Item.height = 42;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(0, 1, 0, 0);
    }
    
    // Implementation continues...
}
```

### Corresponding Localization

```hjson
Items: {
    WarriorsEpic: {
        DisplayName: Warriors Epic
        Tooltip: 
            '''
            {Keyword:Vigor}: Gain 15% increased Melee damage
            {Keyword:DeathsDoor}: Gain <Buff:Warriors Resolve> for 10 seconds
            '''
        Flavor: A lifetime of battles bound in parchment and hide
    }
}

TooltipEntities: {
    WarriorsResolve: {
        DisplayName: Warriors Resolve
        Description: {
            Line0: "+ Your next Melee hit gains 100% crit chance"
            Line1: "+ Gain 25% increased movement speed" 
            Line2: "+ Restore 40 health on the next enemy you kill"
        }
    }
}
```

## Best Practices

### Localization
- Always use localization keys instead of hardcoded strings
- Follow the established naming patterns for consistency
- Use descriptive keys that make sense to translators

### Performance
- Cache localized strings when possible
- Keep tooltip entity lists reasonably sized
- Use appropriate priorities to control display order

### User Experience  
- Keep descriptions concise but informative
- Use consistent color coding for similar effect types
- Group related information together
- Provide clear visual hierarchy with priorities

### Content Guidelines
- Use positive language for buffs ("+", "Gain", "Increases")
- Use negative language for debuffs ("-", "Lose", "Decreases")  
- Be specific about numbers and durations
- Explain complex mechanics clearly

## Troubleshooting

### Keywords Not Highlighting
- Verify keyword exists in localization file under `Keywords` section
- Check that keyword is wrapped correctly: `{Keyword:ExactName}`
- Ensure capitalization matches localization key exactly

### Tooltips Not Appearing
- Confirm item implements `ITooltipEntities` correctly
- Check that `TooltipObjects()` returns non-empty list
- Verify textures exist at specified paths
- Make sure player is holding Shift key

### Localization Not Working
- Check localization key paths match exactly
- Verify hjson file syntax is valid
- Confirm localization file is being loaded by mod
- Check for typos in Language.GetTextValue() calls