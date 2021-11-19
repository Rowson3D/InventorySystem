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
            // Populate cboMonth
            cboMonth.Items.Add("Jan");
            cboMonth.Items.Add("Feb");
            cboMonth.Items.Add("Mar");
            cboMonth.Items.Add("Apr");
            cboMonth.Items.Add("May");
            cboMonth.Items.Add("Jun");
            cboMonth.Items.Add("Jul");
            cboMonth.Items.Add("Aug");
            cboMonth.Items.Add("Sep");
            cboMonth.Items.Add("Oct");
            cboMonth.Items.Add("Nov");
            cboMonth.Items.Add("Dec");

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

                sql = "INSERT INTO tbl_Order_Details (order_id, order_name, order_address, order_shipper, order_total, order_shipping, order_date) VALUES ('" + order_id + "', '" + order_name + "', '" + order_address + "', '" + order_shipper + "', '" + total + "', '" + shipping + "', '" + order_date + "')";
                config.Execute_Query(sql);
                // Create a messagebox that will display the order number and the email address of the customer
                MessageBox.Show("Order Number: " + order_id + "\r\n" + "E-mail: " + semail);
                UpdateProdList();
        }
        // Create a method that will update the Tax and Total labels called lblTax and lblTotal
        // First calculate the subtotal using dtgCart price * dtgCart quantity
        // Then calculate the tax using the subtotal * 0.06
        // Then calculate the total using the subtotal + tax
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
            // For every key pressed in the search box
            // Show the products that match the search criteria
            // If the search box is empty, show all products
            if (txtSearch.Text == "")
            {
                UpdateProdList();
            }
            else
            {
            sql = "SELECT * FROM tbl_Products WHERE prod_name LIKE '%" + txtSearch.Text + "%'";
            config.Load_DTG(sql, dtgInventory);
            }
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
