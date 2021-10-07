using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    CinemachineVirtualCamera _vcamGameplay = null;
    [SerializeField]
    CinemachineVirtualCamera _vcamMenu = null;

    CinemachineBrain _cinemachineBrain;

    private void Awake() => _cinemachineBrain = GetComponentInChildren<CinemachineBrain>();

    private void OnEnable()
    {
        MainMenu.OnPlay += EnableGameplayCamera;
        MainMenu.OnBackToMenu += EnableMenuCamera;
    }

    private void OnDisable()
    {
        MainMenu.OnPlay -= EnableGameplayCamera;
        MainMenu.OnBackToMenu -= EnableMenuCamera;
    }

    void EnableGameplayCamera() => _vcamGameplay.gameObject.SetActive(true);

    void EnableMenuCamera()
    {
        CinemachineBlendDefinition t_blendDefinition = new CinemachineBlendDefinition();
        t_blendDefinition.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
        t_blendDefinition.m_Time = 2f;
        _cinemachineBrain.m_DefaultBlend = t_blendDefinition;

        _vcamMenu.gameObject.SetActive(true);
        _vcamGameplay.gameObject.SetActive(false);
    }
}
