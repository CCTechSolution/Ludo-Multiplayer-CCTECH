using UnityEngine;
using UnityEngine.UI;

namespace HK.UI
{
  [RequireComponent(typeof(Image))]
  public class ButtonColor : BaseButton
  {
    [Space]
    public Color pressedColor = new Color(200 / 255f, 200 / 255f, 200 / 255f, 255 / 255f);

    protected Color normalColor;

    protected override void Awake()
    {
      base.Awake();

      normalColor = GetColor();
    }

    protected override void AnimatePress()
    {
      //Kill/*Sequence*/s();
      /*Sequence*/Color(pressedColor);
    }

    protected override void AnimateUnpress()
    {
      //Kill/*Sequence*/s();
      /*Sequence*/Color(normalColor);
    }
  }
}
