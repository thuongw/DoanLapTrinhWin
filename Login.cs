using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DAL.Models;
using DevExpress.XtraWaitForm;
using Shared.DTOs;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Doan
{
    public partial class Login : Form
    {
        FootballDB db = new FootballDB();
        public Login()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();

            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var userInfor = new UserloginDto
            {
                UserName = txtUser.Text,
                Password = txtPass.Text,
            };

            var login = new BUS.Login();

            var result = login.UserLogin(userInfor);

            if (result == true)
            {
                MessageBox.Show("Đăng nhập thành công!");
                Home mainForm = new Home();
                mainForm.Show();

                this.Hide(); 
            }
            else
            {                     
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.");
            } 
        }

        private void chkPass_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPass.Checked)
            {
                txtPass.PasswordChar = '\0';  
            }
            else
            {
                txtPass.PasswordChar = '*';  
            }
        }
    }
}
