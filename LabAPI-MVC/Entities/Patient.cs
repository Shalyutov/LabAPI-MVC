namespace LabAPI_MVC.Entities;

public class Patient
{
    public Guid? Id { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Document { get; set; }
    public DateOnly BirthDate { get; set; }
}