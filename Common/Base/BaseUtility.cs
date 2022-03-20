using Microsoft.Xna.Framework;
using System;

namespace OvermorrowMod.Common.Base
{
    public class BaseUtility
    {
        //------------------------------------------------------//
        //------------------BASE UTILITY CLASS------------------//
        //------------------------------------------------------//
        // Contains utility methods a mod might want to use.    //
        //------------------------------------------------------//
        //  Author(s): Grox the Great                           //
        //------------------------------------------------------// 


        /*
         * Combines two int arrays.
         */
        public static int[] CombineArrays(int[] array1, int[] array2)
        {
            int[] newArray = new int[array1.Length + array2.Length];
            for (int m = 0; m < array1.Length; m++) { newArray[m] = array1[m]; }
            for (int m = 0; m < array2.Length; m++) { newArray[array1.Length + m] = array2[m]; }
            return newArray;
        }

        /*
         * Returns true if value is in the given int array.
         */
        public static bool InArray(int[] array, int value)
        {
            for (int m = 0; m < array.Length; m++) { if (value == array[m]) { return true; } }
            return false;
        }

        /*
         * Returns true if value is in the given int array.
         * 
         * index : sets this to the index of the value in the array.
         */
        public static bool InArray(int[] array, int value, ref int index)
        {
            for (int m = 0; m < array.Length; m++) { if (value == array[m]) { index = m; return true; } }
            return false;
        }

        /*
		 * Allows lerping between N float values.
		 */
        public static float MultiLerp(float percent, params float[] floats)
        {
            float per = 1f / ((float)floats.Length - 1);
            float total = per;
            int currentID = 0;
            while ((percent / total) > 1f && (currentID < floats.Length - 2)) { total += per; currentID++; }
            return MathHelper.Lerp(floats[currentID], floats[currentID + 1], (percent - (per * currentID)) / per);
        }

        /*
         * Returns a rotation from startPos pointing to endPos.
         */
        public static float RotationTo(Vector2 startPos, Vector2 endPos)
        {
            return (float)Math.Atan2(endPos.Y - startPos.Y, endPos.X - startPos.X);
        }

        /*
         * Rotates a vector based on the origin and the given point to 'look' at.
         * The rotation vector is *NOT* relative to the origin.
         */
        public static Vector2 RotateVector(Vector2 origin, Vector2 vecToRot, float rot)
        {
            float newPosX = (float)(Math.Cos(rot) * (vecToRot.X - origin.X) - Math.Sin(rot) * (vecToRot.Y - origin.Y) + origin.X);
            float newPosY = (float)(Math.Sin(rot) * (vecToRot.X - origin.X) + Math.Cos(rot) * (vecToRot.Y - origin.Y) + origin.Y);
            return new Vector2(newPosX, newPosY);
        }
    }
}