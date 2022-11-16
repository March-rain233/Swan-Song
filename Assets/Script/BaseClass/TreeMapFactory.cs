using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class TreeMapFactory
{
    /// <summary>
    /// 根据传入描述创建地图
    /// </summary>
    /// <param name="description">描述</param>
    public static TreeMap CreateTreeMap(string description)
    {
        //todo
        PlaceType[] Battles = new PlaceType[2]
        {
            PlaceType.NormalBattle,
            PlaceType.AdvancedBattle
        };
        PlaceType[] NotBattle = new PlaceType[5]
        {
            PlaceType.BonFire,
            PlaceType.PlatForm,
            PlaceType.Store,
            PlaceType.Casino,
            PlaceType.FightForJus
        };
        Random random = new Random();
        int step = random.Next(7, 10);
        TreeMap map = new TreeMap();
        int[][] nodes = new int[step][];
        nodes[0] = new int[1];
        nodes[0][0] = map.AddNode(new TreeMapNodeData() { PlaceType = PlaceType.Start });


        for (int i = 1; i < step - 1; ++i)
        {
            //分配有多少个节点 和多少战斗节点
            int NodeNum = random.Next(2, 5);
            int BattleNum = random.Next(1, NodeNum + 1);
            PlaceType[] Choose = new PlaceType[NodeNum];
            int j = 0;
            for (; j < BattleNum; ++j)
            {
                Choose[j] = Battles[random.Next(2)];
            }
            for (; j < NodeNum; ++j)
            {
                Choose[j] = NotBattle[random.Next(5)];
            }

            //打乱index 然后在接下来赋予节点的时候 在已经有的节点里面随机挑选一个赋值
            int[] temp = new int[NodeNum];
            for (j = 0; j < NodeNum; j++)
            {
                temp[j] = j;
            }
            RandomArr(temp);


            nodes[i] = new int[NodeNum];
            for (j = 0; j < NodeNum; ++j)
            {
                nodes[i][j] = map.AddNode(new TreeMapNodeData() { PlaceType = Choose[temp[j]] });
            }
        }
        nodes[step - 1] = new int[1];
        nodes[step - 1][0] = map.AddNode(new TreeMapNodeData() { PlaceType = PlaceType.BossBattle });

        for (int i = 1; i < step; ++i)
        {
            int l1 = nodes[i - 1].Length;
            int l2 = nodes[i].Length;
            //int l3 = nodes[i + 1].Length;
            for (int j = 0; j < l2; j++)
            {
                int PreConn = random.Next(1, l1 + 1);
                int[] pre = new int[l1];
                for (int k = 0; k < PreConn; k++)
                {
                    pre[k] = 1;
                }
                RandomArr(pre);
                for (int k = 0; k < l1; k++)
                {
                    if (pre[k] == 1) map.Connect(nodes[i - 1][k], nodes[i][j]);
                }

            }


        }
        for (int i = 1; i < step - 1; ++i)
        {
            int l1 = nodes[i].Length;
            int l2 = nodes[i + 1].Length;
            for (int j = 0; j < l1; j++)
            {
                if (!map.HavingChildren(nodes[i][j]))
                {
                    int nextnode = random.Next(l2);
                    map.Connect(nodes[i][j], nodes[i + 1][nextnode]);

                }
            }
        }
        return map;
    }
    static void RandomArr(int[] arr)
    {
        Random r = new Random();//创建随机类对象，定义引用变量为r
        for (int i = 0; i < arr.Length; i++)
        {
            int index = r.Next(arr.Length);//随机获得0（包括0）到arr.Length（不包括arr.Length）的索引
                                           //Console.WriteLine("index={0}", index);//查看index的值
            int temp = arr[i];  //当前元素和随机元素交换位置
            arr[i] = arr[index];
            arr[index] = temp;
        }
    }

}