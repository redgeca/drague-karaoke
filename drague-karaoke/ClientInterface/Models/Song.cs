using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClientInterface.Models
{
    public class Song
    {
        public long mId { get; set; }
        public String mTitle { get; set; }
        public long mCategory { get; set; }
        public String mContent { get; set; }

 //       [Key]
 //       public Guid uniqueKey { get; set; }
    }
 }