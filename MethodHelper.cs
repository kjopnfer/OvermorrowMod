using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod
{
    public static class MethodHelper
    {
        public static float Percentage(float percent)
        {
            float percentage = percent / 100;
            return percentage;
        }
        public static int SecondsToTicks(int seconds)
        {
            return seconds * 60;
        }
        public static int GetProjAmount(int projectile)
        {
            int projCount = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].type == projectile && Main.projectile[i].active)
                {
                    projCount++;
                }
            }
            return projCount;
        }
        public static Vector2 GetRandomVector(int MaxX, int MaxY, int MinX = 0, int MinY = 0)
        {
            return new Vector2(Main.rand.Next(MinX, MaxX), Main.rand.Next(MinY, MaxY));
        }
    }
}
