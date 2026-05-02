namespace LabAPI_MVC.Entities;

public class Sample
{
    public Guid? Id { get; set; }
    public DateTime? IssuedAt { get; set; }
    public BioCase? BioCase { get; set; }
    public Referral? Referral { get; set; }
}