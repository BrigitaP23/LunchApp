using System.Text.Json;
using LunchApp.Models;

namespace LunchApp.Services
{
    public class FileService
    {
        private readonly string usersPath = "data/users.json";
        private readonly string signupsPath = "data/signups.json";

        public FileService()
        {
            // Poskrbi, da mapa obstaja
            var directory = "data";
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Če datoteke ne obstajajo, jih ustvari prazne
            if (!File.Exists(usersPath))
                File.WriteAllText(usersPath, "[]");
            if (!File.Exists(signupsPath))
                File.WriteAllText(signupsPath, "[]");
        }

        public List<User> LoadUsers()
        {
            return JsonSerializer.Deserialize<List<User>>(File.ReadAllText(usersPath)) ?? new List<User>();
        }

        public void SaveUsers(List<User> users)
        {
            File.WriteAllText(usersPath, JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true }));
        }

        public List<MealSignup> LoadSignups()
        {
            return JsonSerializer.Deserialize<List<MealSignup>>(File.ReadAllText(signupsPath)) ?? new List<MealSignup>();
        }

        public void SaveSignups(List<MealSignup> signups)
        {
            File.WriteAllText(signupsPath, JsonSerializer.Serialize(signups, new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}
