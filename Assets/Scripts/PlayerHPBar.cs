using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyPlayer;
public class PlayerHPBar : AnimatedBarUI
{
    Player target;
    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    protected override void Update()
    {
        base.Update();
        targetScale = target.hpComp.hp / target.hpComp.maxHp;
    }
}