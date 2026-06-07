namespace LabAPI_MVC.Entities;

public class ResultDetail
{
    public WorkItem WorkItem { get; set; }
    public int Indicator { get; set; }
    public bool? BoolValue { get; set; }
    public string? StringValue { get; set; }
    public decimal? DecimalValue { get; set; }
}