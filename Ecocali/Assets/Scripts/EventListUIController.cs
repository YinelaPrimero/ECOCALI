using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventListUIController : MonoBehaviour
{
    [Header("UI References")]
    public Transform eventsPanel;
    public GameObject eventCardPrefab;
    public GameObject eventDetailPanel;
    public Text detailTitle, detailDescription, detailDateHour;

    [Header("Services")]
    public FirebaseRestService restService;

    private void Start()
    {
        StartCoroutine(restService.GetFirstNEvents(6, OnEventsLoaded));
    }

    private void OnEventsLoaded(List<Event> events)
    {
        foreach (var ev in events)
        {
            var card = Instantiate(eventCardPrefab, eventsPanel);

            card.transform.Find("TitleText")
                .GetComponent<Text>().text = ev.Title;

            card.transform.Find("Info/Date/DateText")
                .GetComponent<Text>().text = ev.Date;

            card.transform.Find("Info/Hour/HourText")
                .GetComponent<Text>().text = ev.Hour;

            card.transform.Find("Info/Place/PlaceText")
                .GetComponent<Text>().text = ev.Place;

            card.transform.Find("Info/Organizers/OrganizersText")
                .GetComponent<Text>().text = ev.Organizer;

            card.transform.Find("Price/PriceText")
                .GetComponent<Text>().text = ev.Price != 0
                    ? ev.Price.ToString("F2")
                    : "Gratis";

            var button = card.transform.Find("EventButton").GetComponent<Button>();
            button.onClick.AddListener(() => ShowEventDetail(ev));
        }
    }

    private void ShowEventDetail(Event ev)
    {
        detailTitle.text = ev.Title;
        detailDescription.text = ev.Description;
        detailDateHour.text = $"Fecha: {ev.Date}\nHora: {ev.Hour}";
        eventDetailPanel.SetActive(true);
    }

    public void CloseEventDetail()
    {
        eventDetailPanel.SetActive(false);
    }
}
