namespace BL.Domain;

public class Media
{
    public int id {get; set;}
    
    public string url {get; set;}
    
    public string description {get; set;}
    
    public MediaType type {get; set;}
    
    public Media()
    {
    }
    
    public Media(string url, string description, MediaType type)
    {
        this.url = url;
        this.description = description;
        this.type = type;
    }
}