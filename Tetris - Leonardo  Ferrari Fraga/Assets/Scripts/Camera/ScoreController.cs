using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScoreController : MonoBehaviour
{
    [SerializeField]
    float _scoreMultiplierPerLine = 0f;

    [SerializeField, Space(15)]
    AudioClip _sfxExplosion = null;
    [SerializeField]
    AudioClip _sfxExplosionCombo = null;

    SOVarInt _soVarScore;
    SOGridConfig _soGridConfig;

    public static UnityAction OnScore;

    private void Awake()
    {
        _soVarScore = Resources.Load<SOVarInt>("Score");
        _soGridConfig = Resources.Load<SOGridConfig>("GridConfig");
    }

    private void OnEnable() => MainMenu.OnBackToMenu += ResetScore;

    private void OnDisable() => MainMenu.OnBackToMenu -= ResetScore;

    private void Start() => _soVarScore.Value = 0;

    public void Score(int pNumberOfLines)
    {
        int t_score = Mathf.RoundToInt(pNumberOfLines * _soGridConfig.GridSize.x);
        t_score *= (int)Mathf.Round(1f + (_scoreMultiplierPerLine * (pNumberOfLines - 1)));

        _soVarScore.Value = _soVarScore.Value + t_score;

        if (t_score > 0)
        {
            SoundManager.instance.PlaySFX(_sfxExplosion, true);

            if (pNumberOfLines == 4)
                SoundManager.instance.PlaySFX(_sfxExplosionCombo, true);

            OnScore?.Invoke();
        }
    }

    void ResetScore() => StartCoroutine(CoResetScore());

    IEnumerator CoResetScore()
    {
        yield return new WaitForSeconds(2f);
        _soVarScore.Value = 0;
    }
}
