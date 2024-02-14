using Business.Abstract;
using DataAccess.Abstract;
using DataAccess.Concrete;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Business.Concrete
{
    public class EntryManager : IEntryService
    {
        IEntryDal _entryDal;

        public EntryManager(IEntryDal entryDal)
        {
            _entryDal = entryDal;
        }

        public List<Entry> GetAll()
        {
            return _entryDal.GetAll();
        }

        public List<Entry> GetDate(DateTime date)
        {
            return new List<Entry>(_entryDal.GetAll(d => d.Tarih == date));
        }

        public List<Entry> GetPerson(int sicil)
        {
            return new List<Entry>(_entryDal.GetAll(d => d.Sicil == sicil));
        }
    }
}
