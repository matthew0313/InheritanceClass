using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : AnimatedBarUI
{
    [SerializeField] Enemy target;
    protected override void Update()
    {
        base.Update();
        targetScale = target.hp / target.maxHp;
    }
}