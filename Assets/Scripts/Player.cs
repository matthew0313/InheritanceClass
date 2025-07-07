using UnityEngine;

namespace MyPlayer
{
    [RequireComponent(typeof(Player_Movements), typeof(Player_Equipments))]
    public class Player : MonoBehaviour
    {
        public Player_Movements movements { get; private set; }
        public Player_Equipments equipments { get; private set; }
        private void Awake()
        {
            movements = GetComponent<Player_Movements>();
            equipments = GetComponent<Player_Equipments>();
        }
    }
}
