namespace Juga.CrossCuttingConcerns.Exceptions.Types;

public class BusinessException:Exception
{
    public BusinessException()
    {
          
    }
    public BusinessException(string? message):base()
    {

    }
    public BusinessException(string? message, Exception? innerException) : base()
    {

    }
}