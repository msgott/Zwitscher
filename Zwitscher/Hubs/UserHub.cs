using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Zwitscher.Data;
using Zwitscher.Models;

namespace Zwitscher.Hubs
{
    public interface IUserClient
    {
        Task<string> GetconnectionId();
        void RetrieveUserList();
        void RetrieveUser();
    }
    public class UserHub : Hub<IUserClient>
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
    }
}
