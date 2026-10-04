using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PassengerSpawner : MonoBehaviour
{
    public int passengerCount = 30;
    public bool showDebug = true;

    private string[] firstNames = {
        "Rahul", "Amit", "Priya", "Sneha", "Vikram", "Pooja", "Rohit", "Neha",
        "Arjun", "Kavita", "Suresh", "Anita", "Manoj", "Deepak", "Sunita", "Ravi"
    };

    private string[] lastNames = {
        "Sharma", "Verma", "Patel", "Singh", "Yadav", "Gupta", "Mishra", "Joshi",
        "Kumar", "Chouhan", "Jain", "Tiwari"
    };

    IEnumerator Start()
    {
        // Wait one frame so the berth numbers already exist
        yield return null;
        SpawnAll();
    }

    void SpawnAll()
    {
        List<Berth> berths = new List<Berth>(GetComponentsInChildren<Berth>());

        if (berths.Count == 0)
        {
            Debug.LogWarning("No berths found. Is BerthNumberer on the Coach?");
            return;
        }

        // Shuffle the berths so passengers get random places
        for (int i = 0; i < berths.Count; i++)
        {
            int j = Random.Range(i, berths.Count);
            Berth temp = berths[i];
            berths[i] = berths[j];
            berths[j] = temp;
        }

        GameObject parent = new GameObject("Passengers");
        int count = Mathf.Min(passengerCount, berths.Count);

        for (int i = 0; i < count; i++)
        {
            CreatePassenger(berths[i], parent.transform);
        }
    }

    void CreatePassenger(Berth berth, Transform parent)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        go.transform.SetParent(parent);
        go.transform.localScale = new Vector3(0.3f, 0.45f, 0.3f);

        Vector3 pos = berth.transform.position;
        float top = pos.y + berth.transform.lossyScale.y * 0.5f;

        if (berth.type == BerthType.Lower || berth.type == BerthType.SideLower)
        {
            // Sitting upright
            go.transform.position = new Vector3(pos.x, top + 0.45f, pos.z);
        }
        else if (berth.type == BerthType.SideUpper)
        {
            // Lying along the side berth
            go.transform.position = new Vector3(pos.x, top + 0.15f, pos.z);
            go.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
        else
        {
            // Lying across a middle or upper berth
            go.transform.position = new Vector3(pos.x, top + 0.15f, pos.z);
            go.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        }

        // Random colour so passengers look different
        go.GetComponent<Renderer>().material.color =
            Color.HSVToRGB(Random.value, 0.5f, 0.9f);

        // Give them their data
        Passenger p = go.AddComponent<Passenger>();
        p.passengerName = firstNames[Random.Range(0, firstNames.Length)] + " " +
                          lastNames[Random.Range(0, lastNames.Length)];
        p.actualBerth = berth.number;
        p.pnr = Random.Range(100000, 999999).ToString() + Random.Range(1000, 9999).ToString();

        string today = System.DateTime.Now.ToString("dd-MM-yyyy");
        string yesterday = System.DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy");

        // Start with a perfect ticket
        p.hasTicket = true;
        p.ticketBerth = berth.number;
        p.ticketDate = today;
        p.ticketClass = "SL";
        p.status = TicketStatus.Valid;

        // Then break some tickets
        float roll = Random.value;

        if (roll < 0.15f)
        {
            p.hasTicket = false;
            p.status = TicketStatus.NoTicket;
        }
        else if (roll < 0.30f)
        {
            int other = Random.Range(1, 73);
            if (other == berth.number) other = (other % 72) + 1;
            p.ticketBerth = other;
            p.status = TicketStatus.WrongBerth;
        }
        else if (roll < 0.45f)
        {
            p.ticketDate = yesterday;
            p.status = TicketStatus.WrongDate;
        }
        else if (roll < 0.60f)
        {
            p.ticketClass = "GEN";
            p.status = TicketStatus.GeneralTicket;
        }

        go.name = p.passengerName + " (Berth " + berth.number + ")";

        if (showDebug)
        {
            Debug.Log(go.name + " | " + p.status);
        }
    }
}