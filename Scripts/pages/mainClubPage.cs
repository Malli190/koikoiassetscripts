using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;

public class mainClubPage : MonoBehaviour
{
    public GameObject new_club_panel;
    public GameObject club_page;
    public GameObject fade_panel;
    public GameObject back_page;
    public GameObject content_list;
    GameObject[] club_list = new GameObject[0];

    public GameObject club_panel_prefab;

    public DataSnapshot clubs_snapshot = null;
    public DataSnapshot current_club_snapshot = null;
    public DatabaseReference dbClubs = null;
    void Start()
    {
        //dbClubs = transform.parent.gameObject.GetComponent<profileScript>().dbClubs;
        clubs_snapshot = transform.parent.GetComponent<profileScript>().snapShotClub;

        fade_panel.SetActive(false);
    }
    void dbClubs_ChildChanged(object sender, ChildChangedEventArgs e)
    {
        clubs_snapshot = e.Snapshot;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !Input.GetKeyUp(KeyCode.Escape)) // Ловим кнопку назад
        {
            transform.gameObject.SetActive(false);
            back_page.SetActive(true);
        }
    }
    public void onNewClubButtonClick() // Нажимаем на новый клуб
    {
        fade_panel.SetActive(false);
        transform.gameObject.SetActive(false);
        new_club_panel.GetComponent<createClubPage>().dbClubs = dbClubs;
        new_club_panel.GetComponent<createClubPage>().clubs_snapshot = clubs_snapshot;
        new_club_panel.GetComponent<createClubPage>().back_page = transform.gameObject;
        new_club_panel.SetActive(true);
    }
    public void onSearchClubClick()
    {
        fade_panel.SetActive(true);
    }
    public void onCloseButtonClick()
    {
        fade_panel.SetActive(false);
    }
    public void onEnterClubButtonClick()
    {
        fade_panel.SetActive(false);
    }
    public void update_club_db()
    {
        Debug.Log("обновляю дб клубов");
        dbClubs.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                current_club_snapshot = task.Result;
                
                return;
            }
            else if (task.IsFaulted)
            {
                return;
            }
        });
        StartCoroutine(update_club_panel());
    }
    public IEnumerator update_club_panel()
    {
        yield return new WaitForSeconds(0.3f);
        Debug.Log("смотрю клубы");
        club_list = GameObject.FindGameObjectsWithTag("club_list");
        Debug.Log(")");
        if (club_list.Length > 0)
        {
            Debug.Log("какие-то есть");
            for (int i = 0; i < club_list.Length; i++)
            {
                GameObject tmpObj = club_list[i];
                Debug.Log("уничтожаю " + tmpObj.transform.Find("Text").GetComponent<Text>().text);
                Destroy(tmpObj);
            }
        }
        Debug.Log("создаю новых");
        foreach (DataSnapshot club in current_club_snapshot.Children)
        {
            if (club.Child("created").Value.ToString() == PlayerPrefs.GetString("userID"))
            {
                GameObject tmpObj = Instantiate(club_panel_prefab, content_list.transform);
                tableClubs tmpClub = JsonUtility.FromJson<tableClubs>(club.GetRawJsonValue());
                tmpObj.transform.Find("Text").gameObject.GetComponent<Text>().text = tmpClub.name;
                tmpObj.GetComponent<Button>().onClick.AddListener(delegate()
                {
                    onClub_click(tmpClub, gameObject);
                });
            }
        }
        Debug.Log("готово");
    }
    void onClub_click(tableClubs tmpClub, GameObject parent)
    {
        parent.SetActive(false);
        club_page.SetActive(true);
        club_page.gameObject.GetComponent<clubPage>().back_page = transform.gameObject;
        club_page.gameObject.GetComponent<clubPage>().update_club_data(tmpClub);
    }
    public void Initializate()
    {
        dbClubs.ChildChanged += dbClubs_ChildChanged;
    }
    void OnDestroy()
    {
        dbClubs.ChildChanged -= dbClubs_ChildChanged;
    }
}
