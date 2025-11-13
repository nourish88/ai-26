namespace AdminBackend.Application.Dtos
{
    public  record LlmDto
    (
       long Id ,
       long LlmProviderId ,
       int MaxInputTokenSize ,
       int MaxOutputTokenSize ,
       string Url ,
       string ModelName 
    );
}
