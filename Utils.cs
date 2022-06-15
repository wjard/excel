using System.Globalization;
using Humanizer;
using Humanizer.Localisation;
using System.Linq;

namespace excel;

public struct Money
{
 public string intPart { get; set; }
 public string floatPart { get; set; }
 public string intPartPlural { get; set; }
 public string floatPartPlural { get; set; }
 public string concatPartner { get; set; }
}

public static class Utils
{
 public static Dictionary<string, Money> MoneyDescription = new Dictionary<string, Money>(){
  { "pt-BR", new Money{ intPart = "real", floatPart = "centavo", intPartPlural = "reais", floatPartPlural = "centavos", concatPartner = "e"}},
  { "en-US", new Money{ intPart = "dollar", floatPart = "cent", intPartPlural = "dollars", floatPartPlural = "cents", concatPartner = "and"}}
 };

 public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
 {
  if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
  if (key == null) throw new ArgumentNullException(nameof(key));

  TValue value;
  return dictionary.TryGetValue(key, out value) ? value : defaultValue;
 }

 public static string NumberToWords(double doubleNumber, CultureInfo culture = null)
 {
  culture = culture ?? new CultureInfo("pt-BR");
  Money money = MoneyDescription.GetValueOrDefault(culture.Name, MoneyDescription.LastOrDefault().Value);
  money = MoneyDescription[culture.Name];
  var splitDoubleNumber = Math.Round(doubleNumber, 2).ToString(culture).Split(culture.NumberFormat.NumberDecimalSeparator);

  var beforeFloatingPoint = int.Parse(splitDoubleNumber[0]);
  var beforeFloatingPointWord = $"{NumberToWords((int)beforeFloatingPoint, culture)} {(Math.Abs(beforeFloatingPoint) > 1 ? money.intPartPlural : money.intPart)}";
  var afterFloatingPoint = (int)splitDoubleNumber.Length > 1 ? double.Parse(splitDoubleNumber[1].PadRight(2, '0')) : 0;
  if (splitDoubleNumber[0] == "-0")
   afterFloatingPoint *= -1;
  var afterFloatingPointWord = $"{SmallNumberToWord((int)(beforeFloatingPoint == 0 ? afterFloatingPoint : Math.Abs(afterFloatingPoint)), culture)} {(Math.Abs(afterFloatingPoint) > 1 ? money.floatPartPlural : money.floatPart)}";
  return beforeFloatingPoint == 0
   ? $"{afterFloatingPointWord}"
   : afterFloatingPoint != 0 ? $"{beforeFloatingPointWord} {money.concatPartner} {afterFloatingPointWord}"
   : $"{beforeFloatingPointWord}";
 }

 private static string NumberToWords(int number, CultureInfo culture = null)
 {
  if (number == 0)
   return string.Empty;
  return SmallNumberToWord(number, culture); ;
 }

 private static string SmallNumberToWord(int number, CultureInfo culture = null)
 {
  return number.ToWords(culture: culture);
 }
}