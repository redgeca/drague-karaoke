using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KaraokeServices.Data;
using Microsoft.EntityFrameworkCore;
using KaraokeCoreObjects;

namespace KaraokeServices.Controllers
{
    [Produces("application/json")]
    [Route("api/Playlist")]
    public class PlaylistController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public PlaylistController(ApplicationDbContext pContext)
        {
            dbContext = pContext;
        }

        //        [Authorize(Roles="animation,admin")]
        [HttpGet]
        public ActionResult GetPlaylists()
        {
            var actualPlaylist = dbContext.Playlists
                .Include(p => p.Request).ThenInclude(r => r.Song).ThenInclude(s => s.Artist)
                .Where(p => p.IsDone == 0).OrderBy(p => p.Order);

            return Ok(actualPlaylist);
        }

        //        [Authorize(Roles="animation,admin")]
        [HttpGet("{id}")]
        public ActionResult GetPlaylists(int id)
        {
            Playlist playlistEntry = dbContext.Playlists
                .Include(p => p.Request).ThenInclude(r => r.Song).ThenInclude(s => s.Artist)
                .Where(p => p.Id == id).FirstOrDefault();

            if (playlistEntry == null)
            {
                return BadRequest("Entry doesn't exist");
            }

            return Ok(playlistEntry);
        }


        //        [Authorize(Roles="animation,admin")]
        [HttpPost]
        public ActionResult AddToPlaylist(int id, [FromBody] int pPosition)
        {
            Request request = dbContext.Requests.Where(s => s.Id == id).FirstOrDefault();

            if (pPosition <= 0)
            {
                return BadRequest("Invalid position");
            }

            // Request desn't exist in playlist...  Return an error
            if (request == null)
            {
                return BadRequest("Request doesn't exist");
            }

            // Position cannot be greater than count play song + 1
            int totalCount = dbContext.Playlists.Where(p => p.IsDone == 0).Count();
            if (pPosition > totalCount + 1)
            {
                pPosition = totalCount + 1;
            }


            List<Playlist> orderPlaylist = dbContext.Playlists
                .Where(p => p.Order >= pPosition && p.IsDone == 0)
                .OrderBy(p => p.Order).ToList();

            Playlist entry = new Playlist();
            entry.RequestId = id;
            entry.Order = pPosition;
            dbContext.Playlists.Add(entry);

            foreach (Playlist song in orderPlaylist)
            {
                pPosition++;
                song.Order = pPosition;
            }
            dbContext.SaveChanges();

            entry = dbContext.Playlists
                .Include(p => p.Request)
                .ThenInclude(r => r.Song)
                .ThenInclude(s => s.Artist)
                .Where(p => p.Id == entry.Id).FirstOrDefault();
            return Ok(entry);
        }

        //        [Authorize(Roles="animation,admin")]
        [HttpDelete]
        public ActionResult MarkAsDone(int id)
        {
            Playlist entry = dbContext.Playlists
                .Include(p => p.Request)
                .Where(p => p.Id == id).FirstOrDefault();

            if (entry == null)
            {
                return BadRequest("Entry doesn't exist in plylist");
            }

            dbContext.Playlists.Remove(entry);
            dbContext.Requests.Remove(entry.Request);
            dbContext.SaveChanges();

            List<Playlist> orderPlaylist = dbContext.Playlists
                .Where(p => p.Order >= entry.Order && p.IsDone == 0)
                .OrderBy(p => p.Order).ToList();

            int newOrder = entry.Order;
            foreach (Playlist playEntry in orderPlaylist)
            {
                playEntry.Order = newOrder;
                newOrder++;
                dbContext.Playlists.Update(playEntry);
            }

            dbContext.SaveChanges();
            return Ok();
        }
    }
}