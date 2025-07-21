namespace ErpApp.Services;

public class DbContextSelectorService
{
    private string _current = "mssql";
    public string Current => _current;

    public void Set(string dbName)
    {
        _current = dbName.ToLower();
    }
}
