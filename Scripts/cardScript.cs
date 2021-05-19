using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Firebase.Database;

public class onlineCard
{
    public string month;
    public string flover;
    public int type;
    public int num;
    public int score;
    public string image_path;
    public string player_id;
    public string online_key; // ключ родителя
    public float posX;
    public float posY;
    public float posZ;
    public float targetX, targetY, targetZ;
    public string targetCard;
    public string state;
    public bool open;
    public onlineCard() { }
}
public class cardScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public string month;
    public string flover;
    public int type;
    public int num;
    public int score;
    public string pKey;
    public string player_id;

    float speed = 10f;
    float rotateVal = 0;

    public bool open;
    public bool move;
    public bool moveToList;
    public bool goToHand;
    public bool check_move = false;

    GameObject table_panel;
    GameObject hand_panel;
    GameObject hand_lod_panel;
    GameObject fade_panel;
    GameObject cards;
    GameObject canvas;

    public GameObject target_card;
    public Transform target_lod;
    public AudioClip card_clip_move;
    public AudioClip card_clip_stop;

    AudioSource card_audio_source;

    Transform left_panel, right_panel;

    [SerializeField]
    public Transform current_parent;

    public Transform default_parent;
    public List<GameObject> cards_on_table = new List<GameObject>();
    public Vector3 target_position;

    RectTransform rect;
    Quaternion default_rotate;

    public DataSnapshot onlineState;

    GameObject card_child_image;
    Image curentImage;
    Sprite defImage;
    public Sprite cardImage;

    int numPlayers;

    void Start()
    {
        target_position = Vector3.zero;
        hand_panel = GameObject.Find("hand_panel");
        hand_lod_panel = GameObject.Find("hand_lod_panel");
        table_panel = GameObject.Find("table_panel");

        left_panel = table_panel.transform.Find("left_panel").transform;
        right_panel = table_panel.transform.Find("right_panel").transform;

        cards = table_panel.transform.Find("card_list").gameObject;
        canvas = GameObject.Find("Canvas");
        //cardImage = Resources.Load<Sprite>("cards/pae0");
        card_child_image = transform.Find("card_image").gameObject;
        defImage = Resources.Load<Sprite>("card");
        
        curentImage = card_child_image.GetComponent<Image>();
        //defImage = curentImage.sprite;

        curentImage.sprite = defImage;

        rect = GetComponent<RectTransform>();

        card_audio_source = GetComponent<AudioSource>();

        numPlayers = gameSceneManager.numPlayers;

        rotateVal = Random.Range(1f, 10f);
        default_rotate = rect.rotation;

        open = false;
        move = false;
        moveToList = false;
        goToHand = false;
    }
    public void play_card_sound(string role)
    {
        if (role == "move")
            card_audio_source.PlayOneShot(card_clip_move, 0.9f);
        else if (role == "stop")
            card_audio_source.PlayOneShot(card_clip_stop, 0.6f);
    }
    void Update()
    {
        if (open) curentImage.sprite = cardImage;
        else curentImage.sprite = defImage;
        
        if (move)
        {
            
            transform.position = Vector3.MoveTowards(transform.position, target_position, Vector3.Distance(transform.position, target_position) * speed * Time.deltaTime);
            //if (current_parent == left_panel.transform || current_parent == right_panel.transform)
                rect.Rotate(new Vector3(0, 0, 1f), Vector3.Distance(transform.position, target_position) * rotateVal);
            
            if (Vector3.Distance(transform.position, target_position) < 0.08f)
            {
                transform.SetParent(current_parent);
                transform.position = target_position;
                transform.SetAsLastSibling();

                if (transform.parent != left_panel && transform.parent != right_panel) rect.rotation = default_rotate;
                if (transform.parent == hand_panel.transform || transform.parent == left_panel || transform.parent == right_panel ) open = true;
                else open = false;
                
                float sizeDelta = 0f;

                if (transform.parent == hand_panel.transform)
                {
                    sizeDelta = 0.099f * table_panel.GetComponent<RectTransform>().rect.width / canvas.GetComponent<Canvas>().scaleFactor;
                }
                if (transform.parent == left_panel && transform.parent == right_panel)
                {
                    //transform.SetAsFirstSibling();
                    sizeDelta = 0.12f * table_panel.GetComponent<RectTransform>().rect.width / canvas.GetComponent<Canvas>().scaleFactor;
                }
                if (transform.parent != left_panel && transform.parent != right_panel && transform.parent != hand_panel.transform)
                {
                    sizeDelta = 0.05f * table_panel.GetComponent<RectTransform>().rect.width / canvas.GetComponent<Canvas>().scaleFactor;
                }

                if (target_card != null && target_card.tag != "table")
                {
                    if (current_parent == target_card.transform)
                    {
                        Transform par = transform;
                        //transform.SetParent(target_card.transform.parent);
                        //target_card.transform.SetParent(par);

                        open = true;
                        //sizeDelta = 0.085f * table_panel.GetComponent<RectTransform>().rect.height / canvas.GetComponent<Canvas>().scaleFactor;

                        target_position = target_lod.position;
                        current_parent = target_lod;
                        target_card.GetComponent<cardScript>().target_position = target_position;
                        target_card.GetComponent<cardScript>().current_parent = current_parent;
                        
                        goToHand = true;
                        target_card.GetComponent<cardScript>().goToHand = true;
                        play_card_sound("move");
                    }
                }
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeDelta);
                if (!gameSceneManager.isRazdacha)
                    play_card_sound("stop");
                move = false;
            }
        }
        if (goToHand)
        {
            transform.position = Vector3.MoveTowards(transform.position, target_position, Vector3.Distance(transform.position, target_position) * speed * Time.deltaTime);
            rect.Rotate(new Vector3(0, 0, 1f), Vector3.Distance(transform.position, target_position) * rotateVal);
            if (Vector3.Distance(transform.position, target_position) < 0.08f)
            {
                transform.SetParent(current_parent);
                transform.position = target_position;
                transform.SetAsFirstSibling();
                var sizeDelta = 1.1f * (current_parent.gameObject.GetComponent<RectTransform>().rect.width * 1f) / canvas.GetComponent<Canvas>().scaleFactor;

                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeDelta);
                rect.rotation = default_rotate;
                play_card_sound("stop");

                open = true;
                goToHand = false;
            }
        }
        if (moveToList)
        {
            transform.position = Vector3.MoveTowards(transform.position, target_position, Vector3.Distance(transform.position, target_position) * speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, target_position) < 0.08f)
            {
                transform.SetParent(default_parent);
                transform.position = target_position;

                moveToList = false;
            }
        }
    }
    public void onClick()
    {
        if (transform.parent == hand_panel.transform)
        {
            Debug.Log("Переход. " + transform.position.ToString() + " name: " + transform.Find("Text").GetComponent<Text>().text);
            check_move = true;
            //cards_on_table = table_panel
        }
        if (transform.parent == cards.transform)
        {
            Debug.Log("Взял из колоды");
            check_move = true;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (transform.parent.gameObject.tag == "table_lod")
        {
            fade_panel = transform.parent.gameObject;
            transform.parent.gameObject.GetComponent<lod_button_script>().show_fade_panel();
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        fade_panel.GetComponent<lod_button_script>().hide_fade_panel();
    }
    public enum MONTH
    {
        NONE = 0,
        JAN = 1,
        FEB = 2,
        MAR = 3,
        APR = 4,
        MAY = 5,
        JUN = 6,
        JUL = 7,
        AUG = 8,
        SEP = 9,
        OCT = 10,
        NOV = 11,
        DEC = 12
    }

    public enum FLOWER
    {
        NONE = 0,
        MATSU = 1,
        UME = 2,
        SAKURA = 3,
        FUJI = 4,
        AYAME = 5,
        BOTAN = 6,
        HAGI = 7,
        SUSUKI = 8,
        KIKU = 9,
        MOMIJI = 10,
        YANAGI = 11,
        KIRI = 12
    }

    public enum DIVISION
    {
        NONE = 0,
        TSURU = 1,
        MAKU = 2,
        TSUKI = 3,
        ONO = 4,
        HOUOU = 5,
        UGUISU = 6,
        HOTOTOGISU = 7,
        YATSUHASHI = 8,
        KARI = 9,
        SAKAZUKI = 10,
        TSUBAME = 11,
        INOSISI = 12,
        SHIKA = 13,
        TYOU = 14,
        TANZAKU = 15,
        AKATANZAKU = 16,
        AOTANZAKU = 17,
        KASU1 = 18,
        KASU2 = 19,
        KASU3 = 20
    }

    public enum TYPE
    {
        NONE = 0,
        HIKARI = 1,
        TANE = 2,
        TANZAKU = 3,
        KASU = 4,
        BAKE = 5,
    }
}
