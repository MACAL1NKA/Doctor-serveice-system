namespace Models;

public class Booking
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } 

    public int TimeSlotId { get; set; }

    public TimeSlot TimeSlots { get; set; } 

    public decimal PaidAmount { get; set; }

    public decimal Commission { get; set; }

    public decimal DoctorRevenue { get; set; }

    public string PaymentMethod { get; set; } = "";

    public int TransactionId { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime AppointmetTime { get; set; }

}