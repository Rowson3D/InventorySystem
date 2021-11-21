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
    public partial class frmCustomer : Form
    {
        string sname, stype, sid, saddress, sphone, szip, semail;
        public frmCustomer(string name, string type, string id, string address, string phone, string zip, string email)
        {
            sname = name;
            stype = type;
            sid = id;
            saddress = address;
            sphone = phone;
            szip = zip;
            semail = email;
            InitializeComponent();
        }

        SQLConfig config = new SQLConfig();
        usableFunction funct = new usableFunction();
        string sql;
        private void frmCustomer_Load(object sender, EventArgs e)
        {
            UpdateProdList();
            InitializeColumns();
            ShowUserInfo();
            InitializePaymentDates();
        }

        // Create a method that shows the logged in users information
        private void ShowUserInfo()
        {
            txtCustomerInfo.Text = "Name: " + sname;
            txtCustomerInfo.Text += "\r\nAddress: " + saddress;
            txtCustomerInfo.Text += "\r\nZip: " + szip;
            txtCustomerInfo.Text += "\r\nE-mail: " + semail;
            txtCustomerInfo.Text += "\r\nPhone: " + funct.FormatPhoneNumber(sphone); // Formats phone #
            txtCustomerInfo.Text += "\r\nCustomer Level: " + funct.GetName(stype);
            txtCustomerInfo.Text += "\r\nCustomer Number: " + sid;

        }


        // Create a method that populates cboMonth and cboYear
        // cboMonth will be Jan - Dec
        // cboYear will be the current year - 20 years
        private void InitializePaymentDates()
        {

            // Populate cboYear
            int year = DateTime.Now.Year;
            for (int i = 0; i < 20; i++)
            {
                cboYear.Items.Add(year);
                year++;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            // This button will log the user out
            this.Close();

        }


        // Used for order generation //
        double total { get; set; }
        double shipping { get; set; }
        double tax { get; set; }
        double subtotal { get; set; }
        //                           //


        private void btnOrder_Click(object sender, EventArgs e)
        {   
                string order_id = funct.GenerateOrderNumber();
                string order_name = sname;
                string order_address = saddress;
                string order_shipper = funct.GetShipper();
                string order_date = funct.GetDate();
                // Using GetLastFourDigits to get last four digits of the credit card number
                int order_cc = Convert.ToInt32(funct.getLastFourDigits(txtCC.Text));
                // create string order_month and pad it with zeros if less than 10
                string order_month = Convert.ToString(cboMonth.SelectedIndex + 1);
                if (order_month.Length < 2)
                {
                    order_month = "0" + order_month;
                }
                // Using GetYear, to get the year from the cboYear
                int order_year = Convert.ToInt32(funct.getYear(cboYear));
                string order_exp = (order_month + "" + order_year);
                
                sql = "INSERT INTO tbl_Order_Details (order_id, order_name, order_address, order_shipper, order_total, order_shipping, order_date, order_cc, order_exp) VALUES ('" + order_id + "', '" + order_name + "', '" + order_address + "', '" + order_shipper + "', '" + total + "', '" + shipping + "', '" + order_date + "', '" + order_cc + "', '" + order_exp + "')";
                config.Execute_Query(sql);
                // Insert the items into the tbl_Order_Items table.
                // The four columns are: id, item, qty, price
                // The contents from dtgCart will be inserted into the tbl_Order_Items table.
                // id will be the order_id generated above.
                // item will be the product name
                // qty will be the quantity of the product as an integer
                // price will be the price of the product as a double
                // Ignore the last row of the dtgCart.
                for (int i = 0; i < dtgCart.Rows.Count - 1; i++)
                {
                    string item = dtgCart.Rows[i].Cells[1].Value.ToString();
                    int qty = Convert.ToInt32(dtgCart.Rows[i].Cells[5].Value);
                    double price = Convert.ToDouble(dtgCart.Rows[i].Cells[4].Value);
                    sql = "INSERT INTO tbl_Order_Items (order_id, item, qty, price) VALUES ('" + order_id + "', '" + item + "', '" + qty + "', '" + price + "')";
                    config.Execute_Query(sql);
                }
                // Create a messagebox that will display the order number and the email address of the customer
                // Make the messagebox a success message
                MessageBox.Show("Order #" + order_id + " has been placed.\r\n" + "Your order will be shipped to " + saddress + ".\nPlease check " + semail + ".", "Order Placed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateProdList();
                // Now clear the cart
                ClearCart();
        }
        // Create a method that will update the Tax and Total labels called lblTax and lblTotal
        // First calculate the subtotal using dtgCart price * dtgCart quantity
        // Then calculate the tax using the subtotal * 0.06
        // Then calculate the total using the subtotal + tax

        private void ClearCart()
        {
            dtgCart.Rows.Clear();
            lblTax.Text = "$0.00";
            lblTotal.Text = "$0.00";
            lblSubtotal.Text = "$0.00";
            lblShipping.Text = "$0.00";
        }
        
        public void UpdateTotal()
        {

            // For each row get price and quantity and calculate subtotal
            foreach (DataGridViewRow row in dtgCart.Rows)
            {
                subtotal += Convert.ToDouble(row.Cells[4].Value) * Convert.ToDouble(row.Cells[5].Value);
            }

            tax = subtotal * 0.06;
            shipping = subtotal * 0.05;
            total = subtotal + tax;
            lblShipping.Text = shipping.ToString("C");
            lblSubtotal.Text = subtotal.ToString("C");
            lblTax.Text = tax.ToString("C");
            lblTotal.Text = total.ToString("C");
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            sql = "SELECT prod_id as 'SKU', prod_name as 'Name', prod_desc as 'Description', prod_type as 'Type', prod_price as 'Price', prod_quan as 'Quantity', prod_unit as 'Packaging' from tbl_Products WHERE prod_id LIKE '%" + txtSearch.Text + "%'  OR prod_name LIKE '%" + txtSearch.Text + "%' OR prod_desc LIKE '%" + txtSearch.Text + "%' OR prod_type LIKE '%" + txtSearch.Text + "%'";
            config.Load_DTG(sql, dtgInventory);
        }

        private void dtgCart_KeyPress(object sender, KeyPressEventArgs e)
        {
            UpdateTotal();
        }

        private void dtgCart_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dtgCart_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            UpdateTotal();
        }

        private void btnDiscount_Click(object sender, EventArgs e)
        {

        }

        public void UpdateProdList()
        {
            config.Load_DTG("SELECT prod_id as 'SKU' , prod_name as 'Name', prod_desc as 'Description', prod_type as 'Type', prod_price as 'Price', prod_quan as 'Quantity', prod_unit as 'Packaging' from tbl_Products", dtgInventory);
        }

        // Create a new method initializes columns in dtgCart
        public void InitializeColumns()
        {
            dtgCart.Columns.Add("SKU", "SKU");
            dtgCart.Columns.Add("Name", "Name");
            dtgCart.Columns.Add("Description", "Description");
            dtgCart.Columns.Add("Type", "Type");
            dtgCart.Columns.Add("Price", "Price");
            dtgCart.Columns.Add("Quantity", "Quantity");
            dtgCart.Columns.Add("Packaging", "Packaging");
        }
        private void dtgInventory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // This button will add the selected row in dtgInventory to dtgCart
            // First check if the quantity is greater than 0
            // If it is, add the row to dtgCart
            // If it is not, show a messagebox saying that the quantity is 0
            // If the quantity is 0, show a messagebox saying that the product is out of stock
            if (dtgInventory.SelectedRows.Count > 0)
            {
                // Check if quantity is not 0 in dtgInventory and if the selected row column name doesn't already exist in dtgCart
                if (Convert.ToInt32(dtgInventory.SelectedRows[0].Cells[5].Value) > 0)
                {
                    dtgCart.Rows.Add(dtgInventory.SelectedRows[0].Cells[0].Value, dtgInventory.SelectedRows[0].Cells[1].Value, dtgInventory.SelectedRows[0].Cells[2].Value, dtgInventory.SelectedRows[0].Cells[3].Value, dtgInventory.SelectedRows[0].Cells[4].Value, 1, dtgInventory.SelectedRows[0].Cells[6].Value);
                    // Update the quantity in dtgInventory
                    dtgInventory.SelectedRows[0].Cells[5].Value = Convert.ToInt32(dtgInventory.SelectedRows[0].Cells[5].Value) - 1;
                    UpdateTotal();
                }
                else
                {
                    MessageBox.Show("This product is out of stock");
                }
            }
            else
            {
                MessageBox.Show("Please select a product");
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
        }
    }
}
