using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    float _shakeForce = 0f;

    [SerializeField]
    float _shakeDuration = 0f;

    Coroutine _coShaking;

    CinemachineVirtualCamera _virtualCamera;
    CinemachineBasicMultiChannelPerlin _basicPerlin;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _basicPerlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void OnEnable()
    {
        PieceController.OnPiecePlaced += SmallShake;
        ScoreController.OnScore += ShakeCamera;
    }

    private void OnDisable()
    {
        _basicPerlin.m_AmplitudeGain = 0f;

        PieceController.OnPiecePlaced -= SmallShake;
        ScoreController.OnScore -= ShakeCamera;
    }

    void StartShake() => _basicPerlin.m_AmplitudeGain = _shakeForce / 2f;

    void EndShake() => _basicPerlin.m_AmplitudeGain = 0f;

    void ShakeCamera()
    {
        if (_coShaking != null)
            StopCoroutine(_coShaking);

        StartCoroutine(CoShake());
    }

    void SmallShake(Transform pNull)
    {
        if (_coShaking != null)
            StopCoroutine(_coShaking);

        StartCoroutine(CoShake(0.2f, 0.2f));
    }

    IEnumerator CoShake(float pForceMultiplier = 1f, float pDurationMultiplier = 1f)
    {
        if (_basicPerlin == null)
        {
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
            _basicPerlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        _basicPerlin.m_AmplitudeGain = _shakeForce * pForceMultiplier;

        float t_time = 0f;

        while (t_time < _shakeDuration * pDurationMultiplier)
        {
            _basicPerlin.m_AmplitudeGain = Mathf.Lerp(_shakeForce * pForceMultiplier, 0f, t_time / _shakeDuration * pDurationMultiplier);
            t_time += (Time.deltaTime / Time.timeScale);
            yield return null;
        }

        _basicPerlin.m_AmplitudeGain = 0f;           
    }
}
