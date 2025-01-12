using BUS;
using DAL.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Doan
{
    public partial class Home1 : Form
    {
        private FootballDB _context = new FootballDB();

        public Home1()
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var addSanForm = new fromaddSan(this);
            addSanForm.Show();
        }

        private void Home1_Load(object sender, EventArgs e)
        {
            LoadData();
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvSan.Rows[e.RowIndex];
                string tenKH = row.Cells["TenKH"].Value?.ToString();
                string sdt = row.Cells["SDT"].Value?.ToString();
                string tenSan = row.Cells["TENSAN"].Value?.ToString();
                DateTime ngayDat;

                if (DateTime.TryParse(row.Cells["NgayDat"].Value?.ToString(), out ngayDat))
                {
                    var ngayDatFormatted = ngayDat.ToString("dd/MM/yyyy");
                    DateTime thoiGianBD;
                    DateTime thoiGianKT;

                    if (DateTime.TryParse(row.Cells["ThoiGianBD"].Value?.ToString(), out thoiGianBD) &&
                        DateTime.TryParse(row.Cells["ThoiGianKT"].Value?.ToString(), out thoiGianKT))
                    {
                        TimeSpan timeDifference = thoiGianKT - thoiGianBD;
                        string soGioThue = $"{(int)timeDifference.TotalHours} giờ {timeDifference.Minutes} phút";

                        txtTen.Text = tenKH;
                        txtSDT.Text = sdt;
                        txtSan.Text = tenSan;
                        txtGio.Text = soGioThue;
                        txtNgayDat.Text = ngayDatFormatted;
                    }
                    else
                    {
                        MessageBox.Show("Dữ liệu thời gian không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Dữ liệu ngày đặt không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvSan.CurrentRow != null)
            {
                var row = dgvSan.CurrentRow;
                int maKH = Convert.ToInt32(row.Cells["MaKH"].Value);
                string tenKH = row.Cells["TenKH"].Value.ToString();
                string sdt = row.Cells["SDT"].Value.ToString();
                string tenSan = row.Cells["TENSAN"].Value.ToString();
                DateTime ngayDat = Convert.ToDateTime(row.Cells["NgayDat"].Value);
                DateTime thoiGianBD = Convert.ToDateTime(row.Cells["ThoiGianBD"].Value);
                DateTime thoiGianKT = Convert.ToDateTime(row.Cells["ThoiGianKT"].Value);

                var updateSanForm = new UpdateSan(this, maKH, tenKH, sdt, tenSan, ngayDat, thoiGianBD, thoiGianKT);
                updateSanForm.Show();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng để sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            if (dgvSan.CurrentRow != null)
            {
                var row = dgvSan.CurrentRow;
                string tenKH = row.Cells["TenKH"].Value.ToString();
                string tenSan = row.Cells["TENSAN"].Value.ToString();
                DateTime thoiGianBD = Convert.ToDateTime(row.Cells["ThoiGianBD"].Value);
                DateTime thoiGianKT = Convert.ToDateTime(row.Cells["ThoiGianKT"].Value);

                var thanhToanForm = new frmThanhToan(tenKH, tenSan, thoiGianBD, thoiGianKT, dgvSan, dgvSan.CurrentRow.Index, this);
                thanhToanForm.Show();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng để thanh toán.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
