using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 可被攻击接口
/// </summary>
/// <remarks>继承该接口的类可以被攻击</remarks>
public interface IHurtable
{
    /// <summary>
    /// 伤害值计算
    /// </summary>
    /// <remarks>计算伤害值，但不实际扣除</remarks>
    /// <param name="damage">原始伤害值</param>
    /// <param name="type">伤害类型</param>
    /// <param name="source">伤害源</param>
    /// <returns>受到的伤害数值</returns>
    float HurtCalculate(float damage, HurtType type, object source);
    /// <summary>
    /// 伤害目标
    /// </summary>
    /// <remarks>会实际扣除生命值</remarks>
    /// <param name="damage">原始伤害值</param>
    /// <param name="type">伤害类型</param>
    /// <param name="source">伤害源</param>
    /// <returns>受到的伤害数值</returns>
    float Hurt(float damage, HurtType type, object source)
    {
        var res = HurtCalculate(damage, type, source);
        OnHurt(res);
        return res;
    }
    /// <summary>
    /// 当受到伤害
    /// </summary>
    /// <remarks>请实现受到伤害后实际扣除血量的过程</remarks>
    /// <param name="damage">运算后获得的伤害值</param>
    protected void OnHurt(float damage);
}