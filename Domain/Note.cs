namespace BL.Domain;

public class Note
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public int SessionId { get; set; }
    public int QuestionId { get; set; }
}