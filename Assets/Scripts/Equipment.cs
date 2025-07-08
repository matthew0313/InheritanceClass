using UnityEngine;
using MyPlayer;

public abstract class Equipment : MonoBehaviour
{
    public Sprite equipmentIcon;
    public virtual void Init() { }
    public virtual void OnEquip(Entity wielder)
    {
        gameObject.SetActive(true);
    }
    public virtual void OnEquipUpdate(Entity wielder) { }
    public virtual void OnUnequip(Entity wielder)
    {
        gameObject.SetActive(false);
    }
}
