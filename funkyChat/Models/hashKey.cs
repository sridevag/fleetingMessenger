using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace funkyChat.Models
{
    public class hashKey
    {
        public int Id { get; set; }

        [Required]
        public string key { get; set; }

        public string keyTimeStamp = DateTime.Now.ToString("mmssffff");
    }
}
