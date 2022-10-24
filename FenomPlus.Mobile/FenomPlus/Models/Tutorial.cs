using System;

using Xamarin.Forms;

namespace FenomPlus.Models
{
    public class Tutorial
    {
        public string Title { get; set; }
        public string Info { get; set; }
        public string Illustration { get; set; }
        public bool ShowImage { get { return (!ShowStep); } }
        public bool ShowGuage { get { return (ShowStep); } }
        public bool ShowStep { get; set; }
    }
}

