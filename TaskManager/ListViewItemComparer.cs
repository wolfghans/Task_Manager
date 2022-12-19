using System;
using System.Collections;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SortOrder = System.Windows.Forms.SortOrder;

namespace TaskManager
{
    internal class ListViewItemComparer : IComparer
    {
        private int _Index;

        public int Index
        { 
            get { return _Index; }
            set { _Index = value; }
        }

        private SortOrder _sortDirection;

        public SortOrder SortDirection
        {
            get { return _sortDirection; }
            set { _sortDirection = value; }
        }

        //конструктор для класса
        public ListViewItemComparer()
        {
            _sortDirection = SortOrder.None;
        }


        public int Compare(object x, object y)
        {
           ListViewItem listViewItemX = x as ListViewItem;
           ListViewItem listViewItemY = y as ListViewItem;

            int result;

            switch(_Index)
            {
                case 0: 
                    result = string.Compare(listViewItemX.SubItems[_Index].Text, listViewItemY.SubItems[_Index].Text, false); 
                break;


                case 1:
                    double vX = double.Parse(listViewItemX.SubItems[_Index].Text);
                    double vY = double.Parse(listViewItemY.SubItems[_Index].Text);

                    result = vX.CompareTo(vY);
                break;


                default:
                    result = string.Compare(listViewItemX.SubItems[_Index].Text, listViewItemY.SubItems[_Index].Text, false);
                break;
            }
            
            if (_sortDirection == SortOrder.Descending)
            {
                return result;
            }
            else
            {
                return -result;
            }
        }
    }
}
