namespace BL.Domain;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<Flow> Flows { get; set; }
    public string AdminId { get; set; }
    public List<ProjectFacilitator> ProjectFacilitators { get; set; }
    
    public Project(string name, List<Flow> flows, string description, string adminId)
    {
        Name = name;
        Flows = flows;
        Description = description;
        AdminId = adminId;
    }
    
    public Project(string name, string description)
    {
        Name = name;
        Description = description;
    }
    
    public Project()
    {
    }
}