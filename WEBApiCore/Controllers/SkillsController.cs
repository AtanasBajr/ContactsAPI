using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEBApiCore.Models;

namespace WEBApiCore.Controllers
{
    [Route("api")]
    [ApiController]
    public class SkillsController : ControllerBase
    {
        private readonly ContactsDBContext _context;

        public SkillsController(ContactsDBContext context)
        {
            _context = context;
        }

        // GET: api/Skills
        /// <summary>
        /// Retrieves all the available skills that a contact can have. For every skill, it retrieves a list of all the contacts who have this skill , also it gets the id of the expertise level they possess
        /// </summary>
        [Authorize]
        [Route("[controller]")]
        [HttpGet]
        public IEnumerable<System.Object> GetSkills()
        {
            var result = (from a in _context.Skills
                          select new
                          {
                              a.SkillId,
                              a.SkillName,
                              ContactsWhoPossessTheSkill = (from u in _context.ContactSkillExpertise
                                                            join c in _context.Contact on u.ContactId equals c.ContactId
                                                            where a.SkillId == u.SkillId
                                                            select new
                                                            {
                                                               
                                                                c.ContactId,
                                                                c.Fullname,
                                                                ContactSkillID = u.ContactSkillId,
                                                                u.ExpertiseLvlid
                                                            }
                                            )



                          }).ToList();
            return result;
        }

        // GET: api/Skills/5
        /// <summary>
        /// Retrieves a skill with an unique id, also it gets a list of the contacts who have the skill and the id of the expertise level of the skill they have
        /// </summary>
        /// <response code="400">Bad request</response>
        /// <response code="404">The record with specific the specific id is not present in the DB</response>
        [Authorize]
        [Route("[controller]/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetSkills([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var skills = await _context.Skills.FindAsync(id);

            if (skills == null)
            {
                return NotFound("The record with id " + id + " does not exist in the database");
            }
            var result = new {
                              skills.SkillId,
                              skills.SkillName,
                              ContactsWhoPossessTheSkill = (from u in _context.ContactSkillExpertise
                                                            join c in _context.Contact on u.ContactId equals c.ContactId
                                                            where skills.SkillId == u.SkillId
                                                            select new
                                                            {

                                                                c.ContactId,
                                                                c.Fullname,
                                                                ContactSkillID = u.ContactSkillId,
                                                                u.ExpertiseLvlid

                                                            }
                                            )



                          };
            return Ok(result);
        }

        // PUT: api/Skills/5
        /// <summary>
        /// It edits the skill with the id specified ( it requires an admin token),  
        /// </summary>
        /// <response code="400">Bad request</response>
        /// <response code="404">The record with specific the specific id is not present in the DB</response>
        [Route("[controller]/{id}")]
        [Authorize(Roles = "Administrator")]
        [HttpPut]
        public async Task<IActionResult> PutSkills([FromRoute] int id, [FromBody] EditSkillModel skills)
        {
            //In this api we are using this type of model that complicates the code, because swagger generates the
            // scheme in the documentation more precise with this one
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != skills.SkillId)
            {
                return BadRequest("No SkillID is added or the added one is wrong");
            }

            _context.Entry(new SkillModel()
            {
                SkillId = skills.SkillId,
                SkillName = skills.SkillName

            }).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                if (!SkillsExists(id))
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
        /// It modifies a record that keeps information about which contact has which skill ( if this method is called with a token without Administrator role, he can only edit the skiil to the contact that has the same id as the one added as a claim to the token)
        /// </summary>
        /// <response code="400">Bad request</response>
        /// <response code="404">The record with specific the specific id is not present in the DB</response>
        [Authorize]
        [Route("[controller]/ContactSkill/{ContactSkillID}")]
        [HttpPut]
        public async Task<IActionResult> EditSkillOfContact([FromRoute] int ContactSkillID, [FromBody] ContactSkillExpertiseModel skill)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                
                if (!(identity.Claims.FirstOrDefault().Value == skill.ContactId.ToString() || identity.Claims.FirstOrDefault().Value == "Administrator"))
                    return BadRequest("You do not have permission to access this api");

            }
            if (ContactSkillID != skill.ContactSkillId)
            {
                return BadRequest("No SkillID is added or the added one is wrong");
            }
           
