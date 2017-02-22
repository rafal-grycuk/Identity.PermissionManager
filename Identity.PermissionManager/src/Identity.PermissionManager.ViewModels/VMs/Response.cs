namespace Identity.PermissionManager.ViewModels.VMs
{
    public class Response<TDto>
    {
        public string ResponseMessage { get; set; }

        public TDto ResponseBody { get; set; }

        public Response(string message)
        {
            this.ResponseMessage = message;
        }

        public Response(string message, TDto responseBody) : this(message)
        {
            this.ResponseBody = responseBody;
        }
    }
}
