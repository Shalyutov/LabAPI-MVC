namespace LabAPI_MVC.Entities;

public class BioContainer
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Biomaterial? Biomaterial { get; set; }
}