using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Confusion : RoundBuff
{
    //混乱状态
    public override BuffData GetBuffData()
    {
        var data = base.GetBuffData();
        data.Name = "混乱";
        data.Description = "释放的卡牌有50%概率失效";
        return data;
    }
}