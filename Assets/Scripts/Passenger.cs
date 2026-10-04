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
    public int actualBerth;      // where the person is really sitting
    public bool hasTicket;
    public string pnr;
    public int ticketBerth;      // berth printed on the ticket
    public string ticketDate;
    public string ticketClass;   // SL = Sleeper, GEN = General
    public TicketStatus status;  // the hidden truth
}