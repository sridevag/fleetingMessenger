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
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Npgsql;
using Microsoft.AspNetCore.Mvc.Diagnostics;

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


        public void initDictionaryKeysStartup(){

            var addDict = new hashDict
            {
                keyTimeStamp = DateTime.Now.ToString("mmssffff"),
                publicPrivatePairs=JsonConvert.SerializeObject(new Dictionary<string, string>()),
            };

            _db.hashDict.Add(addDict);
            _db.SaveChanges();

            return;
        }

        public async Task<IActionResult> fetchAndAddKeysToDictionary(string pub, string priv)
        {
            hashDict dict = await _db.hashDict.FirstOrDefaultAsync(x=>x.Id==4);
            
            if (dict == null)
            {
                return NotFound();
            }

            string keysDict = dict.publicPrivatePairs;
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(keysDict);

            dictionary.Add(pub, priv);

            var updatedKeysDict = JsonConvert.SerializeObject(dictionary);

            dict.publicPrivatePairs = updatedKeysDict;

            _db.Entry(dict).State = EntityState.Modified;

            await _db.SaveChangesAsync();

            return Ok(200);
        }

        public async Task<IActionResult> verifyKeysAndReturnPrivate(string pub)
        {
            hashDict dict = await _db.hashDict.FirstOrDefaultAsync(x => x.Id == 1);

            if (dict == null)
            {
                return NotFound();
            }

            string keysDict = dict.publicPrivatePairs;
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(keysDict);
            string resultingPrivateKey="";

            if (dictionary.ContainsKey(pub))
            {
                return NotFound();
            }
            else
            {
                resultingPrivateKey = dictionary[pub];
            }
            return Ok(resultingPrivateKey);
        }
        public async Task<(string, string)> ReturnNewRSAPair()
        {
            initDictionaryKeysStartup();
            RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider();
            string publicKeyBase64 = Convert.ToBase64String(rsaKey.ExportRSAPublicKey());
            string privateKeyBase64 = Convert.ToBase64String(rsaKey.ExportRSAPrivateKey());

            //use a captcha so you generate them infrequently and avoid spam and storage waste
            var result = await fetchAndAddKeysToDictionary(publicKeyBase64, privateKeyBase64);

            if (result is OkObjectResult okObjectResult){
                  return (privateKeyBase64, publicKeyBase64);
            }else if (result is NotFoundResult)
            {
                return ("", "");
            }
            return (publicKeyBase64, privateKeyBase64);
        }

        private static bool checkConnection(ApplicationDbContext _db)
        {
            bool isConnected = _db.Database.CanConnect();
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> createCredentials()
        {
            (TempData["privateKey"], TempData["publicKey"]) = await ReturnNewRSAPair();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> createRoom(string yurUsrKey, string othrUsrKey)
        {
            //Check if strings are base64 encoded with a certain length

            //replace this with checking if the pairs are in the dictionary 
            /*
            if (!(isBase64(othrUsrKey) && isBase64(yurUsrKey)) && (othrUsrKey != yurUsrKey))
                //return the view with a message of invalid key pair
                return RedirectToAction("Index", new { messageString = "enter valid key value pair" });
            */

            if (!checkConnection(_db))
                return RedirectToAction("Index", new { messageString = "Can't connect to Database" });

            var privateKey = await verifyKeysAndReturnPrivate(othrUsrKey);
            string otherUserPrivateKey = "";

            if (privateKey is OkObjectResult okObjectResult){
                var p = okObjectResult.Value;
                p = otherUserPrivateKey;
            }else if (privateKey is NotFoundResult){
                //create a new room from scratch 
                return RedirectToAction("Index", new { messageString = "Other user doesn't exist" });
            }

            var result = await fetchRoom(yurUsrKey, otherUserPrivateKey);
            Room room = null;

            //both yurUsrKey is your private key and othrUsrKey is the public key of the other user

            if (result is OkObjectResult okObjectResult2){
                var res = okObjectResult2.Value;
                room = (Room)res;
            }else if (result is NotFoundResult){
                //create a new room from scratch 
                room = await createNewRoom(yurUsrKey, otherUserPrivateKey);
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