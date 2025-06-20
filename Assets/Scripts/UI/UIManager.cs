using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class UIManager : MonoBehaviour
{
    [Header("로그인용")]
    [SerializeField]
    private GameObject loginPanel;
    [SerializeField]
    private TMP_InputField emailInputField;
    [SerializeField]
    private TMP_InputField passwordInputField;
    [SerializeField]
    private Button loginButton;
    [SerializeField]
    private Button createAccountButton;

    [Header("가입용")]
    [SerializeField]
    private GameObject createAccountPanel;
    [SerializeField]
    private TMP_InputField createEmailInputField;
    [SerializeField]
    private TMP_InputField createNicknameInputField;
    [SerializeField]
    private TMP_InputField createPasswordInputField;
    [SerializeField]
    private TMP_InputField createPasswordConfirmInputField;
    [SerializeField]
    private Button createAccountConfirmButton;
    [SerializeField]
    private Button createAccountCancelButton;
    
    [Header("게시판")]
    [SerializeField]
    private GameObject _postListPanel;
    
    [Header("글작성")]
    [SerializeField]
    private Button _showPostWriteButton;
    [SerializeField]
    private GameObject _postWritePanel;

    private void Awake()
    {
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        createAccountButton.onClick.AddListener(OnCreateAccountButtonClicked);
        createAccountConfirmButton.onClick.AddListener(OnCreateAccountConfirmButtonClicked);
        createAccountCancelButton.onClick.AddListener(OnCreateAccountCancelButtonClicked);
        _showPostWriteButton.onClick.AddListener(ShowPostWritePanel);
    }

    private void ShowPostWritePanel()
    {
        loginPanel.SetActive(false);
        createAccountPanel.SetActive(false);
        _postListPanel.SetActive(false);
        _postWritePanel.SetActive(true);
    }

    private void OnLoginButtonClicked()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;
        _ = LoginAsync(email, password);
    }

    private async Task LoginAsync(string email, string password)
    {
        try
        {
            await AccountManager.Instance.LoginAsync(email, password);
            _postListPanel.SetActive(true);
            Debug.Log("로그인 성공");
            // 로그인 성공 후 UI 처리 추가 가능
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"로그인 실패: {ex.Message}");
            // 실패 시 UI 처리 추가 가능
        }
    }


    private void OnCreateAccountButtonClicked()
    {
        loginPanel.SetActive(false);
        createAccountPanel.SetActive(true);
    }

    private void OnCreateAccountConfirmButtonClicked()
    {
        string email = createEmailInputField.text;
        string nickname = createNicknameInputField.text;
        string password = createPasswordInputField.text;
        string passwordConfirm = createPasswordConfirmInputField.text;
        if (password != passwordConfirm)
        {
            Debug.LogError("비밀번호가 일치하지 않습니다.");
            return;
        }
        _ = CreateAccountAsync(email, password, nickname);
    }

    private async Task CreateAccountAsync(string email, string password, string nickname)
    {
        try
        {
            await AccountManager.Instance.CreateAccountAsync(email, password, nickname);
            Debug.Log("계정 생성 성공");
            // 계정 생성 성공 후 UI 처리 추가 가능
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"계정 생성 실패: {ex.Message}");
            // 실패 시 UI 처리 추가 가능
        }
    }


    private void OnCreateAccountCancelButtonClicked()
    {
        createAccountPanel.SetActive(false);
        loginPanel.SetActive(true);
    }


}
