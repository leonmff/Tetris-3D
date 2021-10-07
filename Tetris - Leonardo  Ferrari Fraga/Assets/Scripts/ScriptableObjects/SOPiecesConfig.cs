using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PiecesConfig", menuName = "Grid/Pieces Config")]
public class SOPiecesConfig : ScriptableObject
{
    public float InitialSpeed;
    [Range(1f, 2f)]
    public float SpeedIncreaseMultiplier;
    public float AccelerationMultiplier;
    public float AdjustDelay;

    [SerializeField, InspectorReadOnly]
    float _speed;
    public float Speed { get => _speed; }

    public void SetInitialSpeed() => _speed = InitialSpeed;

    public void IncreaseSpeed() => _speed *= SpeedIncreaseMultiplier;
}
