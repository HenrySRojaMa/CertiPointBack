namespace EntitiesDomain.Responses
{
    public class Response<T>
    {
        public System.Net.HttpStatusCode Code { get; set; } = System.Net.HttpStatusCode.OK;
        public string Message { get; set; }
        public T Data { get; set; }
        public List<Error> Errors { get; set; } = new();
    }
    public class Error
    {
        public string Id { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public string LineNumber { get; set; }
        public string DateTime { get; set; }
        public string Detail { get; set; }
    }
}
