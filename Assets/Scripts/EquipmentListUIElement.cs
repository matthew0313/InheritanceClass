using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentListUIElement : MonoBehaviour
{
    [SerializeField] Image equipmentIcon;
    [SerializeField] TMP_Text equipmentIndexText;
    [SerializeField] TMP_Text equipmentText;

    Equipment equipment;
    public void Set(Equipment equipment, int index)
    {
        this.equipment = equipment;
        equipmentIcon.preserveAspect = true;
        equipmentIcon.sprite = equipment.equipmentIcon;
        equipmentIndexText.text = (index+1).ToString();
        if(equipment is ITextDisplayed)
        {
            equipmentText.gameObject.SetActive(true);
            UpdateEquipmentText();
        }
        else equipmentText.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (equipment is ITextDisplayed) UpdateEquipmentText();
    }
    void UpdateEquipmentText()
    {
        equipmentText.text = (equipment as ITextDisplayed).textDisplayed;
    }
}
