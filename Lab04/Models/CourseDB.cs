using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lab04.Models
{
    public partial class Course
    {
        public string Name;
        public string LectureName;
        public List<Category> ListCategory = new List<Category>();

        public bool isLogin = false;
        public bool isShowGoing = false;
        public bool isShowFollow = false;

    }
}