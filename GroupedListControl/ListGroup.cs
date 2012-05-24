using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using System.Runtime.InteropServices;
using System.ComponentModel;



namespace GroupedListControl
{
    public class ListGroup : ListView
    {
      
        #region MEMBER DECLARATIONS

        // The area occupied by the ListView header. 
        private Rectangle _headerRect;

        // The default ContextMenu strip, which needs to be left alone by the client:
        private ContextMenuStrip defaultContextMenuStrip;


        #region DELEGATES AND EVENT DEFINITIONS

        // NOTE: The events and delegates related to Column and Item Addition/Removal are
        // called by the inner classes ListGroupColumnCollection and LIstGroupItemCollection. 
        // The proper function of the control depends upon these. 

        // Delegates to handle Column addition and removal Events:
        public delegate void ColumnAddedHandler(object sender, ListGroupColumnEventArgs e);
        public delegate void ColumnRangeAddedHandler(object sender, EventArgs e);
        public delegate void ColumnRemovedHandler(object sender, ListGroupColumnEventArgs e);

        // Events related to Column Addition and removal:
        public event ColumnAddedHandler ColumnAdded;
        public event ColumnRangeAddedHandler ColumnRangeAdded;
        public event ColumnRemovedHandler ColumnRemoved;

        // Delegates to handle Item Addition and Removal events:
        public delegate void ItemAddedHandler(object sender, ListGroupItemEventArgs e);
        public delegate void ItemRangeAddedHandler(object sender, ListGroupItemEventArgs e);
        public delegate void ItemRemovedHandler(object sender, ListGroupItemEventArgs e);
        public delegate void ItemRemovedAtHandler(object sender, ListGroupItemEventArgs e);

        // Events related to Item Addition and Removal:
        public event ItemAddedHandler ItemAdded;
        public event ItemRangeAddedHandler ItemRangeAdded;
        public event ItemRemovedHandler ItemRemoved;
        public event ItemRemovedAtHandler ItemRemovedAt;

        // Delegate and related events to process Group Expansion and Collapse:
        public delegate void GroupExpansionHandler(object sender, EventArgs e);
        public event GroupExpansionHandler GroupExpanded;
        public event GroupExpansionHandler GroupCollapsed;

        // DElegate and related eevents to handle Listview Header Right Clicks:
        public delegate void ColumnRightClickHandler(object sender, ColumnClickEventArgs e);
        public event ColumnRightClickHandler ColumnRightClick;


        #endregion // DELEGATES AND EVENT DEFINITIONS


        #region INNER CLASS DECLARATIONS

        private ListGroupItemCollection _Items;
        private ListGroupColumnCollection _Columns;


        #endregion // INNER CLASS DECLARATION


        #region LOCAL STATIC MEMBERS

        // Text strings used as Image keys for the expanded/Collapsed image in the 
        // left-most columnHeader:
        static string COLLAPSED_IMAGE_KEY = "CollapsedImage";
        static string EXPANDED_IMAGE_KEY = "ExpandedImageKey";
        static string EMPTY_IMAGE_KEY = "EmptyImageKey";

        // "Magic number" approximates the height of the List View Column Header:
        static int HEADER_HEIGHT = 25;


        #endregion // LOCAL STATIC MEMBERS


        #endregion MEMBER DECLARATIONS


        #region CONSTRUCTOR

