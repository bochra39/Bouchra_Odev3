using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bouchra_Odev3.Model
{
    public class FirebaseServices
    {
        static string project_id = "bouchra-project";
        static string ApiKey = "AIzaSyCjS8qTJY8yP2MiteAskcGFXkJc3Q8_9Vs";
        static string AuthDomain = "bouchra-project.web.app";

        static FirebaseAuthConfig config = new FirebaseAuthConfig()
        {
            ApiKey = ApiKey,
            AuthDomain = AuthDomain,
            Providers = new[] { new EmailProvider() }
        };

        private static readonly FirebaseClient firebaseClient = new FirebaseClient("https://bouchra-project-default-rtdb.firebaseio.com/");

        public static async Task<User> Login(string email, string pass)
        {
            try
            {
                var client = new FirebaseAuthClient(config);
                var res = await client.SignInWithEmailAndPasswordAsync(email, pass);
                return res.User;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task<User> Register(string dispname, string email, string pass)
        {
            try
            {
                var client = new FirebaseAuthClient(config);
                var res = await client.CreateUserWithEmailAndPasswordAsync(email, pass, dispname);
                return res.User;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task<List<TaskItem>> GetAllTasks()
        {
            return (await firebaseClient
              .Child("Tasks")
              .OnceAsync<TaskItem>()).Select(item => new TaskItem
              {
                  Name = item.Object.Name,
                  Detail = item.Object.Detail,
                  Date = item.Object.Date,
                  Time = item.Object.Time,
                  IsComplete = item.Object.IsComplete
              }).ToList();
        }

        public static async Task AddTask(TaskItem task)
        {
            await firebaseClient
              .Child("Tasks")
              .PostAsync(task);
        }

        public static async Task DeleteTask(string name)
        {
            var toDeleteTask = (await firebaseClient
              .Child("Tasks")
              .OnceAsync<TaskItem>()).FirstOrDefault(a => a.Object.Name == name);
            await firebaseClient.Child("Tasks").Child(toDeleteTask.Key).DeleteAsync();
        }

        public static async Task UpdateTask(TaskItem task)
        {
            var toUpdateTask = (await firebaseClient
              .Child("Tasks")
              .OnceAsync<TaskItem>()).FirstOrDefault(a => a.Object.Name == task.Name);
            await firebaseClient
              .Child("Tasks")
              .Child(toUpdateTask.Key)
              .PutAsync(task);
        }
    }
}