using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;

public class createClubPage : MonoBehaviour
{
    public GameObject back_page;
    public GameObject clubs_menu_page;

    public Text club_name_text;

    public DataSnapshot clubs_snapshot = null;
    public DatabaseReference dbClubs = null;
    void Start()
    {
        dbClubs = transform.parent.GetComponent<profileScript>().dbClubs;
        clubs_snapshot = transform.parent.GetComponent<profileScript>().snapShotClub;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !Input.GetKeyUp(KeyCode.Escape)) // Ловим кнопку назад
        {
            transform.gameObject.SetActive(false);
            back_page.SetActive(true);
        }
    }
    public void onSaveButtonClick()
    {
        if (club_name_text.text.Length > 0)
        {
            string newKey = "";
            string plKey = "";
            tableClubs clubOnline = new tableClubs();
            clubPlayer clubPlayer = new clubPlayer();
            
            clubOnline.name = club_name_text.text;
            clubOnline.date_created = System.DateTime.Now.ToString();
            clubOnline.created = PlayerPrefs.GetString("userID");
            
            clubPlayer.id = clubOnline.created;
            clubPlayer.name = PlayerPrefs.GetString("loginName");
            clubPlayer.level = "0";
            
            newKey = dbClubs.Push().Key;
            plKey = dbClubs.Child(newKey).Child("players").Push().Key;
            
            dbClubs.Child(newKey).SetRawJsonValueAsync(JsonUtility.ToJson(clubOnline));
            dbClubs.Child(newKey).Child("players").Child(plKey).SetRawJsonValueAsync(JsonUtility.ToJson(clubPlayer));

            transform.gameObject.SetActive(false);
            clubs_menu_page.SetActive(true);
            clubs_menu_page.GetComponent<mainClubPage>().dbClubs = dbClubs;
            clubs_menu_page.GetComponent<mainClubPage>().update_club_db();
        }
        else
        {

        }
    }
}
