﻿/*using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Quests.Requirements;
using OvermorrowMod.Quests.Rewards;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Quests.ModQuests
{
    public class MultiStepQuest : BaseQuest
    {
        public override string QuestName => "Trip";

        public override QuestRepeatability Repeatability => QuestRepeatability.OncePerWorldPerPlayer;
        public override QuestType Type => QuestType.Fetch;
        public override int QuestGiver => NPCID.Guide;
        public override void SetDefaults()
        {
            Requirements = new[]
            {
                new ChainRequirement(
                    new IQuestRequirement[]
                    {
                        new ItemRequirement("wood", ItemID.Wood, 20, "Obtain 20 Wood", true),
                        new TravelRequirement(() => ProjectToGround(Main.spawnTileX + 20, Main.spawnTileY), "Travel to Right of Main Spawn", "t1"),
                        new TravelRequirement(() => ProjectToGround(Main.spawnTileX - 20, Main.spawnTileY), "Travel to Left of Main Spawn", "t2"),
                        new ItemRequirement("gold", ItemID.GoldCoin, 1, "Obtain 1 Gold Coin", true),
                        new MiscRequirement("campfire", "Relight the Campfire"),
                    }, "chain")
            };
            Rewards = new[]
            {
                new ItemReward(ItemID.DirtBlock, 1)
            };

            QuestDialogue.Add("Give me 20 wood, then go on a trip, and finally come back and giving me a gold coin.");
            QuestHint.Add("You got me that wood and gold yet?");
            QuestEndDialogue.Add("Thanks, sucker");
        }

        private bool CanStandOn(int x, int y)
        {
            var tile = Main.tile[x, y];
            return tile.HasTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]);
        }

        private bool CanStandIn(int x, int y)
        {
            var tile = Main.tile[x, y];
            return !tile.HasTile || !Main.tileSolid[tile.TileType];
        }

        private Vector2 ProjectToGround(int startX, int startY)
        {
            int y = startY;
            while (true)
            {
                if (!CanStandIn(startX, y))
                {
                    y -= 1;
                }
                else if (!CanStandOn(startX, y + 1))
                {
                    y += 1;
                }
                else
                {
                    break;
                }
            }
            return new Vector2(startX, y);
        }

        protected override bool IsValidFor(Player player)
        {
            return player.GetModPlayer<DialoguePlayer>().unlockedGuideCampfire;
        }
    }
}
*/