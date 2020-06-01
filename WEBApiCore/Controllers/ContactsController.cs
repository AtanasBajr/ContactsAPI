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
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly ContactsDBContext _context;

        public ContactsController(ContactsDBContext context)
        {
            _context = context;
        }

        //GET: api/Contacts
        /// <summary>
        /// With this api you can list all the contacts in the database with all the skills they possess
        /// </summary>
        /// <response code="400">Bad request</response>
        
        [Authorize]
        [HttpGet]
        public System.Object GetContact()
        {
            var result = (from a in _context.Contact
                          select new
                          {
                              a.ContactId,
                              a.Firstname,
                              a.Lastname,
                              a.Fullname,
                              a.Address,
                              a.Email,

                              a.MobileNum,
                              ContactSkillExpertise = (from u in _context.ContactSkillExpertise
                                        join b in _context.Skills on u.SkillId equals b.SkillId
                                        join c in _context.ExpertiseLev on u.ExpertiseLvlid equals c.ExpertiseLvlid
                                        where u.ContactId == a.ContactId
                                        select new
                                        {
                                            u.ContactSkillId,
                                            b.SkillId,
                                            b.SkillName,
                                            c.ExpertiseLvlid,
                                            c.ExpertiseLevel
                                        }
                                            )



        }).ToList();

           


            return result;
        }


        // GET: api/Contacts/5
        /// <summary>
        /// With this api you can list a contact ( with the specific id ) with all the skills he has
        /// </summary>
        /// <response code="400">Bad request</response>
        /// <response code="404">The record with specific the id is not present in the DB</response>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContact([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var a = await _context.Contact.FindAsync(id);
            if (a == null)
            {
                return NotFound("The record does not exist in the database");
            }
            var result = new
            {
                a.ContactId,
                a.Firstname,
                a.Lastname,
                a.Fullname,
                a.Address,
                a.Email,

                a.MobileNum,
                ContactSkillExpertise = (from u in _context.ContactSkillExpertise
                          join b in _context.Skills on u.SkillId equals b.SkillId
                          join c in _context.ExpertiseLev on u.ExpertiseLvlid equals c.ExpertiseLvlid
                          where u.ContactId == a.ContactId
                          select new
                          {
                              u.ContactSkillId,
                              b.SkillId,
                              b.SkillName,
                              c.ExpertiseLvlid,
                              c.ExpertiseLevel
                          }
                                            )
            };
            

            return Ok(result);
        }

        // PUT: api/Contacts/5
        /// <summary>
        /// With this api you can edit the information of a contact ( including the list of skills he has), if one has an admin token he can
        /// edit any contact, if not,  he can only edit one contact that has the same id as the one added as a claim to the token
        /// </summary>
        /// <response code="400">Bad request</response>
        /// <response code="404">The record with specific the id is not present in the DB</response>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContact([FromRoute] int id, [FromBody] ContactEditModel contact)
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
                IEnumerable<Claim> claims = identity.Claims;
                if (!(claims.FirstOrDefault().Value == id.ToString() || claims.FirstOrDefault().Value == "Administrator"))
                    return BadRequest("You do not have permission to edit this Contact");


            }
            if (id != contact.ContactId)
            {
                return BadRequest("No contactID added or the added one is wrong");
            }

            var existingSkills = from u in _context.ContactSkillExpertise
                                where u.ContactId == id 
                                select u;
            
            foreach (var item in existingSkills)
            {
                _context.ContactSkillExpertise.Remove(item);
            }




            foreach (var item in contact.ContactSkillExpertise)
            {
            
            if (_context.Skills.FirstOrDefault(x => x.SkillId == item.SkillId) == null)
            {
                return BadRequest("One of the skills you are trying to add or modify does not exist as a base skill");
            }
                ICollection<ContactSkillExpertiseModel> ContactSkillList = new HashSet<ContactSkillExpertiseModel>();
                foreach (var itemm in contact.ContactSkillExpertise)
                {
                    ContactSkillExpertiseModel ContactSkill = new ContactSkillExpertiseModel();
                    ContactSkill.ExpertiseLvlid = itemm.ExpertiseLvlid;
                    ContactSkill.SkillId = itemm.SkillId;
                    ContactSkillList.Add(ContactSkill);
                }
                _context.Contact.Update(new ContactModel()
                {
                    ContactId = contact.ContactId,
                    Firstname = contact.Firstname,
                    Lastname = contact.Lastname,
                    Fullname = contact.Fullname,
                    Email = contact.Email,
                    Address = contact.Address,
                    MobileNum = contact.MobileNum,
                    ContactSkillExpertise = ContactSkillList
                });
                
            
                


            }
            
            

            

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                if (!ContactExists(id))
                {
                    return NotFound("The record with id " + id + " does not exist in the database");
                }
                else
                {
                    return NotFound(e.InnerException.Message);
                }
            }

            return Ok("Success");
        }
        /// <summary>
        /// With this api you can add a contact together with a list of the skills he possess
        /// </summary>
        /// <response code="400">Bad request</response>
        /// <response code="404">The record with specific the id is not present in the DB</response>
        // POST: api/Contacts
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> PostContact([FromBody] ContactInputModel contact)
        {
            //In this api we are using this type of model that complicates the code, because swagger generates the
            // scheme in the documentation more precise with this one
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                ICollection<ContactSkillExpertiseModel> ContactSkillList = new HashSet<ContactSkillExpertiseModel>();
                
                foreach (var item in contact.ContactSkillExpertise)
                {
                    ContactSkillExpertiseModel ContactSkill = new ContactSkillExpertiseModel();
                    //ContactSkill.ContactId = item.ContactId;
                    ContactSkill.ExpertiseLvlid = item.ExpertiseLvlid;
                    ContactSkill.SkillId = item.SkillId;
                    ContactSkillList.Add(ContactSkill);
                }
                _context.Contact.Add(new ContactModel() {
                    Firstname = contact.Firstname,
                    Lastname = contact.Lastname,
                    Fullname = contact.Fullname,
                    Email = contact.Email,
                    Address = contact.Address,
                    MobileNum = contact.MobileNum,
                    ContactSkillExpertise = ContactSkillList
                });
                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                return BadRequest (e.InnerException.Message);
            }
            
            

            return Ok("Success");
        }

        // DELETE: api/Contacts/5
        /// <summary>
        /// With this api you can delete a contact with all the skills he possess, without admin token you can delete only one contact .. 
        /// </summary>
        /// <response code="400">Bad request</response>
        /// <response code="404">The record with specific the id is not present in the DB</response>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                if (!(claims.FirstOrDefault().Value == id.ToString() || claims.FirstOrDefault().Value == "Administrator"))
                    return BadRequest("You do not have permission to edit this Contact");


            }
            var contact = await _context.Contact.FindAsync(id);
            if (contact == null)
            {
                return NotFound("The item with id " + id + " is not present in the database");
            }
            var contactSkills = from u in _context.ContactSkillExpertise
                                where u.ContactId == id
                                select u;
            foreach (var item in contactSkills)
            {
                 _context.ContactSkillExpertise.Remove(item);
            }
            
            

            _context.Contact.Remove(contact);
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

        private bool ContactExists(int id)
        {
            return _context.Contact.Any(e => e.ContactId == id);
        }
    }
}