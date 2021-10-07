using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridShine : MonoBehaviour
{
    [SerializeField]
    Vector2 _minMaxDelay = Vector2.zero;

    Animator _anim;

    private void Awake() => _anim = GetComponent<Animator>();

    private void Start() => StartCoroutine(Shine());

    IEnumerator Shine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(_minMaxDelay.x, _minMaxDelay.y));
            _anim.SetTrigger("Shine");
        }
    }
}
