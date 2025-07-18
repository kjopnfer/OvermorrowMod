using System.Collections.Generic;
using System;
using Terraria;
using Terraria.DataStructures;

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

        public void AddEffect<TKeyword>(Func<Player, object[], bool> condition, Action<Player, object[]> effect) where TKeyword : AccessoryKeyword
        {
            var keywordType = typeof(TKeyword);
            if (!KeywordEffects.ContainsKey(keywordType))
                KeywordEffects[keywordType] = new List<AccessoryEffect>();

            Func<Player, object[], bool> wrappedCondition = (player, args) =>
            {
                if (!player.GetModPlayer<AccessoryPlayer>().HasAccessory(AccessoryType))
                    return false;

                return condition(player, args);
            };

            KeywordEffects[keywordType].Add(new AccessoryEffect(wrappedCondition, effect));
        }

        public void AddEffect(Type keywordType, Func<Player, object[], bool> condition, Action<Player, object[]> effect)
        {
            if (!typeof(AccessoryKeyword).IsAssignableFrom(keywordType))
                throw new ArgumentException($"Keyword type must inherit from AccessoryKeyword: {keywordType.Name}");

            if (!KeywordEffects.ContainsKey(keywordType))
                KeywordEffects[keywordType] = new List<AccessoryEffect>();

            KeywordEffects[keywordType].Add(new AccessoryEffect(condition, effect));
        }

        public void AddRetaliateEffect(Func<Player, NPC, Player.HurtInfo, bool> condition, Action<Player, NPC, Player.HurtInfo> effect)
        {
            AddEffect<RetaliateKeyword>(
                (player, args) => condition(player, (NPC)args[0], (Player.HurtInfo)args[1]),
                (player, args) => effect(player, (NPC)args[0], (Player.HurtInfo)args[1])
            );
        }

        public void AddStrikeEffect(Func<Player, NPC, NPC.HitInfo, int, bool> condition, Action<Player, NPC, NPC.HitInfo, int> effect)
        {
            AddEffect<StrikeKeyword>(
                (player, args) => condition(player, (NPC)args[0], (NPC.HitInfo)args[1], (int)args[2]),
                (player, args) => effect(player, (NPC)args[0], (NPC.HitInfo)args[1], (int)args[2])
            );
        }

        public void AddProjectileStrikeEffect(Func<Player, Projectile, NPC, NPC.HitInfo, int, bool> condition, Action<Player, Projectile, NPC, NPC.HitInfo, int> effect)
        {
            AddEffect<StrikeProjectileKeyword>(
                (player, args) => condition(player, (Projectile)args[0], (NPC)args[1], (NPC.HitInfo)args[2], (int)args[3]),
                (player, args) => effect(player, (Projectile)args[0], (NPC)args[1], (NPC.HitInfo)args[2], (int)args[3])
            );
        }

        public void AddVigorEffect(Func<Player, bool> condition, Action<Player> effect)
        {
            AddEffect<VigorKeyword>(
                (player, args) => condition(player),
                (player, args) => effect(player)
            );
        }

        public void AddDeathsDoorEffect(Func<Player, bool> condition, Action<Player> effect)
        {
            AddEffect<DeathsDoorKeyword>(
                (player, args) => condition(player),
                (player, args) => effect(player)
            );
        }

        public void AddMindDownEffect(Func<Player, bool> condition, Action<Player> effect)
        {
            AddEffect<MindDownKeyword>(
                (player, args) => condition(player),
                (player, args) => effect(player)
            );
        }

        public void AddExecuteEffect(Func<Player, NPC, bool> condition, Action<Player, NPC> effect)
        {
            AddEffect<ExecuteKeyword>(
                (player, args) => condition(player, (NPC)args[0]),
                (player, args) => effect(player, (NPC)args[0])
            );
        }

        public void AddAmbushEffect(Func<Player, NPC, NPC.HitInfo, int, bool> condition, Action<Player, NPC, NPC.HitInfo, int> effect)
        {
            AddEffect<AmbushKeyword>(
                (player, args) => condition(player, (NPC)args[0], (NPC.HitInfo)args[1], (int)args[2]),
                (player, args) => effect(player, (NPC)args[0], (NPC.HitInfo)args[1], (int)args[2])
            );
        }

        public void AddAerialEffect(Func<Player, bool> condition, Action<Player> effect)
        {
            AddEffect<AerialKeyword>(
                (player, args) => condition(player),
                (player, args) => effect(player)
            );
        }

        public void AddFocusEffect(Func<Player, Projectile, bool> condition, Action<Player, Projectile> effect)
        {
            AddEffect<FocusKeyword>(
                (player, args) => condition(player, (Projectile)args[0]),
                (player, args) => effect(player, (Projectile)args[0])
            );
        }

        public void AddReloadEffect(Func<Player, bool, bool> condition, Action<Player, bool> effect)
        {
            AddEffect<ReloadKeyword>(
                (player, args) => condition(player, (bool)args[0]),
                (player, args) => effect(player, (bool)args[0])
            );
        }

        public void AddMisfireEffect(Func<Player, bool> condition, Action<Player> effect)
        {
            AddEffect<MisfireKeyword>(
                (player, args) => condition(player),
                (player, args) => effect(player)
            );
        }

        public void AddQuickslotEffect(Func<Player, Item, bool> condition, Action<Player, Item> effect)
        {
            AddEffect<QuickslotKeyword>(
                (player, args) => condition(player, (Item)args[0]),
                (player, args) => effect(player, (Item)args[0])
            );
        }

        public void AddRespiteEffect(Func<Player, bool> condition, Action<Player> effect)
        {
            AddEffect<RespiteKeyword>(
                (player, args) => condition(player),
                (player, args) => effect(player)
            );
        }

        public void AddSecondaryEffect(Func<Player, Item, bool> condition, Action<Player, Item> effect)
        {
            AddEffect<SecondaryKeyword>(
                (player, args) => condition(player, (Item)args[0]),
                (player, args) => effect(player, (Item)args[0])
            );
        }

        public void AddProjectileSpawnEffect(Func<Player, Projectile, IEntitySource, bool> condition, Action<Player, Projectile, IEntitySource> effect)
        {
            AddEffect<ProjectileSpawnKeyword>(
                (player, args) => condition(player, (Projectile)args[0], (IEntitySource)args[1]),
                (player, args) => effect(player, (Projectile)args[0], (IEntitySource)args[1])
            );
        }

        /// <summary>
        /// Uses HitModifiers instead of the StrikeEffect's HitInfo.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="effect"></param>
        public void AddProjectileModifyHitEffect(Func<Player, Projectile, NPC, NPC.HitModifiers, bool> condition, Action<Player, Projectile, NPC, NPC.HitModifiers> effect)
        {
            AddEffect<ProjectileModifyHitKeyword>(
                (player, args) => condition(player, (Projectile)args[0], (NPC)args[1], (NPC.HitModifiers)args[2]),
                (player, args) => effect(player, (Projectile)args[0], (NPC)args[1], (NPC.HitModifiers)args[2])
            );
        }
    }
}