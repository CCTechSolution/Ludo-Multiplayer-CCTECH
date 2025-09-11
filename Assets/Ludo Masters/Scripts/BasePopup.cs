using System.Collections;

using DuloGames.UI;
using HK.UI;
using UnityEngine;

//[RequireComponent(typeof(Transform))]
public class BasePopup : CanvasElement
{
    public enum Event
    {
        DONone,
        DOLocalMove,
        DOMove,
        DOScale,
        DOShake,
        DORotate,
        DOLocalRotate,
    }
    [Header("References")]
    [SerializeField] private Event m_AnimationType = Event.DOLocalMove;
    [SerializeField] private GameObject target;
    [SerializeField] private Vector3 mShowVector = new Vector3(1, 1, 1);
    [SerializeField] private Vector3 mHideVector = new Vector3(0, -2000, 0);

    [Range(0, 5)]
    public float mDuration = 0.5f;
    [SerializeField] protected AudioClip m_AudioClipOut;
    [SerializeField] protected AudioClip m_AudioClipIn;
    private Vector3 defaultVector;

    //protected virtual void AnimateShow() { }
    // protected virtual void AnimateHide() { }

    protected virtual void AnimateShow()
    {
        if (target != null)
        {
            // Playing audio immediately
            if (UIAudioSource.Instance != null && this.m_AudioClipOut != null)
            {
                UIAudioSource.Instance.PlayAudio(this.m_AudioClipOut);
            }

            switch (m_AnimationType)
            {
                case Event.DOLocalMove:
                    defaultVector = target.transform.localPosition;
                    target.transform.DOLocalMove(mShowVector, mDuration, () => {
                        Show();
                    });
                    //target.transform.localPosition = mShowVector;
                    //Show();
                    //StartCoroutine(ResetLocalPosition(defaultVector, mDuration));
                    break;
                case Event.DOMove:
                    defaultVector = target.transform.position;
                    target.transform.DOPosition(mShowVector, mDuration, () => {
                        Show();
                    });
                    //target.transform.position = mShowVector;
                    //Show();
                    //StartCoroutine(ResetPosition(defaultVector, mDuration));
                    break;
                case Event.DOScale:
                    defaultVector = target.transform.localScale;
                    target.transform.DOScale(mShowVector, mDuration, ()=> {
                        Show();
                    });
                    //target.transform.localScale = mShowVector;
                   // Show();
     //StartCoroutine(ResetScale(defaultVector, mDuration));
                    break;
                case Event.DOShake:
                    // Custom implementation needed for shake, currently not handled
                    break;
                case Event.DORotate:
                    defaultVector = target.transform.rotation.eulerAngles;
                    target.transform.eulerAngles = mShowVector;
                    Show();
    //StartCoroutine(ResetRotation(defaultVector, mDuration));
                    break;
                case Event.DOLocalRotate:
                    defaultVector = target.transform.localRotation.eulerAngles;
                    target.transform.localEulerAngles = mShowVector;
                    Show();
    //StartCoroutine(ResetLocalRotation(defaultVector, mDuration));
                    break;
            }
        }
    }

    private IEnumerator ResetLocalPosition(Vector3 defaultPosition, float duration)
    {
        yield return new WaitForSeconds(0.3f);
        target.transform.localPosition = defaultPosition;
    }

    private IEnumerator ResetPosition(Vector3 defaultPosition, float duration)
    {
        yield return new WaitForSeconds(0.3f);
        target.transform.position = defaultPosition;
    }

    private IEnumerator ResetScale(Vector3 defaultScale, float duration)
    {
        yield return new WaitForSeconds(0.3f);
        target.transform.localScale = defaultScale;
    }

    private IEnumerator ResetRotation(Vector3 defaultRotation, float duration)
    {
        yield return new WaitForSeconds(0.3f);
        target.transform.eulerAngles = defaultRotation;
    }

    private IEnumerator ResetLocalRotation(Vector3 defaultLocalRotation, float duration)
    {
        yield return new WaitForSeconds(0.3f);
        target.transform.localEulerAngles = defaultLocalRotation;
    }



    protected virtual void AnimateHide()
    {
        if (target != null)
        {
            if (UIAudioSource.Instance != null && this.m_AudioClipIn != null)
            {
                UIAudioSource.Instance.PlayAudio(this.m_AudioClipIn);
            }

            switch (m_AnimationType)
            {
                case Event.DOLocalMove:
                    defaultVector = target.transform.localPosition;
                    target.transform.localPosition = mHideVector;
                    StartCoroutine(ResetLocalPosition(defaultVector, mDuration));
                    Hide();
                    break;
                case Event.DOMove:
                    defaultVector = target.transform.position;
                    target.transform.position = mHideVector;
                    StartCoroutine(ResetPosition(defaultVector, mDuration));
                    Hide();
                    break;
                case Event.DOScale:
                    defaultVector = target.transform.localScale;
                    target.transform.localScale = mHideVector;
                    StartCoroutine(ResetScale(defaultVector, mDuration));
                    Hide();
                    break;
                case Event.DOShake:
                    // Custom implementation needed for shake, currently not handled
                    break;
                case Event.DORotate:
                    defaultVector = target.transform.rotation.eulerAngles;
                    target.transform.eulerAngles = mHideVector;
                    StartCoroutine(ResetRotation(defaultVector, mDuration));
                    Hide();
                    break;
                case Event.DOLocalRotate:
                    defaultVector = target.transform.localRotation.eulerAngles;
                    target.transform.localEulerAngles = mHideVector;
                    StartCoroutine(ResetLocalRotation(defaultVector, mDuration));
                    Hide();
                    break;
            }
        }
    }

}
