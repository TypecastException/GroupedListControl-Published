using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GroupedListControl
{
    public class ListGroupItemEventArgs : EventArgs
    {
        public ListGroupItemEventArgs(ListViewItem Item)
        {
            this.Item = Item;
        }


        public ListGroupItemEventArgs(ListViewItem[] Items)
        {
            this.Items = Items;
        }


        public ListViewItem Item { get; set; }
        public ListViewItem[] Items { get; set; }
    }
}
