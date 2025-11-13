namespace Juga.Abstractions.Data.Enums;

public enum InsertStrategy
{
    /// <summary>
    /// Main entity ve child entitylerin primary key alanlarının belirtilip belirtilmemesine bakmaksızın hepsini insert etmek için kullanılır.
    /// </summary>
    InsertAll,

    /// <summary>
    /// Main entitynin primary key alanının belirtilip belirtilmemesine bakmaksızın insert edilmesi ,
    /// child entityler için primary key belirtilmeyenlerin insert, belirtilenlerin ignore edilmesi için kullanılır.
    /// </summary>
    MainIfRequiredChilds,

    /// <summary>
    /// Main entitynin primary key alanının belirtilip belirtilmemesine bakmaksızın insert, child entitylerin hepsinin ignore edilmesi için kullanılır.
    /// </summary>
    OnlytMain
}