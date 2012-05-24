using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GroupedListControl
{
    class ListGroupEventArgs : System.EventArgs
    {
        public ListGroupEventArgs(ListGroup ListGroup)
        {

        }


        public ListGroup ListGroup { get; set; }
    }
}
