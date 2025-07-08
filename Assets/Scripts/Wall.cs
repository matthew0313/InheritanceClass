using UnityEngine;

public class Wall : MonoBehaviour, IDamagable
{
    [SerializeField] Animator anim;
    [SerializeField] int hitsNeeded = 3;

    bool broken = false;
    int counter = 0;
    public bool GetDamage(float damage)
    {
        if (broken) return false;
        if (++counter >= hitsNeeded)
        {
            broken = true;
            anim.SetTrigger("Break");
        }
        else anim.SetTrigger("Hit");
        return true;
    }
}