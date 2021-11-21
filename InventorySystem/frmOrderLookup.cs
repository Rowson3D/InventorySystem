using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventorySystem
{
    public partial class frmOrderLookup : Form
    {
        public frmOrderLookup()
        {
            InitializeComponent();
        }

        SQLConfig config = new SQLConfig();
        usableFunction funct = new usableFunction();
        string sql;

        private void frmOrderLookup_Load(object sender, EventArgs e)
        {
            UpdateOrderList();
        }

        // Create a method called UpdateOrderList()
        // This method will populate the data grid view called dtgOrders with all the orders
        // from the database table tbl_Order_Details
        private void UpdateOrderList()
        {
            // Create a string variable called sql and set it to the following SQL statement
            // SELECT * FROM tbl_Order_Details
        config.Load_DTG("SELECT order_id as 'Order ID', order_name as 'Name', order_address as 'Address', order_shipper as 'Shipper', order_total as 'Total', order_shipping as 'Shipping Cost', order_date as 'Order Date', order_cc as 'Credit Card', order_exp as 'Expiration' from tbl_Order_Details", dtgOrders);
        }
    }
}
