using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Zwitscher.Data;
using Zwitscher.Models;

namespace Zwitscher.Hubs
{
    public interface IUserClient
        //Client interface for possible SignalR Client
    {
        Task<string> GetconnectionId();
        void RetrieveUserList();
        void RetrieveUser();
        Task SendAsync(string method, string message);
    }
    public class UserHub : Hub<IUserClient>
    //Client for SignalRHub 
    //Not in use
    //Was for Apps notification but didnt work as intended
    {
        private readonly ZwitscherContext _context;
        public UserHub(ZwitscherContext context)
        {
                _context = context;
        }
        public void TestConnection(string Message)
        {
            string connectionId = Context.ConnectionId;
            Console.WriteLine(connectionId + ": " + Message);
            
            Console.WriteLine(_context.User.First().Username.ToString());
        }
        public List<User> GetUserList()
        {
            string connectionId = Context.ConnectionId;
            return _context.User.ToListAsync().Result;
        }

        public async Task newComment(string json)
            //Just for Testing Purposes
        {
            await Clients.All.SendAsync("newComment", json);
        }
    }
}
