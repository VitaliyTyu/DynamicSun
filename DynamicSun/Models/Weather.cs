using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace DynamicSun.Models;

public class Weather
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int Day { get; set; }
    
    [Required]
    public int Month { get; set; }
    
    [Required]
    public int Year { get; set; }

    [Required]
    public string Time { get; set; }

    [Required]
    public double Temperature { get; set; }

    // относительная влажность    
    [Required]
    public double RelativeHumidity { get; set; }

    // точка росы
    [Required]
    public double Td { get; set; }

    // атмосферное давление
    [Required]
    public int AtmosphericPressure { get; set; }

    // направление ветра
    public string? WindDirection { get; set; }
    
    // скорость ветра
    public double? WindSpeed { get; set; }
    
    // облачность
    public double? CloudCover { get; set; }
    
    // нижняя граница облачности
    public double? H { get; set; }
    
    // горизонтальная видимость
    public double? VV { get; set; }
    
    // погодные явления
    public string? WeatherPhenomena { get; set; }
    
}