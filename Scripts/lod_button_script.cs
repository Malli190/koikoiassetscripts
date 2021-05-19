using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class lod_button_script : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject lod_fade_panel;

    List<GameObject> card_in_lod = new List<GameObject>();
    List<GameObject> card_list = new List<GameObject>();

    GameObject canvas;
    GameObject[] cards;

    void Start()
    {
        //lod_fade_panel = GameObject.Find("lod_fade_panel");

        canvas = GameObject.Find("Canvas");
    }
    void Update()
    {
    }
    // Удерживаем lod и увеличиваем карты
    public void OnPointerDown(PointerEventData eventData)
    {
        show_fade_panel();
    }
    public void show_fade_panel()
    {
        int count = transform.GetComponentInChildren<Transform>().childCount;
        var sizeDelta = 0.3f * lod_fade_panel.GetComponent<RectTransform>().rect.width / canvas.GetComponent<Canvas>().scaleFactor;
        Debug.Log("внутри " + count + " карт");
        lod_fade_panel.SetActive(true);
        if (count > 0)
        {
            cards = GameObject.FindGameObjectsWithTag("card");
            foreach (GameObject card in cards)
                if (card.transform.parent == transform)
                {
                    card.transform.SetParent(lod_fade_panel.transform);
                    card_in_lod.Add(card);
                }
        }
    }
    public void hide_fade_panel()
    {
        Debug.Log("отпустил с " + this.name);
        if (card_in_lod.Count > 0)
        {
            foreach (GameObject card in card_in_lod)
            {
                card.transform.SetParent(gameObject.transform);
            }
            card_in_lod.Clear();
        }
        lod_fade_panel.SetActive(false);
    }
    //
    // Отпускаем lod и возвращаемкарты
    //
    public void OnPointerUp(PointerEventData eventData)
    {
        hide_fade_panel();
    }
}
