using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ϰ���ͼ��
/// </summary>
public class BarrierTile : Tile
{
    public override void AddStatus<TStatus>(TStatus status)
    {
        
    }

    protected override void OnEnter(Unit unit)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnExit(Unit unit)
    {
        throw new System.NotImplementedException();
    }

    public override bool CheckPlaceable(Unit unit)
    {
        return false;
    }
}
