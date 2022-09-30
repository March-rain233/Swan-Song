using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class GameStatus
{
    protected internal abstract void OnUpdata();

    protected internal abstract void OnEnter();

    protected internal abstract void OnExit();
}