using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalSelling.Data
{
    partial class Client
    {
        public int OrderCount
        {
            get
            {
                return App.DataBaseContext.Orders.Include("IdClientNavigation").Where(x => x.IdClientNavigation.Id == x.IdClient).Count();
            }
        }
    }
}
