using Microsoft.Xna.Framework;
using OvermorrowMod.Core.UI;
using OvermorrowMod.Core.WorldGeneration;
using SubworldLibrary;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace OvermorrowMod.Core.Runs
{
    public class RunDeathPlayer : ModPlayer
    {
        public bool RunDead { get; private set; }
        public bool RunOver { get; private set; }
        public string DeathText { get; private set; }
        public string BiomeName { get; private set; }

        private Vector2 deathPosition;

        public static bool LocalDead => !Main.gameMenu && Main.LocalPlayer.GetModPlayer<RunDeathPlayer>().RunDead;

        public static bool IsTargetable(Player player) => player.active && !player.dead && !player.GetModPlayer<RunDeathPlayer>().RunDead;

        public void Clear()
        {
            RunDead = false;
            RunOver = false;
            DeathText = null;
            BiomeName = null;
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (!RunState.InRun) return true;

            Player.statLife = 1;
            if (RunDead || Player.whoAmI != Main.myPlayer) return false;

            RunDead = true;
            RunOver = true;
            DeathText = damageSource.GetDeathText(Player.name).ToString();
            BiomeName = Player.GetModPlayer<RunSavePlayer>().BiomeName;
            deathPosition = Player.position;

            CloseInterfaces();
            Player.mount.Dismount(Player);
            Player.RemoveAllGrapplingHooks();
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                Player.buffType[i] = 0;
                Player.buffTime[i] = 0;
            }

            SoundEngine.PlaySound(SoundID.PlayerKilled, Player.Center);
            for (int i = 0; i < 100; i++)
                Dust.NewDust(Player.position, Player.width, Player.height, DustID.Blood, 2 * hitDirection, -2f);

            Main.NewText(DeathText, 225, 25, 25);
            Player.SavePlayer(Main.ActivePlayerFileData);
            return false;
        }

        private void CloseInterfaces()
        {
            Main.playerInventory = false;
            Main.mapFullscreen = false;
            Player.chest = -1;
            Player.SetTalkNPC(-1);
            Main.npcChatText = "";
            if (Main.inFancyUI) IngameFancyUI.Close();
            RewardSelectionManager.Hide();
        }

        public override void SetControls()
        {
            if (!RunDead) return;

            Player.controlLeft = false;
            Player.controlRight = false;
            Player.controlUp = false;
            Player.controlDown = false;
            Player.controlJump = false;
            Player.controlUseItem = false;
            Player.controlUseTile = false;
            Player.controlThrow = false;
            Player.controlInv = false;
            Player.controlHook = false;
            Player.controlTorch = false;
            Player.controlSmart = false;
            Player.controlMount = false;
            Player.controlQuickHeal = false;
            Player.controlQuickMana = false;
            Player.controlCreativeMenu = false;
            Player.controlMap = false;
            Player.mapFullScreen = false;
            Player.mapStyle = false;
            Player.mapAlphaUp = false;
            Player.mapAlphaDown = false;
            Player.mapZoomIn = false;
            Player.mapZoomOut = false;
            Player.controlDownHold = false;
        }

        public override void PreUpdateMovement()
        {
            if (RunDead) Pin();
        }

        public override void PostUpdate()
        {
            if (Player.whoAmI != Main.myPlayer) return;

            if (RunDead)
            {
                Pin();
                Player.immune = true;
                Player.immuneNoBlink = true;
                Player.immuneTime = 2;
                Player.breath = Player.breathMax;
                Player.statLife = 1;
                Main.playerInventory = false;
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    Player.buffType[i] = 0;
                    Player.buffTime[i] = 0;
                }
            }

            if (RunOver && !SubworldSystem.AnyActive()) RunReset.Reset(Player);
        }

        private void Pin()
        {
            Player.position = deathPosition;
            Player.velocity = Vector2.Zero;
            Player.fallStart = (int)(deathPosition.Y / 16f);
        }

        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            if (!RunDead) return;

            foreach (PlayerDrawLayer layer in PlayerDrawLayerLoader.Layers)
                layer.Hide();
        }

        public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot) => !RunDead;

        public override bool CanBeHitByProjectile(Projectile proj) => !RunDead;

        public override void SaveData(TagCompound tag)
        {
            if (RunOver) tag["RunOver"] = true;
        }

        public override void LoadData(TagCompound tag) => RunOver = tag.GetBool("RunOver");
    }
}
