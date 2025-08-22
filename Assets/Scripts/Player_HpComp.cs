using UnityEngine;
using System.Collections;

namespace MyPlayer
{
    public class Player_HpComp : MonoBehaviour
    {
        Player origin;

        [SerializeField] float respawnTime = 3.0f;
        [SerializeField] Transform respawnPoint;
        public float maxHp;
        public float hp;
        public bool dead = false;
        private void Awake()
        {
            origin = GetComponent<Player>();
            hp = maxHp;
        }
        IEnumerator Respawn()
        {
            yield return new WaitForSeconds(respawnTime);
            transform.position = respawnPoint.transform.position;
            hp = maxHp;
            dead = false;
            origin.movements.enabled = true;
            origin.anim.SetBool("Dead", false);
        }
    }
}
