using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRDemo.Models
{
    public class ChatHistory
    {
        public ChatHistory()
        {
            Created = DateTime.Now;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public DateTime Created { get; set; }
    }
}