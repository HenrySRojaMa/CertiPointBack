using EntitiesDomain.Responses;
using EntitiesDomain.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace CertiPoint.Middlewares
{
    public class RequestResult : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is ObjectResult result)
            {
                if (result.Value is Response<object> response)
                {
                    if (response.IsOk())
                    {
                        context.Result = new ObjectResult(new
                        {
                            code = "00",
                            response.Message,
                            response.Data
                        })
                        { StatusCode = (int)response.Code };
                    }
                    else
                    {
                        string id = Guid.NewGuid().ToString();
                        context.Result = new ObjectResult(new
                        {
                            code = "01",
                            Message = response.Message?.Replace("(LOGID)", "(" + id + ")"),
                            response.Data
                        })
                        { StatusCode = (int)response.Code };

                        if (response.Errors.Count > 0)
                        {
                            string path = context.HttpContext.Request.Path.Value;
                            string contentType = context.HttpContext.Request.ContentType;
                            Dictionary<string, string> Params = context.HttpContext.Request.Query.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.FirstOrDefault());
                            object body = contentType == "application/json" ? await GetRequestBody(context) : null;
                            LogJson("RequestLog.txt", new List<object>() { new { id, path, Params, body } });
                            response.Errors.ForEach(error => { error.Id = id; });
                            LogJson("ExceptionLog.txt", response.Errors);
                        }
                    }
                }
            }
            await next();
        }
        public async Task<object> GetRequestBody(ResultExecutingContext context)
        {
            string body = string.Empty;
            context.HttpContext.Request.Body.Position = 0;
            using (StreamReader reader = new(context.HttpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                body = await reader.ReadToEndAsync();
            return JsonConvert.DeserializeObject(ReplaceLongStrings(body));
        }
        string ReplaceLongStrings(string input)
        {
            string pattern = @"\""(.*?)\""";
            return Regex.Replace(input, pattern, match =>
            {
                string foundString = match.Groups[1].Value;
                if (foundString.Length > 300) return "\"too_long\"";
                return match.Value;
            });
        }
        public void LogJson<T>(string logFilePath, List<T> objects)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, append: true))
                {
                    foreach (T obj in objects)
                    {
                        string jsonString = JsonConvert.SerializeObject(obj);
                        writer.WriteLine(jsonString);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
