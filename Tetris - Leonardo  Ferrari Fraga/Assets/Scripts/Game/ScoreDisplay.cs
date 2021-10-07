using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    SOVarInt _soVarScore;

    TextMeshPro _text;

    private void Awake()
    {
        _soVarScore = Resources.Load<SOVarInt>("Score");

        _text = GetComponentInChildren<TextMeshPro>();
    }

    private void Update() => _text.text = $"{_soVarScore.Value}";
}
