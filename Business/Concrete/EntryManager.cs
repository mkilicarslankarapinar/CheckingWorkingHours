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

        // Ay bazında bütün kişilerin aylık toplam çalışma sürelerini hesaplar.
        public Dictionary<string, TimeSpan> CalculateMonthlyWorkingHours(DateTime date)
        {
            Dictionary<string, TimeSpan> monthlyWorkingHours = new Dictionary<string, TimeSpan>();

            // Ayın ilk günü
            DateTime startDate = new DateTime(date.Year, date.Month, 1);

            // Ayın son günü
            DateTime endDate = startDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

            // Tüm kayıtları çek
            List<Entry> entries = _entryDal.GetAll(d => d.Tarih >= startDate && d.Tarih <= endDate);

            // Her bir sicil için çalışma saatlerini hesapla
            foreach (var sicil in entries.Select(entry => entry.Sicil).Distinct())
            {
                // Sicile ait kayıtları filtrele
                List<Entry> sicilEntries = entries.Where(entry => entry.Sicil == sicil).ToList();

                // Gün bazında toplam çalışma süresi hesapla
                TimeSpan totalWorkingHours = TimeSpan.Zero;
                DateTime currentDate = startDate;

                while (currentDate <= endDate)
                {
                    // Günün başlangıcı
                    DateTime currentDayStart = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 0, 0, 0);
                    // Günün bitişi
                    DateTime currentDayEnd = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 23, 59, 59);

                    // Gün içindeki kayıtları al
                    List<Entry> dayEntries = sicilEntries.Where(entry => entry.Tarih >= currentDayStart && entry.Tarih <= currentDayEnd).ToList();

                    // Gün içindeki çalışma süresini hesapla ve toplam çalışma süresine ekle
                    TimeSpan dailyWorkingHours = CalculateDailyWorkingHours(dayEntries);
                    totalWorkingHours += dailyWorkingHours;

                    // Bir sonraki güne geç
                    currentDate = currentDate.AddDays(1);
                }

                // Sicilin toplam çalışma süresini ekle
                monthlyWorkingHours.Add(sicil, totalWorkingHours);
            }

            return monthlyWorkingHours;
        }

        // Gün içindeki çalışma süresini hesaplayan yardımcı metod
        private TimeSpan CalculateDailyWorkingHours(List<Entry> entries)
        {
            // Giriş ve çıkışları zamana göre sırala
            entries = entries.OrderBy(e => e.Tarih).ToList();

            // İlk giriş çıkışları kontrol et
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

            foreach (var currentEntity in entries)
            {
                var nextEntity = entries.FirstOrDefault(e => e.Tarih > currentEntity.Tarih);

                if (currentEntity.Yon == nextEntity?.Yon)
                {
                    if (currentEntity.Yon == "1" && !skipNext)
                    {
                        entry = currentEntity.Tarih;
                        skipNext = true;
                    }

                    continue;
                }
                else if (skipNext)
                {
                    if (entry == null)
                    {
                        entry = currentEntity.Tarih;
                    }

                    skipNext = false;
                    continue;
                }
                else
                {
                    if (currentEntity.Yon == "1" && entry == null)
                    {
                        entry = currentEntity.Tarih;
                    }

                    if (currentEntity.Yon == "0" && exit == null)
                    {
                        exit = currentEntity.Tarih;
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

        // Kişinin belli bir gündeki toplam çalışma saatini hesaplar.
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
                var nextEntity = entries.Count == i + 1 ? null : entries[i + 1];

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
