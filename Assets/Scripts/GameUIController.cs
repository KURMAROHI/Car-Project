using System.Collections;

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class GameUIController : MonoBehaviour
{

    public static GameUIController Instance;
    [SerializeField] Transform AccleratorNormal, AccelaratorPressed;
    [SerializeField] Transform BreakNormal, BreakPressed;
    [SerializeField] RectTransform _DistanceContent;
    [SerializeField] Text _DistText;

    float _Multiplier, _ContentYpos;
    HorizontalLayoutGroup _DistHorizantallayoutGroup;

    public bool iSAccelratorApplied = false, isbreakApplied = false;

    [SerializeField] GameObject _Bar;

    float ViewPortWidth;

    [SerializeField] RenderTexture renderTexture;
    public Sprite UIScreenShotImage;
    [SerializeField] Transform GameCompleteCanvas;
    [SerializeField] GameObject BgFuelControl;


    bool IsScreenShottaken = false;

    void Awake()
    {

        PlayerPrefs.SetInt("OnGameEnd", 0);
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void OnEnable()
    {
        CollisionChek.HeadCollision += OnGameEnd;
    }

    void OnDisable()
    {

        CollisionChek.HeadCollision -= OnGameEnd;
    }

    void OnGameEnd()
    {
        Debug.Log("=====>game End");
        StartCoroutine("GameEndAnimation");
    }

    IEnumerator GameEndAnimation()
    {
        yield return new WaitForSeconds(1.5f);
        DriveCar.Instance.Car.linearVelocity = Vector2.zero;
        DriveCar.Instance.Car.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        DriveCar.Instance.BackTire.freezeRotation = true;
        DriveCar.Instance.FrontTire.freezeRotation = true;
        DriveCar.Instance.Car.bodyType = RigidbodyType2D.Static;
        StartCoroutine("TakeScreenShot");
        yield return new WaitUntil(() => IsScreenShottaken);
        IsScreenShottaken = false;
        BgFuelControl.transform.DOScale(new Vector3(2f, 2f, 2f), 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {

            GameObject ScreenShotGameobject = Instantiate(Resources.Load<GameObject>("ScreenShotParent"), GameCompleteCanvas);
        });
    }



    IEnumerator TakeScreenShot()
    {
        IsScreenShottaken = false;
        yield return new WaitForEndOfFrame();
        RenderTexture currentActiveRT = RenderTexture.active;
        RenderTexture.active = renderTexture;
        int _Width = Screen.width;
        int _Height = Screen.height;
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        // byte[] bytearray = texture.EncodeToPNG();
        // System.IO.File.WriteAllBytes(Application.dataPath + "/NewScreenShot.png", bytearray);
        RenderTexture.active = currentActiveRT;
        UIScreenShotImage = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
        IsScreenShottaken = true;
    }

    void Start()
    {
        _ContentYpos = _DistanceContent.anchoredPosition.y;
        _DistHorizantallayoutGroup = _DistanceContent.GetComponent<HorizontalLayoutGroup>();
        _Multiplier = _DistanceContent.GetChild(0).GetComponent<RectTransform>().rect.width / 100;
        ViewPortWidth = _DistanceContent.parent.GetComponent<RectTransform>().rect.width + 100f; //100f is An offset that i am Adding Extra

    }


    [SerializeField] float breakForce = 500f;
    public void AcceleratorBreakPointerDown(BaseEventData data)
    {
        // Debug.Log("onpointer Down");
        PointerEventData _pointerEventData = (PointerEventData)(data as BaseEventData);

        if (_pointerEventData.position.x > 0f && _pointerEventData.position.x < (Screen.width / 2 - 100f))
        {
            BreakNormal.gameObject.SetActive(false);
            BreakPressed.gameObject.SetActive(true);
            isbreakApplied = true;
        }
        else if (_pointerEventData.position.x > Screen.width / 2 + 100f && _pointerEventData.position.x < Screen.width)
        {

            AccleratorNormal.gameObject.SetActive(false);
            AccelaratorPressed.gameObject.SetActive(true);
            iSAccelratorApplied = true;

        }

    }




    public void AccleratorBreakPointerUp(BaseEventData data)
    {
        // Debug.Log("onpointer up");
        PointerEventData _pointerEventData = (PointerEventData)(data as BaseEventData);

        if (_pointerEventData.position.x > 0f && _pointerEventData.position.x < (Screen.width / 2 - 100f))
        {
            BreakNormal.gameObject.SetActive(true);
            BreakPressed.gameObject.SetActive(false);

            isbreakApplied = false;
        }
        else if (_pointerEventData.position.x > Screen.width / 2 + 100f && _pointerEventData.position.x < Screen.width)
        {
            AccleratorNormal.gameObject.SetActive(true);
            AccelaratorPressed.gameObject.SetActive(false);

            iSAccelratorApplied = false;
        }


    }

    int OldDistance = 0;
    public void SetDistanceTravelled(int Distancetravelled)
    {

        if (_DistanceContent.anchoredPosition.x < -(_DistanceContent.rect.width - ViewPortWidth))
        {
            Instantiate(_Bar, _DistanceContent);
        }
        if (OldDistance != Distancetravelled)
        {
            //` Debug.Log("Distnce travelled==>" + Distancetravelled);
            OldDistance = Distancetravelled;
            _DistanceContent.DOAnchorPosX(-Distancetravelled * 2, 0.1f).SetEase(Ease.Linear);
        }
        _DistText.text = Distancetravelled.ToString();
    }
}


