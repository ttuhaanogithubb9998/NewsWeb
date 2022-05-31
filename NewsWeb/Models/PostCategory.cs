using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NewsWeb.Models
{
    public class PostCategory
    {
        public int Id { set; get; }

        public string Name { set; get; }

        public List<Post> Posts { set; get; }
    }
}
