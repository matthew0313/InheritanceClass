using UnityEngine;

namespace MyPlayer
{
    [RequireComponent(typeof(Player_Movements), typeof(Player_Equipments), typeof(Player_HpComp))]
    public class Player : Entity
    {
        public Animator anim;
        public Player_Movements movements;
        public Player_Equipments equipments;
        public Player_HpComp hpComp;
        private void Awake()
        {
            movements = GetComponent<Player_Movements>();
            equipments = GetComponent<Player_Equipments>();
            hpComp = GetComponent<Player_HpComp>();
        }
    }
}
