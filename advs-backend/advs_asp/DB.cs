using Newtonsoft.Json;
using advs_backend.DB;
using advs_backend.JSON;

namespace advs_backend
{
    internal class Database
    {
        static public string Connection()
        {
            using (AdvsContext db = new())
            {
                var advs = db.Advs.ToList();
                List<AdvJSON> result = new();
                foreach (Adv adv in advs)
                {
                    AdvJSON advJSON = new()
                    {
                        ID = adv.AdvId,
                        Name = adv.Name,
                        Location = adv.Location,
                        Discription = adv.Discription,
                        Price = adv.Price
                    };
                    result.Add(advJSON);

                }
                return JsonConvert.SerializeObject(result);
            }
        }
        static public string GetAdv(int ID)
        {
            using (AdvsContext db = new())
            {
                var adv = (from Advs in db.Advs
                           where Advs.AdvId == ID
                           select Advs).First();
                var creator = (from User in db.Users
                               where User.UserId == adv.UserId
                               select User.Email).First();
                Console.WriteLine(creator);
                AdvJSON advJSON = new()
                {
                    ID = adv.AdvId,
                    Name = adv.Name,
                    Location = adv.Location,
                    Discription = adv.Discription,
                    Price = adv.Price,
                    Email = creator
                };
                return JsonConvert.SerializeObject(advJSON);
            }
        }
        static public string AddAdv(NewAdvJSON adv)
        {
            using (AdvsContext db = new())
            {
                Adv newAdv = new()
                {
                    Name = adv.Name,
                    Location = adv.Location,
                    Discription = adv.Discription,
                    Price = adv.Price,
                    UserId = adv.UserId
                };
                db.Advs.Add(newAdv);
                db.SaveChanges();

                AdvJSON advJSON = new()
                {
                    ID = newAdv.AdvId,
                    Name = newAdv.Name,
                    Location = newAdv.Location,
                    Discription = newAdv.Discription,
                    Price = newAdv.Price
                };
                return JsonConvert.SerializeObject(advJSON);
            }
        }
        static public User? Login(string strUser)
        {
            using (AdvsContext db = new())
            {
                UserJSON userJSON = JsonConvert.DeserializeObject<UserJSON>(strUser);
                var user = (from Users in db.Users
                            where Users.Email == userJSON.Email
                            select Users).FirstOrDefault();
                return user;
                //Result result;

                //if (user != null && userJSON.Password == user.Password)
                //{
                //    Console.WriteLine("User: " + "\n\tID: " + user.UserId + "\n\tEmail: " + user.Email);
                //    result = new()
                //    {
                //        Message = "Authenticated"
                //    };
                //}
                //else
                //{
                //    result = new()
                //    {
                //        Message = "Неверный логин или пароль!"
                //    };
                //}

                //return JsonConvert.SerializeObject(result);
            }
        }
    }
}