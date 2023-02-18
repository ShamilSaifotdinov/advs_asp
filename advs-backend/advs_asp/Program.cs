using advs_asp;
using advs_backend;
using advs_backend.JSON;
using Newtonsoft.Json;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseMiddleware<CORS_Middleware>();

app.Map("/advs/{id:int}", (int id) => Database.GetAdv(id));
app.MapPost("/advs/new", async (context) =>
{
    var Request = context.Request;
    var Response = context.Response;

    string bodyStr = await BodyToString(Request);

    NewAdvJSON adv = JsonConvert.DeserializeObject<NewAdvJSON>(bodyStr);

    Response.Headers.ContentType = "application/json; charset=utf-8";
    string result = Database.AddAdv(adv);
    Console.WriteLine(result);

    await Response.WriteAsync(result);
});
app.Map("/advs", () => Database.Connection());
app.MapPost("/login", async (context) =>
{
    var Request = context.Request;
    var Response = context.Response;

    string bodyStr = await BodyToString(Request);
    string result = Database.Login(bodyStr);
    Console.WriteLine(result);

    await Response.WriteAsync(result);
});

app.Run();

async Task<string> BodyToString(HttpRequest Request)
{
    Request.EnableBuffering();
    var buffer = new byte[Convert.ToInt32(Request.ContentLength)];
    await Request.Body.ReadAsync(buffer, 0, buffer.Length);
    //get body string here...
    string bodyStr = Encoding.UTF8.GetString(buffer);

    Request.Body.Position = 0;  //rewinding the stream to 0

    return bodyStr;
}