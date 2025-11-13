namespace Juga.Abstractions.Data.Enums;

public enum DeleteStrategy
{
    /// <summary>
    /// Main ve child entityler için primary key alanları belirtilenlerin delete edilmesi için kullanılır.
    /// </summary>
    DeleteAll,

    /// <summary>
    /// Main entitynin primary key alanı belirlenmişse delete etmek, child entitylerin primary key alanları belirlenmemişse insert etmek için kullanılır.
    /// Primary key alanı belirlenmiş child entityler ignore edilir.
    /// </summary>
    MainIfRequiredAddChilds,

    /// <summary>
    /// Main entitynin primary key alanının belirtilip belirtilmemesine bakmaksızın delete, child entitylerin hepsinin ignore edilmesi için kullanılır.
    /// </summary>
    OnlyMain
}