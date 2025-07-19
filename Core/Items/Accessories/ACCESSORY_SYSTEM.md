# Accessory System Documentation

## Overview

The accessory system is built around **keywords** that trigger under specific game conditions. 
Keywords can be explicit ("Strike", "Retaliate") or implicit (like "OnSpawn", "OnKill").
Each keyword represents a timing or condition where accessories can register effects to execute when those keywords trigger.

## System Architecture

```
OvermorrowAccessory (base class)
├── Uses AccessoryDefinition to register keyword effects
├── Stores instance variables (charges, cooldowns, etc.)
└── Found via GetInstance<T>(player) from anywhere

AccessoryKeywords (static events)
├── Defines all keyword events (OnStrike, OnRetaliate, etc.)
└── Triggered by game events in GlobalPlayer/GlobalProjectiles

AccessoryManager (ModSystem)
├── Subscribes to keyword events
├── Manages which players have which effects active
└── Executes effects when keywords trigger
```

## Creating a New Accessory

### 1. Basic Accessory Structure

```csharp
public class MyAccessory : OvermorrowAccessory
{
    // Instance variables for storing accessory-specific data
    public int MyCounter { get; private set; } = 0;
    public bool MyFlag { get; private set; } = false;

    protected override void SafeSetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(0, 1, 0, 0);
    }

    public override void ResetVariables()
    {
        // Called when equipped OR unequipped
        MyCounter = 0;
        MyFlag = false;
    }

    protected override void UpdateAccessoryEffects(Player player)
    {
        // Direct effects applied every frame (stat bonuses, lighting, etc.)
        player.GetDamage(DamageClass.Melee) += 0.1f;
    }

    protected override void SetAccessoryEffects(AccessoryDefinition definition)
    {
        // Keyword-based effects (triggers on specific events)
        definition.AddStrikeEffect(/*...*/);
    }
}
```

### 2. Using Instance Variables in Effects

```csharp
protected override void SetAccessoryEffects(AccessoryDefinition definition)
{
    definition.AddStrikeEffect(
        condition: (player, target, hit, damageDone) =>
        {
            var me = GetInstance<MyAccessory>(player);
            return me?.MyCounter < 5; // Check instance variable
        },
        effect: (player, target, hit, damageDone) =>
        {
            var me = GetInstance<MyAccessory>(player);
            if (me != null)
            {
                me.MyCounter++; // Modify instance variable
                player.Heal(10);
            }
        }
    );
}
```

## Adding New Keywords

### Step 1: Add to AccessoryKeywords.cs

```csharp
// Add event
public static event Action<Player, CustomEventArgs> OnMyNewKeyword;

// Add trigger method
public static void TriggerMyNewKeyword(Player player, CustomEventArgs args) => 
    OnMyNewKeyword?.Invoke(player, args);

// Add to AccessoryKeywordTypes class
public static readonly Type MyNewKeyword = typeof(MyNewKeywordClass);

// Add keyword class
public class MyNewKeywordClass : AccessoryKeyword { }
```

### Step 2: Add to AccessoryDefinition.cs

```csharp
public void AddMyNewKeywordEffect(Func<Player, CustomEventArgs, bool> condition, 
                                  Action<Player, CustomEventArgs> effect)
{
    AddEffect<MyNewKeywordClass>(
        (player, args) => condition(player, (CustomEventArgs)args[0]),
        (player, args) => effect(player, (CustomEventArgs)args[0])
    );
}
```

### Step 3: Add to AccessoryManager.cs

```csharp
// In PostSetupContent()
AccessoryKeywords.OnMyNewKeyword += (player, args) =>
    TriggerKeywordEffects(AccessoryKeywordTypes.MyNewKeyword, player, args);
```

### Step 4: Trigger in Game Code

```csharp
// In appropriate Global class or system
public void SomeGameEvent()
{
    // When your condition occurs, trigger the keyword
    AccessoryKeywords.TriggerMyNewKeyword(player, eventArgs);
}
```

## Common Patterns

### Charge/Stack System
```csharp
public int Charges { get; private set; } = 0;
const int MaxCharges = 3;

// Gain charges over time
protected override void UpdateAccessoryEffects(Player player)
{
    if (Charges < MaxCharges)
    {
        chargeTimer++;
        if (chargeTimer >= 900) // 15 seconds
        {
            Charges++;
            chargeTimer = 0;
        }
    }
}

// Consume charges on effect
definition.AddRetaliateEffect(
    condition: (player, attacker, hurtInfo) => GetInstance<MyAccessory>(player)?.Charges > 0,
    effect: (player, attacker, hurtInfo) =>
    {
        var me = GetInstance<MyAccessory>(player);
        if (me != null)
        {
            hurtInfo.Damage -= me.Charges * 10; // Reduce damage
            me.Charges = 0; // Consume all charges
        }
    }
);
```

### Cooldown System
```csharp
public int Cooldown { get; private set; } = 0;

protected override void UpdateAccessoryEffects(Player player)
{
    if (Cooldown > 0) Cooldown--;
}

definition.AddExecuteEffect(
    condition: (player, killedNPC) => GetInstance<MyAccessory>(player)?.Cooldown <= 0,
    effect: (player, killedNPC) =>
    {
        var me = GetInstance<MyAccessory>(player);
        if (me != null)
        {
            player.Heal(50);
            me.Cooldown = 1800; // 30 second cooldown
        }
    }
);
```

### Conditional Bonuses
```csharp
// Vigor effect (above 80% health)
definition.AddVigorEffect(
    condition: (player) => true, // Vigor keyword already checks health
    effect: (player) => player.GetDamage(DamageClass.Melee) += 0.15f
);

// Death's Door effect (below 20% health)
definition.AddDeathsDoorEffect(
    condition: (player) => GetInstance<MyAccessory>(player)?.Cooldown <= 0,
    effect: (player) =>
    {
        var me = GetInstance<MyAccessory>(player);
        player.AddBuff(ModContent.BuffType<MyBuff>(), 600);
        me.Cooldown = 1800;
    }
);
```

## Best Practices

### Do:
- Store accessory-specific data as instance variables
- Use `GetInstance<T>(player)` to access your accessory from keyword effects
- Reset important variables in `ResetVariables()` 
- Use direct effects (`UpdateAccessoryEffects`) for simple stat bonuses
- Use keyword effects for event-based triggers

### Don't:
- Store per-player data in static variables
- Forget to reset variables when accessories are unequipped
- Mix direct effects and keyword effects for the same functionality
- Access instance variables from static contexts without `GetInstance<T>`

## Integration with Other Systems

### Bow Modifiers
```csharp
public class MyBowAccessory : OvermorrowAccessory, IBowModifier
{
    public void ModifyBowStats(BowStats stats, Player player)
    {
        var me = GetInstance<MyBowAccessory>(player);
        if (me?.SomeCondition == true)
        {
            stats.ChargeSpeed *= 1.5f;
        }
    }
}
```

### Gun Modifiers
```csharp
public class MyGunAccessory : OvermorrowAccessory, IGunModifier
{
    public void ModifyGunStats(GunStats stats, Player player)
    {
        stats.ReloadSpeedMultiplier *= 0.8f; // Faster reload
    }
}
```

### Tooltips
```csharp
public class MyAccessory : OvermorrowAccessory, ITooltipEntities
{
    public List<TooltipEntity> TooltipObjects()
    {
        return new List<TooltipEntity>
        {
            new BuffTooltip(texture, "Buff Name", ["Description line"], 10, BuffTooltipType.Buff)
        };
    }
}
```