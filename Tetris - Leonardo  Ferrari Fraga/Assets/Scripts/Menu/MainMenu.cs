using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    GameObject _objMainMenu = null;
    [SerializeField]
    GameObject _objPauseMenu = null;

    public static UnityAction OnPlay, OnBackToMenu;

    private void Start()
    {
        if (PlayerPrefs.GetInt("Restart") != 1)
            StartCoroutine(DelayActivation());
    }

    IEnumerator DelayActivation()
    {
        yield return new WaitForSeconds(2f);
        _objMainMenu.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_objMainMenu.activeInHierarchy)
                return;

            if (_objPauseMenu.activeInHierarchy)
                Resume();
            else
                Pause();
        }
    }

    // Called by button
    public void Play()
    {
        SoundManager.instance.PlayButtonSFX();
        OnPlay?.Invoke();
    }

    // Called by button
    public void Quit()
    {
        SoundManager.instance.PlayButtonSFX();
        Application.Quit(); 
    }

    // Called by button
    public void Pause()
    {
        _objPauseMenu.SetActive(true);
        Time.timeScale = 0f;

        SoundManager.instance.PlayButtonSFX();
    }

    // Called by button
    public void Menu()
    {
        OnBackToMenu?.Invoke();
        _objPauseMenu.SetActive(false);

        Time.timeScale = 1f;

        StartCoroutine(ActivateMenuDelayed());
    }

    IEnumerator ActivateMenuDelayed()
    {
        yield return new WaitForSeconds(2f);
        _objMainMenu.SetActive(true);
    }

    // Called by button
    public void Resume()
    {
        _objPauseMenu.SetActive(false);
        Time.timeScale = 1f;

        SoundManager.instance.PlayButtonSFX();
    }

    // Called by button
    public void Restart()
    {
        PlayerPrefs.SetInt("Restart", 1);
        SoundManager.instance.PlayButtonSFX();

        SceneManager.LoadScene(0);
    }
}
