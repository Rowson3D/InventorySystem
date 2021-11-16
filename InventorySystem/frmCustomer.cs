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
        string sname, stype, sid, saddress, sphone, szip;
        public frmCustomer(string name, string type, string id, string address, string phone, string zip)
        {
            sname = name;
            stype = type;
            sid = id;
            saddress = address;
            sphone = phone;
            szip = zip;
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

        private void btnOrder_Click(object sender, EventArgs e)
        {

        }

        // Create a method that will update the Tax and Total labels called lblTax and lblTotal
        // First calculate the subtotal using dtgCart price * dtgCart quantity
        // Then calculate the tax using the subtotal * 0.06
        // Then calculate the total using the subtotal + tax
        private void UpdateTotal()
        {
            double subtotal = 0;
            double tax = 0;
            double total = 0;
            double shipping = 0;

            // For each row number 4 in the datagridview
            // Add them all up and store them in subtotal
            foreach (DataGridViewRow row in dtgCart.Rows)
            {
                subtotal += Convert.ToDouble(row.Cells[4].Value);
            }

            tax = subtotal * 0.06;
            shipping = tax * 0.05;
            total = subtotal + tax;
            lblShipping.Text = shipping.ToString("C");
            lblSubtotal.Text = subtotal.ToString("C");
            lblTax.Text = tax.ToString("C");
            lblTotal.Text = total.ToString("C");
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
            // This button will add the selected product to the cart
            // - 1 from the quantity of the item selected
            // - Add the product to the cart
            // - Update the total
            // - Update the quantity of the product in the inventory
            // - Update the dtgInventory
            // - Update the dtgCart
            // - Update the total
            if (dtgInventory.SelectedRows.Count > 0)
            {
                string sku = dtgInventory.SelectedRows[0].Cells[0].Value.ToString();
                string name = dtgInventory.SelectedRows[0].Cells[1].Value.ToString();
                string desc = dtgInventory.SelectedRows[0].Cells[2].Value.ToString();
                string type = dtgInventory.SelectedRows[0].Cells[3].Value.ToString();
                string price = dtgInventory.SelectedRows[0].Cells[4].Value.ToString();
                string quan = dtgInventory.SelectedRows[0].Cells[5].Value.ToString();
                string unit = dtgInventory.SelectedRows[0].Cells[6].Value.ToString();

                // Check if the product is already in the cart
                bool found = false;
                if (dtgCart.Rows.Count > 1)
                {
                    dtgCart.Rows.Add(sku, name, desc, type, price, quan, unit);
                    UpdateTotal();
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
