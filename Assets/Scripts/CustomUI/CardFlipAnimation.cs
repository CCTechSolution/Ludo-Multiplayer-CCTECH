using UnityEngine;
using UnityEngine.UI;

public class CardFlipAnimation : MonoBehaviour
{
    [SerializeField] private GameObject cardFront;
    [SerializeField] private GameObject cardBack;
    void Start()
    {
        cardFront.GetComponent<Button>().onClick.AddListener(HalfRotate);
        cardBack.GetComponent<Button>().onClick.AddListener(HalfReverseRotate);
    }

    void RotateFront(float degrees, string onComplete)
    {
        iTween.RotateTo(cardFront, iTween.Hash(
            "rotation", new Vector3(0, degrees, 0),
            "time", 0.1f,
            "easetype", iTween.EaseType.linear,
            "onComplete", onComplete,
            "onCompleteTarget", gameObject
            ));
    }

    void RotateFront(float degrees)
    {
        iTween.RotateTo(cardFront, iTween.Hash(
            "rotation", new Vector3(0, degrees, 0),
            "time", 0.1f,
            "easetype", iTween.EaseType.linear
            ));
    }

    void RotateBack(float degrees)
    {
        iTween.RotateTo(cardBack, iTween.Hash(
            "rotation", new Vector3(0, degrees, 0),
            "time", 0.1f,
            "easetype", iTween.EaseType.linear
            ));

        iTween.MoveTo(gameObject, iTween.Hash(


            ));
    }

    void HalfRotate()
    {
        FullRotate();
        RotateBack(270);
    }

    void FullRotate()
    {
        cardFront.transform.SetAsFirstSibling();
        RotateFront(180);
        RotateBack(360);
    }

    void HalfReverseRotate()
    {
        RotateFront(90, "FullReverseRotate");
        RotateBack(270);
    }

    void FullReverseRotate()
    {
        cardBack.transform.SetAsFirstSibling();
        RotateFront(0);
        RotateBack(180);
    }
}
 
