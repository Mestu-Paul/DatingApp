using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class Connection
    {
        public Connection()
        {
        }

        public Connection(string connectedId, string username)
        {
            ConnectedId = connectedId;
            Username = username;
        }

        [Key]
        public string ConnectedId { get; set; }
        public string Username { get; set; }
    }
}