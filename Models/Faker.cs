using System;
using System.ComponentModel;
using static DataSecurity.Models.FakerType;

namespace DataSecurity.Models
{
    public class Faker : ViewModel
    {
        private Guid _id;
        private String _name;
        private Types _type;
        private String _img;
        private String _data;
        private BindingList<Faker> _fakers;
        private DateTime _dateTimeCreated;
        private DateTime _dateTimeUpdate;

        public Faker(Guid id, string name, FakerType.Types type, string img, string data = null)
        {
            _id = id;
            _name = name;
            _type = type;
            _img = img;
            _data = data;
            _fakers = new BindingList<Faker>();
            _dateTimeCreated = DateTime.Now;
        }

        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public String Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        public Types Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged("Type");
            }
        }
        public BindingList<Faker> Fakers
        {
            get { return _fakers; }
            set
            {
                _fakers = value;
                OnPropertyChanged("Fakers");
            }
        }
        public String Img
        {
            get { return _img; }
            set {
                _img = value;
                OnPropertyChanged("Img");
                OnPropertyChanged("DisplayIco");
            }
        }
        public String Data
        {
            get { return _data; }
            set
            { 
                _data = value; 
                OnPropertyChanged("Data");
            }
        }
        public DateTime DateTimeCreated
        {
            get { return _dateTimeCreated; }
        }
        public DateTime DateTimeUpdate
        {
            get { return _dateTimeUpdate; }
            set 
            { 
                _dateTimeUpdate = value; 
                OnPropertyChanged("DateTimeUpdate");

            }
        }
        public String DisplayIco
        {
            get { return String.Format(@"{0}\Images\{1}", Environment.CurrentDirectory, _img); }
        }
    }
}
