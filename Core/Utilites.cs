namespace Core;

public class Utilites
{
    public static List<BomLine> GenerateRandomData(double linesToGenerate)
    {
        var random = new Random();
        var bom = new List<BomLine>();

        for (var i = 0; i < linesToGenerate; i++)
        {
            var bomLine = new BomLine();

            var properties = typeof(BomLine).GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(int))
                {
                    property.SetValue(bomLine, random.Next());
                }
                else if (property.PropertyType == typeof(List<string>))
                {
                    property.SetValue(bomLine, new List<string> {
                        GetRandomString(random, 5),
                        GetRandomString(random, 5),
                        GetRandomString(random, 5)});
                }
                else
                {
                    property.SetValue(bomLine, GetRandomString(random, 10));
                }
            }

            bom.Add(bomLine);
        }

        return bom;
    }
    
    private static string GetRandomString(Random random, int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}