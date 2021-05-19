using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.EventSystems;
public class gameSceneManager : MonoBehaviour
{
    gameUser user;
    table tableOnline;
    table default_table;
    public GameObject player_prefab;
    public GameObject card_prefab;
    public Transform card_parent;

    GameObject game_menu;
    GameObject hand_panel;
    GameObject hand_lod_panel;
    GameObject left_player_panel;
    GameObject center_player_panel;
    GameObject right_player_panel;
    GameObject table_panel;
    GameObject cards;
    GameObject player_list;
    
    [SerializeField]  GameObject[] card_list;

    List<Transform> playerS = new List<Transform>();
    List<Transform> playerTabels = new List<Transform>();
    List<Transform> tablePoints = new List<Transform>();
    public Transform[] tableLods;
    public tablePlayer[] tablePlayers;

    GameObject playerSelect_menu;
    GameObject gameOver_menu;
    GameObject who_play_panel;

    GameObject leftP_image;
    GameObject centerP_image;
    GameObject rigthP_image;

    GameObject left_info_panel;
    GameObject center_info_panel;
    GameObject right_info_panel;

    GameObject canvas;
    GameObject lod_fade_panel;

    GameObject button_play_online_game;

    Transform table_left, table_right;

    Text player_list_text;
    Text player_who_wake_text;
    Text player_maessage;

    Vector3 playerPos;

    List<GameObject> playerCards = new List<GameObject>();

    DataSnapshot[] gamePlayerSnap;

    TouchScreenKeyboard oldBoard;

    private string dbLink = "https://testfile-482fe.firebaseio.com/";
    private DatabaseReference usersDB;
    private DatabaseReference tableDB;
    private DataSnapshot snapShotClub = null;
    private DataSnapshot tableSnapshot = null;

    string[] months; 
    static string[] flovers = new string[] { "Мацу", "Умэ", "Сакура", "Фудзи", "Аямэ", "Ботан", "Хаги", "Сусуки", "Кику", "Момидзи", "Янаги", "Кири"};
    string[] groups = new string[] { "Касу", "Танзаку", "Танэ"};

    float startTime = 0;
    
    public static int numPlayers = 2;
    public static int whoPlay = 0;
    public static string whoPlayID = "";
    int who_play_first = 0;
    int player_how_turn = 0;
    string current_table;

    bool gamePaused;
    public static bool is_card_move;
    public static bool isRazdacha;

    public void backToMenu_button()
    {
        clearTable();
        SceneManager.LoadScene(0);
    }
    public void showHideGameMenu()
    {
        game_menu.gameObject.SetActive(!game_menu.gameObject.activeSelf);
    }
    void Start()
    {
        months = new string[]{ "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" };
        cards = GameObject.Find("card_list");

        playerSelect_menu = GameObject.Find("playerSelect_menu");
        gameOver_menu = GameObject.Find("gameOver_menu");
        player_list = playerSelect_menu.transform.Find("players_list").gameObject;
        player_list_text = playerSelect_menu.transform.Find("Text").GetComponent<Text>();

        who_play_panel = GameObject.Find("who_play_panel");
        player_who_wake_text = who_play_panel.transform.Find("Text").GetComponent<Text>();

        player_maessage = GameObject.Find("player_message").GetComponent<Text>();

        game_menu = GameObject.Find("game_menu");
        hand_panel = GameObject.Find("hand_panel");
        left_player_panel = GameObject.Find("left_player_panel");
        center_player_panel = GameObject.Find("center_player_panel");
        right_player_panel = GameObject.Find("right_player_panel");
        table_panel = GameObject.Find("table_panel");

        table_left = table_panel.transform.Find("left_panel").transform;
        table_right = table_panel.transform.Find("right_panel").transform;

        leftP_image = GameObject.Find("left_player_image");
        centerP_image = GameObject.Find("center_player_image");
        rigthP_image = GameObject.Find("right_player_image");

        left_info_panel = GameObject.Find("left_player_info_panel");
        center_info_panel = GameObject.Find("center_player_info_panel");
        right_info_panel = GameObject.Find("right_player_info_panel");

        playerS.AddRange(new Transform[] { hand_panel.transform, left_player_panel.transform, center_player_panel.transform, right_player_panel.transform });

        hand_lod_panel = GameObject.Find("hand_lod_panel");
        
        tableLods = new Transform[3];
        tableLods[0] = GameObject.Find("left_player_lod_panel").transform;
        tableLods[1] = GameObject.Find("center_player_lod_panel").transform;
        tableLods[2] = GameObject.Find("right_player_lod_panel").transform;

        button_play_online_game = playerSelect_menu.transform.Find("button_start_game").gameObject;

        GameObject[] objects = GameObject.FindGameObjectsWithTag("table_positions");

        for (int i = 0; i < objects.Length; i++)
            tablePoints.Add(objects[i].transform);

        canvas = GameObject.Find("Canvas");
        lod_fade_panel = GameObject.Find("lod_fade_panel");

        playerPos = Camera.main.ViewportToWorldPoint(new Vector3(0.8f, 0.2f, 0));
        
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(dbLink);
        tableDB = FirebaseDatabase.DefaultInstance.GetReference("tables");
        usersDB = FirebaseDatabase.DefaultInstance.GetReference("users");

        tableDB.ChildChanged += tableDB_ChildChanged;

        default_table = new table();
        default_table.table_name = "Временный стол";
        default_table.table_type = "temp";
        default_table.game_status = "wait";
        default_table.table_id = PlayerPrefs.GetString("userID");
        default_table.table_parent = PlayerPrefs.GetString("loginName");

        player_maessage.text = "";

        clearTable();
        StartCoroutine(dataUpdated());
        gamePaused = true;
        isRazdacha = false;

        left_player_panel.gameObject.SetActive(false);
        center_player_panel.gameObject.SetActive(false);
        right_player_panel.gameObject.SetActive(false);
        leftP_image.gameObject.SetActive(false);
        centerP_image.gameObject.SetActive(false);
        rigthP_image.gameObject.SetActive(false);
        left_info_panel.gameObject.SetActive(false);
        center_info_panel.gameObject.SetActive(false);
        right_info_panel.gameObject.SetActive(false);

        game_menu.gameObject.SetActive(false);
        button_play_online_game.gameObject.SetActive(false);
        who_play_panel.gameObject.SetActive(false);
        lod_fade_panel.SetActive(false);

        gameOver_menu.SetActive(false);
    }
    
