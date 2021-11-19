using System.Collections.Generic;
using System.Linq;

namespace DataSecurity.Models
{
    public static class FakerType
    {
        public enum Types
        {
            FOLDERS,
            PASSWORD,
            EMAIL,
            ACCOUNT,
            USERNAME,
            URL,
            DATE,
            DATETIME,
            PHONE,
            PIN,
            SERIALNUMBER,
            NUMBER,
            HOSTNAME,
            IPADDRESS,
            PORT,
            RDP,
            ADDRESS,
            UNKNOWN
        }

        public static Dictionary<Types, string> TMask = new Dictionary<Types, string>
        {
            { Types.FOLDERS, @"^[a-zA-Zа-яА-Я ]+[\w]+$" },
            { Types.PASSWORD, @"[\w]+" },
            { Types.EMAIL, @"^[^@\s]+@[^@\s]+\.[^@\s]+$" }, //!
            { Types.ACCOUNT, @"^[a-zA-Z0-9-_]+$" }, //!
            { Types.USERNAME, "^[a-zA-Zа-яА-Я0-9-_]+$" }, //!
            { Types.URL, @"^http(s)?://([\w]+.)+" }, //! "^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$"
            { Types.DATE, @"^\d{1,2}.\d{1,2}.\d{4}$" }, //!
            { Types.DATETIME, @"^\d{2,2}.\d{2,2}.\d{4} \d{2,2}:\d{2,2}:\d{2,2}$" }, //!
            { Types.PHONE, @"[+]{0,1}[\d]{6,10}$" },
            { Types.PIN, @"^[0-9]{0,4}$" }, //!
            { Types.SERIALNUMBER, @"^[0-9]{0,6}-[0-9]{0,6}-[0-9]{0,6}$" }, //!
            { Types.NUMBER, @"^[\d]+$" }, //!
            { Types.HOSTNAME, @"^[a-zA-Z]+[-_0-9]*$" }, //!
            { Types.IPADDRESS,@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b" }, //!
            { Types.RDP,@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b" }, //!
            { Types.PORT, @"^[\d]+$" }, //!
            { Types.ADDRESS, "^[a-zA-Zа-яА-Я]+[0-9]*$" }, //!
        };

        public static Dictionary<Types, string> TTypes = new Dictionary<Types, string>
        {
            { Types.FOLDERS, "Папка"},
            { Types.PASSWORD, "Пароль"},
            { Types.EMAIL, "Почта"},
            { Types.ACCOUNT, "Аккаунт"},
            { Types.USERNAME, "Имя пользователя"},
            { Types.URL, "ССылка URL"},
            { Types.DATE, "Дата"},
            { Types.DATETIME, "Дата и время"},
            { Types.PHONE, "Телефон"},
            { Types.PIN, "Пин код"},
            { Types.SERIALNUMBER, "Серийный номер"},
            { Types.NUMBER, "Номер"},
            { Types.HOSTNAME, "Имя хоста"},
            { Types.IPADDRESS, "IP адрес"},
            { Types.RDP, "Удаленный рабочий стол"},
            { Types.PORT, "Порт"},
            { Types.ADDRESS, "Адрес"}
        };

        /// <summary>
        /// Получить Enum по значению
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Types getTypeEnum(string type)
        {
            var t = TTypes.SingleOrDefault(item => item.Value == type);
            if (t.Value == type)
                return t.Key;

            return Types.UNKNOWN;
        }

        /// <summary>
        /// Получить название
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string getTypeValue(Types type)
        {
            var t = TTypes.SingleOrDefault(item => item.Key == type);
            if (t.Key == type)
                return t.Value;

            return "";
        }
    }
}
