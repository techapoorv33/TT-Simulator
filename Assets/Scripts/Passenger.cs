using UnityEngine;

public enum TicketStatus
{
    Valid,
    NoTicket,
    WrongBerth,
    WrongDate,
    GeneralTicket
}

public class Passenger : MonoBehaviour
{
    public string passengerName;
    public string coachCode;     // coach where he is sitting
    public string coachName;     // for example Sleeper
    public int actualBerth;      // where the person is really sitting
    public bool hasTicket;
    public string pnr;
    public int ticketBerth;      // berth printed on the ticket
    public string ticketDate;
    public string ticketClass;   // SL, 3A, 2A, 1A, CC, EC, GEN
    public TicketStatus status;  // the hidden truth
}