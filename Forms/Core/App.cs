using SamplyGame.Ethereum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Urho;
using Urho.Forms;
using Xamarin.Forms;

namespace SamplyGame
{
    public class App : Xamarin.Forms.Application
    {
        public App()
        {
            var navPage = new NavigationPage(new UrhoPage());
      
            MainPage = navPage;
        }
    }


}


