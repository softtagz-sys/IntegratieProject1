namespace IP_MVC.Models
{
    public class AnswerViewModel
    {
        public int QuestionId { get; set; }
        public List<string> AnswerPlayer1 { get; set; } 
        public List<string> AnswerPlayer2 { get; set; }
        public bool NextOrPreviousButtonClicked { get; set; }
    }
}