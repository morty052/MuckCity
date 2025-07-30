using UnityEngine;

public class DelveTicketDispatcher : Interactable
{
    public override void Interact()
    {
        DelveTicket delveTicket = new(TicketTier.ON_WORLD);
        DelveManager.Instance.IssueDelveTicket(delveTicket);
    }
}
