using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDauTienVS2._0.Models
{
    public class QuestionsContain
    {
        public long C_ID_Q { set; get; }
        public int C_Parent { set; get; }
        public string C_Title { set; get; }
        public string C_Contents { set; get; }
        public string C_CreaData { set; get; }
        public string C_ModiData { set; get; }
        public string C_Username { set; get; }
        public bool C_Hide { set; get; }
        public string C_Fullname { set; get; }
        public string C_Avatar { set; get; }
        public int C_Likes { set; get; }
        public int C_Cups { set; get; }
        public int C_Dislike { set; get; }
        public int C_Liked { set; get; }
    }
}