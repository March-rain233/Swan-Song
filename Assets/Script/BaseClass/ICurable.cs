using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 可治愈接口
/// </summary>
/// <remarks>继承该接口的类可以被治愈</remarks>
public interface ICurable
{
    /// <summary>
    /// 治愈目标
    /// </summary>
    /// <remarks>会实际恢复血量</remarks>
    /// <param name="power">原始治愈量</param>
    /// <param name="source">治愈源</param>
    /// <returns>恢复的血量数值</returns>
    float Cure(float power, object source)
    {
        var res = CureCalculate(power, source);
        OnCure(res);
        return res;
    }
    /// <summary>
    /// 计算恢复的血量
    /// </summary>
    /// <remarks>不会实际恢复血量</remarks>
    /// <param name="power">原始治愈量</param>
    /// <param name="source">治愈源</param>
    /// <returns>恢复的血量数值</returns>
    float CureCalculate(float power, object source);
    /// <summary>
    /// 当恢复
    /// </summary>
    /// <remarks>请实现恢复血量的过程</remarks>
    /// <param name="power">运算后需要恢复的血量</param>
    protected void OnCure(float power);
}