using System.Collections;
using UnityEngine;

public class PieceController : MonoBehaviour
{
    [SerializeField, InspectorReadOnly]
    Transform _centerBlock;

    [SerializeField]
    SOColorConfig.GridColor _colorEnum;

    [SerializeField, Space(15)]
    AudioClip _sfxMove = null;

    SOGridConfig _soGridConfig;
    SOPiecesConfig _soPiecesConfig;
    SOColorConfig _soColorConfig;

    SnapToGridPiece _snapToGrid;

    Vector3 _initialPosition;

    bool _canUpdate;
    bool _canAdjust;
    bool _onAdjustDelay;

    const float PING_PONG_SIZE_DURATION = 0.1f;

    public delegate void PiecesEvents(Transform pPiece = null);
    public static event PiecesEvents OnPiecePlaced, OnPieceOnDelayBeforePlaced, OnGameOver;

    private void OnValidate() => ChangeBoxesColors();

    private void Awake()
    {
        _soGridConfig = Resources.Load<SOGridConfig>("GridConfig");
        _soPiecesConfig = Resources.Load<SOPiecesConfig>("PiecesConfig");
        _soColorConfig = Resources.Load<SOColorConfig>("ColorConfig");

        _snapToGrid = GetComponent<SnapToGridPiece>();

        _centerBlock = GetCenterBlock();
    }

    private void OnEnable() => MainMenu.OnBackToMenu += DestroySelf;

    private void OnDisable() => MainMenu.OnBackToMenu -= DestroySelf;

    void DestroySelf() => Destroy(gameObject);

    private void Start()
    {
        ChangeBoxesColors();

        _initialPosition = transform.position;

        _canUpdate = true;
        _canAdjust = true;
    }

    void ChangeBoxesColors()
    {
        for (int index = 0; index < transform.childCount; index++)
        {
            Block t_block = transform.GetChild(index).GetComponent<Block>();
            if (t_block)
                t_block.SetColor(_colorEnum);
        }
    }

    void ChangeBoxesColors(Color pColor)
    {
        for (int index = 0; index < transform.childCount; index++)
        {
            Block t_block = transform.GetChild(index).GetComponent<Block>();
            if (t_block)
                t_block.SetColor(pColor);
        }
    }

    Transform GetCenterBlock()
    {
        Transform t_centerBlock = null;

        for (int index = 0; index < transform.childCount; index++)
        {
            Transform t_block = transform.GetChild(index);

            if (t_block.CompareTag("CenterBlock"))
            {
                t_centerBlock = transform.GetChild(index);
                break;
            }
        }

        return t_centerBlock;
    }

    private void Update()
    {
        if (_canUpdate)
            MoveDown();

        if (_canAdjust)
        {
            MoveHorizontaly();
            RotatePiece();
        }
    }

    void MoveDown()
    {
        float t_accelerate = (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && !_onAdjustDelay ? _soPiecesConfig.AccelerationMultiplier : 1f;

        Vector3 t_offset = new Vector3(0f, ((_soPiecesConfig.Speed * t_accelerate) * Time.deltaTime) * -1f, 0f);
        if (ValidatePosition(t_offset))
            transform.position += t_offset;
        else if (!_onAdjustDelay)
        {
            _onAdjustDelay = true;

            OnPieceOnDelayBeforePlaced?.Invoke();
            StartCoroutine(DelayToAdjust());
        }
    }

    IEnumerator DelayToAdjust()
    {
        yield return StartCoroutine(LerpColor(_soColorConfig.White, _soPiecesConfig.AdjustDelay));

        _onAdjustDelay = false;

        Vector3 t_offset = new Vector3(0f, ((_soPiecesConfig.Speed * 2f) * Time.deltaTime) * -1f, 0f);
        if (ValidatePosition(t_offset))
            yield break;

        _canUpdate = false;
        _canAdjust = false;

        _snapToGrid.Snap();

        for (int index = 0; index < transform.childCount; index++)
        {
            transform.GetChild(index).gameObject.layer = LayerMask.NameToLayer("Block");
        }

        if (Vector3.Distance(transform.position, _initialPosition) > _soGridConfig.GridOffset * 0.9f)
        {
            _soPiecesConfig.IncreaseSpeed();

            yield return new WaitForEndOfFrame();
            OnPiecePlaced?.Invoke(transform);
            Destroy(_snapToGrid);
        }
        else
            OnGameOver?.Invoke();
    }

    IEnumerator LerpColor(Color pColor, float pDuration)
    {
        float t_elapsedTime = 0f;
        Color t_initialColor = _soColorConfig.White;
        Color t_finalColor = _soColorConfig.GetColor(_colorEnum);

        while (t_elapsedTime < pDuration)
        {
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                ChangeBoxesColors(t_finalColor);
                yield break;
            }

            ChangeBoxesColors(Color.Lerp(t_initialColor, t_finalColor, t_elapsedTime / pDuration));
            t_elapsedTime += Time.deltaTime;
            yield return null;
        }

        ChangeBoxesColors(t_finalColor);
    }

