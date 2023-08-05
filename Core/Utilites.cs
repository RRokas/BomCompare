namespace Core;

public class Utilites
{
    public static List<BomLine> GenerateRandomData(double linesToGenerate)
    {
        var random = new Random();
        var bom = new List<BomLine>();

        for (var i = 0; i < linesToGenerate; i++)
        {
            var bomLine = new BomLine
            {
                InternalPartId = GetRandomString(random, 5),
                ManufacturerPartId = GetRandomString(random, 10),
                ManufacturerName = GetRandomString(random, 10),
                PartDescription = GetRandomString(random, 30),
                Quantity = random.Next(1000000, 9999999),
                Value = GetRandomString(random, 10),
                Positions = new List<string> {
                    GetRandomString(random, 5),
                    GetRandomString(random, 5),
                    GetRandomString(random, 5)}
            };
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