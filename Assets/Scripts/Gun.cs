using UnityEngine;
using MyPlayer;
using System.Collections;
using System;

public class Gun : Equipment, IUsable, IReloadable, ITextDisplayed
{
    [SerializeField] Animator anim;
    [SerializeField] bool auto;
    [SerializeField] float damage, fireRate, range, bulletSpeed, spread;
    [SerializeField] int shotCount = 1, magSize;
    [SerializeField] float reloadTime;
    [SerializeField] Transform firePoint;
    [SerializeField] Bullet bullet;
    [SerializeField] AudioSource fireSound, reloadSound;

    public string textDisplayed => isReloading ? "Reloading..." : $"{mag}/{magSize}";

    bool isReloading = false;
    public int mag { get; private set; }
    float counter = 0.0f;
    Coroutine reloading;

    public override void Init()
    {
        base.Init();
        mag = magSize;
    }
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
        if (!auto && !down || isReloading || counter < fireRate || mag <= 0) return;
        counter = 0.0f;
        mag--;
        Fire();
    }
    public bool shouldReload => mag <= 0;
    public void Reload(Entity wielder)
    {
        if (isReloading || mag == magSize) return;
        reloadSound.Play();
        anim.SetTrigger("Reload");
        isReloading = true;
        reloading = StartCoroutine(Reloading());
    }
    IEnumerator Reloading()
    {
        yield return new WaitForSeconds(reloadTime);
        mag = magSize;
        isReloading = false;
    }
    void Fire()
    {
        fireSound.Play();
        anim.SetTrigger("Fire");
        for(int i = 0; i < shotCount; i++)
        {
            var bul = Instantiate(bullet, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, UnityEngine.Random.Range(-spread, spread)));
            bul.transform.localScale = firePoint.lossyScale;
            bul.Set(damage, range, bulletSpeed);
        }
    }
    public override void OnUnequip(Entity wielder)
    {
        base.OnUnequip(wielder);
        if (isReloading)
        {
            StopCoroutine(reloading);
            isReloading = false;
        }
    }
}