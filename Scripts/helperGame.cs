using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using Firebase.Database;

public class helperGame : MonoBehaviour
{
    public static tableClubs tmp_club_data;
    public static string GetMd5Hash(MD5 md5Hash, string input)
    {
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
        StringBuilder sBuilder = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }
        return sBuilder.ToString();
    }
    public static int getMyScore(List<cardScript> cards)
    {
        int score = 0;
        foreach (cardScript card in cards)
        {
            score += card.score;
        }
        return score;
    }
    public static void getCardScore(ref GameObject card, int mouth, int type)
    {
        switch (mouth)
        {
            case 0:
                {
                    if (type == 0)
                    {
                        card.GetComponent<cardScript>().score = 20;
                    }
                    else if (type == 1)
                    {
                        card.GetComponent<cardScript>().score = 5;
                    }
                    else card.GetComponent<cardScript>().score = 0;
                    break;
                }
            case 1:
                {
                    if (type == 0)
                    {
                        card.GetComponent<cardScript>().score = 10;
                    }
                    else if (type == 1)
                    {
                        card.GetComponent<cardScript>().score = 5;
                    }
                    else card.GetComponent<cardScript>().score = 0;
                    break;
                }
            case 2:
                {
                    if (type == 0)
                    {
                        card.GetComponent<cardScript>().score = 20;
                    }
                    else if (type == 1)
                    {
                        card.GetComponent<cardScript>().score = 5;
                    }
                    else card.GetComponent<cardScript>().score = 0;
                    break;
                }
            case 3:
                {
                    if (type == 0)
                    {
                        card.GetComponent<cardScript>().score = 10;
                    }
                    else if (type == 1)
                    {
                        card.GetComponent<cardScript>().score = 5;
                    }
                    else card.GetComponent<cardScript>().score = 0;
                    break;
                }
            case 4:
                {
                    if (type == 0)
                    {
                        card.GetComponent<cardScript>().score = 10;
                    }
                    else if (type == 1)
                    {
                        card.GetComponent<cardScript>().score = 5;
                    }
                    else card.GetComponent<cardScript>().score = 0;
                    break;
                }
            case 5:
                {
                    if (type == 0)
                    {
                        card.GetComponent<cardScript>().score = 10;
                    }
                    else if (type == 1)
                    {
                        card.GetComponent<cardScript>().score = 5;
                    }
                    else card.GetComponent<cardScript>().score = 0;
                    break;
                }
            case 6:
                {
                    if (type == 0)
                    {
                        card.GetComponent<cardScript>().score = 10;
                    }
                    else if (type == 1)
                    {
                        card.GetComponent<cardScript>().score = 5;
                    }
                    else card.GetComponent<cardScript>().score = 0;
                    break;
                }
            case 7:
                {
                    if (type == 0)
                    {
                        card.GetComponent<cardScript>().score = 20;
                    }
                    else if (type == 1)
                    {
                        card.GetComponent<cardScript>().score = 10;
                    }
                    else card.GetComponent<cardScript>().score = 0;
                    break;
                }
            case 8:
                {
                    if (type == 0)
                    {
                        card.GetComponent<cardScript>().score = 10;
                    }
                    else if (type == 1)
                    {
                        card.GetComponent<cardScript>().score = 5;
                    }
                    else card.GetComponent<cardScript>().score = 0;
                    break;
                }
            case 9:
                {
                    if (type == 0)
                    {
                        card.GetComponent<cardScript>().score = 10;
                    }
                    else if (type == 1)
                    {
                        card.GetComponent<cardScript>().score = 5;
                    }
                    else card.GetComponent<cardScript>().score = 0;
                    break;
                }
            case 10:
                {
                    if (type == 0)
                    {
                        card.GetComponent<cardScript>().score = 20;
                    }
                    else if (type == 1)
                    {
                        card.GetComponent<cardScript>().score = 10;
                    }
                    else if (type == 2)
                    {
                        card.GetComponent<cardScript>().score = 5;
                    }
                    else card.GetComponent<cardScript>().score = 0;
                    break;
                }
            case 11:
                {
                    if (type == 0)
                    {
                        card.GetComponent<cardScript>().score = 20;
                    }
                    else if (type == 1)
                    {
                        card.GetComponent<cardScript>().score = 10;
                    }
                    else card.GetComponent<cardScript>().score = 0;
                    break;
                }
        }
    }
}
