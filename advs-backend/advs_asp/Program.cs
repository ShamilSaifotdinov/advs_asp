using advs_backend;
using advs_backend.JSON;
using Newtonsoft.Json;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Run(async context =>
{
    var Request = context.Request;
    var Response = context.Response;
    var connection = context.Connection;
    var path = Request.Path;

    string log = connection.RemoteIpAddress + ":" + connection.RemotePort + ": " + path;

    Response.Headers["Access-Control-Allow-Origin"] = "*";

    string result;

    if (path == "/advs")
    {
        result = Database.Connection();
        Response.Headers.ContentType = "application/json; charset=utf-8";
    }
    else if (path == "/advs/new" && Request.Method == "POST")
    {
        string bodyStr = await BodyToString(Request);
        log += " " + bodyStr;

        NewAdvJSON adv = JsonConvert.DeserializeObject<NewAdvJSON>(bodyStr);
        result = Database.AddAdv(adv);
        Response.Headers.ContentType = "application/json; charset=utf-8";
    }
    else if (Regex.IsMatch(path, "^/advs/([0-9]+)$"))
    {
        try
        {
            //Console.WriteLine(path);
            string[] parsedURI = path.ToString().Split('/');

            //foreach (string str in parsedURI) {
            //    Console.WriteLine(str);
            //}
            //Console.WriteLine(parsedURI[2]);

            if (parsedURI.Length > 3) throw new Exception();
            int ID = Int32.Parse(parsedURI[2]);

            result = Database.GetAdv(ID);
        }
        catch(Exception e)
        {
            log += " " + e;
            result = "Not found";
            Response.StatusCode = 404;
        }
    } 
    else if (path == "/login" && Request.Method == "POST")
    {
        try
        {
            string bodyStr = await BodyToString(Request);
            log += " " + bodyStr;
            result = Database.Login(bodyStr);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            result = "Not found";
            Response.StatusCode = 404;
        }
    }
    else
    {
        result = "Not found";
        Response.StatusCode= 404;
    }

    await Response.WriteAsync(result);
    Console.WriteLine(log + " " + result);
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