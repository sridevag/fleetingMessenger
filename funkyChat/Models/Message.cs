using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace funkyChat.Models
{
    public class Message
    {
        public int Id { get; set; }


        //maybe store these on redis?
        [Required]
        public string message { get; set; } 

        [Required]
        public string roomTimeStamp = DateTime.Now.ToString("mmssffff");

        [Required]
        public string Ukey { get; set; }
    }
}
