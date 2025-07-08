using UnityEngine;
using MyPlayer;

public class Bullet : MonoBehaviour
{
    float damage, range, speed;
    float counter = 0.0f;
    public void Set(float damage, float range, float speed)
    {
        this.damage = damage;
        this.range = range;
        this.speed = speed;
    }
    private void Update()
    {
        transform.Translate(transform.right * (transform.localScale.x > 0 ? 1.0f : -1.0f) * speed * Time.deltaTime);
        counter += Time.deltaTime;
        if (counter * speed > range) Destroy(gameObject);
    }
}