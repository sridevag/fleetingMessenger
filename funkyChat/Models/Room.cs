using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace funkyChat.Models
{
    [Table("Room")]
    public class Room
    {
        public int Id { get; set; }
        [Required]
        public int roomId { get; set; }

        [Required]
        public string keyPairHash { get; set; }

        [Required]
        public string roomTimeStamp { get; set; }

        public List<Message> messages = new List<Message>();
    }
}