            var valueForUpdate = _context.ContactSkillExpertise.FirstOrDefault(u => u.ContactId == skill.ContactId && u.SkillId == skill.SkillId);
            if (valueForUpdate != null)
            {
                if(valueForUpdate.ContactSkillId == skill.ContactSkillId)
                {
                    valueForUpdate.ExpertiseLvlid = skill.ExpertiseLvlid;
                    _context.ContactSkillExpertise.Update(valueForUpdate);
                    
                }
                else
                {
                    return BadRequest("The skill that you try to modify is already possessed by the user and it is not the record that you try to modify");
                }
                
            }
            else
            {
                return BadRequest("Wrong information provided to the request");
            }

           

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Success");
            }
            catch (Exception e)
            {

                return BadRequest(e.InnerException.Message);

            }

            
        }

        // POST: api/Skills
        /// <summary>
        /// It adds another skill ( which can be added to a contact by using an other method ( admin token is needed for this method)
        /// </summary>
        /// <response code="400">Bad request</response>
        [Authorize(Roles = "Administrator")]
        [Route("[controller]")]
        [HttpPost]
        public async Task<IActionResult> PostSkills([FromBody] InputSkillModel skill)
        {
            //In this api we are using this type of model that complicates the code, because swagger generates the
            // scheme in the documentation more precise with this one
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

           
            try
            {
                _context.Skills.Add(new SkillModel()
                {
                    
                    SkillName = skill.SkillName
                });
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }

            return Ok("Success");
        }
        /// <summary>
        /// It adds a skill to a contact ( this method can be called with or without administrator role, if it is called with a simple token, it will be able to add skills to contact that has the same id as the one added as a claim to the token)
        /// </summary>
        /// <response code="400">Bad request</response>
        /// <response code="404">The record with specific the specific id is not present in the DB</response>
        [Authorize]
        [Route("[controller]/ContactSkill")]
        [HttpPost]
        public async Task<IActionResult> AddSkillToContact([FromBody] ContactSkillExpertiseEditModel skill)
        {
            //In this api we are using this type of model that complicates the code, because swagger generates the
            // scheme in the documentation more precise with this one
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {

                if (!(identity.Claims.FirstOrDefault().Value == skill.ContactId.ToString() || identity.Claims.FirstOrDefault().Value == "Administrator"))
                    return BadRequest("You do not have permission to add a Skill to this Contact");

            }
           
            if (_context.ContactSkillExpertise.FirstOrDefault(u => u.ContactId == skill.ContactId && skill.SkillId == u.SkillId) == null)
                _context.ContactSkillExpertise.Add(new ContactSkillExpertiseModel() {
                    ContactId = skill.ContactId,
                    SkillId = skill.SkillId,
                    ExpertiseLvlid = skill.ExpertiseLvlid
                });
            else
            {
                return BadRequest("The skill is already possessed by the contact or no such skill is available for adding");
            }
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

        // DELETE: api/BaseSkills/5
        /// <summary>
        /// It deletes a Skill, removes this skill from all the contacts that possess it, also ( admin token is required for this method)
        /// </summary>
        /// <response code="400">Bad request</response>
        /// <response code="404">The record with specific the specific id is not present in the DB</response>
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSkills([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

           //var skills = from u in _context.ContactSkillExpertise
            //             where id == u.SkillId
            //             select u;
            //foreach(var item in skills)
            //{
            //    _context.ContactSkillExpertise.Remove(item);
            //}
            var itemToDelete = _context.Skills.FirstOrDefault(x => x.SkillId == id);
            if (itemToDelete == null)
            {
                return NotFound("The record with id "+id+" is not present in the DB");
            }
            
            _context.Skills.Remove(itemToDelete);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }

            return Ok("The record with id:" + id  + " is deleted Successfully");
        }
        /// <summary>
        /// It removes a skill from the possession of a contact ( with adminToken  you can delete any record, with normal token , only a record with the same ContactID as the one added as a Claim in the token)
        /// </summary>
        /// <response code="400">Bad request</response>
        /// <response code="404">The record with specific the specific id is not present in the DB</response>
        [Route("[controller]/ContactSkill/{contactID}&{skillID}")]
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteSkillFromContact([FromRoute] int contactID, [FromRoute] int skillID)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {

                if (!(identity.Claims.FirstOrDefault().Value == contactID.ToString() || identity.Claims.FirstOrDefault().Value == "Administrator"))
                    return BadRequest("You do not have permission to access this api");

            }
            ContactSkillExpertiseModel skill =  _context.ContactSkillExpertise.FirstOrDefault(x => x.SkillId == skillID && contactID == x.ContactId);
            if (skill == null)
            {
                return NotFound("Record Not found");
            }

            _context.ContactSkillExpertise.Remove(skill);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message);
            }

            return Ok("Success");
        }

        private bool SkillsExists(int id)
        {
            return _context.Skills.Any(e => e.SkillId == id);
        }
    }
}