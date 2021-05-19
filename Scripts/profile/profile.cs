using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class profile : MonoBehaviour
{
   
}
public class gameUser
{
    public string name;
    public string password;
    public string email;
    public string club_name;
    public string status;
    public string last_day_online;
    public string id;
    public string club_id;
    public int coins;
    public int fishki;
    public int gems;

    public gameUser() { status = "создано"; }
    public gameUser(string id, string name, string password)
    {
        this.id = id;
        this.name = name;
        this.password = password;

        status = "создано";

        last_day_online = System.DateTime.Now.ToString();
    }
}
