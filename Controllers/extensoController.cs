using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace excel.Controllers;

[ApiController]
[Route("[controller]")]
public class extensoController : ControllerBase
{
 private readonly ILogger<extensoController> _logger;

 public extensoController(ILogger<extensoController> logger)
 {
  _logger = logger;
 }

 [HttpGet("{value}/{culture}")]
 public IActionResult GetConverter(string value, string culture)
 {
  culture = string.IsNullOrEmpty(culture) || string.IsNullOrWhiteSpace(culture) ? "pt-BR" : culture;
  if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value))
  {
   var ci = new CultureInfo(culture);
   if (double.TryParse(value, NumberStyles.Any, ci, out double conv))
   {
    return Ok(new { value,        
        result = $"{Utils.NumberToWords(conv, ci)}" });
   }
  }
  return NoContent();
 }

 public string Get([FromQuery] string value, [FromQuery] string culture)
 {
  culture = string.IsNullOrEmpty(culture) || string.IsNullOrWhiteSpace(culture) ? "pt-BR" : culture;
  if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value))
  {
   var ci = new CultureInfo(culture);
   if (double.TryParse(value, NumberStyles.Any, ci, out double conv))
   {
    return $"{Utils.NumberToWords(conv, ci)}";
   }
  }
  return string.Empty;
 }
}
