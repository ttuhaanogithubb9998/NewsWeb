using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace NewsWeb.Models
{
    public class Comment
    {   
        public int Id { set; get; }

        [DisplayName("Thời gian")]

        public DateTime Date { set; get; }

        public string Description { set; get; }

        public int PostId { set; get; }
        public Post Post { set; get; }

        public int? AccountId { set; get; }
        public Account Account { get; set; }

        public List<Favorite> Favorites { set; get; }

    }
}