        public ListGroup() : base()
        {
            defaultContextMenuStrip = new ContextMenuStrip();
            defaultContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(regularListViewMenu_Opening);
            this.ContextMenuStrip = defaultContextMenuStrip;

            _Columns = new ListGroupColumnCollection(this);
            _Items = new ListGroupItemCollection(this);

            // The Imagelist is used to hold images for the expanded and contracted icons in the
            // Left-most columnheader:
            this.SmallImageList = new ImageList();

            // The tilting arrow images are available in the app resources:
            this.SmallImageList.Images.Add(COLLAPSED_IMAGE_KEY, Properties.Resources.CollapsedGroupSmall_png_1616);
            this.SmallImageList.Images.Add(EXPANDED_IMAGE_KEY, Properties.Resources.ExpandedGroupSmall_png_1616);
            this.SmallImageList.Images.Add(EMPTY_IMAGE_KEY, Properties.Resources.EmptyGroupSmall_png_1616);


            // The stateImageList is used as a hack method to allow larger Row Heights:
            this.StateImageList = new ImageList();

            // Default configuration (for this sample. Obviously, configure to fit your needs:
            this.View = System.Windows.Forms.View.Details;
            this.FullRowSelect = true;
            this.GridLines = true;
            this.LabelEdit = false;
            this.Margin = new Padding(0);
            this.SetAutoSizeMode(AutoSizeMode.GrowAndShrink);
            this.MaximumSize = new System.Drawing.Size(1000, 300);
            
            // Subscribe to local Events:
            this.ColumnClick += new ColumnClickEventHandler(ListGroup_ColumnClick);
           
        }


        #endregion // CONSTRUCTOR


        #region PROPERTIES


        /// <summary>
        /// Hides the ListViewItemCollection internal to the base class, and uses the 
        /// new implementation defined as an inner class, which sources an "ItemAdded" Event:
        /// </summary>
        public new ListGroupItemCollection Items
        {
            get { return _Items; }
        }



        /// <summary>
        /// Hides the ColumnHeaderCollection internal to the base class, and uses the 
        /// new implementation defined as an inner class, which sources a "ColumnAdded" Event:
        /// </summary>
        public new ListGroupColumnCollection Columns
        {
            get { return _Columns; }
        }



        /// <summary>
        /// Gets the total width of all column headers in the control:
        /// </summary>
        public int TotalColumnWidths
        {
            get { return this.Columns.TotalColumnWidths; }
        }


        /// <summary>
        /// Gets or sets the height of the ListViewItem rows. 
        /// </summary>
        public int RowHeight
        {
            get { return this.StateImageList.ImageSize.Height; }
            set
            {
                this.StateImageList.ImageSize = new System.Drawing.Size(value, value);
            }
        }



        #endregion // PROPERTIES


        #region ITEM ADDITION AND REMOMVAL EVENT HANDLING (Local)



        /// <summary>
        /// Raises the ItemAdded Event when a new item is
        /// added to the items collection.
        /// </summary>
        /// <param name="lvi"></param>
        private void OnItemAdded(ListViewItem lvi)
        {
            this.SetControlHeight();
            if(ItemAdded != null)
                this.ItemAdded(this, new ListGroupItemEventArgs(lvi));
        }


        /// <summary>
        /// Raises the ItemRangeAdded Event when a range of items are 
        /// added to the items collection.
        /// </summary>
        /// <param name="Items"></param>
        private void OnItemRangeAdded(ListViewItem[] Items)
        {
            this.SetControlHeight();
            if(ItemRangeAdded != null)
                this.ItemRangeAdded(this, new ListGroupItemEventArgs(Items));
        }


        /// <summary>
        /// Raises the ItemRemoved Event when an item is
        /// removed from the items collection.
        /// </summary>
        /// <param name="Item"></param>
        private void OnItemRemoved(ListViewItem Item)
        {
            this.Height = this.PreferredControlHeight();
            if(ItemRemoved != null)
                this.ItemRemoved(this, new ListGroupItemEventArgs(Item));
        }


        /// <summary>
        /// Raises the ItemRemovedAt Event when an item is
        /// removed from the items collection.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="Item"></param>
        private void OnItemRemovedAt(int index, ListViewItem Item)
        {
            this.Height = this.PreferredControlHeight();
            if(this.ItemRemovedAt != null)
                this.ItemRemovedAt(this, new ListGroupItemEventArgs(Item));
        }



