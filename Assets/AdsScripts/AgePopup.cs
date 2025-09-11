using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgePopup : MonoBehaviour
{
    public List<Button> AnswerButton;
    public List<Button> AgeButtons;

    public List<RectTransform> DoneBtn;

    public List<Text> TextMsgs;

    bool correct = false;
    int age = 0;
    Button answerSelected;
    string msg = "";


    public static bool Succeeded
    {
        get { return UserAge() > 0; }
    }

    public static int UserAge()
    {
        return PlayerPrefs.GetInt("USERAGE", -1);
    }

    // Start is called before the first frame update
    void Start()
    {
        AnswerButton.ForEach(b => b.GetComponent<Image>().color = Color.white);
        DoneBtn.ForEach(b => b.GetComponent<Button>().interactable = false);

        msg = TextMsgs[0].text;
    }


    public void OnAnswer(Button button)
    {
        AgeButtons.ForEach(b => b.GetComponent<Image>().color = Color.white);


        AnswerButton.ForEach(b => b.GetComponent<Image>().color = Color.white);

        correct = button.GetComponentInChildren<Text>().text == "8";
        button.GetComponent<Image>().color = Color.gray;
        answerSelected = button;

        if (!correct)
        {
            DoneBtn.ForEach(b => b.GetComponent<Image>().color = Color.red);
            DoneBtn.ForEach(b => b.GetComponent<Button>().interactable = false);
        }

    }


    int mAge = -1;

    public void HighlightAgeButton(Button btn)
    {
        AgeButtons.ForEach(b => b.GetComponent<Image>().color = Color.white);
        btn.GetComponent<Image>().color = Color.gray;
    }

    public void OnAge(int age)
    {

        if (answerSelected == null && !correct)
        {
            StartCoroutine(PromtQuestion());
            return;
        }
        mAge = age;

        if (correct)
        {

            StartCoroutine(OnAgeSelected());
        }
        else
        {

            StartCoroutine(Nodge());
        }
    }

    IEnumerator PromtQuestion()
    {
        TextMsgs.ForEach(x => x.text = "How Much is 4+4");

        AnswerButton.ForEach(b => b.transform.localScale = Vector2.one * 1.5F);

        //for (int i = 9; i > 1; i--)
        //{
        //    AnswerButton.ForEach(b => b.transform.localScale = Vector2.one * (1.5F - (i / 9)));
        //    yield return new WaitForSeconds((i * Time.deltaTime * 2));
        //}

        yield return new WaitForSeconds(1);

        TextMsgs.ForEach(x => x.text = msg);
        AnswerButton.ForEach(b => b.transform.localScale = Vector2.one);
        AgeButtons.ForEach(b => b.GetComponent<Image>().color = Color.white);

        yield return new WaitForEndOfFrame();
    }


    IEnumerator OnAgeSelected()
    {
        if (answerSelected)
            answerSelected.GetComponent<Image>().color = Color.green;

        DoneBtn.ForEach(b => b.GetComponent<Image>().color = Color.white);
        DoneBtn.ForEach(b => b.GetComponent<Button>().interactable = true);
        yield return new WaitForEndOfFrame();
    }


    IEnumerator Nodge()
    {

        DoneBtn.ForEach(b => b.GetComponent<Image>().color = Color.red);
        DoneBtn.ForEach(b => b.GetComponent<Button>().interactable = false);

        if (answerSelected)
            answerSelected.GetComponent<Image>().color = Color.red;

        for (int i = 0; i < 9; i++)
        {
            var offset = new Vector2((60 / (i + 1)) * (7 - i) * 0.5F * ((i % 2 == 0) ? 1 : -1)
                    , 0);


            for (int j = 0; j < DoneBtn.Count; j++)
            {
                var b = DoneBtn[j];

                b.offsetMin =
                b.offsetMax = offset;

            }
            yield return new WaitForSeconds((i * Time.deltaTime));
        }

        DoneBtn.ForEach(b =>
        {
            b.offsetMax = Vector2.zero;
        });


        AnswerButton.ForEach(b => b.GetComponent<Image>().color = Color.white);
        AgeButtons.ForEach(b => b.GetComponent<Image>().color = Color.white);

        answerSelected = null;

        yield return new WaitForEndOfFrame();
    }

    public void OnDone()
    {

        PlayerPrefs.SetInt("USERAGE", mAge);
        PlayerPrefs.Save();
        gameObject.SetActive(false);
    }
}
