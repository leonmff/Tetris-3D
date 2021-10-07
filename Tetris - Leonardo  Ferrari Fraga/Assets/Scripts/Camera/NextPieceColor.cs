using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextPieceColor : MonoBehaviour
{
    [SerializeField]
    SOColorConfig.GridColor _color;

    SOColorConfig _soColorConfig;

    private void OnValidate()
    {
        if (!_soColorConfig)
            _soColorConfig = Resources.Load<SOColorConfig>("ColorConfig");

        SetColor();
    }

    private void Awake() => _soColorConfig = Resources.Load<SOColorConfig>("ColorConfig");

    private void Start() => SetColor();

    void SetColor()
    {
        for (int index = 0; index < transform.childCount; index++)
        {
            MeshRenderer t_meshRenderer = transform.GetChild(index).GetComponent<MeshRenderer>();
            MaterialPropertyBlock t_propertyBlock = new MaterialPropertyBlock();
            t_meshRenderer.GetPropertyBlock(t_propertyBlock, 0);
            t_propertyBlock.SetColor("_BaseColor", _soColorConfig.GetColor(_color));
            t_meshRenderer.SetPropertyBlock(t_propertyBlock, 0);
        }
    }
}
