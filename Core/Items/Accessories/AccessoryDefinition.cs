using System.Collections.Generic;
using System;
using Terraria;

namespace OvermorrowMod.Core.Items.Accessories
{
    public class AccessoryDefinition
    {
        public Type AccessoryType { get; set; }
        public Dictionary<Type, List<AccessoryEffect>> KeywordEffects { get; set; } = new();

        public AccessoryDefinition(Type accessoryType)
        {
            AccessoryType = accessoryType;
            AccessoryManager.RegisterAccessory(this);
        }

        public void AddEffect<TKeyword>(Func<Player, bool> condition, Action<Player, object[]> effect) where TKeyword : AccessoryKeyword
        {
            var keywordType = typeof(TKeyword);
            if (!KeywordEffects.ContainsKey(keywordType))
                KeywordEffects[keywordType] = new List<AccessoryEffect>();

            KeywordEffects[keywordType].Add(new AccessoryEffect(condition, effect));
        }

        public void AddEffect(Type keywordType, Func<Player, bool> condition, Action<Player, object[]> effect)
        {
            if (!typeof(AccessoryKeyword).IsAssignableFrom(keywordType))
                throw new ArgumentException($"Keyword type must inherit from AccessoryKeyword: {keywordType.Name}");

            if (!KeywordEffects.ContainsKey(keywordType))
                KeywordEffects[keywordType] = new List<AccessoryEffect>();

            KeywordEffects[keywordType].Add(new AccessoryEffect(condition, effect));
        }

        // Type-safe helper methods
        public void AddRetaliateEffect(Func<Player, bool> condition, Action<Player, NPC, Player.HurtInfo> effect)
        {
            AddEffect<RetaliateKeyword>(condition, (player, args) => effect(player, (NPC)args[0], (Player.HurtInfo)args[1]));
        }

        public void AddStrikeEffect(Func<Player, bool> condition, Action<Player, NPC, NPC.HitInfo, int> effect)
        {
            AddEffect<StrikeKeyword>(condition, (player, args) => effect(player, (NPC)args[0], (NPC.HitInfo)args[1], (int)args[2]));
        }

        public void AddStrikeProjectileEffect(Func<Player, bool> condition, Action<Player, Projectile, NPC, NPC.HitInfo, int> effect)
        {
            AddEffect<StrikeProjectileKeyword>(condition, (player, args) => effect(player, (Projectile)args[0], (NPC)args[1], (NPC.HitInfo)args[2], (int)args[3]));
        }

        public void AddVigorEffect(Func<Player, bool> condition, Action<Player> effect)
        {
            AddEffect<VigorKeyword>(condition, (player, args) => effect(player));
        }

        public void AddDeathsDoorEffect(Func<Player, bool> condition, Action<Player> effect)
        {
            AddEffect<DeathsDoorKeyword>(condition, (player, args) => effect(player));
        }

        public void AddMindDownEffect(Func<Player, bool> condition, Action<Player> effect)
        {
            AddEffect<MindDownKeyword>(condition, (player, args) => effect(player));
        }

        public void AddExecuteEffect(Func<Player, bool> condition, Action<Player, NPC> effect)
        {
            AddEffect<ExecuteKeyword>(condition, (player, args) => effect(player, (NPC)args[0]));
        }

        public void AddAmbushEffect(Func<Player, bool> condition, Action<Player, NPC, NPC.HitInfo, int> effect)
        {
            AddEffect<AmbushKeyword>(condition, (player, args) => effect(player, (NPC)args[0], (NPC.HitInfo)args[1], (int)args[2]));
        }

        public void AddAerialEffect(Func<Player, bool> condition, Action<Player> effect)
        {
            AddEffect<AerialKeyword>(condition, (player, args) => effect(player));
        }

        public void AddFocusEffect(Func<Player, bool> condition, Action<Player, Projectile> effect)
        {
            AddEffect<FocusKeyword>(condition, (player, args) => effect(player, (Projectile)args[0]));
        }

        public void AddReloadEffect(Func<Player, bool> condition, Action<Player, bool> effect)
        {
            AddEffect<ReloadKeyword>(condition, (player, args) => effect(player, (bool)args[0]));
        }

        public void AddMisfireEffect(Func<Player, bool> condition, Action<Player> effect)
        {
            AddEffect<MisfireKeyword>(condition, (player, args) => effect(player));
        }

        public void AddQuickslotEffect(Func<Player, bool> condition, Action<Player, Item> effect)
        {
            AddEffect<QuickslotKeyword>(condition, (player, args) => effect(player, (Item)args[0]));
        }

        public void AddRespiteEffect(Func<Player, bool> condition, Action<Player> effect)
        {
            AddEffect<RespiteKeyword>(condition, (player, args) => effect(player));
        }

        public void AddSecondaryEffect(Func<Player, bool> condition, Action<Player, Item> effect)
        {
            AddEffect<SecondaryKeyword>(condition, (player, args) => effect(player, (Item)args[0]));
        }
    }
}