using System;

namespace DataSecurity.Models
{
    public class Icon : ViewModel
    {
        private Guid _id;
        private String _path;
        private String _name;

        public Icon(Guid id, string path, string name = null)
        {
            _id = id;
            _path = path;
            _name = name;
        }

        public Guid Id
        {
            get { return _id; }
        }

        public String Path
        {
            get { return _path; }
        }

        public String Name
        {
            get { return _name; }
        }

        public String Img
        {
            get { return String.Format(@"{0}\Images\{1}", Environment.CurrentDirectory, _name); }
        }
    }
}
