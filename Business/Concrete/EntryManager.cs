using Business.Abstract;
using DataAccess.Abstract;
using Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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

            // Giriş ve çıkışları zamana göre sırala
            entries = entries.OrderBy(e => e.Tarih).ToList();

            // Eğer ilk kayıt çıkışsa veya son kayıt girişse bu kayıtları listeden çıkar
            while (entries.Count > 0 && entries[0].Yon == "0")
            {
                entries.RemoveAt(0); // İlk kaydı çıkar
            }

            while (entries.Count > 0 && entries[entries.Count - 1].Yon == "1")
            {
                entries.RemoveAt(entries.Count - 1); // Son kaydı çıkar
            }

            List<TimeSpan> workPeriods = new List<TimeSpan>();

            DateTime? entry = null;
            DateTime? exit = null;
            bool skipNext = false;

            for (int i = 0; i < entries.Count; i++)
            {
                var currentEntity = entries[i];
                var nextEntity = entries.Count == i+1 ? null : entries[i + 1];

                if (currentEntity.Yon == nextEntity?.Yon)
                {
                    if (currentEntity.Yon == "1" && !skipNext)
                    {
                        entry = currentEntity?.Tarih;
                        skipNext = true;
                    }

                    continue;
                }
                else if (skipNext)
                {
                    if (entry == null)
                    {
                        entry = currentEntity?.Tarih;
                    }

                    skipNext = false;
                    continue;
                }
                else
                {
                    if (currentEntity.Yon == "1" && entry == null)
                    {
                        entry = currentEntity?.Tarih;
                    }

                    if (currentEntity.Yon == "0" && exit == null)
                    {
                        exit = currentEntity?.Tarih;
                    }

                    skipNext = false;
                }

                if (entry != null && exit != null && !skipNext)
                {
                    workPeriods.Add(exit.Value - entry.Value);
                    entry = null;
                    exit = null;
                    skipNext = false;
                }
            }

            // Tüm çalışma sürelerini topla
            TimeSpan totalWorkingHours = TimeSpan.Zero;
            foreach (var period in workPeriods)
            {
                totalWorkingHours += period;
            }

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
