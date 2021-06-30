using OvermorrowMod.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Weapons.Artifacts
{
    public class CorruptedMirror : Artifact
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corrupted Mirror");
            Tooltip.SetDefault("[c/00FF00:{ Artifact }]\nConsume 2 Soul Essences to gain a buff that reflects all damage for 1 minute\n" +
                "All players on the same team gain the same buff for 1 minute\n" +
                "'You can't shake the feeling of something otherworldy gazing from the mirror'");
        }

        public override void SafeSetDefaults()
        {
            item.width = 28;
            item.height = 44;
            item.rare = ItemRarityID.Green;
            item.useAnimation = 32;
            item.useTime = 32;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.noMelee = true;
            item.UseSound = SoundID.Item103;
            item.consumable = false;
            item.autoReuse = false;
        }

        public override bool CanUseItem(Player player)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            if (modPlayer.soulResourceCurrent >= 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool UseItem(Player player)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                ConsumeSouls(2, player);
            }
            player.AddBuff(ModContent.BuffType<MirrorBuff>(), 3600);

            // Loop through all players and check if they are on the same team
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (Main.player[i].team == player.team && player.team != 0)
                    {
                        Main.player[i].AddBuff(ModContent.BuffType<MirrorBuff>(), 3600);
                    }
                }
            }
            return true;
        }
    }
}