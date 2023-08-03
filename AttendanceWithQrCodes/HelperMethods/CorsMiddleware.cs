namespace AttendanceWithQrCodes.HelperMethods
{
    public class CorsMiddleware
    {
        private readonly RequestDelegate _next;

        public CorsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            httpContext.Response.Headers.Add("Content-Range", "posts 0-20/*");
            httpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Range");
             return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CorsMiddlewareExtensions
    {
        public static IApplicationBuilder UseCorsExposeHeadersMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorsMiddleware>();
        }
    }
}
