using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KaraokeServices.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using KaraokeCoreObjects;

namespace KaraokeServices.Controllers
{
    [Produces("application/json")]
    [Route("api/Requests")]
    public class RequestController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public RequestController(ApplicationDbContext pContext)
        {
            dbContext = pContext;
        }

//        [Authorize(Roles="animation,admin")]
        [HttpGet]
        public ActionResult GetRequests()
        {
            var queuedRequests = dbContext.Playlists.Select(p => p.RequestId).ToList();

            var validRequests = dbContext.Requests
                .Include(r => r.Song).ThenInclude(s => s.Artist)
                .OrderBy(r => r.RequestTime)
                .Where(r => !queuedRequests.Contains(r.Id) && r.RequestTime >= new DateTime(2017, 11, 05, 12, 04, 16, DateTimeKind.Local)).ToList();

            return Ok(validRequests);
        }
        
        // GET: api/KaraokeState
        //        [Authorize(Roles="animation,admin")]
        [HttpGet("{id}")]
        public ActionResult GetRequest(int id)
        {
            Request request = dbContext.Requests
                .Include(r => r.Song)
                .ThenInclude(s => s.Artist)
                .Where(r => r.Id == id).FirstOrDefault();

            if (request == null)
            {
                return BadRequest("Request doesn't exist");
            }
            return Ok(request);
        }

        //        [Authorize(Roles="animation,admin")]
        [HttpPost]
        public ActionResult AddToRequestlist([FromBody] Request pRequest)
        {
            Song song = dbContext.Songs.Where(s => s.Id == pRequest.SongId).FirstOrDefault();
            if (song == null)
            {
                return BadRequest("Invalid Song");
            }

            if (pRequest.SingerName == null || pRequest.SingerName.Trim().Equals(""))
            {
                return BadRequest("Invalid Singer name");
            }

            Request request = new Request();
            request.RequestTime = DateTime.Now;
            request.SingerName = pRequest.SingerName;
            request.Notes = pRequest.Notes;
            request.Song = song;

            dbContext.Requests.Add(request);
            dbContext.SaveChanges();

            int requestId = request.Id;
            Request newRequest = dbContext.Requests
                .Include(r => r.Song).ThenInclude(s => s.Artist)
                .Where(r => r.Id == requestId).FirstOrDefault();

            return Ok(newRequest);
        }
    }
}