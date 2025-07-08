using UnityEngine;
using MyPlayer;
using System.Collections.Generic;

public class Melee : Equipment, IUsable
{
    [SerializeField] Animator anim;
    [SerializeField] float damage, slashRate;
    [SerializeField] AudioSource slashSound, hitSound;

    float counter = 0.0f;
    public override void OnEquip(Entity wielder)
    {
        base.OnEquip(wielder);
        counter = 0.0f;
    }
    public override void OnEquipUpdate(Entity wielder)
    {
        base.OnEquipUpdate(wielder);
        counter += Time.deltaTime;
    }
    public void Use(Entity wielder, bool down)
    {
        if (!down || counter < slashRate) return;
        counter = 0.0f;
        Slash();
    }
    void Slash()
    {
        hitList.Clear();
        slashSound.Play();
        anim.SetTrigger("Slash");
    }
    List<GameObject> hitList = new();
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject hit = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
        if (hitList.Contains(hit)) return;
        hitList.Add(hit);

        if (hit.TryGetComponent(out IDamagable damagable) && damagable.GetDamage(damage)) hitSound.Play();
    }
}