using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
public class Orbs : MonoBehaviour
{
    public bool healOrb;
    public float value;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ThirdPersonController>(out var player))
        {
            if (healOrb)
            {
                player.playerSO.UpdateCurrentMaxHp(player.playerSO.currentMaxHp + value);
                player.playerSO.UpdateCurrentHp(value);
            }
            else
            {
                player.playerSO.UpdateCurrentEnergy(value);
            }
            gameObject.SetActive(false);

        }
    }
}
