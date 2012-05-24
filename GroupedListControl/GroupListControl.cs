using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using System.Runtime.InteropServices;

namespace GroupedListControl
{

    public class GroupListControl : FlowLayoutPanel
    {

        public GroupListControl()
        {
            // Default configuration. Adapt to suit your needs:
            this.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.AutoScroll = true;
            this.WrapContents = false;

            // Add a local handler for the ControlAdded Event.
            this.ControlAdded += new ControlEventHandler(GroupListControl_ControlAdded);
        }


        /// <summary>
        /// COnsumed by the Win API calls below:
        /// </summary>
        private enum ScrollBarDirection
        {
            SB_HORZ = 0,
            SB_VERT = 1,
            SB_CTL = 2,
            SB_BOTH = 3
        }


        /// <summary>
        /// Disables the horizontal scrollbar in the primary container control.
        /// Individual ListGroups within the GroupList have their own scrollbars
        /// if needed. 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="wBar"></param>
        /// <param name="bShow"></param>
        /// <returns></returns>
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            // Call to unmanaged WinAPI:
            ShowScrollBar(this.Handle, (int)ScrollBarDirection.SB_HORZ, false);
            base.WndProc(ref m);
        }


        /// <summary>
        /// Imported from WinAPI: Method to control Scrollbar visibility.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="wBar"></param>
        /// <param name="bShow"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);


        /// <summary>
        /// Handles the ControlAdded Event for the current instance. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GroupListControl_ControlAdded(object sender, ControlEventArgs e)
        {
            ListGroup lg = (ListGroup)e.Control;
            lg.Width = this.Width;
            lg.GroupCollapsed += new ListGroup.GroupExpansionHandler(lg_GroupCollapsed);
            lg.GroupExpanded += new ListGroup.GroupExpansionHandler(lg_GroupExpanded);
        }


        /// <summary>
        /// Gets or Sets a boolean value indicating whether multiple ListGroups
        /// may be in the expanded state at the same time. When set to true, the current expanded 
        /// ListGroup is collapsed when a new ListGroup is expanded. 
        /// </summary>
        public bool SingleItemOnlyExpansion { get; set; }


        /// <summary>
        /// Handles the Expanded event for the current instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void lg_GroupExpanded(object sender, EventArgs e)
        {
            // Grab a reference to the ListGroup which sent the message:
            ListGroup expanded = (ListGroup)sender;
            
            // If Single item only expansion, collapse all ListGroups in except
            // the one currently exanding:
            if(this.SingleItemOnlyExpansion)
            {
                this.SuspendLayout();
                foreach(ListGroup lg in this.Controls)
                {
                    if (!lg.Equals(expanded))
                        lg.Collapse();
                }
                this.ResumeLayout(true);
            }

        }


        /// <summary>
        /// Handles the Collapsed event for the current instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void lg_GroupCollapsed(object sender, EventArgs e)
        {
            // No need.
        }


        /// <summary>
        /// Expands all listgroups contained in the current instance. 
        /// </summary>
        public void ExpandAll()
        {
            foreach(ListGroup lg in this.Controls)
            {
                lg.Expand();
            }
        }


        /// <summary>
        /// Collapses all ListGroups contained in the current instance.
        /// </summary>
        public void CollapseAll()
        {
            foreach (ListGroup lg in this.Controls)
            {
                lg.Collapse();
            }
        }

    }
   
}
