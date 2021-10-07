using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineVerifier : MonoBehaviour
{
    [SerializeField]
    float _durationBlocksFall = 0f;

    SOGridConfig _soGridConfig;

    ScoreController _scoreControl;

    private void Awake()
    {
        _soGridConfig = Resources.Load<SOGridConfig>("GridConfig");

        _scoreControl = GetComponent<ScoreController>();
    }

    private void OnEnable() => PieceController.OnPiecePlaced += Verify;

    private void OnDisable() => PieceController.OnPiecePlaced -= Verify;

    void Verify(Transform pPiece)
    {
        GetMinMaxBlocks(out float t_yMin, out float t_yMax, pPiece);

        int t_quantityLinesDestroyed = 0;
        int t_linesToVerify = Mathf.FloorToInt((t_yMax - t_yMin) / _soGridConfig.GridOffset) + 1;

        RaycastHit[] t_raycastHitsLast = null;

        for (int index = 0; index < t_linesToVerify; index++)
        {
            Vector3 t_rayStartPosition = new Vector3(-1f, t_yMin + (_soGridConfig.GridOffset * index), 0);
            RaycastHit[] t_raycastHits = Physics.RaycastAll(t_rayStartPosition, Vector3.right, _soGridConfig.GridSize.x * _soGridConfig.GridOffset, LayerMask.GetMask("Block"));

            if (LineCompleted(t_raycastHits.Length))
            {
                DestroyBlocks(t_raycastHits);
                t_quantityLinesDestroyed++;

                t_raycastHitsLast = t_raycastHits;
            }
        }

        if (t_raycastHitsLast != null && t_raycastHitsLast.Length > 0)
            BringAboveBlocksDown(t_raycastHitsLast, t_quantityLinesDestroyed);

        _scoreControl.Score(t_quantityLinesDestroyed);
    }

    void BringAboveBlocksDown(RaycastHit[] pRaycastHitsLast, int pNumberOfLinesDown)
    {
        for (int index = 0; index < pRaycastHitsLast.Length; index++)
        {
            RaycastHit[] t_raycastHits = Physics.RaycastAll(pRaycastHitsLast[index].transform.position, Vector3.up, _soGridConfig.GridSize.y * _soGridConfig.GridOffset, LayerMask.GetMask("Block"));
            for (int innerIndex = 0; innerIndex < t_raycastHits.Length; innerIndex++)
            {
                Vector3 t_newPosition = t_raycastHits[innerIndex].transform.position - new Vector3(0f, _soGridConfig.GridOffset * pNumberOfLinesDown, 0f);
                StartCoroutine(LerpTransformToPosition(t_raycastHits[innerIndex].transform, t_newPosition, _durationBlocksFall));
            }
        }
    }

    void CheckAboveBlock(Transform pBlock)
    {
        RaycastHit t_hit;
        if (Physics.Raycast(pBlock.position, Vector3.up, out t_hit, _soGridConfig.GridOffset, LayerMask.GetMask("Block")))
        {
            Transform t_aboveBlock = t_hit.transform;

            CheckAboveBlock(t_aboveBlock);

            StartCoroutine(LerpTransformToPosition(t_aboveBlock, pBlock.position, _durationBlocksFall));
        }
    }

    void GetMinMaxBlocks(out float outMin, out float outMax, Transform pPiece)
    {
        outMin = float.MaxValue;
        outMax = 0f;

        for (int index = 0; index < pPiece.childCount; index++)
        {
            Transform t_block = pPiece.GetChild(index);
            if (t_block.position.y < outMin)
                outMin = t_block.position.y;

            if (t_block.position.y > outMax)
                outMax = t_block.position.y;
        }
    }

    bool LineCompleted(float pRaycastHitsLength) => pRaycastHitsLength == _soGridConfig.GridSize.x;

    void DestroyBlocks(RaycastHit[] pRaycastHits)
    {
        for (int index = 0; index < pRaycastHits.Length; index++)
        {
            Block t_block = pRaycastHits[index].transform.GetComponent<Block>();
            if (t_block)
                t_block.Destroy();
        }
    }

    IEnumerator LerpTransformToPosition(Transform pTransform, Vector3 pFinalPosition, float pDuration)
    {
        float t_elapsedTime = 0f;
        Vector3 t_initialPosition = pTransform.position;

        while (t_elapsedTime < pDuration)
        {
            pTransform.position = Vector3.Lerp(t_initialPosition, pFinalPosition, t_elapsedTime / pDuration);
            t_elapsedTime += Time.deltaTime;
            yield return null;
        }

        pTransform.position = pFinalPosition;
    }
}
