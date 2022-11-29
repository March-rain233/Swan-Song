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
        //简单的节点分配，后续可能考虑到关卡合理性会有改动
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
        //随机生成节点层数 7 - 9层  第一层和最后一层固定
        int step = random.Next(7, 10);
        TreeMap map = new TreeMap();
        int[][] nodes = new int[step][];

        //生成并加入开始节点，开始节点占第一层
        nodes[0] = new int[1];
        nodes[0][0] = map.AddNode(new TreeMapNodeData() { PlaceType = PlaceType.Start });

        //开始中间节点层的生成和分配
        for (int i = 1; i < step - 1; ++i)
        {
            //分配有多少个非战斗节点和多少个战斗节点
            int NodeNum = random.Next(2, 5);
            int BattleNum = random.Next(1, NodeNum + 1);
            PlaceType[] Choose = new PlaceType[NodeNum];//记录每个节点的type
            //先按顺序把所有type加进去，然后进行打乱 达到随机的效果
            int j = 0;
            for (; j < BattleNum; ++j)
            {
                Choose[j] = Battles[random.Next(2)];
            }
            for (; j < NodeNum; ++j)
            {
                Choose[j] = NotBattle[random.Next(5)];
            }

            //打乱，先生成一个int数组记录index，然后打乱这个数组，在加入节点的时候使用该数组记录值作为下标，达到打乱的效果  不过有更简单的方法吗（？）
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

        //生成并加入最后的boss关卡
        nodes[step - 1] = new int[1];
        nodes[step - 1][0] = map.AddNode(new TreeMapNodeData() { PlaceType = PlaceType.BossBattle });


        //然后是节点连接的策略
        for (int i = 0; i < step - 1; ++i)
        {
            int l1 = nodes[i].Length;
            int l2 = nodes[i + 1].Length;
            int curIndex = 0;
            for (int j = 0; j < l1; ++j)
            {
                int k = curIndex;
                //随机决定要不要从上个节点结束的位置开始连接动作 当然第一个节点不可以有这个动作 因为他只能从0开始
                if (j != 0 && curIndex < l2 - 1)
                {
                    k = random.Next(curIndex, curIndex + 2);
                }
                if (j == l1 - 1)//判断当前是不是需要分配的最后一个节点 如果是的话把剩下的节点全部分配给他
                {
                    for (; k < l2; k++)
                    {
                        map.Connect(nodes[i][j], nodes[i + 1][k]);
                    }
                }
                else
                {
                    //表示要连接下一层几个节点
                    int conn_node = random.Next(1, l2 - curIndex + 1);
                    for (; k < curIndex + conn_node; ++k)
                    {
                        map.Connect(nodes[i][j], nodes[i + 1][k]);
                    }
                    curIndex += conn_node - 1;
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