        #endregion // ITEM ADDITION AND REMOVAL EVENT HANDLING (Local)


        #region COLUMNHEADER ADDITION AND REMOVAL EVENT HANDLING (Local)


        /// <summary>
        /// Raises the ColumnAdded Event when a new column is
        /// added to the ColumnHeaders collection.
        /// </summary>
        /// <param name="ColumnIndex"></param>
        private void OnColumnAdded(int ColumnIndex)
        {
            if (ColumnIndex == 0)
            {
                this.Columns[0].ImageKey = EMPTY_IMAGE_KEY;
                this.SetControlHeight();
            }

            if(this.ColumnAdded != null)
                this.ColumnAdded(this, new ListGroupColumnEventArgs(ColumnIndex));

        }

        

        void ListGroup_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (this.Height > HEADER_HEIGHT)

                // When the width of a column is changed, the Horizontal Scrollbar might
                // appear. If so, the height of this ListGroup will be adjusted slightly to
                // accomodate the scrollbar without partially obscuring the last visible item:
                this.Height = this.PreferredControlHeight();
        }



        /// <summary>
        /// Raises the ColumnRangeAdded Event when a range of new columns are
        /// added to the ColumnHeaders collection.
        /// </summary>
        /// <param name="Columns"></param>
        private void OnColumnRangeAdded(ColumnHeader[] Columns)
        {
            if(this.Columns.Count > 0)
                this.Columns[0].ImageKey = EXPANDED_IMAGE_KEY;
            
            if(this.ColumnRangeAdded != null)
                this.ColumnRangeAdded(this, new EventArgs());
        }



        /// <summary>
        /// Raises the ColumnRemoved Event when a column is
        /// remmoved from the ColumnHeaders collection.
        /// </summary>
        private void OnColumnRemoved(int ColumnIndex)
        {
            // REMOVE ColumnWidthChanged Handler before removing columns:
            this.ColumnWidthChanged -= new ColumnWidthChangedEventHandler(ListGroup_ColumnWidthChanged);

            this.Height = this.PreferredControlHeight();

            // RESTORE ColumnWidthChanged Event Handler:
            this.ColumnWidthChanged += new ColumnWidthChangedEventHandler(ListGroup_ColumnWidthChanged);


            if(this.ColumnRemoved != null)
                this.ColumnRemoved(this, new ListGroupColumnEventArgs(ColumnIndex));
        }



        /// <summary>
        /// Handles the ListGroup ColumnClick Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ListGroup_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            int columnClicked = e.Column;
            
