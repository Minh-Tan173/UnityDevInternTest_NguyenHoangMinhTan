using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PlayerSO : ScriptableObject
{
    [Header("Layer")]
    public LayerMask ballLayer;
    public LayerMask boundaryLayer;

    [Header("Player's Data")]
    public float walkSpeed;
    public float runSpeed;
    public float rotateSpeed;
}
