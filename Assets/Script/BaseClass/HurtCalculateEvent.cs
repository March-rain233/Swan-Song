using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameToolKit;

public class HurtCalculateEvent : EventBase
{
    public float OriDamage;
    public float DamageAdd;
    public float Rate;
    public HurtType Type;
    public object Source;
    public IHurtable Target;
    public int FinalDamage => UnityEngine.Mathf.FloorToInt((OriDamage + DamageAdd) * Rate);
}
