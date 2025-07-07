using UnityEngine;
using MyPlayer;
using System.Collections;
using AudioManagement;

public class Gun : Equipment, IUsable, IReloadable, ITextDisplayed
{
    [SerializeField] Animator anim;
    [SerializeField] bool auto;
    [SerializeField] float damage, fireRate, range, bulletSpeed, spread;
    [SerializeField] int shotCount = 1, magSize;
    [SerializeField] float reloadTime;
    [SerializeField] Transform firePoint;
    [SerializeField] Bullet bullet;
    [SerializeField] Sound fireSound, reloadSound;

    bool isReloading = false;
    int mag = -1;
    float counter = 0.0f;
    Coroutine reloading;

    public string textDisplayed => $"{mag}/{magSize}";
    public override void OnEquip(Player wielder)
    {
        base.OnEquip(wielder);
        counter = 0.0f;
        if (mag == -1) mag = magSize;
    }
    public override void OnEquipUpdate(Player wielder)
    {
        base.OnEquipUpdate(wielder);
        counter += Time.deltaTime;
    }

    public void Use(Player wielder, bool down)
    {
        if (!auto && !down || isReloading || counter < fireRate || mag <= 0) return;
        counter = 0.0f;
        mag--;
        Fire();
    }
    PlayingAudio reloadingSound = null;
    public void Reload(Player wielder)
    {
        if (isReloading || mag == magSize) return;
        reloadingSound = AudioManager.Instance.PlaySound(reloadSound);
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
        AudioManager.Instance.PlaySound(fireSound);
        anim.SetTrigger("Fire");
        for(int i = 0; i < shotCount; i++)
        {
            var bul = Instantiate(bullet, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, UnityEngine.Random.Range(-spread, spread)));
            bul.transform.localScale = firePoint.lossyScale;
            bul.Set(damage, range, bulletSpeed);
        }
    }
    public override void OnUnequip(Player wielder)
    {
        base.OnUnequip(wielder);
        if (isReloading)
        {
            reloadingSound.Stop();
            StopCoroutine(reloading);
            isReloading = false;
        }
    }
}