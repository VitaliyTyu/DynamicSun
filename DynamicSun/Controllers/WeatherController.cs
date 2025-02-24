using DynamicSun.Data;
using DynamicSun.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace DynamicSun.Controllers;

public class WeatherController : Controller
{
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly WeatherDbContext _context;

    public WeatherController(IWebHostEnvironment hostEnvironment, WeatherDbContext context)
    {
        _hostEnvironment = hostEnvironment;
        _context = context;
    }
    
    // GET
    public async Task<IActionResult> Index(int month = -1, int year = -1)
    {
        var weather = new List<Weather>();

        if (month == -1 && year == -1)
        {
            return View(weather);
        }
        
        if (month != -1 && year != -1)
        {
            weather = await _context.Weather
                .Where(item => item.Month == month && item.Year == year)
                .ToListAsync();
        }
        else if (month != -1)
        {
            weather = await _context.Weather
                .Where(item => item.Month == month)
                .ToListAsync();
        }
        else if (year != -1)
        {
            weather = await _context.Weather
                .Where(item => item.Year == year)
                .ToListAsync();
        }
        
        return View(weather);
    }
    
    [HttpGet]
    public IActionResult Import()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Import(List<IFormFile> files)
    {
        foreach (var file in files)
        {
            if (file.FileName.Split(".")[1] != "xlsx")
            {
                return RedirectToAction("IncorrectFileType");
            }

            var filePath = Path.Combine(_hostEnvironment.ContentRootPath, file.FileName);

            await using var fileStream1 = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            await file.CopyToAsync(fileStream1);
            fileStream1.Close();

            IWorkbook workbook;

            await using (var fileStream2 = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(fileStream2);
            }

            await ProcessSheets(workbook);
            
        }

        return RedirectToAction("Index");
    }

    private async Task ProcessSheets(IWorkbook workbook)
    {
        for (int sheetIndex = 0; sheetIndex < workbook.NumberOfSheets; sheetIndex++)
        {
            var sheet = workbook.GetSheetAt(sheetIndex);

            var monthData = await ProcessRows(sheet);

            await _context.Weather.AddRangeAsync(monthData);

        }
        
        await _context.SaveChangesAsync();
    }


    private async Task<List<Weather>> ProcessRows(ISheet sheet)
    {
        var monthData = new List<Weather>();

        for (var rowIndex = 4; rowIndex < sheet.LastRowNum + 1; rowIndex++)
        {
            var row = sheet.GetRow(rowIndex);

            var weatherData = await ParseWeatherData(row);
            
            monthData.Add(weatherData);
        }

        return monthData;
    }

    private async Task<Weather> ParseWeatherData(IRow row)
    {
        var day = int.Parse(row.GetCell(0).StringCellValue.Split(".")[0]);
        var month = int.Parse(row.GetCell(0).StringCellValue.Split(".")[1]);
        var year = int.Parse(row.GetCell(0).StringCellValue.Split(".")[2]);

        var tempDate = row.GetCell(0).StringCellValue.Split(".");
        var tempTime = row.GetCell(1).StringCellValue.Split(":");
        var dateAndTime = new DateTimeOffset(int.Parse(tempDate[2]), int.Parse(tempDate[1]), int.Parse(tempDate[0]),
            int.Parse(tempTime[0]), int.Parse(tempTime[1]), 00, TimeSpan.Zero);
        var time = dateAndTime.TimeOfDay.ToString();
        
        var temperature = row.GetCell(2).NumericCellValue;
        var relativeHumidity = row.GetCell(3).NumericCellValue;
        var temperatureDew = row.GetCell(4).NumericCellValue;
        var atmosphericPressure = int.Parse($"{row.GetCell(5).NumericCellValue}");

        var windDirection = await ParseCellStringValue(row.GetCell(6));
        var windSpeed = await ParseCellNumericValue(row.GetCell(7));
        var cloudCover = await ParseCellNumericValue(row.GetCell(8));
        var cloudBase = await ParseCellNumericValue(row.GetCell(9));
        var visibility = await ParseCellNumericValue(row.GetCell(10));
        var weatherPhenomena = await ParseCellStringValue(row.GetCell(11));
        
        return new Weather
        {
            Day = day,
            Month = month,
            Year = year,
            Time = time,
            Temperature = temperature,
            RelativeHumidity = relativeHumidity,
            Td = temperatureDew,
            AtmosphericPressure = atmosphericPressure,
            WindDirection = windDirection,
            WindSpeed = windSpeed,
            CloudCover = cloudCover,
            H = cloudBase,
            VV = visibility,
            WeatherPhenomena = weatherPhenomena,
        };
    }

    private async Task<string?> ParseCellStringValue(ICell cell)
    {
        try
        {
            var value = cell.StringCellValue;
            return string.IsNullOrEmpty(value) ? null : value;
        }
        catch (Exception e)
        {
            return null;
        }
    }
    
    private async Task<double?> ParseCellNumericValue(ICell cell)
    {
        try
        {
            var value = cell.NumericCellValue;
            return value;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public IActionResult IncorrectFileType()
    {
        return View("IncorrectFileType");
    }
}