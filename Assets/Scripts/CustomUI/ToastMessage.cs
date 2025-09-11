
using HK;
using HK.UI;
using UnityEngine;
using UnityEngine.UI;

public class ToastMessage : CanvasElement
{

    [SerializeField] private AudioClip m_AudioClipOut;
    [SerializeField] private AudioClip m_AudioClipIn;
    public override void Subscribe()
    {
        Events.RequestToast += OnShow;
    }

    public override void Unsubscribe()
    {
        Events.RequestToast -= OnShow;
    }

   

    protected override void OnStartShowing()
    {
      
    }



    void OnShow(string message)
    {
        Show();
        SoundsController.Instance?.PlayOneShot(GetComponent<AudioSource>(), this.m_AudioClipOut);
        GetComponentInChildren<Text>().text = message;
        transform.DOLocalMove(new Vector3(0, 1145, 0), 0.2f,() => {
            Invoke(nameof(HideToast), 3);
        });
    }

    void HideToast()
    {
        SoundsController.Instance?.PlayOneShot(GetComponent<AudioSource>(), this.m_AudioClipIn);
        transform.DOLocalMove(new Vector3(0, 1700, 0), 0.2f,()=> { Hide(); });

    }

}
