namespace LabAPI_MVC.Entities;

public class Referral
{
    public Guid? Id { get; set; }
    public Patient? Patient { get; set; }
    public DateTime? IssuedAt  { get; set; }
    public required List<Test> Tests { get; set; }
    public required List<Sample> Samples { get; set; }
    
    public decimal? Weight { get; set; }
    public decimal? Height { get; set; }
    public int? Sex { get; set; }
}