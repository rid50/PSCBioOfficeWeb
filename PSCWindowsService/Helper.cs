/******************************************************************************/
/* Library functions for the pr sample programs.                              */
/******************************************************************************/
using gx;

/******************************************************************************/
class Helper
{
    /**************************************************************************/
    /* Set the code and description of the underlying GX exception.      */
    /**************************************************************************/
    public int GetErrorMessage(gxException e, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (gxSystem.GetErrorCode() != 0)
        {
            errorMessage = gxSystem.GetErrorString();
            return gxSystem.GetErrorCode();
        }
        else
            return 0;
    }

    /**************************************************************************/
    /* Waits for a time specified in miliseconds.                             */
    /**************************************************************************/
    public void Wait(int ms)
    {
        System.Threading.Thread.Sleep(ms);
    }
}

class NoDocumentFoundException : System.Exception
{
    public NoDocumentFoundException(string message) : base(message)
    {
    }
}