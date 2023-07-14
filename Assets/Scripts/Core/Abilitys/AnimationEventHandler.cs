using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public void TriggerEndAnim(Ability ability)
    {
        ability.TriggerEndAnim();
    }
}
