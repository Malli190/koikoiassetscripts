using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class table
{
    public string table_id;
    public string table_parent;
    public string table_type;
    public string table_name;
    public static string default_name;
    public string created_date;
    public string game_status;
    public string whoPlay;
    public int whoPlayID;
    public onlineCard[] onlineCards;

    public table()
    {
        created_date = System.DateTime.Now.ToString();
    }
}
public class tablePlayer
{
    public string playerID;
    public string playerName;
    public string status;
    public string message;
    public tablePlayer()
    {
    }
}
public class gameTablePlayer
{
    public Transform position;
    public string id;

    public gameTablePlayer() { }
}