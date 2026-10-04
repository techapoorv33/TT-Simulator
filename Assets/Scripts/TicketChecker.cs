using UnityEngine;
using System.Collections.Generic;

public class TicketChecker : MonoBehaviour
{
    public Camera playerCamera;
    public float checkDistance = 3.5f;

    public int money = 0;
    public int karma = 50;
    public int checkedCount = 0;

    private FirstPersonController movement;
    private Passenger lookingAt;
    private Passenger current;
    private bool popupOpen = false;
    private string message = "";
    private float messageTimer = 0f;
    private HashSet<Passenger> done = new HashSet<Passenger>();

    private GUIStyle titleStyle;
    private GUIStyle textStyle;
    private GUIStyle buttonStyle;
    private GUIStyle hudStyle;

    void Start()
    {
        movement = GetComponent<FirstPersonController>();
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        if (messageTimer > 0f) messageTimer -= Time.deltaTime;

        if (popupOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) ClosePopup();
            return;
        }

        lookingAt = null;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, checkDistance))
        {
            lookingAt = hit.collider.GetComponent<Passenger>();
        }

        if (lookingAt != null && Input.GetMouseButtonDown(0))
        {
            OpenPopup(lookingAt);
        }
    }

    void OpenPopup(Passenger p)
    {
        current = p;
        popupOpen = true;
        if (movement != null) movement.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ClosePopup()
    {
        popupOpen = false;
        current = null;
        if (movement != null) movement.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    int FineAmount(TicketStatus s)
    {
        switch (s)
        {
            case TicketStatus.NoTicket: return 500;
            case TicketStatus.WrongDate: return 400;
            case TicketStatus.GeneralTicket: return 250;
            case TicketStatus.WrongBerth: return 100;
        }
        return 0;
    }

    void Resolve(string action)
    {
        Passenger p = current;
        bool bad = p.status != TicketStatus.Valid;
        int dm = 0;
        int dk = 0;
        string msg = "";
        bool removePassenger = false;

        switch (action)
        {
            case "fine":
                if (!bad) { dk = -10; msg = "His ticket was valid! You fined an honest passenger."; }
                else { dm = FineAmount(p.status); dk = 5; msg = "Fine collected: Rs " + dm + "."; }
                break;

            case "bribe":
                if (!bad) { dk = -10; msg = "You asked an honest passenger for a bribe. He was angry."; }
                else
                {
                    dm = 500; dk = -15; msg = "You took Rs 500 in secret.";
                    if (Random.value < 0.25f)
                    {
                        dm = -1000; dk = -20;
                        msg = "A vigilance officer saw you! Penalty: Rs 1000.";
                    }
                }
                break;

            case "police":
                removePassenger = true;
                if (!bad) { dk = -15; msg = "The police found nothing. You wasted their time."; }
                else { dk = 3; msg = "The police took " + p.passengerName + " away."; }
                break;

            case "throw":
                removePassenger = true;
                if (!bad) { dk = -25; msg = "You threw out an innocent passenger!"; }
                else { dk = -5; msg = "You threw " + p.passengerName + " out at the next station."; }
                break;

            case "general":
                removePassenger = true;
                if (p.status == TicketStatus.GeneralTicket) { dk = 8; msg = "Correct! He was sent to the General coach."; }
                else if (!bad) { dk = -15; msg = "He had a valid Sleeper ticket!"; }
                else { dk = -2; msg = "He was sent to General, but that was not the real problem."; }
                break;

            case "move":
                if (p.status == TicketStatus.WrongBerth)
                {
                    dk = 8; msg = "He moved to his own berth (" + p.ticketBerth + ").";
                }
                else if (!bad) { dk = -5; msg = "He was already in the right place."; }
                else { dk = -2; msg = "Moving him did not solve the problem."; }
                break;

            case "let":
                if (!bad) { dk = 5; msg = "Good call. Nothing was wrong."; }
                else { dk = -5; msg = "You let someone with a problem go."; }
                break;
        }

        money += dm;
        karma = Mathf.Clamp(karma + dk, 0, 100);
        checkedCount++;
        done.Add(p);
        message = msg;
        messageTimer = 5f;

        if (removePassenger) Destroy(p.gameObject);
        ClosePopup();
    }

    void MakeStyles()
    {
        if (titleStyle != null) return;

        titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize = 26;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = Color.white;

        textStyle = new GUIStyle(GUI.skin.label);
        textStyle.fontSize = 20;
        textStyle.normal.textColor = Color.white;

        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 20;

        hudStyle = new GUIStyle(GUI.skin.label);
        hudStyle.fontSize = 22;
        hudStyle.fontStyle = FontStyle.Bold;
        hudStyle.normal.textColor = Color.white;
    }

    void OnGUI()
    {
        MakeStyles();

        float s = Screen.height / 720f;
        GUI.matrix = Matrix4x4.Scale(new Vector3(s, s, 1f));
        float sw = Screen.width / s;

        // Stats at the top left
        GUI.Label(new Rect(15, 10, 500, 30), "Money: Rs " + money, hudStyle);
        GUI.Label(new Rect(15, 40, 500, 30), "Karma: " + karma, hudStyle);
        GUI.Label(new Rect(15, 70, 500, 30), "Checked: " + checkedCount, hudStyle);

        if (!popupOpen)
        {
            // Crosshair
            GUI.Label(new Rect(sw / 2f - 10, 350, 40, 40), "+", titleStyle);

            if (lookingAt != null)
            {
                string txt = done.Contains(lookingAt)
                    ? "Already checked"
                    : "Click to check ticket";
                GUI.Label(new Rect(sw / 2f - 150, 400, 400, 40), txt, textStyle);
            }

            if (messageTimer > 0f)
            {
                GUI.Label(new Rect(sw / 2f - 400, 640, 900, 40), message, textStyle);
            }
            return;
        }

        // Ticket popup
        Rect box = new Rect(sw / 2f - 260, 40, 520, 640);
        GUI.Box(box, "");
        GUILayout.BeginArea(new Rect(box.x + 20, box.y + 15, 480, 610));

        GUILayout.Label("E-TICKET", titleStyle);
        GUILayout.Space(5);

        if (!current.hasTicket)
        {
            GUILayout.Label("He shows NO ticket.", textStyle);
        }
        else
        {
            GUILayout.Label("Name: " + current.passengerName, textStyle);
            GUILayout.Label("PNR: " + current.pnr, textStyle);
            GUILayout.Label("Class: " + current.ticketClass, textStyle);
            GUILayout.Label("Berth: " + current.ticketBerth, textStyle);
            GUILayout.Label("Date: " + current.ticketDate, textStyle);
        }

        GUILayout.Space(10);
        GUILayout.Label("Today: " + System.DateTime.Now.ToString("dd-MM-yyyy"), textStyle);
        GUILayout.Label("This coach: Sleeper (SL)", textStyle);
        GUILayout.Label("He is sitting on: Berth " + current.actualBerth, textStyle);
        GUILayout.Space(10);

        if (done.Contains(current))
        {
            GUILayout.Label("Already dealt with.", textStyle);
        }
        else
        {
            if (GUILayout.Button("Take fine (challan)", buttonStyle, GUILayout.Height(36))) Resolve("fine");
            if (GUILayout.Button("Take bribe (risky)", buttonStyle, GUILayout.Height(36))) Resolve("bribe");
            if (GUILayout.Button("Call police", buttonStyle, GUILayout.Height(36))) Resolve("police");
            if (GUILayout.Button("Throw out at next station", buttonStyle, GUILayout.Height(36))) Resolve("throw");
            if (GUILayout.Button("Send to General coach", buttonStyle, GUILayout.Height(36))) Resolve("general");
            if (GUILayout.Button("Ask to move to correct berth", buttonStyle, GUILayout.Height(36))) Resolve("move");
            if (GUILayout.Button("Let him go", buttonStyle, GUILayout.Height(36))) Resolve("let");
        }

        GUILayout.Space(8);
        if (GUILayout.Button("Close (Esc)", buttonStyle, GUILayout.Height(36))) ClosePopup();

        GUILayout.EndArea();
    }
}
