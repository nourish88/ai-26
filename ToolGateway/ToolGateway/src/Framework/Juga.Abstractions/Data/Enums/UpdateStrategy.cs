namespace Juga.Abstractions.Data.Enums;

public enum UpdateStrategy
{
    /// <summary>
    /// Main ve child entityler için primary key alanları belirtilenlerin update, belirtilmeyenlerin insert edilmesi için kullanılır.
    /// </summary>
    UpdateAll,

    /// <summary>
    /// Main entitynin primary key alanı belirlenmişse update etmek, child entitylerin primary key alanları belirlenmemişse insert etmek için kullanılır.
    /// Primary key alanı belirlenmiş child entityler ignore edilir.
    /// </summary>
    MainIfRequiredAddChilds,

    /// <summary>
    /// Main entitynin primary key alanının belirtilip belirtilmemesine bakmaksızın update, child entitylerin hepsinin ignore edilmesi için kullanılır.
    /// </summary>
    OnlyMain
}