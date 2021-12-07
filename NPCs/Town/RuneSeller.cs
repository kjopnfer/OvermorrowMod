using OvermorrowMod.Items.Consumable;
using OvermorrowMod.Items.Pets;
using OvermorrowMod.Projectiles.NPCs.Town;
using OvermorrowMod.WardenClass.Accessories;
using OvermorrowMod.WardenClass.Weapons.ChainWeapons;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Town
{
    [AutoloadHead]
    public class RuneSeller : ModNPC
    {
        public override string Texture => "OvermorrowMod/NPCs/Town/RuneSeller";

        public override bool Autoload(ref string name)
        {
            name = "Rune Merchant";
            return mod.Properties.Autoload;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rune Merchant");
            Main.npcFrameCount[npc.type] = 26;
            NPCID.Sets.ExtraFramesCount[npc.type] = 9;
            NPCID.Sets.AttackFrameCount[npc.type] = 4;
            NPCID.Sets.DangerDetectRange[npc.type] = 400;
            NPCID.Sets.AttackType[npc.type] = 2;
            NPCID.Sets.AttackTime[npc.type] = 128;
            NPCID.Sets.AttackAverageChance[npc.type] = 30;
        }

        public override void SetDefaults()
        {
            npc.CloneDefaults(NPCID.Guide);
            npc.townNPC = true;
            npc.friendly = true;
            npc.aiStyle = 7;
            npc.damage = 30;
            npc.defense = 30;
            npc.lifeMax = 500;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 0.4f;
            animationType = NPCID.Guide;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            // EoW or BoC
            return NPC.downedBoss2 && Main.player.Any(x => x.active);
        }

        public override string TownNPCName()
        {
            string[] names = { "Daniel", "Frederick", "Mark", "John", "Billy", "Manny", "Jerry", "Dylan", "Thomas",
            "Johnathan", "Roman", "Michael", "Scott", "James", "Ben", "Danny", "Austin", "Texas", "Mississippi" };
            return Main.rand.Next(names);
        }

        public override string GetChat()
        {
            List<string> dialogue = new List<string>
            {
                "I chained myself up! I don't remember why, but it was probably for a good reason.",
                "Where I'm from? Can't seem to recall any defining features other than it being very dark.",
                "If you have Chain Weapons with special debuffs, I can extract and bind them onto a rune for you. For a price of course.",
                "You wouldn't happen to know anybody named Vaema would you?",
                "\"Gods, demons, humanity, monsters, and other eternal beings are no match for 'him.' Azarel, eternity itself, must destroy!\" Ah sorry. Was quoting from my favorite book.",
                "Centuries ago, the dungeon was actually at the bottom of the sea, some ancient ocean energies still reside there",
                "If you find any mysterious looking chests underground, do not open them",
            };

            return Main.rand.Next(dialogue);
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot].SetDefaults(ItemID.ChainKnife);
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<BeanSummon>());
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<SaintRing>());
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<SoulPotion>());
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<FrostburnRune>());
            nextSlot++;

            if (Main.LocalPlayer.HasItem(ModContent.ItemType<VinePiercer>()))
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<PoisonRune>());
                nextSlot++;
            }

            if (Main.LocalPlayer.HasItem(ModContent.ItemType<FungiPiercer>()))
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<ShroomRune>());
                nextSlot++;
            }

            if (Main.LocalPlayer.HasItem(ModContent.ItemType<BlazePiercer>()))
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<FireRune>());
                nextSlot++;
            }
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                shop = true;
            }
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 23;
            knockback = 3f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 5;
            randExtraCooldown = 5;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<MerchantRune>();
            attackDelay = 1;
        }

        public override void TownNPCAttackMagic(ref float auraLightMultiplier)
        {
            base.TownNPCAttackMagic(ref auraLightMultiplier);
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 14f;
            randomOffset = 2f;
        }
    }
}