    void MoveHorizontaly()
    {
        Vector3 t_offset = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            t_offset = new Vector3(_soGridConfig.GridOffset, 0f, 0f);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            t_offset = new Vector3(-_soGridConfig.GridOffset, 0f, 0f);

        if (t_offset != Vector3.zero)
        {
            if (ValidatePosition(t_offset))
            {
                transform.position += t_offset;
                SoundManager.instance.PlaySFX(_sfxMove, true);
            }
        }
    }

    bool ValidatePosition(Vector3 pOffset)
    {
        for (int index = 0; index < transform.childCount; index++)
        {
            Transform t_block = transform.GetChild(index);
            Vector3 t_newPosition = t_block.position + (new Vector3(pOffset.x, pOffset.y * 2f, pOffset.z));

            if (t_newPosition.y < -0.01f || t_newPosition.x < -0.01f || t_newPosition.x > (_soGridConfig.GridSize.x - 1f) * _soGridConfig.GridOffset)
                return false;

            Collider[] t_colliders = Physics.OverlapBox(t_newPosition, t_block.localScale / 2.01f, Quaternion.identity, LayerMask.GetMask("Block"));
            if (t_colliders.Length > 0)
                return false;
        }

        return true;
    }

    void RotatePiece()
    {
        float t_degrees = 90f;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            for (int index = 0; index < 3; index++)
            {
                if (ValidateRotation(t_degrees, _centerBlock))
                {
                    if (_centerBlock)
                        transform.RotateAround(_centerBlock.position, Vector3.forward, t_degrees);
                    else
                        transform.Rotate(Vector3.forward, t_degrees);
                }
                else if (t_degrees < 270)
                    t_degrees += 90f;
            }
        }
    }

    bool ValidateRotation(float pDegrees, Transform pCenterObject = null)
    {
        Quaternion t_originalRotation = transform.rotation;
        Vector3 t_originalPosition = transform.position;

        if (pCenterObject)
            transform.RotateAround(pCenterObject.position, Vector3.forward, pDegrees);
        else
            transform.Rotate(Vector3.forward, pDegrees);

        if (ValidateWithXOffset())
        {
            transform.rotation = t_originalRotation;
            return true;
        }

        transform.rotation = t_originalRotation;
        transform.position = t_originalPosition;

        if (ValidateWithYOffset())
        {
            transform.rotation = t_originalRotation;
            return true;
        }

        transform.rotation = t_originalRotation;
        transform.position = t_originalPosition;
        return false;
    }

    bool ValidateWithXOffset()
    {
        for (int index = 0; index < 3; index++)
        {
            if (AllBoxesOnValidPosition())
                return true;
            else
            {
                if (transform.position.x <= (_soGridConfig.GridSize.x / 2f) * _soGridConfig.GridOffset)
                    transform.position += new Vector3(_soGridConfig.GridOffset, 0f, 0f);
                else
                    transform.position -= new Vector3(_soGridConfig.GridOffset, 0f, 0f);
            }
        }

        return false;
    }

    bool ValidateWithYOffset()
    {
        transform.position += new Vector3(_soGridConfig.GridOffset, 0f, 0f);

        for (int index = 0; index < 3; index++)
        {
            if (AllBoxesOnValidPosition())
                return true;
            else
                transform.position += new Vector3(_soGridConfig.GridOffset, 0f, 0f);
        }

        return false;
    }

    bool AllBoxesOnValidPosition()
    {
        for (int index = 0; index < transform.childCount; index++)
        {
            Transform t_block = transform.GetChild(index);

            if (t_block.position.y < 0f || t_block.position.x < 0f || t_block.position.x > (_soGridConfig.GridSize.x - 1f) * _soGridConfig.GridOffset)
                return false;

            Collider[] t_colliders = Physics.OverlapBox(t_block.position, t_block.localScale / 2.01f, Quaternion.identity, LayerMask.GetMask("Block"));
            if (t_colliders.Length > 0)
                return false;
        }

        return true;
    }
}
