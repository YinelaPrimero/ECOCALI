using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Event
{
    public string Title;
    public string Description;
    public List<string> Activities;
    public string ImageUrl;
    public float Price;
    public string Date;  // en formato "yyyy-MM-dd"
    public string Hour;
    public string Place;
    public string Organizer;
}