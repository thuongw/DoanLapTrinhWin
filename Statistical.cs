using DAL.Models;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Doan
{
    public partial class Statistical : Form
    {
        FootballDB context = new FootballDB();

        public Statistical()
        {
            InitializeComponent();
        }

        private void rbDay_CheckedChanged(object sender, EventArgs e)
        {
            dtpNgay.Visible = rbDay.Checked;
            dtpThang.Visible = false;
            dtpNam.Visible = false;
        }

        private void rbMonth_CheckedChanged(object sender, EventArgs e)
        {
            dtpNgay.Visible = false;
            dtpThang.Visible = rbMonth.Checked;
            dtpNam.Visible = false;
        }

        private void rbYear_CheckedChanged(object sender, EventArgs e)
        {
            dtpNgay.Visible = false;
            dtpThang.Visible = false;
            dtpNam.Visible = rbYear.Checked;
        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            decimal doanhThu = 0;
            string label = "";

            if (rbDay.Checked)
            {
                DateTime selectedDate = dtpNgay.Value.Date;
                doanhThu = context.HOADON
                    .Where(h => h.Ngay.Value.Date == selectedDate)
                    .Sum(h => (decimal?)h.TongTien) ?? 0;
                label = selectedDate.ToShortDateString();
            }
            else if (rbMonth.Checked)
            {
                DateTime selectedMonth = dtpThang.Value;
                doanhThu = context.HOADON
                    .Where(h => h.Ngay.HasValue && h.Ngay.Value.Month == selectedMonth.Month && h.Ngay.Value.Year == selectedMonth.Year)
                    .Sum(h => (decimal?)h.TongTien) ?? 0;
                label = $"{selectedMonth:MM/yyyy}";
            }
            else if (rbYear.Checked)
            {
                int selectedYear = dtpNam.Value.Year;
                doanhThu = context.HOADON
                    .Where(h => h.Ngay.HasValue && h.Ngay.Value.Year == selectedYear)
                    .Sum(h => (decimal?)h.TongTien) ?? 0;
                label = selectedYear.ToString();
            }

            ShowChart(label, doanhThu);
        }


        private void ShowChart(string label, decimal doanhThu)
        {
            chartThongKe.Series.Clear();
            var series = chartThongKe.Series.Add("Doanh thu");
            series.Points.AddXY(label, doanhThu);
            chartThongKe.Titles.Clear();
            chartThongKe.Titles.Add($"Thống kê doanh thu: {label}");
        }
    }
}
