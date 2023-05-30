﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BTLWebNC
{
    public partial class QuanLyKetQuaThuUser : System.Web.UI.Page
    {
        DBSupport dBSupport = new DBSupport();
        protected void Page_Load(object sender, EventArgs e)
        {
            //id user khi xem kết quả thi của user 
            //id đề khi khi xem những người đã làm một bài thi

            int id = Convert.ToInt32(Request.QueryString["id"].ToString());

            //Danh sách người dùng
            DataTable dt = DataTableNguoiDung();

            string html = "";
            html += "<div class='name-user'>";
            foreach (DataRow row in dt.Rows)
            {
                if ((bool)Session["adminXemKetQuaThiUser"] == true && Convert.ToInt32(row["PK_MaNguoiDung"].ToString()) == id)
                {
                    html += "<h3>Kết quả thi của " + row["sHoTen"].ToString() + "</h3>";
                    break;
                }
                if ((bool)Session["adminXemNhungNguoiDaLamBaiThi"] == true)
                {
                    html += "";
                    break;
                }
                if ((bool)Session["adminXemKetQuaThiUser"] == false && (bool)Session["adminXemNhungNguoiDaLamBaiThi"] == false)
                {
                    html += "<h3>Kết quả thi của " + Session["hoTen"] + "</h3>";
                    break;
                }
            }

            


            html += "</div>";
            html += "<div class='table-result-user'>";
            html += "<table>";
            
            html += "<tr>";
            html += "<th>STT</th>";
            if ((bool)Session["adminXemNhungNguoiDaLamBaiThi"] == true)
            {
                html += "<th>Họ Tên</th>";
            }
            else
            {
                html += "<th>Tiêu Đề Bài Thi</th>";
            }
            html += "<th>Thời Gian Bắt Đầu</th>";
            html += "<th>Thời Gian Kết Thúc</th>";
            html += "<th>Thời Gian Làm Bài</th>";
            html += "<th>Điểm Số</th>";
            html += "<th>Chi Tiết</th>";
            html += "</tr>";
            int stt = 1;

            //Danh sách kết quả thi
            DataTable dtkqt = DataTableKetQuaThi();
            foreach (DataRow row in dtkqt.Rows)
            {
                if (Convert.ToInt32(row["FK_MaNguoiDung"].ToString()) == id && (bool)Session["adminXemKetQuaThiUser"] == true)
                {
                    html += "<tr>";
                    html += "<td>" + stt + "</td>";
                    html += "<td>" + row["sTieuDe"].ToString() + "</td>";
                    html += "<td>" + row["sThoiGianBatDau"].ToString() + "</td>";
                    html += "<td>" + row["sThoiGianKetThuc"].ToString() + "</td>";
                    html += "<td>" + row["sThoiGianLamBai"].ToString() + "</td>";
                    html += "<td>" + row["fDiemSo"].ToString() + "</td>";
                    html += "<td><button id='btnHuyKetQua' type='button' onclick='return toastHuy(" + row["PK_MaKetQuaThi"].ToString() + ")' name='btnHuyKq' value='" + row["PK_MaKetQuaThi"].ToString() + "'>Hủy</button></td></td>";
                    html += "</tr>";
                    stt++;
                }
                else if (Convert.ToInt32(row["FK_MaBaiThi"].ToString()) == id && (bool)Session["adminXemNhungNguoiDaLamBaiThi"] == true)
                {
                    html += "<tr>";
                    html += "<td>" + row["sHoTen"].ToString() + "</td>";
                    html += "<td>" + row["sThoiGianBatDau"].ToString() + "</td>";
                    html += "<td>" + row["sThoiGianKetThuc"].ToString() + "</td>";
                    html += "<td>" + row["sThoiGianLamBai"].ToString() + "</td>";
                    html += "<td>" + row["fDiemSo"].ToString() + "</td>";
                    // html += "<td><a href = '' >Xem </a></td>";
                    html += "<td><button id='btnHuyKetQua' type='button' onclick='return toastHuy(" + row["PK_MaKetQuaThi"].ToString() + ")' name='btnHuyKq' value='" + row["PK_MaKetQuaThi"].ToString() + "'>Hủy</button></td></td>";

                    html += "</tr>";
                }
                else if (Convert.ToInt32(row["FK_MaNguoiDung"].ToString()) == Convert.ToInt32(Session["id"].ToString()) && (bool)Session["adminXemNhungNguoiDaLamBaiThi"] == false)
                {
                    html += "<tr>";
                    html += "<td>" + row["sTieuDe"].ToString() + "</td>";
                    html += "<td>" + row["sThoiGianBatDau"].ToString() + "</td>";
                    html += "<td>" + row["sThoiGianKetThuc"].ToString() + "</td>";
                    html += "<td>" + row["sThoiGianLamBai"].ToString() + "</td>";
                    html += "<td>" + row["fDiemSo"].ToString() + "</td>";
                    //html += "<td><a href = '' >Xem </a></td>";
                    html += "<td><button id='btnHuyKetQua' type='button' onclick='return toastHuy(" + row["PK_MaKetQuaThi"].ToString() + ")' name='btnHuyKq' value='" + row["PK_MaKetQuaThi"].ToString() + "'>Hủy</button></td></td>";

                    html += "</tr>";
                }

            }

            html += "</table>";
            html += "</div>";

            resultUser.InnerHtml = html;

            Session["adminXemNhungNguoiDaLamBaiThi"] = false;
            Session["adminXemKetQuaThiUser"] = false;

            ///////////////////Huy Ket qua thi
            ///
            int maKqThi = 0;

            if (Request.QueryString["huyid"] != null)
            {
                maKqThi = Convert.ToInt32(Request.QueryString["huyid"].ToString());
                if (maKqThi != 0)
                {

                    SqlParameter[] sqlPar = new SqlParameter[1];
                    sqlPar[0] = new SqlParameter("@MaKetQuaThi", SqlDbType.Int);
                    sqlPar[0].Value = maKqThi;
                    dBSupport.getCmd_StoredProcedure(sqlPar, "SP_HuyBaiThi");
                    
                    
                        if (dtkqt.Rows.Count > 0)
                        {
                            try
                            {
                                string json = JsonConvert.SerializeObject(dtkqt);
                                Response.Clear();
                                Response.Write(json);
                                HttpContext.Current.Response.Flush();
                                HttpContext.Current.Response.SuppressContent = true;
                                HttpContext.Current.ApplicationInstance.CompleteRequest();
                            }
                            catch (Exception ex)
                            {
                                Response.Write(ex.Message);
                            }
                        }
                        else
                        {
                            string json = "no data";
                            Response.Clear();
                            Response.Write(json);
                            Response.End();
                        }
                    

                }
            }

            ///////
        }

        private DataTable DataTableKetQuaThi()
        {
            SqlParameter[] sqlPars = new SqlParameter[0];

            DataTable dt = dBSupport.getDataTable_StoredProcedure(sqlPars, "SP_DanhSachKetQuaThi");
            return dt;
        }

        private DataTable DataTableNguoiDung()
        {
            SqlParameter[] sqlPars = new SqlParameter[0];

            DataTable dt = dBSupport.getDataTable_StoredProcedure(sqlPars, "SP_DanhSachNguoiDung");
            return dt;
        }

    }
}