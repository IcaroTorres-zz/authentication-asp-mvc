using System;

namespace App.Models
{
    public class MessageDisplayModel
    {
        public MessageDisplayModel()
        {
            Title = "Application warning";
            Message = "Error executing an action.";
            Date = DateTime.Now;
            Color = "secondary";
            TextColor = "light";
        }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string Color { get; set; }
        public string TextColor { get; set; }
    }
}