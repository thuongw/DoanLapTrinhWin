using DAL.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Doan
{
    public partial class frmTimKiem : Form
    {
        private FootballDB _context = new FootballDB();

        public frmTimKiem()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.Font = new Font("Arial", 12, FontStyle.Bold);
            this.ForeColor = Color.Blue;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
        }

        public void LoadData()
        {
            var query = from KH in _context.KHACHHANG
                        join PD in _context.PHIEUDAT on KH.MaKH equals PD.MaKH
                        join CTPD in _context.CHITIETPHIEUDAT on PD.MaPhieu equals CTPD.MaPhieu
                        join SB in _context.SANBONG on CTPD.MaSan equals SB.MaSan
                        select new
                        {
                            KH.MaKH,
                            KH.TenKH,
                            KH.SDT,
                            PD.NgayDat,
                            CTPD.ThoiGianBD,
                            CTPD.ThoiGianKT,
                            SB.TENSAN
                        };

            dgvSan.DataSource = query.ToList();
            ConfigureGridColumns();
        }

        private void ConfigureGridColumns()
        {
            dgvSan.Columns["MaKH"].Visible = false;
            dgvSan.Columns["TenKH"].HeaderText = "Tên";
            dgvSan.Columns["SDT"].HeaderText = "SĐT";
            dgvSan.Columns["NgayDat"].HeaderText = "Ngày";
            dgvSan.Columns["ThoiGianBD"].HeaderText = "Giờ bắt đầu";
            dgvSan.Columns["ThoiGianKT"].HeaderText = "Giờ kết thúc";
            dgvSan.Columns["TENSAN"].HeaderText = "Sân";

            dgvSan.Columns["ThoiGianBD"].DefaultCellStyle.Format = "HH:mm";
            dgvSan.Columns["ThoiGianKT"].DefaultCellStyle.Format = "HH:mm";
        }

        private void txtFind_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtFind.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(keyword))
            {
                LoadData();
            }
            else
            {
                FilterData(keyword);
            }
        }

        private void FilterData(string keyword)
        {
            var query = from KH in _context.KHACHHANG
                        join PD in _context.PHIEUDAT on KH.MaKH equals PD.MaKH
                        join CTPD in _context.CHITIETPHIEUDAT on PD.MaPhieu equals CTPD.MaPhieu
                        join SB in _context.SANBONG on CTPD.MaSan equals SB.MaSan
                        where KH.TenKH.ToLower().Contains(keyword) ||
                              KH.SDT.Contains(keyword) ||
                              SB.TENSAN.ToLower().Contains(keyword)
                        select new
                        {
                            KH.MaKH,
                            KH.TenKH,
                            KH.SDT,
                            PD.NgayDat,
                            CTPD.ThoiGianBD,
                            CTPD.ThoiGianKT,
                            SB.TENSAN
                        };

            dgvSan.DataSource = query.ToList();
            ConfigureGridColumns();
        }

        private void frmTimKiem_Load(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
