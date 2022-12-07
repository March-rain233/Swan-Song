using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 可配置权重的随机数生成器
/// </summary>
internal class AdvanceRandom
{
    public struct Item
    {
        public int Value;
        public int Weight;
    }

    public List<Item> Pool;

    public int Draw()
    {
        return Draw(Pool);
    }

    public static int Draw(IEnumerable<Item> pool)
    {
        int sum = pool.Sum(e => e.Weight);
        int r = UnityEngine.Random.Range(0, sum);
        foreach (Item item in pool)
        {
            r -= item.Weight;
            if (r < 0)
            {
                return item.Value;
            }
        }
        throw new ArgumentException();
    }
}
