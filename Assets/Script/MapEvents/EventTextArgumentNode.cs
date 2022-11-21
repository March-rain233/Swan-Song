using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameToolKit.Dialog;
using GameToolKit;

internal class EventTextArgumentNode : SourceNode
{
    [Port("Value",  PortDirection.Output, new[] { typeof(TextArgument) })]
    [Port("Value", PortDirection.Input, true)]
    public EventPanel.EventTextArgument EventTextArgument;
    protected override void OnValueUpdate()
    {
        
    }
}

