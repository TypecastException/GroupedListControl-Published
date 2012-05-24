using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GroupedListControl;

namespace GroupedListDemo
{
    public partial class Form1 : Form
    {
        // A ContextMenuStrip as member variable. We need to use this instead of the 
        // context menus available in each ListGroup:
        ContextMenuStrip _ListGroupContextMenu;
        ToolStripMenuItem _addOption = new ToolStripMenuItem("Add");
        ToolStripMenuItem _editOption = new ToolStripMenuItem("Edit");
        ToolStripMenuItem _deleteOption = new ToolStripMenuItem("Delete");

        public Form1()
        {
            InitializeComponent();

            //  Set up the context menu to use with the GroupedList Items:
            _ListGroupContextMenu = new ContextMenuStrip();

            // Add some sample ContextMenuStrip Items:
            _addOption = new ToolStripMenuItem("Add");
            _addOption.Click += new EventHandler(addOption_Click);
            _ListGroupContextMenu.Items.Add(_addOption);

            _editOption = new ToolStripMenuItem("Edit");
            _editOption.Click += new EventHandler(editOption_Click);
            _ListGroupContextMenu.Items.Add(_editOption);

            _deleteOption = new ToolStripMenuItem("Delete");
            _deleteOption.Click += new EventHandler(deleteOption_Click);
            _ListGroupContextMenu.Items.Add(_deleteOption);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Create a GroupedListControl instance:
            GroupListControl glc = this.groupListControl1;
            glc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            
            // Add some sample columns:
            for(int i = 1; i <=5; i++)
            {
                ListGroup lg = new ListGroup();
                lg.Columns.Add("List Group " + i.ToString(), 120);
                lg.Columns.Add("Group " + i + " SubItem 1", 150);
                lg.Columns.Add("Group " + i + " Subitem 2", 150);
                lg.Name = "Group " + i;

                // Now add some sample items:
                for (int j = 1; j <= 5; j++)
                {
                    ListViewItem item = lg.Items.Add("Item " + j.ToString());
                    item.SubItems.Add(item.Text + " SubItem 1");
                    item.SubItems.Add(item.Text + " SubItem 2");
                }

                // Add handling for the columnRightClick Event:
                lg.ColumnRightClick += new ListGroup.ColumnRightClickHandler(lg_ColumnRightClick);
                lg.MouseClick += new MouseEventHandler(lg_MouseClick);

                glc.Controls.Add(lg);
            }
        }



        void lg_MouseClick(object sender, MouseEventArgs e)
        {
            ListGroup lg = (ListGroup)sender;

            ListViewHitTestInfo info = lg.HitTest(e.X, e.Y);
            ListViewItem item = info.Item;

            if (e.Button == MouseButtons.Right)
            {
                // Tuck the Active ListGroup into the Tag property:
                _ListGroupContextMenu.Tag = lg;

                // Make sure the Delete and Edit options are enabled:
                _editOption.Enabled = true;
                _deleteOption.Enabled = true;

                // Because we are not using the GroupedList's own ContextMenuStrip, 
                // we need to use the PointToClient method so that the menu appears 
                // in the correct position relative to the control:
                _ListGroupContextMenu.Show(lg, lg.PointToClient(MousePosition));
            }

        }



        void lg_ColumnRightClick(object sender, ColumnClickEventArgs e)
        {
            ListGroup lg = (ListGroup)sender;

            // Tuck the Active ListGroup into the Tag property:
            _ListGroupContextMenu.Tag = lg;

            // If the header is right-clicked, the user has not indicated an item to edit or delete.
            // Disable those options:
            _editOption.Enabled = false;
            _deleteOption.Enabled = false;

            // Because we are not using the GroupedList's own ContextMenuStrip, 
            // we need to use the PointToClient method so that the menu appears 
            // in the correct position relative to the control:
            _ListGroupContextMenu.Show(lg, lg.PointToClient(MousePosition));
        }



        void deleteOption_Click(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem)sender;
            var menu = menuItem.Owner;
            var selectedGroup = (ListGroup)menu.Tag;

            // Only one selected item allowed for this demo:
            var selectedItem = selectedGroup.SelectedItems[0];

            string groupName = selectedGroup.Name;
            string itemName = selectedItem.Text;

            MessageBox.Show("Delete  " + itemName + " from " + selectedGroup.Name);
        }



        void editOption_Click(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem)sender;
            var menu = menuItem.Owner;
            var selectedGroup = (ListGroup)menu.Tag;

            // Only one selected item allowed for this demo:
            var selectedItem = selectedGroup.SelectedItems[0];

            string groupName = selectedGroup.Name;
            string itemName = selectedItem.Text;

            MessageBox.Show("Edit  " + itemName + " from " + selectedGroup.Name);
        }



        void addOption_Click(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem)sender;
            var menu = menuItem.Owner;
            var selectedGroup = (ListGroup)menu.Tag;

            MessageBox.Show("Add a new item to " + selectedGroup.Name);
        }


        // Determine whether or not to use SingleItemOnly Expansion:
        private void chkSingleItemOnlyMode_CheckedChanged(object sender, EventArgs e)
        {
            this.groupListControl1.SingleItemOnlyExpansion = this.chkSingleItemOnlyMode.Checked;
            if (this.groupListControl1.SingleItemOnlyExpansion)
            {
                this.groupListControl1.CollapseAll();
            }
            else
            {
                this.groupListControl1.ExpandAll();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach(ListGroup lg in this.groupListControl1.Controls)
            {
                lg.Columns.RemoveAt(lg.Columns.Count-1);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (ListGroup lg in this.groupListControl1.Controls)
            {
                lg.Items.RemoveAt(lg.Items.Count - 1);
            }
        }


        


    }
}
