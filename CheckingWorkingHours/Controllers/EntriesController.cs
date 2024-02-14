using Business.Abstract;
using Business.Concrete;
using DataAccess.Concrete;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CheckingWorkingHours.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntriesController : ControllerBase
    {
        IEntryService _entryService;

        public EntriesController(IEntryService entryService)
        {
            _entryService = entryService;
        }

        [HttpGet("getall")]
        public List<Entry> Get()
        {
            IEntryService entryService = new EntryManager(new EfEntryDal());
            var result = entryService.GetAll();
            return result;

        }

        [HttpGet("getentriesfordate")]
        public List<Entry> Get(DateTime date)
        {
            IEntryService entryService = new EntryManager(new EfEntryDal());
            var result = entryService.GetEntriesForDate(date);
            return result;

        }

        [HttpGet("getentriesfordateandsicil")]
        public List<Entry> GetEntriesForDateAndSicil(DateTime date, int sicil)
        {
            IEntryService entryService = new EntryManager(new EfEntryDal());
            var result = entryService.GetEntriesForDateAndPerson(date, sicil);
            return result;

        }

        [HttpGet("getperson")]
        public List<Entry> Get(int sicil)
        {
            IEntryService entryService = new EntryManager(new EfEntryDal());
            var result = entryService.GetPerson(sicil);
            return result;

        }

        //[HttpGet("getall")]
        //public IActionResult Get()
        //{
        //    var result = _entryService.GetAll();
        //    if (result != null)
        //    {
        //        return Ok(result);
        //    }
        //    return BadRequest();

        //}
    }
}
