using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecePreview : MonoBehaviour
{
    [SerializeField, InspectorReadOnly]
    Transform _piece;
    [SerializeField]
    GameObject _prefabPreviewBlock = null;

    [SerializeField, InspectorReadOnly]
    List<Transform> _listBlocks;

    SOGridConfig _soGridConfig;

    bool _canUpdate;

    private void Awake()
    {
        _soGridConfig = Resources.Load<SOGridConfig>("GridConfig");
        GetBlocks();
    }

    private void OnEnable()
    {
        SpawnController.OnSpawnPiece += CreatePreviewPiece;
        PieceController.OnPieceOnDelayBeforePlaced += ClearPreview;
        MainMenu.OnBackToMenu += ClearPreview;
    }

    private void OnDisable()
    {
        SpawnController.OnSpawnPiece -= CreatePreviewPiece;
        PieceController.OnPieceOnDelayBeforePlaced -= ClearPreview;
        MainMenu.OnBackToMenu -= ClearPreview;
    }

    void ClearPreview() => StartCoroutine(ClearPreviewAfterDelay());

    IEnumerator ClearPreviewAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        ClearPreview(null);
    }

    void ClearPreview(Transform pNull)
    {
        _piece = null;

        for (int index = 0; index < _listBlocks.Count; index++)
        {
            Destroy(_listBlocks[index].gameObject);
        }

        _listBlocks.Clear();
    }

    void CreatePreviewPiece(GameObject pPiece, SOColorConfig.GridColor pNull)
    {
        _piece = pPiece.transform;
        transform.position = _piece.transform.position;

        for (int index = 0; index < _piece.transform.childCount; index++)
        {
            Transform t_previewBlock = Instantiate(_prefabPreviewBlock, transform).transform;
            t_previewBlock.localPosition = _piece.GetChild(index).localPosition;
            t_previewBlock.localRotation = _piece.GetChild(index).localRotation;

            _listBlocks.Add(t_previewBlock);
        }

        _canUpdate = true;
    }

    void Update()
    {
        if (!_canUpdate)
            return;

        if (_piece != null && _piece.gameObject.activeInHierarchy)
            PlacePreviewPiece();
    }

    void PlacePreviewPiece()
    {
        transform.rotation = _piece.rotation;
        transform.position = new Vector3(_piece.position.x, transform.position.y, transform.position.z);

        for (int index = 0; index < _piece.childCount; index++)
        {
            RaycastHit t_hit;
            if (Physics.Raycast(_piece.GetChild(index).position, Vector3.down, out t_hit, _soGridConfig.GridSize.y * _soGridConfig.GridOffset, LayerMask.GetMask("Block", "Ground")))
            {
                if (t_hit.transform.position.y > _listBlocks[index].transform.position.y)
                {
                    transform.position = new Vector3(transform.position.x, _piece.position.y, transform.position.z);
                    break;
                }
            }
        }

        while (ValidatePosition())
        {
            transform.position -= new Vector3(0f, 0.1f, 0f);
        }

        transform.position += new Vector3(0f, 0.1f, 0f);
    }

    bool ValidatePosition()
    {
        for (int index = 0; index < _listBlocks.Count; index++)
        {
            Transform t_block = _listBlocks[index];

            Collider[] t_colliders = Physics.OverlapBox(t_block.position, t_block.localScale / 2.1f, Quaternion.identity, LayerMask.GetMask("Block", "Ground"));
            if (t_colliders.Length > 0)
                return false;
        }

        return true;
    }

    void GetBlocks()
    {
        _listBlocks = new List<Transform>();
        for (int index = 0; index < transform.childCount; index++)
        {
            _listBlocks.Add(transform.GetChild(index));
        }
    }
}
