using advs_backend;
using advs_backend.JSON;
using advs_backend.DB;
using Newtonsoft.Json;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(); // добавляем сервисы CORS
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // указывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = true,
            // строка, представляющая издателя
            ValidIssuer = AuthOptions.ISSUER,
            // будет ли валидироваться потребитель токена
            ValidateAudience = true,
            // установка потребителя токена
            ValidAudience = AuthOptions.AUDIENCE,
            // будет ли валидироваться время существования
            ValidateLifetime = true,
            // установка ключа безопасности
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            // валидация ключа безопасности
            ValidateIssuerSigningKey = true,
        };
    });

var app = builder.Build();

app.Use(async (context, next) =>
{
    await next.Invoke();

    string[] values =
    {
        $"[{DateTime.Now}]",
        context.Connection.RemoteIpAddress.ToString()+':'+context.Connection.RemotePort,
        context.Response.StatusCode.ToString(),
        context.Request.Path
    };
    Console.WriteLine(String.Join(" ", values));

    if (context.Response.StatusCode == 404)
        await context.Response.WriteAsJsonAsync(new { message = "Страница отсутствует" });
});

//app.UseMiddleware<CORS_Middleware>();
app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseAuthentication();
app.UseAuthorization();

app.Map("/advs/{id:int}", (int id) => Database.GetAdv(id));
app.MapDelete("/advs/{id:int}", [Authorize] (HttpContext context, int id) =>
{
    var Request = context.Request;

    Console.WriteLine(context.User.FindFirst("Id").Value);
    int userId = Int32.Parse(context.User.FindFirst("Id").Value);

    using (AdvsContext db = new())
    {
        var adv = (from Advs in db.Advs
                   where Advs.AdvId == id
                   select Advs).First();
        if (adv is not null && adv.UserId == userId)
        {
            db.Advs.Remove(adv);
            db.SaveChanges();

            return new { message = "Объект удален!" };
        }
        else
        {

            return new { message = "Объект не доступен!" };
        }
    }
});
app.MapPost("/advs/new", [Authorize] async (HttpContext context) =>
{
    var Request = context.Request;

    Adv adv = await Request.ReadFromJsonAsync<Adv>();
    Console.WriteLine(context.User.FindFirst("Id").Value);
    int id = Int32.Parse(context.User.FindFirst("Id").Value);
    adv.UserId = id;

    using (AdvsContext db = new())
    {
        db.Advs.Add(adv);
        db.SaveChanges();

        return adv;
    }
});
app.Map("/advs", () =>
{
    using (AdvsContext db = new())
    {
        return db.Advs.ToList();
    }
});
app.MapPost("/login", async (context) =>
{
    var Request = context.Request;
    var Response = context.Response;

    User candidate;
    try
    {
        candidate = await Request.ReadFromJsonAsync<User>();
        var email = new System.Net.Mail.MailAddress(candidate.Email);
        if (candidate.Password.Length == 0)
        {
            throw new NullReferenceException("Некорректные данные!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        Response.StatusCode = 422;
        await Response.WriteAsJsonAsync(new { message = ex.Message });
        return;
    }

    /*
    //string bodyStr = await BodyToString(Request);
    //Console.WriteLine(bodyStr);

    //if (bodyStr is null)
    //{
    //    //Results.Unauthorized();
    //    Response.StatusCode = 401;
    //    await context.Response.WriteAsJsonAsync(new { message = "Неверный логин или пароль!" });
    //    return;
    //}

    //User result = Database.Login(bodyStr);
    */

    User result;
    using (AdvsContext db = new())
    {
        result = (from Users in db.Users
                  where Users.Email == candidate.Email
                  select Users).FirstOrDefault();

        if (result is null)
        {
            //Console.WriteLine(candidate.UserId);
            db.Users.Add(candidate);
            db.SaveChanges();
            //Console.WriteLine(candidate.UserId);

            result = (from Users in db.Users
                      where Users.Email == candidate.Email
                      select Users).FirstOrDefault();
            //await context.Response.WriteAsJsonAsync(new { message = $"Новый пользователь ID: {candidate.UserId}!" });
            //return;
        }
        else if (result.Password != candidate.Password)
        {
            //Results.Unauthorized();
            Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { message = "Неверный логин или пароль!" });
            return;
        }
        
        //Console.WriteLine($"{result.UserId} {result.Email}");
        var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, result.Email),
            new Claim("Id", result.UserId.ToString())
        };
        var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        // формируем ответ  
        var response = new
        {
            Token = encodedJwt,
            Id = result.UserId,
            Email = result.Email
        };
        Console.WriteLine(response.Email);
        await Response.WriteAsJsonAsync(response);
    }    
});
app.Map("/profile", [Authorize] (HttpContext context) => 
{
    var userId = context.User.FindFirst("Id").Value;
    List<Adv> adv;
    using (AdvsContext db = new())
    {
        adv = (from Advs in db.Advs
                   where Advs.UserId == Int32.Parse(userId)
                   select Advs).ToList();
    };
    
    return adv;
});

//app.Map("/profile", async (context) => {
//    Console.WriteLine("Authorization " + context.Request.Headers["Authorization"]);
//    await context.Response.WriteAsJsonAsync(new { message = "Hello World!" });
//    return;
//});

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

//class Message
//{
//    public string Text { get; set; } = "";
//}

public class AuthOptions
{
    public const string ISSUER = "MyAuthServer"; // издатель токена
    public const string AUDIENCE = "MyAuthClient"; // потребитель токена
    const string KEY = "mysupersecret_secretkey!123";   // ключ для шифрации
    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}