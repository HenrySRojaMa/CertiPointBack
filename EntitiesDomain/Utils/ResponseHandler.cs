using EntitiesDomain.Responses;
using System.Diagnostics;

namespace EntitiesDomain.Utils
{
    public static class ResponseHandler
    {
        public static void AddError<T>(this Response<T> response, Exception ex, string currentClass, string currentMethod, bool isGeneric = true)
        {
            string id = Guid.NewGuid().ToString();
            int lineNumber = 0;
            if (isGeneric)
            {
                response.Code = System.Net.HttpStatusCode.InternalServerError;
                response.Message = "Something happened, contact to support.";
            }
            response.Message += " (" + id + ")";
            response.Data = default(T);

            StackTrace stackTrace = new(ex, true);
            foreach (var frame in stackTrace.GetFrames())
                if (frame.GetFileLineNumber() > 0)
                    lineNumber = frame.GetFileLineNumber();

            response.Errors.Add(new()
            {
                Id = id,
                ClassName = currentClass,
                MethodName = currentMethod,
                LineNumber = lineNumber.ToString(),
                Detail = ex.ToString(),
                DateTime = DateTime.Now.ToString()
            });

        }
        public static bool IsOk<T>(this Response<T> response)
        {
            return new HttpResponseMessage(response.Code).IsSuccessStatusCode;
        }
        public static bool IsOk<T, P>(this Response<T> response, Response<P> partial)
        {
            response.Code = partial.Code;
            response.Message = partial.Message;
            response.Errors.AddRange(partial.Errors);
            return new HttpResponseMessage(partial.Code).IsSuccessStatusCode;
        }
    }
}
