using UnityEngine;

public enum TicketTier
{
    ON_WORLD,
    RECON
}

public class DelveTicket
{
    public TicketTier _ticketTier = TicketTier.ON_WORLD;
    public DelveTicket(TicketTier ticketTier)
    {
        _ticketTier = ticketTier;
    }

}
