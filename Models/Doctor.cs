namespace Models;

public class Doctor
{ 
    public int Id { get; set; }

    public int UserId { get; set; }
    
    public User? User { get; set; }   // Navigation property 

    public string Phone { get; set; } = "" ;

    public string Specialty { get; set; } = "" ;

    public bool IsDisabled { get; set; }

    public DateTime CreateAt { get; set; }

    public string Picture { get; set; } = "" ;

    public string Bio { get; set; } = "" ;

    public string Certicate { get; set; } = "" ;

    public decimal TicketPrice { get; set; } 

    public bool Isverified { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<Schedule> Schedule { get; set; } = new();
}
