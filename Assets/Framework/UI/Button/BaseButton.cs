using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using HK.Core;
using System.Collections;



namespace HK.UI
{
  [RequireComponent(typeof(Image))]
  public abstract class BaseButton : Base, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
  {
    public Image target;
    public UnityEvent onClick;

    private Image image;

    public bool IsInteractable() { return image.raycastTarget; }
    public void SetIsInteractable(bool isInteractable) { image.raycastTarget = isInteractable; }

    protected virtual void Awake()
    {
      image = GetComponent<Image>();
      image.raycastTarget = true;
      target.raycastTarget = false;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
      onClick?.Invoke();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
      AnimatePress();
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
      AnimateUnpress();
    }
        public virtual void AnimateFadeShow(float delay = 0, float showTime = 0.6f)
        {
            target.color = new Color(target.color.r, target.color.g, target.color.b, 0);
            ShowAnimationUtil(() => target.color = new Color(target.color.r, target.color.g, target.color.b, 1) ,delay);
        }

        public virtual void AnimateScaleShow(float delay = 0, float showTime = 0.3f)
        {
            target.transform.localScale = Vector3.zero;
            ShowAnimationUtil(() => target.transform.localScale= Vector3.one, delay);
        }

        private void ShowAnimationUtil(System.Action tweenAction, float delay)
        {
            SetIsInteractable(false);
            tweenAction?.Invoke();
            StartCoroutine(PerformAfterDelay(delay));
        }

        private IEnumerator PerformAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            // Enable interactability after the delay
            SetIsInteractable(true);
        }


        protected abstract void AnimatePress();
    protected abstract void AnimateUnpress();
  }
}
