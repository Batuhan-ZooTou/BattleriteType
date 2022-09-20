using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouble : MonoBehaviour
{
    public GameObject owner;
    public enum BoubleType
    {
        Pearl,
        Oldur,
        Ashka
    }
    public BoubleType boubleType;
    public float speed;
    
}
