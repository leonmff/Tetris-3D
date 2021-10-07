using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextPieceDisplay : MonoBehaviour
{
    [SerializeField]
    List<GameObject> _listNextPiece = null;

    private void OnEnable()
    {
        SpawnController.OnSpawnPiece += ShowNextPiece;
        MainMenu.OnBackToMenu += DisableAllWithDelay;
    }

    private void OnDisable()
    {
        SpawnController.OnSpawnPiece -= ShowNextPiece;
        MainMenu.OnBackToMenu -= DisableAllWithDelay;
    }

    private void Start()
    {
        DisableAllPieces();
    }

    void ShowNextPiece(GameObject pPiece, SOColorConfig.GridColor pColor)
    {
        DisableAllPieces();
        _listNextPiece[(int)pColor].SetActive(true);
    }

    void DisableAllPieces()
    {
        for (int index = 0; index < _listNextPiece.Count; index++)
        {
            _listNextPiece[index].SetActive(false);
        }
    }

    void DisableAllWithDelay() => StartCoroutine(CoDisableAllPieces());

    IEnumerator CoDisableAllPieces()
    {
        yield return new WaitForSeconds(2f);
        DisableAllPieces();
    }
}
