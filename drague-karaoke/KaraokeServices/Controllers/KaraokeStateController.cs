using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KaraokeServices.Data;
using KaraokeCoreObjects.misc;
using KaraokeCoreObjects;
using Microsoft.AspNetCore.Authorization;

namespace KaraokeServices.Controllers
{
    [Produces("application/json")]
    [Route("api/KaraokeState")]
    public class KaraokeStateController : Controller
    {

        private readonly ApplicationDbContext dbContext;

        public KaraokeStateController(ApplicationDbContext pContext)
        {
            dbContext = pContext;
        }
        // GET: api/KaraokeState
        [HttpGet]
        public ActionResult Get()
        {
            KaraokeConfiguration state = dbContext.KaraokeConfiguration.Where(c => c.key == Constants.KARAOKE_STATE_FLAG).FirstOrDefault();
            if (state != null)
            {
                return Ok(state);
            }
            else
            {
                KaraokeConfiguration initialState = new KaraokeConfiguration();
                initialState.key = Constants.KARAOKE_STATE_FLAG;
                initialState.value = Constants.STOPPED_FLAG;
                dbContext.KaraokeConfiguration.Add(initialState);
                dbContext.SaveChanges();
                state = initialState;
            }
            return Ok(state);
        }

        // PUT: api/KaraokeState
//        [Authorize(Roles = "animation,admin")]
        [HttpPut]
        public ActionResult Put([FromBody] string pState)
        {
            if (Constants.RUNNING_FLAG.Equals(pState) || Constants.STOPPED_FLAG.Equals(pState))
            {
                KaraokeConfiguration state = dbContext.KaraokeConfiguration.Where(c => c.key == Constants.KARAOKE_STATE_FLAG).FirstOrDefault();
                state.value = pState;

                // Log last startup date and time...
                if (Constants.RUNNING_FLAG.Equals(state.value))
                {
                    KaraokeConfiguration existingDate = dbContext.KaraokeConfiguration.Where(c => c.key == Constants.LAST_STARTED_FLAG).FirstOrDefault();
                    if (existingDate == null)
                    {
                        KaraokeConfiguration lastChange = new KaraokeConfiguration();
                        lastChange.key = Constants.LAST_STARTED_FLAG;
                        lastChange.value = String.Format("{0:yyyy-MM-dd hh:mm:ss}", DateTime.Now);
                        dbContext.KaraokeConfiguration.Add(lastChange);
                    } else
                    {
                        existingDate.value = String.Format("{0:yyyy-MM-dd hh:mm:ss}", DateTime.Now);
                        dbContext.KaraokeConfiguration.Update(existingDate);
                    }
                }
                dbContext.SaveChanges();
                return Ok(state);    
            }
            return BadRequest("Invalid State");
        }
        
    }
}
