namespace LabAPI_MVC.Entities;

public class LabResult
{
    public int WorkItemId { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public List<ResultDetail> ResultDetails { get; set; } = [];
}