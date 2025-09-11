using System.Text.RegularExpressions;
using HK;
using UnityEngine;
using UnityEngine.UI;

public class CustomLogin : BasePopup
{
    [Header("References")]
    public GameObject loadingPanel;    
    public GameObject networkErrorPanel;

    [Header("Sign-In")]
    public GameObject signinPanel;
    public InputField emailText;
    public InputField passwordText;
    public Text errorMessage;
    public Toggle rememberMeToggle;

    [Header("Sign-Up")]
    public GameObject signupPanel;
    public InputField r_emailText;
    public InputField r_passwordText;
    public InputField r_nickNameText;
    public Text r_errorMessage;

    [Header("Forget-Password")]
    public GameObject forgetPasswordPanel;
    public GameObject forgetEmail;
    public Text f_errorMessage;

    string email = "";
    string password = "";
    string nickname = "";
    bool isActive = false;

    public override void Subscribe()
    {
        Events.RequestIdLogin += AnimateShow;
        Events.RequestIdRegisterFailed += OnRegisterFailed;
        Events.RequestIdLoginFailed += OnLoginFailed;
        Events.RequestOnResetPassword += OnResetPassword;
    }

    public override void Unsubscribe()
    {
        Events.RequestIdLogin -= AnimateShow;
        Events.RequestIdRegisterFailed -= OnRegisterFailed;
        Events.RequestIdLoginFailed -= OnLoginFailed;
        Events.RequestOnResetPassword -= OnResetPassword;
    }

    protected override void OnStartShowing()
    {
        isActive = true;
        ShowLogin();
    }

    protected override void OnFinishHiding()
    {
        isActive = false;
    }


    public void ShowLogin()
    {
        loadingPanel.SetActive(false);
        signinPanel.SetActive(true);
        signupPanel.SetActive(false);
        forgetPasswordPanel.SetActive(false);
        networkErrorPanel.SetActive(false);
        rememberMeToggle.isOn = intToBool(PlayerPrefs.GetInt(Config.REMEMBER_ME_KEY, 1));
        errorMessage.text = "";
    }


    public void ShowRegister()
    {
        loadingPanel.SetActive(false);
        signinPanel.SetActive(false);
        signupPanel.SetActive(true);
        forgetPasswordPanel.SetActive(false);
        networkErrorPanel.SetActive(false);
        r_errorMessage.text = "";
    }

    public void ShowReset()
    {
        loadingPanel.SetActive(false);
        signinPanel.SetActive(false);
        signupPanel.SetActive(false);
        forgetPasswordPanel.SetActive(true);
        forgetEmail.SetActive(true);        
        f_errorMessage.gameObject.SetActive(false);
        f_errorMessage.text = "";        
        networkErrorPanel.SetActive(false);
    }

    public void OnIDLogin()
    {
        errorMessage.text = "";
        email = emailText.text;
        password = passwordText.text;

        if (!IsValidEmail(email))
        {
            errorMessage.text = "Invalid email formate";
            return;
        }

        if (!IsValidPassword(password))
        {
            errorMessage.text = "Invalid email password";
            return;
        }                
        loadingPanel.SetActive(true);
        signinPanel.SetActive(false);
        PlayFabManager.Instance?.LoginWithEmailAccount(email, password);       

    }

    public void OnRememberMe()
    {
        PlayerPrefs.SetInt(Config.REMEMBER_ME_KEY, boolToInt(rememberMeToggle.isOn));
        PlayerPrefs.Save();

    }



    public void OnIDRegister()
    {
        errorMessage.text = "";
        email = r_emailText.text;
        password = r_passwordText.text;
        nickname= r_nickNameText.text;

        if (!IsValidEmail(email))
        {
            errorMessage.text = "Invalid email formate";
            return;
        }

        if (!IsValidPassword(password))
        {
            errorMessage.text = "Invalid password";
            return;
        }

        if (!IsValidNickname(nickname))
        {
            errorMessage.text = "Nickname is empty";
            return;
        }

        loadingPanel.SetActive(true);
        signinPanel.SetActive(false);
        PlayFabManager.Instance?.LoginWithEmailAccount(email, password);

    }

    public void OnReset()
    {
        email = r_emailText.text;
        forgetEmail.SetActive(true);
        f_errorMessage.gameObject.SetActive(false);
        f_errorMessage.text = "";
        PlayFabManager.Instance?.ResetPassword(email);
    }     

    void OnRegisterFailed(string message)
    {
        if (!isActive)
            Show();
        loadingPanel.SetActive(false);
        signupPanel.SetActive(true);
        r_errorMessage.text = message;
    }

    void OnLoginFailed(string message)
    {
        if(!isActive)
            Show();
        loadingPanel.SetActive(false);
        signinPanel.SetActive(true);
        errorMessage.text = message;
    }

    void OnResetPassword(string message)
    {
        if (!isActive)
            Show();
        loadingPanel.SetActive(false);
        forgetPasswordPanel.SetActive(true);
        forgetEmail.SetActive(false);
        f_errorMessage.gameObject.SetActive(true);
        f_errorMessage.text = message;
    }

    public void OnClose()
    {
        loadingPanel.SetActive(false);
        signinPanel.SetActive(false);
        signupPanel.SetActive(false);
        forgetPasswordPanel.SetActive(false);
        Hide();
    }


    bool IsValidEmail(string _email)
    {
        return Regex.IsMatch(_email, @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" + @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$");
    }

    bool IsValidPassword(string _password)
    {
        return _password.Length >= 6;
    }

    bool IsValidNickname(string _nickname)
    {
        return _nickname.Length >= 6;
    }

    int boolToInt(bool val)
    {
        if (val)
            return 1;
        else
            return 0;
    }

    bool intToBool(int val)
    {
        if (val != 0)
            return true;
        else
            return false;
    }
}
