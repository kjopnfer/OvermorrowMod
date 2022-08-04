using Microsoft.Xna.Framework;
using System.ComponentModel;
using Terraria;
using Terraria.ModLoader.Config;

namespace Overmorrow.Common
{ 
    [BackgroundColor(40, 15, 40, 200)]
    public class Config : ModConfig
    {
        const int embedRed = 60;
        const int embedGreen = 15;
        const int embedBlue = 120;
        const int embedAlpha = 255;
        public static Color InTrajectorySend;
        public static Color OutTrajectorySend;
        /*public static float chargeMeterXSend;
        public static float chargeMeterYSend;*/
        public static bool bowChargeLockSend;
        public static bool improveBowsSend;
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [BackgroundColor(embedRed, embedGreen, embedBlue, embedAlpha)]
        [DefaultValue(true)]
        public bool gay;

        [Header("Vanilla Feature Enhancements")]
        [Label("Complete bow overhaul")]
        [BackgroundColor(embedRed, embedGreen, embedBlue, embedAlpha)]
        [DefaultValue(true)]
        public bool improveBows;

        [Label("Lock Bow Charge Meter position")]
        [BackgroundColor(embedRed, embedGreen, embedBlue, embedAlpha)]
        [DefaultValue(false)]
        public bool lockBowChargePos;

        [Label("Bow Trajectory Telegraph Color")]
        [BackgroundColor(embedRed, embedGreen, embedBlue, embedAlpha)]
        public bowSimpleData Trolg;



        /*[Label("Bow Charge Meter position")]
        [BackgroundColor(embedRed, embedGreen, embedBlue, embedAlpha)]
        public chargeMeterSimpleData chargeSimple;  */

        public Config()
        {
            Trolg = new bowSimpleData()
            {
                OuterArrowTrajectory = new Color(255, 255, 255, 255),
                InnerArrowTrajectory = new Color(255, 255, 255, 255)
            };
            /*chargeSimple = new chargeMeterSimpleData()
            {
                chargeMeterX = 744,
                chargeMeterY = 405
            };*/
        }
        public override void OnChanged()
        {
            Player player = Main.LocalPlayer;
            InTrajectorySend = Trolg.InnerArrowTrajectory;
            OutTrajectorySend = Trolg.OuterArrowTrajectory;
            bowChargeLockSend = lockBowChargePos;
            improveBowsSend = improveBows;
            //todo loop through all items and SetDefaults();
        }
    }
    public class bowSimpleData
    {
        const int embedRed = 120;
        const int embedGreen = 15;
        const int embedBlue = 120;
        const int embedAlpha = 255;
        [Label("Outer Trajectory Telegraph Color")]
        [BackgroundColor(embedRed, embedGreen, embedBlue, embedAlpha)]
        [DefaultValue(typeof(Color), "255, 255, 255, 255")]
        public Color OuterArrowTrajectory;

        [Label("Inner Trajectory Telegraph Color")]
        [BackgroundColor(embedRed, embedGreen, embedBlue, embedAlpha)]
        [DefaultValue(typeof(Color), "255, 255, 255, 255")]
        public Color InnerArrowTrajectory;
    }
    /*public class chargeMeterSimpleData
    {
        const int embedRed = 120;
        const int embedGreen = 15;
        const int embedBlue = 120;
        const int embedAlpha = 255;

        [Label("Bow Charge Meter X position")]
        [BackgroundColor(embedRed, embedGreen, embedBlue, embedAlpha)]
        [Range(0, 1600f)]
        [DefaultValue(744)] //800-56
        public float chargeMeterX;
        [Label("Bow Charge Meter Y position")]
        [BackgroundColor(embedRed, embedGreen, embedBlue, embedAlpha)]
        [Range(0, 900f)]
        [DefaultValue(405)]
        public float chargeMeterY;
    }*/
}