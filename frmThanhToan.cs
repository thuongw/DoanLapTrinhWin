using DAL.Models;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Doan
{
    public partial class frmThanhToan : Form
    {
        private readonly string tenKH;
        private readonly string tenSan;
        private readonly DateTime thoiGianBD;
        private readonly DateTime thoiGianKT;
        private readonly DataGridView dgvSan;
        private readonly int selectedRowIndex;
        private readonly Home1 homeForm;

        public frmThanhToan(string tenKH, string tenSan, DateTime thoiGianBD, DateTime thoiGianKT, DataGridView dgvSan, int selectedRowIndex, Home1 homeForm)
        {
            InitializeComponent();
            this.tenKH = tenKH;
            this.tenSan = tenSan;
            this.thoiGianBD = thoiGianBD;
            this.thoiGianKT = thoiGianKT;
            this.dgvSan = dgvSan;
            this.selectedRowIndex = selectedRowIndex;
            this.homeForm = homeForm;

            InitializeFormData();
        }

        private void InitializeFormData()
        {
            txtHoTen.Text = tenKH;
            cmbSan.Text = tenSan;
            txtGio.Text = $"{thoiGianBD:HH:mm} - {thoiGianKT:HH:mm}";
            LoadSoGioVaTongTien();
        }

        private void LoadSoGioVaTongTien()
        {
            using (var context = new FootballDB())
            {
                var san = context.SANBONG.FirstOrDefault(s => s.TENSAN == tenSan);
                if (san != null)
                {
                    decimal giaThue = san.GiaThue ?? 0;
                    double soGio = (thoiGianKT - thoiGianBD).TotalHours;
                    txtGio.Text = $"{soGio:0.00} giờ";
                    decimal tongTien = (decimal)soGio * giaThue;
                    txtTien.Text = $"{tongTien:N0} VND";
                }
                else
                {
                    MessageBox.Show("Không tìm thấy thông tin sân.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            if (selectedRowIndex >= 0)
            {
                ProcessPayment();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng để thanh toán.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ProcessPayment()
        {
            using (var context = new FootballDB())
            {
                int maKH = Convert.ToInt32(dgvSan.Rows[selectedRowIndex].Cells["MaKH"].Value);
                var khachHang = context.KHACHHANG.FirstOrDefault(kh => kh.MaKH == maKH);

                if (khachHang != null)
                {
                    RemoveReservationData(maKH, context);
                    context.KHACHHANG.Remove(khachHang);
                    context.SaveChanges();

                    MessageBox.Show("Thanh toán thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    homeForm.LoadData();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy khách hàng để thanh toán.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RemoveReservationData(int maKH, FootballDB context)
        {
            var phieuDatList = context.PHIEUDAT.Where(pd => pd.MaKH == maKH).ToList();
            foreach (var phieuDat in phieuDatList)
            {
                var chiTietList = context.CHITIETPHIEUDAT.Where(ct => ct.MaPhieu == phieuDat.MaPhieu).ToList();
                context.CHITIETPHIEUDAT.RemoveRange(chiTietList);
                context.PHIEUDAT.Remove(phieuDat);
            }
        }

        private void frmThanhToan_Load(object sender, EventArgs e)
        {
        }
    }
}
