using System;
using System.ComponentModel;

namespace DataSecurity.Models
{
    public class User : ViewModel
    {
        private BindingList<Faker> _fakers;

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public TimeSpan TimeAuth { get; set; }
        public DateTime DateRegistration { get; set; }
        public BindingList<Faker> Fakers
        {
            get { return _fakers; }
            set
            {
                _fakers = value;
                OnPropertyChanged("Fakers");
            }
        }
    }
}
