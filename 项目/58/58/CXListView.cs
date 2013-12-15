using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.InteropServices;

namespace CXListView
{
    public partial class CXListView : ListView
    {

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wPar, IntPtr lPar);

        // ListView messages
        private const int LVM_FIRST = 0x1000;
        private const int LVM_GETCOLUMNORDERARRAY = (LVM_FIRST + 59);

        // Windows Messages
        private const int WM_PAINT = 0x000F;

        public CXListView()
        {
            InitializeComponent();

            this.OwnerDraw = true;
            this.FullRowSelect = true;
            this.View = View.Details;
            this.MultiSelect = false;
        }

        /// <summary>
        /// Structure to hold an embedded control's info
        /// </summary>
        private struct EmbeddedControl
        {
            public Control Control;
            public int Column;
            public int Row;
            public DockStyle Dock;
            public ListViewItem Item;
        }

        private ArrayList _embeddedControls = new ArrayList();

        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            base.OnDrawSubItem(e);
        }
        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            base.OnDrawItem(e);
            e.DrawDefault = true;
        }

        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            base.OnDrawColumnHeader(e);
            e.DrawDefault = true;
        }

        public void AddControlToSubItem(int col, int row, EXControlListViewSubItem subitem,DockStyle Dock)
        {
            //control.Bounds = subitem.Bounds;
            if (col >= Columns.Count || row >= Items.Count)
                throw new ArgumentOutOfRangeException();

            EmbeddedControl ec;
            ec.Control = subitem.Btn;
            ec.Column = col;
            ec.Row = row;
            ec.Dock = Dock;
            ec.Item = Items[row];

            _embeddedControls.Add(ec);

            subitem.Btn.Bounds = subitem.Bounds;
            subitem.Btn.Click += new EventHandler(SubItemBtn_Click);
            this.Controls.Add(subitem.Btn);
        }

        /// <summary>
        /// Retrieve the order in which columns appear
        /// </summary>
        /// <returns>Current display order of column indices</returns>
        protected int[] GetColumnOrder()
        {
            IntPtr lPar = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)) * Columns.Count);

            IntPtr res = SendMessage(Handle, LVM_GETCOLUMNORDERARRAY, new IntPtr(Columns.Count), lPar);
            if (res.ToInt32() == 0)	// Something went wrong
            {
                Marshal.FreeHGlobal(lPar);
                return null;
            }

            int[] order = new int[Columns.Count];
            Marshal.Copy(lPar, order, 0, Columns.Count);

            Marshal.FreeHGlobal(lPar);

            return order;
        }

        /// <summary>
        /// Retrieve the bounds of a ListViewSubItem
        /// </summary>
        /// <param name="Item">The Item containing the SubItem</param>
        /// <param name="SubItem">Index of the SubItem</param>
        /// <returns>Subitem's bounds</returns>
        protected Rectangle GetSubItemBounds(ListViewItem Item, int SubItem)
        {
            Rectangle subItemRect = Rectangle.Empty;

            if (Item == null)
                throw new ArgumentNullException("Item");

            int[] order = GetColumnOrder();
            if (order == null) // No Columns
                return subItemRect;

            if (SubItem >= order.Length)
                throw new IndexOutOfRangeException("SubItem " + SubItem + " out of range");

            // Retrieve the bounds of the entire ListViewItem (all subitems)
            Rectangle lviBounds = Item.GetBounds(ItemBoundsPortion.Entire);
            int subItemX = lviBounds.Left;

            // Calculate the X position of the SubItem.
            // Because the columns can be reordered we have to use Columns[order[i]] instead of Columns[i] !
            ColumnHeader col;
            int i;
            for (i = 0; i < order.Length; i++)
            {
                col = this.Columns[order[i]];
                if (col.Index == SubItem)
                    break;
                subItemX += col.Width;
            }

            subItemRect = new Rectangle(subItemX, lviBounds.Top, this.Columns[order[i]].Width, lviBounds.Height);

            return subItemRect;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_PAINT:
                    if (View != View.Details)
                        break;

                    // Calculate the position of all embedded controls
                    foreach (EmbeddedControl ec in _embeddedControls)
                    {
                        Rectangle rc = this.GetSubItemBounds(ec.Item, ec.Column);

                        if ((this.HeaderStyle != ColumnHeaderStyle.None) &&
                            (rc.Top < this.Font.Height)) // Control overlaps ColumnHeader
                        {
                            ec.Control.Visible = false;
                            continue;
                        }
                        else
                        {
                            ec.Control.Visible = true;
                        }

                        switch (ec.Dock)
                        {
                            case DockStyle.Fill:
                                break;
                            case DockStyle.Top:
                                rc.Height = ec.Control.Height;
                                break;
                            case DockStyle.Left:
                                rc.Width = ec.Control.Width;
                                break;
                            case DockStyle.Bottom:
                                rc.Offset(0, rc.Height - ec.Control.Height);
                                rc.Height = ec.Control.Height;
                                break;
                            case DockStyle.Right:
                                rc.Offset(rc.Width - ec.Control.Width, 0);
                                rc.Width = ec.Control.Width;
                                break;
                            case DockStyle.None:
                                rc.Size = ec.Control.Size;
                                break;
                        }

                        // Set embedded control's bounds
                        ec.Control.Bounds = rc;
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        public class EXControlListViewSubItem : System.Windows.Forms.ListViewItem.ListViewSubItem
        {
            private Button SubItemBtn;
            public EXControlListViewSubItem()
            {
                SubItemBtn = new Button();
            }

            public Button Btn
            {
                get { return SubItemBtn; }
                set { SubItemBtn = value; }
            }
        }

        public void SubItemBtn_Click(object sender, EventArgs e)
        {
            foreach (EmbeddedControl ec in _embeddedControls)
            {
                if (ec.Control == (Control)sender)
                {
                    this.SelectedItems.Clear();
                    ec.Item.Selected = true;
                }
            }
        }
        
    }
}
