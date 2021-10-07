using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SnapToGridPiece : MonoBehaviour
{
    [SerializeField, InspectorReadOnly]
    List<Transform> _listBlocks;

    [SerializeField, InspectorReadOnly]
    Transform _leftBlock;
    [SerializeField, InspectorReadOnly]
    Transform _rightBlock;
    [SerializeField, InspectorReadOnly]
    Transform _topBlock;
    [SerializeField, InspectorReadOnly]
    Transform _bottomBlock;

    SOGridConfig _soGridConfig;

    private void Awake()
    {
        _soGridConfig = Resources.Load<SOGridConfig>("GridConfig");

        GetBlocks();
    }

    private void LateUpdate()
    {
        if (!Application.isPlaying)
        {
            GetBlocks();
            FetchGridConfig();
        }

        KeepWithinGrid();

        if (!Application.isPlaying)
            Snap();
    }

    void KeepWithinGrid()
    {
        GetEdgeBlocks();
        transform.position += GetOutOfGridOffset();
    }

    public void Snap()
    {
        Vector3 t_newPosition = new Vector3
        (
            Mathf.Round(transform.position.x / _soGridConfig.GridOffset) * _soGridConfig.GridOffset,
            Mathf.Round(transform.position.y / _soGridConfig.GridOffset) * _soGridConfig.GridOffset,
            Mathf.Round(transform.position.z / _soGridConfig.GridOffset) * _soGridConfig.GridOffset
        );

        t_newPosition = new Vector3
        (
            Mathf.Clamp(t_newPosition.x, 0f, (_soGridConfig.GridSize.x - 1f) * _soGridConfig.GridOffset),
            Mathf.Clamp(t_newPosition.y, 0f, (_soGridConfig.GridSize.y - 1f) * _soGridConfig.GridOffset),
            Mathf.Clamp(t_newPosition.z, -100f, (_soGridConfig.GridSize.x - 1f) * _soGridConfig.GridOffset)
        );

        transform.position = t_newPosition;
    }

    Vector3 GetOutOfGridOffset()
    {
        float t_gridMaxX = (_soGridConfig.GridSize.x - 1f) * _soGridConfig.GridOffset;
        float t_gridMaxY = (_soGridConfig.GridSize.y - 1f) * _soGridConfig.GridOffset;

        Vector3 t_offset = Vector3.zero;

        if (_leftBlock.position.x < 0f)
            t_offset = new Vector3(Mathf.Abs(_leftBlock.position.x), 0f, 0f);
        else if (_rightBlock.position.x > t_gridMaxX)
            t_offset = new Vector3((_rightBlock.position.x - t_gridMaxX) * -1f, 0f, 0f);

        if (_bottomBlock.position.y < 0f)
            t_offset = new Vector3(t_offset.x, Mathf.Abs(_bottomBlock.position.y), 0f);
        else if (_topBlock.position.y > t_gridMaxY)
            t_offset = new Vector3(t_offset.x, (_topBlock.position.y - t_gridMaxY) * -1f, 0f);

        return t_offset;
    }

    void GetEdgeBlocks()
    {
        float t_minX = Mathf.Infinity;
        float t_maxX = -Mathf.Infinity;
        float t_minY = Mathf.Infinity;
        float t_maxY = -Mathf.Infinity;

        for (int index = 0; index < _listBlocks.Count; index++)
        {
            Transform t_block = _listBlocks[index];

            if (t_block.position.x > t_maxX)
            {
                t_maxX = t_block.position.x;
                _rightBlock = t_block;
            }
            
            if (t_block.position.x < t_minX)
            {
                t_minX = t_block.position.x;
                _leftBlock = t_block;
            }

            if (t_block.position.y > t_maxY)
            {
                t_maxY = t_block.position.y;
                _topBlock = t_block;
            }

            if (t_block.position.y < t_minY)
            {
                t_minY = t_block.position.y;
                _bottomBlock = t_block;
            }
        }
    }

    public void GetBlocks()
    {
        _listBlocks = new List<Transform>();
        for (int index = 0; index < transform.childCount; index++)
        {
            _listBlocks.Add(transform.GetChild(index));
        }
    }

    void FetchGridConfig()
    {
        if (!_soGridConfig)
            _soGridConfig = Resources.Load<SOGridConfig>("GridConfig");
    }
}
