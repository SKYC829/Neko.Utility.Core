using Neko.Utility.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neko.Utility.Core.Common
{
    /// <summary>
    /// <see cref="Random"/>帮助类
    /// <para>用于生成随机数字字符串(基于<see cref="Guid"/>的<see cref="Guid.GetHashCode"/>)</para>
    /// <para>以及根据权重从一个列表中随机返回一定数量的元素(可用于抽奖等场景)</para>
    /// </summary>
    public sealed class RandomUtil
    {
        /// <summary>
        /// 随机生成一组数字
        /// </summary>
        /// <param name="count">要生成的数字的个数</param>
        /// <param name="canRepeat">是否允许重复数字</param>
        /// <returns></returns>
        public static int[] Next(int count,bool canRepeat = false)
        {
            return Next(new Random(), count, canRepeat);
        }

        /// <summary>
        /// 随机生成一组数字
        /// </summary>
        /// <param name="random">随机数生成器</param>
        /// <param name="count">要生成的数字的个数</param>
        /// <param name="canRepeat">是否允许重复数字</param>
        /// <returns></returns>
        public static int[] Next(Random random,int count,bool canRepeat = false)
        {
            List<int> result = new List<int>();
            if(random == null)
            {
                return result.ToArray();
            }
            for (int i = 0; i < count; i++)
            {
                int item = random.Next(count + i);
                if(result.Contains(item) && !canRepeat)
                {
                    i--;
                    continue;
                }
                result.Add(item);
            }
            return result.ToArray();
        }

        /// <summary>
        /// 获取一组对象中的前<paramref name="count"/>个元素
        /// </summary>
        /// <typeparam name="Titem">对象的类型</typeparam>
        /// <param name="items">要获取元素的数组</param>
        /// <param name="count">要获取元素的个数</param>
        /// <returns></returns>
        public static Titem[] Draw<Titem>(IList<Titem> items,int count)
        {
            List<double> odds = new List<double>();
            for (int i = 0; i < items.Count; i++)
            {
                odds.Add(1);
            }
            return Draw<Titem>(items, odds, count);
        }

        /// <summary>
        /// 根据<paramref name="odds"/>分配的权重随机返回一组对象中前<paramref name="count"/>个元素
        /// </summary>
        /// <typeparam name="Titem">对象的类型</typeparam>
        /// <param name="random">随机数生成器</param>
        /// <param name="items">要获取元素的数组</param>
        /// <param name="odds">分配的权重,权重元素位置与<paramref name="items"/>的元素位置相对应</param>
        /// <param name="count">要获取元素的个数</param>
        /// <returns></returns>
        public static Titem[] Draw<Titem>(IList<Titem> items,IList<double> odds,int count)
        {
            Random random = new Random();
            Dictionary<Titem, double> oddsMap = new Dictionary<Titem, double>();
            for (int i = 0; i < items.Count; i++)
            {
                oddsMap.Add(items.ElementAt(i), Math.Max((double)0, (double)(random.Next(100) * odds.ElementAt(i))));
            }
            return Draw(oddsMap, count);
        }

        /// <summary>
        /// 根据<paramref name="oddsMap"/>的值进行排序并返回前<paramref name="count"/>个元素
        /// </summary>
        /// <typeparam name="Titem">对象的类型</typeparam>
        /// <param name="oddsMap">元素和权重的键值对</param>
        /// <param name="count">要获取元素的个数</param>
        /// <returns></returns>
        public static Titem[] Draw<Titem>(IDictionary<Titem, double> oddsMap,int count)
        {
            List<Titem> results = new List<Titem>();
            if(oddsMap == null || oddsMap.Count <= 0 || oddsMap.Count < count)
            {
                return results.ToArray();
            }
            List<KeyValuePair<Titem, double>> sortOddsMap = DictionaryUtil.SortDictionary<Titem, double>(oddsMap).ToList().GetRange(0, count);
            for (int i = 0; i < sortOddsMap.Count; i++)
            {
                KeyValuePair<Titem, double> result = sortOddsMap.ElementAt(i);
                results.Add(result.Key);
            }
            return results.ToArray();
        }
        /// <summary>
        /// 生成随机数字字符串
        /// <para>注意:因为是根据<see cref="Guid.GetHashCode"/>来生成的随机数,<br/>
        /// 所以短时间内多次调用此方法的话,生成重复的字符串的概率较大</para>
        /// </summary>
        /// <param name="length">字符串的长度</param>
        /// <returns></returns>
        public static string GenerateRandomNo(int length)
        {
            int seed = Guid.NewGuid().GetHashCode();
            Random random = new Random(seed);
            int[] index = new int[length];
            for (int i = 0; i < length; i++)
            {
                index[i] = i + 1;
            }
            int[] results = new int[length];
            int cursor = 0,max = length;
            for (int i = 0; i < length; i++)
            {
                cursor = random.Next(0, max - 1);
                results[i] = index[cursor];
                index[cursor] = index[max - 1];
                max--;
            }
            string result = string.Empty;
            for (int i = 0; i < results.Length; i++)
            {
                result += results.ElementAt(i).ToString();
            }
            if(result.Length > length)
            {
                result = result.Substring(0, length);
            }
            return result;
        }
    }
}
