using UnityEngine;

namespace MyPlayer
{
    [RequireComponent(typeof(Player_Movements), typeof(Player_HpComp))]
    public class Player : Entity
    {
        public Animator anim;
        public Player_Movements movements;
        public Player_HpComp hpComp;
        private void Awake()
        {
            movements = GetComponent<Player_Movements>();
        }
    }
}
