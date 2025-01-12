using DAL.Models;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace Doan
{
    public partial class Home : Form
    {
        private FootballDB _context;

        public event EventHandler DangXuat;

        public Home()
        {
            InitializeComponent();
            ConfigureForm();
        }

        private void ConfigureForm()
        {
            this.IsMdiContainer = true;
            this.Font = new Font("Arial", 12, FontStyle.Bold);
            this.ForeColor = Color.Blue;
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Home1());
        }

        private void OpenChildForm(Form childForm)
        {
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panel3.Controls.Clear();
            panel3.Controls.Add(childForm);
            childForm.Show();
        }



        private void btnStatistical_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Statistical());
        }

        private void BtnDeleteUser_Click(object sender, EventArgs e)
        {

        }

        private void Home_Load(object sender, EventArgs e)
        {

        }

        private void btnHome_Click_1(object sender, EventArgs e)
        {
            OpenChildForm(new Home1());
        }

        private void btnLogout_Click_1(object sender, EventArgs e)

        {
            Logout();
        }

        private void Logout()
        {
            var loginForm = new Login();
            loginForm.Show();
            this.Close();
        }

        private void BtnDeleteUser_Click_1(object sender, EventArgs e)
        {
            OpenChildForm(new frmXoa());
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmTimKiem());
        }
    }

}
