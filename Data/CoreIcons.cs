using System;
using System.IO;
using System.Windows.Resources;
using System.Windows;
using System.Collections.Generic;
using DataSecurity.Models;

namespace DataSecurity.Data
{
    class ModelIcon
    {
        public String Filename { get; set; }
        public StreamResourceInfo Resource { get; set; }
    }

    class CoreIcons
    {
        private List<ModelIcon> ResourceIcons = new List<ModelIcon>();

        public CoreIcons()
        {
            AddToList(new string[] {
                "aliexpres", "epic", "blizzard", "bluestacks", "discord", "facebook", "folder", "geforce", "google_chrome", "google_drive", "google_mail",
                "google_messages", "google_play", "gzdoom_macos", "instagram", "java", "key", "linux", "mail", "mail_ru", "microsoft", "microsoft_onedrive",
                "microsoft_onedrive2", "minecraft", "minecraft_logo", "netflix", "nvidia_logo", "origin", "shazam", "skype", "spotify", "spotify_icon",
                "steam", "steampowered", "telegram", "twitter", "whatsapp", "wi_fi", "xbox", "yandex", "youtube"
            });
        }

        private void AddToList(string[] filenames)
        {
            foreach(var filename in filenames)
            {
                ResourceIcons.Add(
                    new ModelIcon
                    {
                        Filename = filename,
                        Resource = Application.GetResourceStream(new Uri($"Files/icons/{filename}.png", UriKind.Relative))
                    }
                );
            }
        }

        /// <summary>
        /// Сгенирировать иконки из ресурсов
        /// </summary>
        public void Generate()
        {
            foreach(ModelIcon r in ResourceIcons)
            {
                if (r.Resource == null) continue;
                Stream temp = r.Resource.Stream;
                if (!File.Exists(String.Format(@"{0}\Images\{1}.png", Environment.CurrentDirectory, r.Filename)))
                {
                    FileStream fileStream = new FileStream(String.Format(@"{0}\Images\{1}.png", Environment.CurrentDirectory, r.Filename), FileMode.OpenOrCreate);
                    r.Resource.Stream.CopyTo(fileStream);
                    fileStream.Close();

                }
                r.Resource.Stream.Close();
                r.Resource.Stream.Dispose();
            }
        }

        /// <summary>
        /// Получить файлы иконок
        /// </summary>
        /// <returns></returns>
        public static List<Icon> GetIcons()
        {
            new CoreIcons().Generate();

            string iconsPath = @".\Images";//String.Format(@"{0}\Images", Environment.CurrentDirectory);
            DirectoryInfo di = new DirectoryInfo(iconsPath);
            FileInfo[] files = di.GetFiles("*.png");
            List<Icon> icons = new List<Icon>();

            foreach (var file in files)
            {
                if (File.Exists($@"./Images/{file.Name}"))
                icons.Add(new Icon(Guid.NewGuid(), file.FullName, file.Name));
            }
            return icons;
        }
    }
}
