using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NewsWeb.Models
{
    public class SavedPost
    {
        public int id { set; get; }

        public int AccountId { set; get; }

        public Account Account { set; get; }

        public int PostId { set; get; }
        
        public Post Post { set; get; }
    }
}
