using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverWindow : MonoBehaviour
{
    [SerializeField]
    Vector2 _minMaxScale = Vector2.zero;
    [SerializeField]
    Vector2 _minMaxPositionY = Vector2.zero;
    [SerializeField]
    Vector2 _minMaxOffsetY = Vector2.zero;

    [SerializeField, Space(7)]
    float _duration = 0f;

    [SerializeField, Space(15)]
    AudioClip _sfxGameOver = null;

    Material _mat;

    private void Awake() => _mat = GetComponent<MeshRenderer>().material;

    private void OnEnable() => PieceController.OnGameOver += ShowGameOver;

    private void OnDisable() => PieceController.OnGameOver -= ShowGameOver;

    void ShowGameOver(Transform pPiece) => StartCoroutine(AnimateGameOver());

    IEnumerator AnimateGameOver()
    {
        yield return new WaitForSeconds(0.5f);

        SoundManager.instance.PlaySFX(_sfxGameOver, true);

        float t_elapsedTime = 0f;

        Vector3 t_initialScale = new Vector3(transform.localScale.x, _minMaxScale.x, transform.localScale.z);
        Vector3 t_finalscale = new Vector3(transform.localScale.x, _minMaxScale.y, transform.localScale.z);

        Vector3 t_initialPosition = new Vector3(transform.position.x, _minMaxPositionY.x, transform.position.z);
        Vector3 t_finalPosition = new Vector3(transform.position.x, _minMaxPositionY.y, transform.position.z);

        Vector2 t_initialOffset = new Vector2(0f, _minMaxOffsetY.x);
        Vector2 t_finalOffset = new Vector2(0f, _minMaxOffsetY.y);

        while (t_elapsedTime < _duration)
        {
            transform.localScale = Vector3.Lerp(t_initialScale, t_finalscale, t_elapsedTime / _duration);
            transform.position = Vector3.Lerp(t_initialPosition, t_finalPosition, t_elapsedTime / _duration);
            _mat.SetTextureOffset("_BaseMap", Vector2.Lerp(t_initialOffset, t_finalOffset, t_elapsedTime / _duration));

            t_elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.localScale = t_finalscale;
        transform.position = t_finalPosition;
        _mat.SetTextureOffset("_BaseMap", t_finalOffset);

        DisableAllBlocks();

        yield return new WaitForSeconds(1.5f);

        t_elapsedTime = 0f;

        while (t_elapsedTime < _duration)
        {
            transform.localScale = Vector3.Lerp(t_finalscale, t_initialScale, t_elapsedTime / _duration);
            transform.position = Vector3.Lerp(t_finalPosition, t_initialPosition, t_elapsedTime / _duration);
            _mat.SetTextureOffset("_BaseMap", Vector2.Lerp(t_finalOffset, t_initialOffset, t_elapsedTime / _duration));

            t_elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.localScale = t_initialScale;
        transform.position = t_initialPosition;
        _mat.SetTextureOffset("_BaseMap", t_initialOffset);

        yield return new WaitForSeconds(0.65f);

        PlayerPrefs.SetInt("Restart", 1);
        SceneManager.LoadScene(0);
    }

    void DisableAllBlocks()
    {
        Collider[] t_blocks = Physics.OverlapSphere(Vector3.zero, 50f, LayerMask.GetMask("Block"));
        for (int index = 0; index < t_blocks.Length; index++)
        {
            t_blocks[index].gameObject.SetActive(false);
        }
    }
}
