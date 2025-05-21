using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using MiniJSON;  // Asegúrate de tener MiniJSON en Assets/Scripts/Plugins

public class FirebaseRestService : MonoBehaviour
{
    private const string EventsUrl =
        "https://ecocali-uao-default-rtdb.firebaseio.com/events.json";

    /// <summary>
    /// Obtiene los primeros N eventos con fecha >= hoy, ordenados por Date.
    /// </summary>
    public IEnumerator GetFirstNEvents(int n, Action<List<Event>> callback)
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        string url = $"{EventsUrl}?orderBy=\"Date\"&startAt=\"{today}\"&limitToFirst={n}";
        Debug.Log($"[REST] GET {url}");

        using (var req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError ||
                req.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"[REST] Error GET events: {req.error}");
                callback(new List<Event>());
                yield break;
            }

            // Deserializar el JSON crudo
            var root = Json.Deserialize(req.downloadHandler.text)
                       as Dictionary<string, object>;

            var list = new List<Event>();
            if (root != null)
            {
                foreach (var kv in root)
                {
                    if (!(kv.Value is Dictionary<string, object> data)) continue;

                    // Mapear manualmente a tu clase Event
                    var ev = new Event
                    {
                        Title = data.GetValue<string>("Title"),
                        Description = data.GetValue<string>("Description"),
                        ImageUrl = data.GetValue<string>("ImageUrl"),
                        Price = data.GetValue<float>("Price", 0f),
                        Date = data.GetValue<string>("Date"),
                        Hour = data.GetValue<string>("Hour"),
                        Place = data.GetValue<string>("Place"),
                        Organizer = data.GetValue<string>("Organizer"),
                        Activities = data.GetList<string>("Activities")
                    };
                    list.Add(ev);
                }

                // Aseguramos el orden por fecha
                list = list.OrderBy(e => e.Date).ToList();
            }

            callback(list);
        }
    }
}

/// <summary>
/// Extensiones para extraer valores de Dictionary<string, object>.
/// </summary>
public static class MiniJsonExtensions
{
    /// <summary>
    /// Obtiene un valor primitivo (string, int, float…) y lo convierte.
    /// </summary>
    public static T GetValue<T>(this Dictionary<string, object> dict,
                                string key, T defaultValue = default)
    {
        if (dict.TryGetValue(key, out var obj) && obj != null)
        {
            try
            {
                // Manejo especial para float (MiniJSON parsea números como double)
                if (typeof(T) == typeof(float))
                {
                    return (T)(object)Convert.ToSingle(obj);
                }
                // Para los demás tipos mantenemos ChangeType
                return (T)Convert.ChangeType(obj, typeof(T));
            }
            catch
            {
                // Si falla la conversión, devolvemos el default
            }
        }
        return defaultValue;
    }

    /// <summary>
    /// Obtiene una lista de valores T a partir de un List<object>.
    /// </summary>
    public static List<T> GetList<T>(this Dictionary<string, object> dict, string key)
    {
        var result = new List<T>();
        if (dict.TryGetValue(key, out var obj) && obj is List<object> rawList)
        {
            foreach (var item in rawList)
            {
                try
                {
                    if (typeof(T) == typeof(string))
                    {
                        // Para strings, toString() suele bastar
                        result.Add((T)(object)item.ToString());
                    }
                    else
                    {
                        result.Add((T)Convert.ChangeType(item, typeof(T)));
                    }
                }
                catch
                {
                    // Saltamos elementos que no conviertan
                }
            }
        }
        return result;
    }
}
