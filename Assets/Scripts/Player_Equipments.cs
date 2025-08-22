using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MyPlayer
{
    public class Player_Equipments : MonoBehaviour
    {
        Player origin;
        public List<Equipment> equipments;
        public int equippedIndex = 0;
        public Equipment equipped;
        private void Awake()
        {
            origin = GetComponent<Player>();
            foreach (var i in equipments) i.Init();
            SwitchEquipment(0);
        }
        private void Update()
        {
            for(int i = 0; i < equipments.Count; i++)
            {
                if(Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    SwitchEquipment(i); break;
                }
            }
            if (equipped is IUsable && Input.GetMouseButton(0)) (equipped as IUsable).Use(origin, Input.GetMouseButtonDown(0));
            if (equipped is IReloadable && Input.GetKeyDown(KeyCode.R)) (equipped as IReloadable).Reload(origin);
            equipped.OnEquipUpdate(origin);
        }
        public void SwitchEquipment(int index)
        {
            if (index < 0 || index >= equipments.Count || equipped == equipments[index]) return;
            if (equipped != null) equipped.OnUnequip(origin);
            equipped = equipments[index];
            equipped.OnEquip(origin);
            equippedIndex = index;
        }
        private void OnDisable()
        {
            equipped.OnUnequip(origin);
        }
        private void OnEnable()
        {
            equipped.OnEquip(origin);
        }
    }
}
