using UnityEngine;
using MyPlayer;

public abstract class Equipment : MonoBehaviour
{
    public virtual void OnEquip(Player wielder)
    {
        gameObject.SetActive(true);
    }
    public virtual void OnEquipUpdate(Player wielder) { }
    public virtual void OnUnequip(Player wielder)
    {
        gameObject.SetActive(false);
    }
}
