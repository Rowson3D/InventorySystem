using Microsoft.Reporting.WebForms;
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
    public partial class frmReports : Form
    {
        public frmReports()
        {
            InitializeComponent();
        }
        SQLConfig config = new SQLConfig();
        string sql;

        private void reports(string sql, string rptname)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        

        private void btnInventory_Click(object sender, EventArgs e)
        {
            // When clicking the button, populate the multiline textbox with a console style report of the inventory
            sql = "SELECT * FROM tbl_Products";
            config.sqlQuery(sql);
            txtReport.Text = "";
            txtReport.Text = "Inventory Report\r\n";
            // Header will use prod_id, prod_name, prod_desc, prod_type, prod_price, prod_quan, prod_unit
            txtReport.Text += "ID\tName\tDescription\tType\tPrice\tQuantity\tUnit\r\n";
            // Create a separator line
            txtReport.Text += "--------------------------------------------------------------------------------\r\n";
            // foreach row in the database table tbl_Products
            // Get each row from the table and add it to the multiline textbox
            foreach (DataRow row in config.dt.Rows)
            {
                txtReport.Text += row["prod_id"].ToString() + "\t" + row["prod_name"].ToString() + "\t" + row["prod_desc"].ToString() + "\t" + row["prod_type"].ToString() + "\t" + row["prod_price"].ToString() + "\t" + row["prod_quan"].ToString() + "\t" + row["prod_unit"].ToString() + "\r\n";
            }
        }
    }
}
