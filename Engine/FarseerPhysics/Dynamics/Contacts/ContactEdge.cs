namespace FarseerPhysics.Dynamics.Contacts;

public sealed class ContactEdge
{
    public Contact Contact { get; set; }

    public ContactEdge Next { get; set; }

    public Body Other { get; set; }

    public ContactEdge Prev { get; set; }
}
