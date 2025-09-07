namespace CertiPoint.Middlewares
{
    public class RequestLogger
    {
        private readonly RequestDelegate _next;

        public RequestLogger(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();
            await _next(context);
        }
    }
}
