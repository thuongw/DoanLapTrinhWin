using DAL.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Doan
{
    public partial class frmXoa : Form
    {
        private FootballDB _context = new FootballDB();

        public frmXoa()
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvSan.CurrentRow != null)
            {
                if (int.TryParse(dgvSan.CurrentRow.Cells[0].Value?.ToString(), out int maKH))
                {
                    var khachHang = _context.KHACHHANG.FirstOrDefault(kh => kh.MaKH == maKH);
                    if (khachHang != null)
                    {
                        DeleteReservation(maKH);
                        MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy khách hàng để xóa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Dữ liệu không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DeleteReservation(int maKH)
        {
            var phieuDatList = _context.PHIEUDAT.Where(pd => pd.MaKH == maKH).ToList();
            foreach (var phieuDat in phieuDatList)
            {
                var chiTietList = _context.CHITIETPHIEUDAT.Where(ct => ct.MaPhieu == phieuDat.MaPhieu).ToList();
                _context.CHITIETPHIEUDAT.RemoveRange(chiTietList);
                _context.PHIEUDAT.Remove(phieuDat);
            }

            _context.KHACHHANG.Remove(_context.KHACHHANG.First(kh => kh.MaKH == maKH));
            _context.SaveChanges();
        }

        private void dgvSan_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void frmXoa_Load(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
