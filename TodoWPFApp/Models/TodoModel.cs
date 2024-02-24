using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoWPFApp.Models
{
    public class TodoModel
    {
        private bool _isDone;
        private string _description;
        public DateTime CreationDate { get; set; } = DateTime.Now;

        public bool IsDone
        {
            get { return _isDone; }
            set { _isDone = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }
}
