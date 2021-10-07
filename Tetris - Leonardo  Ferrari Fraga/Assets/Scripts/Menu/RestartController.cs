using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class RestartController : MonoBehaviour
{
    [SerializeField]
    GameObject _vcamStart = null;
    [SerializeField]
    GameObject _vcamGameplay = null;

    CinemachineBrain _cinemachineBrain;

    public static UnityAction OnRestart;

    private void Awake() => _cinemachineBrain = GetComponentInChildren<CinemachineBrain>();

    private void Start()
    {
        if (PlayerPrefs.GetInt("Restart") == 1)
        {
            _vcamStart.SetActive(false);
            _vcamGameplay.SetActive(true);

            CinemachineBlendDefinition t_newBlend = new CinemachineBlendDefinition();
            t_newBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
            _cinemachineBrain.m_DefaultBlend = t_newBlend;

            Time.timeScale = 1f;

            OnRestart?.Invoke();

            StartCoroutine(ResetRestartPrefs());
        }
    }

    IEnumerator ResetRestartPrefs()
    {
        yield return new WaitForSeconds(0.1f);
        PlayerPrefs.SetInt("Restart", 0);
    }
}
