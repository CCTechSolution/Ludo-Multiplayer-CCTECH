using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(ScrollRect))]
public class ScrollRectEnsureVisible : MonoBehaviour
{

	#region Public Variables

	public float _AnimTime = 0.15f;
	public bool _Snap = false;
	public float topMargin = 0;
	#endregion

	#region Private Variables

	private ScrollRect mScrollRect;
	private RectTransform mContent;

	#endregion

	#region Unity Methods

	private void Awake ()
	{
		mScrollRect = GetComponent<ScrollRect> ();
		mContent = mScrollRect.content;
	}

	#endregion

	#region Public Methods
	
	public void CenterOnItem (RectTransform target)
	{
		Canvas.ForceUpdateCanvases();

		var targetPosition = LocalPositionWithinAncestor(mContent, target);
		var top = (-targetPosition.y) - target.rect.height / 2;
		var bottom = (-targetPosition.y) + target.rect.height / 2;

		 // this is here because there are headers over the buttons sometimes

		var result = mContent.anchoredPosition;
		if (result.y > top - topMargin)
			result.y = top - topMargin;
		if (result.y + mScrollRect.viewport.rect.height < bottom)
			result.y = bottom - mScrollRect.viewport.rect.height;

		//Debug.Log($"{targetPosition} {target.rect.size} {top} {bottom} {scrollRect.content.anchoredPosition}->{result}");

		
		if (_Snap)
		{
			mScrollRect.content.anchoredPosition = result;
		}
		else
		{
			
			mContent.anchoredPosition = result;

        }
		 
	}

	Vector3 LocalPositionWithinAncestor(Transform ancestor, Transform target)
	{
		var result = Vector3.zero;
		while (ancestor != target && target != null)
		{
			result += target.localPosition;
			target = target.parent;
		}
		return result;
	}
	#endregion
}