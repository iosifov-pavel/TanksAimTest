using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    [SerializeField]
    private int _priority;
    [SerializeField]
    private Transform _aimTarget;

    public int Priority => _priority;
    public Transform Target => _aimTarget;
}
