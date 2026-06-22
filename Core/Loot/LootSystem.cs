using OvermorrowMod.Core.Loot.Pools;
using OvermorrowMod.Core.Items.Collectibles;
using OvermorrowMod.Content.Dungeons.Inkwell;
using OvermorrowMod.Core.WorldGeneration.ArchiveSubworld;
using OvermorrowMod.Core.WorldGeneration.TestSubworld;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Loot
{
    public class LootSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            LootPoolRegistry.Register<ArchivePool>(() => SubworldSystem.IsActive<ArchiveSubworld>() || SubworldSystem.IsActive<TestSubworld>() || SubworldSystem.IsActive<TestSubworld2>() || SubworldSystem.IsActive<InkwellSubworld>());
            LootPoolRegistry.Register<CollectiblesPool>(() => false);
            ScanLootAttributes();
            RegisterVanillaWildcards();
            RegisterCollectibles();
        }

        private void RegisterCollectibles()
        {
            foreach (var modItem in Mod.GetContent<ModItem>())
            {
                if (modItem is CollectibleItem collectible)
                    LootMetadata.Set(typeof(CollectiblesPool), modItem, ItemType.Generic, collectible.Rarity);
            }
        }

        private static readonly (int ItemId, ItemType Affinity, Rarity Rarity)[] VanillaWildcards =
        {
            (ItemID.HermesBoots, ItemType.Generic, Rarity.Common),
            (ItemID.RocketBoots, ItemType.Generic, Rarity.Common),
            (ItemID.CloudinaBottle, ItemType.Generic, Rarity.Common),
            (ItemID.BlizzardinaBottle, ItemType.Generic, Rarity.Common),
            (ItemID.SandstorminaBottle, ItemType.Generic, Rarity.Common),
            (ItemID.TsunamiInABottle, ItemType.Generic, Rarity.Common),
            (ItemID.ShinyRedBalloon, ItemType.Generic, Rarity.Common),
            (ItemID.LuckyHorseshoe, ItemType.Generic, Rarity.Common),
            (ItemID.AnkletoftheWind, ItemType.Generic, Rarity.Common),
            (ItemID.Aglet, ItemType.Generic, Rarity.Common),
            (ItemID.FrogLeg, ItemType.Generic, Rarity.Common),
            (ItemID.BandofRegeneration, ItemType.Generic, Rarity.Rare),
            (ItemID.FeralClaws, ItemType.Melee, Rarity.Common),
            (ItemID.CobaltShield, ItemType.Melee, Rarity.Common),
            (ItemID.MagmaStone, ItemType.Melee, Rarity.Common),
            (ItemID.MagicQuiver, ItemType.Ranged, Rarity.Common),
            (ItemID.BandofStarpower, ItemType.Magic, Rarity.Common),
            (ItemID.CelestialMagnet, ItemType.Magic, Rarity.Common),
            (ItemID.PygmyNecklace, ItemType.Summon, Rarity.Common),
            (ItemID.SpectreBoots, ItemType.Generic, Rarity.Epic),
            (ItemID.CloudinaBalloon, ItemType.Generic, Rarity.Rare),
            (ItemID.BlizzardinaBalloon, ItemType.Generic, Rarity.Rare),
            (ItemID.SandstorminaBalloon, ItemType.Generic, Rarity.Rare),
            (ItemID.ManaRegenerationBand, ItemType.Magic, Rarity.Rare),
            (ItemID.MagicCuffs, ItemType.Magic, Rarity.Rare),
            (ItemID.ManaFlower, ItemType.Magic, Rarity.Rare),
            (ItemID.CelestialCuffs, ItemType.Magic, Rarity.Rare),
            (ItemID.CreativeWings, ItemType.Generic, Rarity.Epic),
            (ItemID.BundleofBalloons, ItemType.Generic, Rarity.Epic),
            (ItemID.Revolver, ItemType.Ranged, Rarity.Common),
            (ItemID.Handgun, ItemType.Ranged, Rarity.Common),
            (ItemID.TheUndertaker, ItemType.Ranged, Rarity.Common),
            (ItemID.Boomstick, ItemType.Ranged, Rarity.Common),
            (ItemID.Musket, ItemType.Ranged, Rarity.Common),
            (ItemID.QuadBarrelShotgun, ItemType.Ranged, Rarity.Rare),
            (ItemID.Minishark, ItemType.Ranged, Rarity.Rare),
            (ItemID.PhoenixBlaster, ItemType.Ranged, Rarity.Epic),
        };

        private void RegisterVanillaWildcards()
        {
            foreach (var poolType in LootPoolRegistry.AllPoolTypes())
            {
                if (!LootPoolRegistry.Get(poolType).AcceptsWildcards) continue;
                foreach (var (itemId, affinity, rarity) in VanillaWildcards)
                {
                    LootMetadata.Set(poolType, itemId, affinity, rarity);
                }
            }
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
                            if (!LootPoolRegistry.Get(poolType).AcceptsWildcards) continue;
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
