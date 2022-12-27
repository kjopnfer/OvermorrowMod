using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Overmorrow.Common;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace OvermorrowMod.Common.VanillaOverrides.Bow
{
    public class TrajectoryTelegraph : UIElement
    {
        Color color = new Color(255, 255, 255, 175);
        /*public const int chargeUiWidth = 56;
        public const int chargeUiHeight = 54;
        public static Vector2 defaultPos = new Vector2((Main.screenWidth / 2) - (TrajectoryTelegraph.chargeUiWidth / 2), (Main.screenHeight / 2) - 45);*/
        double vectDist;
        void getDistance(Vector2 pointA, Vector2 pointB)
        {
            vectDist = Math.Pow((pointA.X - pointB.X), 2) + Math.Pow((pointA.Y - pointB.Y), 2);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            TrajectoryPlayer trajectoryPlayer = Main.LocalPlayer.GetModPlayer<TrajectoryPlayer>();

            //trajectory telegraph
            if (trajectoryPlayer.drawTrajectory && !Main.LocalPlayer.dead)
            {
                Player player = Main.LocalPlayer;
                float distanceX = (Main.MouseWorld.X - trajectoryPlayer.trajPointX) * -1;
                float distanceY = (Main.MouseWorld.Y - trajectoryPlayer.trajPointY) * -1;
                //Main.screenPosition = Main.MouseWorld * -1; ech
                /*Main.NewText("mousewolrdX: " + Main.MouseWorld.X + " mouseworldY: " + Main.MouseWorld.Y);
                Main.NewText("trajPointX: " + trajectoryPlayer.trajPointX + " trajPointY: " + trajectoryPlayer.trajPointY);
                Main.NewText("distx: " + distanceX + " disty: " + distanceY);
                Main.NewText(vectDist + 100000);*/
                getDistance(Main.MouseWorld, new Vector2(trajectoryPlayer.trajPointX, trajectoryPlayer.trajPointY));
                int dots = (((int)vectDist + 100000) / 100000);
                if ((int)vectDist + 100000 >= 125000)
                    dots++;
                if ((int)vectDist + 100000 >= 175000)
                    dots++;
                float scale = 1.5f;
                //Color color = new Color(255 - (51 * dots), 0f + (51 * dots), 0, 175);
                //color = new Color(255 - (25 * dots), (25 * dots), 0, 255);
                for (int i = 0; dots > i; i++)
                {
                    spriteBatch.Draw(ModContent.Request<Texture2D>("OvermorrowMod/Content/UI/trajectoryOut").Value, new Vector2((player.position.X - Main.screenPosition.X) + (distanceX / (dots + 1)) * (i + 1), ((player.position.Y + (player.height / 2)) - Main.screenPosition.Y) + (distanceY / dots) * (i + 1)), null, Config.OutTrajectorySend, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    spriteBatch.Draw(ModContent.Request<Texture2D>("OvermorrowMod/Content/UI/trajectoryIn").Value, new Vector2((player.position.X - Main.screenPosition.X) + (distanceX / (dots + 1)) * (i + 1), ((player.position.Y + (player.height / 2)) - Main.screenPosition.Y) + (distanceY / dots) * (i + 1)), null, Config.InTrajectorySend, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    scale -= 0.05f; //.15
                }
            }
        }
    }

    public class TrajectoryDraw : UIState
    {
        public TrajectoryTelegraph trajTele;

        public override void OnInitialize()
        {
            trajTele = new TrajectoryTelegraph();
            Append(trajTele);
        }
    }
}