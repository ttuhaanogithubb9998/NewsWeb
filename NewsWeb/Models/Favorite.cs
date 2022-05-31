using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsWeb.Models
{
    public class Favorite
    {
        public int Id { set; get; }

        public bool? State { set; get; }
       
        public int? AccountId { set; get; }

        public Account Account { set; get; }

        public int? CommentId { get; set; }

        public Comment Comment { set; get; }

    }
}
