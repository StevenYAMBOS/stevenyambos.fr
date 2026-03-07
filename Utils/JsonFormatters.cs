using Newtonsoft.Json;
using Portfolio.Entities;

namespace Portfolio.Formatters;

public class JsonFomatters()
{

    public static string JsonStringFormatter(string param)
    {
        return JsonConvert.SerializeObject(param, Formatting.Indented);
    }
    public static string JsonUserFormatter(ApplicationUser param)
    {
        return JsonConvert.SerializeObject(param, Formatting.Indented);
    }
}