            // The first column (Column[0]) is what activates the expansion/collapse of the 
            // List view item group:
            if (columnClicked == 0)
            {
                this.SuspendLayout();
                this.SetControlHeight();
                this.ResumeLayout();
            }
        }


        #endregion // COLUMNHEADER ADDITION AND REMOMVAL EVENT HANDLING (Local)


        #region CONTROL EXPANSION/COLLAPSE


        private int PreferredControlHeight()
        {
            int output = HEADER_HEIGHT;
            int rowHeight = 0;

            // determine the height of an individual list item:
            if(this.Items.Count > 0)
                rowHeight = this.Items[0].Bounds.Height;

            // In case the horizontal scrollbar makes an appearance, we will
            // need to modify the height of the expanded list so that it does not
            // obscure the last item (default is 10 px to leave a little space no mattter what:
            int horizScrollBarOffset = 10;

            // if the Width of the columns is greater than the width of the control, the vertical scroll bar will be shown. 
            // Increase that offset height by the height of the scrollbar (approximately the same as the height of a row):
            if (this.Columns.TotalColumnWidths > this.Width)
                horizScrollBarOffset = rowHeight + 10;

            // Increase the height of the control to accomodate the Columnheader, all of the current items, 
            // and the value of the horizontal scroll bar (if present):
            output = HEADER_HEIGHT + (this.Items.Count) * rowHeight + horizScrollBarOffset + this.Groups.Count * HEADER_HEIGHT;

            return output;
        }


        /// <summary>
        /// Causes the list of items to expand, showing all items in the 
        /// Items collection.
        /// </summary>
        public void Expand()
        {
            if (this.Columns.Count > 0)
            {
                this.Height = this.PreferredControlHeight();

                if (this.Items.Count > 0)
                    // Set the image in the first column to indicate an expanded state:
                    this.Columns[0].ImageKey = EXPANDED_IMAGE_KEY;
                else
                    // Set the image in the first column to indicate an empty state:
                    this.Columns[0].ImageKey = EMPTY_IMAGE_KEY;

                this.Scrollable = true;

                // Raise the Expanded event to notify client code that the ListGroup has expanded:
                if (this.GroupExpanded != null)
                    this.GroupExpanded(this, new EventArgs());
            }
        }



        /// <summary>
        /// Causes the Displayed list of items to collapse, hiding all items and 
        /// displaying only the columnheaders. 
        /// </summary>
        public void Collapse()
        {
            if (this.Columns.Count > 0)
            {
                this.Scrollable = false;

                // Collapse the ListGroup to show only the header:
                this.Height = HEADER_HEIGHT;

                if (this.Items.Count > 0)
                    // Set the image in the first column to indicate a collapsed state:
                    this.Columns[0].ImageKey = COLLAPSED_IMAGE_KEY;
                else
                    // Set the image in the first column to indicate an empty state:
                    this.Columns[0].ImageKey = EMPTY_IMAGE_KEY;

                // Raise the Collapsed event to notify client code that the ListGroup has expanded:
                if (this.GroupCollapsed != null)
                    this.GroupCollapsed(this, new EventArgs());
            }

        }


        /// <summary>
        /// Adjusts the item display area of the control in response to changes in the 
        /// expanded or collapsed state of the control. 
        /// </summary>
        public void SetControlHeight()
        {
            if (this.Height == HEADER_HEIGHT && this.Items.Count != 0)
                this.Expand();
            else
                this.Collapse();
        }

        #endregion CONTROL EXPANSION/COLLAPSE


        #region ListGroupItemCollection


        // Core Concept in this section was adapted from: http://www.codeproject.com/Articles/4406/An-Observer-Pattern-and-an-Extended-ListView-Event

        /// <summary>
        /// Inner class defined for ListGroup to contain List items. Derived from ListViewItemCollection
        /// and modified to source events indicating item addition and removal. 
        /// </summary>
        public class ListGroupItemCollection : System.Windows.Forms.ListView.ListViewItemCollection
        {
            private ListGroup _Owner;
            public ListGroupItemCollection(ListGroup Owner) : base(Owner)
            {
                _Owner = Owner;
            }


            /// <summary>
            /// New implementation of Add method hides Add method defined on base class
            /// and causes an event to be sourced informing the parent about item additions.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="text"></param>
            /// <param name="imageIndex"></param>
            /// <returns></returns>
            public new ListViewItem Add(string key, string text, int imageIndex)
            {
                ListViewItem item = base.Add(key, text, imageIndex);
                _Owner.OnItemAdded(item);
                return item;
            }


            /// <summary>
            /// New implementation of Add method hides Add method defined on base class
            /// and causes an event to be sourced informing the parent about item additions.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="text"></param>
            /// <param name="imageIndex"></param>
            /// <returns></returns>
            public new ListViewItem Add(string text)
            {
                ListViewItem item = base.Add(text);
                _Owner.OnItemAdded(item);
                return item;
            }


            
            /// <summary>
            /// New implementation of Add method hides Add method defined on base class
            /// and causes an event to be sourced informing the parent about item additions.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="text"></param>
            /// <param name="imageKey"></param>
            /// <returns></returns>
            public new ListViewItem Add(string key, string text, string imageKey)
            {
                ListViewItem item = base.Add(key, text, imageKey);
                _Owner.OnItemAdded(item);
                return item;
            }


            /// <summary>
            /// New implementation of Add method hides Add method defined on base class
            /// and causes an event to be sourced informing the parent about item additions.
            /// </summary>
            /// <param name="text"></param>
            /// <param name="imageIndex"></param>
            /// <returns></returns>
            public new ListViewItem Add(string text, int imageIndex)
            {
                ListViewItem item = base.Add(text, imageIndex);
                _Owner.OnItemAdded(item);
                return item;
            }



            /// <summary>
            /// New implementation of Add method hides Add method defined on base class
            /// and causes an event to be sourced informing the parent about item additions.
            /// </summary>
            /// <param name="text"></param>
            /// <param name="imageKey"></param>
            /// <returns></returns>
            public new ListViewItem Add(string text, string imageKey)
            {
                ListViewItem item = base.Add(text, imageKey);
                _Owner.OnItemAdded(item);
                return item;
            }



            /// <summary>
            /// New implementation of Add method hides Add method defined on base class
            /// and causes an event to be sourced informing the parent about item additions.
            /// </summary>
            /// <param name="Item"></param>
            public new void Add(ListViewItem Item)
            {
                base.Add(Item);
                _Owner.OnItemAdded(Item);
            }



            /// <summary>
            /// New implementation of Add method hides Add method defined on base class
            /// and causes an event to be sourced informing the parent about item additions.
            /// </summary>
            /// <param name="Item"></param>
            public new void AddRange(ListViewItem[] Item)
            {
                base.AddRange(Item);
                _Owner.OnItemRangeAdded(Item);
            }



            /// <summary>
            /// New implementation of Remove method hides Remove method defined on base class
            /// and causes an event to be sourced informing the parent about item Removals.
            /// </summary>
            /// <param name="Item"></param>
            public new void Remove(ListViewItem Item)
            {
                base.Remove(Item);
                _Owner.OnItemRemoved(Item);
            }


            /// <summary>
            /// New implementation of Remove method hides Remove method defined on base class
            /// and causes an event to be sourced informing the parent about item Removals.
            /// </summary>
            /// <param name="index"></param>
            public new void RemoveAt(int index)
            {
                System.Windows.Forms.ListViewItem Item = this[index];
                base.RemoveAt(index);
                _Owner.OnItemRemovedAt(index, Item);
            }

        } // ListGroupItemCollection
        
        
        #endregion // ListGroupItemCollection


        #region ListGroupColumnCollection


        // Core Concept in this section was adapted from: http://www.codeproject.com/Articles/4406/An-Observer-Pattern-and-an-Extended-ListView-Event

        /// <summary>
        /// Inner class defined for ListGroup to contain ColumnHeaders. Derived from ListView.ColumnHeaderCollection
        /// and modified to source events indicating column addition and removal. 
        /// </summary>
        public class ListGroupColumnCollection : ListView.ColumnHeaderCollection
        {
            // Reference to the containing ListGroup Control
            private ListGroup _Owner;

            public ListGroupColumnCollection(ListGroup Owner) : base(Owner)
            {
                _Owner = Owner;
            }



            /// <summary>
            /// Gets the total width of all columns currently defined in the control.
            /// </summary>
            public int TotalColumnWidths
            {
                get 
                {
                    int totalColumnWidths = 0;
                    foreach (ColumnHeader clm in this)
                        totalColumnWidths = totalColumnWidths + clm.Width;
                    return totalColumnWidths; 
                }
            }


            /// <summary>
            /// Adds a column to the current collection and raises the OnColumnAddedEvent on the parent control.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public new int Add(ColumnHeader value)
            {
                int clm = base.Add(value);
                _Owner.OnColumnAdded(clm);
                return clm;
            }


            /// <summary>
            /// Adds a column to the current collection and raises the OnColumnAddedEvent on the parent control.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="text"></param>
            /// <returns></returns>
            public new ColumnHeader Add(string key, string text)
            {
                ColumnHeader clm = base.Add(key, text);
                _Owner.OnColumnAdded(clm.Index);
                return clm;
            }


            /// <summary>
            /// Adds a column to the current collection and raises the OnColumnAddedEvent on the parent control.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="text"></param>
            /// <param name="width"></param>
            /// <returns></returns>
            public new ColumnHeader Add(string key, string text, int width)
            {
                ColumnHeader clm = base.Add(key, text, width);
                _Owner.OnColumnAdded(clm.Index);
                return clm;
            }


            /// <summary>
            /// Adds a column to the current collection and raises the OnColumnAddedEvent on the parent control.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="text"></param>
            /// <param name="width"></param>
            /// <param name="textAlign"></param>
            /// <param name="imageIndex"></param>
            /// <returns></returns>
            public new ColumnHeader Add(string key, string text, int width, HorizontalAlignment textAlign, int imageIndex)
            {
                ColumnHeader clm = base.Add(key, text, width, textAlign, imageIndex);
                _Owner.OnColumnAdded(clm.Index);
                return clm;
            }


            /// <summary>
            /// Adds a column to the current collection and raises the OnColumnAddedEvent on the parent control.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="text"></param>
            /// <param name="width"></param>
            /// <param name="textAlign"></param>
            /// <param name="imageKey"></param>
            /// <returns></returns>
            public new ColumnHeader Add(string key, string text, int width, HorizontalAlignment textAlign, string imageKey)
            {
                ColumnHeader clm = base.Add(key, text, width, textAlign, imageKey);
                _Owner.OnColumnAdded(clm.Index);
                return clm;
            }


            /// <summary>
            /// Adds a column to the current collection and raises the OnColumnAddedEvent on the parent control.
            /// </summary>
            /// <param name="text"></param>
            /// <returns></returns>
            public new ColumnHeader Add(string text)
            {
                ColumnHeader clm = base.Add(text);
                _Owner.OnColumnAdded(clm.Index);
                return clm;
            }


            /// <summary>
            /// Adds a column to the current collection and raises the OnColumnAddedEvent on the parent control.
            /// </summary>
            /// <param name="text"></param>
            /// <param name="width"></param>
            /// <returns></returns>
            public new ColumnHeader Add(string text, int width)
            {
                ColumnHeader clm = base.Add(text, width);
                _Owner.OnColumnAdded(clm.Index);
                return clm;
            }


            /// <summary>
            /// Adds a column to the current collection and raises the OnColumnAddedEvent on the parent control.
            /// </summary>
            /// <param name="text"></param>
            /// <param name="width"></param>
            /// <param name="textAlign"></param>
            /// <returns></returns>
            public new ColumnHeader Add(string text, int width, HorizontalAlignment textAlign)
            {
                ColumnHeader clm = base.Add(text, width, textAlign);
                _Owner.OnColumnAdded(clm.Index);
                return clm;
            }


            /// <summary>
            /// Adds a range of columns to the current collection and raises the OnColumnAdded Event on the parent control.
            /// </summary>
            /// <param name="values"></param>
            public new void  AddRange(ColumnHeader[] values)
            {
 	             base.AddRange(values);
                 _Owner.OnColumnRangeAdded(values);
            }


            /// <summary>
            /// Removes a column from the current collection and raises the OnColumnRemoved Event on the parent control.
            /// </summary>
            /// <param name="column"></param>
            public new void Remove(ColumnHeader column)
            {
                int index = column.Index;
                base.Remove(column);
                _Owner.OnColumnRemoved(index);
            }


            /// <summary>
            /// Removes a column from the current collection and raises the OnColumnRemoved Event on the parent control.
            /// </summary>
            /// <param name="index"></param>
            public new void RemoveAt(int index)
            {
                ColumnHeader clm = this[index];
                base.RemoveAt(index);
                _Owner.OnColumnRemoved(index);
            }

            /// <summary>
            /// Removes a column from the current collection and raises the OnColumnRemoved Event on the parent control.
            /// </summary>
            /// <param name="key"></param>
            public new void RemoveByKey(string key)
            {
                ColumnHeader clm = this[key];
                int index = clm.Index;              
                base.RemoveByKey(key);
                _Owner.OnColumnRemoved(index);
            }


            public new void Clear()
            {
                base.Clear();
            }

        } // ListGroupColumnCollection





        #endregion // ListGroupColumnCollection


        #region ListViewColumnHeader Right Click Event Sourcing

        // The following code was adapted from: http://www.codeproject.com/Articles/23330/Handling-Right-Click-Events-in-ListView-Column-Hea

        // This returns an array of ColumnHeaders in the order they are
        // displayed by the ListView.  
        private static ColumnHeader[] GetOrderedHeaders(ListView lv)
        {
            ColumnHeader[] arr = new ColumnHeader[lv.Columns.Count];

            foreach (ColumnHeader header in lv.Columns)
            {
                arr[header.DisplayIndex] = header;
            }

            return arr;
        }


        // Called when the user right-clicks anywhere in the ListView, including the
        // header bar.  It displays the appropriate context menu for the data row or
        // header that was right-clicked. 
        private void regularListViewMenu_Opening(object sender, CancelEventArgs e)
        {
            // This call indirectly calls EnumWindowCallBack which sets _headerRect
            // to the area occupied by the ListView's header bar.
            EnumChildWindows(this.Handle, new EnumWinCallBack(EnumWindowCallBack), IntPtr.Zero);

            // If the mouse position is in the header bar, cancel the display
            // of the regular context menu and display the column header context menu instead.
            if (_headerRect.Contains(Control.MousePosition))
            {
                e.Cancel = true;

                // The xoffset is how far the mouse is from the left edge of the header.
                int xoffset = Control.MousePosition.X - _headerRect.Left;

                // Iterate through the column headers in the order they are displayed, adding up
                // their widths as we go.  When the sum exceeds the xoffset, we know the mouse
                // is on the current header. 
                int sum = 0;
                foreach (ColumnHeader header in GetOrderedHeaders(this))
                {
                    sum += header.Width;
                    if (sum > xoffset)
                    {
                        OnColumnRightClick(header);
                        break;
                    }
                }
            }
            else
            {
                // Allow the regular context menu to be displayed.
                // I am not using the regular menu, so this is empty. 
            }
        }

        // Called when the specified column header is right-clicked.
        private void OnColumnRightClick(ColumnHeader header)
        {
            this.ColumnRightClick(this, new ColumnClickEventArgs(header.Index));
        }


        // This should get called with the only child window of the ListView,
        // which should be the header bar.
        private bool EnumWindowCallBack(IntPtr hwnd, IntPtr lParam)
        {
            // Determine the rectangle of the ListView header bar and save it in _headerRect.
            RECT rct;

            if (!GetWindowRect(hwnd, out rct))
            {
                _headerRect = Rectangle.Empty;
            }
            else
            {
                _headerRect = new Rectangle(rct.Left, rct.Top, rct.Right - rct.Left, rct.Bottom - rct.Top);
            }

            return false; // Stop the enum
        }


        // Delegate that is called for each child window of the ListView.
        private delegate bool EnumWinCallBack(IntPtr hwnd, IntPtr lParam);

        // Calls EnumWinCallBack for each child window of hWndParent (i.e. the ListView).
        [DllImport("user32.Dll")]
        private static extern int EnumChildWindows(IntPtr hWndParent, EnumWinCallBack callBackFunc, IntPtr lParam);

        // Gets the bounding rectangle of the specified window (ListView header bar).
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }


        #endregion // ListViewColumnHeader Right Click Event Sourcing
    }



}
