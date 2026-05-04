namespace LabAPI_MVC.Entities;

public class WorkItem
{
    public int? Id { get; set; }
    public required Referral Referral { get; set; }
    public required Test Test { get; set; }
    public required Sample Sample { get; set; }
    public required Equipment Equipment { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? Processed { get; set; }
    public DateTime? Canceled { get; set; }
}