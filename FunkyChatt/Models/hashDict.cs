using funkyChat.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace FunkyChatt.Models
{
    public class hashDict
    {
        public int Id { get; set; }

        //it might be a problem with read times or just straight up annoying but each hashKey has its own time stamp <--- extremely inefficient if you ask me
        [Required]
        public string publicPrivatePairs { get; set; } //= JsonConvert.SerializeObject(new Dictionary<string, string>());

        [Required]
        public string keyTimeStamp { get; set; } // DateTime.Now.ToString("mmssffff");



    }
}
