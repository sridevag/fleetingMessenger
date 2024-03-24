using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;



namespace funkyChat.Models
{
    public class hashKey
    {
        public int Id { get; set; }

        //public string ppPair = JsonConvert.SerializeObject(new Dictionary<string, string>());

        [Required]
        public string key { get; set; }

        public string keyTimeStamp = DateTime.Now.ToString("mmssffff");
    }
}
