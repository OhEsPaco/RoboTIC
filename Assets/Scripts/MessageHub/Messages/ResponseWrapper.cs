public class ResponseWrapper<TPetition, TResponse>
{
    private TPetition petition;
    private TResponse response;

    public TPetition Petition
    {
        get
        {
            return petition;
        }
    }

    public TResponse Response
    {
        get
        {
            return response;
        }
    }

    public ResponseWrapper(TPetition petition, TResponse response)
    {
        this.petition = petition;
        this.response = response;
    }
}