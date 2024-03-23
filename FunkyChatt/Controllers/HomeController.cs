using funkyChat.Models;
using Microsoft.AspNetCore.Mvc;
using System.Buffers.Text;
using System.Diagnostics;
using System;
using funkyChat.Data;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;
using FunkyChatt.Models;
using System.Security.Cryptography;

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

        public (string, string) returnNewRSAPair()
        {
            RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider();
            string privateKey = rsaKey.ToXmlString(true);
            string publicKey = rsaKey.ToXmlString(false);

            //convert these xml to a simple single line string format and return them
            //store them into a database (redis)?
            //use a captcha so you generate them infrequently and avoid spam and storage waste

            return (privateKey, publicKey);
        }

        private static bool checkConnection(ApplicationDbContext _db)
        {
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

            var room = await _db.room.FirstOrDefaultAsync(x => x.keyPairHash == pairHash);

            if (room == null)
            {
                return NotFound();
            }

            return Ok(room);
        }

        public async Task<Room> createNewRoom(string k1, string k2)
        {
            var newRoom = new Room
            {
                keyPairHash = k1 + k2,
                roomTimeStamp = DateTime.Now.ToString("mmssffff")
            };

            _db.room.Add(newRoom);
            _db.SaveChanges();

            var room = await _db.room.FirstOrDefaultAsync(x => x.keyPairHash == k1+k2);
            return room;
}

        public IActionResult Index()
        {
            TempData["privateKey"] = "";
            TempData["publicKey"] = "";
            return View();
        }

        [HttpPost]
        public IActionResult createCredentials()
        {
            (TempData["privateKey"], TempData["publicKey"]) = returnNewRSAPair();
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> createRoom(string yurUsrKey, string othrUsrKey)
        {
            //Check if strings are base64 encoded with a certain length
            Debug.WriteLine("HERE!!!!");

            if (!(isBase64(othrUsrKey) && isBase64(yurUsrKey)) && (othrUsrKey != yurUsrKey))
                //return the view with a message of invalid key pair
                return RedirectToAction("Index", new { messageString = "enter valid key value pair" });

            if (!checkConnection(_db))
                return RedirectToAction("Index", new { messageString = "Can't connect to Database" });

            var result = await fetchRoom(yurUsrKey, othrUsrKey);
            Room room = null;

            if (result is OkObjectResult okObjectResult)
            {
                var res = okObjectResult.Value;
                room = (Room)res;
            }
            else if (result is NotFoundResult)
            {
                //create a new room from scratch 
                room = await createNewRoom(yurUsrKey, othrUsrKey);
            }

            return RedirectToAction("Room", new { Room = room });
        }

        public IActionResult Room(Room Room)
        {
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