using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FBase.Foundations
{
    public static class Randomizer
    {
        public static string CharacterSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public static Random RandomObject = new Random(DateTime.Now.Millisecond);

        #region strings

        public static char GenerateRandomCharacter(string characterSet = null)
        {
            Basics.SetIfEmptyStringRef(ref characterSet, CharacterSet);
            return characterSet[RandomObject.Next(0, characterSet.Length - 1)];
        }

        public static string GenerateString(int size, string characterSet = null)
        {
            StringBuilder SB = new StringBuilder();

            for (int i = 0; i < size; i++)
            {
                SB.Append(GenerateRandomCharacter(characterSet));
            }

            return SB.ToString();
        }
        #endregion

        #region Integers
        // Wrapper for the R.Next random function
        public static int GenerateRandomInteger(int minimum, int maximum)
        {
            return RandomObject.Next(minimum, maximum);
        }
        /// <summary>
        /// This function generates an integer array of length length whose entries
        /// are distinct and from the set [minimum, maximum], in no particular order.
        /// </summary>
        /// <param name="minimum">minimum integer</param>
        /// <param name="maximum">maximum integer</param>
        /// <param name="length">how many integers to generate</param>
        /// <returns></returns>
        public static int[] GenerateRandomDistinctIntegers(int minimum, int maximum, int length)
        {
            if (minimum > maximum)
            {
                Basics.Switch<int>(ref minimum, ref maximum);
            }

            if ((int)System.Math.Abs(minimum - maximum) + 1 < length)
            {
                throw new Exception("Excessive length specified for ensuring a set of distinct integers");
            }

            List<int> bankSet = new List<int>();
            for (int i = 0; i <= (int)System.Math.Abs(minimum - maximum); i++)
            {
                bankSet.Add(minimum + i);
            }

            // now eliminate
            int[] integerSet = new int[length];
            for (int j = 0; j < length; j++)
            {
                int chosen = GenerateRandomInteger(0, bankSet.Count);
                integerSet[j] = bankSet[chosen];
                bankSet.Remove(bankSet[chosen]);
            }

            return integerSet;
        }
        /// <summary>
        /// This function returns an integer array whose entries are distinct from the
        /// set [minimum, maximum], in no particular order
        /// </summary>
        /// <param name="minimum">minumum integer</param>
        /// <param name="maximum">maximum integer</param>
        /// <returns>an integer array of size abs(minimum - maximum) + 1 with distinct
        ///     values from the set [minimum, maximum]
        /// </returns>
        public static int[] GenerateRandomDistinctIntegers(int minimum, int maximum)
        {
            if (minimum > maximum)
            {
                Basics.Switch<int>(ref minimum, ref maximum);
            }

            int numItems = (int)System.Math.Abs(minimum - maximum) + 1;

            int[] integerSet = new int[numItems];
            int k = 0;
            for (int j = minimum; j <= maximum; j++)
            {
                integerSet[k] = j;
                k++;
            }
            // Scramble ITimes;
            for (int i = 0; i < numItems; i++)
            {
                Basics.Switch<int>(ref integerSet[GenerateRandomInteger(0, numItems)],
                                   ref integerSet[GenerateRandomInteger(0, numItems)]);
            }

            return integerSet;
        }
        #endregion

        #region Real Numbers

        /// <summary>
        /// Returns a double value between min and max, inclusive.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double RandomDouble(double min, double max)
        {
            double d = RandomObject.NextDouble();

            return (max - min) * d + min;

        }

        /// <summary>
        /// Returns a double value from baseNumber - allowance to baseNumber + allowance, inclusive.
        /// </summary>
        /// <param name="baseNumber"></param>
        /// <param name="allowance"></param>
        /// <returns></returns>
        public static double RandomOffset(double baseNumber, double allowance)
        {
            int sign = GenerateRandomInteger(0, 1);
            return baseNumber + ((sign == 0) ? 1 : -1) * RandomDouble(0, allowance);
        }

        #endregion

        #region List
        /// <summary>
        /// Select random item from an arbitrary list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T SelectRandom<T>(IList<T> list)
        {
            return list[RandomObject.Next(0, list.Count)];
        }

        /// <summary>
        /// Select random n distinct items from the given list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static IList<T> SelectRandom<T>(IList<T> list, int n)
        {
            if (n >= list.Count)
            {
                return list;
            }
            else
            {
                List<T> toReturn = new List<T>();

                int[] indices = GenerateRandomDistinctIntegers(0, list.Count, n);
                for (int i = 0; i < indices.Length; i++)
                {
                    toReturn.Add(list[indices[i]]);
                }

                return toReturn;
            }
        }

        /// <summary>
        /// Scrambles the order of the items of a list. The original list is not affected.        /// 
        /// </summary>
        /// <typeparam name="T">Any arbitrary object</typeparam>
        /// <param name="list">List of an arbitrary object</param>
        /// <returns>a new list whose entries are equivalent to the originals, however, in a scrambled order</returns>
        public static IList<T> Scramble<T>(List<T> list)
        {
            T[] array = list.ToArray();
            List<T> newList = new List<T>();
            int numItems = array.Length;
            for (int i = 0; i < numItems; i++)
            {
                Basics.Switch<T>(ref array[GenerateRandomInteger(0, numItems)],
                                   ref array[GenerateRandomInteger(0, numItems)]);
            }

            //newList = Basics.ArrayToList<T>(array);
            //return newList;
            return array.ToList();
        }

        #endregion

    }
}
