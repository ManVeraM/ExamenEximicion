namespace ExamenEximicion.RegistroInaiso.Models;
using System.ComponentModel.DataAnnotations;
public class Reservation
{
    public long User_Id { get; set; }
    public string? User_name { get; set; }
    
    public long App_Id { get; set; }
    public string? App_name { get; set; }
    [Key]
    public DateTime ReservedAt { get; set; }
}