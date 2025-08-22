using MyPlayer;
using UnityEngine;

public class BombBarrel : MonoBehaviour, IDamagable
{
    [SerializeField] Animator anim;
    [SerializeField] float damage, explosionRadius;
    [SerializeField] LayerMask hitMask;
    [SerializeField] AudioSource explodeSound;
    bool exploded = false;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
    public bool GetDamage(float damage)
    {
        if (exploded) return false;
        exploded = true;
        Explode();
        return true;
    }
    void Explode()
    {
        anim.SetTrigger("Explode");
        explodeSound.Play();
        foreach (var i in Physics2D.OverlapCircleAll(transform.position, explosionRadius, hitMask))
        {
            GameObject hit = i.attachedRigidbody != null ? i.attachedRigidbody.gameObject : i.gameObject;
            if (hit.TryGetComponent(out IDamagable damagable)) damagable.GetDamage(this.damage);
        }
    }
}