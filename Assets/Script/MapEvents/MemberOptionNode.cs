using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameToolKit.Dialog;

internal class MemberOptionNode : OptionNode
{
    protected override IEnumerable<OptionArgument> OnGetOptions()
    {
        var list = new List<OptionArgument>();
        foreach(var member in GameManager.Instance.GameData.Members)
        {
            list.Add(new OptionArgument()
            {
                IsEnable = true,
                Option = $"{member.Name}"
            });
        }
        return list;
    }
}