    void updatePlayerDB()
    {
        usersDB.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                snapShotClub = task.Result;
                return;
            }
        });
    }
    void update_tableDB()
    {
        tableDB.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                tableSnapshot = task.Result;
                return;
            }
        });
    }
    //
    // Инициализация игроков
    //                          ---------------------------------------------------------------------------------------------------------------------------------------------------
    void InitializePlayers() // ---------------------------------------------------------------------------------------------------------------------------------------------------
    {
        clearTable();
        playerS.Clear();
        Transform firstPlayer;
        Transform twoPlayer;
        tablePlayer[] gamePlayers;
        whoPlayID = tableSnapshot.Child(current_table).Child("whoPlay").Value.ToString();
        tablePlayers = new tablePlayer[numPlayers];
        gamePlayers = new tablePlayer[numPlayers - 1];
        playerTabels.Clear();

        firstPlayer = hand_panel.transform;
        twoPlayer = left_player_panel.transform;
        int c = 0;
        int tpp = 0;
        foreach (var onPlayer in tableSnapshot.Child(current_table).Child("players").Children)
        {
            var TP = JsonUtility.FromJson<tablePlayer>(onPlayer.GetRawJsonValue());
            tablePlayers[c] = TP;
            if (TP.playerID != PlayerPrefs.GetString("userID"))
            {
                gamePlayers[tpp] = TP;
                tpp++;
            }
            c++;
        }
        // Тут запоминаем пользователя и сдвигаем налево (для каждого)
        bool polo = false;
        playerS.AddRange(new Transform[] { twoPlayer, center_player_panel.transform, right_player_panel.transform, twoPlayer, center_player_panel.transform, right_player_panel.transform });
        for (int i = 0; i < numPlayers; i++)
        {
            if (tablePlayers[i].playerID == PlayerPrefs.GetString("userID"))
            {
                polo = true;
                playerTabels.Add(firstPlayer);
            }
            else
            {
                if (polo)
                    playerTabels.Add(playerS[i - 1]);
                else playerTabels.Add(playerS[i]);
            }
        }
        playerTabels.Add(table_left);
        playerTabels.Add(table_right);

        switch (numPlayers)
        {
            case 2:
                {
                    center_player_panel.gameObject.SetActive(false);
                    right_player_panel.gameObject.SetActive(false);
                    centerP_image.gameObject.SetActive(false);
                    rigthP_image.gameObject.SetActive(false);
                    center_info_panel.gameObject.SetActive(false);
                    right_info_panel.gameObject.SetActive(false);

                    left_info_panel.gameObject.SetActive(true);
                    left_player_panel.gameObject.SetActive(true);
                    leftP_image.gameObject.SetActive(true);

                    left_player_panel.GetComponent<RectTransform>().anchorMin = new Vector2(0.06f, 0.89f);
                    left_player_panel.GetComponent<RectTransform>().anchorMax = new Vector2(0.98f, 0.98f);
                    
                    left_info_panel.transform.Find("Text").GetComponent<Text>().text = gamePlayers[0].playerName;
                    break;
                }
            case 3:
                {
                    right_player_panel.gameObject.SetActive(false);
                    rigthP_image.gameObject.SetActive(false);
                    right_info_panel.gameObject.SetActive(false);

                    left_info_panel.gameObject.SetActive(true);
                    left_player_panel.gameObject.SetActive(true);
                    leftP_image.gameObject.SetActive(true);

                    center_player_panel.gameObject.SetActive(true);
                    centerP_image.gameObject.SetActive(true);
                    center_info_panel.gameObject.SetActive(true);

                    left_player_panel.GetComponent<RectTransform>().anchorMin = new Vector2(0.06f, 0.89f);
                    left_player_panel.GetComponent<RectTransform>().anchorMax = new Vector2(0.49f, 0.98f);

                    centerP_image.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.89f);
                    centerP_image.GetComponent<RectTransform>().anchorMax = new Vector2(0.545f, 0.98f);

                    center_player_panel.GetComponent<RectTransform>().anchorMin = new Vector2(0.55f, 0.89f);
                    center_player_panel.GetComponent<RectTransform>().anchorMax = new Vector2(0.98f, 0.98f);

                    center_info_panel.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.78f);
                    center_info_panel.GetComponent<RectTransform>().anchorMax = new Vector2(0.61f, 0.89f);
                    
                    left_info_panel.transform.Find("Text").GetComponent<Text>().text = gamePlayers[0].playerName;
                    center_info_panel.transform.Find("Text").GetComponent<Text>().text = gamePlayers[1].playerName;

                    break;
                }
            case 4:
                {
                    left_info_panel.gameObject.SetActive(true);
                    left_player_panel.gameObject.SetActive(true);
                    leftP_image.gameObject.SetActive(true);

                    center_player_panel.gameObject.SetActive(true);
                    centerP_image.gameObject.SetActive(true);
                    center_info_panel.gameObject.SetActive(true);

                    right_player_panel.gameObject.SetActive(true);
                    rigthP_image.gameObject.SetActive(true);
                    right_info_panel.gameObject.SetActive(true);

                    left_info_panel.transform.Find("Text").GetComponent<Text>().text = gamePlayers[0].playerName;
                    center_info_panel.transform.Find("Text").GetComponent<Text>().text = gamePlayers[1].playerName;
                    right_info_panel.transform.Find("Text").GetComponent<Text>().text = gamePlayers[2].playerName;

                    break;
                }
            case 5:
                {
                    break;
                }
            default:
                break;
        }
    }
    // Нажимаем на кнопку выбор игроков
    public void onSelectPlayersClick()
    {
        peretasovka();
        tableDB.Child(current_table).Child("game_status").SetValueAsync("start");
        Debug.Log("Создатель начал игру");
    }
    void tableDB_ChildChanged(object sender, ChildChangedEventArgs e) // Изменения в таблице
    {
        updatePlayerDB();
        update_tableDB();
        StartCoroutine(check_change_table(e.Snapshot));
    }
    //                  ##                              ##
    // ИГРА            ####################################
    //                  ##                              ##
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !Input.GetKeyUp(KeyCode.Escape))
        {
            showHideGameMenu();
        }
        if (!gamePaused)
        {
            foreach (GameObject card in playerCards)
            {
                // получаем ключ о движении карты
                if (card.GetComponent<cardScript>().check_move)
                {
                    Debug.Log("это карта которую нажали");
                    card.GetComponent<cardScript>().check_move = false;
                    if (whoPlayID == PlayerPrefs.GetString("userID") && player_how_turn < 2)
                    {
                        if (card.transform.parent == cards.transform) // Если взял из колоды
                        {
                            bool allCardsOK = false;
                            foreach (GameObject cardInList in playerCards)
                            {
                                if (cardInList.transform.parent == table_left || cardInList.transform.parent == table_right) // ищем карту на столе
                                {
                                    if (card.GetComponent<cardScript>().month == cardInList.GetComponent<cardScript>().month)
                                    {
                                        StartCoroutine(show_my_message(card.GetComponent<cardScript>().month + " на стол. осталось: " + cards.transform.GetComponentInChildren<Transform>().childCount));
                                        card.GetComponent<cardScript>().open = true;
                                        StartCoroutine(update_game_card(card, cardInList, 0.4f));
                                        //StartCoroutine(nextWhoPlay());
                                        allCardsOK = true;
                                        player_how_turn++;
                                        break;
                                    }
                                }
                            }
                            if (!allCardsOK) // если нет совпадений но карты есть то кидаем на стол
                            {
                                StartCoroutine(show_my_message("нет совпадений. сл ход"));
                                Transform tr;
                                if (table_left.transform.GetComponentInChildren<Transform>().childCount >= table_right.transform.GetComponentInChildren<Transform>().childCount)
                                    tr = table_right;
                                else tr = table_left;
                                card.GetComponent<cardScript>().open = true;
                                StartCoroutine(update_game_card(card, tr.gameObject, 0.4f));
                                //StartCoroutine(nextWhoPlay());
                                player_how_turn++;
                            }
                            
                            break;
                        }
                        else
                        {
                            // Если из руки
                            bool cardOK = false;
                            foreach (GameObject cardintable in playerCards)
                            {
                                if (cardintable.transform.parent == table_left || cardintable.transform.parent == table_right) // на столе
                                {
                                    if (card.GetComponent<cardScript>().month == cardintable.GetComponent<cardScript>().month)
                                    {
                                        StartCoroutine(show_my_message("карта " + card.GetComponent<cardScript>().month));
                                        StartCoroutine(update_game_card(card, cardintable, 0.1f));
                                        //StartCoroutine(nextWhoPlay());
                                        cardOK = true;
                                        player_how_turn++;
                                        break;
                                    }
                                }
                            }
                            if (!cardOK) // Если не совпало то на стол
                            {
                                StartCoroutine(show_my_message("нет совпадений. сл ход"));
                                Transform tr;
                                if (table_left.transform.GetComponentInChildren<Transform>().childCount >= table_right.transform.GetComponentInChildren<Transform>().childCount)
                                    tr = table_right;
                                else tr = table_left;
                                card.GetComponent<cardScript>().open = true;
                                StartCoroutine(update_game_card(card, tr.gameObject, 0.2f));
                                //StartCoroutine(nextWhoPlay());
                                player_how_turn++;
                            }
                        }
                        break;
                    }
                    else Debug.Log("это не твоя карта");
                }
                if (card.transform.parent == hand_panel.transform)
                {
                    if (whoPlayID != PlayerPrefs.GetString("userID"))
                    {
                        card.transform.Find("card_image").GetComponent<Image>().color = new Color(120, 120, 120, 255);
                    }
                    else card.transform.Find("card_image").GetComponent<Image>().color = new Color(255, 255, 255, 255);
                }
            }
            if (cards.transform.GetComponentInChildren<Transform>().childCount < 1 || hand_panel.transform.GetComponentInChildren<Transform>().childCount < 2)
            {
                StartCoroutine(show_my_message("Колода закончилась"));
                tableDB.Child(current_table).Child("game_status").SetValueAsync("stop");
                tableDB.Child(current_table).Child("cards").RemoveValueAsync();
                gamePaused = true;
            }
            if (player_how_turn >= 2 && whoPlayID == PlayerPrefs.GetString("userID"))
            {
                player_how_turn = 0;
                StartCoroutine(nextWhoPlay());
            }
        }
    }
    //
    // Карту к ###############################################################################################################################################################################################
    //
    IEnumerator update_game_card(GameObject card, GameObject cardInList, float z) // Записываем карту
    {
        Debug.Log("обновляю карту готовлю к движению к " + cardInList.name);
        RectTransform card_rect = card.GetComponent<RectTransform>();
        DataSnapshot cardsnapshot = tableSnapshot.Child(current_table).Child("cards").Child(card.GetComponent<cardScript>().pKey);
        onlineCard onCard = JsonUtility.FromJson<onlineCard>(cardsnapshot.GetRawJsonValue());
        var sizeDelta = 0.14f * table_panel.transform.GetComponent<RectTransform>().rect.width / canvas.GetComponent<Canvas>().scaleFactor;

        card.GetComponent<cardScript>().target_position = cardInList.transform.position;
        card.GetComponent<cardScript>().current_parent = cardInList.transform;

        card_rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeDelta);
        onCard.state = "move";
        if (cardInList.tag != "table")
        {
            onCard.targetCard = cardInList.GetComponent<cardScript>().num.ToString();
            onCard.player_id = PlayerPrefs.GetString("userID");
            card.GetComponent<cardScript>().target_card = cardInList;
            card.GetComponent<cardScript>().target_lod = hand_lod_panel.transform;
        }
        else
        {
            onCard.targetCard = "";
            card.GetComponent<cardScript>().target_card = null;
            card.GetComponent<cardScript>().target_lod = null;
        }
        onCard.open = card.GetComponent<cardScript>().open;
        onCard.targetX = cardInList.transform.position.x;
        onCard.targetY = cardInList.transform.position.y;
        onCard.targetZ = cardInList.transform.position.z;

        Debug.Log("кидаю карту в бд");
        tableDB.Child(current_table).Child("cards").Child(cardsnapshot.Key).SetRawJsonValueAsync(JsonUtility.ToJson(onCard));

        yield return new WaitForSeconds(z);
        sizeDelta = 0.1f * table_panel.transform.GetComponent<RectTransform>().rect.width / canvas.GetComponent<Canvas>().scaleFactor;
        card_rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeDelta);
        card.GetComponent<cardScript>().move = true;
        is_card_move = true;
        card.GetComponent<cardScript>().play_card_sound("move");
    }
    void clearTable()
    {
        card_list = GameObject.FindGameObjectsWithTag("card");

        foreach (var c in card_list)
        {
            Destroy(c);
        }
        
    }
    void resetGame() 
    {
        clearTable();
        playerCards.Clear();
        float posX = 0, posY = 0, posZ = 0;
        int cardsCount = numPlayers > 2 ? 52 : 48;
        int mouthCount = 0;
        int cardType = 0;
        for (int i = 0; i < cardsCount; i++)
        {
            GameObject card = Instantiate(card_prefab, cards.transform.position, Quaternion.identity);

            card.GetComponent<cardScript>().cardImage = Resources.Load<Sprite>("cards/pae" + i);
            card.GetComponent<cardScript>().default_parent = cards.transform;
            helperGame.getCardScore(ref card, mouthCount, cardType);
            if (cardType > 4)
            {
                cardType = 0;
                mouthCount++;
            }
            card.GetComponent<cardScript>().type = cardType;
            card.GetComponent<cardScript>().month = months[mouthCount];
            card.GetComponent<cardScript>().pKey = i.ToString();
            
            card.transform.Find("Text").GetComponent<Text>().text = (i + 1).ToString();
            card.transform.position = Vector3.zero;
            card.transform.SetParent(cards.transform);

            RectTransform rect = card.GetComponent<RectTransform>();

            posX += 0.002f;
            posY += 0.002f;
            posZ += 0.001f;

            var sizeDelta = 0.1f * table_panel.transform.GetComponent<RectTransform>().rect.width / canvas.GetComponent<Canvas>().scaleFactor;
            rect.localScale = new Vector3(1f, 1f, 1f);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeDelta);
            rect.position = new Vector3(cards.transform.position.x + posX, cards.transform.position.y + posY, cards.transform.position.z - posZ);

            playerCards.Add(card);
            cardType++;
        }
        //StartCoroutine(peretasovka());
    }
    IEnumerator back_to_list()
    {
        yield return new WaitForSeconds(1.3f);
        foreach (var card in playerCards)
        {
            if (card.transform.parent != cards.transform)
            {
                card.GetComponent<cardScript>().target_position = cards.transform.position;
                card.transform.SetParent(cards.transform);
                var sizeDelta = 0.3f * table_panel.transform.GetComponent<RectTransform>().rect.height / canvas.GetComponent<Canvas>().scaleFactor;
                card.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeDelta);
                card.GetComponent<cardScript>().moveToList = true;
            }
            yield return new WaitForSeconds(0.08f);
        }
        peretasovka();
    }
    void peretasovka()
    {
        float posX = 0, posY = 0, posZ = 0;
        update_tableDB();
        resetGame();
        for (int i = 0; i < playerCards.Count; i++)
        {
            int j = Random.Range(i, playerCards.Count);
            var tmp = playerCards[i];

            playerCards[i] = playerCards[j];
            playerCards[j] = tmp;
        }
        string mass = "[";
        for (int i = 0; i < playerCards.Count; i++)
        {
            var obj = playerCards[i];

            obj.transform.position = new Vector3(cards.transform.position.x + posX, cards.transform.position.y + posY, cards.transform.position.z - posZ);
            obj.GetComponent<SortingGroup>().sortingOrder = i;
            posX += 0.002f;
            posY += 0.002f;
            posZ += 0.005f;
            obj.transform.SetAsLastSibling();
            var c = new onlineCard();
            c.num = int.Parse(obj.transform.Find("Text").GetComponent<Text>().text);
            c.online_key = i.ToString();
            c.posX = obj.transform.position.x;
            c.posY = obj.transform.position.y;
            c.posZ = obj.transform.position.z;
            c.image_path = obj.GetComponent<cardScript>().cardImage.name;
            c.month = obj.GetComponent<cardScript>().month;
            c.type = obj.GetComponent<cardScript>().type;
            c.score = obj.GetComponent<cardScript>().score;
            mass += JsonUtility.ToJson(c) + ",";
        }
        mass += "]";
        // Кидаем на стол перетасованную колоду mass[]
        
        tableDB.Child(current_table).Child("cards").SetRawJsonValueAsync(mass);
        clearTable();
    }
    IEnumerator razdacha()
    {
        isRazdacha = true;
        update_tableDB();
        StartCoroutine(show_my_message("начинаю раздачу"));
        int forPlayer = 0;
        int forTableCount = 4;
        int playerC = 0;
        int gCircle = 0;
        clearTable();
        yield return new WaitForSeconds(1.0f);
        List<onlineCard> oCards = new List<onlineCard>();
        foreach (var card in tableSnapshot.Child(current_table).Child("cards").Children)
        {
            onlineCard oCard = JsonUtility.FromJson<onlineCard>(card.GetRawJsonValue());
            oCards.Add(oCard);
        }
        playerCards.Clear();
        for (int i = 0; i < oCards.Count; i++)
        {
            GameObject card = Instantiate(card_prefab, cards.transform.position, Quaternion.identity);
            RectTransform rect = card.GetComponent<RectTransform>();

            card.transform.position = new Vector3(oCards[i].posX, oCards[i].posY, oCards[i].posZ);
            //card.transform.position = Vector3.zero;
            card.GetComponent<cardScript>().month = oCards[i].month;
            card.GetComponent<cardScript>().type = oCards[i].type;
            card.GetComponent<cardScript>().default_parent = cards.transform;
            card.GetComponent<cardScript>().num = oCards[i].num;
            card.GetComponent<cardScript>().pKey = oCards[i].online_key;
            card.GetComponent<cardScript>().score = oCards[i].score;
            
            card.transform.Find("Text").GetComponent<Text>().text = oCards[i].num.ToString();
            card.GetComponent<cardScript>().cardImage = Resources.Load<Sprite>("cards/" + oCards[i].image_path);
            card.transform.SetParent(cards.transform);
            card.transform.SetAsLastSibling();

            var sizeDelta = 0.1f * table_panel.transform.GetComponent<RectTransform>().rect.width / canvas.GetComponent<Canvas>().scaleFactor;
            rect.localScale = new Vector3(1f, 1f, 1f);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeDelta);
            rect.position = new Vector3(oCards[i].posX, oCards[i].posY, oCards[i].posZ);
            
            playerCards.Add(card);
        }
        for (int i = playerCards.Count - 1; i >= 0; i--)
        {
            GameObject obj = playerCards[i];
            Vector3 target_pos = Vector3.zero;

            yield return new WaitForSeconds(0.15f);

            if (playerC == playerTabels.Count - 2) forTableCount = 4;
            else if (playerC == playerTabels.Count - 1) forTableCount = 4;
            else forTableCount = 5; // По сколько карт кидать
            if (forPlayer >= forTableCount)
            {
                forPlayer = 0;
                playerC++;
            }
            if (playerC >= playerTabels.Count)
            {
                playerC = 0;
                gCircle++;
                if (gCircle == 1) break;
            }
            target_pos = playerTabels[playerC].transform.position;
            
            obj.GetComponent<cardScript>().target_position = target_pos;
            obj.GetComponent<cardScript>().current_parent = playerTabels[playerC];
            obj.GetComponent<cardScript>().move = true;
            obj.GetComponent<cardScript>().play_card_sound("move");
            forPlayer++;
        }
        //
        // Устанавливаем карте id владельца
        //
        List<onlineCard> tmpCards = new List<onlineCard>();
        foreach (GameObject card in playerCards)
        {
            if (card.transform.parent == hand_panel.transform)
            {
                foreach (DataSnapshot onlineTable in tableSnapshot.Child(current_table).Child("cards").Children)
                {
                    if (card.transform.Find("Text").GetComponent<Text>().text == onlineTable.Child("num").Value.ToString())
                    {
                        onlineCard onlineC = JsonUtility.FromJson<onlineCard>(onlineTable.GetRawJsonValue());
                        onlineC.player_id = PlayerPrefs.GetString("userID");
                        card.GetComponent<cardScript>().player_id = onlineC.player_id;
                        onlineC.online_key = onlineTable.Key;
                        tmpCards.Add(onlineC);
                    }
                }
            }
        }
        // передаем своим картам свой id
        foreach (onlineCard card in tmpCards)
        {
            tableDB.Child(current_table).Child("cards").Child(card.online_key).SetRawJsonValueAsync(JsonUtility.ToJson(card));
        }
        // 
        // Уточняем кто раздает
        //
        if (PlayerPrefs.GetString("userID") == current_table)
        {
            update_game_player_snapshot(tableSnapshot);
            tableDB.Child(current_table).Child("whoPlay").SetValueAsync(gamePlayerSnap[whoPlay].Child("playerID").Value.ToString());
            //tableDB.Child(current_table).Child("players").Child(gamePlayerSnap[whoPlay].Key).Child("status").SetValueAsync("walk");
        }
        gamePaused = false;
        isRazdacha = false;
    }
    
    IEnumerator dataUpdated()
    {
        updatePlayerDB();
        yield return new WaitForSeconds(0.2f);
        if (snapShotClub != null)
        {
            foreach (var sn in snapShotClub.Children)
            {
                gameUser tmpUser = JsonUtility.FromJson<gameUser>(sn.GetRawJsonValue());
                if (PlayerPrefs.GetString("loginName") == tmpUser.name)
                {
                    if (PlayerPrefs.GetString("password") == tmpUser.password)
                    {
                        Debug.Log("pass ok");
                        tableOnline = new table();
                        user = tmpUser;
                        user.status = "enter";
                        usersDB.Child(sn.Key).SetRawJsonValueAsync(JsonUtility.ToJson(user));
                        StartCoroutine(tablesUpdated());
                        break;
                    }
                }
            }
        }
        else StartCoroutine(dataUpdated());
    }
    //
    // входим в стол
    //
    IEnumerator tablesUpdated() 
    {
        update_tableDB();
        
        table tmpTable = new table();
        tablePlayer tableP = new tablePlayer();
        tableP.playerID = PlayerPrefs.GetString("userID");
        tableP.playerName = PlayerPrefs.GetString("loginName");
        tableP.status = "wait";
        StartCoroutine(show_my_message("входим в лобби"));
        yield return new WaitForSeconds(0.5f);
        if (tableSnapshot != null)
        {
            if ((int)tableSnapshot.ChildrenCount > 0)
            {
                bool sch = false;
                foreach (var t in tableSnapshot.Children)
                {
                    tmpTable = JsonUtility.FromJson<table>(t.GetRawJsonValue());
                    if ((int)t.Child("players").ChildrenCount < 4 && tmpTable.game_status == "wait")
                    {
                        tableDB.Child(t.Key).Child("players").Child(tableP.playerID).SetRawJsonValueAsync(JsonUtility.ToJson(tableP));
                        //tableDB.Child(t.Key).Child("players").Child(t.Child("players").ChildrenCount.ToString()).ValueChanged += onTablePlayersChanche;
                        tableOnline = tmpTable;
                        sch = true;
                        break;
                    }
                }
                if (!sch)
                {
                    if (tableSnapshot.Child(tableP.playerID).Key == tableP.playerID)
                    {
                        tableP.playerID = (tableP.playerName + Time.deltaTime).ToString();
                    }
                    tableDB.Child(tableP.playerID).SetRawJsonValueAsync(JsonUtility.ToJson(default_table));
                    tableDB.Child(tableP.playerID).Child("table_id").SetValueAsync(tableP.playerID);
                    tableDB.Child(tableP.playerID).Child("players").Child(tableP.playerID).SetRawJsonValueAsync(JsonUtility.ToJson(tableP));
                    tableOnline = default_table;
                    StartCoroutine(show_my_message("нет свободного стола"));
                }
                
            }
            else
            {
                tableDB.Child(tableP.playerID).SetRawJsonValueAsync(JsonUtility.ToJson(default_table));
                tableDB.Child(tableP.playerID).Child("table_id").SetValueAsync(tableP.playerID);
                tableDB.Child(tableP.playerID).Child("players").Child(tableP.playerID).SetRawJsonValueAsync(JsonUtility.ToJson(tableP));
                tableOnline = default_table;
                StartCoroutine(show_my_message("нет стола. создаем новый"));
            }
            
        }
        current_table = tableOnline.table_id;
        Debug.Log("tables ok");
        //StartCoroutine(update_start_player_list());
    }
    //                                       ##                                                          ##
    // Проверка события на изменение в столе ##############################################################
    //                                       ##                                                          ##
    IEnumerator check_change_table(DataSnapshot tSnap) 
    {
        table tmpTable = JsonUtility.FromJson<table>(tSnap.GetRawJsonValue());
        tablePlayer tmPlayer = JsonUtility.FromJson<tablePlayer>(tSnap.Child("players").Child(PlayerPrefs.GetString("userID")).GetRawJsonValue());
        yield return new WaitForSeconds(0.3f);
        //gamePlayerSnap = new DataSnapshot[(int)tSnap.Child("players").ChildrenCount];
        
        if (tSnap.Key == tableOnline.table_id)
        {
            numPlayers = (int)tSnap.Child("players").ChildrenCount;
            if (tmpTable.game_status == "wait")
            {
                player_list_text.text = "Стол: " + tSnap.Key + " Количество игроков: " + numPlayers;

                update_waight_players(tSnap);

                if (numPlayers > 1 && PlayerPrefs.GetString("userID") == tableOnline.table_id) button_play_online_game.gameObject.SetActive(true);
                else button_play_online_game.gameObject.SetActive(false);
            }
            else if (tmpTable.game_status == "start")
            {
                bool allReady = true;
                foreach (var pl in tSnap.Child("players").Children)
                {
                    if (pl.Child("status").Value.ToString() != "ingame" || pl.Child("status").Value.ToString() == "stop")
                    {
                        allReady = false;
                    }
                }
                if (!allReady)
                    check_players_on_start(tSnap);
                else
                {
                    if (tmpTable.table_id == PlayerPrefs.GetString("userID"))
                        tableDB.Child(current_table).Child("game_status").SetValueAsync("game");
                    InitializePlayers();
                    playerSelect_menu.gameObject.SetActive(false);
                    gameOver_menu.SetActive(false);
                    StartCoroutine(razdacha());
                }
            }
            else if (tmpTable.game_status == "game")
            {
                check_players_in_game(tSnap);
            }
            else if (tmpTable.game_status == "stop" && tmPlayer.status != "stop")
            {
                whoPlayID = "";
                
                List<cardScript> cardsList = new List<cardScript>();
                for (int i = 0; i < hand_lod_panel.GetComponentInChildren<Transform>().childCount - 1; i++)
                {
                    cardsList.Add(hand_lod_panel.GetComponentInChildren<Transform>().GetChild(i).gameObject.GetComponent<cardScript>());
                }
                int myscore = helperGame.getMyScore(cardsList);
                StartCoroutine(show_my_message("игра закончена. счет: " + myscore));
                
                tmPlayer.message = myscore.ToString();
                tmPlayer.status = "stop";

                tableDB.Child(current_table).Child("players").Child(PlayerPrefs.GetString("userID")).SetRawJsonValueAsync(JsonUtility.ToJson(tmPlayer));
                
            }
            else if (tmpTable.game_status == "stop" && tmPlayer.status == "stop")
            {
                tablePlayer offPlayers = new tablePlayer();
                int bestScore = 0;
                bool allOK = true;
                foreach (DataSnapshot playerSnap in tSnap.Child("players").Children)
                {
                    tablePlayer tPlayer = JsonUtility.FromJson<tablePlayer>(playerSnap.GetRawJsonValue());
                    if (tPlayer.message != "")
                    {
                        int tScore = int.Parse(tPlayer.message);
                        if (tScore > bestScore)
                        {
                            bestScore = tScore;
                            offPlayers = tPlayer;
                        }
                    }
                    else allOK = false;
                }
                if (allOK)
                {
                    gameOver_menu.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Игра закончена";
                    gameOver_menu.transform.GetChild(1).gameObject.GetComponent<Text>().text = "Победил <" + offPlayers.playerName + "> счет: " + bestScore + "\r\n Ваш счет: " + tmPlayer.message;

                    if (tmpTable.table_parent == PlayerPrefs.GetString("loginName"))
                    {
                        gameOver_menu.transform.Find("button_start_game").gameObject.SetActive(true);
                        if (tSnap.Child("whoPlay").Value.ToString() != "")
                            tableDB.Child(current_table).Child("whoPlay").SetValueAsync("");
                    }
                    else gameOver_menu.transform.Find("button_start_game").gameObject.SetActive(false);
                }
                gameOver_menu.SetActive(true);
                gamePaused = true;
            }
        }
    }
    //
    // проверка начального экрана игроков перед стартом игры
    //
    void update_waight_players(DataSnapshot player_snapshot) 
    {
        Debug.Log("обновляю список игроков");
        GameObject[] gmb = GameObject.FindGameObjectsWithTag("online_start_player");
        foreach (var p in gmb)
            Destroy(p);
        // обновляем список онлайн игроков
        foreach (var pl in player_snapshot.Child("players").Children)
        {
            tablePlayer tmpPlayer = JsonUtility.FromJson<tablePlayer>(pl.GetRawJsonValue());
            GameObject tmpObj = Instantiate(player_prefab);
            string nameText = tmpPlayer.playerName;

            if (tmpPlayer.playerID == player_snapshot.Child("table_id").Value.ToString()) nameText += " (!)";

            tmpObj.transform.Find("Text").GetComponent<Text>().text = nameText;
            tmpObj.transform.SetParent(player_list.transform);
            tmpObj.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
    //
    // проверка игрока на старте игры
    //
    void check_players_on_start(DataSnapshot player_snapshot) 
    {
        foreach (var pl in player_snapshot.Child("players").Children)
        {
            tablePlayer tmpPlayer = JsonUtility.FromJson<tablePlayer>(pl.GetRawJsonValue());
            if (tmpPlayer.status == "wait" && tmpPlayer.playerID == PlayerPrefs.GetString("userID"))
            {
                tmpPlayer.status = "ingame";
                tableDB.Child(player_snapshot.Key).Child("players").Child(pl.Key).Child("status").SetValueAsync(tmpPlayer.status);
                break;
            }
            else if (tmpPlayer.status == "stop" && tmpPlayer.playerID == PlayerPrefs.GetString("userID"))
            {
                tmpPlayer.status = "ingame";
                tableDB.Child(player_snapshot.Key).Child("players").Child(pl.Key).Child("status").SetValueAsync(tmpPlayer.status);
                break;
            }
        }
    }
    //
    // Проверка игрока в игре
    //
    void check_players_in_game(DataSnapshot player_snapshot) 
    {
        foreach (var pl in player_snapshot.Child("players").Children)
        {
            tablePlayer tmpPlayer = JsonUtility.FromJson<tablePlayer>(pl.GetRawJsonValue());
            if (tmpPlayer.status == "ingame" && tmpPlayer.playerID == PlayerPrefs.GetString("userID")) // Первый заход в игру
            {
                
                tmpPlayer.status = "wait";
                tableDB.Child(player_snapshot.Key).Child("players").Child(pl.Key).Child("status").SetValueAsync(tmpPlayer.status);
                break;
            }
            if (tmpPlayer.status == "walk" && tmpPlayer.playerID == PlayerPrefs.GetString("userID")) // 
            {
                // Делаем ход и устанавливаем следующего
                tmpPlayer.status = "wait";
                tableDB.Child(player_snapshot.Key).Child("players").Child(pl.Key).Child("status").SetValueAsync(tmpPlayer.status);
                break;
            }
        }
        // Показываем кто играет
        if (whoPlayID != player_snapshot.Child("whoPlay").Value.ToString())
        {
            Debug.Log("чекаю кто ходит");
            whoPlayID = player_snapshot.Child("whoPlay").Value.ToString();
            //update_game_player_snapshot(player_snapshot);
            StartCoroutine(game_show_who_play(player_snapshot));
        }
        else
            update_cards_in_game(player_snapshot);
    }
    void update_cards_in_game(DataSnapshot player_snapshot)//проверка карт
    {
        foreach (DataSnapshot cardSnapShot in player_snapshot.Child("cards").Children)
        {
            onlineCard oncard = JsonUtility.FromJson<onlineCard>(cardSnapShot.GetRawJsonValue());

            if (oncard.state == "move" && whoPlayID != PlayerPrefs.GetString("userID"))
            {
                foreach (GameObject gameCard in playerCards)
                {
                    if (gameCard.GetComponent<cardScript>().num == oncard.num && !gameCard.GetComponent<cardScript>().move)
                    {
                        StartCoroutine(update_enemy_card(gameCard, oncard, player_snapshot));
                        break;
                    }
                }
                break;
            }
        }
    }
    //
    // Ловим карты соперника и отправляем по логике
    //
    IEnumerator update_enemy_card(GameObject card, onlineCard oncard, DataSnapshot player_snapshot)
    {
        
        Transform[] tbs = new Transform[] { table_left, table_right };
        GameObject targetT = tbs[Random.Range(0, 1)].gameObject;
        if (table_left.transform.GetComponentInChildren<Transform>().childCount >= table_right.transform.GetComponentInChildren<Transform>().childCount)
        {
            targetT = table_right.gameObject;
        }
        else targetT = table_left.gameObject;
        if (oncard.targetCard != "")
        {
            foreach (GameObject tmpCard in playerCards)
            {
                if (tmpCard.GetComponent<cardScript>().num.ToString() == oncard.targetCard) { targetT = tmpCard; break; }
            }
        }
        for (int i = 0; i < tablePlayers.Length; i++)
        {
            if (tablePlayers[i].playerID == oncard.player_id)
            {
                card.GetComponent<cardScript>().target_lod = tableLods[i];
                break;
            }
        }
        oncard.state = "";
        oncard.targetCard = "";
        tableDB.Child(player_snapshot.Key).Child("cards").Child(oncard.online_key).SetRawJsonValueAsync(JsonUtility.ToJson(oncard));
        card.GetComponent<cardScript>().target_position = targetT.transform.position;
        card.GetComponent<cardScript>().current_parent = targetT.transform;
        card.GetComponent<cardScript>().target_card = targetT;
        card.GetComponent<cardScript>().open = oncard.open;
        yield return new WaitForSeconds(0.4f);
        card.GetComponent<cardScript>().move = true;
        card.GetComponent<cardScript>().play_card_sound("move");
    }
    //
    //Показать кто теперь играет
    IEnumerator game_show_who_play(DataSnapshot playerSnapshot)
    {
        update_tableDB();
        yield return new WaitForSeconds(0.3f);
        who_play_panel.gameObject.SetActive(true);
        string query_name = playerSnapshot.Child("players").Child(whoPlayID).Child("playerName").Value.ToString();
        player_who_wake_text.text = "Играет " + query_name;
        
        yield return new WaitForSeconds(3f);
        who_play_panel.gameObject.SetActive(false);
    }
    IEnumerator show_my_message(string message)
    {
        player_maessage.text = message;
        yield return new WaitForSeconds(3f);
        player_maessage.text = "";
    }
    void OnDestroy()
    {
        tableDB.ChildChanged -= tableDB_ChildChanged;
        Debug.Log("prepare");
        foreach (var sn in snapShotClub.Children)
        {
            gameUser tmpUser = JsonUtility.FromJson<gameUser>(sn.GetRawJsonValue());
            if (PlayerPrefs.GetString("loginName") == tmpUser.name && PlayerPrefs.GetString("password") == tmpUser.password)
            {
                user = tmpUser;
                user.status = "menu";
                usersDB.Child(sn.Key).SetRawJsonValueAsync(JsonUtility.ToJson(user));
                break;
            }
        }
        foreach (var t in tableSnapshot.Children)
        {
            table tmpTable = JsonUtility.FromJson<table>(t.GetRawJsonValue());
            if (t.Child("players").ChildrenCount > 1)
            {
                foreach (var pl in t.Child("players").Children)
                {
                    tablePlayer pl2 = JsonUtility.FromJson<tablePlayer>(pl.GetRawJsonValue());
                    if (pl2.playerID == PlayerPrefs.GetString("userID"))
                    {
                        tableDB.Child(t.Key).Child("players").Child(pl.Key).RemoveValueAsync();
                        Debug.Log("user removed");
                        break;
                    }
                }
            }
            else if (t.Child("players").ChildrenCount <= 1 && t.Child("players").Child(PlayerPrefs.GetString("userID")).Exists)
            {
                tableDB.Child(t.Key).RemoveValueAsync();
                Debug.Log("table " + t.Key + " removed");
                break;
            }
        }
    }
    void OnApplicationPause(bool appPause)
    {
        if (appPause)
        {
            foreach (var sn in snapShotClub.Children)
            {
                gameUser tmpUser = JsonUtility.FromJson<gameUser>(sn.GetRawJsonValue());
                if (PlayerPrefs.GetString("loginName") == tmpUser.name && PlayerPrefs.GetString("password") == tmpUser.password)
                {
                    user = tmpUser;
                    user.status = "offline";
                    usersDB.Child(sn.Key).SetRawJsonValueAsync(JsonUtility.ToJson(user));
                    break;
                }
            }
            updatePlayerDB();
        }
    }
    void update_game_player_snapshot(DataSnapshot snapshot)
    {
        int count = (int)snapshot.Child(current_table).Child("players").ChildrenCount;
        whoPlay = Random.Range(0, count);
        gamePlayerSnap = new DataSnapshot[count];
        int snapsCount = 0;
        foreach (DataSnapshot players in snapshot.Child(current_table).Child("players").Children)
        {
            gamePlayerSnap[snapsCount] = players;
            snapsCount++;
        }
        whoPlayID = gamePlayerSnap[whoPlay].Key;
    }
    IEnumerator nextWhoPlay()
    {
        update_tableDB();
        yield return new WaitForSeconds(0.2f);
        int count = (int)tableSnapshot.Child(current_table).Child("players").ChildrenCount;
        gamePlayerSnap = new DataSnapshot[count];
        int snapsCount = 0;
        int whoPlayNext = 0;
        DataSnapshot snapD;
        foreach (DataSnapshot players in tableSnapshot.Child(current_table).Child("players").Children)
        {
            gamePlayerSnap[snapsCount] = players;
            snapsCount++;
        }
        for (int i = 0; i < count; i++)
        {
            if (gamePlayerSnap[i].Key == whoPlayID || whoPlayID == "")
            {
                Debug.Log("есть сл на " + i);
                whoPlayNext = i;
                break;
            }
        }
        whoPlayNext = whoPlayNext + 1;
        if (whoPlayNext >= count) whoPlayNext = 0;
        whoPlay = whoPlayNext;
        snapD = gamePlayerSnap[whoPlay];
        
        tableDB.Child(current_table).Child("whoPlay").SetValueAsync(snapD.Key);
        //StartCoroutine(show_my_message("сл игрок из: " + gamePlayerSnap.Length + " - " + snapD.Key));
    }
}
