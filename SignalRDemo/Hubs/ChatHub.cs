using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalRDemo.Models;
using System.Threading;

namespace SignalRDemo.Hubs
{
    public class ChatHub : Hub
    {
        public void Send(string name, string message)
        {
            // ADD THE CHAT NAME AND MESSAGE TO THE HISTORY
            ApplicationDbContext db = new ApplicationDbContext();

            var historyItem = new ChatHistory();
            historyItem.Message = message;
            historyItem.Name = name;
            db.History.Add(historyItem);
            db.SaveChanges();

            // PUBLISH THE MESSAGE TO ALL CONNECTED BROWSERS
            Clients.All.addNewMessageToPage(name, message);
        }

        // LISTEN FOR THE CLIENT PUBLISHING 'RESETMESSAGES'
        public void ResetMessages()
        {
            // DELETE MESSAGE HISTORY
            var db = new ApplicationDbContext();
            var allItems = db.History.ToList();

            foreach (var item in allItems)
            {
                db.History.Remove(item);
            }

            db.SaveChanges();

            // PUBLISH THE RESET MESSAGE TO ALL CLIENTS
            Clients.All.resetMessageBroadcast(Context.ConnectionId);
        }

        // LISTEN FOR THE CLIENT PUBLISHING 'LOADHISTORY'
        public void LoadHistory()
        {
            // GET ALL CHATS FROM THE DB
            var db = new ApplicationDbContext();
            List<ChatHistory> allHistory = db.History.OrderBy(x => x.Created).ToList();

            // ITERATE THROUGH EACH CHAT
            foreach (var history in allHistory)
            {
                // CALL THE EXISTNG 'ADDNEWMESSAGETOPAGE' METHOD TO POPULATE THE HISTORY IN THE UI
                // NOTICE THAT WE ARE ONLY PUBLISHING THIS TO THE CALLER (NOT ALL)
                Clients.Caller.addNewMessageToPage(history.Name, history.Message);
                
                // THIS NEXT LINE WOULD NOT BE IN PRODUCTION CODE BUT
                // DEMONSTRATES CLEARLY THAT THE EVENTS ARE FIRED SEPARATELY BY 
                // INSERTING A 500 MS DELAY BETWEEN PUBLISHES
                Thread.Sleep(500);
            }
        }
    } 
}