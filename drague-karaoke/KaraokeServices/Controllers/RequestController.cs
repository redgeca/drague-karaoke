using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KaraokeServices.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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
        // GET: api/KaraokeState
//        [Authorize(Roles="animation,admin")]
        [HttpGet]
        public ActionResult GetRequests()
        {
            var queuedRequests = dbContext.Playlists.Select(p => p.RequestId).ToList();

            var validRequests = dbContext.Requests
                .Include(r => r.Song).ThenInclude(s => s.Artist)
//                .Include(r => r.Song).ThenInclude(s => s.Category)
                .OrderBy(r => r.RequestTime)
                .Where(r => !queuedRequests.Contains(r.Id) && r.RequestTime >= new DateTime(2017, 11, 05, 12, 04, 16, DateTimeKind.Local)).ToList();

            return Ok(validRequests);
        }
    }
}