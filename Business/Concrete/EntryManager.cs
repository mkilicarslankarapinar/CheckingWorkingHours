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

        public int CalcutaleWorkingHours()
        {
            throw new NotImplementedException();
        }

        public List<Entry> GetAll()
        {
            return _entryDal.GetAll();
        }

        public List<Entry> GetEntriesForDate(DateTime date)
        {
            DateTime startDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            DateTime endDate = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
            return new List<Entry>(_entryDal.GetAll(d => d.Tarih >= startDate && d.Tarih <= endDate));
        }

        public List<Entry> GetEntriesForDateAndPerson(DateTime date, int sicil)
        {
            DateTime startDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0); // Tarihin başlangıcı (00:00:00)
            DateTime endDate = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59); // Tarihin sonu (23:59:59)


            // Belirtilen tarih aralığı ve sicil numarasına göre giriş çıkış verilerini filtreleyin
            return new List<Entry>(_entryDal.GetAll(d => d.Tarih >= startDate && d.Tarih <= endDate && d.Sicil == sicil));
        }

        public List<Entry> GetPerson(int sicil)
        {
            return new List<Entry>(_entryDal.GetAll(d => d.Sicil == sicil));
        }
    }
}
