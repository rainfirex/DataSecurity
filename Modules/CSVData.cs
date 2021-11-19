using CsvHelper;
using DataSecurity.Models;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Modules
{
    class CSVData
    {
        public static void exportOld(string path, BindingList<User> list)
        {
            using (var mem = new MemoryStream())
            using (var writer = new StreamWriter(mem))
            using (var csvWriter = new CsvWriter(writer, System.Globalization.CultureInfo.CurrentCulture))
            {
                //csvWriter.WriteHeader<Group>();
                csvWriter.WriteRecords(list);

                writer.Flush();
                string result = Encoding.UTF8.GetString(mem.ToArray());
                File.WriteAllText($"{path}", result);
            }
        }

        internal static string export(string path, Faker faker)
        {
            using (var mem = new MemoryStream())
            using (var writer = new StreamWriter(mem))
            using (var csvWriter = new CsvWriter(writer, System.Globalization.CultureInfo.CurrentCulture))
            {
                csvWriter.WriteRecord(faker);
                csvWriter.NextRecord();

                foreach (Faker fak in faker.Fakers)
                {
                    csvWriter.WriteField(fak.Id);
                    //csvWriter.WriteField(group.Description);
                    //csvWriter.WriteField(password.Id);
                    //csvWriter.WriteField(password.Title);
                    //csvWriter.WriteField(password.Description);
                    //csvWriter.WriteField(password.Login);
                    //csvWriter.WriteField(password.Email);
                    //csvWriter.WriteField(password.Host);
                    //csvWriter.WriteField(password.IP);
                    //csvWriter.WriteField(password.Type);
                    //csvWriter.WriteField(password.Pass);
                    csvWriter.NextRecord();
                }


                writer.Flush();
                var result = Encoding.UTF8.GetString(mem.ToArray());

                if (!String.IsNullOrEmpty(result))
                {
                    File.WriteAllText(path, result);
                }
                return path;
            }
        }
    }
}
