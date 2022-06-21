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
  try
  {
   culture = string.IsNullOrEmpty(culture) || string.IsNullOrWhiteSpace(culture) ? "pt-BR" : culture;
   if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value))
   {
    value = value.Replace(" ", "");
    var ci = new CultureInfo(culture);
    if (double.TryParse(value, NumberStyles.Any, ci, out double conv))
    {
     return Ok(new
     {
      value,
      result = $"{Utils.NumberToWords(conv, ci)}"
     });
    }
   }
   return BadRequest(new
   {
    value,
    result = "Possível valor inválido"
   });
  }
  catch (OverflowException)
  {
   return BadRequest(new
   {
    value,
    result = "Valor maior que o permitido (int64 inválido)"
   });
  }
  catch (Exception ex)
  {
   return BadRequest(new
   {
    value,
    result = ex.Message
   });
  }
 }

 public string Get([FromQuery] string value, [FromQuery] string culture)
 {
  try
  {
   culture = string.IsNullOrEmpty(culture) || string.IsNullOrWhiteSpace(culture) ? "pt-BR" : culture;
   if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value))
   {
    value = value.Replace(" ", "");
    var ci = new CultureInfo(culture);
    if (double.TryParse(value, NumberStyles.Any, ci, out double conv))
    {
     return $"{Utils.NumberToWords(conv, ci)}";
    }
   }
   return "Possível valor inválido";
  }
  catch (OverflowException)
  {
   return "Valor maior que o permitido (int64 inválido)";
  }
  catch (Exception ex)
  {
   return ex.Message;
  }
 }
}
