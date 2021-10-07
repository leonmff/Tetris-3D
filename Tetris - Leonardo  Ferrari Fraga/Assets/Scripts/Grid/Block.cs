using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField, InspectorReadOnly]
    bool _selected;
    public bool Selected { get => _selected; }

    SOColorConfig.GridColor _color = SOColorConfig.GridColor.Aqua;

    SOColorConfig _soColorConfig;

    [SerializeField]
    ParticleSystem _psExplosion = null;

    BoxCollider _boxCollider;
    MeshRenderer _meshRenderer;
    MeshRenderer _meshRendererChild;

    private void Awake()
    {
        _soColorConfig = Resources.Load<SOColorConfig>("ColorConfig");

        _boxCollider = GetComponent<BoxCollider>();
        
        GetRenderers();
    }

    public void SetColor(SOColorConfig.GridColor pColor)
    {
        GetRenderers();

        _color = pColor;

        MaterialPropertyBlock t_propertyBlock = new MaterialPropertyBlock();
        _meshRenderer.GetPropertyBlock(t_propertyBlock, 0);
        t_propertyBlock.SetColor("_BaseColor", _soColorConfig.GetColor(_color));
        _meshRenderer.SetPropertyBlock(t_propertyBlock, 0);

        t_propertyBlock = new MaterialPropertyBlock();
        _meshRendererChild.GetPropertyBlock(t_propertyBlock, 0);
        t_propertyBlock.SetColor("_BaseColor", _soColorConfig.GetColor(_color));
        _meshRendererChild.SetPropertyBlock(t_propertyBlock, 0);
    }

    public void SetColor(Color pColor)
    {
        GetRenderers();

        MaterialPropertyBlock t_propertyBlock = new MaterialPropertyBlock();
        _meshRenderer.GetPropertyBlock(t_propertyBlock, 0);
        t_propertyBlock.SetColor("_BaseColor", pColor);
        _meshRenderer.SetPropertyBlock(t_propertyBlock, 0);

        t_propertyBlock = new MaterialPropertyBlock();
        _meshRendererChild.GetPropertyBlock(t_propertyBlock, 0);
        t_propertyBlock.SetColor("_BaseColor", pColor);
        _meshRendererChild.SetPropertyBlock(t_propertyBlock, 0);
    }

    void GetRenderers()
    {
        if (!_meshRenderer)
            _meshRenderer = GetComponent<MeshRenderer>();

        if (!_meshRendererChild)
            _meshRendererChild = transform.GetChild(0).GetComponent<MeshRenderer>();

        if (!_soColorConfig)
            _soColorConfig = Resources.Load<SOColorConfig>("ColorConfig");
    }

    public void Destroy()
    {
        ParticleSystem.MainModule t_main = _psExplosion.main;
        t_main.startColor = _soColorConfig.GetColor(_color);
        _psExplosion.Play();
        _meshRenderer.enabled = false;
        _meshRendererChild.enabled = false;
        _boxCollider.enabled = false;
        Destroy(gameObject, 2f);
    }
}
