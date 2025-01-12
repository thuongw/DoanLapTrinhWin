using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Doan
{
    public partial class fromaddSan : Form
    {
        private readonly FootballDB context;
        private readonly Home1 _homeForm;

        public fromaddSan(Home1 homeForm)
        {
            InitializeComponent();
            _homeForm = homeForm;
            context = new FootballDB();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime chonNgay = dtpNgay.Value.Date;
                DateTime thoiGianBD = GetSelectedStartDateTime();
                DateTime thoiGianKT = GetSelectedEndDateTime();

                if (thoiGianKT <= thoiGianBD)
                {
                    MessageBox.Show("Vui lòng chọn thời gian kết thúc lớn hơn thời gian bắt đầu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string hoTen = txtHoTen.Text.Trim();
                string sdt = txtSDT.Text.Trim();
                string soSan = cmbSan.SelectedItem.ToString();

                if (IsValidInput(hoTen, sdt, soSan))
                {
                    ProcessTransaction(hoTen, sdt, chonNgay, thoiGianBD, thoiGianKT);
                    MessageBox.Show("Thêm sân thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsValidInput(string hoTen, string sdt, string soSan)
        {
            if (string.IsNullOrEmpty(hoTen) || string.IsNullOrEmpty(sdt) || string.IsNullOrEmpty(soSan))
            {
                MessageBox.Show("Vui lòng ghi đầy đủ thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(sdt, @"^\d{9,10}$"))
            {
                MessageBox.Show("Số điện thoại không hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private DateTime GetSelectedStartDateTime()
        {
            DateTime chonNgay = dtpNgay.Value.Date;
            int chonGioBatDau = int.Parse(cmbGiobatDau.SelectedItem.ToString());
            int chonPhutBatDau = int.Parse(cmbPhutBatDau.SelectedItem.ToString());
            return new DateTime(chonNgay.Year, chonNgay.Month, chonNgay.Day, chonGioBatDau, chonPhutBatDau, 0);
        }

        private DateTime GetSelectedEndDateTime()
        {
            DateTime chonNgay = dtpNgay.Value.Date;
            int chonGioKetThuc = int.Parse(cmbGioKetTHuc.SelectedItem.ToString());
            int chonPhutKetThuc = int.Parse(cmbPhutKetThuc.SelectedItem.ToString());
            return new DateTime(chonNgay.Year, chonNgay.Month, chonNgay.Day, chonGioKetThuc, chonPhutKetThuc, 0);
        }

        private void ProcessTransaction(string hoTen, string sdt, DateTime chonNgay, DateTime thoiGianBD, DateTime thoiGianKT)
        {
            using (var db = new FootballDB())
            {
                var transaction = db.Database.BeginTransaction();

                var kHACHHANG = new KHACHHANG { TenKH = hoTen, SDT = sdt };
                db.KHACHHANG.Add(kHACHHANG);
                db.SaveChanges();

                int maKH = kHACHHANG.MaKH;

                var phieuDatSan = new PHIEUDAT
                {
                    NgayDat = chonNgay,
                    MaKH = maKH
                };
                db.PHIEUDAT.Add(phieuDatSan);
                db.SaveChanges();

                var cHITIETPHIEUDAT = new CHITIETPHIEUDAT
                {
                    ThoiGianBD = thoiGianBD,
                    ThoiGianKT = thoiGianKT,
                    MaPhieu = phieuDatSan.MaPhieu,
                    MaSan = Convert.ToInt32(cmbSan.SelectedValue)
                };
                db.CHITIETPHIEUDAT.Add(cHITIETPHIEUDAT);
                db.SaveChanges();

                transaction.Commit();
                _homeForm.LoadData();
                this.Close();
            }
        }

        private void ClearInputFields()
        {
            txtHoTen.Clear();
            txtSDT.Clear();
            cmbSan.SelectedIndex = -1;
            cmbGiobatDau.SelectedIndex = -1;
            cmbPhutBatDau.SelectedIndex = -1;
            cmbGioKetTHuc.SelectedIndex = -1;
            cmbPhutKetThuc.SelectedIndex = -1;
        }

        private void fromaddSan_Load(object sender, EventArgs e)
        {
            var listSan = context.SANBONG.ToList();
            FillSanCombobox(listSan);
            PopulateComboBoxes();
        }

        private void FillSanCombobox(List<SANBONG> listSan)
        {
            cmbSan.DataSource = listSan;
            cmbSan.ValueMember = "MaSan";
            cmbSan.DisplayMember = "TenSan";
        }

        private void PopulateComboBoxes()
        {
            PopulateTimeComboBox(cmbGiobatDau);
            PopulateTimeComboBox(cmbPhutBatDau);
            PopulateTimeComboBox(cmbGioKetTHuc);
            PopulateTimeComboBox(cmbPhutKetThuc);
        }

        private void PopulateTimeComboBox(ComboBox comboBox)
        {
            for (int i = 0; i < 60; i++)
            {
                comboBox.Items.Add(i.ToString("D2"));
            }
            comboBox.SelectedIndex = 0;
        }

        private void txtSDT_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Vui lòng chỉ nhập số", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
