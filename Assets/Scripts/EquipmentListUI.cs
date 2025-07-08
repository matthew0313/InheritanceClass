using MyPlayer;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EquipmentListUI : MonoBehaviour
{
    Player player;

    [SerializeField] EquipmentListUIElement elementPrefab;
    [SerializeField] Transform elementAnchor;
    [SerializeField] TMP_Text equipmentName, equipmentText;
    [SerializeField] RectTransform equippedIndicator;
    List<EquipmentListUIElement> elements = new();
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    private void Start()
    {
        for (int i = 0; i < player.equipments.equipments.Count; i++)
        {
            var tmp = Instantiate(elementPrefab, elementAnchor);
            tmp.gameObject.SetActive(true);
            tmp.Set(player.equipments.equipments[i], i);
            elements.Add(tmp);
        }
        OnEquipmentChange(player.equipments.equipped);
    }
    private void Update()
    {
        if (equipment is ITextDisplayed) UpdateEquipmentText();
        if (equipment != player.equipments.equipped) OnEquipmentChange(player.equipments.equipped);
        equippedIndicator.transform.position = elements[player.equipments.equippedIndex].transform.position;
    }
    Equipment equipment;
    void OnEquipmentChange(Equipment equipment)
    {
        this.equipment = equipment;
        equipmentName.text = equipment.name;
        if (equipment is ITextDisplayed)
        {
            equipmentText.gameObject.SetActive(true);
            UpdateEquipmentText();
        }
        else equipmentText.gameObject.SetActive(false);
    }
    void UpdateEquipmentText()
    {
        equipmentText.text = (equipment as ITextDisplayed).textDisplayed;
    }
}
