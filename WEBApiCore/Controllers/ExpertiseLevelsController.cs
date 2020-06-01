using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WEBApiCore.Models;

namespace WEBApiCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ExpertiseLevelsController : ControllerBase
    {
        private readonly ContactsDBContext _context;
      

        public ExpertiseLevelsController(ContactsDBContext context, IOptions<JWTSettings> jwtsettings)
        {
            _context = context;
            
        }

        // GET: api/BaseExpertiseLevels
        /// <summary>
        /// Retrieves all the available expertise levels that a contact can have for a skill
        /// </summary>
        [Authorize]
        [HttpGet]
        public IEnumerable<System.Object> GetExpertiseLev()
        {
            
            var result = (from a in _context.ExpertiseLev
                          select new
                          {
                              a.ExpertiseLvlid,
                              a.ExpertiseLevel

                          });
            return result;


        }
        /// <summary>
        /// Retrieves an expertise level record with the unique id
        /// </summary>
       
        
        /// <response code="404">The record with specific the id is not present in the DB</response>
       
        // GET: api/BaseExpertiseLevels/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExpertiseLev([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var expertiseLev = await _context.ExpertiseLev.FindAsync(id);

            if (expertiseLev == null)
            {
                return NotFound("The record with id "+id+" does not exist in the database");
            }
            var result =  new
                          {
                              expertiseLev.ExpertiseLvlid,
                              expertiseLev.ExpertiseLevel

                          };

            return Ok(result);
        }

        // PUT: api/BaseExpertiseLevels/5
        /// <summary>
        /// Updates an Expertise level record with the specific id ( Amin Token is needed )
        /// </summary>
        ///   <response code="403">You are forbidden from using this method</response>
        /// <response code="400">Bad request</response>
        /// <response code="404">The record with specific the specific id is not present in the DB</response>
        [Authorize(Roles = "Administrator")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExpertiseLev([FromRoute] int id, [FromBody] ExpertiseLvLEditModel expertiseL)
        {
            //In this api we are using this type of model that complicates the code, because swagger generates the
            // scheme in the documentation more precise with this one
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != expertiseL.ExpertiseLvlid)
            {
                return BadRequest("No ExpertiseLvLID is added to the input or the added one is wrong");
            }

            try
            {
                _context.ExpertiseLev.Update(new ExpertiseLvLModel()
                {
                    ExpertiseLvlid = expertiseL.ExpertiseLvlid,
                    ExpertiseLevel = expertiseL.ExpertiseLevel
                });
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                if (!ExpertiseLevExists(id))
                {
                    return NotFound("The record with id " + id + " does not exist in the database");
                }
                else
                {
                    return BadRequest(e.InnerException.Message);
                }
            }

            return Ok("Success");
        }
        /// <summary>
        /// With this api you can add an Expertise level  ( Amin Token is needed )
        /// </summary>

        /// <response code="400">Bad request</response>
        /// /// <response code="403">You are forbidden from using this method</response>
        /// <response code="404">The record with specific the id is not present in the DB</response>
        // POST: api/BaseExpertiseLevels
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> PostExpertiseLev([FromBody] ExpertiseLvLInputModel expertiseIn)
        {
            //In this api we are using this type of model that complicates the code, because swagger generates the
            // scheme in the documentation more precise with this one
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.ExpertiseLev.Add(new ExpertiseLvLModel()
            {
                ExpertiseLevel = expertiseIn.ExpertiseLevel
            }
                );
            try
            {
                
                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }

            

            return Ok("Success");
        }

        // DELETE: api/BaseExpertiseLevels/5
        /// <summary>
        /// With this api you can delete an Expertise level  ( Amin Token is needed )
        /// </summary>

        /// <response code="400">Bad request</response>
        ///  <response code="403">You are forbidden from using this method</response>
        /// <response code="404">The record with specific the id is not present in the DB</response>
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpertiseLev([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var expertiseLev = await _context.ExpertiseLev.FindAsync(id);
            if (expertiseLev == null)
            {
                return NotFound("The record with id " + id + " does not exist in the database");
            }

            _context.ExpertiseLev.Remove(expertiseLev);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }

            return Ok("The record with id:"+expertiseLev.ExpertiseLvlid+" is deleted");
        }

        private bool ExpertiseLevExists(int id)
        {
            return _context.ExpertiseLev.Any(e => e.ExpertiseLvlid == id);
        }
    }
}