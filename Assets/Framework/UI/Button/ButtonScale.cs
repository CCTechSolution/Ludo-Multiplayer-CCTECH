using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;



namespace HK.UI
{
    [RequireComponent(typeof(Image))]
    public class ButtonScale : BaseButton
    {
        protected override void AnimatePress()
        {
            //Kill/*Sequence*/s();
            /*Sequence*/
            target.transform.DOScale(0.8f, 0.1f);
        }

        protected override void AnimateUnpress()
        {
            //Kill/*Sequence*/s();
            /*Sequence*/
            target.transform.DOScale(1f, 0.1f);
        }
    }
}

public static class TransformExtensions
{
    public class CoroutineHelper : MonoBehaviour { }
    private static CoroutineHelper mCoroutineHelper;

    private static CoroutineHelper coroutineHelper()
    {
        if (mCoroutineHelper == null)
        {
            GameObject helperObject = new GameObject("CoroutineHelper");
            mCoroutineHelper = helperObject.AddComponent<CoroutineHelper>();
            GameObject.DontDestroyOnLoad(helperObject);
        }

        return mCoroutineHelper;
    }


    // Double the scale of the transform
    private static IEnumerator mDOScale(Transform transform, float endScale, float duration)
    {
        Vector3 startScale = transform.localScale;

        float elapsedTime = 0f;
        //int gap = 0;
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, Vector3.one * endScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.one * endScale;
    }


    private static IEnumerator mDOScale(Transform transform, Vector3 endScale, float duration)
    {
        Vector3 startScale = transform.localScale;

        float elapsedTime = 0f;
        //int gap = 0;
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = endScale;
    }


    private static IEnumerator mDOScale(Transform transform, Vector3 endScale, float duration, System.Action onComplete = null)
    {
        Vector3 startScale = transform.localScale;
        float elapsedTime = 0f;
        //int gap = 0;
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;


            yield return null;
        }
        transform.localScale = endScale;
        onComplete?.Invoke();
    }
    private static IEnumerator mDOPosition(Transform transform, Vector3 endPosition, float duration)
    {
        Vector3 startPosition = transform.position;

        float elapsedTime = 0f;
        //int gap = 0;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        transform.position = endPosition;
    }
    private static IEnumerator mDOPosition(Transform transform, Vector3 endPosition, float duration, System.Action onComplete = null)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;
        //int gap = 0;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = endPosition;
        onComplete?.Invoke();
    }
    private static IEnumerator mDOLocalMove(Transform transform, Vector3 endPosition, float duration, System.Action onComplete = null)
    {
        Vector3 startPosition = transform.localPosition;
        float elapsedTime = 0f;
        //int gap = 0;
        while (elapsedTime < duration)
        {
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = endPosition;
        onComplete?.Invoke();
    }
    private static IEnumerator mDOLocalMove(Transform transform, Vector3 endPosition, float duration)
    {
        Vector3 startPosition = transform.localPosition;

        float elapsedTime = 0f;
        //int gap = 0;
        while (elapsedTime < duration)
        {
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = endPosition;
    }
    private static IEnumerator mDOAnchorPosY(RectTransform rectTransform, float endValue, float duration, System.Action onComplete = null)
    {
        float startY = rectTransform.anchoredPosition.y;
        float elapsedTime = 0f;
        //int gap = 0;
        while (elapsedTime < duration)
        {
            float newY = Mathf.Lerp(startY, endValue, elapsedTime / duration);
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, newY);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, endValue);
        onComplete?.Invoke();
    }
    private static IEnumerator mDOAnchorPos(RectTransform rectTransform, Vector2 endValue, float duration, System.Action onComplete = null)
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        float elapsedTime = 0f;
        //int gap = 0;
        while (elapsedTime < duration)
        {
            Vector2 newPos = Vector2.Lerp(startPos, endValue, elapsedTime / duration);
            rectTransform.anchoredPosition = newPos;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = endValue;
        onComplete?.Invoke();
    }



    
    public static void DOScale(this Transform transform, float endScale, float duration) { coroutineHelper().StartCoroutine(mDOScale(transform, endScale, duration)); }

    public static void DOScale(this Transform transform, Vector3 endScale, float duration) { coroutineHelper().StartCoroutine(mDOScale(transform, endScale, duration)); }
    public static void DOScale(this Transform transform, Vector3 endScale, float duration, System.Action onComplete = null) { coroutineHelper().StartCoroutine(mDOScale(transform, endScale, duration, onComplete)); }
    public static void DOPosition(this Transform transform, Vector3 endPosition, float duration) { coroutineHelper().StartCoroutine(mDOPosition(transform, endPosition, duration)); }
    public static void DOPosition(this Transform transform, Vector3 endPosition, float duration, System.Action onComplete = null) { coroutineHelper().StartCoroutine(mDOPosition(transform, endPosition, duration, onComplete)); }
    public static void DOLocalMove(this Transform transform, Vector3 endPosition, float duration, System.Action onComplete = null) { coroutineHelper().StartCoroutine(mDOLocalMove(transform, endPosition, duration, onComplete)); }
    public static void DOLocalMove(this Transform transform, Vector3 endPosition, float duration) { coroutineHelper().StartCoroutine(mDOLocalMove(transform, endPosition, duration)); }
    public static void DOAnchorPosY(this RectTransform rectTransform, float endValue, float duration, System.Action onComplete = null) { coroutineHelper().StartCoroutine(mDOAnchorPosY(rectTransform, endValue, duration, onComplete)); }
    public static void DOAnchorPos(this RectTransform rectTransform, Vector2 endValue, float duration, System.Action onComplete = null) { coroutineHelper().StartCoroutine(mDOAnchorPos(rectTransform, endValue, duration, onComplete)); }

}