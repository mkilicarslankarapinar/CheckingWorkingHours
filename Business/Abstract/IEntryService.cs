using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IEntryService
    {
        List<Entry> GetAll();
        List<Entry> GetEntriesForDate(DateTime date);
        List<Entry> GetEntriesForDateAndPerson(DateTime date, int sicil);
        List<Entry> GetPerson(int sicil);
        int CalcutaleWorkingHours();
    }
}
