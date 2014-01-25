using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using SAHIBINDEN.Library.Business;
using SAHIBINDEN.Library.Utility;

using Microsoft.Xrm.Sdk;
using System.Text;
namespace SAHIBINDEN.Web.CustomerPayments
{
    public partial class index : System.Web.UI.Page
    {
        SqlDataAccess sda;
        protected void Page_Load(object sender, EventArgs e)
        {
            string contactId = string.Empty;

            if (Request.QueryString["userid"] != null)
                contactId = Request.QueryString["userid"].ToString();
            else
            {
                tblContainer.InnerHtml = "<center><h2>Kullanıcı Crm Id parametresi eksik.<br>Kullancı bilgisini tanımlayıp tekrar deneyiniz.</h2></center>";
                return;
            }

            StringBuilder tableHtml = new StringBuilder();

            try
            {
                sda = new SqlDataAccess();
                sda.openConnection(Globals.ConnectionString);

                List<CrmPaymentInfo> paymentList = ContactHelper.GetContactPayments(new Guid(contactId), sda);

                if (paymentList.Count > 0)
                {
                    List<PaymentData> pdList = new List<PaymentData>();
                    for (int i = 0; i < paymentList.Count; i++)
                    {
                        if (paymentList[i].PaymentLineList.Count > 0)
                        {
                            for (int j = 0; j < paymentList[i].PaymentLineList.Count; j++)
                            {
                                PaymentData pd = new PaymentData();
                                pd.Id = paymentList[i].AdminPaymentId;
                                pd.Description = paymentList[i].PaymentLineList[j].Description;
                                pd.ConfirmDate = paymentList[i].ConfirmDate;
                                pd.StatusCode = paymentList[i].StatusCodeName;
                                pd.PaymentType = paymentList[i].PaymentTypeName;
                                pd.Total = paymentList[i].Total;

                                pdList.Add(pd);
                            }
                        }
                    }

                    var pList = (from a in pdList
                                 orderby a.ConfirmDate descending
                                 select a).ToList();

                    if (pList.Count > 0)
                    {
                        for (int i = 0; i < pList.Count; i++)
                        {
                            tableHtml.AppendLine("<tr>");
                            tableHtml.AppendLine("<td class='tertiary-text'>" + (pList[i].Id != null ? pList[i].Id : "-") + "</td>");                            
                            tableHtml.AppendLine("<td class='tertiary-text'>" + (pList[i].Description != null ? pList[i].Description : "-") + "</td>");
                            tableHtml.AppendLine("<td class='tertiary-text'>" + (pList[i].ConfirmDate != null ? pList[i].ConfirmDate.ToString("dd.MM.yyyy") : "-") + "</td>");
                            tableHtml.AppendLine("<td class='tertiary-text'>" + (pList[i].StatusCode != null ? pList[i].StatusCode : "-") + "</td>");
                            tableHtml.AppendLine("<td class='tertiary-text'>" + (pList[i].PaymentType != null ? pList[i].PaymentType : "-") + "</td>");
                            tableHtml.AppendLine("<td class='tertiary-text'>" + (pList[i].Total != 0 ? pList[i].Total.ToString() : "-") + "</td>");
                            tableHtml.AppendLine("</tr>");

                            tableHtml.AppendLine("<tr><td colspan='6'><table class='striped' style='margin-left:5%;'><tr class='warning'><td class='tertiary-text'>Deneme1</td><td class='tertiary-text'>Deneme2</td><td class='tertiary-text'>Deneme3</td></tr></table></td></tr>");
                        }
                    }

                }
                else
                {
                    tableHtml.AppendLine("<tr>");
                    tableHtml.AppendLine("<td colspan='6'><center><h3 style='color:red !important'>İlgili kullanıcıya ait herhangi bir ödeme kaydı yoktur.</h3></center></td>");
                    tableHtml.AppendLine("</tr>");
                }

                tblPaymentsBody.InnerHtml = tableHtml.ToString();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                sda.closeConnection();
            }

        }

        private class PaymentData
        {
            public string Id { get; set; }
            public string Description { get; set; }
            public DateTime ConfirmDate { get; set; }
            public string StatusCode { get; set; }
            public string PaymentType { get; set; }
            public decimal Total { get; set; }
        }
    }
}