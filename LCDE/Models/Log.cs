namespace LCDE.Models
{
    public class Log
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime Date { get; set; }
    }
}
