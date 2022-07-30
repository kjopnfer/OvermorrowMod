using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Bow
{
    //useless useless useless ...for now?
    //Its not usless, I need for 1.4 port
    public class TrajectoryPlayer : ModPlayer
    {
        public float trajPointX;
        public float trajPointY;
        public bool drawTrajectory;
        public bool drawChargeBar;
        public int bowTiming;
        public int bowTimingMax = 180; // 3 seconds
        public int bowTimingReduce = 0;
        public int chargeVelocityDivide = 1;

        public override void ResetEffects()
        {
            Player player = Main.LocalPlayer;
            trajPointX = player.Center.X;
            trajPointY = player.Center.Y;
            /*drawChargeBar = player.HeldItem.type == ItemType<BowButCool>();
            drawTrajectory = (player.channel && player.HeldItem.type == ItemType<BowButCool>());*/
            drawChargeBar = false;
            drawTrajectory = false;
        }
    }
}