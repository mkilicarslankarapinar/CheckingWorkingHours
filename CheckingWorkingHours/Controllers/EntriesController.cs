﻿using Business.Abstract;
using Business.Concrete;
using ClosedXML.Excel;
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


        [HttpGet("calculatedailyworkinghours")]
        public TimeSpan CalculateWorkingHours(DateTime date, string sicil)
        {
            IEntryService entryService = new EntryManager(new EfEntryDal());
            var result = entryService.CalculateWorkingHours(date, sicil);
            return result;

        }

        [HttpGet("calculatemonthlyworkinghours")]
        public Dictionary<string, TimeSpan> CalculateMonthlyWorkingHours(DateTime date)
        {
            IEntryService entryService = new EntryManager(new EfEntryDal());
            var result = entryService.CalculateMonthlyWorkingHours(date);
            return result;

        }

        [HttpGet("downloadmonthlyworkinghours")]
        public IActionResult CalculateWorkingHours(DateTime date)
        {
            // Çalışma saatlerini hesapla
            Dictionary<string, TimeSpan> result = _entryService.CalculateMonthlyWorkingHours(date);

            // Excel dosyasına aktar
            byte[] fileContents = ExportToExcel(result);

            // Dosyayı kullanıcıya indir
            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MonthlyWorkingHours.xlsx");
        }
        private byte[] ExportToExcel(Dictionary<string, TimeSpan> data)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("MonthlyWorkingHours");

                // Başlık satırını ekle
                worksheet.Cell(1, 1).Value = "Sicil";
                worksheet.Cell(1, 2).Value = "Toplam Çalışma Saati";

                // Verileri Excel'e aktar
                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cell(row, 1).Value = item.Key;
                    // Saat ve dakika olarak formatla
                    worksheet.Cell(row, 2).Value = string.Format("{0}:{1:00}", (int)item.Value.TotalHours, item.Value.Minutes);
                    row++;
                }

                // Belleğe kaydet
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }


    }
}
