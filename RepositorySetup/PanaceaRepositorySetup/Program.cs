
using Newtonsoft.Json;
using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanaceaRepositorySetup
{
    class Program
    {
        static RepoSettings _settings;

        static List<KeyValuePair<string, Action>> _options = new List<KeyValuePair<string, Action>>()
        {
            new KeyValuePair<string, Action>("Setup repositories.", SetupRepositories),
            new KeyValuePair<string, Action>("Set environment variables.", SetupEnvironmentVariables),
            new KeyValuePair<string, Action>("Exit.", ()=> Environment.Exit(0)),
        };

        static void Main(string[] args)
        {

            MainMenu();
        }

        static void SetupEnvironmentVariables()
        {
           
        }

      
        static void SetupRepositories()
        {
            
            using (var reader = new StreamReader("settings.json"))
            {
                Console.WriteLine("Doing work. Don't look at me like that. The work should have finished by now. But you are still reading. Whatev.");
                _settings = JsonConvert.DeserializeObject<RepoSettings>(reader.ReadToEnd());


                var git = new GitHubClient(new ProductHeaderValue("MyAmazingApp"), new Uri("https://git.i3inc.ca/"));
                git.Credentials = new Credentials(_settings.Username, _settings.Password);

                var org = git.Organization.Get("Panacea2-1").Result;
                var repos = git.Repository.GetAllForOrg("Panacea2-1").Result.ToList();


                var apis = repos.Where(r => r.Name.StartsWith("Panacea.Modularity."))
                                .ToList();
                repos.RemoveAll(r => apis.Contains(r));

                var modules = repos
                                .Where(r => r.Name.StartsWith("Panacea.Modules."))
                                .ToList();

                repos.RemoveAll(r => modules.Contains(r));

                var apps = repos.Where(r => r.Name == "Panacea");
                repos.RemoveAll(r => apps.Contains(r));

                var libs = repos;
                var path = _settings.RootDir;
               
                CloneRepos(apis.Select(r => r.Name), Path.Combine(path, "Libraries"));
                CloneRepos(libs.Select(r => r.Name), Path.Combine(path, "Libraries"));
                CloneRepos(modules.Select(r => r.Name), Path.Combine(path, "Modules"));
                CloneRepos(apps.Select(r => r.Name), Path.Combine(path, "Applications"));
            }
            Console.WriteLine("Press any key");
            Console.ReadKey();
        }

        static void CloneRepos(IEnumerable<string> repos, string path)
        {

            foreach (var repo in repos)
            {
                var url = new Uri(_settings.GithubEnterpriseUrl, _settings.Organization + "/" + repo + ".git");
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(repo);
                Console.ForegroundColor = color;
                Clone(url.ToString(), Path.Combine(path, repo));
            }

        }

        static void Clone(string url, string path)
        {
            var info = new ProcessStartInfo()
            {
                FileName = @"C:\Program Files\Git\bin\git.exe",
                Arguments = "clone " + url + " \"" + path + "\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            var p = new Process()
            {
                StartInfo = info,

            };
            p.Start();
            p.WaitForExit();
            var res = p.StandardError.ReadToEnd();
            if (p.ExitCode !=0)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(res);
                Console.ForegroundColor = color;
            }
            else
            {
                Console.WriteLine(res);
            }
        }

        static void MainMenu()
        {
            while (true)
            {
                int choice;
                bool res;
                bool error = false;
                do
                {
                    Console.Clear();

                    for (var i = 0; i < _options.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {_options[i].Key}");
                    }
                    if (error)
                    {
                        var color = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Wrong choice!");
                        Console.ForegroundColor = color;
                    }
                    error = true;
                    res = int.TryParse(Console.ReadLine(), out choice);
                } while (!res || choice > _options.Count || choice < 1);
                Console.Clear();
                error = false;
                _options[choice - 1].Value.Invoke();
            }

        }

    }

    public class RepoSettings
    {
        public Uri GithubEnterpriseUrl { get; set; }

        public string Organization { get; set; }
        public string Username { get; set; }

        public string Password { get; set; }

        public string RootDir { get; set; }
    }
}

