using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace CoffeeBot.Models
{
    // Defines a state property used to track information about the user.
    public class UserData
    {
        // public String Id { get; set; }

        public String Name { get; set; }
        // public String CoffeeMenu { get; set; }
        // public String CoffeeSize { get; set; }
        // public String CoffeeTemp { get; set; }


        public List<CoffeeOrder> Orders { get; set; } = new List<CoffeeOrder>();
    }
}
