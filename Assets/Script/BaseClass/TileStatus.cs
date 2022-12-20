using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 图块的叠加状态
/// </summary>
public abstract class TileStatus
{
    public Tile Tile { get; private set; }

    /// <summary>
    /// 剩余计数
    /// </summary>
    public int Times;

    /// <summary>
    /// 该效果是否生效
    /// </summary>
    public bool IsEnable
    {
        get;
        private set;
    }

    /// <summary>
    /// 使效果生效
    /// </summary>
    public void Enable()
    {
        IsEnable = true;
        OnEnable();
    }

    /// <summary>
    /// 使效果失效
    /// </summary>
    public void Disable()
    {
        IsEnable = false;
        OnDisable();
    }

    protected abstract void OnEnable();

    protected abstract void OnDisable();

    internal void Register(Tile tile)
    {
        Tile = tile;
    }

    /// <summary>
    /// 处理单位进入时的需要处理buff的图格状态
    /// </summary>
    internal protected abstract void StatusProcessOnEnter(IEnumerable<Unit> units);

    /// <summary>
    /// 处理单位退出时的需要处理uff的图格状态
    /// </summary>
    internal protected abstract void StatusProcessOnExit(IEnumerable<Unit> units);

    /// <summary>
    /// 处理单位处于图块上时的需要处理buff的图格状态
    /// </summary>
    internal protected abstract void StatusProcessOnUpdata(IEnumerable<Unit> units);
}

public abstract class RoundStatus : TileStatus
{
    protected internal override void StatusProcessOnUpdata(IEnumerable<Unit> units)
    {
        if(Times > 0)
        {
            Times -= 1;
        }
        else if(Times == 0)
        {
            Tile.RemoveStatus(this);
        }
    }
}

public class FireStatus : RoundStatus
{
    protected override void OnDisable()
    {
        
    }

    protected override void OnEnable()
    {
        
    }

    protected internal override void StatusProcessOnEnter(IEnumerable<Unit> units)
    {

    }

    protected internal override void StatusProcessOnExit(IEnumerable<Unit> units)
    {
        
    }

    protected internal override void StatusProcessOnUpdata(IEnumerable<Unit> units)
    {
        base.StatusProcessOnUpdata(units);
        foreach (var unit in units.ToList())
        {
            unit.AddBuff(new Burn() { Time = 2 });
        }
    }
}

public class PoisonStatus : RoundStatus
{
    protected override void OnDisable()
    {

    }

    protected override void OnEnable()
    {

    }

    protected internal override void StatusProcessOnEnter(IEnumerable<Unit> units)
    {

    }

    protected internal override void StatusProcessOnExit(IEnumerable<Unit> units)
    {

    }

    protected internal override void StatusProcessOnUpdata(IEnumerable<Unit> units)
    {
        base.StatusProcessOnUpdata(units);
        foreach (var unit in units.ToList())
        {
            unit.AddBuff(new Poison() { Time = 2, Damage = 10 });
        }
    }
}

public class CaltropStatus : RoundStatus
{
    public float Damage;
    protected override void OnDisable()
    {

    }

    protected override void OnEnable()
    {

    }

    protected internal override void StatusProcessOnEnter(IEnumerable<Unit> units)
    {
        foreach(IHurtable unit in units.ToList())
        {
            unit.Hurt(Damage, HurtType.FromTile | HurtType.AD, this);
        }
    }

    protected internal override void StatusProcessOnExit(IEnumerable<Unit> units)
    {

    }
}

public class HealMatrixStatus : RoundStatus
{
    public float Heal;
    protected internal override void StatusProcessOnUpdata(IEnumerable<Unit> units)
    {
        base.StatusProcessOnUpdata(units);
        foreach(ICurable unit in units)
        {
            unit.Cure(Heal, this);
        }
    }
    protected override void OnDisable()
    {

    }

    protected override void OnEnable()
    {

    }

    protected internal override void StatusProcessOnEnter(IEnumerable<Unit> units)
    {

    }

    protected internal override void StatusProcessOnExit(IEnumerable<Unit> units)
    {

    }
}

public class AttackMatrixStatus : RoundStatus
{
    public Unit User;
    protected override void OnDisable()
    {
        
    }

    protected override void OnEnable()
    {
        
    }

    protected internal override void StatusProcessOnEnter(IEnumerable<Unit> units)
    {
        
    }

    protected internal override void StatusProcessOnExit(IEnumerable<Unit> units)
    {
        
    }

    protected internal override void StatusProcessOnUpdata(IEnumerable<Unit> units)
    {
        base.StatusProcessOnUpdata(units);
        if(Times == 0)
        {
            var list = units.Where(p=>p.Camp == Camp.Enemy).ToList();
            foreach(IHurtable unit in list)
            {
                unit.Hurt(User.UnitData.Attack * 2, HurtType.AP | HurtType.FromTile, this);
            }
        }
    }
}

public class SilkscreenStatus : TileStatus
{
    protected override void OnDisable()
    {
        
    }

    protected override void OnEnable()
    {
        
    }

    protected internal override void StatusProcessOnEnter(IEnumerable<Unit> units)
    {
        
    }

    protected internal override void StatusProcessOnExit(IEnumerable<Unit> units)
    {
        
    }

    protected internal override void StatusProcessOnUpdata(IEnumerable<Unit> units)
    {
        
    }
}