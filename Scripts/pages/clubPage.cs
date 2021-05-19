using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class clubPage : MonoBehaviour
{
    public GameObject back_page;
    public tableClubs club;
    public string name;
    public string id;
    void Start()
    {
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !Input.GetKeyUp(KeyCode.Escape)) // Ловим кнопку назад
        {
            transform.gameObject.SetActive(false);
            back_page.SetActive(true);
        }
    }
    public void update_club_data(tableClubs tmpClub)
    {
        this.club = tmpClub;
        transform.Find("club_panel").Find("name_label").GetComponent<Text>().text = tmpClub.name;
        transform.Find("club_panel").Find("id_label").GetComponent<Text>().text = tmpClub.created;
    }
}
