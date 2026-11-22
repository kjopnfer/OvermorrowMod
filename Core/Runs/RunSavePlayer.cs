using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Misc;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.UI.ClassSelection;
using OvermorrowMod.Core.WorldGeneration;
using OvermorrowMod.Core.WorldGeneration.TestSubworld;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Core.Runs
{
    public class RunSavePlayer : ModPlayer
    {
        public string ActiveSubworld;
        public Guid MainWorldId;
        public string BiomeName;
        public int TimerTicks = -1;
        public readonly Dictionary<string, Vector2> LastPosition = new();

        private Vector2? lastSafePosition;
        private bool restorePending;

        public bool PendingReset;
        public CharacterClass? PendingNewRun;

        public bool HasSavedRun => ActiveSubworld != null && MainWorldId == Main.ActiveWorldFileData.UniqueId;

        public void ClearRun()
        {
            ActiveSubworld = null;
            MainWorldId = Guid.Empty;
            BiomeName = null;
            TimerTicks = -1;
            LastPosition.Clear();
        }

        /// <summary>
        /// Standing on real tiles only. Bridges, stairs, shelves and ladders get their collision from NPCs that spawn after load, so a player restored onto one would fall through.
        /// </summary>
        private static bool HasSolidGround(Vector2 position, int width, int height)
        {
            int row = (int)((position.Y + height + 2f) / 16f);
            int left = (int)(position.X / 16f);
            int right = (int)((position.X + width - 1f) / 16f);
            for (int x = left; x <= right; x++)
            {
                Tile tile = Framing.GetTileSafely(x, row);
                if (tile.HasTile && !tile.IsActuated && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType])) return true;
            }
            return false;
        }

        private bool IsSafeStanding() => !Player.dead && Player.velocity.Y == 0f && !Player.wet && !Player.pulley && Player.grapCount == 0 && Player.gravDir == 1f && HasSolidGround(Player.position, Player.width, Player.height) && !CombatOrchestrator.AnyFightRunning();

        public void CommitPosition()
        {
            if (lastSafePosition is Vector2 position && ActiveSubworld != null)
                LastPosition[ActiveSubworld] = position;
        }

        public void PrepareCheckpoint()
        {
            if (IsSafeStanding()) lastSafePosition = Player.position;
        }

        public override void OnEnterWorld()
        {
            if (RunState.InRun)
            {
                Subworld current = SubworldSystem.Current;
                ActiveSubworld = current.FullName;
                MainWorldId = Guid.Parse(Path.GetFileName(Path.GetDirectoryName(SubworldSystem.CurrentPath)));
                BiomeName = current is TestSubworld test ? test.TitleCardText : current.DisplayName.Value;
                lastSafePosition = null;
                restorePending = LastPosition.ContainsKey(ActiveSubworld);
                return;
            }

            if (PendingReset && !SubworldSystem.AnyActive())
            {
                PendingReset = false;
                RunReset.Reset(Player);
            }
        }

        public override void PreUpdate()
        {
            if (Player.whoAmI != Main.myPlayer || !restorePending) return;

            restorePending = false;
            Vector2 position = LastPosition[ActiveSubworld];
            if (Collision.SolidCollision(position, Player.width, Player.height) || !HasSolidGround(position, Player.width, Player.height)) return;

            Player.position = position;
            Player.velocity = Vector2.Zero;
            Player.fallStart = (int)(position.Y / 16f);
        }

        public override void PostUpdate()
        {
            if (Player.whoAmI != Main.myPlayer) return;

            if (RunState.InRun && IsSafeStanding())
                lastSafePosition = Player.position;

            if (PendingNewRun is CharacterClass characterClass && !SubworldSystem.AnyActive() && ClassSelection.BeginRun(Player, characterClass))
                PendingNewRun = null;
        }

        public override void SaveData(TagCompound tag)
        {
            if (ActiveSubworld == null) return;

            if (Player.GetModPlayer<SubworldPlayer>().inSubworldNow)
            {
                CommitPosition();
                TimerTicks = SubworldTimer.Running ? SubworldTimer.RemainingTicks : -1;
            }

            tag["ActiveSubworld"] = ActiveSubworld;
            tag["MainWorldId"] = MainWorldId.ToString();
            tag["BiomeName"] = BiomeName ?? "";
            tag["TimerTicks"] = TimerTicks;
            tag["LastPositionKeys"] = LastPosition.Keys.ToList();
            tag["LastPositionValues"] = LastPosition.Values.ToList();
        }

        public override void LoadData(TagCompound tag)
        {
            string active = tag.GetString("ActiveSubworld");
            ActiveSubworld = active == "" ? null : active;
            Guid.TryParse(tag.GetString("MainWorldId"), out MainWorldId);
            BiomeName = tag.GetString("BiomeName");
            TimerTicks = tag.ContainsKey("TimerTicks") ? tag.GetInt("TimerTicks") : -1;

            LastPosition.Clear();
            IList<string> keys = tag.GetList<string>("LastPositionKeys");
            IList<Vector2> values = tag.GetList<Vector2>("LastPositionValues");
            for (int i = 0; i < keys.Count; i++)
                LastPosition[keys[i]] = values[i];
        }
    }
}
