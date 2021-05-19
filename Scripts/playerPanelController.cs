using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerPanelController : MonoBehaviour
{
    public GameObject back_page;
    public GameObject profileMenu;
    public GameObject default_page;
    GameObject magazineMenu = null;
    void Start()
    {
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !Input.GetKeyUp(KeyCode.Escape)) // Ловим кнопку назад
        {
            transform.parent.gameObject.SetActive(false);
            back_page.SetActive(true);
        }
    }
    public void onPlayerProfileButtonClick()
    {
        transform.parent.gameObject.SetActive(false);
        profileMenu.gameObject.SetActive(true);
    }
    public void onMagazineButtonClick()
    {
    }
}
