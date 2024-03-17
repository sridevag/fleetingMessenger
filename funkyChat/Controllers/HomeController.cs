using funkyChat.Models;
using Microsoft.AspNetCore.Mvc;
using System.Buffers.Text;
using System.Diagnostics;
using System;
using funkyChat.Data;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;

namespace funkyChat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db) 
        {
            _db = db;
            _logger = logger;
        }

        private static bool checkConnection(ApplicationDbContext _db) {
            bool isConnected = _db.Database.CanConnect();

            Debug.WriteLine("HELLO MARIO ME MARIO LUIGI");
            Debug.WriteLine(isConnected);

            if (isConnected)
                return true;

            return false;
        }

        private static bool isBase64(string input)
        {
            Debug.WriteLine(input.Length);
            if (input.Length != 88)
                return false;

            try
            {
                byte[] buffer = Convert.FromBase64String(input);
                return true; 
            }
            catch (FormatException) { return false; }
        }

        public async Task<IActionResult> fetchRoom(string k1, string k2)
        {

            string pairHash = k1 + k2;

            Debug.WriteLine("WHATS UP FREAK MOTHAFUKAS");

            var room = await _db.room.FirstOrDefaultAsync(x => x.keyPairHash == pairHash);
            
            if (room == null) {
                return NotFound();
            }

            return Ok(room);
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> createRoom(string yurUsrKey, string othrUsrKey)
        {
            //Check if strings are base64 encoded with a certain length
            Debug.WriteLine("HERE!!!!");

            if (!(isBase64(othrUsrKey) && isBase64(yurUsrKey)) && (othrUsrKey != yurUsrKey))
                //return the view with a message of invalid key pair
                return RedirectToAction("Index", new { messageString = "enter valid key value pair" });
            /*
                        if (!checkConnection(_db))
                            return RedirectToAction("Index", new { messageString = "Can't connect to Database" });
            */

            var result = await fetchRoom(yurUsrKey, othrUsrKey);
            Room room = null;

            if(result is OkObjectResult okObjectResult)
            {
                var res = okObjectResult.Value;
                room = (Room)res;
            }else if(result is NotFoundResult)
            {
                //create a new room from scratch 
                Debug.WriteLine("YOOOOO");
            }

            Debug.WriteLine("WE IN THE CLUB WE IN THE CITY WE INT HE CITY FUCK FUCK FUCK");
            Debug.WriteLine(room);

            return RedirectToAction("Room", new { Room = room });
        }

        public IActionResult Room(Room Room)
        {
            //is this where real time shit happens?
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}