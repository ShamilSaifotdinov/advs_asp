namespace advs_asp
{
    public class CORS_Middleware
    {
        private readonly RequestDelegate next;
        public CORS_Middleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            await next.Invoke(context);
        }
    }
}
