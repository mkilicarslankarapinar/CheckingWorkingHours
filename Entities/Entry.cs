using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Entry: IEntity
    {
        [Key]
        public int Sicil { get; set; }
        public DateTime Tarih { get; set; }
        public string? Yon { get; set; }
        public string? Tanim { get; set; }
        public int TerminalID { get; set; }
    }
}
