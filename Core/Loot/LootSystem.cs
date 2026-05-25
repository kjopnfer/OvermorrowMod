using OvermorrowMod.Core.Loot.Pools;
using OvermorrowMod.Core.WorldGeneration.ArchiveSubworld;
using OvermorrowMod.Core.WorldGeneration.TestSubworld;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Loot
{
    public class LootSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            LootPoolRegistry.Register<ArchivePool>(() => SubworldSystem.IsActive<ArchiveSubworld>() || SubworldSystem.IsActive<TestSubworld>());
            ScanLootAttributes();
        }

        public override void Unload()
        {
            LootPoolRegistry.Clear();
            LootMetadata.Clear();
        }

        private void ScanLootAttributes()
        {
            var allPoolTypes = new List<Type>(LootPoolRegistry.AllPoolTypes());

            foreach (var modItem in Mod.GetContent<ModItem>())
            {
                var attrs = modItem.GetType().GetCustomAttributes(false);

                var specificPools = new HashSet<Type>();
                foreach (var attr in attrs)
                {
                    var poolType = ReadSpecificPoolType(attr);
                    if (poolType != null) specificPools.Add(poolType);
                }

                foreach (var attr in attrs)
                {
                    var specificPool = ReadSpecificPoolType(attr);
                    if (specificPool != null)
                    {
                        var (specAff, specRar) = ReadGenericPayload(attr);
                        LootMetadata.Set(specificPool, modItem, specAff, specRar);
                        continue;
                    }

                    if (attr is LootAttribute wildcard)
                    {
                        foreach (var poolType in allPoolTypes)
                        {
                            if (specificPools.Contains(poolType)) continue;
                            LootMetadata.Set(poolType, modItem, wildcard.Affinities, wildcard.Rarity);
                        }
                    }
                }
            }
        }

        private static Type ReadSpecificPoolType(object attr)
        {
            var attrType = attr.GetType();
            if (!attrType.IsGenericType) return null;
            if (attrType.GetGenericTypeDefinition() != typeof(LootAttribute<>)) return null;
            return attrType.GetGenericArguments()[0];
        }

        private static (ItemType, Rarity) ReadGenericPayload(object attr)
        {
            var attrType = attr.GetType();
            var affinities = (ItemType)attrType.GetProperty(nameof(LootAttribute.Affinities)).GetValue(attr);
            var rarity = (Rarity)attrType.GetProperty(nameof(LootAttribute.Rarity)).GetValue(attr);
            return (affinities, rarity);
        }
    }
}
