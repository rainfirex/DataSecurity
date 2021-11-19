using DataSecurity.Data;
using System;

namespace DataSecurity.Models
{
    public class Config
    {   
        public string ProgrammName { get; } = "Password";
        public string Version { get; } = "1.5";
        public string VersionType { get; } = "Beta";
        public string Programmist { get;  } = "Poplavskiy Aleksandr";
        public string ProgrammistEmail { get;  } = "rainfire491@gmail.com";        
        public string SharedSecret { get; set; } = "23n0`9@9j_!#4-1r";
        public TimeSpan TimeoutSession { get; set; } = new TimeSpan(0, 02, 59);
        public bool IsUseLastEmailAuth { get; set; } = true;
        public string LastEmailAuth { get; set; }
        public string LastIconFaker { get; set; }
        public string LastTypeFaker { get; set; }
        public string SMTP_HOST { get; set; } = "smtp.mail.ru";
        public int SMTP_PORT { get; set; } = 587;
        public string SMTP_USER { get; set; } = "";
        public string SMTP_PASSWORD { get; set; } = "";
        public bool SMTP_SSL { get; set; } = true;
        public int SMTP_TIMEOUT { get; set; } = 50;
        public bool SPEAK_ON { get; set; } = false;

        public Config()
        {
            SharedSecret = CoreStorage.LoadSharedSecret() ?? SharedSecret;
        }
    }
}
