using System.Collections;

using DuloGames.UI;
using UnityEngine;

public class PlayTween : MonoBehaviour
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
    [SerializeField] public GameObject target;
    [SerializeField] private Vector3 mShowVector = new Vector3(0, 2000, 0);
    [SerializeField] private Vector3 mHideVector = new Vector3(0, -2000, 0);
    [Range(0, 5)]
    public float mDuration = 0.3f;
    [SerializeField] private AudioClip m_AudioClipOut;
    [SerializeField] private AudioClip m_AudioClipIn;

    private Vector3 defaultVector;


    private void OnShow()
    {
        this.gameObject.SetActive(true);
    }


    public void AnimateShow()
    {
        StartCoroutine(Show());
    }

    public IEnumerator Show()
    {
        if (target != null)
        {
            UIAudioSource.Instance.PlayAudio(this.m_AudioClipOut);
            switch (m_AnimationType)
            {
                case Event.DOLocalMove:
                    defaultVector = target.transform.localPosition;
                    target.transform.localPosition = mShowVector;
                    OnShow();
                    yield return new WaitForSeconds(0.3f);
                    target.transform.localPosition = defaultVector;
                    break;
                case Event.DOMove:
                    defaultVector = target.transform.position;
                    target.transform.position = mShowVector;
                    OnShow();
                    yield return new WaitForSeconds(0.3f);
                    target.transform.position = defaultVector;
                    break;
                case Event.DOScale:
                    defaultVector = target.transform.localScale;
                    target.transform.localScale = mShowVector;
                    OnShow();
                    yield return new WaitForSeconds(0.3f);
                    target.transform.localScale = defaultVector;
                    break;
                case Event.DOShake:
                    // No direct equivalent for DOShake, you might need a custom implementation
                    break;
                case Event.DORotate:
                    defaultVector = target.transform.rotation.eulerAngles;
                    target.transform.eulerAngles = mShowVector;
                    OnShow();
                    yield return new WaitForSeconds(0.3f);
                    target.transform.eulerAngles = defaultVector;
                    break;
                case Event.DOLocalRotate:
                    defaultVector = target.transform.localRotation.eulerAngles;
                    target.transform.localEulerAngles = mShowVector;
                    OnShow();
                    yield return new WaitForSeconds(0.3f);
                    target.transform.localEulerAngles = defaultVector;
                    break;
            }
        }
    }


    private void OnHide()
    {
        this.gameObject.SetActive(false);
    }


    public void AnimateHide()
    {
        try
        {
            StartCoroutine(Hide());

        }
        catch (System.Exception)
        {
        }
    }

    public IEnumerator Hide()
    {
        if (target != null)
        {
            UIAudioSource.Instance.PlayAudio(this.m_AudioClipIn);
            yield return new WaitForSeconds(0.3f);

            switch (m_AnimationType)
            {
                case Event.DOLocalMove:
                    target.transform.localPosition = mHideVector;
                    OnHide();
                    target.transform.localPosition = defaultVector;
                    break;
                case Event.DOMove:
                    target.transform.position = mHideVector;
                    OnHide();
                    target.transform.position = defaultVector;
                    break;
                case Event.DOScale:
                    target.transform.localScale = mHideVector;
                    OnHide();
                    target.transform.localScale = defaultVector;
                    break;
                case Event.DOShake:
                    // No direct equivalent for DOShake, you might need a custom implementation
                    break;
                case Event.DORotate:
                    target.transform.eulerAngles = mHideVector;
                    OnHide();
                    target.transform.eulerAngles = defaultVector;
                    break;
                case Event.DOLocalRotate:
                    target.transform.localEulerAngles = mHideVector;
                    OnHide();
                    target.transform.localEulerAngles = defaultVector;
                    break;
            }
        }
    }

}
