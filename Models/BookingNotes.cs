namespace Models;

public class BookingNotes
{ 
    public int Id { get; set; }

    public int BookingId { get; set; }
    public Booking Bookings { get; set; } = new();

    public string Note { get; set; } = "";

    public DateTime CreateAt { get; set; } 

}
