namespace Core.Tests.Utilities;

public static class TestDataDirectory
{
    public static FileInfo GetFile(string fileName)
    {
        var foundFile = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_data"), fileName, SearchOption.AllDirectories);
            
        if (foundFile.Length == 0)
            throw new FileNotFoundException($"File {fileName} not found in test_data directory");
        if (foundFile.Length > 1)
            throw new FileNotFoundException($"Multiple files with name {fileName} found in test_data directory");
            
        return new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_data", foundFile[0]));
    }
}