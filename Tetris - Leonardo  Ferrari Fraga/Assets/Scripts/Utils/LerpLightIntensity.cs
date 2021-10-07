using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpLightIntensity : MonoBehaviour
{
    [SerializeField]
    Vector2 _minMaxIntensity = Vector2.zero;
    [SerializeField]
    float _duration = 0f;

    Light _light;

    private void Awake() => _light = GetComponent<Light>();

    private void Start() => StartCoroutine(LerpIntensity());

    IEnumerator LerpIntensity()
    {
        float t_elapsedTime = 0f;

        while (t_elapsedTime < _duration)
        {
            _light.intensity = Mathf.Lerp(_minMaxIntensity.x, _minMaxIntensity.y, t_elapsedTime / _duration);
            t_elapsedTime += Time.deltaTime;
            yield return null;
        }

        _light.intensity = _minMaxIntensity.y;
    }
}
