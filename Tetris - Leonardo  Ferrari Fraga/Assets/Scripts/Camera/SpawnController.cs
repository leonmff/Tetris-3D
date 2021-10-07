using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnController : MonoBehaviour
{
    [SerializeField]
    float _delayBeforeStartFalling = 0f;

    [SerializeField, Space(15)]
    List<GameObject> _listPieces = null;

    [SerializeField, Space(15)]
    AudioClip _sfxSpawnPiece = null;

    SOGridConfig _soGridConfig;

    Vector3 _spawnLocation;

    SOColorConfig.GridColor _currentPieceColor;
    SOColorConfig.GridColor _nextPieceColor;

    public delegate void SpawnPiecesEvents(GameObject pPiece, SOColorConfig.GridColor pColor);
    public static event SpawnPiecesEvents OnSpawnPiece;

    private void Awake() => _soGridConfig = Resources.Load<SOGridConfig>("GridConfig");

    private void OnEnable()
    {
        MainMenu.OnPlay += StartSpawningDelayed;
        MainMenu.OnBackToMenu += DestroyAllPieces;
        RestartController.OnRestart += StartSpawning;
        PieceController.OnPiecePlaced += SpawnPiece;
    }

    private void OnDisable()
    {
        MainMenu.OnPlay -= StartSpawningDelayed;
        MainMenu.OnBackToMenu -= DestroyAllPieces;
        RestartController.OnRestart -= StartSpawning;
        PieceController.OnPiecePlaced -= SpawnPiece;
    }

    void DestroyAllPieces() => StartCoroutine(CoDestroyAllPieces());

    IEnumerator CoDestroyAllPieces()
    {
        yield return new WaitForSeconds(2f);

        Collider[] t_colliders = Physics.OverlapSphere(transform.position, 50f, LayerMask.GetMask("Block"));
        for (int index = 0; index < t_colliders.Length; index++)
        {
            Destroy(t_colliders[index].gameObject);
        }
    }

    private void Start()
    {
        SetSpawnLocation();

        _nextPieceColor = GetRandomEnum<SOColorConfig.GridColor>();

        Resources.Load<SOPiecesConfig>("PiecesConfig").SetInitialSpeed();
    }

    void SetSpawnLocation() => _spawnLocation = new Vector3((_soGridConfig.GridSize.x / 2f) * _soGridConfig.GridOffset, (_soGridConfig.GridSize.y - 1f) * _soGridConfig.GridOffset, 0f);

    public void StartSpawningDelayed() => StartCoroutine(DelayAction(2f, () => SpawnPiece()));

    public void StartSpawning() => SpawnPiece();

    IEnumerator DelayAction(float pDelay, Action pAction)
    {
        yield return new WaitForSeconds(pDelay);
        pAction?.Invoke();
    }

    void SpawnPiece(Transform pNull = null)
    {
        _currentPieceColor = _nextPieceColor;
        _nextPieceColor = GetRandomEnum<SOColorConfig.GridColor>();

        GameObject t_piece = Instantiate(_listPieces[(int)_currentPieceColor], null);
        t_piece.transform.position = _spawnLocation;

        for (int index = 0; index < t_piece.transform.childCount; index++)
        {
            t_piece.transform.GetChild(index).gameObject.layer = LayerMask.NameToLayer("Default");
        }

        StartCoroutine(DelayBeforeFall(t_piece));
    }

    IEnumerator DelayBeforeFall(GameObject pPiece)
    {
        pPiece.SetActive(true);

        float t_elapsedTime = 0f;
        while (t_elapsedTime < _delayBeforeStartFalling)
        {
            pPiece.transform.position = _spawnLocation;
            t_elapsedTime += Time.deltaTime;
            yield return null;
        }

        SoundManager.instance.PlaySFX(_sfxSpawnPiece, true);
        OnSpawnPiece?.Invoke(pPiece, _nextPieceColor);
    }

    T GetRandomEnum<T>()
    {
        Array t_enumArray = Enum.GetValues(typeof(T));
        return (T)t_enumArray.GetValue(UnityEngine.Random.Range(0, t_enumArray.Length));
    }
}
