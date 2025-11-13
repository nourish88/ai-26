namespace Juga.Data.Configuration;


public class DatabaseOptions
{
    public const string DatabaseOptionsSection = "Juga:Data:UnitOfWork.DatabaseOptions";
    /// <summary>
    /// Kullanılacak olan veritabanının tipini <seealso cref="Juga.Abstractions.Data.Enums.DatabaseType"/> belirtmek için kullanılır.Default SqlServer.
    /// </summary>
    public virtual DatabaseType DatabaseType { get; set; } = DatabaseType.SqlServer;
    /// <summary>
    /// Veritabanına bağlantısı sağlamak için kullanılacak olan connectionstring değeri.
    /// </summary>
    public virtual string ConnectionString { get; set; }
    /// <summary>
    /// Migration için kullanılacak assembly nin adı.
    /// </summary>
    public virtual string MigrationAssemblyName { get; set; }
}