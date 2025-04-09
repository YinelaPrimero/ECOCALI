using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

public class FirebaseEventService : MonoBehaviour
{
    private DatabaseReference dbReference;
    public bool FirebaseReady { get; private set; } = false;

    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == Firebase.DependencyStatus.Available)
            {
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;
                FirebaseReady = true;
            }
            else
            {
                Debug.LogError("No se pudo inicializar Firebase en FirebaseEventService.");
            }
        });
    }

    public async Task<List<Event>> GetFirstSixEventsAsync()
    {
        List<Event> firstSixEvents = new List<Event>();

        if (!FirebaseReady || dbReference == null)
        {
            Debug.LogWarning("Firebase no está listo todavía.");
            return firstSixEvents;
        }

        // Fecha actual en formato "yyyy-MM-dd"
        string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

        // Query: obtener los eventos ordenados por Date, a partir de la fecha actual
        Query query = FirebaseDatabase.DefaultInstance
            .GetReference("events")
            .OrderByChild("Date")
            .LimitToFirst(6); 

        try
        {
            DataSnapshot snapshot = await query.GetValueAsync();
            if (snapshot.Exists)
            {
                foreach (DataSnapshot child in snapshot.Children)
                {
                    string json = child.GetRawJsonValue();
                    Event ev = JsonUtility.FromJson<Event>(json);
                    // Verificar que la fecha sea >= currentDate (en caso de discrepancias de formato)
                    if (!string.IsNullOrEmpty(ev.Date) && string.Compare(ev.Date, currentDate) >= 0)
                    {
                        firstSixEvents.Add(ev);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error obteniendo los 6 primeros eventos: " + ex.Message);
        }

        return firstSixEvents;
    }

}