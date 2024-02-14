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


        public TimeSpan CalculateWorkingHours(DateTime date, string sicil)
        {
            DateTime startDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0); // Tarihin başlangıcı (00:00:00)
            DateTime endDate = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59); // Tarihin sonu (23:59:59)

            List<Entry> entries = _entryDal.GetAll(d => d.Tarih >= startDate && d.Tarih <= endDate && d.Sicil == sicil);

            List<DateTime> entryTimes = new List<DateTime>();
            List<DateTime> exitTimes = new List<DateTime>();

            foreach (var entry in entries)
            {
                int yonValue;
                if (Int32.TryParse(entry.Yon, out yonValue)) // Yon değerini integer'a dönüştür
                {
                    if (yonValue == 1) // Giriş
                    {
                        entryTimes.Add(entry.Tarih);
                    }
                    else if (yonValue == 0) // Çıkış
                    {
                        exitTimes.Add(entry.Tarih);
                    }
                }
                else
                {
                    // Yon değeri uygun bir şekilde dönüştürülemedi, uygun bir hata işleme mekanizması geliştirmelisiniz
                }
            }

            if (entryTimes.Count == 0 || exitTimes.Count == 0)
            {
                // Giriş veya çıkış bulunamadı, uygun bir hata işleme mekanizması geliştirmelisiniz
                return TimeSpan.Zero;
            }

            // En erken giriş ve en son çıkışı bulun
            DateTime firstEntryTime = entryTimes.Min();
            DateTime lastExitTime = exitTimes.Max();

            TimeSpan totalWorkingHours = lastExitTime - firstEntryTime;
            return totalWorkingHours;
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

        public List<Entry> GetEntriesForDateAndPerson(DateTime date, string sicil)
        {
            DateTime startDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0); // Tarihin başlangıcı (00:00:00)
            DateTime endDate = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59); // Tarihin sonu (23:59:59)


            // Belirtilen tarih aralığı ve sicil numarasına göre giriş çıkış verilerini filtreleyin
            return new List<Entry>(_entryDal.GetAll(d => d.Tarih >= startDate && d.Tarih <= endDate && d.Sicil == sicil));
        }

        public List<Entry> GetPerson(string sicil)
        {
            return new List<Entry>(_entryDal.GetAll(d => d.Sicil == sicil));
        }
    }
}
