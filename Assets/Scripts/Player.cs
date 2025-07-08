using UnityEngine;

namespace MyPlayer
{
    [RequireComponent(typeof(Player_Movements))]
    public class Player : Entity
    {
        public Animator anim;
        public Player_Movements movements { get; private set; }
        private void Awake()
        {
            movements = GetComponent<Player_Movements>();
        }
    }
}
