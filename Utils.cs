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
 private static BrazilianPortugueseNumberToWordsConverter brazilConv = new BrazilianPortugueseNumberToWordsConverter();

 public static Dictionary<string, Money> MoneyDescription = new Dictionary<string, Money>(){
  { "PT-BR", new Money{ intPart = "real", floatPart = "centavo", intPartPlural = "reais", floatPartPlural = "centavos", concatPartner = "e"}},
  { "EN-US", new Money{ intPart = "dollar", floatPart = "cent", intPartPlural = "dollars", floatPartPlural = "cents", concatPartner = "and"}}
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
  CultureInfo cultureInfoOri = Thread.CurrentThread.CurrentCulture;
  try
  {
   Thread.CurrentThread.CurrentCulture = culture;
   Money money = MoneyDescription.GetValueOrDefault(culture.Name, MoneyDescription.LastOrDefault().Value);
   money = MoneyDescription[culture.Name.ToUpper()];
   var splitDoubleNumber = Math.Round(doubleNumber, 2).ToString("n", culture).Replace(culture.NumberFormat.NumberGroupSeparator, "").Split(culture.NumberFormat.NumberDecimalSeparator);

   if (splitDoubleNumber.Length > 1)
   {
    if (splitDoubleNumber[1].Length > 2 && splitDoubleNumber[1].EndsWith("0"))
     splitDoubleNumber[1] = splitDoubleNumber[1].Substring(0, 2);
   }
   var beforeFloatingPoint = long.Parse(splitDoubleNumber[0], culture);
   var beforeFloatingPointWord = $"{NumberToWords((long)beforeFloatingPoint, culture)} {(Math.Abs(beforeFloatingPoint) > 1 ? money.intPartPlural : money.intPart)}";
   var afterFloatingPoint = (long)splitDoubleNumber.Length > 1 ? double.Parse(splitDoubleNumber[1].PadRight(2, '0'), culture) : 0;
   if (splitDoubleNumber[0] == "-0")
    afterFloatingPoint *= -1;
   var afterFloatingPointWord = $"{SmallNumberToWord((long)(beforeFloatingPoint == 0 ? afterFloatingPoint : Math.Abs(afterFloatingPoint)), culture)} {(Math.Abs(afterFloatingPoint) > 1 ? money.floatPartPlural : money.floatPart)}";
   return beforeFloatingPoint == 0
    ? $"{afterFloatingPointWord}"
    : afterFloatingPoint != 0 ? $"{beforeFloatingPointWord} {money.concatPartner} {afterFloatingPointWord}"
    : $"{beforeFloatingPointWord}";
  }
  finally
  {
   Thread.CurrentThread.CurrentCulture = cultureInfoOri;
  }
 }

 private static string NumberToWords(long number, CultureInfo culture = null)
 {
  if (number == 0)
   return string.Empty;
  return SmallNumberToWord(number, culture); ;
 }

 private static string SmallNumberToWord(long number, CultureInfo culture = null)
 {
  if (culture?.Name.IndexOf("pt-BR", StringComparison.InvariantCultureIgnoreCase) > -1)
   return brazilConv.Convert(number);
  return number.ToWords(culture: culture);
 }
}