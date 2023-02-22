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

builder.Services.AddCors(); // добавл€ем сервисы CORS
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // указывает, будет ли валидироватьс€ издатель при валидации токена
            ValidateIssuer = true,
            // строка, представл€юща€ издател€
            ValidIssuer = AuthOptions.ISSUER,
            // будет ли валидироватьс€ потребитель токена
            ValidateAudience = true,
            // установка потребител€ токена
            ValidAudience = AuthOptions.AUDIENCE,
            // будет ли валидироватьс€ врем€ существовани€
            ValidateLifetime = true,
            // установка ключа безопасности
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            // валидаци€ ключа безопасности
            ValidateIssuerSigningKey = true,
        };
    });

var app = builder.Build();

app.Use(async (context, next) =>
{
    await next.Invoke();

    Console.WriteLine(context.Response.StatusCode);

    if (context.Response.StatusCode == 404)
        await context.Response.WriteAsJsonAsync(new { message = "—траница отсутствует" });
});

//app.UseMiddleware<CORS_Middleware>();
app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.Map("/advs/{id:int}", (int id) => Database.GetAdv(id));
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
            throw new NullReferenceException("Ќекорректные данные!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
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
    //    await context.Response.WriteAsJsonAsync(new { message = "Ќеверный логин или пароль!" });
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
            //await context.Response.WriteAsJsonAsync(new { message = $"Ќовый пользователь ID: {candidate.UserId}!" });
            //return;
        }
        else if (result.Password != candidate.Password)
        {
            //Results.Unauthorized();
            Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { message = "Ќеверный логин или пароль!" });
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

/*
app.MapPost("/login", async (context) =>
{
    var Request = context.Request;
    var Response = context.Response;

    string bodyStr = await BodyToString(Request);
    User result = Database.Login(bodyStr);

    if (result is null)
    {
        //Results.Unauthorized();
        Response.StatusCode = 401;
        await context.Response.WriteAsJsonAsync(new { message = "Ќеверный логин или пароль!" });
        return;
    }
    Console.WriteLine(result.Email + " " + result.UserId);

    //производитс€ установка аутентификационных кук, которые будут примен€тьс€ дл€ определени€ клиента и его прав в приложении:
    //Claim - грубо говор€ набор данных, которые описывают пользовател€. аждый такой claim принимает тип и значение.
    var claims = new List<Claim> { new Claim(ClaimTypes.Name, result.Email) };

    // создаетс€ объект ClaimsIdentity, который нужен дл€ инициализации ClaimsPrincipal.
    // ¬ ClaimsIdentity передаетс€ ранее созданный список claims и тип аутентификации, в данном случае "Cookies"
    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
    // установка аутентификационных куки
    context.User = new ClaimsPrincipal(claimsIdentity);

    //var bytes = Encoding.UTF8.GetBytes(result.ToString());
    //await Response.Body.WriteAsync(bytes, 0, bytes.Length);

    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
    //await Response.WriteAsJsonAsync(new { ID = result.UserId });
});

*/

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
    const string KEY = "mysupersecret_secretkey!123";   // ключ дл€ шифрации
    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}