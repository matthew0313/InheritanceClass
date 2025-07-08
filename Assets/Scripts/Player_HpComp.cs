using System.Collections;
using UnityEngine;

namespace MyPlayer
{
    public class Player_HpComp : MonoBehaviour, IDamagable
    {
        Player origin;

        [SerializeField] float respawnTime = 3.0f;
        [SerializeField] Transform respawnPoint;
        public float maxHp;
        public float hp { get; private set; }
        public bool dead { get; private set; } = false;
        private void Awake()
        {
            origin = GetComponent<Player>();
            hp = maxHp;
        }
        public bool GetDamage(float damage)
        {
            if (dead) return false;
            hp = Mathf.Max(hp - damage, 0);
            if(hp <= 0)
            {
                dead = true;
                origin.movements.enabled = false;
                origin.equipments.enabled = false;
                origin.anim.SetBool("Dead", true);
                StartCoroutine(Respawn());
            }
            return true;
        }
        IEnumerator Respawn()
        {
            yield return new WaitForSeconds(respawnTime);
            transform.position = respawnPoint.transform.position;
            hp = maxHp;
            dead = false;
            origin.movements.enabled = true;
            origin.equipments.enabled = true;
            origin.anim.SetBool("Dead", false);
        }
    }
}
