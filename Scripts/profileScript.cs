using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.UI;
using System.Security.Cryptography;

public class profileScript : MonoBehaviour
{
    public InputField nameField;
    public InputField passField;

    InputField logField, passRegField;
    InputField searth_club_field;

    public Text loginErrorText;
    public Text passErrorText;

    gameUser user;

    public GameObject player_head_panel;
    Text chips_count_label;
    Text coins_count_label;
    Text diamond_count_label;
    Text player_club_name_label;
    Text player_name_label;
    Text player_id_label;

    Transform logo_panel;
    Transform enter_panel;
    Transform register_panel;
    Transform menu_club_panel;
    Transform menu_main_club_panel;
    Transform menu_create_club_panel;
    Transform club_page;
    Transform profile_menu;
    
    private DatabaseReference dbRef = null;
    public DatabaseReference dbClubs = null;
    private DataSnapshot snapShot = null;
    public DataSnapshot snapShotClub = null;

    private string dbLink = "https://test.firebaseio.com/";
    private string login = "";
    private string password;
    private string player_club_name;
    private string player_club_id;
    string fishki;

    public static int numPlayers = 4;

    [SerializeField]public static float gameVersion = 0.44f;

    public bool namecheck = false, passcheck = false;

    string playerCheck;

    void Start()
    {
        playerCheck = PlayerPrefs.GetString("playerLogin");


        logo_panel = transform.Find("logo_panel");
        enter_panel = transform.Find("login_panel");
        register_panel = transform.Find("register_panel");
        menu_club_panel = transform.Find("club_menu");
        menu_main_club_panel = transform.Find("main_club_menu");
        menu_create_club_panel = transform.Find("create_club_menu");
        profile_menu = transform.Find("profile_menu");
        club_page = transform.Find("club_page");

        searth_club_field = menu_club_panel.Find("searth_input_field").GetComponent<InputField>();

        loginErrorText = transform.Find("login_panel").Find("loginErrorLabel").GetComponent<Text>();
        passErrorText = transform.Find("login_panel").Find("passErrorLabel").GetComponent<Text>();
        logField = transform.Find("register_panel").Find("login_field").GetComponent<InputField>();
        passRegField = transform.Find("register_panel").Find("pas_field").GetComponent<InputField>();

        chips_count_label = player_head_panel.transform.Find("chips_panel").Find("chips_count_label").GetComponent<Text>();
        coins_count_label = player_head_panel.transform.Find("coins_panel").Find("coins_count_label").GetComponent<Text>();
        diamond_count_label = player_head_panel.transform.Find("diamond_panel").Find("diamond_count_label").GetComponent<Text>();
        //player_club_name_label = menu_main_club_panel.Find("club_name_label").GetComponent<Text>();
        player_name_label = profile_menu.Find("player_name_label").GetComponent<Text>();
        player_id_label = profile_menu.Find("player_id_label").GetComponent<Text>();

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(dbLink);
        dbRef = FirebaseDatabase.DefaultInstance.GetReference("users");
        dbClubs = FirebaseDatabase.DefaultInstance.GetReference("clubs");

        menu_main_club_panel.GetComponent<mainClubPage>().dbClubs = dbClubs;
        menu_main_club_panel.GetComponent<mainClubPage>().Initializate();

        dbRef.ValueChanged += onDBChange;
        dbRef.ChildChanged += dbRef_ChildChanged;
        
        //dbClubs.ChildChanged += new System.EventHandler<ChildChangedEventArgs>(dbClubs_ChildChanged);
        dbClubs.ValueChanged += new System.EventHandler<ValueChangedEventArgs>(onDBClubChildChange);

        register_panel.gameObject.SetActive(false);
        logo_panel.gameObject.SetActive(false);
        profile_menu.gameObject.SetActive(false);
        enter_panel.gameObject.SetActive(false);
        menu_main_club_panel.gameObject.SetActive(false);
        menu_create_club_panel.gameObject.SetActive(false);
        club_page.gameObject.SetActive(false);

        if (PlayerPrefs.GetFloat("version") != gameVersion)
        {
            if (playerCheck == "login")
            {
                enter_panel.gameObject.SetActive(true);
            }
            else
            {
                logo_panel.gameObject.SetActive(true);
                StartCoroutine(show_logo());
            }
            PlayerPrefs.SetFloat("version", gameVersion);
        }
        else
        {
            if (playerCheck == "login")
            {
                menu_club_panel.gameObject.SetActive(true);
                StartCoroutine(updateGame());
            }
            else
            {
                logo_panel.gameObject.SetActive(true);
                StartCoroutine(show_logo());
            }
        }
        //string genStr = helperGame.GetMd5Hash(MD5.Create(), "6");
        //Debug.Log(genStr);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !Input.GetKeyUp(KeyCode.Escape)) // Ловим кнопку назад
        {
            if (menu_club_panel.gameObject.activeSelf) // Если находимся на главном меню
            {
                menu_club_panel.gameObject.SetActive(false);
                PlayerPrefs.DeleteAll();
                enter_panel.gameObject.SetActive(true);
            }
            StartCoroutine(updateGame());
        }
    }
    void UpdateDataBase()
    {
        dbRef.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                snapShot = task.Result;
                return;
            }
            else if (task.IsFaulted)
            {
                return;
            }
        });
    }
    void UpdateClubBase()
    {
        dbClubs.GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                snapShotClub = task.Result;
                return;
            }
            else if (task.IsFaulted)
            {
                return;
            }
        });
    }
    void onDBChange(object sender, ValueChangedEventArgs args) // Если где-то в user меняется значение
    {
        //gameUser user = JsonUtility.FromJson<gameUser>(args.Snapshot.GetRawJsonValue());
        //Debug.Log("user "+args.Snapshot.Key+" change value");
    }
    void dbRef_ChildChanged(object sender, ChildChangedEventArgs args) // изменение в дочернем элементе базы игроков
    {
        //StartCoroutine(updateGame());
        Debug.Log("user " + args.Snapshot.Key + " change value");
    }
    void onDBClubChildChange(object sender, ValueChangedEventArgs args) // Если где-то в user меняется значение
    {
        StartCoroutine(updateGame());
        Debug.Log("событие в "+args.Snapshot.Key.ToString());
    }
    void dbClubs_ChildChanged(object sender, ChildChangedEventArgs e) // При изменении в клубе
    {
        Debug.Log("событие из: "+e.Snapshot.Key);
    }
    public void onSendLognButton() // Клик логин на форме входа
    {
        UpdateDataBase();
        StartCoroutine("passCheck");
    }
    public void onGoogleLogin()
    {
        Debug.Log("авторизация гугл");
    }
    public void onVKLogin()
    {
        Debug.Log("авторизация вк");
    }
    public void onFBLogin()
    {
        Debug.Log("авторизация фейсбук");
    }
    public void onRegisterButton() // Кнопка регистрации
    {
        Debug.Log("запрос на регистрацию");
        UpdateDataBase();
        StartCoroutine(RegisterUser());
    }
    public void toRegisterPanel() // Переход с логина на регистрацию
    {
        enter_panel.gameObject.SetActive(false);
        register_panel.gameObject.SetActive(true);
    }
    public void onFromProfileToClubMenuButton() // переход с профиля на страницу меню
    {
        StartCoroutine(updateGame());
        profile_menu.gameObject.SetActive(false);
        menu_club_panel.gameObject.SetActive(true);
    }
    public void onEnterClub() // Заходим на страницу Клуб
    {
        searth_club_field.text = "";
        menu_club_panel.gameObject.SetActive(false);
        menu_main_club_panel.gameObject.SetActive(true);
        menu_main_club_panel.GetComponent<mainClubPage>().back_page = menu_club_panel.gameObject;
        menu_main_club_panel.GetComponent<mainClubPage>().update_club_db();
    }
    public void onCreateClubButton() // Переход на страницу создать клуб
    {
        
        profile_menu.gameObject.SetActive(false);
        menu_main_club_panel.gameObject.SetActive(false);
        menu_club_panel.gameObject.SetActive(false);
        
        StartCoroutine(updateGame());
        menu_create_club_panel.gameObject.SetActive(true);
        menu_create_club_panel.GetComponent<createClubPage>().back_page = menu_club_panel.gameObject;
    }
    IEnumerator passCheck()
    {
        yield return new WaitForSeconds(0.3f);
        foreach (var sn in snapShot.Children) // Ищем
        {
            gameUser tmpObj = JsonUtility.FromJson<gameUser>(sn.GetRawJsonValue());
            if (nameField.text == tmpObj.name)
            {
                namecheck = true;
                Debug.Log("Login OK! namecheck = " + namecheck);
                if (passField.text == tmpObj.password)
                {
                    Debug.Log("ok " + tmpObj.last_day_online);
                    passcheck = true;
                    user = tmpObj;
                    user.last_day_online = System.DateTime.Now.ToString();
                    user.status = "онлайн";
                    Debug.Log("password OK! passcheck = " + passcheck);

                    string jsonUser = JsonUtility.ToJson(user);
                    dbRef.Child(sn.Key).SetRawJsonValueAsync(jsonUser);
                    break;
                }
            }
        }
        if (passcheck)
        {
            loginErrorText.text = "";
            passErrorText.text = "вход выполнен";
            
            namecheck = false;
            passcheck = false;

            playerCheck = "login";

            PlayerPrefs.DeleteAll();

            PlayerPrefs.SetString("playerLogin", playerCheck);
            PlayerPrefs.SetString("loginName", user.name);
            PlayerPrefs.SetString("password", user.password);
            PlayerPrefs.SetString("userHach", user.id);
            PlayerPrefs.SetString("userID", user.id);
            PlayerPrefs.SetFloat("version", gameVersion);
            PlayerPrefs.Save();

            yield return new WaitForSeconds(0.1f);

            enter_panel.gameObject.SetActive(false);

            chips_count_label.text = user.fishki.ToString();
            menu_club_panel.gameObject.SetActive(true);
        }
        else
        {
            loginErrorText.text = "неверный логин";
            passErrorText.text = "неверный пароль";
        }
    }
    IEnumerator updateGame() // обновляем БД игры
    {
        UpdateDataBase();
        yield return new WaitForSeconds(0.3f);
        if (snapShot != null)
            foreach (var sn in snapShot.Children) // Ищем
            {
                gameUser tmpObj = JsonUtility.FromJson<gameUser>(sn.GetRawJsonValue());
                if (PlayerPrefs.GetString("loginName") == tmpObj.name)
                {
                    if (PlayerPrefs.GetString("password") == tmpObj.password)
                    {
                        Debug.Log("pass ok");
                        user = tmpObj;
                        
                        fishki = user.fishki.ToString();
                        player_name_label.text = user.name;
                        player_id_label.text = sn.Key;
                        user.status = "online";
                        user.last_day_online = System.DateTime.Now.ToString();
                        dbRef.Child(sn.Key).SetRawJsonValueAsync(JsonUtility.ToJson(user));
                        Debug.Log("data updated");
                        break;
                    }
                }
            }
        else 
            StartCoroutine(updateGame());
        chips_count_label.text = fishki;
        coins_count_label.text = user.coins.ToString();
        diamond_count_label.text = user.gems.ToString();
    }
    IEnumerator RegisterUser()
    {
        register_panel.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        if (snapShot != null)
        {
            if (logField.text.Length > 0 && passRegField.text.Length > 0)
            {
                gameUser regUser = new gameUser();
                int c = (int)snapShot.ChildrenCount;
                
                regUser.email = passRegField.text;
                regUser.name = logField.text;
                regUser.password = "123456";
                regUser.coins = 100;
                regUser.last_day_online = System.DateTime.Now.ToString();
                regUser.id = regUser.name + regUser.email;
                regUser.status = "зарегистрирован";

                var idGen = MD5.Create();
                string genStr = helperGame.GetMd5Hash(idGen, regUser.id.ToString());
                regUser.id = genStr;

                dbRef.Child(regUser.id).SetRawJsonValueAsync(JsonUtility.ToJson(regUser));
                enter_panel.gameObject.SetActive(true);
                Debug.Log("регистрация прошла успешно");
            }
            else
            {
                loginErrorText.text = "неверный логин";
                passErrorText.text = "неверный пароль";
                register_panel.gameObject.SetActive(true);
            }
        }
        else
        {
            UpdateDataBase();
            StartCoroutine(RegisterUser());
        }
    }
    IEnumerator openClub(string club_id)
    {
        UpdateClubBase();
        yield return new WaitForSeconds(0.5f);
        foreach (var club in snapShotClub.Children)
        {
            if (club.Child("clubId").Value.ToString() == club_id)
            {
                player_club_name = club.Child("clubName").Value.ToString();
            }
            Debug.Log("id club: " + club.Key);
        }
        chips_count_label.text = fishki;
        if (menu_main_club_panel.gameObject.activeSelf)
            player_club_name_label.text = player_club_name;
    }
    IEnumerator show_logo()
    {
        UpdateDataBase();
        yield return new WaitForSeconds(2f);
        logo_panel.gameObject.SetActive(false);
        enter_panel.gameObject.SetActive(true);
    }
    public void playCustomGame() // Запустить одиночную игру
    {
        menu_club_panel.gameObject.SetActive(false);
        SceneManager.LoadScene(1);
    }
    void OnDestroy()
    {
        
        dbRef.ValueChanged -= onDBChange;
        dbRef.ChildChanged -= dbRef_ChildChanged;
        dbClubs.ChildChanged -= dbClubs_ChildChanged;
        dbClubs.ValueChanged -= onDBClubChildChange;
    }
    void OnApplicationQuit()
    {
        if (playerCheck == "login")
            foreach (var sn in snapShot.Children) // Ищем
            {
                gameUser tmpObj = JsonUtility.FromJson<gameUser>(sn.GetRawJsonValue());
                if (PlayerPrefs.GetString("loginName") == tmpObj.name)
                {
                    if (PlayerPrefs.GetString("password") == tmpObj.password)
                    {
                        user = tmpObj;
                        user.last_day_online = System.DateTime.Now.ToString();
                        user.status = "offline";
                        dbRef.Child(sn.Key).SetRawJsonValueAsync(JsonUtility.ToJson(user));
                        break;
                    }
                }
            }
    }
    void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            foreach (var sn in snapShot.Children) // Ищем
            {
                gameUser tmpObj = JsonUtility.FromJson<gameUser>(sn.GetRawJsonValue());
                if (PlayerPrefs.GetString("loginName") == tmpObj.name)
                {
                    if (PlayerPrefs.GetString("password") == tmpObj.password)
                    {
                        user = tmpObj;
                        user.last_day_online = System.DateTime.Now.ToString();
                        user.status = "offline";
                        dbRef.Child(sn.Key).SetRawJsonValueAsync(JsonUtility.ToJson(user));
                        break;
                    }
                }
            }
        }
    }
}

