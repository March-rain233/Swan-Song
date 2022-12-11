using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameToolKit.Dialog;
using GameToolKit;

public class RandomSelectorNode : ProcessNode
{
    [Port("Input", PortDirection.Input)]
    public int Input;
    protected override void OnPlay()
    {
        Finish();
    }
    protected override void RunSubsequentNode()
    {
        var num = Children.Count;
        var v = UnityEngine.Random.Range(0, num);
        Children[(Input + v) % num].Play();
    }
}
