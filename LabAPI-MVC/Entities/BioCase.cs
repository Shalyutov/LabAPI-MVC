namespace LabAPI_MVC.Entities;

public class BioCase
{
    public int? Id { get; set; }
    public BioContainer? BioContainer { get; set; }
    public Supplier? Supplier { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}