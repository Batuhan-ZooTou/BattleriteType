using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName ="PlayerScriptableObject",menuName ="ScriptableObject/PlayerSO")]
public class PlayerSO : ScriptableObject 
{
    public float maxHp;
    public float currentMaxHp;
    public float currentHp;
    public float currentActionProgress;
    [System.NonSerialized]
    public UnityEvent<float> takeDamageEvent;
    public UnityEvent<float> updateCurrentMaxHpEvent;
    public UnityEvent<float> updateActionProgress;
    public UnityEvent<float> updateActionProgressMax;


    private void OnEnable()
    {
        currentMaxHp = maxHp;
        currentHp = currentMaxHp;
        currentActionProgress = 0;
        if (takeDamageEvent==null)
        {
            takeDamageEvent = new UnityEvent<float>();
        }
        if (updateCurrentMaxHpEvent == null)
        {
            updateCurrentMaxHpEvent = new UnityEvent<float>();
        }
        if (updateActionProgress == null)
        {
            updateActionProgress = new UnityEvent<float>();
        }
        if (updateActionProgressMax == null)
        {
            updateActionProgressMax = new UnityEvent<float>();
        }
    }
    public void UpdateCurrentHp(float value)
    {
        currentHp += value;
        currentHp=Mathf.Clamp(currentHp, 0, currentMaxHp);
        takeDamageEvent.Invoke(currentHp);
    }
    public void UpdateCurrentMaxHp(float value)
    {
        currentMaxHp = value;
        currentMaxHp=Mathf.Clamp(currentMaxHp, 40, maxHp);
        updateCurrentMaxHpEvent.Invoke(currentMaxHp);
    }
    public void UpdateActionProgress(float value)
    {
        UpdateActionProgressMax(value);
        currentActionProgress += Time.deltaTime;
        updateActionProgress.Invoke(currentActionProgress);
        if (currentActionProgress>= value)
        {
            currentActionProgress = 0;
        }
    }
    public void UpdateActionProgressMax(float value)
    {
        updateActionProgressMax.Invoke(value);
    }


}
