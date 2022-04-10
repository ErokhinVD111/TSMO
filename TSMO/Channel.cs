using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSMO
{
    internal class Channel
    {
        public bool IsActive { get; set; }

        public int Number { get; set; }

        public int IndexRequest { get; set; }


        /// <summary>
        /// Время обслуживания заявки
        /// </summary>
        public List<double> timeBusy = new();

        /// <summary>
        /// Время, когда заявка пришла и стала обслуживаться каналом
        /// </summary>
        public List<double> timeCommingRequest = new();

       
    }
}
