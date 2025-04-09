using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EventListUIController : MonoBehaviour
{
    public FirebaseEventService eventService;     // Referencia a nuestro servicio
    public Transform eventsPanel;                 // Padre donde se instanciarán los prefabs
    public GameObject eventCardPrefab;           // Prefab de la tarjeta

    // Referencia al panel de detalle de evento
    public GameObject eventDetailPanel;
    // Referencias a los elementos del panel de detalle (Text, Image, etc.)
    public Text detailTitle;
    public Text detailDescription;
    public Text detailDateHour;
    public Text detailActivities;
    // etc.

    void OnEnable()
    {
        FirebaseInitializer.OnFirebaseInitialized += LoadAndDisplayFirstSixEvents;
    }

    void OnDisable()
    {
        FirebaseInitializer.OnFirebaseInitialized -= LoadAndDisplayFirstSixEvents;
    }

    private async void LoadAndDisplayFirstSixEvents()
    {
        List<Event> firstSixEvents = await eventService.GetFirstSixEventsAsync();

        foreach (Event ev in firstSixEvents)
        {
            // Crear la tarjeta
            GameObject newCard = Instantiate(eventCardPrefab, eventsPanel);

            // Buscar referencias a los elementos UI de la tarjeta
            Text titleText = newCard.transform.Find("TitleText").GetComponent<Text>();
            Text dateText = newCard.transform.Find("Info/Date/DateText").GetComponent<Text>();
            Text hourText = newCard.transform.Find("Info/Hour/HourText").GetComponent<Text>();
            Text placeText = newCard.transform.Find("Info/Place/PlaceText").GetComponent<Text>();
            Text priceText = newCard.transform.Find("Price/PriceText").GetComponent<Text>();
            Text organizersText = newCard.transform.Find("Info/Organizers/OrganizersText").GetComponent<Text>();
            Button button = newCard.transform.Find("EventButton").GetComponent<Button>();

            // Asignar valores
            titleText.text = ev.Title;
            dateText.text = ev.Date;
            hourText.text = ev.Hour;
            placeText.text = ev.Place;
            organizersText.text = ev.Organizer;

            if (ev.Price != 0)
            {
                priceText.text = ev.Price.ToString();
            }
            
            // Asignar el evento onClick
            button.onClick.AddListener(() =>
            {
                ShowEventDetail(ev);
            });
        }
    }

    private void ShowEventDetail(Event ev)
    {
        // Mostrar el panel de detalle
        eventDetailPanel.SetActive(true);

        // Rellenar los campos de texto
        detailTitle.text = ev.Title;
        detailDescription.text = ev.Description;
        detailDateHour.text = $"Fecha: {ev.Date}\nHora: {ev.Hour}";
        if (ev.Activities != null && ev.Activities.Count > 0)
        {
            detailActivities.text = $"- {string.Join("\n- ", ev.Activities)}";
        }
        else
        {
            detailActivities.text = "No hay actividades disponibles";
        }


        // etc.

        // Si tu panel de inicio se debe ocultar, haz algo como:
        // eventsPanel.gameObject.SetActive(false);
    }

    // Método para cerrar el panel de detalle (por ejemplo, en un botón "Volver")
    public void CloseEventDetail()
    {
        eventDetailPanel.SetActive(false);
        // eventsPanel.gameObject.SetActive(true);
    }
}