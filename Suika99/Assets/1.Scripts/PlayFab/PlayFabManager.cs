using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayFabManager : MonoBehaviour
{
    public static string userName;

    [SerializeField] private GameObject accountPanel;

    [Header("Login")]
    [SerializeField] private GameObject loginPanel;

    [SerializeField] private TMP_InputField loginEmailInput;
    [SerializeField] private TMP_InputField loginPasswordInput;
    [SerializeField] private Toggle loginRemember;

    [SerializeField] private TMP_Text loginErrorText;

    [Header("Register")]
    [SerializeField] private GameObject registerPanel;

    [SerializeField] private TMP_InputField registerNameInput;
    [SerializeField] private TMP_InputField registerEmailInput;
    [SerializeField] private TMP_InputField registerPasswordInput;
    [SerializeField] private TMP_InputField registerRepeatPasswordInput;

    [SerializeField] private TMP_Text registerErrorText;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("email"))
        {
            string email = PlayerPrefs.GetString("email");

            LoginWithEmailAddressRequest request = new()
            {
                Email = email,
                Password = PlayerPrefs.GetString("password")
            };

            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        }
        else
        {
            accountPanel.SetActive(true);
            loginPanel.SetActive(true);
        }
    }

    public void Login()
    {
        LoginWithEmailAddressRequest request = new()
        {
            Email = loginEmailInput.text,
            Password = loginPasswordInput.text
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    public void Logout()
    {
        PlayerPrefs.DeleteKey("email");
        PlayerPrefs.DeleteKey("password");

        accountPanel.SetActive(true);
        loginPanel.SetActive(true);
    }

    public void Register()
    {
        if (registerPasswordInput.text == registerRepeatPasswordInput.text)
        {
            RegisterPlayFabUserRequest request = new()
            {
                Email = registerEmailInput.text,
                Password = registerRepeatPasswordInput.text,
                Username = registerNameInput.text
            };

            PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
        }
        else
        {
            registerErrorText.text = "Password don't match";
        }
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login success");
        if (loginRemember.isOn)
        {
            PlayerPrefs.SetString("email", loginEmailInput.text);
            PlayerPrefs.SetString("password", loginPasswordInput.text);
        }

        GetAccountInfoRequest request = new()
        {
            Email = loginEmailInput.text
        };

        PlayFabClientAPI.GetAccountInfo(request,
            result =>
            {
                userName = result.AccountInfo.Username;
                Debug.Log(userName);

                accountPanel.SetActive(false);
                loginPanel.SetActive(false);
                registerPanel.SetActive(false);
            },
            error =>
            {
                loginErrorText.text = error.ErrorMessage;
            });
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.Log("Login fail");
        loginErrorText.text = error.ErrorMessage;

        accountPanel.SetActive(true);
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Regist success");

        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.Log("Regist fail");
        registerErrorText.text = error.ErrorMessage;

        accountPanel.SetActive(true);
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }
}
