using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider currentHp;
    [SerializeField] private Slider currentMaxHp;
    [SerializeField] private Slider dummyCurrentHp;
    [SerializeField] private Slider dummyCurrentMaxHp;
    [SerializeField] private Slider dummyCurrentShield;
    [SerializeField] private Slider dummyCurrentEffect;
    [SerializeField] private Slider currentEnergy;
    [SerializeField] private TextMeshProUGUI currentEffectName;
    [SerializeField] private Slider currentEffect;
    [SerializeField] PlayerSO playerSO;
    [SerializeField] DummyTarget dummyTarget;
    [SerializeField] private Slider currentActionProgress;
    [SerializeField] private TextMeshProUGUI dummyHp;
    [SerializeField] private TextMeshProUGUI dummyCurrentEffectName;


    void Start()
    {
        ChangeCurrentHpValue(playerSO.currentHp);
        ChangeCurrentMaxHpValue(playerSO.currentMaxHp);
        ChangeActionProgressValue(0);
        ChangeActionProgressMaxValue(1);
        ChangeCurrentDummyHpValue(dummyTarget.currentHp);
        ChangeCurrentDummyMaxHpValue(dummyTarget.currentMaxHp);
        ChangeDummyEffectValue(0);
        ChangeDummyEffectMaxValue(0);
        ChangeCurrentEnergyValue(0);
        ChangeDummyEffectName(dummyTarget.currentEffectName);
        ChangeDummyCurrentShieldValue(0);
    }

    private void OnEnable()
    {
        playerSO.takeDamageEvent.AddListener(ChangeCurrentHpValue);
        playerSO.updateCurrentMaxHpEvent.AddListener(ChangeCurrentMaxHpValue);
        playerSO.updateActionProgress.AddListener(ChangeActionProgressValue);
        playerSO.updateActionProgressMax.AddListener(ChangeActionProgressMaxValue);
        playerSO.dealDamageEvent.AddListener(ChangeCurrentEnergyValue);

        dummyTarget.takeDamageEvent.AddListener(ChangeCurrentDummyHpValue);
        dummyTarget.updateCurrentMaxHpEvent.AddListener(ChangeCurrentDummyMaxHpValue);
        dummyTarget.updateEffectEvent.AddListener(ChangeDummyEffectValue);
        dummyTarget.updateEffectMaxEvent.AddListener(ChangeDummyEffectMaxValue);
        dummyTarget.updateEffectNameEvent.AddListener(ChangeDummyEffectName);
        dummyTarget.updateCurrentShieldEvent.AddListener(ChangeDummyCurrentShieldValue);
    }
    private void OnDisable()
    {
        playerSO.takeDamageEvent.RemoveListener(ChangeCurrentHpValue);
        playerSO.updateCurrentMaxHpEvent.RemoveListener(ChangeCurrentMaxHpValue);
        playerSO.updateActionProgress.RemoveListener(ChangeActionProgressValue);
        playerSO.updateActionProgressMax.RemoveListener(ChangeActionProgressMaxValue);
        playerSO.dealDamageEvent.RemoveListener(ChangeCurrentEnergyValue);

        dummyTarget.takeDamageEvent.RemoveListener(ChangeCurrentDummyHpValue);
        dummyTarget.updateCurrentMaxHpEvent.RemoveListener(ChangeCurrentDummyMaxHpValue);
        dummyTarget.updateEffectEvent.RemoveListener(ChangeDummyEffectValue);
        dummyTarget.updateEffectMaxEvent.RemoveListener(ChangeDummyEffectMaxValue);
        dummyTarget.updateEffectNameEvent.RemoveListener(ChangeDummyEffectName);
        dummyTarget.updateCurrentShieldEvent.RemoveListener(ChangeDummyCurrentShieldValue);
    }
    public void ChangeCurrentHpValue(float value)
    {
        currentHp.value = value;
    }
    public void ChangeCurrentMaxHpValue(float value)
    {
        currentMaxHp.value = value;
    }
    public void ChangeCurrentDummyHpValue(float value)
    {
        dummyCurrentHp.value = value;
        value = Mathf.FloorToInt(value);
        dummyHp.text =value.ToString();
    }
    public void ChangeCurrentDummyMaxHpValue(float value)
    {
        dummyCurrentMaxHp.value = value;
    }
    public void ChangeDummyCurrentShieldValue(float value)
    {
        dummyCurrentShield.value = value;
    }
    public void ChangeActionProgressValue(float value)
    {
        currentActionProgress.value = value;
    }
    public void ChangeActionProgressMaxValue(float value)
    {
        currentActionProgress.maxValue = value;
    }
    public void ChangeDummyEffectValue(float value)
    {
        dummyCurrentEffect.value = value;
    }
    public void ChangeDummyEffectMaxValue(float value)
    {
        dummyCurrentEffect.maxValue = value;
    }
    public void ChangeDummyEffectName(string value)
    {
        dummyCurrentEffectName.text = value;
    }
    public void ChangeCurrentEnergyValue(float value)
    {
        currentEnergy.value = value;
    }
}
