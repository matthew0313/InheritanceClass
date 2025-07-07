using UnityEngine;
using MyPlayer;
using System.Collections.Generic;
using AudioManagement;

public class Melee : Equipment, IUsable
{
    [SerializeField] Animator anim;
    [SerializeField] float damage, slashRate;
    [SerializeField] Sound slashSound;

    float counter = 0.0f;
    public override void OnEquip(Player wielder)
    {
        base.OnEquip(wielder);
        counter = 0.0f;
    }
    public override void OnEquipUpdate(Player wielder)
    {
        base.OnEquipUpdate(wielder);
        counter += Time.deltaTime;
    }
    public void Use(Player wielder, bool down)
    {
        if (!down || counter < slashRate) return;
        counter = 0.0f;
        Slash();
    }
    void Slash()
    {
        hitList.Clear();
        AudioManager.Instance.PlaySound(slashSound);
        anim.SetTrigger("Slash");
    }
    List<GameObject> hitList = new();
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject hit = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
        if (hitList.Contains(hit)) return;
        hitList.Add(hit);

        if (hit.TryGetComponent(out IDamagable damagable)) damagable.GetDamage(damage);
    }
}