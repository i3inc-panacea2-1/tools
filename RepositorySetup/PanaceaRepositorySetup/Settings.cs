using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanaceaRepositorySetup
{

    public class Settings
    {
        const string settingsFile = "settings.json";

        public Settings()
        {
            Tags = new ObservableCollection<Tag>();
            ProjectSettings = new ObservableCollection<ProjectSettings>();
        }
        public ObservableCollection<Tag> Tags { get; set; }

        public ObservableCollection<ProjectSettings> ProjectSettings { get; set; }

        public static Settings Load()
        {
            if (File.Exists(settingsFile))
            {
                using (var r = new StreamReader(settingsFile))
                {
                    return JsonConvert.DeserializeObject<Settings>(r.ReadToEnd());
                }
            }
            return new Settings();
        }

        public void Save()
        {
            using (var r = new StreamWriter(settingsFile))
            {
                r.Write(JsonConvert.SerializeObject(this));
            }
        }
    }


    public class ProjectSettings
    {
        public string Name { get; set; }

        public List<RepositorySettings> RepositorySettings { get; set; }
    }


    public class RepositorySettings
    {
        public string Path { get; set; }

        public string Name { get; set; }

        public List<Tag> Tags { get; set; }


    }
}
