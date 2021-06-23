using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Configuration;
using System.Data.SqlClient;
using System.IO.Compression;


using System.IO;

namespace WINSTLD
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BTN_CLEARLBLES.PerformClick();
            button3.PerformClick();
            btn_unzip.PerformClick();

            BTN_PMIX.PerformClick();
            button5.PerformClick();///Original STLD CHECK
            BTN_CLEARLBLES.PerformClick();

            //// check file by file//////////
            foreach (string file in Directory.EnumerateFiles("E:\\ImpotSheiV2\\Extract\\Processingfiles", "*.xml"))
            {
                string contents = File.ReadAllText(file);
                ///////////////////// File Reading/////////////////////////

                lbl_nameval.Text = Convert.ToString(file.ToString());
                if (lbl_nameval.Text.Contains("STLD"))
                {

                    string connString = SQLCON.ConnectionString2;
                    lbl_STLD.Text = file;

                    SQLCON vb = new SQLCON();
                    vb.OpenConnection();
                    DataSet ds = new DataSet();


                    try
                    {

                        ds.ReadXml(lbl_STLD.Text);

                        DataTable CHKTLD = ds.Tables["TLD"];
                        foreach (DataRow row in CHKTLD.Rows)
                        {
                            // ... Write value of first field as integer.
                            //string storloc = row.ItemArray[7].ToString(); CHnages of adc
                            string storloc = row.ItemArray[8].ToString();
                            string storbusinessdate = row.ItemArray[1].ToString();



                            lbl_location.Text = storloc;



                            lbl_business.Text = storbusinessdate;
                        }


                        using (SqlConnection con1 = new SqlConnection(connString))
                        {

                            con1.Open();

                            SqlCommand comm = new SqlCommand("SELECT COUNT(*) FROM [STLDPROCESS_STATUS] where [STLD_Location]=@STLD_Location and business_date=@business_date", con1);
                            comm.Parameters.AddWithValue("@STLD_Location", SqlDbType.VarChar).Value = lbl_location.Text;
                            comm.Parameters.AddWithValue("@business_date", SqlDbType.VarChar).Value = lbl_business.Text;

                            Int32 count = Convert.ToInt32(comm.ExecuteScalar());
                            if (count > 0)
                            {
                                lblCount.Text = Convert.ToString(count.ToString()); //For example a Label
                            }
                            else
                            {
                                if (lbl_location.Text.Contains("online"))
                                {
                                    lblCount.Text = "wrong";

                                }
                                else
                                {
                                    //lblCount.Text = "0";


                                    SqlCommand comm2 = new SqlCommand("SELECT [StoreID]   FROM [STLD].[dbo].[TB_STOREMASTER] where [StoreID]=@STLD_Location", con1);
                                    comm2.Parameters.AddWithValue("@STLD_Location", SqlDbType.VarChar).Value = lbl_location.Text;
                                    Int32 count2 = Convert.ToInt32(comm2.ExecuteScalar());

                                    if (count2 > 0)
                                    {
                                        lblCount.Text = "0"; //For example a Label
                                    }
                                    else
                                    {
                                        lblCount.Text = "wrong";
                                    }

                                }
                            }
                            con1.Close();

                        }


                        using (SqlConnection con = new SqlConnection(connString))
                        {
                            con.Open();

                            if (lblCount.Text.Contains('0'))
                            {

                                using (var connection = con)
                                {
                                    using (var tran = con.BeginTransaction())
                                    {


                                        try
                                        {




                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {

                                                //DataTable TLD = ds.Tables["TLD"];
                                                //foreach (DataRow row in TLD.Rows)
                                                //{
                                                //    // ... Write value of first field as integer.
                                                //    //string storloc = row.ItemArray[8].ToString();/// Change 20201016 shehan
                                                //    //string storbusinessdate = row.ItemArray[1].ToString();

                                                //    //lbl_location.Text = storloc;
                                                //    //lbl_business.Text = storbusinessdate;
                                                //}






                                                //////Shehan Under con



                                                DataTable TRX_Sale = ds.Tables["TRX_Sale"];

                                                TRX_Sale.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TRX_Sale.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TRX_Sale.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }



                                                bc.DestinationTableName = "TRX_Sale";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("TRX_Sale_Id", "TRX_Sale_Id");
                                                bc.ColumnMappings.Add("status", "status");
                                                bc.ColumnMappings.Add("POD", "POD");
                                                bc.ColumnMappings.Add("RemPOD", "RemPOD");
                                                bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                bc.WriteToServer(TRX_Sale);

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable Info = ds.Tables["Info"];
                                                if (Info != null)
                                                {
                                                    Info.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    Info.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in Info.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "Info";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("name", "name");
                                                    bc.ColumnMappings.Add("value", "value");
                                                    bc.ColumnMappings.Add("CustomInfo_Id", "CustomInfo_Id");
                                                    bc.WriteToServer(Info);

                                                }

                                            }



                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {

                                                //------- Order Table////////////////
                                                DataTable Order_TB = ds.Tables["Order"];
                                                Order_TB.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Order_TB.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Order_TB.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Order_TB";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                bc.ColumnMappings.Add("Timestamp", "Timestamp");
                                                bc.ColumnMappings.Add("uniqueId", "uniqueId");
                                                bc.ColumnMappings.Add("kind", "kind");
                                                bc.ColumnMappings.Add("key", "key");
                                                bc.ColumnMappings.Add("major", "major");
                                                bc.ColumnMappings.Add("minor", "minor");
                                                bc.ColumnMappings.Add("side", "side");
                                                bc.ColumnMappings.Add("receiptNumber", "receiptNumber");
                                                bc.ColumnMappings.Add("fpReceiptNumber", "fpReceiptNumber");
                                                //bc.ColumnMappings.Add("boot", "boot");
                                                bc.ColumnMappings.Add("saleType", "saleType");
                                                bc.ColumnMappings.Add("totalAmount", "totalAmount");
                                                bc.ColumnMappings.Add("nonProductAmount", "nonProductAmount");
                                                bc.ColumnMappings.Add("totalTax", "totalTax");
                                                bc.ColumnMappings.Add("nonProductTax", "nonProductTax");
                                                bc.ColumnMappings.Add("orderSrc", "orderSrc");
                                                bc.ColumnMappings.Add("startSaleDate", "startSaleDate");
                                                bc.ColumnMappings.Add("startSaleTime", "startSaleTime");
                                                bc.ColumnMappings.Add("endSaleDate", "endSaleDate");
                                                bc.ColumnMappings.Add("endSaleTime", "endSaleTime");
                                                bc.ColumnMappings.Add("TRX_Sale_Id", "TRX_Sale_Id");
                                                bc.WriteToServer(Order_TB);

                                                //----------------------------------



                                            }

                                            //------refund
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con))
                                            {
                                                // DataTable TRX_Refund = ds.Tables["TRX_Refund"];

                                                // TRX_Refund.Columns.Add(new DataColumn()
                                                // {
                                                //     ColumnName = "STLD_Location",
                                                //     DataType = typeof(string)
                                                // });

                                                // TRX_Refund.Columns.Add(new DataColumn()
                                                // {
                                                //     ColumnName = "business_date",
                                                //     DataType = typeof(string)
                                                // });
                                                // foreach (DataRow vrow in TRX_Refund.Rows)
                                                // {

                                                //     vrow["STLD_Location"] = lbl_location.Text;
                                                //     vrow["business_date"] = lbl_business.Text;

                                                // }



                                                // bc.DestinationTableName = "TRX_Refund";
                                                // bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                // bc.ColumnMappings.Add("business_date", "business_date");
                                                // bc.ColumnMappings.Add("TRX_Refund_Id", "TRX_Refund_Id");
                                                //bc.ColumnMappings.Add("status", "status");
                                                // bc.ColumnMappings.Add("POD", "POD");
                                                // bc.ColumnMappings.Add("RemPOD", "RemPOD");
                                                // bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                // bc.WriteToServer(TRX_Refund);

                                            }



                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {

                                                //------- Item Table////////////////
                                                DataTable Item_TB = ds.Tables["Item"];
                                                Item_TB.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Item_TB.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Item_TB.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }

                                                bc.DestinationTableName = "Item_TB";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("Item_Id", "Item_Id");
                                                bc.ColumnMappings.Add("code", "code");
                                                bc.ColumnMappings.Add("type", "type");
                                                bc.ColumnMappings.Add("action", "action");
                                                bc.ColumnMappings.Add("level", "level");
                                                bc.ColumnMappings.Add("id", "id");
                                                bc.ColumnMappings.Add("displayOrder", "displayOrder");
                                                bc.ColumnMappings.Add("qty", "qty");
                                                bc.ColumnMappings.Add("grillQty", "grillQty");
                                                bc.ColumnMappings.Add("grillModifier", "grillModifier");
                                                bc.ColumnMappings.Add("qtyPromo", "qtyPromo");
                                                bc.ColumnMappings.Add("chgAfterTotal", "chgAfterTotal");
                                                bc.ColumnMappings.Add("BPPrice", "BPPrice");
                                                bc.ColumnMappings.Add("BPTax", "BPTax");
                                                bc.ColumnMappings.Add("BDPrice", "BDPrice");
                                                bc.ColumnMappings.Add("BDTax", "BDTax");
                                                bc.ColumnMappings.Add("totalPrice", "totalPrice");
                                                bc.ColumnMappings.Add("totalTax", "totalTax");
                                                bc.ColumnMappings.Add("category", "category");
                                                bc.ColumnMappings.Add("familyGroup", "familyGroup");
                                                bc.ColumnMappings.Add("daypart", "daypart");
                                                bc.ColumnMappings.Add("description", "description");
                                                bc.ColumnMappings.Add("department", "department");
                                                bc.ColumnMappings.Add("departmentClass", "departmentClass");
                                                bc.ColumnMappings.Add("departmentSubClass", "departmentSubClass");
                                                bc.ColumnMappings.Add("unitPrice", "unitPrice");
                                                bc.ColumnMappings.Add("unitTax", "unitTax");
                                                bc.ColumnMappings.Add("solvedChoice", "solvedChoice");
                                                bc.ColumnMappings.Add("isUpcharge", "isUpcharge");
                                                bc.ColumnMappings.Add("Item_Id_0", "Item_Id_0");
                                                bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                bc.WriteToServer(Item_TB);


                                                ////////////////////////////////////

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {
                                                ///-------------- Promo table-----------------
                                                DataTable TRX_Overring = ds.Tables["TRX_Overring"];
                                                if (TRX_Overring != null)
                                                {
                                                    TRX_Overring.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });
                                                    TRX_Overring.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in TRX_Overring.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "TRX_Overring";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("TRX_Overring_Id", "TRX_Overring_Id");
                                                    bc.ColumnMappings.Add("POD", "POD");
                                                    bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                    bc.WriteToServer(TRX_Overring);
                                                }
                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {
                                                ///-------------- Promo table-----------------
                                                DataTable Product = ds.Tables["Product"];
                                                if (Product != null)
                                                {
                                                    Product.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });
                                                    Product.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in Product.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "Product";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("code", "code");
                                                    bc.ColumnMappings.Add("quantity", "quantity");
                                                    bc.ColumnMappings.Add("Components_Id", "Components_Id");
                                                    bc.WriteToServer(Product);
                                                }
                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {
                                                ///-------------- Promo table-----------------
                                                DataTable Components = ds.Tables["Components"];
                                                if (Components != null)
                                                {
                                                    Components.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });
                                                    Components.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in Components.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "Components";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("Components_Id", "Components_Id");
                                                    bc.ColumnMappings.Add("Ev_BreakValueMeal_Id", "Ev_BreakValueMeal_Id");
                                                    bc.WriteToServer(Components);
                                                }
                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {
                                                ///-------------- Promo table-----------------
                                                DataTable Ev_BreakValueMeal = ds.Tables["Ev_BreakValueMeal"];
                                                if (Ev_BreakValueMeal != null)
                                                {
                                                    Ev_BreakValueMeal.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });
                                                    Ev_BreakValueMeal.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in Ev_BreakValueMeal.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "Ev_BreakValueMeal";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("ProductCode", "ProductCode");
                                                    bc.ColumnMappings.Add("Quantity", "Quantity");
                                                    bc.ColumnMappings.Add("Ev_BreakValueMeal_Id", "Ev_BreakValueMeal_Id");
                                                    bc.WriteToServer(Ev_BreakValueMeal);
                                                }
                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {
                                                ///-------------- Promo table-----------------
                                                DataTable Promo = ds.Tables["Promo"];
                                                if (Promo != null)
                                                {
                                                    Promo.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    Promo.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in Promo.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }


                                                    bc.DestinationTableName = "Promo";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("id", "id");
                                                    bc.ColumnMappings.Add("name", "name");
                                                    bc.ColumnMappings.Add("qty", "qty");
                                                    bc.ColumnMappings.Add("Item_id", "Item_id");
                                                    bc.WriteToServer(Promo);
                                                }
                                                //------------------------------------------
                                            }

                                            using (SqlBulkCopy bc = new SqlBulkCopy(con))
                                            {
                                                ///////------------------------
                                                // DataTable PromotionApplied = ds.Tables["PromotionApplied"];
                                                //  if (PromotionApplied != null)
                                                //  {
                                                //     PromotionApplied.Columns.Add(new DataColumn()
                                                //    {
                                                //        ColumnName = "STLD_Location",
                                                //        DataType = typeof(string)
                                                //    });

                                                //    PromotionApplied.Columns.Add(new DataColumn()
                                                //    {
                                                //        ColumnName = "business_date",
                                                //        DataType = typeof(string)
                                                //    });
                                                //    foreach (DataRow vrow in PromotionApplied.Rows)
                                                //    {

                                                //        vrow["STLD_Location"] = lbl_location.Text;
                                                //        vrow["business_date"] = lbl_business.Text;

                                                //    }
                                                //    bc.DestinationTableName = "PromotionApplied";
                                                //    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                //    bc.ColumnMappings.Add("business_date", "business_date");
                                                //    bc.ColumnMappings.Add("promotionId", "promotionId");
                                                //    bc.ColumnMappings.Add("promotionCounter", "promotionCounter");
                                                //    bc.ColumnMappings.Add("eligible", "eligible");
                                                //    bc.ColumnMappings.Add("originalPrice", "originalPrice");
                                                //    bc.ColumnMappings.Add("discountAmount", "discountAmount");
                                                //    bc.ColumnMappings.Add("discountType", "discountType");
                                                //    bc.ColumnMappings.Add("originalItemPromoQty", "originalItemPromoQty");
                                                //    bc.ColumnMappings.Add("originalProductCode", "originalProductCode");
                                                //    bc.ColumnMappings.Add("offerId", "offerId");
                                                //    bc.ColumnMappings.Add("Item_Id", "Item_Id");
                                                //    bc.WriteToServer(PromotionApplied);
                                                //    ///////////////////////

                                                //}
                                            }

                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {
                                                ////////// Offer ////// Table
                                                DataTable Offers = ds.Tables["Offers"];

                                                if (Offers != null)
                                                {

                                                    Offers.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    Offers.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in Offers.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "Offers";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("offerId", "offerId");
                                                    bc.ColumnMappings.Add("beforeOfferPrice", "beforeOfferPrice");
                                                    bc.ColumnMappings.Add("discountAmount", "discountAmount");
                                                    bc.ColumnMappings.Add("discountType", "discountType");
                                                    bc.ColumnMappings.Add("Item_Id", "Item_Id");
                                                    bc.ColumnMappings.Add("customerId", "customerId");
                                                    bc.ColumnMappings.Add("offerName", "offerName");
                                                    bc.ColumnMappings.Add("override", "override");
                                                    bc.ColumnMappings.Add("applied", "applied");
                                                    bc.ColumnMappings.Add("promotionId", "promotionId");
                                                    bc.ColumnMappings.Add("offerBarcodeType", "offerBarcodeType");
                                                    bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                    bc.WriteToServer(Offers);
                                                }
                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {
                                                DataTable TaxChain = ds.Tables["TaxChain"];

                                                TaxChain.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TaxChain.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TaxChain.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TaxChain";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("id", "id");
                                                bc.ColumnMappings.Add("name", "name");
                                                bc.ColumnMappings.Add("rate", "rate");
                                                bc.ColumnMappings.Add("baseAmount", "baseAmount");
                                                bc.ColumnMappings.Add("amount", "amount");
                                                bc.ColumnMappings.Add("BDBaseAmount", "BDBaseAmount");
                                                bc.ColumnMappings.Add("BDAmount", "BDAmount");
                                                bc.ColumnMappings.Add("Item_Id", "Item_Id");
                                                // bc.ColumnMappings.Add("BPBaseAmount", "BPBaseAmount");
                                                //bc.ColumnMappings.Add("BPAmount", "BPAmount");
                                                bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                bc.WriteToServer(TaxChain);

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {

                                                DataTable Promotions = ds.Tables["Promotions"];
                                                if (Promotions != null)
                                                {
                                                    Promotions.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    Promotions.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in Promotions.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }

                                                    bc.DestinationTableName = "Promotions";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("Promotions_Id", "Promotions_Id");
                                                    bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                    bc.WriteToServer(Promotions);
                                                }
                                            }


                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {

                                                DataTable Promotion = ds.Tables["Promotion"];
                                                if (Promotion != null)
                                                {


                                                    Promotion.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    Promotion.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in Promotion.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }

                                                    bc.DestinationTableName = "Promotion";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("promotionId", "promotionId");
                                                    bc.ColumnMappings.Add("promotionName", "promotionName");
                                                    bc.ColumnMappings.Add("promotionCounter", "promotionCounter");
                                                    bc.ColumnMappings.Add("discountType", "discountType");
                                                    bc.ColumnMappings.Add("discountAmount", "discountAmount");
                                                    bc.ColumnMappings.Add("offerId", "offerId");
                                                    bc.ColumnMappings.Add("exclusive", "exclusive");
                                                    bc.ColumnMappings.Add("promotionOnTender", "promotionOnTender");
                                                    bc.ColumnMappings.Add("countTowardsPromotionLimit", "countTowardsPromotionLimit");
                                                    bc.ColumnMappings.Add("returnedValue", "returnedValue");
                                                    bc.ColumnMappings.Add("Promotions_Id", "Promotions_Id");
                                                    bc.WriteToServer(Promotion);

                                                }
                                            }


                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {

                                                DataTable Customer = ds.Tables["Customer"];
                                                if (Customer != null)
                                                {
                                                    Customer.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    Customer.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in Customer.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }


                                                    bc.DestinationTableName = "Customer";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("id", "id");
                                                    bc.ColumnMappings.Add("nickname", "nickname");
                                                    bc.ColumnMappings.Add("greeting", "greeting");
                                                    bc.ColumnMappings.Add("loyaltyCardId", "loyaltyCardId");
                                                    bc.ColumnMappings.Add("loyaltyCardType", "loyaltyCardType");
                                                    bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                    bc.WriteToServer(Customer);
                                                }
                                            }


                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {

                                                DataTable CustomInfo = ds.Tables["CustomInfo"];
                                                if (CustomInfo != null)
                                                {
                                                    CustomInfo.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    CustomInfo.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in CustomInfo.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "CustomInfo";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("CustomInfo_Id", "CustomInfo_Id");
                                                    bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                    bc.WriteToServer(CustomInfo);


                                                }
                                            }


                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {

                                                DataTable Tenders = ds.Tables["Tenders"];
                                                Tenders.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Tenders.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Tenders.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Tenders";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("Tenders_Id", "Tenders_Id");
                                                bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                bc.WriteToServer(Tenders);
                                            }

                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {

                                                DataTable Tender = ds.Tables["Tender"];
                                                Tender.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Tender.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Tender.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Tender";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("TenderId", "TenderId");
                                                bc.ColumnMappings.Add("TenderKind", "TenderKind");
                                                bc.ColumnMappings.Add("TenderName", "TenderName");
                                                bc.ColumnMappings.Add("TenderQuantity", "TenderQuantity");
                                                bc.ColumnMappings.Add("FaceValue", "FaceValue");
                                                bc.ColumnMappings.Add("TenderAmount", "TenderAmount");
                                                bc.ColumnMappings.Add("BaseAction", "BaseAction");
                                                bc.ColumnMappings.Add("Persisted", "Persisted");
                                                bc.ColumnMappings.Add("CardProviderID", "CardProviderID");
                                                bc.ColumnMappings.Add("CashlessData", "CashlessData");
                                                bc.ColumnMappings.Add("TaxOption", "TaxOption");
                                                bc.ColumnMappings.Add("SubtotalOption", "SubtotalOption");
                                                bc.ColumnMappings.Add("ForeignCurrencyIndicator", "ForeignCurrencyIndicator");
                                                bc.ColumnMappings.Add("DiscountDescription", "DiscountDescription");
                                                bc.ColumnMappings.Add("CashlessTransactionID", "CashlessTransactionID");
                                                bc.ColumnMappings.Add("PaymentChannel", "PaymentChannel");
                                                bc.ColumnMappings.Add("Tenders_Id", "Tenders_Id");
                                                bc.WriteToServer(Tender);


                                            }

                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable POSTiming = ds.Tables["POSTiming"];
                                                if (POSTiming != null)
                                                {
                                                    POSTiming.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    POSTiming.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in POSTiming.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }

                                                }


                                            }

                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable EventsDetail = ds.Tables["EventsDetail"];
                                                if (EventsDetail != null)
                                                {
                                                    EventsDetail.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    EventsDetail.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in EventsDetail.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "EventsDetail";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("EventsDetail_Id", "EventsDetail_Id");
                                                    bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                    bc.WriteToServer(EventsDetail);

                                                }


                                            }

                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable SaleEvent = ds.Tables["SaleEvent"];
                                                if (SaleEvent != null)
                                                {
                                                    SaleEvent.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    SaleEvent.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in SaleEvent.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    //bc.DestinationTableName = "SaleEvent";
                                                    //bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    //bc.ColumnMappings.Add("business_date", "business_date");
                                                    //bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                    //bc.ColumnMappings.Add("Ev_SaleStored", "Ev_SaleStored");
                                                    //bc.ColumnMappings.Add("Ev_SaleRecalled", "Ev_SaleRecalled");
                                                    //bc.ColumnMappings.Add("Ev_BackFromTotal", "Ev_BackFromTotal");
                                                    //bc.ColumnMappings.Add("Ev_SaleTotal", "Ev_SaleTotal");
                                                    //bc.ColumnMappings.Add("Type", "Type");
                                                    //bc.ColumnMappings.Add("Time", "Time");
                                                    //bc.ColumnMappings.Add("EventsDetail_Id", "EventsDetail_Id");
                                                    //bc.WriteToServer(SaleEvent);

                                                }

                                            }

                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable Ev_SaleIncrementItemQty = ds.Tables["Ev_SaleIncrementItemQty"];
                                                if (Ev_SaleIncrementItemQty != null)
                                                {
                                                    Ev_SaleIncrementItemQty.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    Ev_SaleIncrementItemQty.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in Ev_SaleIncrementItemQty.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "Ev_SaleIncrementItemQty";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("SaleIndex", "SaleIndex");
                                                    bc.ColumnMappings.Add("Quantity", "Quantity");
                                                    bc.ColumnMappings.Add("ProductCode", "ProductCode");
                                                    bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                    bc.WriteToServer(Ev_SaleIncrementItemQty);

                                                }

                                            }

                                            using (SqlBulkCopy bc = new SqlBulkCopy(con))
                                            {


                                                DataTable TRX_GetAuthorization = ds.Tables["TRX_GetAuthorization"];
                                                if (TRX_GetAuthorization != null)
                                                {
                                                    TRX_GetAuthorization.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    TRX_GetAuthorization.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in TRX_GetAuthorization.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    //bc.DestinationTableName = "TRX_GetAuthorization";
                                                    //bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    //bc.ColumnMappings.Add("business_date", "business_date");
                                                    //bc.ColumnMappings.Add("Action", "Action");
                                                    //bc.ColumnMappings.Add("ManagerID", "ManagerID");
                                                    //bc.ColumnMappings.Add("ManagerName", "ManagerName");
                                                    //bc.ColumnMappings.Add("SecurityLevel", "SecurityLevel");
                                                    //bc.ColumnMappings.Add("ExpirationDate", "ExpirationDate");
                                                    //bc.ColumnMappings.Add("Password", "Password");
                                                    //bc.ColumnMappings.Add("Islogged", "Islogged");
                                                    //bc.ColumnMappings.Add("Method", "Method");
                                                    ////bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                    //bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                    //bc.WriteToServer(TRX_GetAuthorization);

                                                }

                                            }

                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable EV_NotChargedPromotional = ds.Tables["EV_NotChargedPromotional"];
                                                if (EV_NotChargedPromotional != null)
                                                {
                                                    EV_NotChargedPromotional.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    EV_NotChargedPromotional.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in EV_NotChargedPromotional.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "EV_NotChargedPromotional";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("ProductCode", "ProductCode");
                                                    bc.ColumnMappings.Add("Quantity", "Quantity");
                                                    bc.ColumnMappings.Add("NotChargedValue", "NotChargedValue");
                                                    bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                    bc.WriteToServer(EV_NotChargedPromotional);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable Ev_AddTender = ds.Tables["Ev_AddTender"];
                                                if (Ev_AddTender != null)
                                                {
                                                    Ev_AddTender.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    Ev_AddTender.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in Ev_AddTender.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "Ev_AddTender";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("TenderId", "TenderId");
                                                    bc.ColumnMappings.Add("FaceValue", "FaceValue");
                                                    bc.ColumnMappings.Add("TenderAmount", "TenderAmount");
                                                    bc.ColumnMappings.Add("BaseAction", "BaseAction");
                                                    bc.ColumnMappings.Add("Persisted", "Persisted");
                                                    bc.ColumnMappings.Add("CardProviderID", "CardProviderID");
                                                    bc.ColumnMappings.Add("CashlessData", "CashlessData");
                                                    bc.ColumnMappings.Add("CashlessTransactionID", "CashlessTransactionID");
                                                    bc.ColumnMappings.Add("PreAuthorization", "PreAuthorization");
                                                    bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                    bc.WriteToServer(Ev_AddTender);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable Ev_SetSaleType = ds.Tables["Ev_SetSaleType"];
                                                if (Ev_SetSaleType != null)
                                                {
                                                    Ev_SetSaleType.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    Ev_SetSaleType.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in Ev_SetSaleType.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "Ev_SetSaleType";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("Type", "Type");
                                                    bc.ColumnMappings.Add("ForceExhibition", "ForceExhibition");
                                                    bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                    bc.WriteToServer(Ev_SetSaleType);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable Ev_SaleChoice = ds.Tables["Ev_SaleChoice"];
                                                if (Ev_SaleChoice != null)
                                                {
                                                    Ev_SaleChoice.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    Ev_SaleChoice.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in Ev_SaleChoice.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "Ev_SaleChoice";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("ProductCode", "ProductCode");
                                                    bc.ColumnMappings.Add("ChoiceCode", "ChoiceCode");
                                                    bc.ColumnMappings.Add("Quantity", "Quantity");
                                                    bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                    bc.WriteToServer(Ev_SaleChoice);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable Ev_SaleItem = ds.Tables["Ev_SaleItem"];
                                                if (Ev_SaleItem != null)
                                                {
                                                    Ev_SaleItem.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    Ev_SaleItem.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in Ev_SaleItem.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "Ev_SaleItem";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("ProductCode", "ProductCode");
                                                    bc.ColumnMappings.Add("Quantity", "Quantity");
                                                    bc.ColumnMappings.Add("UpdatedQuantity", "UpdatedQuantity");
                                                    bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                    bc.WriteToServer(Ev_SaleItem);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable Ev_SaleCutomInfo = ds.Tables["Ev_SaleCutomInfo"];
                                                if (Ev_SaleCutomInfo != null)
                                                {
                                                    Ev_SaleCutomInfo.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    Ev_SaleCutomInfo.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in Ev_SaleCutomInfo.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    //bc.DestinationTableName = "Ev_SaleCutomInfo";
                                                    //bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    //bc.ColumnMappings.Add("business_date", "business_date");
                                                    //bc.ColumnMappings.Add("ProductCode", "ProductCode");
                                                    //bc.ColumnMappings.Add("Quantity", "Quantity");
                                                    //bc.ColumnMappings.Add("UpdatedQuantity", "UpdatedQuantity");
                                                    //bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                    //bc.WriteToServer(Ev_SaleCutomInfo);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable Ev_SaleEnd = ds.Tables["Ev_SaleEnd"];
                                                if (Ev_SaleEnd != null)
                                                {
                                                    Ev_SaleEnd.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    Ev_SaleEnd.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in Ev_SaleEnd.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "Ev_SaleEnd";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("Type", "Type");
                                                    bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                    bc.WriteToServer(Ev_SaleEnd);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable Ev_SaleStart = ds.Tables["Ev_SaleStart"];
                                                if (Ev_SaleStart != null)
                                                {
                                                    Ev_SaleStart.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    Ev_SaleStart.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in Ev_SaleStart.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "Ev_SaleStart";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("DisabledChoices", "DisabledChoices");
                                                    bc.ColumnMappings.Add("TenderPersisted", "TenderPersisted");
                                                    bc.ColumnMappings.Add("Multiorder", "Multiorder");
                                                    bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                    bc.WriteToServer(Ev_SaleStart);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable Fiscal_Information = ds.Tables["Fiscal_Information"];
                                                if (Fiscal_Information != null)
                                                {
                                                    Fiscal_Information.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    Fiscal_Information.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in Fiscal_Information.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "Fiscal_Information";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("TIN", "TIN");
                                                    bc.ColumnMappings.Add("name", "name");
                                                    bc.ColumnMappings.Add("address", "address");
                                                    bc.ColumnMappings.Add("ZIP", "ZIP");
                                                    bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                    bc.WriteToServer(Fiscal_Information);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable Ev_DrawerClose = ds.Tables["Ev_DrawerClose"];
                                                if (Ev_DrawerClose != null)
                                                {
                                                    Ev_DrawerClose.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    Ev_DrawerClose.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in Ev_DrawerClose.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "Ev_DrawerClose";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("TotalOpenTime", "TotalOpenTime");
                                                    bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                    bc.WriteToServer(Ev_DrawerClose);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable TRX_OperLogin = ds.Tables["TRX_OperLogin"];
                                                if (TRX_OperLogin != null)
                                                {
                                                    TRX_OperLogin.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    TRX_OperLogin.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in TRX_OperLogin.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "TRX_OperLogin";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("CrewID", "CrewID");
                                                    bc.ColumnMappings.Add("CrewName", "CrewName");
                                                    bc.ColumnMappings.Add("CrewSecurityLevel", "CrewSecurityLevel");
                                                    bc.ColumnMappings.Add("POD", "POD");
                                                    bc.ColumnMappings.Add("RemotePOD", "RemotePOD");
                                                    bc.ColumnMappings.Add("AutoLogin", "AutoLogin");
                                                    bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                    bc.WriteToServer(TRX_OperLogin);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable TRX_SetPOD = ds.Tables["TRX_SetPOD"];
                                                if (TRX_SetPOD != null)
                                                {
                                                    TRX_SetPOD.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    TRX_SetPOD.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in TRX_SetPOD.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "TRX_SetPOD";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("PODId", "PODId");
                                                    bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                    bc.WriteToServer(TRX_SetPOD);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable TRX_DayOpen = ds.Tables["TRX_DayOpen"];
                                                if (TRX_DayOpen != null)
                                                {
                                                    TRX_DayOpen.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    TRX_DayOpen.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in TRX_DayOpen.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "TRX_DayOpen";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("BusinessDate", "BusinessDate");
                                                    bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                    bc.WriteToServer(TRX_DayOpen);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable TRX_TaxTable = ds.Tables["TRX_TaxTable"];
                                                if (TRX_TaxTable != null)
                                                {
                                                    TRX_TaxTable.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    TRX_TaxTable.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in TRX_TaxTable.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "TRX_TaxTable";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("TRX_TaxTable_Id", "TRX_TaxTable_Id");
                                                    bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                    bc.WriteToServer(TRX_TaxTable);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable TaxType = ds.Tables["TaxType"];
                                                if (TaxType != null)
                                                {
                                                    TaxType.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    TaxType.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in TaxType.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "TaxType";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("TaxId", "TaxId");
                                                    bc.ColumnMappings.Add("TaxDescription", "TaxDescription");
                                                    bc.ColumnMappings.Add("TaxRate", "TaxRate");
                                                    bc.ColumnMappings.Add("TaxBasis", "TaxBasis");
                                                    bc.ColumnMappings.Add("TaxCalcType", "TaxCalcType");
                                                    bc.ColumnMappings.Add("Rounding", "Rounding");
                                                    bc.ColumnMappings.Add("Precision", "Precision");
                                                    bc.ColumnMappings.Add("TRX_TaxTable_Id", "TRX_TaxTable_Id");
                                                    bc.WriteToServer(TaxType);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable TRX_TenderTable = ds.Tables["TRX_TenderTable"];
                                                if (TRX_TenderTable != null)
                                                {
                                                    TRX_TenderTable.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    TRX_TenderTable.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in TRX_TenderTable.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "TRX_TenderTable";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("TRX_TenderTable_Id", "TRX_TenderTable_Id");
                                                    bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                    bc.WriteToServer(TRX_TenderTable);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable TenderType = ds.Tables["TenderType"];
                                                if (TenderType != null)
                                                {
                                                    TenderType.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    TenderType.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in TenderType.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "TenderType";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("TenderId", "TenderId");
                                                    bc.ColumnMappings.Add("TenderFiscalIndex", "TenderFiscalIndex");
                                                    bc.ColumnMappings.Add("TenderName", "TenderName");
                                                    bc.ColumnMappings.Add("TenderCategory", "TenderCategory");
                                                    bc.ColumnMappings.Add("Taxoption", "Taxoption");
                                                    bc.ColumnMappings.Add("DefaultSkimLimit", "DefaultSkimLimit");
                                                    bc.ColumnMappings.Add("DefaultHaloLimit", "DefaultHaloLimit");
                                                    bc.ColumnMappings.Add("SubtotalOption", "SubtotalOption");
                                                    bc.ColumnMappings.Add("CurrencyDecimals", "CurrencyDecimals");
                                                    bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                    bc.ColumnMappings.Add("TRX_TenderTable_Id", "TRX_TenderTable_Id");
                                                    bc.WriteToServer(TenderType);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable TenderFlags = ds.Tables["TenderFlags"];
                                                if (TenderFlags != null)
                                                {
                                                    TenderFlags.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    TenderFlags.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in TenderFlags.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "TenderFlags";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                    bc.ColumnMappings.Add("TenderFlags_Id", "TenderFlags_Id");
                                                    bc.WriteToServer(TenderFlags);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con))
                                            {


                                                DataTable TenderFlag = ds.Tables["TenderFlag"];
                                                if (TenderFlag != null)
                                                {
                                                    // TenderFlag.Columns.Add(new DataColumn()
                                                    //  {
                                                    //      ColumnName = "STLD_Location",
                                                    //      DataType = typeof(string)
                                                    //  });

                                                    //  TenderFlag.Columns.Add(new DataColumn()
                                                    //  {
                                                    //      ColumnName = "business_date",
                                                    //      DataType = typeof(string)
                                                    //  });
                                                    //foreach (DataRow vrow in TenderFlag.Rows)
                                                    // {

                                                    //     vrow["STLD_Location"] = lbl_location.Text;
                                                    //     vrow["business_date"] = lbl_business.Text;

                                                    // }
                                                    //  bc.DestinationTableName = "TenderFlag";
                                                    //  bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    //  bc.ColumnMappings.Add("business_date", "business_date");
                                                    //   bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                    //   bc.ColumnMappings.Add("TenderFlags_Id", "TenderFlags_Id");
                                                    //   bc.WriteToServer(TenderFlag);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable TenderChange = ds.Tables["TenderChange"];
                                                if (TenderChange != null)
                                                {
                                                    TenderChange.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    TenderChange.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in TenderChange.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "TenderChange";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("id", "id");
                                                    bc.ColumnMappings.Add("type", "type");
                                                    bc.ColumnMappings.Add("roundToMinAmount", "roundToMinAmount");
                                                    bc.ColumnMappings.Add("maxAllowed", "maxAllowed");
                                                    bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                    bc.WriteToServer(TenderChange);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable GiftCoupon = ds.Tables["GiftCoupon"];
                                                if (GiftCoupon != null)
                                                {
                                                    GiftCoupon.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    GiftCoupon.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in GiftCoupon.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "GiftCoupon";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("LegacyId", "LegacyId");
                                                    bc.ColumnMappings.Add("OperatorDefined", "OperatorDefined");
                                                    bc.ColumnMappings.Add("Amount", "Amount");
                                                    bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                    bc.WriteToServer(GiftCoupon);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable OtherPayments = ds.Tables["OtherPayments"];
                                                if (OtherPayments != null)
                                                {
                                                    //OtherPayments.Columns.Add(new DataColumn()
                                                    //{
                                                    //    ColumnName = "STLD_Location",
                                                    //    DataType = typeof(string)
                                                    //});

                                                    //OtherPayments.Columns.Add(new DataColumn()
                                                    //{
                                                    //    ColumnName = "business_date",
                                                    //    DataType = typeof(string)
                                                    //});
                                                    //foreach (DataRow vrow in OtherPayments.Rows)
                                                    //{

                                                    //    vrow["STLD_Location"] = lbl_location.Text;
                                                    //    vrow["business_date"] = lbl_business.Text;

                                                    //}
                                                    //bc.DestinationTableName = "OtherPayments";
                                                    //bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    //bc.ColumnMappings.Add("business_date", "business_date");
                                                    //bc.ColumnMappings.Add("LegacyId", "LegacyId");
                                                    //bc.ColumnMappings.Add("OperatorDefined", "OperatorDefined");
                                                    //bc.ColumnMappings.Add("Amount", "Amount");
                                                    //bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                    //bc.WriteToServer(OtherPayments);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable ForeignCurrency = ds.Tables["ForeignCurrency"];
                                                if (ForeignCurrency != null)
                                                {
                                                    ForeignCurrency.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    ForeignCurrency.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in ForeignCurrency.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "ForeignCurrency";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("LegacyId", "LegacyId");
                                                    bc.ColumnMappings.Add("ExchangeRate", "ExchangeRate");
                                                    bc.ColumnMappings.Add("Precision", "Precision");
                                                    bc.ColumnMappings.Add("Rounding", "Rounding");
                                                    bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                    bc.WriteToServer(ForeignCurrency);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable ElectronicPayment = ds.Tables["ElectronicPayment"];
                                                if (ElectronicPayment != null)
                                                {
                                                    ElectronicPayment.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    ElectronicPayment.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in ElectronicPayment.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "ElectronicPayment";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("LegacyId", "LegacyId");
                                                    bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                    bc.WriteToServer(ElectronicPayment);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable CreditSales = ds.Tables["CreditSales"];
                                                if (CreditSales != null)
                                                {
                                                    CreditSales.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    CreditSales.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in CreditSales.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "CreditSales";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("LegacyId", "LegacyId");
                                                    bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                    bc.WriteToServer(CreditSales);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable Trx_DayPart = ds.Tables["Trx_DayPart"];
                                                if (Trx_DayPart != null)
                                                {
                                                    Trx_DayPart.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    Trx_DayPart.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in Trx_DayPart.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    //bc.DestinationTableName = "Trx_DayPart";
                                                    //bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    //bc.ColumnMappings.Add("business_date", "business_date");
                                                    //bc.ColumnMappings.Add("LegacyId", "LegacyId");
                                                    //bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                    //bc.WriteToServer(Trx_DayPart);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable TRX_BaseConfig = ds.Tables["TRX_BaseConfig"];
                                                if (TRX_BaseConfig != null)
                                                {
                                                    TRX_BaseConfig.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    TRX_BaseConfig.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in TRX_BaseConfig.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "TRX_BaseConfig";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("TRX_BaseConfig_Id", "TRX_BaseConfig_Id");
                                                    bc.ColumnMappings.Add("POS", "POS");
                                                    bc.ColumnMappings.Add("POD", "POD");
                                                    bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                    bc.WriteToServer(TRX_BaseConfig);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable Config = ds.Tables["Config"];
                                                if (Config != null)
                                                {
                                                    Config.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    Config.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in Config.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "Config";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("MenuPriceBasis", "MenuPriceBasis");
                                                    bc.ColumnMappings.Add("WeekEndBreakfastStartTime", "WeekEndBreakfastStartTime");
                                                    bc.ColumnMappings.Add("WeekEndBreakfastStopTime", "WeekEndBreakfastStopTime");
                                                    bc.ColumnMappings.Add("WeekDayBreakfastStartTime", "WeekDayBreakfastStartTime");
                                                    bc.ColumnMappings.Add("WeekDayBreakfastStopTime", "WeekDayBreakfastStopTime");
                                                    bc.ColumnMappings.Add("DecimalPlaces", "DecimalPlaces");
                                                    bc.ColumnMappings.Add("CheckRefund", "CheckRefund");
                                                    bc.ColumnMappings.Add("GrandTotalFlag", "GrandTotalFlag");
                                                    bc.ColumnMappings.Add("StoreId", "StoreId");
                                                    bc.ColumnMappings.Add("StoreName", "StoreName");
                                                    bc.ColumnMappings.Add("AcceptNegativeQty", "AcceptNegativeQty");
                                                    bc.ColumnMappings.Add("AcceptZeroPricePMix", "AcceptZeroPricePMix");
                                                    bc.ColumnMappings.Add("FloatPriceTenderId", "FloatPriceTenderId");
                                                    bc.ColumnMappings.Add("MinCirculatingAmount", "MinCirculatingAmount");
                                                    bc.ColumnMappings.Add("TRX_BaseConfig_Id", "TRX_BaseConfig_Id");
                                                    bc.WriteToServer(Config);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable POSConfig = ds.Tables["POSConfig"];
                                                if (POSConfig != null)
                                                {
                                                    POSConfig.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    POSConfig.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in POSConfig.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "POSConfig";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("CountTCsFullDiscEM", "CountTCsFullDiscEM");
                                                    bc.ColumnMappings.Add("RefundBehaviour", "RefundBehaviour");
                                                    bc.ColumnMappings.Add("OverringBehaviour", "OverringBehaviour");
                                                    bc.ColumnMappings.Add("TRX_BaseConfig_Id", "TRX_BaseConfig_Id");
                                                    bc.WriteToServer(POSConfig);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable TRX_SetSMState = ds.Tables["TRX_SetSMState"];
                                                if (TRX_SetSMState != null)
                                                {
                                                    TRX_SetSMState.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    TRX_SetSMState.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in TRX_SetSMState.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "TRX_SetSMState";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("POSState", "POSState");
                                                    bc.ColumnMappings.Add("CrewId", "CrewId");
                                                    bc.ColumnMappings.Add("CrewName", "CrewName");
                                                    bc.ColumnMappings.Add("CrewSecurityLevel", "CrewSecurityLevel");
                                                    bc.ColumnMappings.Add("LoginTime", "LoginTime");
                                                    bc.ColumnMappings.Add("LogoutTime", "LogoutTime");
                                                    bc.ColumnMappings.Add("InitialFloat", "InitialFloat");
                                                    bc.ColumnMappings.Add("PODId", "PODId");
                                                    bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                    bc.WriteToServer(TRX_SetSMState);

                                                }

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {


                                                DataTable TRX_InitGTotal = ds.Tables["TRX_InitGTotal"];
                                                if (TRX_InitGTotal != null)
                                                {
                                                    TRX_InitGTotal.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    TRX_InitGTotal.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in TRX_InitGTotal.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "TRX_InitGTotal";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("amount", "amount");
                                                    bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                    bc.WriteToServer(TRX_InitGTotal);

                                                }

                                            }



                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {
                                                DataTable TLD = ds.Tables["TLD"];
                                                bc.DestinationTableName = "TLD";
                                                bc.ColumnMappings.Add("TLD_Id", "TLD_Id");
                                                bc.ColumnMappings.Add("LogVersion", "LogVersion");
                                                bc.ColumnMappings.Add("storeId", "storeId");
                                                bc.ColumnMappings.Add("businessDate", "businessDate");
                                                bc.ColumnMappings.Add("swVersion", "swVersion");
                                                bc.ColumnMappings.Add("checkPoint", "checkPoint");
                                                bc.ColumnMappings.Add("end", "end");
                                                bc.ColumnMappings.Add("productionStatus", "productionStatus");
                                                bc.ColumnMappings.Add("hasMoreContent", "hasMoreContent");
                                                bc.WriteToServer(TLD);

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {
                                                DataTable Node = ds.Tables["Node"];
                                                Node.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Node.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Node.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Node";
                                                bc.ColumnMappings.Add("Node_Id", "Node_Id");
                                                bc.ColumnMappings.Add("TLD_Id", "TLD_Id");
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("nodeStatus", "nodeStatus");
                                                bc.ColumnMappings.Add("id", "id");
                                                bc.WriteToServer(Node);

                                            }
                                            using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                            {
                                                DataTable Event = ds.Tables["Event"];
                                                Event.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Event.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Event.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Event";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                //  bc.ColumnMappings.Add("TRX_UnaDrawerOpening", "TRX_UnaDrawerOpening");
                                                bc.ColumnMappings.Add("RegId", "RegId");
                                                bc.ColumnMappings.Add("Type", "Type");
                                                bc.ColumnMappings.Add("Time", "Time");
                                                bc.ColumnMappings.Add("Node_Id", "Node_Id");
                                                bc.WriteToServer(Event);

                                            }





                                            string smt = "INSERT INTO [STLD].[dbo].[STLDPROCESS_STATUS] ([STLD_Location] ,[business_date]) VALUES (@STLD_Location,@business_date)";
                                            SqlCommand cmd = new SqlCommand(smt, con, tran);
                                            cmd.Parameters.AddWithValue("@STLD_Location", SqlDbType.VarChar).Value = lbl_location.Text;
                                            cmd.Parameters.AddWithValue("@business_date", SqlDbType.VarChar).Value = lbl_business.Text;
                                            cmd.ExecuteNonQuery();
                                            File.Move(file, Path.ChangeExtension(file, ".Proceed"));

                                            //////////////////////


                                            


                                            tran.Commit();


                                        }
                                        catch
                                        {
                                            tran.Rollback();
                                            // throw;
                                        }

                                    }

                                }









                            }
                        }
                    }
                    catch
                    {

                    }




                    ///////////////////////////// Check dublicate records////////////////////////








                    //////////////////////////////////////////////////////////////////

                }
                else
                {
                    //File.Delete(file);
                }

            }


            //////// other stld format checking 
            // BTN_OTEHR.PerformClick();
        }







































        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            SQLCON prms = new SQLCON();
            prms.OpenConnection();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Btn_rename_Click(object sender, EventArgs e)
        {
            BTN_CLEARLBLES.PerformClick();
            foreach (string file20 in Directory.GetFiles("E:\\ImpotSheiV2\\Extract\\"))
            {

                File.Move(file20, Path.ChangeExtension(file20, ".zip"));
            }





            foreach (string file11 in Directory.GetFiles("E:\\ImpotSheiV2\\Extract", "*.zip"))
            {
                try
                {
                    ZipFile.ExtractToDirectory(file11, "E:\\ImpotSheiV2\\Extract\\Processingfiles");
                    File.Delete(file11);
                }
                catch
                {
                    File.Delete(file11);
                }




            }

            foreach (string file10 in Directory.GetFiles("E:\\ImpotSheiV2\\Extract", "*.log"))
            {
                File.Delete(file10);
            }

            foreach (string file11 in Directory.GetFiles("E:\\ImpotSheiV2\\Extract\\Processingfiles", "*.zip"))
            {
                ZipFile.ExtractToDirectory(file11, "E:\\ImpotSheiV2\\Extract\\Processingfiles");
                File.Delete(file11);


            }

            foreach (string file12 in Directory.GetFiles("E:\\ImpotSheiV2\\Extract\\Processingfiles", "*.log"))
            {
                // ZipFile.ExtractToDirectory(file12, "D:\\ImpotSheiV2\\Extract\\Processingfiles");
                File.Delete(file12);


            }

            foreach (string file5 in Directory.GetFiles("E:\\ImpotSheiV2\\Extract\\Processingfiles", "*.log"))
            {
                File.Delete(file5);

            }
            foreach (string file6 in Directory.GetFiles("E:\\ImpotSheiV2\\Extract\\", "*.log"))
            {
                File.Delete(file6);

            }

            //// check file by file//////////
            BTN_PMIX.PerformClick();

            button4.PerformClick();
            //BTN_PMIX.PerformClick();///pmix importing
            foreach (string file in Directory.EnumerateFiles("E:\\ImpotSheiV2\\Extract\\Processingfiles", "*.xml"))
            {


                string contents = File.ReadAllText(file);
                ///////////////////// File Reading/////////////////////////

                lbl_nameval.Text = Convert.ToString(file.ToString());
                if (lbl_nameval.Text.Contains("STLD"))
                {



                    string connString = SQLCON.ConnectionString2;
                    lbl_STLD.Text = file;

                    SQLCON vb = new SQLCON();
                    vb.OpenConnection();

                    DataSet ds = new DataSet();

                    ds.ReadXml(lbl_STLD.Text);









                    DataTable CHKTLD = ds.Tables["TLD"];
                    foreach (DataRow row in CHKTLD.Rows)
                    {
                        // ... Write value of first field as integer.
                        // string storloc = row.ItemArray[7].ToString();Commented due to changes of adc
                        string storloc = row.ItemArray[8].ToString();
                        string storbusinessdate = row.ItemArray[1].ToString();

                        lbl_location.Text = storloc;






                        lbl_business.Text = storbusinessdate;
                    }

                    using (SqlConnection con1 = new SqlConnection(connString))
                    {

                        con1.Open();

                        SqlCommand comm = new SqlCommand("SELECT COUNT(*) FROM [STLDPROCESS_STATUS] where [STLD_Location]=@STLD_Location and business_date=@business_date", con1);
                        comm.Parameters.AddWithValue("@STLD_Location", SqlDbType.VarChar).Value = lbl_location.Text;
                        comm.Parameters.AddWithValue("@business_date", SqlDbType.VarChar).Value = lbl_business.Text;
                        Int32 count = Convert.ToInt32(comm.ExecuteScalar());
                        if (count > 0)
                        {
                            lblCount.Text = Convert.ToString(count.ToString()); //For example a Label
                        }
                        else
                        {
                            if (lbl_location.Text.Contains("online"))
                            {
                                lblCount.Text = "wrong";
                            }
                            else
                            {
                                lblCount.Text = "0";
                                SqlCommand comm2 = new SqlCommand("SELECT [StoreID]   FROM [STLD].[dbo].[TB_STOREMASTER] where [StoreID]=@STLD_Location", con1);
                                comm2.Parameters.AddWithValue("@STLD_Location", SqlDbType.VarChar).Value = lbl_location.Text;
                                Int32 count2 = Convert.ToInt32(comm2.ExecuteScalar());

                                if (count2 > 0)
                                {
                                    lblCount.Text = "0"; //For example a Label
                                }
                                else
                                {
                                    lblCount.Text = "wrong";
                                }
                            }
                        }
                        con1.Close();

                    }
                    
                    ///////////////////////////// Check dublicate records////////////////////////






                    using (SqlConnection con = new SqlConnection(connString))
                    {
                        con.Open();

                        if (lblCount.Text.Contains('0'))
                        {

                            using (var connection = con)
                            {
                                using (var tran = con.BeginTransaction())
                                {


                                    try
                                    {




                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {

                                            DataTable TLD = ds.Tables["TLD"];
                                            foreach (DataRow row in TLD.Rows)
                                            {
                                                // ... Write value of first field as integer.
                                                //string storloc = row.ItemArray[7].ToString();//Adc Changes
                                                string storloc = row.ItemArray[8].ToString();
                                                string storbusinessdate = row.ItemArray[1].ToString();

                                                lbl_location.Text = storloc;
                                                lbl_business.Text = storbusinessdate;
                                            }





                                            DataTable TRX_Sale = ds.Tables["TRX_Sale"];

                                            TRX_Sale.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "STLD_Location",
                                                DataType = typeof(string)
                                            });

                                            TRX_Sale.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "business_date",
                                                DataType = typeof(string)
                                            });
                                            foreach (DataRow vrow in TRX_Sale.Rows)
                                            {

                                                vrow["STLD_Location"] = lbl_location.Text;
                                                vrow["business_date"] = lbl_business.Text;

                                            }



                                            bc.DestinationTableName = "TRX_Sale";
                                            bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            bc.ColumnMappings.Add("business_date", "business_date");
                                            bc.ColumnMappings.Add("TRX_Sale_Id", "TRX_Sale_Id");
                                            bc.ColumnMappings.Add("status", "status");
                                            bc.ColumnMappings.Add("POD", "POD");
                                            bc.ColumnMappings.Add("RemPOD", "RemPOD");
                                            bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                            bc.WriteToServer(TRX_Sale);

                                        }




                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {

                                            //------- Order Table////////////////
                                            DataTable Order_TB = ds.Tables["Order"];
                                            Order_TB.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "STLD_Location",
                                                DataType = typeof(string)
                                            });

                                            Order_TB.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "business_date",
                                                DataType = typeof(string)
                                            });
                                            foreach (DataRow vrow in Order_TB.Rows)
                                            {

                                                vrow["STLD_Location"] = lbl_location.Text;
                                                vrow["business_date"] = lbl_business.Text;

                                            }
                                            bc.DestinationTableName = "Order_TB";
                                            bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            bc.ColumnMappings.Add("business_date", "business_date");
                                            bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                            bc.ColumnMappings.Add("Timestamp", "Timestamp");
                                            bc.ColumnMappings.Add("uniqueId", "uniqueId");
                                            bc.ColumnMappings.Add("kind", "kind");
                                            bc.ColumnMappings.Add("key", "key");
                                            bc.ColumnMappings.Add("major", "major");
                                            bc.ColumnMappings.Add("minor", "minor");
                                            bc.ColumnMappings.Add("side", "side");
                                            bc.ColumnMappings.Add("receiptNumber", "receiptNumber");
                                            bc.ColumnMappings.Add("fpReceiptNumber", "fpReceiptNumber");
                                            //bc.ColumnMappings.Add("boot", "boot");
                                            bc.ColumnMappings.Add("saleType", "saleType");
                                            bc.ColumnMappings.Add("totalAmount", "totalAmount");
                                            bc.ColumnMappings.Add("nonProductAmount", "nonProductAmount");
                                            bc.ColumnMappings.Add("totalTax", "totalTax");
                                            bc.ColumnMappings.Add("nonProductTax", "nonProductTax");
                                            bc.ColumnMappings.Add("orderSrc", "orderSrc");
                                            bc.ColumnMappings.Add("startSaleDate", "startSaleDate");
                                            bc.ColumnMappings.Add("startSaleTime", "startSaleTime");
                                            bc.ColumnMappings.Add("endSaleDate", "endSaleDate");
                                            bc.ColumnMappings.Add("endSaleTime", "endSaleTime");
                                            bc.ColumnMappings.Add("TRX_Sale_Id", "TRX_Sale_Id");
                                            bc.WriteToServer(Order_TB);

                                            //----------------------------------



                                        }

                                        //------refund
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con))
                                        {
                                            // DataTable TRX_Refund = ds.Tables["TRX_Refund"];

                                            // TRX_Refund.Columns.Add(new DataColumn()
                                            // {
                                            //     ColumnName = "STLD_Location",
                                            //     DataType = typeof(string)
                                            // });

                                            // TRX_Refund.Columns.Add(new DataColumn()
                                            // {
                                            //     ColumnName = "business_date",
                                            //     DataType = typeof(string)
                                            // });
                                            // foreach (DataRow vrow in TRX_Refund.Rows)
                                            // {

                                            //     vrow["STLD_Location"] = lbl_location.Text;
                                            //     vrow["business_date"] = lbl_business.Text;

                                            // }



                                            // bc.DestinationTableName = "TRX_Refund";
                                            // bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            // bc.ColumnMappings.Add("business_date", "business_date");
                                            // bc.ColumnMappings.Add("TRX_Refund_Id", "TRX_Refund_Id");
                                            //bc.ColumnMappings.Add("status", "status");
                                            // bc.ColumnMappings.Add("POD", "POD");
                                            // bc.ColumnMappings.Add("RemPOD", "RemPOD");
                                            // bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                            // bc.WriteToServer(TRX_Refund);

                                        }



                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {

                                            //------- Item Table////////////////
                                            DataTable Item_TB = ds.Tables["Item"];
                                            Item_TB.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "STLD_Location",
                                                DataType = typeof(string)
                                            });

                                            Item_TB.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "business_date",
                                                DataType = typeof(string)
                                            });
                                            foreach (DataRow vrow in Item_TB.Rows)
                                            {

                                                vrow["STLD_Location"] = lbl_location.Text;
                                                vrow["business_date"] = lbl_business.Text;

                                            }

                                            bc.DestinationTableName = "Item_TB";
                                            bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            bc.ColumnMappings.Add("business_date", "business_date");
                                            bc.ColumnMappings.Add("Item_Id", "Item_Id");
                                            bc.ColumnMappings.Add("code", "code");
                                            bc.ColumnMappings.Add("type", "type");
                                            bc.ColumnMappings.Add("action", "action");
                                            bc.ColumnMappings.Add("level", "level");
                                            bc.ColumnMappings.Add("id", "id");
                                            bc.ColumnMappings.Add("displayOrder", "displayOrder");
                                            bc.ColumnMappings.Add("qty", "qty");
                                            bc.ColumnMappings.Add("grillQty", "grillQty");
                                            bc.ColumnMappings.Add("grillModifier", "grillModifier");
                                            bc.ColumnMappings.Add("qtyPromo", "qtyPromo");
                                            bc.ColumnMappings.Add("chgAfterTotal", "chgAfterTotal");
                                            bc.ColumnMappings.Add("BPPrice", "BPPrice");
                                            bc.ColumnMappings.Add("BPTax", "BPTax");
                                            bc.ColumnMappings.Add("BDPrice", "BDPrice");
                                            bc.ColumnMappings.Add("BDTax", "BDTax");
                                            bc.ColumnMappings.Add("totalPrice", "totalPrice");
                                            bc.ColumnMappings.Add("totalTax", "totalTax");
                                            bc.ColumnMappings.Add("category", "category");
                                            bc.ColumnMappings.Add("familyGroup", "familyGroup");
                                            bc.ColumnMappings.Add("daypart", "daypart");
                                            bc.ColumnMappings.Add("description", "description");
                                            bc.ColumnMappings.Add("department", "department");
                                            bc.ColumnMappings.Add("departmentClass", "departmentClass");
                                            bc.ColumnMappings.Add("departmentSubClass", "departmentSubClass");
                                            bc.ColumnMappings.Add("unitPrice", "unitPrice");
                                            bc.ColumnMappings.Add("unitTax", "unitTax");
                                            bc.ColumnMappings.Add("solvedChoice", "solvedChoice");
                                            bc.ColumnMappings.Add("isUpcharge", "isUpcharge");
                                            bc.ColumnMappings.Add("Item_Id_0", "Item_Id_0");
                                            bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                            bc.WriteToServer(Item_TB);


                                            ////////////////////////////////////

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {
                                            ///-------------- Promo table-----------------
                                            DataTable TRX_Overring = ds.Tables["TRX_Overring"];
                                            if (TRX_Overring != null)
                                            {
                                                TRX_Overring.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });
                                                TRX_Overring.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TRX_Overring.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TRX_Overring";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("TRX_Overring_Id", "TRX_Overring_Id");
                                                bc.ColumnMappings.Add("POD", "POD");
                                                bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                bc.WriteToServer(TRX_Overring);
                                            }
                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {
                                            ///-------------- Promo table-----------------
                                            DataTable Product = ds.Tables["Product"];
                                            if (Product != null)
                                            {
                                                Product.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });
                                                Product.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Product.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Product";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("code", "code");
                                                bc.ColumnMappings.Add("quantity", "quantity");
                                                bc.ColumnMappings.Add("Components_Id", "Components_Id");
                                                bc.WriteToServer(Product);
                                            }
                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {
                                            ///-------------- Promo table-----------------
                                            DataTable Components = ds.Tables["Components"];
                                            if (Components != null)
                                            {
                                                Components.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });
                                                Components.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Components.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Components";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("Components_Id", "Components_Id");
                                                bc.ColumnMappings.Add("Ev_BreakValueMeal_Id", "Ev_BreakValueMeal_Id");
                                                bc.WriteToServer(Components);
                                            }
                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {
                                            ///-------------- Promo table-----------------
                                            DataTable Ev_BreakValueMeal = ds.Tables["Ev_BreakValueMeal"];
                                            if (Ev_BreakValueMeal != null)
                                            {
                                                Ev_BreakValueMeal.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });
                                                Ev_BreakValueMeal.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Ev_BreakValueMeal.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Ev_BreakValueMeal";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("ProductCode", "ProductCode");
                                                bc.ColumnMappings.Add("Quantity", "Quantity");
                                                bc.ColumnMappings.Add("Ev_BreakValueMeal_Id", "Ev_BreakValueMeal_Id");
                                                bc.WriteToServer(Ev_BreakValueMeal);
                                            }
                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {
                                            ///-------------- Promo table-----------------
                                            DataTable Promo = ds.Tables["Promo"];
                                            if (Promo != null)
                                            {
                                                Promo.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Promo.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Promo.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }


                                                bc.DestinationTableName = "Promo";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("id", "id");
                                                bc.ColumnMappings.Add("name", "name");
                                                bc.ColumnMappings.Add("qty", "qty");
                                                bc.ColumnMappings.Add("Item_id", "Item_id");
                                                bc.WriteToServer(Promo);
                                            }
                                            //------------------------------------------
                                        }

                                        using (SqlBulkCopy bc = new SqlBulkCopy(con))
                                        {
                                            ///////------------------------
                                            // DataTable PromotionApplied = ds.Tables["PromotionApplied"];
                                            //  if (PromotionApplied != null)
                                            //  {
                                            //     PromotionApplied.Columns.Add(new DataColumn()
                                            //    {
                                            //        ColumnName = "STLD_Location",
                                            //        DataType = typeof(string)
                                            //    });

                                            //    PromotionApplied.Columns.Add(new DataColumn()
                                            //    {
                                            //        ColumnName = "business_date",
                                            //        DataType = typeof(string)
                                            //    });
                                            //    foreach (DataRow vrow in PromotionApplied.Rows)
                                            //    {

                                            //        vrow["STLD_Location"] = lbl_location.Text;
                                            //        vrow["business_date"] = lbl_business.Text;

                                            //    }
                                            //    bc.DestinationTableName = "PromotionApplied";
                                            //    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            //    bc.ColumnMappings.Add("business_date", "business_date");
                                            //    bc.ColumnMappings.Add("promotionId", "promotionId");
                                            //    bc.ColumnMappings.Add("promotionCounter", "promotionCounter");
                                            //    bc.ColumnMappings.Add("eligible", "eligible");
                                            //    bc.ColumnMappings.Add("originalPrice", "originalPrice");
                                            //    bc.ColumnMappings.Add("discountAmount", "discountAmount");
                                            //    bc.ColumnMappings.Add("discountType", "discountType");
                                            //    bc.ColumnMappings.Add("originalItemPromoQty", "originalItemPromoQty");
                                            //    bc.ColumnMappings.Add("originalProductCode", "originalProductCode");
                                            //    bc.ColumnMappings.Add("offerId", "offerId");
                                            //    bc.ColumnMappings.Add("Item_Id", "Item_Id");
                                            //    bc.WriteToServer(PromotionApplied);
                                            //    ///////////////////////

                                            //}
                                        }

                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {
                                            ////////// Offer ////// Table
                                            DataTable Offers = ds.Tables["Offers"];

                                            if (Offers != null)
                                            {

                                                Offers.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Offers.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Offers.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Offers";
                                                if (Offers.Columns.Contains("STLD_Location"))
                                                {

                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                }
                                                if (Offers.Columns.Contains("business_date"))
                                                {

                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                }
                                                if (Offers.Columns.Contains("offerId"))
                                                {

                                                    bc.ColumnMappings.Add("offerId", "offerId");
                                                }

                                                if (Offers.Columns.Contains("beforeOfferPrice"))
                                                {

                                                    bc.ColumnMappings.Add("beforeOfferPrice", "beforeOfferPrice");
                                                }

                                                if (Offers.Columns.Contains("discountAmount"))
                                                {

                                                    bc.ColumnMappings.Add("discountAmount", "discountAmount");
                                                }

                                                if (Offers.Columns.Contains("discountType"))
                                                {

                                                    bc.ColumnMappings.Add("discountType", "discountType");
                                                }


                                                if (Offers.Columns.Contains("Item_Id"))
                                                {

                                                    bc.ColumnMappings.Add("Item_Id", "Item_Id");
                                                }

                                                if (Offers.Columns.Contains("Item_Id"))
                                                {

                                                    bc.ColumnMappings.Add("Item_Id", "Item_Id");
                                                }

                                                if (Offers.Columns.Contains("customerId"))
                                                {

                                                    bc.ColumnMappings.Add("customerId", "customerId");
                                                }
                                                if (Offers.Columns.Contains("offerName"))
                                                {

                                                    bc.ColumnMappings.Add("offerName", "offerName");
                                                }

                                                if (Offers.Columns.Contains("override"))
                                                {

                                                    bc.ColumnMappings.Add("override", "override");
                                                }
                                                if (Offers.Columns.Contains("applied"))
                                                {

                                                    bc.ColumnMappings.Add("applied", "applied");
                                                }
                                                if (Offers.Columns.Contains("promotionId"))
                                                {

                                                    bc.ColumnMappings.Add("promotionId", "promotionId");
                                                }

                                                if (Offers.Columns.Contains("offerBarcodeType"))
                                                {

                                                    bc.ColumnMappings.Add("offerBarcodeType", "offerBarcodeType");
                                                }

                                                if (Offers.Columns.Contains("Order_Id"))
                                                {

                                                    bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                }

                                                bc.WriteToServer(Offers);
                                            }
                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {
                                            DataTable TaxChain = ds.Tables["TaxChain"];

                                            TaxChain.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "STLD_Location",
                                                DataType = typeof(string)
                                            });

                                            TaxChain.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "business_date",
                                                DataType = typeof(string)
                                            });
                                            foreach (DataRow vrow in TaxChain.Rows)
                                            {

                                                vrow["STLD_Location"] = lbl_location.Text;
                                                vrow["business_date"] = lbl_business.Text;

                                            }
                                            bc.DestinationTableName = "TaxChain";
                                            bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            bc.ColumnMappings.Add("business_date", "business_date");
                                            bc.ColumnMappings.Add("id", "id");
                                            bc.ColumnMappings.Add("name", "name");
                                            bc.ColumnMappings.Add("rate", "rate");
                                            bc.ColumnMappings.Add("baseAmount", "baseAmount");
                                            bc.ColumnMappings.Add("amount", "amount");
                                            bc.ColumnMappings.Add("BDBaseAmount", "BDBaseAmount");
                                            bc.ColumnMappings.Add("BDAmount", "BDAmount");
                                            bc.ColumnMappings.Add("Item_Id", "Item_Id");
                                            // bc.ColumnMappings.Add("BPBaseAmount", "BPBaseAmount");
                                            //bc.ColumnMappings.Add("BPAmount", "BPAmount");
                                            bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                            bc.WriteToServer(TaxChain);

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {

                                            DataTable Promotions = ds.Tables["Promotions"];
                                            if (Promotions != null)
                                            {
                                                Promotions.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Promotions.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Promotions.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }

                                                bc.DestinationTableName = "Promotions";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("Promotions_Id", "Promotions_Id");
                                                bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                bc.WriteToServer(Promotions);
                                            }
                                        }


                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {

                                            DataTable Promotion = ds.Tables["Promotion"];
                                            if (Promotion != null)
                                            {


                                                Promotion.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Promotion.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Promotion.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }

                                                bc.DestinationTableName = "Promotion";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("promotionId", "promotionId");
                                                bc.ColumnMappings.Add("promotionName", "promotionName");
                                                bc.ColumnMappings.Add("promotionCounter", "promotionCounter");
                                                bc.ColumnMappings.Add("discountType", "discountType");
                                                bc.ColumnMappings.Add("discountAmount", "discountAmount");
                                                bc.ColumnMappings.Add("offerId", "offerId");
                                                bc.ColumnMappings.Add("exclusive", "exclusive");
                                                bc.ColumnMappings.Add("promotionOnTender", "promotionOnTender");
                                                bc.ColumnMappings.Add("countTowardsPromotionLimit", "countTowardsPromotionLimit");
                                                bc.ColumnMappings.Add("returnedValue", "returnedValue");
                                                bc.ColumnMappings.Add("Promotions_Id", "Promotions_Id");
                                                bc.WriteToServer(Promotion);

                                            }
                                        }


                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {

                                            DataTable Customer = ds.Tables["Customer"];
                                            if (Customer != null)
                                            {
                                                Customer.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Customer.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Customer.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }


                                                bc.DestinationTableName = "Customer";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("id", "id");
                                                bc.ColumnMappings.Add("nickname", "nickname");
                                                bc.ColumnMappings.Add("greeting", "greeting");
                                                bc.ColumnMappings.Add("loyaltyCardId", "loyaltyCardId");
                                                bc.ColumnMappings.Add("loyaltyCardType", "loyaltyCardType");
                                                bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                bc.WriteToServer(Customer);
                                            }
                                        }


                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {

                                            DataTable CustomInfo = ds.Tables["CustomInfo"];
                                            if (CustomInfo != null)
                                            {
                                                CustomInfo.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                CustomInfo.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in CustomInfo.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "CustomInfo";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("CustomInfo_Id", "CustomInfo_Id");
                                                bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                bc.WriteToServer(CustomInfo);


                                            }
                                        }


                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {

                                            DataTable Tenders = ds.Tables["Tenders"];
                                            Tenders.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "STLD_Location",
                                                DataType = typeof(string)
                                            });

                                            Tenders.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "business_date",
                                                DataType = typeof(string)
                                            });
                                            foreach (DataRow vrow in Tenders.Rows)
                                            {

                                                vrow["STLD_Location"] = lbl_location.Text;
                                                vrow["business_date"] = lbl_business.Text;

                                            }
                                            bc.DestinationTableName = "Tenders";
                                            bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            bc.ColumnMappings.Add("business_date", "business_date");
                                            bc.ColumnMappings.Add("Tenders_Id", "Tenders_Id");
                                            bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                            bc.WriteToServer(Tenders);
                                        }

                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {

                                            DataTable Tender = ds.Tables["Tender"];
                                            Tender.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "STLD_Location",
                                                DataType = typeof(string)
                                            });

                                            Tender.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "business_date",
                                                DataType = typeof(string)
                                            });
                                            foreach (DataRow vrow in Tender.Rows)
                                            {

                                                vrow["STLD_Location"] = lbl_location.Text;
                                                vrow["business_date"] = lbl_business.Text;

                                            }
                                            bc.DestinationTableName = "Tender";
                                            bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            bc.ColumnMappings.Add("business_date", "business_date");
                                            bc.ColumnMappings.Add("TenderId", "TenderId");
                                            bc.ColumnMappings.Add("TenderKind", "TenderKind");
                                            bc.ColumnMappings.Add("TenderName", "TenderName");
                                            bc.ColumnMappings.Add("TenderQuantity", "TenderQuantity");
                                            bc.ColumnMappings.Add("FaceValue", "FaceValue");
                                            bc.ColumnMappings.Add("TenderAmount", "TenderAmount");
                                            bc.ColumnMappings.Add("BaseAction", "BaseAction");
                                            bc.ColumnMappings.Add("Persisted", "Persisted");
                                            bc.ColumnMappings.Add("CardProviderID", "CardProviderID");
                                            bc.ColumnMappings.Add("CashlessData", "CashlessData");
                                            bc.ColumnMappings.Add("TaxOption", "TaxOption");
                                            bc.ColumnMappings.Add("SubtotalOption", "SubtotalOption");
                                            bc.ColumnMappings.Add("ForeignCurrencyIndicator", "ForeignCurrencyIndicator");
                                            bc.ColumnMappings.Add("DiscountDescription", "DiscountDescription");
                                            bc.ColumnMappings.Add("CashlessTransactionID", "CashlessTransactionID");
                                            bc.ColumnMappings.Add("PaymentChannel", "PaymentChannel");
                                            bc.ColumnMappings.Add("Tenders_Id", "Tenders_Id");
                                            bc.WriteToServer(Tender);


                                        }

                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable POSTiming = ds.Tables["POSTiming"];
                                            if (POSTiming != null)
                                            {
                                                POSTiming.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                POSTiming.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in POSTiming.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }

                                            }


                                        }

                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable EventsDetail = ds.Tables["EventsDetail"];
                                            if (EventsDetail != null)
                                            {
                                                EventsDetail.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                EventsDetail.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in EventsDetail.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "EventsDetail";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("EventsDetail_Id", "EventsDetail_Id");
                                                bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                bc.WriteToServer(EventsDetail);

                                            }


                                        }

                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable SaleEvent = ds.Tables["SaleEvent"];
                                            if (SaleEvent != null)
                                            {
                                                SaleEvent.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                SaleEvent.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in SaleEvent.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                //bc.DestinationTableName = "SaleEvent";
                                                //bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                //bc.ColumnMappings.Add("business_date", "business_date");
                                                //bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                //bc.ColumnMappings.Add("Ev_SaleStored", "Ev_SaleStored");
                                                //bc.ColumnMappings.Add("Ev_SaleRecalled", "Ev_SaleRecalled");
                                                //bc.ColumnMappings.Add("Ev_BackFromTotal", "Ev_BackFromTotal");
                                                //bc.ColumnMappings.Add("Ev_SaleTotal", "Ev_SaleTotal");
                                                //bc.ColumnMappings.Add("Type", "Type");
                                                //bc.ColumnMappings.Add("Time", "Time");
                                                //bc.ColumnMappings.Add("EventsDetail_Id", "EventsDetail_Id");
                                                //bc.WriteToServer(SaleEvent);

                                            }

                                        }

                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Ev_SaleIncrementItemQty = ds.Tables["Ev_SaleIncrementItemQty"];
                                            if (Ev_SaleIncrementItemQty != null)
                                            {
                                                Ev_SaleIncrementItemQty.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Ev_SaleIncrementItemQty.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Ev_SaleIncrementItemQty.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Ev_SaleIncrementItemQty";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("SaleIndex", "SaleIndex");
                                                bc.ColumnMappings.Add("Quantity", "Quantity");
                                                bc.ColumnMappings.Add("ProductCode", "ProductCode");
                                                bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                bc.WriteToServer(Ev_SaleIncrementItemQty);

                                            }

                                        }

                                        using (SqlBulkCopy bc = new SqlBulkCopy(con))
                                        {


                                            DataTable TRX_GetAuthorization = ds.Tables["TRX_GetAuthorization"];
                                            if (TRX_GetAuthorization != null)
                                            {
                                                TRX_GetAuthorization.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TRX_GetAuthorization.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TRX_GetAuthorization.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                //bc.DestinationTableName = "TRX_GetAuthorization";
                                                //bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                //bc.ColumnMappings.Add("business_date", "business_date");
                                                //bc.ColumnMappings.Add("Action", "Action");
                                                //bc.ColumnMappings.Add("ManagerID", "ManagerID");
                                                //bc.ColumnMappings.Add("ManagerName", "ManagerName");
                                                //bc.ColumnMappings.Add("SecurityLevel", "SecurityLevel");
                                                //bc.ColumnMappings.Add("ExpirationDate", "ExpirationDate");
                                                //bc.ColumnMappings.Add("Password", "Password");
                                                //bc.ColumnMappings.Add("Islogged", "Islogged");
                                                //bc.ColumnMappings.Add("Method", "Method");
                                                ////bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                //bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                //bc.WriteToServer(TRX_GetAuthorization);

                                            }

                                        }

                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable EV_NotChargedPromotional = ds.Tables["EV_NotChargedPromotional"];
                                            if (EV_NotChargedPromotional != null)
                                            {
                                                EV_NotChargedPromotional.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                EV_NotChargedPromotional.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in EV_NotChargedPromotional.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "EV_NotChargedPromotional";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("ProductCode", "ProductCode");
                                                bc.ColumnMappings.Add("Quantity", "Quantity");
                                                bc.ColumnMappings.Add("NotChargedValue", "NotChargedValue");
                                                bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                bc.WriteToServer(EV_NotChargedPromotional);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Ev_AddTender = ds.Tables["Ev_AddTender"];
                                            if (Ev_AddTender != null)
                                            {
                                                Ev_AddTender.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Ev_AddTender.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Ev_AddTender.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Ev_AddTender";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("TenderId", "TenderId");
                                                bc.ColumnMappings.Add("FaceValue", "FaceValue");
                                                bc.ColumnMappings.Add("TenderAmount", "TenderAmount");
                                                bc.ColumnMappings.Add("BaseAction", "BaseAction");
                                                bc.ColumnMappings.Add("Persisted", "Persisted");
                                                bc.ColumnMappings.Add("CardProviderID", "CardProviderID");
                                                bc.ColumnMappings.Add("CashlessData", "CashlessData");
                                                bc.ColumnMappings.Add("CashlessTransactionID", "CashlessTransactionID");
                                                bc.ColumnMappings.Add("PreAuthorization", "PreAuthorization");
                                                bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                bc.WriteToServer(Ev_AddTender);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Ev_SetSaleType = ds.Tables["Ev_SetSaleType"];
                                            if (Ev_SetSaleType != null)
                                            {
                                                Ev_SetSaleType.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Ev_SetSaleType.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Ev_SetSaleType.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Ev_SetSaleType";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("Type", "Type");
                                                bc.ColumnMappings.Add("ForceExhibition", "ForceExhibition");
                                                bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                bc.WriteToServer(Ev_SetSaleType);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Ev_SaleChoice = ds.Tables["Ev_SaleChoice"];
                                            if (Ev_SaleChoice != null)
                                            {
                                                Ev_SaleChoice.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Ev_SaleChoice.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Ev_SaleChoice.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Ev_SaleChoice";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("ProductCode", "ProductCode");
                                                bc.ColumnMappings.Add("ChoiceCode", "ChoiceCode");
                                                bc.ColumnMappings.Add("Quantity", "Quantity");
                                                bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                bc.WriteToServer(Ev_SaleChoice);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Ev_SaleItem = ds.Tables["Ev_SaleItem"];
                                            if (Ev_SaleItem != null)
                                            {
                                                Ev_SaleItem.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Ev_SaleItem.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Ev_SaleItem.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Ev_SaleItem";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("ProductCode", "ProductCode");
                                                bc.ColumnMappings.Add("Quantity", "Quantity");
                                                bc.ColumnMappings.Add("UpdatedQuantity", "UpdatedQuantity");
                                                bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                bc.WriteToServer(Ev_SaleItem);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Ev_SaleCutomInfo = ds.Tables["Ev_SaleCutomInfo"];
                                            if (Ev_SaleCutomInfo != null)
                                            {
                                                Ev_SaleCutomInfo.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Ev_SaleCutomInfo.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Ev_SaleCutomInfo.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                //bc.DestinationTableName = "Ev_SaleCutomInfo";
                                                //bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                //bc.ColumnMappings.Add("business_date", "business_date");
                                                //bc.ColumnMappings.Add("ProductCode", "ProductCode");
                                                //bc.ColumnMappings.Add("Quantity", "Quantity");
                                                //bc.ColumnMappings.Add("UpdatedQuantity", "UpdatedQuantity");
                                                //bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                //bc.WriteToServer(Ev_SaleCutomInfo);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Ev_SaleEnd = ds.Tables["Ev_SaleEnd"];
                                            if (Ev_SaleEnd != null)
                                            {
                                                Ev_SaleEnd.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Ev_SaleEnd.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Ev_SaleEnd.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Ev_SaleEnd";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("Type", "Type");
                                                bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                bc.WriteToServer(Ev_SaleEnd);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Ev_SaleStart = ds.Tables["Ev_SaleStart"];
                                            if (Ev_SaleStart != null)
                                            {
                                                Ev_SaleStart.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Ev_SaleStart.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Ev_SaleStart.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Ev_SaleStart";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("DisabledChoices", "DisabledChoices");
                                                bc.ColumnMappings.Add("TenderPersisted", "TenderPersisted");
                                                bc.ColumnMappings.Add("Multiorder", "Multiorder");
                                                bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                bc.WriteToServer(Ev_SaleStart);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Fiscal_Information = ds.Tables["Fiscal_Information"];
                                            if (Fiscal_Information != null)
                                            {
                                                Fiscal_Information.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Fiscal_Information.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Fiscal_Information.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Fiscal_Information";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("TIN", "TIN");
                                                bc.ColumnMappings.Add("name", "name");
                                                bc.ColumnMappings.Add("address", "address");
                                                bc.ColumnMappings.Add("ZIP", "ZIP");
                                                bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                bc.WriteToServer(Fiscal_Information);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Ev_DrawerClose = ds.Tables["Ev_DrawerClose"];
                                            if (Ev_DrawerClose != null)
                                            {
                                                Ev_DrawerClose.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Ev_DrawerClose.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Ev_DrawerClose.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Ev_DrawerClose";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("TotalOpenTime", "TotalOpenTime");
                                                bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                bc.WriteToServer(Ev_DrawerClose);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable TRX_OperLogin = ds.Tables["TRX_OperLogin"];
                                            if (TRX_OperLogin != null)
                                            {
                                                TRX_OperLogin.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TRX_OperLogin.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TRX_OperLogin.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TRX_OperLogin";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("CrewID", "CrewID");
                                                bc.ColumnMappings.Add("CrewName", "CrewName");
                                                bc.ColumnMappings.Add("CrewSecurityLevel", "CrewSecurityLevel");
                                                bc.ColumnMappings.Add("POD", "POD");
                                                bc.ColumnMappings.Add("RemotePOD", "RemotePOD");
                                                bc.ColumnMappings.Add("AutoLogin", "AutoLogin");
                                                bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                bc.WriteToServer(TRX_OperLogin);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable TRX_SetPOD = ds.Tables["TRX_SetPOD"];
                                            if (TRX_SetPOD != null)
                                            {
                                                TRX_SetPOD.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TRX_SetPOD.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TRX_SetPOD.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TRX_SetPOD";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("PODId", "PODId");
                                                bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                bc.WriteToServer(TRX_SetPOD);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable TRX_DayOpen = ds.Tables["TRX_DayOpen"];
                                            if (TRX_DayOpen != null)
                                            {
                                                TRX_DayOpen.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TRX_DayOpen.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TRX_DayOpen.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TRX_DayOpen";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("BusinessDate", "BusinessDate");
                                                bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                bc.WriteToServer(TRX_DayOpen);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable TRX_TaxTable = ds.Tables["TRX_TaxTable"];
                                            if (TRX_TaxTable != null)
                                            {
                                                TRX_TaxTable.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TRX_TaxTable.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TRX_TaxTable.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TRX_TaxTable";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("TRX_TaxTable_Id", "TRX_TaxTable_Id");
                                                bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                bc.WriteToServer(TRX_TaxTable);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable TaxType = ds.Tables["TaxType"];
                                            if (TaxType != null)
                                            {
                                                TaxType.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TaxType.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TaxType.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TaxType";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("TaxId", "TaxId");
                                                bc.ColumnMappings.Add("TaxDescription", "TaxDescription");
                                                bc.ColumnMappings.Add("TaxRate", "TaxRate");
                                                bc.ColumnMappings.Add("TaxBasis", "TaxBasis");
                                                bc.ColumnMappings.Add("TaxCalcType", "TaxCalcType");
                                                bc.ColumnMappings.Add("Rounding", "Rounding");
                                                bc.ColumnMappings.Add("Precision", "Precision");
                                                bc.ColumnMappings.Add("TRX_TaxTable_Id", "TRX_TaxTable_Id");
                                                bc.WriteToServer(TaxType);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable TRX_TenderTable = ds.Tables["TRX_TenderTable"];
                                            if (TRX_TenderTable != null)
                                            {
                                                TRX_TenderTable.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TRX_TenderTable.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TRX_TenderTable.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TRX_TenderTable";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("TRX_TenderTable_Id", "TRX_TenderTable_Id");
                                                bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                bc.WriteToServer(TRX_TenderTable);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable TenderType = ds.Tables["TenderType"];
                                            if (TenderType != null)
                                            {
                                                TenderType.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TenderType.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TenderType.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TenderType";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("TenderId", "TenderId");
                                                bc.ColumnMappings.Add("TenderFiscalIndex", "TenderFiscalIndex");
                                                bc.ColumnMappings.Add("TenderName", "TenderName");
                                                bc.ColumnMappings.Add("TenderCategory", "TenderCategory");
                                                bc.ColumnMappings.Add("Taxoption", "Taxoption");
                                                bc.ColumnMappings.Add("DefaultSkimLimit", "DefaultSkimLimit");
                                                bc.ColumnMappings.Add("DefaultHaloLimit", "DefaultHaloLimit");
                                                bc.ColumnMappings.Add("SubtotalOption", "SubtotalOption");
                                                bc.ColumnMappings.Add("CurrencyDecimals", "CurrencyDecimals");
                                                bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                bc.ColumnMappings.Add("TRX_TenderTable_Id", "TRX_TenderTable_Id");
                                                bc.WriteToServer(TenderType);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable TenderFlags = ds.Tables["TenderFlags"];
                                            if (TenderFlags != null)
                                            {
                                                TenderFlags.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TenderFlags.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TenderFlags.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TenderFlags";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                bc.ColumnMappings.Add("TenderFlags_Id", "TenderFlags_Id");
                                                bc.WriteToServer(TenderFlags);

                                            }

                                        }

                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Info = ds.Tables["Info"];
                                            if (Info != null)
                                            {
                                                Info.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Info.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Info.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Info";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("name", "name");
                                                bc.ColumnMappings.Add("value", "value");
                                                bc.ColumnMappings.Add("CustomInfo_Id", "CustomInfo_Id");
                                                bc.WriteToServer(Info);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con))
                                        {


                                            DataTable TenderFlag = ds.Tables["TenderFlag"];
                                            if (TenderFlag != null)
                                            {
                                                // TenderFlag.Columns.Add(new DataColumn()
                                                //  {
                                                //      ColumnName = "STLD_Location",
                                                //      DataType = typeof(string)
                                                //  });

                                                //  TenderFlag.Columns.Add(new DataColumn()
                                                //  {
                                                //      ColumnName = "business_date",
                                                //      DataType = typeof(string)
                                                //  });
                                                //foreach (DataRow vrow in TenderFlag.Rows)
                                                // {

                                                //     vrow["STLD_Location"] = lbl_location.Text;
                                                //     vrow["business_date"] = lbl_business.Text;

                                                // }
                                                //  bc.DestinationTableName = "TenderFlag";
                                                //  bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                //  bc.ColumnMappings.Add("business_date", "business_date");
                                                //   bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                //   bc.ColumnMappings.Add("TenderFlags_Id", "TenderFlags_Id");
                                                //   bc.WriteToServer(TenderFlag);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable TenderChange = ds.Tables["TenderChange"];
                                            if (TenderChange != null)
                                            {
                                                TenderChange.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TenderChange.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TenderChange.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TenderChange";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("id", "id");
                                                bc.ColumnMappings.Add("type", "type");
                                                bc.ColumnMappings.Add("roundToMinAmount", "roundToMinAmount");
                                                bc.ColumnMappings.Add("maxAllowed", "maxAllowed");
                                                bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                bc.WriteToServer(TenderChange);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable GiftCoupon = ds.Tables["GiftCoupon"];
                                            if (GiftCoupon != null)
                                            {
                                                GiftCoupon.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                GiftCoupon.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in GiftCoupon.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "GiftCoupon";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("LegacyId", "LegacyId");
                                                bc.ColumnMappings.Add("OperatorDefined", "OperatorDefined");
                                                bc.ColumnMappings.Add("Amount", "Amount");
                                                bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                bc.WriteToServer(GiftCoupon);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable OtherPayments = ds.Tables["OtherPayments"];
                                            if (OtherPayments != null)
                                            {
                                                //OtherPayments.Columns.Add(new DataColumn()
                                                //{
                                                //    ColumnName = "STLD_Location",
                                                //    DataType = typeof(string)
                                                //});

                                                //OtherPayments.Columns.Add(new DataColumn()
                                                //{
                                                //    ColumnName = "business_date",
                                                //    DataType = typeof(string)
                                                //});
                                                //foreach (DataRow vrow in OtherPayments.Rows)
                                                //{

                                                //    vrow["STLD_Location"] = lbl_location.Text;
                                                //    vrow["business_date"] = lbl_business.Text;

                                                //}
                                                //bc.DestinationTableName = "OtherPayments";
                                                //bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                //bc.ColumnMappings.Add("business_date", "business_date");
                                                //bc.ColumnMappings.Add("LegacyId", "LegacyId");
                                                //bc.ColumnMappings.Add("OperatorDefined", "OperatorDefined");
                                                //bc.ColumnMappings.Add("Amount", "Amount");
                                                //bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                //bc.WriteToServer(OtherPayments);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable ForeignCurrency = ds.Tables["ForeignCurrency"];
                                            if (ForeignCurrency != null)
                                            {
                                                ForeignCurrency.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                ForeignCurrency.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in ForeignCurrency.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "ForeignCurrency";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("LegacyId", "LegacyId");
                                                bc.ColumnMappings.Add("ExchangeRate", "ExchangeRate");
                                                bc.ColumnMappings.Add("Precision", "Precision");
                                                bc.ColumnMappings.Add("Rounding", "Rounding");
                                                bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                bc.WriteToServer(ForeignCurrency);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable ElectronicPayment = ds.Tables["ElectronicPayment"];
                                            if (ElectronicPayment != null)
                                            {
                                                ElectronicPayment.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                ElectronicPayment.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in ElectronicPayment.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "ElectronicPayment";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("LegacyId", "LegacyId");
                                                bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                bc.WriteToServer(ElectronicPayment);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable CreditSales = ds.Tables["CreditSales"];
                                            if (CreditSales != null)
                                            {
                                                CreditSales.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                CreditSales.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in CreditSales.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "CreditSales";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("LegacyId", "LegacyId");
                                                bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                bc.WriteToServer(CreditSales);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Trx_DayPart = ds.Tables["Trx_DayPart"];
                                            if (Trx_DayPart != null)
                                            {
                                                Trx_DayPart.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Trx_DayPart.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Trx_DayPart.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                //bc.DestinationTableName = "Trx_DayPart";
                                                //bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                //bc.ColumnMappings.Add("business_date", "business_date");
                                                //bc.ColumnMappings.Add("LegacyId", "LegacyId");
                                                //bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                //bc.WriteToServer(Trx_DayPart);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable TRX_BaseConfig = ds.Tables["TRX_BaseConfig"];
                                            if (TRX_BaseConfig != null)
                                            {
                                                TRX_BaseConfig.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TRX_BaseConfig.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TRX_BaseConfig.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TRX_BaseConfig";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("TRX_BaseConfig_Id", "TRX_BaseConfig_Id");
                                                bc.ColumnMappings.Add("POS", "POS");
                                                bc.ColumnMappings.Add("POD", "POD");
                                                bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                bc.WriteToServer(TRX_BaseConfig);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Config = ds.Tables["Config"];
                                            if (Config != null)
                                            {
                                                Config.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Config.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Config.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Config";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("MenuPriceBasis", "MenuPriceBasis");
                                                bc.ColumnMappings.Add("WeekEndBreakfastStartTime", "WeekEndBreakfastStartTime");
                                                bc.ColumnMappings.Add("WeekEndBreakfastStopTime", "WeekEndBreakfastStopTime");
                                                bc.ColumnMappings.Add("WeekDayBreakfastStartTime", "WeekDayBreakfastStartTime");
                                                bc.ColumnMappings.Add("WeekDayBreakfastStopTime", "WeekDayBreakfastStopTime");
                                                bc.ColumnMappings.Add("DecimalPlaces", "DecimalPlaces");
                                                bc.ColumnMappings.Add("CheckRefund", "CheckRefund");
                                                bc.ColumnMappings.Add("GrandTotalFlag", "GrandTotalFlag");
                                                bc.ColumnMappings.Add("StoreId", "StoreId");
                                                bc.ColumnMappings.Add("StoreName", "StoreName");
                                                bc.ColumnMappings.Add("AcceptNegativeQty", "AcceptNegativeQty");
                                                bc.ColumnMappings.Add("AcceptZeroPricePMix", "AcceptZeroPricePMix");
                                                bc.ColumnMappings.Add("FloatPriceTenderId", "FloatPriceTenderId");
                                                bc.ColumnMappings.Add("MinCirculatingAmount", "MinCirculatingAmount");
                                                bc.ColumnMappings.Add("TRX_BaseConfig_Id", "TRX_BaseConfig_Id");
                                                bc.WriteToServer(Config);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable POSConfig = ds.Tables["POSConfig"];
                                            if (POSConfig != null)
                                            {
                                                POSConfig.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                POSConfig.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in POSConfig.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "POSConfig";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("CountTCsFullDiscEM", "CountTCsFullDiscEM");
                                                bc.ColumnMappings.Add("RefundBehaviour", "RefundBehaviour");
                                                bc.ColumnMappings.Add("OverringBehaviour", "OverringBehaviour");
                                                bc.ColumnMappings.Add("TRX_BaseConfig_Id", "TRX_BaseConfig_Id");
                                                bc.WriteToServer(POSConfig);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable TRX_SetSMState = ds.Tables["TRX_SetSMState"];
                                            if (TRX_SetSMState != null)
                                            {
                                                TRX_SetSMState.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TRX_SetSMState.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TRX_SetSMState.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TRX_SetSMState";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("POSState", "POSState");
                                                bc.ColumnMappings.Add("CrewId", "CrewId");
                                                bc.ColumnMappings.Add("CrewName", "CrewName");
                                                bc.ColumnMappings.Add("CrewSecurityLevel", "CrewSecurityLevel");
                                                bc.ColumnMappings.Add("LoginTime", "LoginTime");
                                                bc.ColumnMappings.Add("LogoutTime", "LogoutTime");
                                                bc.ColumnMappings.Add("InitialFloat", "InitialFloat");
                                                bc.ColumnMappings.Add("PODId", "PODId");
                                                bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                bc.WriteToServer(TRX_SetSMState);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable TRX_InitGTotal = ds.Tables["TRX_InitGTotal"];
                                            if (TRX_InitGTotal != null)
                                            {
                                                TRX_InitGTotal.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TRX_InitGTotal.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TRX_InitGTotal.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TRX_InitGTotal";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("amount", "amount");
                                                bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                bc.WriteToServer(TRX_InitGTotal);

                                            }

                                        }



                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {
                                            DataTable TLD = ds.Tables["TLD"];
                                            bc.DestinationTableName = "TLD";
                                            bc.ColumnMappings.Add("TLD_Id", "TLD_Id");
                                            bc.ColumnMappings.Add("LogVersion", "LogVersion");
                                            bc.ColumnMappings.Add("storeId", "storeId");
                                            bc.ColumnMappings.Add("businessDate", "businessDate");
                                            bc.ColumnMappings.Add("swVersion", "swVersion");
                                            bc.ColumnMappings.Add("checkPoint", "checkPoint");
                                            bc.ColumnMappings.Add("end", "end");
                                            bc.ColumnMappings.Add("productionStatus", "productionStatus");
                                            bc.ColumnMappings.Add("hasMoreContent", "hasMoreContent");
                                            bc.WriteToServer(TLD);

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {
                                            DataTable Node = ds.Tables["Node"];
                                            Node.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "STLD_Location",
                                                DataType = typeof(string)
                                            });

                                            Node.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "business_date",
                                                DataType = typeof(string)
                                            });
                                            foreach (DataRow vrow in Node.Rows)
                                            {

                                                vrow["STLD_Location"] = lbl_location.Text;
                                                vrow["business_date"] = lbl_business.Text;

                                            }
                                            bc.DestinationTableName = "Node";
                                            bc.ColumnMappings.Add("Node_Id", "Node_Id");
                                            bc.ColumnMappings.Add("TLD_Id", "TLD_Id");
                                            bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            bc.ColumnMappings.Add("business_date", "business_date");
                                            bc.ColumnMappings.Add("nodeStatus", "nodeStatus");
                                            bc.ColumnMappings.Add("id", "id");
                                            bc.WriteToServer(Node);

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {
                                            DataTable Event = ds.Tables["Event"];
                                            Event.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "STLD_Location",
                                                DataType = typeof(string)
                                            });

                                            Event.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "business_date",
                                                DataType = typeof(string)
                                            });
                                            foreach (DataRow vrow in Event.Rows)
                                            {

                                                vrow["STLD_Location"] = lbl_location.Text;
                                                vrow["business_date"] = lbl_business.Text;

                                            }
                                            bc.DestinationTableName = "Event";
                                            bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            bc.ColumnMappings.Add("business_date", "business_date");
                                            bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                            //  bc.ColumnMappings.Add("TRX_UnaDrawerOpening", "TRX_UnaDrawerOpening");
                                            bc.ColumnMappings.Add("RegId", "RegId");
                                            bc.ColumnMappings.Add("Type", "Type");
                                            bc.ColumnMappings.Add("Time", "Time");
                                            bc.ColumnMappings.Add("Node_Id", "Node_Id");
                                            bc.WriteToServer(Event);

                                        }





                                        string smt = "INSERT INTO [STLD].[dbo].[STLDPROCESS_STATUS] ([STLD_Location] ,[business_date]) VALUES (@STLD_Location,@business_date)";
                                        SqlCommand cmd = new SqlCommand(smt, con, tran);
                                        cmd.Parameters.AddWithValue("@STLD_Location", SqlDbType.VarChar).Value = lbl_location.Text;
                                        cmd.Parameters.AddWithValue("@business_date", SqlDbType.VarChar).Value = lbl_business.Text;
                                        cmd.ExecuteNonQuery();
                                        File.Move(file, Path.ChangeExtension(file, ".Proceed"));

                                        //////////////////////

                                        tran.Commit();


                                    }
                                    catch
                                    {
                                        tran.Rollback();
                                        // throw;
                                    }

                                }

                            }









                        }
                    }

                    //////////////////////////////////////////////////////////////////

                }
                else
                {
                    //File.Delete(file);
                }


            }


        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void btn_unzip_Click(object sender, EventArgs e)
        {
            BTN_CLEARLBLES.PerformClick();
            //foreach (string file in Directory.GetFiles("D:\\ImpotSheiV2\\"))
            //    {

            //        File.Move(file, Path.ChangeExtension(file, ".zip"));
            //    }

            foreach (string file99 in Directory.GetFiles(@"\\192.168.1.200\stld", "*.zip"))


            {
                //string destination = "D:\\ImpotSheiV2\\OLDOriginal";

                foreach (string file2 in Directory.GetFiles(@"\\192.168.1.200\stld", "*.zip"))


                {
                    //string destination = "D:\\ImpotSheiV2\\OLDOriginal";
                    FileInfo source = new FileInfo(file2);
                    //File.Delete(file2, "D:\\ImpotSheiV2\\Extract");



                    try
                    {
                        ZipFile.ExtractToDirectory(file2, "E:\\ImpotSheiV2\\Extract");
                        //source.CopyTo(destination, true);
                        //File.Copy(file2., destination);
                        //File.Copy(file2, destination, true);
                        File.Delete(file2);
                    }
                    catch
                    {
                        continue;
                        File.Delete(file2);
                    }






                    ///////////////////////////////////


                }
                foreach (string file20 in Directory.GetFiles("E:\\ImpotSheiV2\\Extract\\"))
                {

                    File.Move(file20, Path.ChangeExtension(file20, ".zip"));
                }

                foreach (string file11 in Directory.GetFiles("E:\\ImpotSheiV2\\Extract", "*.zip"))
                {
                    try
                    {
                        ZipFile.ExtractToDirectory(file11, "E:\\ImpotSheiV2\\Extract\\Processingfiles");
                        File.Delete(file11);
                    }
                    catch
                    {
                        File.Delete(file11);
                    }




                }

                foreach (string file10 in Directory.GetFiles("E:\\ImpotSheiV2\\Extract", "*.log"))
                {
                    File.Delete(file10);
                }

                foreach (string file11 in Directory.GetFiles("E:\\ImpotSheiV2\\Extract\\Processingfiles", "*.zip"))
                {
                    ZipFile.ExtractToDirectory(file11, "E:\\ImpotSheiV2\\Extract\\Processingfiles");
                    File.Delete(file11);


                }

                foreach (string file12 in Directory.GetFiles("E:\\ImpotSheiV2\\Extract\\Processingfiles", "*.log"))
                {
                    // ZipFile.ExtractToDirectory(file12, "D:\\ImpotSheiV2\\Extract\\Processingfiles");
                    File.Delete(file12);


                }

                foreach (string file5 in Directory.GetFiles("E:\\ImpotSheiV2\\Extract\\Processingfiles", "*.log"))
                {
                    File.Delete(file5);

                }
                foreach (string file6 in Directory.GetFiles("E:\\ImpotSheiV2\\Extract\\", "*.log"))
                {
                    File.Delete(file6);

                }


                continue;
            }

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            {
                //ZipFile.ExtractToDirectory(openFileDialog1.FileName, folderBrowserDialog1.SelectedPath);
                //MessageBox.Show("ZIP file extracted successfully!");
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (string file10 in Directory.GetFiles("E:\\ImpotSheiV2\\Extract\\Processingfiles", "*.xml"))
            {
                File.Delete(file10);
            }
            foreach (string file589 in Directory.GetFiles("E:\\ImpotSheiV2\\Extract\\Pmixprocess", "*.xml"))
            {
                File.Delete(file589);
            }

        }


        private void BTN_OTEHR_Click(object sender, EventArgs e)
        {

        }
        private void button4_Click(object sender, EventArgs e)
        {
            // shehan
            
            BTN_CLEARLBLES.PerformClick();
            BTN_PMIX.PerformClick();
            button5.PerformClick();
            //foreach (string file in Directory.EnumerateFiles("E:\\ImpotSheiV2\\Extract\\Processingfiles", "*.xml"))
            //{


            //    string contents = File.ReadAllText(file);
            //    ///////////////////// File Reading/////////////////////////

            //    lbl_nameval.Text = Convert.ToString(file.ToString());
            //    string tempfile = Convert.ToString(file.ToString());
            //    if (lbl_nameval.Text.Contains("STLD"))
            //    {



            //        string connString = SQLCON.ConnectionString2;
            //        lbl_STLD.Text = file;

            //        SQLCON vb = new SQLCON();
            //        vb.OpenConnection();

            //        DataSet ds = new DataSet();
            //        try
            //        {
            //            ds.ReadXml(lbl_STLD.Text);

            //            DataTable CHKTLD = ds.Tables["TLD"];
            //            foreach (DataRow row in CHKTLD.Rows)
            //            {
            //                // ... Write value of first field as integer.
            //                string temploca = file.Substring(0, 57);
            //                string tempdate = file.Substring(0, 66);
            //                string storloc = temploca.Substring(temploca.Length - 6, 6);
            //                string storbusinessdate = tempdate.Substring(tempdate.Length - 8, 8);

            //                lbl_location.Text = storloc;
            //                lbl_business.Text = storbusinessdate;
            //            }

            //            using (SqlConnection con1 = new SqlConnection(connString))
            //            {

            //                con1.Open();

            //                SqlCommand comm = new SqlCommand("SELECT COUNT(*) FROM [STLDPROCESS_STATUS] where [STLD_Location]=@STLD_Location and business_date=@business_date", con1);
            //                comm.Parameters.AddWithValue("@STLD_Location", SqlDbType.VarChar).Value = lbl_location.Text;
            //                comm.Parameters.AddWithValue("@business_date", SqlDbType.VarChar).Value = lbl_business.Text;
            //                Int32 count = Convert.ToInt32(comm.ExecuteScalar());
            //                if (count > 0)
            //                {
            //                    lblCount.Text = Convert.ToString(count.ToString()); //For example a Label
            //                }
            //                else
            //                {
            //                    if (lbl_location.Text.Contains("online"))
            //                    {
            //                        lblCount.Text = "wrong";
            //                    }
            //                    else
            //                    {
            //                        lblCount.Text = "0";


            //                        SqlCommand comm2 = new SqlCommand("SELECT [StoreID]   FROM [STLD].[dbo].[TB_STOREMASTER] where [StoreID]=@STLD_Location", con1);
            //                        comm2.Parameters.AddWithValue("@STLD_Location", SqlDbType.VarChar).Value = lbl_location.Text;
            //                        Int32 count2 = Convert.ToInt32(comm2.ExecuteScalar());

            //                        if (count2 > 0)
            //                        {
            //                            lblCount.Text = "0"; //For example a Label
            //                        }
            //                        else
            //                        {
            //                            lblCount.Text = "wrong";
            //                        }
            //                    }
            //                }
            //                con1.Close();

            //            }


            //            using (SqlConnection con = new SqlConnection(connString))
            //            {
            //                con.Open();

            //                if (lblCount.Text.Contains('0'))
            //                {

            //                    using (var connection = con)
            //                    {
            //                        using (var tran = con.BeginTransaction())
            //                        {


            //                            try
            //                            {




            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {

            //                                    DataTable TLD = ds.Tables["TLD"];
            //                                    foreach (DataRow row in TLD.Rows)
            //                                    {
            //                                        // ... Write value of first field as integer.
            //                                        // string storloc = row.ItemArray[2].ToString();
            //                                        // string storbusinessdate = row.ItemArray[3].ToString();

            //                                        //lbl_location.Text = storloc;
            //                                        //lbl_business.Text = storbusinessdate;
            //                                    }





            //                                    DataTable TRX_Sale = ds.Tables["TRX_Sale"];

            //                                    TRX_Sale.Columns.Add(new DataColumn()
            //                                    {
            //                                        ColumnName = "STLD_Location",
            //                                        DataType = typeof(string)
            //                                    });

            //                                    TRX_Sale.Columns.Add(new DataColumn()
            //                                    {
            //                                        ColumnName = "business_date",
            //                                        DataType = typeof(string)
            //                                    });
            //                                    foreach (DataRow vrow in TRX_Sale.Rows)
            //                                    {

            //                                        vrow["STLD_Location"] = lbl_location.Text;
            //                                        vrow["business_date"] = lbl_business.Text;

            //                                    }



            //                                    bc.DestinationTableName = "TRX_Sale";
            //                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                    bc.ColumnMappings.Add("business_date", "business_date");
            //                                    bc.ColumnMappings.Add("TRX_Sale_Id", "TRX_Sale_Id");
            //                                    bc.ColumnMappings.Add("status", "status");
            //                                    bc.ColumnMappings.Add("POD", "POD");
            //                                    bc.ColumnMappings.Add("RemPOD", "RemPOD");
            //                                    bc.ColumnMappings.Add("Event_Id", "Event_Id");
            //                                    bc.WriteToServer(TRX_Sale);

            //                                }




            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {

            //                                    //------- Order Table////////////////
            //                                    DataTable Order_TB = ds.Tables["Order"];
            //                                    Order_TB.Columns.Add(new DataColumn()
            //                                    {
            //                                        ColumnName = "STLD_Location",
            //                                        DataType = typeof(string)
            //                                    });

            //                                    Order_TB.Columns.Add(new DataColumn()
            //                                    {
            //                                        ColumnName = "business_date",
            //                                        DataType = typeof(string)
            //                                    });
            //                                    foreach (DataRow vrow in Order_TB.Rows)
            //                                    {

            //                                        vrow["STLD_Location"] = lbl_location.Text;
            //                                        vrow["business_date"] = lbl_business.Text;

            //                                    }
            //                                    bc.DestinationTableName = "Order_TB";
            //                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                    bc.ColumnMappings.Add("business_date", "business_date");
            //                                    bc.ColumnMappings.Add("Order_Id", "Order_Id");
            //                                    bc.ColumnMappings.Add("Timestamp", "Timestamp");
            //                                    bc.ColumnMappings.Add("uniqueId", "uniqueId");
            //                                    bc.ColumnMappings.Add("kind", "kind");
            //                                    bc.ColumnMappings.Add("key", "key");
            //                                    bc.ColumnMappings.Add("major", "major");
            //                                    bc.ColumnMappings.Add("minor", "minor");
            //                                    bc.ColumnMappings.Add("side", "side");
            //                                    bc.ColumnMappings.Add("receiptNumber", "receiptNumber");
            //                                    bc.ColumnMappings.Add("fpReceiptNumber", "fpReceiptNumber");
            //                                    //bc.ColumnMappings.Add("boot", "boot");
            //                                    bc.ColumnMappings.Add("saleType", "saleType");
            //                                    bc.ColumnMappings.Add("totalAmount", "totalAmount");
            //                                    bc.ColumnMappings.Add("nonProductAmount", "nonProductAmount");
            //                                    bc.ColumnMappings.Add("totalTax", "totalTax");
            //                                    bc.ColumnMappings.Add("nonProductTax", "nonProductTax");
            //                                    bc.ColumnMappings.Add("orderSrc", "orderSrc");
            //                                    bc.ColumnMappings.Add("startSaleDate", "startSaleDate");
            //                                    bc.ColumnMappings.Add("startSaleTime", "startSaleTime");
            //                                    bc.ColumnMappings.Add("endSaleDate", "endSaleDate");
            //                                    bc.ColumnMappings.Add("endSaleTime", "endSaleTime");
            //                                    bc.ColumnMappings.Add("TRX_Sale_Id", "TRX_Sale_Id");
            //                                    bc.WriteToServer(Order_TB);

            //                                    //----------------------------------



            //                                }

            //                                //------refund
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con))
            //                                {
            //                                    // DataTable TRX_Refund = ds.Tables["TRX_Refund"];

            //                                    // TRX_Refund.Columns.Add(new DataColumn()
            //                                    // {
            //                                    //     ColumnName = "STLD_Location",
            //                                    //     DataType = typeof(string)
            //                                    // });

            //                                    // TRX_Refund.Columns.Add(new DataColumn()
            //                                    // {
            //                                    //     ColumnName = "business_date",
            //                                    //     DataType = typeof(string)
            //                                    // });
            //                                    // foreach (DataRow vrow in TRX_Refund.Rows)
            //                                    // {

            //                                    //     vrow["STLD_Location"] = lbl_location.Text;
            //                                    //     vrow["business_date"] = lbl_business.Text;

            //                                    // }



            //                                    // bc.DestinationTableName = "TRX_Refund";
            //                                    // bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                    // bc.ColumnMappings.Add("business_date", "business_date");
            //                                    // bc.ColumnMappings.Add("TRX_Refund_Id", "TRX_Refund_Id");
            //                                    //bc.ColumnMappings.Add("status", "status");
            //                                    // bc.ColumnMappings.Add("POD", "POD");
            //                                    // bc.ColumnMappings.Add("RemPOD", "RemPOD");
            //                                    // bc.ColumnMappings.Add("Event_Id", "Event_Id");
            //                                    // bc.WriteToServer(TRX_Refund);

            //                                }



            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {

            //                                    //------- Item Table////////////////
            //                                    DataTable Item_TB = ds.Tables["Item"];
            //                                    Item_TB.Columns.Add(new DataColumn()
            //                                    {
            //                                        ColumnName = "STLD_Location",
            //                                        DataType = typeof(string)
            //                                    });

            //                                    Item_TB.Columns.Add(new DataColumn()
            //                                    {
            //                                        ColumnName = "business_date",
            //                                        DataType = typeof(string)
            //                                    });
            //                                    foreach (DataRow vrow in Item_TB.Rows)
            //                                    {

            //                                        vrow["STLD_Location"] = lbl_location.Text;
            //                                        vrow["business_date"] = lbl_business.Text;

            //                                    }

            //                                    bc.DestinationTableName = "Item_TB";
            //                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                    bc.ColumnMappings.Add("business_date", "business_date");
            //                                    bc.ColumnMappings.Add("Item_Id", "Item_Id");
            //                                    bc.ColumnMappings.Add("code", "code");
            //                                    bc.ColumnMappings.Add("type", "type");
            //                                    bc.ColumnMappings.Add("action", "action");
            //                                    bc.ColumnMappings.Add("level", "level");
            //                                    bc.ColumnMappings.Add("id", "id");
            //                                    bc.ColumnMappings.Add("displayOrder", "displayOrder");
            //                                    bc.ColumnMappings.Add("qty", "qty");
            //                                    bc.ColumnMappings.Add("grillQty", "grillQty");
            //                                    bc.ColumnMappings.Add("grillModifier", "grillModifier");
            //                                    bc.ColumnMappings.Add("qtyPromo", "qtyPromo");
            //                                    bc.ColumnMappings.Add("chgAfterTotal", "chgAfterTotal");
            //                                    bc.ColumnMappings.Add("BPPrice", "BPPrice");
            //                                    bc.ColumnMappings.Add("BPTax", "BPTax");
            //                                    bc.ColumnMappings.Add("BDPrice", "BDPrice");
            //                                    bc.ColumnMappings.Add("BDTax", "BDTax");
            //                                    bc.ColumnMappings.Add("totalPrice", "totalPrice");
            //                                    bc.ColumnMappings.Add("totalTax", "totalTax");
            //                                    bc.ColumnMappings.Add("category", "category");
            //                                    bc.ColumnMappings.Add("familyGroup", "familyGroup");
            //                                    bc.ColumnMappings.Add("daypart", "daypart");
            //                                    bc.ColumnMappings.Add("description", "description");
            //                                    bc.ColumnMappings.Add("department", "department");
            //                                    bc.ColumnMappings.Add("departmentClass", "departmentClass");
            //                                    bc.ColumnMappings.Add("departmentSubClass", "departmentSubClass");
            //                                    bc.ColumnMappings.Add("unitPrice", "unitPrice");
            //                                    bc.ColumnMappings.Add("unitTax", "unitTax");
            //                                    bc.ColumnMappings.Add("solvedChoice", "solvedChoice");
            //                                    bc.ColumnMappings.Add("isUpcharge", "isUpcharge");
            //                                    bc.ColumnMappings.Add("Item_Id_0", "Item_Id_0");
            //                                    bc.ColumnMappings.Add("Order_Id", "Order_Id");
            //                                    bc.WriteToServer(Item_TB);


            //                                    ////////////////////////////////////

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {
            //                                    ///-------------- Promo table-----------------
            //                                    DataTable TRX_Overring = ds.Tables["TRX_Overring"];
            //                                    if (TRX_Overring != null)
            //                                    {
            //                                        TRX_Overring.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        TRX_Overring.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in TRX_Overring.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "TRX_Overring";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("TRX_Overring_Id", "TRX_Overring_Id");
            //                                        bc.ColumnMappings.Add("POD", "POD");
            //                                        bc.ColumnMappings.Add("Event_Id", "Event_Id");
            //                                        bc.WriteToServer(TRX_Overring);
            //                                    }
            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {
            //                                    ///-------------- Promo table-----------------
            //                                    DataTable Product = ds.Tables["Product"];
            //                                    if (Product != null)
            //                                    {
            //                                        Product.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        Product.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in Product.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "Product";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("code", "code");
            //                                        bc.ColumnMappings.Add("quantity", "quantity");
            //                                        bc.ColumnMappings.Add("Components_Id", "Components_Id");
            //                                        bc.WriteToServer(Product);
            //                                    }
            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {
            //                                    ///-------------- Promo table-----------------
            //                                    DataTable Components = ds.Tables["Components"];
            //                                    if (Components != null)
            //                                    {
            //                                        Components.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        Components.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in Components.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "Components";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("Components_Id", "Components_Id");
            //                                        bc.ColumnMappings.Add("Ev_BreakValueMeal_Id", "Ev_BreakValueMeal_Id");
            //                                        bc.WriteToServer(Components);
            //                                    }
            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {
            //                                    ///-------------- Promo table-----------------
            //                                    DataTable Ev_BreakValueMeal = ds.Tables["Ev_BreakValueMeal"];
            //                                    if (Ev_BreakValueMeal != null)
            //                                    {
            //                                        Ev_BreakValueMeal.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        Ev_BreakValueMeal.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in Ev_BreakValueMeal.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "Ev_BreakValueMeal";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("ProductCode", "ProductCode");
            //                                        bc.ColumnMappings.Add("Quantity", "Quantity");
            //                                        bc.ColumnMappings.Add("Ev_BreakValueMeal_Id", "Ev_BreakValueMeal_Id");
            //                                        bc.WriteToServer(Ev_BreakValueMeal);
            //                                    }
            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {
            //                                    ///-------------- Promo table-----------------
            //                                    DataTable Promo = ds.Tables["Promo"];
            //                                    if (Promo != null)
            //                                    {
            //                                        Promo.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        Promo.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in Promo.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }


            //                                        bc.DestinationTableName = "Promo";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("id", "id");
            //                                        bc.ColumnMappings.Add("name", "name");
            //                                        bc.ColumnMappings.Add("qty", "qty");
            //                                        bc.ColumnMappings.Add("Item_id", "Item_id");
            //                                        bc.WriteToServer(Promo);
            //                                    }
            //                                    //------------------------------------------
            //                                }

            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con))
            //                                {
            //                                    ///////------------------------
            //                                    // DataTable PromotionApplied = ds.Tables["PromotionApplied"];
            //                                    //  if (PromotionApplied != null)
            //                                    //  {
            //                                    //     PromotionApplied.Columns.Add(new DataColumn()
            //                                    //    {
            //                                    //        ColumnName = "STLD_Location",
            //                                    //        DataType = typeof(string)
            //                                    //    });

            //                                    //    PromotionApplied.Columns.Add(new DataColumn()
            //                                    //    {
            //                                    //        ColumnName = "business_date",
            //                                    //        DataType = typeof(string)
            //                                    //    });
            //                                    //    foreach (DataRow vrow in PromotionApplied.Rows)
            //                                    //    {

            //                                    //        vrow["STLD_Location"] = lbl_location.Text;
            //                                    //        vrow["business_date"] = lbl_business.Text;

            //                                    //    }
            //                                    //    bc.DestinationTableName = "PromotionApplied";
            //                                    //    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                    //    bc.ColumnMappings.Add("business_date", "business_date");
            //                                    //    bc.ColumnMappings.Add("promotionId", "promotionId");
            //                                    //    bc.ColumnMappings.Add("promotionCounter", "promotionCounter");
            //                                    //    bc.ColumnMappings.Add("eligible", "eligible");
            //                                    //    bc.ColumnMappings.Add("originalPrice", "originalPrice");
            //                                    //    bc.ColumnMappings.Add("discountAmount", "discountAmount");
            //                                    //    bc.ColumnMappings.Add("discountType", "discountType");
            //                                    //    bc.ColumnMappings.Add("originalItemPromoQty", "originalItemPromoQty");
            //                                    //    bc.ColumnMappings.Add("originalProductCode", "originalProductCode");
            //                                    //    bc.ColumnMappings.Add("offerId", "offerId");
            //                                    //    bc.ColumnMappings.Add("Item_Id", "Item_Id");
            //                                    //    bc.WriteToServer(PromotionApplied);
            //                                    //    ///////////////////////

            //                                    //}
            //                                }

            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {
            //                                    ////////// Offer ////// Table
            //                                    DataTable Offers = ds.Tables["Offers"];

            //                                    if (Offers != null)
            //                                    {

            //                                        Offers.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        Offers.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in Offers.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "Offers";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("offerId", "offerId");

            //                                        // bc.ColumnMappings.Add("beforeOfferPrice", "beforeOfferPrice");
            //                                        // bc.ColumnMappings.Add("discountAmount", "discountAmount");
            //                                        //  bc.ColumnMappings.Add("discountType", "discountType");
            //                                        // bc.ColumnMappings.Add("Item_Id", "Item_Id");
            //                                        bc.ColumnMappings.Add("customerId", "customerId");
            //                                        bc.ColumnMappings.Add("offerName", "offerName");
            //                                        bc.ColumnMappings.Add("override", "override");
            //                                        bc.ColumnMappings.Add("applied", "applied");
            //                                        bc.ColumnMappings.Add("promotionId", "promotionId");
            //                                        bc.ColumnMappings.Add("offerBarcodeType", "offerBarcodeType");
            //                                        bc.ColumnMappings.Add("Order_Id", "Order_Id");
            //                                        bc.WriteToServer(Offers);
            //                                    }
            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {
            //                                    DataTable TaxChain = ds.Tables["TaxChain"];

            //                                    TaxChain.Columns.Add(new DataColumn()
            //                                    {
            //                                        ColumnName = "STLD_Location",
            //                                        DataType = typeof(string)
            //                                    });

            //                                    TaxChain.Columns.Add(new DataColumn()
            //                                    {
            //                                        ColumnName = "business_date",
            //                                        DataType = typeof(string)
            //                                    });
            //                                    foreach (DataRow vrow in TaxChain.Rows)
            //                                    {

            //                                        vrow["STLD_Location"] = lbl_location.Text;
            //                                        vrow["business_date"] = lbl_business.Text;

            //                                    }
            //                                    bc.DestinationTableName = "TaxChain";
            //                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                    bc.ColumnMappings.Add("business_date", "business_date");
            //                                    bc.ColumnMappings.Add("id", "id");
            //                                    bc.ColumnMappings.Add("name", "name");
            //                                    bc.ColumnMappings.Add("rate", "rate");
            //                                    bc.ColumnMappings.Add("baseAmount", "baseAmount");
            //                                    bc.ColumnMappings.Add("amount", "amount");
            //                                    bc.ColumnMappings.Add("BDBaseAmount", "BDBaseAmount");
            //                                    bc.ColumnMappings.Add("BDAmount", "BDAmount");
            //                                    bc.ColumnMappings.Add("Item_Id", "Item_Id");
            //                                    // bc.ColumnMappings.Add("BPBaseAmount", "BPBaseAmount");
            //                                    //bc.ColumnMappings.Add("BPAmount", "BPAmount");
            //                                    bc.ColumnMappings.Add("Order_Id", "Order_Id");
            //                                    bc.WriteToServer(TaxChain);

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {

            //                                    DataTable Promotions = ds.Tables["Promotions"];
            //                                    if (Promotions != null)
            //                                    {
            //                                        Promotions.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        Promotions.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in Promotions.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }

            //                                        bc.DestinationTableName = "Promotions";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("Promotions_Id", "Promotions_Id");
            //                                        bc.ColumnMappings.Add("Order_Id", "Order_Id");
            //                                        bc.WriteToServer(Promotions);
            //                                    }
            //                                }


            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {

            //                                    DataTable Promotion = ds.Tables["Promotion"];
            //                                    if (Promotion != null)
            //                                    {


            //                                        Promotion.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        Promotion.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in Promotion.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }

            //                                        bc.DestinationTableName = "Promotion";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("promotionId", "promotionId");
            //                                        bc.ColumnMappings.Add("promotionName", "promotionName");
            //                                        bc.ColumnMappings.Add("promotionCounter", "promotionCounter");
            //                                        bc.ColumnMappings.Add("discountType", "discountType");
            //                                        bc.ColumnMappings.Add("discountAmount", "discountAmount");
            //                                        bc.ColumnMappings.Add("offerId", "offerId");
            //                                        bc.ColumnMappings.Add("exclusive", "exclusive");
            //                                        bc.ColumnMappings.Add("promotionOnTender", "promotionOnTender");
            //                                        bc.ColumnMappings.Add("countTowardsPromotionLimit", "countTowardsPromotionLimit");
            //                                        bc.ColumnMappings.Add("returnedValue", "returnedValue");
            //                                        bc.ColumnMappings.Add("Promotions_Id", "Promotions_Id");
            //                                        bc.WriteToServer(Promotion);

            //                                    }
            //                                }


            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {

            //                                    DataTable Customer = ds.Tables["Customer"];
            //                                    if (Customer != null)
            //                                    {
            //                                        Customer.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        Customer.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in Customer.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }


            //                                        bc.DestinationTableName = "Customer";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("id", "id");
            //                                        bc.ColumnMappings.Add("nickname", "nickname");
            //                                        bc.ColumnMappings.Add("greeting", "greeting");
            //                                        bc.ColumnMappings.Add("loyaltyCardId", "loyaltyCardId");
            //                                        bc.ColumnMappings.Add("loyaltyCardType", "loyaltyCardType");
            //                                        bc.ColumnMappings.Add("Order_Id", "Order_Id");
            //                                        bc.WriteToServer(Customer);
            //                                    }
            //                                }


            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {

            //                                    DataTable CustomInfo = ds.Tables["CustomInfo"];
            //                                    if (CustomInfo != null)
            //                                    {
            //                                        CustomInfo.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        CustomInfo.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in CustomInfo.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "CustomInfo";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("CustomInfo_Id", "CustomInfo_Id");
            //                                        bc.ColumnMappings.Add("Order_Id", "Order_Id");
            //                                        bc.WriteToServer(CustomInfo);


            //                                    }
            //                                }


            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {

            //                                    DataTable Tenders = ds.Tables["Tenders"];
            //                                    Tenders.Columns.Add(new DataColumn()
            //                                    {
            //                                        ColumnName = "STLD_Location",
            //                                        DataType = typeof(string)
            //                                    });

            //                                    Tenders.Columns.Add(new DataColumn()
            //                                    {
            //                                        ColumnName = "business_date",
            //                                        DataType = typeof(string)
            //                                    });
            //                                    foreach (DataRow vrow in Tenders.Rows)
            //                                    {

            //                                        vrow["STLD_Location"] = lbl_location.Text;
            //                                        vrow["business_date"] = lbl_business.Text;

            //                                    }
            //                                    bc.DestinationTableName = "Tenders";
            //                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                    bc.ColumnMappings.Add("business_date", "business_date");
            //                                    bc.ColumnMappings.Add("Tenders_Id", "Tenders_Id");
            //                                    bc.ColumnMappings.Add("Order_Id", "Order_Id");
            //                                    bc.WriteToServer(Tenders);
            //                                }

            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {

            //                                    DataTable Tender = ds.Tables["Tender"];
            //                                    Tender.Columns.Add(new DataColumn()
            //                                    {
            //                                        ColumnName = "STLD_Location",
            //                                        DataType = typeof(string)
            //                                    });

            //                                    Tender.Columns.Add(new DataColumn()
            //                                    {
            //                                        ColumnName = "business_date",
            //                                        DataType = typeof(string)
            //                                    });
            //                                    foreach (DataRow vrow in Tender.Rows)
            //                                    {

            //                                        vrow["STLD_Location"] = lbl_location.Text;
            //                                        vrow["business_date"] = lbl_business.Text;

            //                                    }
            //                                    bc.DestinationTableName = "Tender";
            //                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                    bc.ColumnMappings.Add("business_date", "business_date");
            //                                    bc.ColumnMappings.Add("TenderId", "TenderId");
            //                                    bc.ColumnMappings.Add("TenderKind", "TenderKind");
            //                                    bc.ColumnMappings.Add("TenderName", "TenderName");
            //                                    bc.ColumnMappings.Add("TenderQuantity", "TenderQuantity");
            //                                    bc.ColumnMappings.Add("FaceValue", "FaceValue");
            //                                    bc.ColumnMappings.Add("TenderAmount", "TenderAmount");
            //                                    bc.ColumnMappings.Add("BaseAction", "BaseAction");
            //                                    bc.ColumnMappings.Add("Persisted", "Persisted");
            //                                    bc.ColumnMappings.Add("CardProviderID", "CardProviderID");
            //                                    bc.ColumnMappings.Add("CashlessData", "CashlessData");
            //                                    bc.ColumnMappings.Add("TaxOption", "TaxOption");
            //                                    bc.ColumnMappings.Add("SubtotalOption", "SubtotalOption");
            //                                    bc.ColumnMappings.Add("ForeignCurrencyIndicator", "ForeignCurrencyIndicator");
            //                                    bc.ColumnMappings.Add("DiscountDescription", "DiscountDescription");
            //                                    bc.ColumnMappings.Add("CashlessTransactionID", "CashlessTransactionID");
            //                                    bc.ColumnMappings.Add("PaymentChannel", "PaymentChannel");
            //                                    bc.ColumnMappings.Add("Tenders_Id", "Tenders_Id");
            //                                    bc.WriteToServer(Tender);


            //                                }

            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable POSTiming = ds.Tables["POSTiming"];
            //                                    if (POSTiming != null)
            //                                    {
            //                                        POSTiming.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        POSTiming.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in POSTiming.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }

            //                                    }


            //                                }

            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable EventsDetail = ds.Tables["EventsDetail"];
            //                                    if (EventsDetail != null)
            //                                    {
            //                                        EventsDetail.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        EventsDetail.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in EventsDetail.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "EventsDetail";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("EventsDetail_Id", "EventsDetail_Id");
            //                                        bc.ColumnMappings.Add("Order_Id", "Order_Id");
            //                                        bc.WriteToServer(EventsDetail);

            //                                    }


            //                                }

            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable SaleEvent = ds.Tables["SaleEvent"];
            //                                    if (SaleEvent != null)
            //                                    {
            //                                        SaleEvent.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        SaleEvent.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in SaleEvent.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        //bc.DestinationTableName = "SaleEvent";
            //                                        //bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        //bc.ColumnMappings.Add("business_date", "business_date");
            //                                        //bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
            //                                        //bc.ColumnMappings.Add("Ev_SaleStored", "Ev_SaleStored");
            //                                        //bc.ColumnMappings.Add("Ev_SaleRecalled", "Ev_SaleRecalled");
            //                                        //bc.ColumnMappings.Add("Ev_BackFromTotal", "Ev_BackFromTotal");
            //                                        //bc.ColumnMappings.Add("Ev_SaleTotal", "Ev_SaleTotal");
            //                                        //bc.ColumnMappings.Add("Type", "Type");
            //                                        //bc.ColumnMappings.Add("Time", "Time");
            //                                        //bc.ColumnMappings.Add("EventsDetail_Id", "EventsDetail_Id");
            //                                        //bc.WriteToServer(SaleEvent);

            //                                    }

            //                                }

            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable Ev_SaleIncrementItemQty = ds.Tables["Ev_SaleIncrementItemQty"];
            //                                    if (Ev_SaleIncrementItemQty != null)
            //                                    {
            //                                        Ev_SaleIncrementItemQty.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        Ev_SaleIncrementItemQty.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in Ev_SaleIncrementItemQty.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "Ev_SaleIncrementItemQty";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("SaleIndex", "SaleIndex");
            //                                        bc.ColumnMappings.Add("Quantity", "Quantity");
            //                                        bc.ColumnMappings.Add("ProductCode", "ProductCode");
            //                                        bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
            //                                        bc.WriteToServer(Ev_SaleIncrementItemQty);

            //                                    }

            //                                }

            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con))
            //                                {


            //                                    DataTable TRX_GetAuthorization = ds.Tables["TRX_GetAuthorization"];
            //                                    if (TRX_GetAuthorization != null)
            //                                    {
            //                                        TRX_GetAuthorization.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        TRX_GetAuthorization.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in TRX_GetAuthorization.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        //bc.DestinationTableName = "TRX_GetAuthorization";
            //                                        //bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        //bc.ColumnMappings.Add("business_date", "business_date");
            //                                        //bc.ColumnMappings.Add("Action", "Action");
            //                                        //bc.ColumnMappings.Add("ManagerID", "ManagerID");
            //                                        //bc.ColumnMappings.Add("ManagerName", "ManagerName");
            //                                        //bc.ColumnMappings.Add("SecurityLevel", "SecurityLevel");
            //                                        //bc.ColumnMappings.Add("ExpirationDate", "ExpirationDate");
            //                                        //bc.ColumnMappings.Add("Password", "Password");
            //                                        //bc.ColumnMappings.Add("Islogged", "Islogged");
            //                                        //bc.ColumnMappings.Add("Method", "Method");
            //                                        ////bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
            //                                        //bc.ColumnMappings.Add("Event_Id", "Event_Id");
            //                                        //bc.WriteToServer(TRX_GetAuthorization);

            //                                    }

            //                                }

            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable EV_NotChargedPromotional = ds.Tables["EV_NotChargedPromotional"];
            //                                    if (EV_NotChargedPromotional != null)
            //                                    {
            //                                        EV_NotChargedPromotional.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        EV_NotChargedPromotional.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in EV_NotChargedPromotional.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "EV_NotChargedPromotional";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("ProductCode", "ProductCode");
            //                                        bc.ColumnMappings.Add("Quantity", "Quantity");
            //                                        bc.ColumnMappings.Add("NotChargedValue", "NotChargedValue");
            //                                        bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
            //                                        bc.WriteToServer(EV_NotChargedPromotional);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable Ev_AddTender = ds.Tables["Ev_AddTender"];
            //                                    if (Ev_AddTender != null)
            //                                    {
            //                                        Ev_AddTender.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        Ev_AddTender.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in Ev_AddTender.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "Ev_AddTender";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("TenderId", "TenderId");
            //                                        bc.ColumnMappings.Add("FaceValue", "FaceValue");
            //                                        bc.ColumnMappings.Add("TenderAmount", "TenderAmount");
            //                                        bc.ColumnMappings.Add("BaseAction", "BaseAction");
            //                                        bc.ColumnMappings.Add("Persisted", "Persisted");
            //                                        bc.ColumnMappings.Add("CardProviderID", "CardProviderID");
            //                                        bc.ColumnMappings.Add("CashlessData", "CashlessData");
            //                                        bc.ColumnMappings.Add("CashlessTransactionID", "CashlessTransactionID");
            //                                        bc.ColumnMappings.Add("PreAuthorization", "PreAuthorization");
            //                                        bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
            //                                        bc.WriteToServer(Ev_AddTender);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable Ev_SetSaleType = ds.Tables["Ev_SetSaleType"];
            //                                    if (Ev_SetSaleType != null)
            //                                    {
            //                                        Ev_SetSaleType.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        Ev_SetSaleType.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in Ev_SetSaleType.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "Ev_SetSaleType";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("Type", "Type");
            //                                        bc.ColumnMappings.Add("ForceExhibition", "ForceExhibition");
            //                                        bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
            //                                        bc.WriteToServer(Ev_SetSaleType);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable Ev_SaleChoice = ds.Tables["Ev_SaleChoice"];
            //                                    if (Ev_SaleChoice != null)
            //                                    {
            //                                        Ev_SaleChoice.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        Ev_SaleChoice.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in Ev_SaleChoice.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "Ev_SaleChoice";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("ProductCode", "ProductCode");
            //                                        bc.ColumnMappings.Add("ChoiceCode", "ChoiceCode");
            //                                        bc.ColumnMappings.Add("Quantity", "Quantity");
            //                                        bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
            //                                        bc.WriteToServer(Ev_SaleChoice);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable Ev_SaleItem = ds.Tables["Ev_SaleItem"];
            //                                    if (Ev_SaleItem != null)
            //                                    {
            //                                        Ev_SaleItem.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        Ev_SaleItem.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in Ev_SaleItem.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "Ev_SaleItem";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("ProductCode", "ProductCode");
            //                                        bc.ColumnMappings.Add("Quantity", "Quantity");
            //                                        bc.ColumnMappings.Add("UpdatedQuantity", "UpdatedQuantity");
            //                                        bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
            //                                        bc.WriteToServer(Ev_SaleItem);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable Ev_SaleCutomInfo = ds.Tables["Ev_SaleCutomInfo"];
            //                                    if (Ev_SaleCutomInfo != null)
            //                                    {
            //                                        Ev_SaleCutomInfo.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        Ev_SaleCutomInfo.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in Ev_SaleCutomInfo.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        //bc.DestinationTableName = "Ev_SaleCutomInfo";
            //                                        //bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        //bc.ColumnMappings.Add("business_date", "business_date");
            //                                        //bc.ColumnMappings.Add("ProductCode", "ProductCode");
            //                                        //bc.ColumnMappings.Add("Quantity", "Quantity");
            //                                        //bc.ColumnMappings.Add("UpdatedQuantity", "UpdatedQuantity");
            //                                        //bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
            //                                        //bc.WriteToServer(Ev_SaleCutomInfo);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable Ev_SaleEnd = ds.Tables["Ev_SaleEnd"];
            //                                    if (Ev_SaleEnd != null)
            //                                    {
            //                                        Ev_SaleEnd.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        Ev_SaleEnd.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in Ev_SaleEnd.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "Ev_SaleEnd";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("Type", "Type");
            //                                        bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
            //                                        bc.WriteToServer(Ev_SaleEnd);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable Ev_SaleStart = ds.Tables["Ev_SaleStart"];
            //                                    if (Ev_SaleStart != null)
            //                                    {
            //                                        Ev_SaleStart.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        Ev_SaleStart.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in Ev_SaleStart.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "Ev_SaleStart";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("DisabledChoices", "DisabledChoices");
            //                                        bc.ColumnMappings.Add("TenderPersisted", "TenderPersisted");
            //                                        bc.ColumnMappings.Add("Multiorder", "Multiorder");
            //                                        bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
            //                                        bc.WriteToServer(Ev_SaleStart);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable Fiscal_Information = ds.Tables["Fiscal_Information"];
            //                                    if (Fiscal_Information != null)
            //                                    {
            //                                        Fiscal_Information.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        Fiscal_Information.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in Fiscal_Information.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "Fiscal_Information";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("TIN", "TIN");
            //                                        bc.ColumnMappings.Add("name", "name");
            //                                        bc.ColumnMappings.Add("address", "address");
            //                                        bc.ColumnMappings.Add("ZIP", "ZIP");
            //                                        bc.ColumnMappings.Add("Order_Id", "Order_Id");
            //                                        bc.WriteToServer(Fiscal_Information);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable Ev_DrawerClose = ds.Tables["Ev_DrawerClose"];
            //                                    if (Ev_DrawerClose != null)
            //                                    {
            //                                        Ev_DrawerClose.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        Ev_DrawerClose.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in Ev_DrawerClose.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "Ev_DrawerClose";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("TotalOpenTime", "TotalOpenTime");
            //                                        bc.ColumnMappings.Add("Event_Id", "Event_Id");
            //                                        bc.WriteToServer(Ev_DrawerClose);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable TRX_OperLogin = ds.Tables["TRX_OperLogin"];
            //                                    if (TRX_OperLogin != null)
            //                                    {
            //                                        TRX_OperLogin.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        TRX_OperLogin.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in TRX_OperLogin.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "TRX_OperLogin";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("CrewID", "CrewID");
            //                                        bc.ColumnMappings.Add("CrewName", "CrewName");
            //                                        bc.ColumnMappings.Add("CrewSecurityLevel", "CrewSecurityLevel");
            //                                        bc.ColumnMappings.Add("POD", "POD");
            //                                        bc.ColumnMappings.Add("RemotePOD", "RemotePOD");
            //                                        bc.ColumnMappings.Add("AutoLogin", "AutoLogin");
            //                                        bc.ColumnMappings.Add("Event_Id", "Event_Id");
            //                                        bc.WriteToServer(TRX_OperLogin);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable TRX_SetPOD = ds.Tables["TRX_SetPOD"];
            //                                    if (TRX_SetPOD != null)
            //                                    {
            //                                        TRX_SetPOD.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        TRX_SetPOD.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in TRX_SetPOD.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "TRX_SetPOD";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("PODId", "PODId");
            //                                        bc.ColumnMappings.Add("Event_Id", "Event_Id");
            //                                        bc.WriteToServer(TRX_SetPOD);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable TRX_DayOpen = ds.Tables["TRX_DayOpen"];
            //                                    if (TRX_DayOpen != null)
            //                                    {
            //                                        TRX_DayOpen.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        TRX_DayOpen.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in TRX_DayOpen.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "TRX_DayOpen";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("BusinessDate", "BusinessDate");
            //                                        bc.ColumnMappings.Add("Event_Id", "Event_Id");
            //                                        bc.WriteToServer(TRX_DayOpen);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable TRX_TaxTable = ds.Tables["TRX_TaxTable"];
            //                                    if (TRX_TaxTable != null)
            //                                    {
            //                                        TRX_TaxTable.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        TRX_TaxTable.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in TRX_TaxTable.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "TRX_TaxTable";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("TRX_TaxTable_Id", "TRX_TaxTable_Id");
            //                                        bc.ColumnMappings.Add("Event_Id", "Event_Id");
            //                                        bc.WriteToServer(TRX_TaxTable);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable TaxType = ds.Tables["TaxType"];
            //                                    if (TaxType != null)
            //                                    {
            //                                        TaxType.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        TaxType.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in TaxType.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "TaxType";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("TaxId", "TaxId");
            //                                        bc.ColumnMappings.Add("TaxDescription", "TaxDescription");
            //                                        bc.ColumnMappings.Add("TaxRate", "TaxRate");
            //                                        bc.ColumnMappings.Add("TaxBasis", "TaxBasis");
            //                                        bc.ColumnMappings.Add("TaxCalcType", "TaxCalcType");
            //                                        bc.ColumnMappings.Add("Rounding", "Rounding");
            //                                        bc.ColumnMappings.Add("Precision", "Precision");
            //                                        bc.ColumnMappings.Add("TRX_TaxTable_Id", "TRX_TaxTable_Id");
            //                                        bc.WriteToServer(TaxType);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable TRX_TenderTable = ds.Tables["TRX_TenderTable"];
            //                                    if (TRX_TenderTable != null)
            //                                    {
            //                                        TRX_TenderTable.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        TRX_TenderTable.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in TRX_TenderTable.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "TRX_TenderTable";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("TRX_TenderTable_Id", "TRX_TenderTable_Id");
            //                                        bc.ColumnMappings.Add("Event_Id", "Event_Id");
            //                                        bc.WriteToServer(TRX_TenderTable);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable TenderType = ds.Tables["TenderType"];
            //                                    if (TenderType != null)
            //                                    {
            //                                        TenderType.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        TenderType.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in TenderType.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "TenderType";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("TenderId", "TenderId");
            //                                        bc.ColumnMappings.Add("TenderFiscalIndex", "TenderFiscalIndex");
            //                                        bc.ColumnMappings.Add("TenderName", "TenderName");
            //                                        bc.ColumnMappings.Add("TenderCategory", "TenderCategory");
            //                                        bc.ColumnMappings.Add("Taxoption", "Taxoption");
            //                                        bc.ColumnMappings.Add("DefaultSkimLimit", "DefaultSkimLimit");
            //                                        bc.ColumnMappings.Add("DefaultHaloLimit", "DefaultHaloLimit");
            //                                        bc.ColumnMappings.Add("SubtotalOption", "SubtotalOption");
            //                                        bc.ColumnMappings.Add("CurrencyDecimals", "CurrencyDecimals");
            //                                        bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
            //                                        bc.ColumnMappings.Add("TRX_TenderTable_Id", "TRX_TenderTable_Id");
            //                                        bc.WriteToServer(TenderType);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable TenderFlags = ds.Tables["TenderFlags"];
            //                                    if (TenderFlags != null)
            //                                    {
            //                                        TenderFlags.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        TenderFlags.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in TenderFlags.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "TenderFlags";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
            //                                        bc.ColumnMappings.Add("TenderFlags_Id", "TenderFlags_Id");
            //                                        bc.WriteToServer(TenderFlags);

            //                                    }

            //                                }

            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable Info = ds.Tables["Info"];
            //                                    if (Info != null)
            //                                    {
            //                                        Info.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        Info.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in Info.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "Info";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("name", "name");
            //                                        bc.ColumnMappings.Add("value", "value");
            //                                        bc.ColumnMappings.Add("CustomInfo_Id", "CustomInfo_Id");
            //                                        bc.WriteToServer(Info);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con))
            //                                {


            //                                    DataTable TenderFlag = ds.Tables["TenderFlag"];
            //                                    if (TenderFlag != null)
            //                                    {
            //                                        // TenderFlag.Columns.Add(new DataColumn()
            //                                        //  {
            //                                        //      ColumnName = "STLD_Location",
            //                                        //      DataType = typeof(string)
            //                                        //  });

            //                                        //  TenderFlag.Columns.Add(new DataColumn()
            //                                        //  {
            //                                        //      ColumnName = "business_date",
            //                                        //      DataType = typeof(string)
            //                                        //  });
            //                                        //foreach (DataRow vrow in TenderFlag.Rows)
            //                                        // {

            //                                        //     vrow["STLD_Location"] = lbl_location.Text;
            //                                        //     vrow["business_date"] = lbl_business.Text;

            //                                        // }
            //                                        //  bc.DestinationTableName = "TenderFlag";
            //                                        //  bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        //  bc.ColumnMappings.Add("business_date", "business_date");
            //                                        //   bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
            //                                        //   bc.ColumnMappings.Add("TenderFlags_Id", "TenderFlags_Id");
            //                                        //   bc.WriteToServer(TenderFlag);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable TenderChange = ds.Tables["TenderChange"];
            //                                    if (TenderChange != null)
            //                                    {
            //                                        TenderChange.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        TenderChange.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in TenderChange.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "TenderChange";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("id", "id");
            //                                        bc.ColumnMappings.Add("type", "type");
            //                                        bc.ColumnMappings.Add("roundToMinAmount", "roundToMinAmount");
            //                                        bc.ColumnMappings.Add("maxAllowed", "maxAllowed");
            //                                        bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
            //                                        bc.WriteToServer(TenderChange);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable GiftCoupon = ds.Tables["GiftCoupon"];
            //                                    if (GiftCoupon != null)
            //                                    {
            //                                        GiftCoupon.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        GiftCoupon.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in GiftCoupon.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "GiftCoupon";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("LegacyId", "LegacyId");
            //                                        bc.ColumnMappings.Add("OperatorDefined", "OperatorDefined");
            //                                        bc.ColumnMappings.Add("Amount", "Amount");
            //                                        bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
            //                                        bc.WriteToServer(GiftCoupon);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable OtherPayments = ds.Tables["OtherPayments"];
            //                                    if (OtherPayments != null)
            //                                    {
            //                                        //OtherPayments.Columns.Add(new DataColumn()
            //                                        //{
            //                                        //    ColumnName = "STLD_Location",
            //                                        //    DataType = typeof(string)
            //                                        //});

            //                                        //OtherPayments.Columns.Add(new DataColumn()
            //                                        //{
            //                                        //    ColumnName = "business_date",
            //                                        //    DataType = typeof(string)
            //                                        //});
            //                                        //foreach (DataRow vrow in OtherPayments.Rows)
            //                                        //{

            //                                        //    vrow["STLD_Location"] = lbl_location.Text;
            //                                        //    vrow["business_date"] = lbl_business.Text;

            //                                        //}
            //                                        //bc.DestinationTableName = "OtherPayments";
            //                                        //bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        //bc.ColumnMappings.Add("business_date", "business_date");
            //                                        //bc.ColumnMappings.Add("LegacyId", "LegacyId");
            //                                        //bc.ColumnMappings.Add("OperatorDefined", "OperatorDefined");
            //                                        //bc.ColumnMappings.Add("Amount", "Amount");
            //                                        //bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
            //                                        //bc.WriteToServer(OtherPayments);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable ForeignCurrency = ds.Tables["ForeignCurrency"];
            //                                    if (ForeignCurrency != null)
            //                                    {
            //                                        ForeignCurrency.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        ForeignCurrency.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in ForeignCurrency.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "ForeignCurrency";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("LegacyId", "LegacyId");
            //                                        bc.ColumnMappings.Add("ExchangeRate", "ExchangeRate");
            //                                        bc.ColumnMappings.Add("Precision", "Precision");
            //                                        bc.ColumnMappings.Add("Rounding", "Rounding");
            //                                        bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
            //                                        bc.WriteToServer(ForeignCurrency);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable ElectronicPayment = ds.Tables["ElectronicPayment"];
            //                                    if (ElectronicPayment != null)
            //                                    {
            //                                        ElectronicPayment.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        ElectronicPayment.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in ElectronicPayment.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "ElectronicPayment";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("LegacyId", "LegacyId");
            //                                        bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
            //                                        bc.WriteToServer(ElectronicPayment);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable CreditSales = ds.Tables["CreditSales"];
            //                                    if (CreditSales != null)
            //                                    {
            //                                        CreditSales.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        CreditSales.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in CreditSales.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "CreditSales";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("LegacyId", "LegacyId");
            //                                        bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
            //                                        bc.WriteToServer(CreditSales);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable Trx_DayPart = ds.Tables["Trx_DayPart"];
            //                                    if (Trx_DayPart != null)
            //                                    {
            //                                        Trx_DayPart.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        Trx_DayPart.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in Trx_DayPart.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        //bc.DestinationTableName = "Trx_DayPart";
            //                                        //bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        //bc.ColumnMappings.Add("business_date", "business_date");
            //                                        //bc.ColumnMappings.Add("LegacyId", "LegacyId");
            //                                        //bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
            //                                        //bc.WriteToServer(Trx_DayPart);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable TRX_BaseConfig = ds.Tables["TRX_BaseConfig"];
            //                                    if (TRX_BaseConfig != null)
            //                                    {
            //                                        TRX_BaseConfig.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        TRX_BaseConfig.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in TRX_BaseConfig.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "TRX_BaseConfig";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("TRX_BaseConfig_Id", "TRX_BaseConfig_Id");
            //                                        bc.ColumnMappings.Add("POS", "POS");
            //                                        bc.ColumnMappings.Add("POD", "POD");
            //                                        bc.ColumnMappings.Add("Event_Id", "Event_Id");
            //                                        bc.WriteToServer(TRX_BaseConfig);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable Config = ds.Tables["Config"];
            //                                    if (Config != null)
            //                                    {
            //                                        Config.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        Config.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in Config.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "Config";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("MenuPriceBasis", "MenuPriceBasis");
            //                                        bc.ColumnMappings.Add("WeekEndBreakfastStartTime", "WeekEndBreakfastStartTime");
            //                                        bc.ColumnMappings.Add("WeekEndBreakfastStopTime", "WeekEndBreakfastStopTime");
            //                                        bc.ColumnMappings.Add("WeekDayBreakfastStartTime", "WeekDayBreakfastStartTime");
            //                                        bc.ColumnMappings.Add("WeekDayBreakfastStopTime", "WeekDayBreakfastStopTime");
            //                                        bc.ColumnMappings.Add("DecimalPlaces", "DecimalPlaces");
            //                                        bc.ColumnMappings.Add("CheckRefund", "CheckRefund");
            //                                        bc.ColumnMappings.Add("GrandTotalFlag", "GrandTotalFlag");
            //                                        bc.ColumnMappings.Add("StoreId", "StoreId");
            //                                        bc.ColumnMappings.Add("StoreName", "StoreName");
            //                                        bc.ColumnMappings.Add("AcceptNegativeQty", "AcceptNegativeQty");
            //                                        bc.ColumnMappings.Add("AcceptZeroPricePMix", "AcceptZeroPricePMix");
            //                                        bc.ColumnMappings.Add("FloatPriceTenderId", "FloatPriceTenderId");
            //                                        bc.ColumnMappings.Add("MinCirculatingAmount", "MinCirculatingAmount");
            //                                        bc.ColumnMappings.Add("TRX_BaseConfig_Id", "TRX_BaseConfig_Id");
            //                                        bc.WriteToServer(Config);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable POSConfig = ds.Tables["POSConfig"];
            //                                    if (POSConfig != null)
            //                                    {
            //                                        POSConfig.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        POSConfig.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in POSConfig.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "POSConfig";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("CountTCsFullDiscEM", "CountTCsFullDiscEM");
            //                                        bc.ColumnMappings.Add("RefundBehaviour", "RefundBehaviour");
            //                                        bc.ColumnMappings.Add("OverringBehaviour", "OverringBehaviour");
            //                                        bc.ColumnMappings.Add("TRX_BaseConfig_Id", "TRX_BaseConfig_Id");
            //                                        bc.WriteToServer(POSConfig);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable TRX_SetSMState = ds.Tables["TRX_SetSMState"];
            //                                    if (TRX_SetSMState != null)
            //                                    {
            //                                        TRX_SetSMState.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        TRX_SetSMState.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in TRX_SetSMState.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "TRX_SetSMState";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("POSState", "POSState");
            //                                        bc.ColumnMappings.Add("CrewId", "CrewId");
            //                                        bc.ColumnMappings.Add("CrewName", "CrewName");
            //                                        bc.ColumnMappings.Add("CrewSecurityLevel", "CrewSecurityLevel");
            //                                        bc.ColumnMappings.Add("LoginTime", "LoginTime");
            //                                        bc.ColumnMappings.Add("LogoutTime", "LogoutTime");
            //                                        bc.ColumnMappings.Add("InitialFloat", "InitialFloat");
            //                                        bc.ColumnMappings.Add("PODId", "PODId");
            //                                        bc.ColumnMappings.Add("Event_Id", "Event_Id");
            //                                        bc.WriteToServer(TRX_SetSMState);

            //                                    }

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {


            //                                    DataTable TRX_InitGTotal = ds.Tables["TRX_InitGTotal"];
            //                                    if (TRX_InitGTotal != null)
            //                                    {
            //                                        TRX_InitGTotal.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "STLD_Location",
            //                                            DataType = typeof(string)
            //                                        });

            //                                        TRX_InitGTotal.Columns.Add(new DataColumn()
            //                                        {
            //                                            ColumnName = "business_date",
            //                                            DataType = typeof(string)
            //                                        });
            //                                        foreach (DataRow vrow in TRX_InitGTotal.Rows)
            //                                        {

            //                                            vrow["STLD_Location"] = lbl_location.Text;
            //                                            vrow["business_date"] = lbl_business.Text;

            //                                        }
            //                                        bc.DestinationTableName = "TRX_InitGTotal";
            //                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                        bc.ColumnMappings.Add("business_date", "business_date");
            //                                        bc.ColumnMappings.Add("amount", "amount");
            //                                        bc.ColumnMappings.Add("Event_Id", "Event_Id");
            //                                        bc.WriteToServer(TRX_InitGTotal);

            //                                    }

            //                                }



            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {
            //                                    DataTable TLD = ds.Tables["TLD"];
            //                                    bc.DestinationTableName = "TLD";
            //                                    bc.ColumnMappings.Add("TLD_Id", "TLD_Id");
            //                                    bc.ColumnMappings.Add("LogVersion", "LogVersion");
            //                                    bc.ColumnMappings.Add("storeId", "storeId");
            //                                    bc.ColumnMappings.Add("businessDate", "businessDate");
            //                                    bc.ColumnMappings.Add("swVersion", "swVersion");
            //                                    bc.ColumnMappings.Add("checkPoint", "checkPoint");
            //                                    bc.ColumnMappings.Add("end", "end");
            //                                    bc.ColumnMappings.Add("productionStatus", "productionStatus");
            //                                    bc.ColumnMappings.Add("hasMoreContent", "hasMoreContent");
            //                                    bc.WriteToServer(TLD);

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {
            //                                    DataTable Node = ds.Tables["Node"];
            //                                    Node.Columns.Add(new DataColumn()
            //                                    {
            //                                        ColumnName = "STLD_Location",
            //                                        DataType = typeof(string)
            //                                    });

            //                                    Node.Columns.Add(new DataColumn()
            //                                    {
            //                                        ColumnName = "business_date",
            //                                        DataType = typeof(string)
            //                                    });
            //                                    foreach (DataRow vrow in Node.Rows)
            //                                    {

            //                                        vrow["STLD_Location"] = lbl_location.Text;
            //                                        vrow["business_date"] = lbl_business.Text;

            //                                    }
            //                                    bc.DestinationTableName = "Node";
            //                                    bc.ColumnMappings.Add("Node_Id", "Node_Id");
            //                                    bc.ColumnMappings.Add("TLD_Id", "TLD_Id");
            //                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                    bc.ColumnMappings.Add("business_date", "business_date");
            //                                    bc.ColumnMappings.Add("nodeStatus", "nodeStatus");
            //                                    bc.ColumnMappings.Add("id", "id");
            //                                    bc.WriteToServer(Node);

            //                                }
            //                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
            //                                {
            //                                    DataTable Event = ds.Tables["Event"];
            //                                    Event.Columns.Add(new DataColumn()
            //                                    {
            //                                        ColumnName = "STLD_Location",
            //                                        DataType = typeof(string)
            //                                    });

            //                                    Event.Columns.Add(new DataColumn()
            //                                    {
            //                                        ColumnName = "business_date",
            //                                        DataType = typeof(string)
            //                                    });
            //                                    foreach (DataRow vrow in Event.Rows)
            //                                    {

            //                                        vrow["STLD_Location"] = lbl_location.Text;
            //                                        vrow["business_date"] = lbl_business.Text;

            //                                    }
            //                                    bc.DestinationTableName = "Event";
            //                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
            //                                    bc.ColumnMappings.Add("business_date", "business_date");
            //                                    bc.ColumnMappings.Add("Event_Id", "Event_Id");
            //                                    //  bc.ColumnMappings.Add("TRX_UnaDrawerOpening", "TRX_UnaDrawerOpening");
            //                                    bc.ColumnMappings.Add("RegId", "RegId");
            //                                    bc.ColumnMappings.Add("Type", "Type");
            //                                    bc.ColumnMappings.Add("Time", "Time");
            //                                    bc.ColumnMappings.Add("Node_Id", "Node_Id");
            //                                    bc.WriteToServer(Event);

            //                                }





            //                                string smt = "INSERT INTO [STLD].[dbo].[STLDPROCESS_STATUS] ([STLD_Location] ,[business_date]) VALUES (@STLD_Location,@business_date)";
            //                                SqlCommand cmd = new SqlCommand(smt, con, tran);
            //                                cmd.Parameters.AddWithValue("@STLD_Location", SqlDbType.VarChar).Value = lbl_location.Text;
            //                                cmd.Parameters.AddWithValue("@business_date", SqlDbType.VarChar).Value = lbl_business.Text;
            //                                cmd.ExecuteNonQuery();
            //                                File.Move(file, Path.ChangeExtension(file, ".Proceed"));

            //                                //////////////////////

            //                                tran.Commit();


            //                            }
            //                            catch
            //                            {
            //                                tran.Rollback();
            //                                // throw;
            //                            }

            //                        }

            //                    }









            //                }
            //            }
            //        }
            //        catch
            //        {

            //        }












            //        ///////////////////////////// Check dublicate records////////////////////////







            //        //////////////////////////////////////////////////////////////////

            //    }
            //    else
            //    {
            //        //File.Delete(file);
            //    }


            //}

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            BTN_PMIX.PerformClick();

            foreach (string file in Directory.EnumerateFiles("E:\\ImpotSheiV2\\Extract\\Processingfiles", "*.xml"))
            {


                string contents = File.ReadAllText(file);
                ///////////////////// File Reading/////////////////////////

                lbl_nameval.Text = Convert.ToString(file.ToString());
                string tempfile = Convert.ToString(file.ToString());
                if (lbl_nameval.Text.Contains("STLD"))
                {


                                                                                                                                                                                                                                                                 
                    string connString = SQLCON.ConnectionString2;
                    lbl_STLD.Text = file;

                    SQLCON vb = new SQLCON();
                    vb.OpenConnection();

                    DataSet ds = new DataSet();

                    ds.ReadXml(lbl_STLD.Text);









                    DataTable CHKTLD = ds.Tables["TLD"];
                    foreach (DataRow row in CHKTLD.Rows)
                    {
                        // ... Write value of first field as integer.
                        //  string temploca = file.Substring(0, 57);
                        // string tempdate = file.Substring(0, 66);
                        string storloc = row.ItemArray[2].ToString();
                        string storbusinessdate = row.ItemArray[3].ToString();


                        lbl_location.Text = storloc;
                        lbl_business.Text = storbusinessdate;
                    }

                    using (SqlConnection con1 = new SqlConnection(connString))
                    {

                        con1.Open();

                        SqlCommand comm = new SqlCommand("SELECT COUNT(*) FROM [STLDPROCESS_STATUS] where [STLD_Location]=@STLD_Location and business_date=@business_date", con1);
                        comm.Parameters.AddWithValue("@STLD_Location", SqlDbType.VarChar).Value = lbl_location.Text;
                        comm.Parameters.AddWithValue("@business_date", SqlDbType.VarChar).Value = lbl_business.Text;
                        Int32 count = Convert.ToInt32(comm.ExecuteScalar());
                        if (count > 0)
                        {
                            lblCount.Text = Convert.ToString(count.ToString()); //For example a Label
                        }
                        else
                        {
                            if (lbl_location.Text.Contains("online"))
                            {
                                lblCount.Text = "wrong";
                            }
                            else
                            {
                                lblCount.Text = "0";

                                SqlCommand comm2 = new SqlCommand("SELECT [StoreID]   FROM [STLD].[dbo].[TB_STOREMASTER] where [StoreID]=@STLD_Location", con1);
                                comm2.Parameters.AddWithValue("@STLD_Location", SqlDbType.VarChar).Value = lbl_location.Text;
                                Int32 count2 = Convert.ToInt32(comm2.ExecuteScalar());

                                if (count2 > 0)
                                {
                                    lblCount.Text = "0"; //For example a Label
                                }
                                else
                                {
                                    lblCount.Text = "wrong";
                                }
                            }
                        }
                        con1.Close();

                    }
                    ///////////////////////////// Check dublicate records////////////////////////






                    using (SqlConnection con = new SqlConnection(connString))
                    {
                        con.Open();

                        if (lblCount.Text.Contains('0'))
                        {

                            using (var connection = con)
                            {
                                using (var tran = con.BeginTransaction())
                                {


                                    try
                                    {




                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {

                                            DataTable TLD = ds.Tables["TLD"];
                                            foreach (DataRow row in TLD.Rows)
                                            {
                                                // ... Write value of first field as integer.
                                                // string storloc = row.ItemArray[2].ToString();
                                                // string storbusinessdate = row.ItemArray[3].ToString();

                                                //lbl_location.Text = storloc;
                                                //lbl_business.Text = storbusinessdate;
                                            }





                                            DataTable TRX_Sale = ds.Tables["TRX_Sale"];

                                            TRX_Sale.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "STLD_Location",
                                                DataType = typeof(string)
                                            });

                                            TRX_Sale.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "business_date",
                                                DataType = typeof(string)
                                            });
                                            foreach (DataRow vrow in TRX_Sale.Rows)
                                            {

                                                vrow["STLD_Location"] = lbl_location.Text;
                                                vrow["business_date"] = lbl_business.Text;

                                            }



                                            bc.DestinationTableName = "TRX_Sale";
                                            bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            bc.ColumnMappings.Add("business_date", "business_date");
                                            bc.ColumnMappings.Add("TRX_Sale_Id", "TRX_Sale_Id");
                                            bc.ColumnMappings.Add("status", "status");
                                            bc.ColumnMappings.Add("POD", "POD");
                                            bc.ColumnMappings.Add("RemPOD", "RemPOD");
                                            bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                            bc.WriteToServer(TRX_Sale);

                                        }




                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {

                                            //------- Order Table////////////////
                                            DataTable Order_TB = ds.Tables["Order"];
                                            Order_TB.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "STLD_Location",
                                                DataType = typeof(string)
                                            });

                                            Order_TB.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "business_date",
                                                DataType = typeof(string)
                                            });
                                            foreach (DataRow vrow in Order_TB.Rows)
                                            {

                                                vrow["STLD_Location"] = lbl_location.Text;
                                                vrow["business_date"] = lbl_business.Text;

                                            }
                                            bc.DestinationTableName = "Order_TB";
                                            bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            bc.ColumnMappings.Add("business_date", "business_date");
                                            bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                            bc.ColumnMappings.Add("Timestamp", "Timestamp");
                                            bc.ColumnMappings.Add("uniqueId", "uniqueId");
                                            bc.ColumnMappings.Add("kind", "kind");
                                            bc.ColumnMappings.Add("key", "key");
                                            bc.ColumnMappings.Add("major", "major");
                                            bc.ColumnMappings.Add("minor", "minor");
                                            bc.ColumnMappings.Add("side", "side");
                                            bc.ColumnMappings.Add("receiptNumber", "receiptNumber");
                                            bc.ColumnMappings.Add("fpReceiptNumber", "fpReceiptNumber");
                                            //bc.ColumnMappings.Add("boot", "boot");
                                            bc.ColumnMappings.Add("saleType", "saleType");
                                            bc.ColumnMappings.Add("totalAmount", "totalAmount");
                                            bc.ColumnMappings.Add("nonProductAmount", "nonProductAmount");
                                            bc.ColumnMappings.Add("totalTax", "totalTax");
                                            bc.ColumnMappings.Add("nonProductTax", "nonProductTax");
                                            bc.ColumnMappings.Add("orderSrc", "orderSrc");
                                            bc.ColumnMappings.Add("startSaleDate", "startSaleDate");
                                            bc.ColumnMappings.Add("startSaleTime", "startSaleTime");
                                            bc.ColumnMappings.Add("endSaleDate", "endSaleDate");
                                            bc.ColumnMappings.Add("endSaleTime", "endSaleTime");
                                            bc.ColumnMappings.Add("TRX_Sale_Id", "TRX_Sale_Id");
                                            bc.WriteToServer(Order_TB);

                                            //----------------------------------



                                        }

                                        //------refund
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con))
                                        {
                                            // DataTable TRX_Refund = ds.Tables["TRX_Refund"];

                                            // TRX_Refund.Columns.Add(new DataColumn()
                                            // {
                                            //     ColumnName = "STLD_Location",
                                            //     DataType = typeof(string)
                                            // });

                                            // TRX_Refund.Columns.Add(new DataColumn()
                                            // {
                                            //     ColumnName = "business_date",
                                            //     DataType = typeof(string)
                                            // });
                                            // foreach (DataRow vrow in TRX_Refund.Rows)
                                            // {

                                            //     vrow["STLD_Location"] = lbl_location.Text;
                                            //     vrow["business_date"] = lbl_business.Text;

                                            // }



                                            // bc.DestinationTableName = "TRX_Refund";
                                            // bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            // bc.ColumnMappings.Add("business_date", "business_date");
                                            // bc.ColumnMappings.Add("TRX_Refund_Id", "TRX_Refund_Id");
                                            //bc.ColumnMappings.Add("status", "status");
                                            // bc.ColumnMappings.Add("POD", "POD");
                                            // bc.ColumnMappings.Add("RemPOD", "RemPOD");
                                            // bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                            // bc.WriteToServer(TRX_Refund);

                                        }



                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {

                                            //------- Item Table////////////////
                                            DataTable Item_TB = ds.Tables["Item"];
                                            if (Item_TB != null)
                                            {
                                                Item_TB.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Item_TB.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Item_TB.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }

                                                bc.DestinationTableName = "Item_TB";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("Item_Id", "Item_Id");
                                                bc.ColumnMappings.Add("code", "code");
                                                bc.ColumnMappings.Add("type", "type");
                                                bc.ColumnMappings.Add("action", "action");
                                                bc.ColumnMappings.Add("level", "level");
                                                bc.ColumnMappings.Add("id", "id");
                                                bc.ColumnMappings.Add("displayOrder", "displayOrder");
                                                bc.ColumnMappings.Add("qty", "qty");
                                                bc.ColumnMappings.Add("grillQty", "grillQty");
                                                bc.ColumnMappings.Add("grillModifier", "grillModifier");
                                                bc.ColumnMappings.Add("qtyPromo", "qtyPromo");
                                                bc.ColumnMappings.Add("chgAfterTotal", "chgAfterTotal");
                                                bc.ColumnMappings.Add("BPPrice", "BPPrice");
                                                bc.ColumnMappings.Add("BPTax", "BPTax");
                                                bc.ColumnMappings.Add("BDPrice", "BDPrice");
                                                bc.ColumnMappings.Add("BDTax", "BDTax");
                                                bc.ColumnMappings.Add("totalPrice", "totalPrice");
                                                bc.ColumnMappings.Add("totalTax", "totalTax");
                                                bc.ColumnMappings.Add("category", "category");
                                                bc.ColumnMappings.Add("familyGroup", "familyGroup");
                                                bc.ColumnMappings.Add("daypart", "daypart");
                                                bc.ColumnMappings.Add("description", "description");
                                                bc.ColumnMappings.Add("department", "department");
                                                bc.ColumnMappings.Add("departmentClass", "departmentClass");
                                                bc.ColumnMappings.Add("departmentSubClass", "departmentSubClass");
                                                bc.ColumnMappings.Add("unitPrice", "unitPrice");
                                                bc.ColumnMappings.Add("unitTax", "unitTax");
                                                bc.ColumnMappings.Add("solvedChoice", "solvedChoice");
                                                bc.ColumnMappings.Add("isUpcharge", "isUpcharge");
                                                bc.ColumnMappings.Add("Item_Id_0", "Item_Id_0");
                                                bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                bc.WriteToServer(Item_TB);

                                            }
                                            ////////////////////////////////////

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {
                                            ///-------------- Promo table-----------------
                                            DataTable TRX_Overring = ds.Tables["TRX_Overring"];
                                            if (TRX_Overring != null)
                                            {
                                                TRX_Overring.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });
                                                TRX_Overring.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TRX_Overring.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TRX_Overring";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("TRX_Overring_Id", "TRX_Overring_Id");
                                                bc.ColumnMappings.Add("POD", "POD");
                                                bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                bc.WriteToServer(TRX_Overring);
                                            }
                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {
                                            ///-------------- Promo table-----------------
                                            DataTable Product = ds.Tables["Product"];
                                            if (Product != null)
                                            {
                                                Product.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });
                                                Product.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Product.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Product";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("code", "code");
                                                bc.ColumnMappings.Add("quantity", "quantity");
                                                bc.ColumnMappings.Add("Components_Id", "Components_Id");
                                                bc.WriteToServer(Product);
                                            }
                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {
                                            ///-------------- Promo table-----------------
                                            DataTable Components = ds.Tables["Components"];
                                            if (Components != null)
                                            {
                                                Components.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });
                                                Components.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Components.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Components";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("Components_Id", "Components_Id");
                                                bc.ColumnMappings.Add("Ev_BreakValueMeal_Id", "Ev_BreakValueMeal_Id");
                                                bc.WriteToServer(Components);
                                            }
                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {
                                            ///-------------- Promo table-----------------
                                            DataTable Ev_BreakValueMeal = ds.Tables["Ev_BreakValueMeal"];
                                            if (Ev_BreakValueMeal != null)
                                            {
                                                Ev_BreakValueMeal.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });
                                                Ev_BreakValueMeal.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Ev_BreakValueMeal.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Ev_BreakValueMeal";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("ProductCode", "ProductCode");
                                                bc.ColumnMappings.Add("Quantity", "Quantity");
                                                bc.ColumnMappings.Add("Ev_BreakValueMeal_Id", "Ev_BreakValueMeal_Id");
                                                bc.WriteToServer(Ev_BreakValueMeal);
                                            }
                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {
                                            ///-------------- Promo table-----------------
                                            DataTable Promo = ds.Tables["Promo"];
                                            if (Promo != null)
                                            {
                                                Promo.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Promo.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Promo.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }


                                                bc.DestinationTableName = "Promo";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("id", "id");
                                                bc.ColumnMappings.Add("name", "name");
                                                bc.ColumnMappings.Add("qty", "qty");
                                                bc.ColumnMappings.Add("Item_id", "Item_id");
                                                bc.WriteToServer(Promo);
                                            }
                                            //------------------------------------------
                                        }

                                        using (SqlBulkCopy bc = new SqlBulkCopy(con))
                                        {
                                            ///////------------------------
                                            // DataTable PromotionApplied = ds.Tables["PromotionApplied"];
                                            //  if (PromotionApplied != null)
                                            //  {
                                            //     PromotionApplied.Columns.Add(new DataColumn()
                                            //    {
                                            //        ColumnName = "STLD_Location",
                                            //        DataType = typeof(string)
                                            //    });

                                            //    PromotionApplied.Columns.Add(new DataColumn()
                                            //    {
                                            //        ColumnName = "business_date",
                                            //        DataType = typeof(string)
                                            //    });
                                            //    foreach (DataRow vrow in PromotionApplied.Rows)
                                            //    {

                                            //        vrow["STLD_Location"] = lbl_location.Text;
                                            //        vrow["business_date"] = lbl_business.Text;

                                            //    }
                                            //    bc.DestinationTableName = "PromotionApplied";
                                            //    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            //    bc.ColumnMappings.Add("business_date", "business_date");
                                            //    bc.ColumnMappings.Add("promotionId", "promotionId");
                                            //    bc.ColumnMappings.Add("promotionCounter", "promotionCounter");
                                            //    bc.ColumnMappings.Add("eligible", "eligible");
                                            //    bc.ColumnMappings.Add("originalPrice", "originalPrice");
                                            //    bc.ColumnMappings.Add("discountAmount", "discountAmount");
                                            //    bc.ColumnMappings.Add("discountType", "discountType");
                                            //    bc.ColumnMappings.Add("originalItemPromoQty", "originalItemPromoQty");
                                            //    bc.ColumnMappings.Add("originalProductCode", "originalProductCode");
                                            //    bc.ColumnMappings.Add("offerId", "offerId");
                                            //    bc.ColumnMappings.Add("Item_Id", "Item_Id");
                                            //    bc.WriteToServer(PromotionApplied);
                                            //    ///////////////////////

                                            //}
                                        }

                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {
                                            ////////// Offer ////// Table
                                            DataTable Offers = ds.Tables["Offers"];

                                            if (Offers != null)
                                            {

                                                Offers.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Offers.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Offers.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Offers";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("offerId", "offerId");

                                                // bc.ColumnMappings.Add("beforeOfferPrice", "beforeOfferPrice");
                                                // bc.ColumnMappings.Add("discountAmount", "discountAmount");
                                                //  bc.ColumnMappings.Add("discountType", "discountType");
                                                // bc.ColumnMappings.Add("Item_Id", "Item_Id");
                                                bc.ColumnMappings.Add("customerId", "customerId");
                                                bc.ColumnMappings.Add("offerName", "offerName");
                                                bc.ColumnMappings.Add("override", "override");
                                                bc.ColumnMappings.Add("applied", "applied");
                                                bc.ColumnMappings.Add("promotionId", "promotionId");
                                                bc.ColumnMappings.Add("offerBarcodeType", "offerBarcodeType");
                                                bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                bc.WriteToServer(Offers);
                                            }
                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {
                                            DataTable TaxChain = ds.Tables["TaxChain"];

                                            TaxChain.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "STLD_Location",
                                                DataType = typeof(string)
                                            });

                                            TaxChain.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "business_date",
                                                DataType = typeof(string)
                                            });
                                            foreach (DataRow vrow in TaxChain.Rows)
                                            {

                                                vrow["STLD_Location"] = lbl_location.Text;
                                                vrow["business_date"] = lbl_business.Text;

                                            }
                                            bc.DestinationTableName = "TaxChain";
                                            bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            bc.ColumnMappings.Add("business_date", "business_date");
                                            bc.ColumnMappings.Add("id", "id");
                                            bc.ColumnMappings.Add("name", "name");
                                            bc.ColumnMappings.Add("rate", "rate");
                                            bc.ColumnMappings.Add("baseAmount", "baseAmount");
                                            bc.ColumnMappings.Add("amount", "amount");
                                            bc.ColumnMappings.Add("BDBaseAmount", "BDBaseAmount");
                                            bc.ColumnMappings.Add("BDAmount", "BDAmount");
                                            bc.ColumnMappings.Add("Item_Id", "Item_Id");
                                            // bc.ColumnMappings.Add("BPBaseAmount", "BPBaseAmount");
                                            //bc.ColumnMappings.Add("BPAmount", "BPAmount");
                                            bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                            bc.WriteToServer(TaxChain);

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {

                                            DataTable Promotions = ds.Tables["Promotions"];
                                            if (Promotions != null)
                                            {
                                                Promotions.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Promotions.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Promotions.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }

                                                bc.DestinationTableName = "Promotions";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("Promotions_Id", "Promotions_Id");
                                                bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                bc.WriteToServer(Promotions);
                                            }
                                        }


                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {

                                            DataTable Promotion = ds.Tables["Promotion"];
                                            if (Promotion != null)
                                            {


                                                Promotion.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Promotion.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Promotion.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }

                                                bc.DestinationTableName = "Promotion";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("promotionId", "promotionId");
                                                bc.ColumnMappings.Add("promotionName", "promotionName");
                                                bc.ColumnMappings.Add("promotionCounter", "promotionCounter");
                                                bc.ColumnMappings.Add("discountType", "discountType");
                                                bc.ColumnMappings.Add("discountAmount", "discountAmount");
                                                bc.ColumnMappings.Add("offerId", "offerId");
                                                bc.ColumnMappings.Add("exclusive", "exclusive");
                                                bc.ColumnMappings.Add("promotionOnTender", "promotionOnTender");
                                                bc.ColumnMappings.Add("countTowardsPromotionLimit", "countTowardsPromotionLimit");
                                                bc.ColumnMappings.Add("returnedValue", "returnedValue");
                                                bc.ColumnMappings.Add("Promotions_Id", "Promotions_Id");
                                                bc.WriteToServer(Promotion);

                                            }
                                        }


                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {

                                            DataTable Customer = ds.Tables["Customer"];
                                            if (Customer != null)
                                            {
                                                Customer.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Customer.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Customer.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }


                                                bc.DestinationTableName = "Customer";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("id", "id");
                                                bc.ColumnMappings.Add("nickname", "nickname");
                                                bc.ColumnMappings.Add("greeting", "greeting");
                                                bc.ColumnMappings.Add("loyaltyCardId", "loyaltyCardId");
                                                bc.ColumnMappings.Add("loyaltyCardType", "loyaltyCardType");
                                                bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                bc.WriteToServer(Customer);
                                            }
                                        }


                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {

                                            DataTable CustomInfo = ds.Tables["CustomInfo"];
                                            if (CustomInfo != null)
                                            {
                                                CustomInfo.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                CustomInfo.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in CustomInfo.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "CustomInfo";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("CustomInfo_Id", "CustomInfo_Id");
                                                bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                bc.WriteToServer(CustomInfo);


                                            }
                                        }


                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {

                                            DataTable Tenders = ds.Tables["Tenders"];
                                            if (Tenders != null)
                                            {
                                                Tenders.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Tenders.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Tenders.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Tenders";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("Tenders_Id", "Tenders_Id");
                                                bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                bc.WriteToServer(Tenders);
                                            }
                                        }

                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {

                                            DataTable Tender = ds.Tables["Tender"];
                                            if (Tender != null)
                                            {
                                                Tender.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Tender.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Tender.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Tender";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("TenderId", "TenderId");
                                                bc.ColumnMappings.Add("TenderKind", "TenderKind");
                                                bc.ColumnMappings.Add("TenderName", "TenderName");
                                                bc.ColumnMappings.Add("TenderQuantity", "TenderQuantity");
                                                bc.ColumnMappings.Add("FaceValue", "FaceValue");
                                                bc.ColumnMappings.Add("TenderAmount", "TenderAmount");
                                                bc.ColumnMappings.Add("BaseAction", "BaseAction");
                                                bc.ColumnMappings.Add("Persisted", "Persisted");
                                                bc.ColumnMappings.Add("CardProviderID", "CardProviderID");
                                                bc.ColumnMappings.Add("CashlessData", "CashlessData");
                                                bc.ColumnMappings.Add("TaxOption", "TaxOption");
                                                bc.ColumnMappings.Add("SubtotalOption", "SubtotalOption");
                                                bc.ColumnMappings.Add("ForeignCurrencyIndicator", "ForeignCurrencyIndicator");
                                                bc.ColumnMappings.Add("DiscountDescription", "DiscountDescription");
                                                bc.ColumnMappings.Add("CashlessTransactionID", "CashlessTransactionID");
                                                bc.ColumnMappings.Add("PaymentChannel", "PaymentChannel");
                                                bc.ColumnMappings.Add("Tenders_Id", "Tenders_Id");
                                                bc.WriteToServer(Tender);

                                            }
                                        }

                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable POSTiming = ds.Tables["POSTiming"];
                                            if (POSTiming != null)
                                            {
                                                POSTiming.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                POSTiming.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in POSTiming.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }

                                            }


                                        }

                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable EventsDetail = ds.Tables["EventsDetail"];
                                            if (EventsDetail != null)
                                            {
                                                EventsDetail.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                EventsDetail.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in EventsDetail.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "EventsDetail";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("EventsDetail_Id", "EventsDetail_Id");
                                                bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                bc.WriteToServer(EventsDetail);

                                            }


                                        }

                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable SaleEvent = ds.Tables["SaleEvent"];
                                            if (SaleEvent != null)
                                            {
                                                SaleEvent.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                SaleEvent.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in SaleEvent.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                //bc.DestinationTableName = "SaleEvent";
                                                //bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                //bc.ColumnMappings.Add("business_date", "business_date");
                                                //bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                //bc.ColumnMappings.Add("Ev_SaleStored", "Ev_SaleStored");
                                                //bc.ColumnMappings.Add("Ev_SaleRecalled", "Ev_SaleRecalled");
                                                //bc.ColumnMappings.Add("Ev_BackFromTotal", "Ev_BackFromTotal");
                                                //bc.ColumnMappings.Add("Ev_SaleTotal", "Ev_SaleTotal");
                                                //bc.ColumnMappings.Add("Type", "Type");
                                                //bc.ColumnMappings.Add("Time", "Time");
                                                //bc.ColumnMappings.Add("EventsDetail_Id", "EventsDetail_Id");
                                                //bc.WriteToServer(SaleEvent);

                                            }

                                        }

                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Ev_SaleIncrementItemQty = ds.Tables["Ev_SaleIncrementItemQty"];
                                            if (Ev_SaleIncrementItemQty != null)
                                            {
                                                Ev_SaleIncrementItemQty.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Ev_SaleIncrementItemQty.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Ev_SaleIncrementItemQty.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Ev_SaleIncrementItemQty";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("SaleIndex", "SaleIndex");
                                                bc.ColumnMappings.Add("Quantity", "Quantity");
                                                bc.ColumnMappings.Add("ProductCode", "ProductCode");
                                                bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                bc.WriteToServer(Ev_SaleIncrementItemQty);

                                            }

                                        }

                                        using (SqlBulkCopy bc = new SqlBulkCopy(con))
                                        {


                                            DataTable TRX_GetAuthorization = ds.Tables["TRX_GetAuthorization"];
                                            if (TRX_GetAuthorization != null)
                                            {
                                                TRX_GetAuthorization.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TRX_GetAuthorization.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TRX_GetAuthorization.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                //bc.DestinationTableName = "TRX_GetAuthorization";
                                                //bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                //bc.ColumnMappings.Add("business_date", "business_date");
                                                //bc.ColumnMappings.Add("Action", "Action");
                                                //bc.ColumnMappings.Add("ManagerID", "ManagerID");
                                                //bc.ColumnMappings.Add("ManagerName", "ManagerName");
                                                //bc.ColumnMappings.Add("SecurityLevel", "SecurityLevel");
                                                //bc.ColumnMappings.Add("ExpirationDate", "ExpirationDate");
                                                //bc.ColumnMappings.Add("Password", "Password");
                                                //bc.ColumnMappings.Add("Islogged", "Islogged");
                                                //bc.ColumnMappings.Add("Method", "Method");
                                                ////bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                //bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                //bc.WriteToServer(TRX_GetAuthorization);

                                            }

                                        }

                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable EV_NotChargedPromotional = ds.Tables["EV_NotChargedPromotional"];
                                            if (EV_NotChargedPromotional != null)
                                            {
                                                EV_NotChargedPromotional.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                EV_NotChargedPromotional.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in EV_NotChargedPromotional.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "EV_NotChargedPromotional";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("ProductCode", "ProductCode");
                                                bc.ColumnMappings.Add("Quantity", "Quantity");
                                                bc.ColumnMappings.Add("NotChargedValue", "NotChargedValue");
                                                bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                bc.WriteToServer(EV_NotChargedPromotional);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Ev_AddTender = ds.Tables["Ev_AddTender"];
                                            if (Ev_AddTender != null)
                                            {
                                                Ev_AddTender.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Ev_AddTender.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Ev_AddTender.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Ev_AddTender";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("TenderId", "TenderId");
                                                bc.ColumnMappings.Add("FaceValue", "FaceValue");
                                                bc.ColumnMappings.Add("TenderAmount", "TenderAmount");
                                                bc.ColumnMappings.Add("BaseAction", "BaseAction");
                                                bc.ColumnMappings.Add("Persisted", "Persisted");
                                                bc.ColumnMappings.Add("CardProviderID", "CardProviderID");
                                                bc.ColumnMappings.Add("CashlessData", "CashlessData");
                                                bc.ColumnMappings.Add("CashlessTransactionID", "CashlessTransactionID");
                                                bc.ColumnMappings.Add("PreAuthorization", "PreAuthorization");
                                                bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                bc.WriteToServer(Ev_AddTender);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Ev_SetSaleType = ds.Tables["Ev_SetSaleType"];
                                            if (Ev_SetSaleType != null)
                                            {
                                                Ev_SetSaleType.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Ev_SetSaleType.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Ev_SetSaleType.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Ev_SetSaleType";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("Type", "Type");
                                                bc.ColumnMappings.Add("ForceExhibition", "ForceExhibition");
                                                bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                bc.WriteToServer(Ev_SetSaleType);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Ev_SaleChoice = ds.Tables["Ev_SaleChoice"];
                                            if (Ev_SaleChoice != null)
                                            {
                                                Ev_SaleChoice.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Ev_SaleChoice.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Ev_SaleChoice.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Ev_SaleChoice";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("ProductCode", "ProductCode");
                                                bc.ColumnMappings.Add("ChoiceCode", "ChoiceCode");
                                                bc.ColumnMappings.Add("Quantity", "Quantity");
                                                bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                bc.WriteToServer(Ev_SaleChoice);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Ev_SaleItem = ds.Tables["Ev_SaleItem"];
                                            if (Ev_SaleItem != null)
                                            {
                                                Ev_SaleItem.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Ev_SaleItem.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Ev_SaleItem.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Ev_SaleItem";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("ProductCode", "ProductCode");
                                                bc.ColumnMappings.Add("Quantity", "Quantity");
                                                bc.ColumnMappings.Add("UpdatedQuantity", "UpdatedQuantity");
                                                bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                bc.WriteToServer(Ev_SaleItem);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Ev_SaleCutomInfo = ds.Tables["Ev_SaleCutomInfo"];
                                            if (Ev_SaleCutomInfo != null)
                                            {
                                                Ev_SaleCutomInfo.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Ev_SaleCutomInfo.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Ev_SaleCutomInfo.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                //bc.DestinationTableName = "Ev_SaleCutomInfo";
                                                //bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                //bc.ColumnMappings.Add("business_date", "business_date");
                                                //bc.ColumnMappings.Add("ProductCode", "ProductCode");
                                                //bc.ColumnMappings.Add("Quantity", "Quantity");
                                                //bc.ColumnMappings.Add("UpdatedQuantity", "UpdatedQuantity");
                                                //bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                //bc.WriteToServer(Ev_SaleCutomInfo);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Ev_SaleEnd = ds.Tables["Ev_SaleEnd"];
                                            if (Ev_SaleEnd != null)
                                            {
                                                Ev_SaleEnd.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Ev_SaleEnd.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Ev_SaleEnd.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Ev_SaleEnd";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("Type", "Type");
                                                bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                bc.WriteToServer(Ev_SaleEnd);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Ev_SaleStart = ds.Tables["Ev_SaleStart"];
                                            if (Ev_SaleStart != null)
                                            {
                                                Ev_SaleStart.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Ev_SaleStart.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Ev_SaleStart.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Ev_SaleStart";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("DisabledChoices", "DisabledChoices");
                                                bc.ColumnMappings.Add("TenderPersisted", "TenderPersisted");
                                                bc.ColumnMappings.Add("Multiorder", "Multiorder");
                                                bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                bc.WriteToServer(Ev_SaleStart);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Fiscal_Information = ds.Tables["Fiscal_Information"];
                                            if (Fiscal_Information != null)
                                            {
                                                Fiscal_Information.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Fiscal_Information.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Fiscal_Information.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Fiscal_Information";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("TIN", "TIN");
                                                bc.ColumnMappings.Add("name", "name");
                                                bc.ColumnMappings.Add("address", "address");
                                                bc.ColumnMappings.Add("ZIP", "ZIP");
                                                bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                bc.WriteToServer(Fiscal_Information);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Ev_DrawerClose = ds.Tables["Ev_DrawerClose"];
                                            if (Ev_DrawerClose != null)
                                            {
                                                Ev_DrawerClose.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Ev_DrawerClose.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Ev_DrawerClose.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Ev_DrawerClose";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("TotalOpenTime", "TotalOpenTime");
                                                bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                bc.WriteToServer(Ev_DrawerClose);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable TRX_OperLogin = ds.Tables["TRX_OperLogin"];
                                            if (TRX_OperLogin != null)
                                            {
                                                TRX_OperLogin.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TRX_OperLogin.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TRX_OperLogin.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TRX_OperLogin";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("CrewID", "CrewID");
                                                bc.ColumnMappings.Add("CrewName", "CrewName");
                                                bc.ColumnMappings.Add("CrewSecurityLevel", "CrewSecurityLevel");
                                                bc.ColumnMappings.Add("POD", "POD");
                                                bc.ColumnMappings.Add("RemotePOD", "RemotePOD");
                                                bc.ColumnMappings.Add("AutoLogin", "AutoLogin");
                                                bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                bc.WriteToServer(TRX_OperLogin);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable TRX_SetPOD = ds.Tables["TRX_SetPOD"];
                                            if (TRX_SetPOD != null)
                                            {
                                                TRX_SetPOD.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TRX_SetPOD.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TRX_SetPOD.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TRX_SetPOD";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("PODId", "PODId");
                                                bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                bc.WriteToServer(TRX_SetPOD);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable TRX_DayOpen = ds.Tables["TRX_DayOpen"];
                                            if (TRX_DayOpen != null)
                                            {
                                                TRX_DayOpen.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TRX_DayOpen.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TRX_DayOpen.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TRX_DayOpen";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("BusinessDate", "BusinessDate");
                                                bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                bc.WriteToServer(TRX_DayOpen);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable TRX_TaxTable = ds.Tables["TRX_TaxTable"];
                                            if (TRX_TaxTable != null)
                                            {
                                                TRX_TaxTable.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TRX_TaxTable.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TRX_TaxTable.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TRX_TaxTable";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("TRX_TaxTable_Id", "TRX_TaxTable_Id");
                                                bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                bc.WriteToServer(TRX_TaxTable);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable TaxType = ds.Tables["TaxType"];
                                            if (TaxType != null)
                                            {
                                                TaxType.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TaxType.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TaxType.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TaxType";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("TaxId", "TaxId");
                                                bc.ColumnMappings.Add("TaxDescription", "TaxDescription");
                                                bc.ColumnMappings.Add("TaxRate", "TaxRate");
                                                bc.ColumnMappings.Add("TaxBasis", "TaxBasis");
                                                bc.ColumnMappings.Add("TaxCalcType", "TaxCalcType");
                                                bc.ColumnMappings.Add("Rounding", "Rounding");
                                                bc.ColumnMappings.Add("Precision", "Precision");
                                                bc.ColumnMappings.Add("TRX_TaxTable_Id", "TRX_TaxTable_Id");
                                                bc.WriteToServer(TaxType);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable TRX_TenderTable = ds.Tables["TRX_TenderTable"];
                                            if (TRX_TenderTable != null)
                                            {
                                                TRX_TenderTable.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TRX_TenderTable.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TRX_TenderTable.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TRX_TenderTable";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("TRX_TenderTable_Id", "TRX_TenderTable_Id");
                                                bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                bc.WriteToServer(TRX_TenderTable);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable TenderType = ds.Tables["TenderType"];
                                            if (TenderType != null)
                                            {
                                                TenderType.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TenderType.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TenderType.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TenderType";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("TenderId", "TenderId");
                                                bc.ColumnMappings.Add("TenderFiscalIndex", "TenderFiscalIndex");
                                                bc.ColumnMappings.Add("TenderName", "TenderName");
                                                bc.ColumnMappings.Add("TenderCategory", "TenderCategory");
                                                bc.ColumnMappings.Add("Taxoption", "Taxoption");
                                                bc.ColumnMappings.Add("DefaultSkimLimit", "DefaultSkimLimit");
                                                bc.ColumnMappings.Add("DefaultHaloLimit", "DefaultHaloLimit");
                                                bc.ColumnMappings.Add("SubtotalOption", "SubtotalOption");
                                                bc.ColumnMappings.Add("CurrencyDecimals", "CurrencyDecimals");
                                                bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                bc.ColumnMappings.Add("TRX_TenderTable_Id", "TRX_TenderTable_Id");
                                                bc.WriteToServer(TenderType);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable TenderFlags = ds.Tables["TenderFlags"];
                                            if (TenderFlags != null)
                                            {
                                                TenderFlags.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TenderFlags.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TenderFlags.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TenderFlags";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                bc.ColumnMappings.Add("TenderFlags_Id", "TenderFlags_Id");
                                                bc.WriteToServer(TenderFlags);

                                            }

                                        }

                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Info = ds.Tables["Info"];
                                            if (Info != null)
                                            {
                                                Info.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Info.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Info.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Info";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("name", "name");
                                                bc.ColumnMappings.Add("value", "value");
                                                bc.ColumnMappings.Add("CustomInfo_Id", "CustomInfo_Id");
                                                bc.WriteToServer(Info);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con))
                                        {


                                            DataTable TenderFlag = ds.Tables["TenderFlag"];
                                            if (TenderFlag != null)
                                            {
                                                // TenderFlag.Columns.Add(new DataColumn()
                                                //  {
                                                //      ColumnName = "STLD_Location",
                                                //      DataType = typeof(string)
                                                //  });

                                                //  TenderFlag.Columns.Add(new DataColumn()
                                                //  {
                                                //      ColumnName = "business_date",
                                                //      DataType = typeof(string)
                                                //  });
                                                //foreach (DataRow vrow in TenderFlag.Rows)
                                                // {

                                                //     vrow["STLD_Location"] = lbl_location.Text;
                                                //     vrow["business_date"] = lbl_business.Text;

                                                // }
                                                //  bc.DestinationTableName = "TenderFlag";
                                                //  bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                //  bc.ColumnMappings.Add("business_date", "business_date");
                                                //   bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                //   bc.ColumnMappings.Add("TenderFlags_Id", "TenderFlags_Id");
                                                //   bc.WriteToServer(TenderFlag);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable TenderChange = ds.Tables["TenderChange"];
                                            if (TenderChange != null)
                                            {
                                                TenderChange.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TenderChange.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TenderChange.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TenderChange";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("id", "id");
                                                bc.ColumnMappings.Add("type", "type");
                                                bc.ColumnMappings.Add("roundToMinAmount", "roundToMinAmount");
                                                bc.ColumnMappings.Add("maxAllowed", "maxAllowed");
                                                bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                bc.WriteToServer(TenderChange);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable GiftCoupon = ds.Tables["GiftCoupon"];
                                            if (GiftCoupon != null)
                                            {
                                                GiftCoupon.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                GiftCoupon.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in GiftCoupon.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "GiftCoupon";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("LegacyId", "LegacyId");
                                                bc.ColumnMappings.Add("OperatorDefined", "OperatorDefined");
                                                bc.ColumnMappings.Add("Amount", "Amount");
                                                bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                bc.WriteToServer(GiftCoupon);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable OtherPayments = ds.Tables["OtherPayments"];
                                            if (OtherPayments != null)
                                            {
                                                //OtherPayments.Columns.Add(new DataColumn()
                                                //{
                                                //    ColumnName = "STLD_Location",
                                                //    DataType = typeof(string)
                                                //});

                                                //OtherPayments.Columns.Add(new DataColumn()
                                                //{
                                                //    ColumnName = "business_date",
                                                //    DataType = typeof(string)
                                                //});
                                                //foreach (DataRow vrow in OtherPayments.Rows)
                                                //{

                                                //    vrow["STLD_Location"] = lbl_location.Text;
                                                //    vrow["business_date"] = lbl_business.Text;

                                                //}
                                                //bc.DestinationTableName = "OtherPayments";
                                                //bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                //bc.ColumnMappings.Add("business_date", "business_date");
                                                //bc.ColumnMappings.Add("LegacyId", "LegacyId");
                                                //bc.ColumnMappings.Add("OperatorDefined", "OperatorDefined");
                                                //bc.ColumnMappings.Add("Amount", "Amount");
                                                //bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                //bc.WriteToServer(OtherPayments);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable ForeignCurrency = ds.Tables["ForeignCurrency"];
                                            if (ForeignCurrency != null)
                                            {
                                                ForeignCurrency.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                ForeignCurrency.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in ForeignCurrency.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "ForeignCurrency";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("LegacyId", "LegacyId");
                                                bc.ColumnMappings.Add("ExchangeRate", "ExchangeRate");
                                                bc.ColumnMappings.Add("Precision", "Precision");
                                                bc.ColumnMappings.Add("Rounding", "Rounding");
                                                bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                bc.WriteToServer(ForeignCurrency);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable ElectronicPayment = ds.Tables["ElectronicPayment"];
                                            if (ElectronicPayment != null)
                                            {
                                                ElectronicPayment.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                ElectronicPayment.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in ElectronicPayment.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "ElectronicPayment";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("LegacyId", "LegacyId");
                                                bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                bc.WriteToServer(ElectronicPayment);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable CreditSales = ds.Tables["CreditSales"];
                                            if (CreditSales != null)
                                            {
                                                CreditSales.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                CreditSales.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in CreditSales.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "CreditSales";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("LegacyId", "LegacyId");
                                                bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                bc.WriteToServer(CreditSales);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Trx_DayPart = ds.Tables["Trx_DayPart"];
                                            if (Trx_DayPart != null)
                                            {
                                                Trx_DayPart.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Trx_DayPart.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Trx_DayPart.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                //bc.DestinationTableName = "Trx_DayPart";
                                                //bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                //bc.ColumnMappings.Add("business_date", "business_date");
                                                //bc.ColumnMappings.Add("LegacyId", "LegacyId");
                                                //bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                //bc.WriteToServer(Trx_DayPart);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable TRX_BaseConfig = ds.Tables["TRX_BaseConfig"];
                                            if (TRX_BaseConfig != null)
                                            {
                                                TRX_BaseConfig.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TRX_BaseConfig.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TRX_BaseConfig.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TRX_BaseConfig";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("TRX_BaseConfig_Id", "TRX_BaseConfig_Id");
                                                bc.ColumnMappings.Add("POS", "POS");
                                                bc.ColumnMappings.Add("POD", "POD");
                                                bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                bc.WriteToServer(TRX_BaseConfig);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable Config = ds.Tables["Config"];
                                            if (Config != null)
                                            {
                                                Config.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                Config.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in Config.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "Config";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("MenuPriceBasis", "MenuPriceBasis");
                                                bc.ColumnMappings.Add("WeekEndBreakfastStartTime", "WeekEndBreakfastStartTime");
                                                bc.ColumnMappings.Add("WeekEndBreakfastStopTime", "WeekEndBreakfastStopTime");
                                                bc.ColumnMappings.Add("WeekDayBreakfastStartTime", "WeekDayBreakfastStartTime");
                                                bc.ColumnMappings.Add("WeekDayBreakfastStopTime", "WeekDayBreakfastStopTime");
                                                bc.ColumnMappings.Add("DecimalPlaces", "DecimalPlaces");
                                                bc.ColumnMappings.Add("CheckRefund", "CheckRefund");
                                                bc.ColumnMappings.Add("GrandTotalFlag", "GrandTotalFlag");
                                                bc.ColumnMappings.Add("StoreId", "StoreId");
                                                bc.ColumnMappings.Add("StoreName", "StoreName");
                                                bc.ColumnMappings.Add("AcceptNegativeQty", "AcceptNegativeQty");
                                                bc.ColumnMappings.Add("AcceptZeroPricePMix", "AcceptZeroPricePMix");
                                                bc.ColumnMappings.Add("FloatPriceTenderId", "FloatPriceTenderId");
                                                bc.ColumnMappings.Add("MinCirculatingAmount", "MinCirculatingAmount");
                                                bc.ColumnMappings.Add("TRX_BaseConfig_Id", "TRX_BaseConfig_Id");
                                                bc.WriteToServer(Config);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable POSConfig = ds.Tables["POSConfig"];
                                            if (POSConfig != null)
                                            {
                                                POSConfig.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                POSConfig.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in POSConfig.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "POSConfig";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("CountTCsFullDiscEM", "CountTCsFullDiscEM");
                                                bc.ColumnMappings.Add("RefundBehaviour", "RefundBehaviour");
                                                bc.ColumnMappings.Add("OverringBehaviour", "OverringBehaviour");
                                                bc.ColumnMappings.Add("TRX_BaseConfig_Id", "TRX_BaseConfig_Id");
                                                bc.WriteToServer(POSConfig);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable TRX_SetSMState = ds.Tables["TRX_SetSMState"];
                                            if (TRX_SetSMState != null)
                                            {
                                                TRX_SetSMState.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TRX_SetSMState.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TRX_SetSMState.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TRX_SetSMState";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("POSState", "POSState");
                                                bc.ColumnMappings.Add("CrewId", "CrewId");
                                                bc.ColumnMappings.Add("CrewName", "CrewName");
                                                bc.ColumnMappings.Add("CrewSecurityLevel", "CrewSecurityLevel");
                                                bc.ColumnMappings.Add("LoginTime", "LoginTime");
                                                bc.ColumnMappings.Add("LogoutTime", "LogoutTime");
                                                bc.ColumnMappings.Add("InitialFloat", "InitialFloat");
                                                bc.ColumnMappings.Add("PODId", "PODId");
                                                bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                bc.WriteToServer(TRX_SetSMState);

                                            }

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {


                                            DataTable TRX_InitGTotal = ds.Tables["TRX_InitGTotal"];
                                            if (TRX_InitGTotal != null)
                                            {
                                                TRX_InitGTotal.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "STLD_Location",
                                                    DataType = typeof(string)
                                                });

                                                TRX_InitGTotal.Columns.Add(new DataColumn()
                                                {
                                                    ColumnName = "business_date",
                                                    DataType = typeof(string)
                                                });
                                                foreach (DataRow vrow in TRX_InitGTotal.Rows)
                                                {

                                                    vrow["STLD_Location"] = lbl_location.Text;
                                                    vrow["business_date"] = lbl_business.Text;

                                                }
                                                bc.DestinationTableName = "TRX_InitGTotal";
                                                bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                bc.ColumnMappings.Add("business_date", "business_date");
                                                bc.ColumnMappings.Add("amount", "amount");
                                                bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                bc.WriteToServer(TRX_InitGTotal);

                                            }

                                        }



                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {
                                            DataTable TLD = ds.Tables["TLD"];
                                            bc.DestinationTableName = "TLD";
                                            bc.ColumnMappings.Add("TLD_Id", "TLD_Id");
                                            bc.ColumnMappings.Add("LogVersion", "LogVersion");
                                            bc.ColumnMappings.Add("storeId", "storeId");
                                            bc.ColumnMappings.Add("businessDate", "businessDate");
                                            bc.ColumnMappings.Add("swVersion", "swVersion");
                                            bc.ColumnMappings.Add("checkPoint", "checkPoint");
                                            bc.ColumnMappings.Add("end", "end");
                                            bc.ColumnMappings.Add("productionStatus", "productionStatus");
                                            bc.ColumnMappings.Add("hasMoreContent", "hasMoreContent");
                                            bc.WriteToServer(TLD);

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {
                                            DataTable Node = ds.Tables["Node"];
                                            Node.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "STLD_Location",
                                                DataType = typeof(string)
                                            });

                                            Node.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "business_date",
                                                DataType = typeof(string)
                                            });
                                            foreach (DataRow vrow in Node.Rows)
                                            {

                                                vrow["STLD_Location"] = lbl_location.Text;
                                                vrow["business_date"] = lbl_business.Text;

                                            }
                                            bc.DestinationTableName = "Node";
                                            bc.ColumnMappings.Add("Node_Id", "Node_Id");
                                            bc.ColumnMappings.Add("TLD_Id", "TLD_Id");
                                            bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            bc.ColumnMappings.Add("business_date", "business_date");
                                            bc.ColumnMappings.Add("nodeStatus", "nodeStatus");
                                            bc.ColumnMappings.Add("id", "id");
                                            bc.WriteToServer(Node);

                                        }
                                        using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                        {
                                            DataTable Event = ds.Tables["Event"];
                                            Event.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "STLD_Location",
                                                DataType = typeof(string)
                                            });

                                            Event.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "business_date",
                                                DataType = typeof(string)
                                            });
                                            foreach (DataRow vrow in Event.Rows)
                                            {

                                                vrow["STLD_Location"] = lbl_location.Text;
                                                vrow["business_date"] = lbl_business.Text;

                                            }
                                            bc.DestinationTableName = "Event";
                                            bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            bc.ColumnMappings.Add("business_date", "business_date");
                                            bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                            //  bc.ColumnMappings.Add("TRX_UnaDrawerOpening", "TRX_UnaDrawerOpening");
                                            bc.ColumnMappings.Add("RegId", "RegId");
                                            bc.ColumnMappings.Add("Type", "Type");
                                            bc.ColumnMappings.Add("Time", "Time");
                                            bc.ColumnMappings.Add("Node_Id", "Node_Id");
                                            bc.WriteToServer(Event);

                                        }





                                        string smt = "INSERT INTO [STLD].[dbo].[STLDPROCESS_STATUS] ([STLD_Location] ,[business_date]) VALUES (@STLD_Location,@business_date)";
                                        SqlCommand cmd = new SqlCommand(smt, con, tran);
                                        cmd.Parameters.AddWithValue("@STLD_Location", SqlDbType.VarChar).Value = lbl_location.Text;
                                        cmd.Parameters.AddWithValue("@business_date", SqlDbType.VarChar).Value = lbl_business.Text;
                                        cmd.ExecuteNonQuery();
                                        File.Move(file, Path.ChangeExtension(file, ".Proceed"));

                                        //////////////////////

                                        tran.Commit();


                                    }
                                    catch
                                    {
                                        tran.Rollback();
                                        throw;
                                    }

                                }

                            }









                        }
                    }

                    //////////////////////////////////////////////////////////////////

                }
                else
                {
                    //File.Delete(file);
                }


            }

        }

        private void BTNPRoductDb_Click(object sender, EventArgs e)
        {
            foreach (string file in Directory.EnumerateFiles("E:\\ImpotSheiV2\\Configfiles", "*.xml"))
            {
                string contents = File.ReadAllText(file);
                ///////////////////// File Reading/////////////////////////

                lbl_nameval.Text = Convert.ToString(file.ToString());
                if (lbl_nameval.Text.Contains("product-db"))
                {

                    string connString = SQLCON.ConnectionString2;
                    lbl_STLD.Text = file;

                    SQLCON vb = new SQLCON();
                    vb.OpenConnection();
                    DataSet ds = new DataSet();
                    ds.ReadXml(lbl_STLD.Text);

                }
            }
        }

        private void Btn_Convert_Click(object sender, EventArgs e)
        {
            button5.PerformClick();
            btn_process.PerformClick();
            button1.PerformClick();
            button4.PerformClick();
            string message = "Process Compleated";
            MessageBox.Show(message);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Form1_Load(1,null);
            try
            {


                BTNTIMERNETWORK.PerformClick();

                btn_process.PerformClick();
                button1.PerformClick();
                button4.PerformClick();
                btn_renamepmix.PerformClick();
                btn_way_pmix.PerformClick();


            }

            catch
            {
            }

        }

        private void BTNTIMERNETWORK_Click(object sender, EventArgs e)
        {
            BTN_CLEARLBLES.PerformClick();
            Btn_rename.PerformClick();
            btn_renamepmix.PerformClick();
            btn_way_pmix.PerformClick();
            button3.PerformClick();

        }

        private void BTN_PMIX_Click(object sender, EventArgs e)
        {
            foreach (string file in Directory.EnumerateFiles("E:\\ImpotSheiV2\\Extract\\Processingfiles", "*.xml"))
            {
                string contents = File.ReadAllText(file);
                ///////////////////// File Reading/////////////////////////

                pmix_file.Text = Convert.ToString(file.ToString());
                if (pmix_file.Text.Contains("PmixReport"))
                {

                    string connString = SQLCON.ConnectionString2;
                    lbl_stld_pmix.Text = file;

                    SQLCON vb = new SQLCON();
                    vb.OpenConnection();
                    DataSet ds = new DataSet();
                    ds.ReadXml(lbl_stld_pmix.Text);

                    DataTable CHKTLD = ds.Tables["Response"];
                    foreach (DataRow row in CHKTLD.Rows)
                    {
                        // ... Write value of first field as integer.
                        string storloc1 = row.ItemArray[3].ToString();
                        string storbusinessdate1 = row.ItemArray[9].ToString();

                        pmix_location.Text = storloc1;
                        pmix_businessdate.Text = storbusinessdate1;
                    }

                    using (SqlConnection conchk = new SqlConnection(connString))
                    {

                        conchk.Open();

                        SqlCommand comm = new SqlCommand("SELECT COUNT(*) FROM [PMIXPROCESS_STATUS] where [STLD_Location]=@STLD_Location and business_date=@business_date", conchk);
                        comm.Parameters.AddWithValue("@STLD_Location", SqlDbType.VarChar).Value = pmix_location.Text;
                        comm.Parameters.AddWithValue("@business_date", SqlDbType.VarChar).Value = pmix_businessdate.Text;
                        Int32 count = Convert.ToInt32(comm.ExecuteScalar());

                        if (count > 0)
                        {
                            pmixcount.Text = Convert.ToString(count.ToString()); //For example a Label
                        }
                        else
                        {
                            pmixcount.Text = "0";

                            SqlCommand comm2 = new SqlCommand("SELECT [StoreID]   FROM [STLD].[dbo].[TB_STOREMASTER] where [StoreID]=@STLD_Location", conchk);
                            comm2.Parameters.AddWithValue("@STLD_Location", SqlDbType.VarChar).Value = pmix_location.Text;
                            Int32 count2 = Convert.ToInt32(comm2.ExecuteScalar());

                            if (count2 > 0)
                            {
                                pmixcount.Text = "0"; //For example a Label
                            }
                            else
                            {
                                pmixcount.Text = "wrong";
                            }
                        }

                        conchk.Close();
                    }
                    if (pmixcount.Text.Contains('0'))
                    {
                        using (SqlConnection conp = new SqlConnection(connString))
                        {
                            conp.Open();
                            using (var connection = conp)
                            {
                                using (var tran2 = conp.BeginTransaction())
                                {

                                    try
                                    {

                                        using (SqlBulkCopy bc = new SqlBulkCopy(conp, SqlBulkCopyOptions.Default, tran2))
                                        {
                                            DataTable ProductInfo = ds.Tables["ProductInfo"];

                                            ProductInfo.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "STLD_Location",
                                                DataType = typeof(string)
                                            });

                                            ProductInfo.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "business_date",
                                                DataType = typeof(string)
                                            });
                                            foreach (DataRow vrow in ProductInfo.Rows)
                                            {

                                                vrow["STLD_Location"] = pmix_location.Text;
                                                vrow["business_date"] = pmix_businessdate.Text;

                                            }

                                            bc.DestinationTableName = "PMIXPRODUCTINFO";
                                            bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            bc.ColumnMappings.Add("business_date", "business_date");
                                            bc.ColumnMappings.Add("id", "id");
                                            bc.ColumnMappings.Add("name", "name");
                                            bc.ColumnMappings.Add("familyGroup", "familyGroup");
                                            bc.ColumnMappings.Add("class", "class");
                                            bc.ColumnMappings.Add("department", "department");
                                            bc.ColumnMappings.Add("ProductTable_Id", "ProductTable_Id");
                                            bc.WriteToServer(ProductInfo);
                                        }


                                        using (SqlBulkCopy bc = new SqlBulkCopy(conp, SqlBulkCopyOptions.Default, tran2))
                                        {
                                            DataTable FamilyGroup = ds.Tables["FamilyGroup"];

                                            FamilyGroup.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "STLD_Location",
                                                DataType = typeof(string)
                                            });

                                            FamilyGroup.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "business_date",
                                                DataType = typeof(string)
                                            });
                                            foreach (DataRow vrow in FamilyGroup.Rows)
                                            {

                                                vrow["STLD_Location"] = pmix_location.Text;
                                                vrow["business_date"] = pmix_businessdate.Text;

                                            }

                                            bc.DestinationTableName = "PMIXFamilyGroup";
                                            bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            bc.ColumnMappings.Add("business_date", "business_date");
                                            bc.ColumnMappings.Add("FamilyGroup_Id", "FamilyGroup_Id");
                                            bc.ColumnMappings.Add("groupCode", "groupCode");
                                            bc.ColumnMappings.Add("groupName", "groupName");
                                            // bc.ColumnMappings.Add("familyGroup", "familyGroup");
                                            bc.ColumnMappings.Add("POS_Id", "POS_Id");
                                            bc.ColumnMappings.Add("Response_Id", "Response_Id");
                                            bc.WriteToServer(FamilyGroup);

                                        }


                                        using (SqlBulkCopy bc = new SqlBulkCopy(conp, SqlBulkCopyOptions.Default, tran2))
                                        {
                                            DataTable Product = ds.Tables["Product"];

                                            Product.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "STLD_Location",
                                                DataType = typeof(string)
                                            });

                                            Product.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "business_date",
                                                DataType = typeof(string)
                                            });
                                            foreach (DataRow vrow in Product.Rows)
                                            {

                                                vrow["STLD_Location"] = pmix_location.Text;
                                                vrow["business_date"] = pmix_businessdate.Text;

                                            }

                                            bc.DestinationTableName = "PMIXProduct";
                                            bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            bc.ColumnMappings.Add("business_date", "business_date");
                                            bc.ColumnMappings.Add("Product_Id", "Product_Id");
                                            bc.ColumnMappings.Add("id", "id");
                                            bc.ColumnMappings.Add("priceType", "priceType");
                                            bc.ColumnMappings.Add("eatinPrice", "eatingPrice");
                                            bc.ColumnMappings.Add("eatinTax", "eatingTax");
                                            bc.ColumnMappings.Add("takeoutPrice", "takeoutPrice");
                                            bc.ColumnMappings.Add("takeoutTax", "takeoutTax");
                                            bc.ColumnMappings.Add("FamilyGroup_Id", "FamilyGroup_Id");
                                            if (Product.Columns.Contains("otherPrice"))
                                            {
                                                bc.ColumnMappings.Add("otherPrice", "otherPrice");
                                            }
                                            if (Product.Columns.Contains("otherTax"))
                                            {
                                                bc.ColumnMappings.Add("otherTax", "otherTax");
                                            }
                                            //bc.ColumnMappings.Add("OperatorSession_Id", "OperatorSession_Id");
                                            bc.WriteToServer(Product);

                                        }

                                        using (SqlBulkCopy bc = new SqlBulkCopy(conp, SqlBulkCopyOptions.Default, tran2))
                                        {
                                            DataTable OperationType = ds.Tables["OperationType"];

                                            OperationType.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "STLD_Location",
                                                DataType = typeof(string)
                                            });

                                            OperationType.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "business_date",
                                                DataType = typeof(string)
                                            });
                                            foreach (DataRow vrow in OperationType.Rows)
                                            {

                                                vrow["STLD_Location"] = pmix_location.Text;
                                                vrow["business_date"] = pmix_businessdate.Text;

                                            }

                                            bc.DestinationTableName = "PMIXOperationType";
                                            bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            bc.ColumnMappings.Add("business_date", "business_date");
                                            bc.ColumnMappings.Add("OperationType_Id", "OperationType_Id");
                                            bc.ColumnMappings.Add("OperationType", "OperationType");
                                            bc.ColumnMappings.Add("Product_Id", "Product_Id");
                                            bc.WriteToServer(OperationType);

                                        }




                                        using (SqlBulkCopy bc = new SqlBulkCopy(conp, SqlBulkCopyOptions.Default, tran2))
                                        {
                                            DataTable PMix = ds.Tables["PMix"];

                                            PMix.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "STLD_Location",
                                                DataType = typeof(string)
                                            });

                                            PMix.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "business_date",
                                                DataType = typeof(string)
                                            });
                                            foreach (DataRow vrow in PMix.Rows)
                                            {

                                                vrow["STLD_Location"] = pmix_location.Text;
                                                vrow["business_date"] = pmix_businessdate.Text;

                                            }

                                            bc.DestinationTableName = "PMIXPMix";
                                            bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            bc.ColumnMappings.Add("business_date", "business_date");
                                            if (PMix.Columns.Contains("qtyEatin"))
                                            {
                                                bc.ColumnMappings.Add("qtyEatin", "qtyEatin");
                                            }
                                            if (PMix.Columns.Contains("netBeforeDiscountEatin"))
                                            {
                                                bc.ColumnMappings.Add("netBeforeDiscountEatin", "netBeforeDiscountEatin");
                                            }
                                            if (PMix.Columns.Contains("taxBeforeDiscountEatin"))
                                            {
                                                bc.ColumnMappings.Add("taxBeforeDiscountEatin", "taxBeforeDiscountEatin");
                                            }
                                            if (PMix.Columns.Contains("qtyTakeOut"))
                                            {
                                                bc.ColumnMappings.Add("qtyTakeOut", "qtyTakeOut");
                                            }
                                            if (PMix.Columns.Contains("netAmtTakeOut"))
                                            {
                                                bc.ColumnMappings.Add("netAmtTakeOut", "netAmtTakeOut");
                                            }
                                            if (PMix.Columns.Contains("taxTakeOut"))
                                            {
                                                bc.ColumnMappings.Add("taxTakeOut", "taxTakeOut");
                                            }
                                            if (PMix.Columns.Contains("netBeforeDiscountTakeOut"))
                                            {
                                                bc.ColumnMappings.Add("netBeforeDiscountTakeOut", "netBeforeDiscountTakeOut");
                                            }
                                            if (PMix.Columns.Contains("taxBeforeDiscountTakeOut"))
                                            {
                                                bc.ColumnMappings.Add("taxBeforeDiscountTakeOut", "taxBeforeDiscountTakeOut");
                                            }
                                            if (PMix.Columns.Contains("netAmtEatin"))
                                            {
                                                bc.ColumnMappings.Add("netAmtEatin", "netAmtEatin");
                                            }
                                            if (PMix.Columns.Contains("taxEatIn"))
                                            {
                                                bc.ColumnMappings.Add("taxEatIn", "taxEatIn");
                                            }
                                            if (PMix.Columns.Contains("Price_Id"))
                                            {
                                                bc.ColumnMappings.Add("Price_Id", "Price_Id");
                                            }
                                            if (PMix.Columns.Contains("OperationType_Id"))
                                            {
                                                bc.ColumnMappings.Add("OperationType_Id", "OperationType_Id");
                                            }
                                            if (PMix.Columns.Contains("qtyOther"))
                                            {
                                                bc.ColumnMappings.Add("qtyOther", "qtyOther");
                                            }
                                            if (PMix.Columns.Contains("netAmtOther"))
                                            {
                                                bc.ColumnMappings.Add("netAmtOther", "netAmtOther");
                                            }
                                            if (PMix.Columns.Contains("taxOther"))
                                            {
                                                bc.ColumnMappings.Add("taxOther", "taxOther");
                                            }
                                            if (PMix.Columns.Contains("netBeforeDiscountOther"))
                                            {
                                                bc.ColumnMappings.Add("netBeforeDiscountOther", "netBeforeDiscountOther");
                                            }
                                            if (PMix.Columns.Contains("taxBeforeDiscountOther"))
                                            {
                                                bc.ColumnMappings.Add("taxBeforeDiscountOther", "taxBeforeDiscountOther");
                                            }

                                            bc.WriteToServer(PMix);

                                        }

                                        string smt = "INSERT INTO [STLD].[dbo].[PMIXPROCESS_STATUS] ([STLD_Location] ,[business_date]) VALUES (@STLD_Location,@business_date)";
                                        SqlCommand cmd = new SqlCommand(smt, conp, tran2);
                                        cmd.Parameters.AddWithValue("@STLD_Location", SqlDbType.VarChar).Value = pmix_location.Text;
                                        cmd.Parameters.AddWithValue("@business_date", SqlDbType.VarChar).Value = pmix_businessdate.Text;
                                        cmd.ExecuteNonQuery();
                                        File.Move(file, Path.ChangeExtension(file, ".Proceed"));


                                        tran2.Commit();
                                    }
                                    catch
                                    {
                                        tran2.Rollback();

                                    }






                                }

                            }
                            conp.Close();
                        }
                    }





                }


            }
        }

        private void btn_renamepmix_Click(object sender, EventArgs e)
        {
            foreach (string file99 in Directory.GetFiles(@"\\192.168.1.200\pmix", "*.zip"))
            {
                try
                {
                    ZipFile.ExtractToDirectory(file99, "E:\\ImpotSheiV2\\Extract\\Pmixprocess");
                    //source.CopyTo(destination, true);
                    //File.Copy(file2., destination);
                    //File.Copy(file2, destination, true);
                    // lbl_re.Text = file99.Substring(0, 50);

                    string temploca = file99.Substring(0, 30);
                    string tempdate = file99.Substring(0, 46);
                    string temstorloc = temploca.Substring(temploca.Length - 6, 6);
                    string temstorbusinessdate = tempdate.Substring(tempdate.Length - 8, 8);

                    // DirectoryInfo d = new DirectoryInfo("E:\\ImpotSheiV2\\Extract\\Processingfiles");

                    foreach (string file100 in Directory.GetFiles("E:\\ImpotSheiV2\\Extract\\Pmixprocess", "*.xml"))
                    {
                        try

                        {
                            string tempexfilen = file100.Substring(0, 55);
                            string filedateway = file100.Substring(0, 59);
                            string filedatetake = filedateway.Substring(filedateway.Length - 10, 10);
                            string teexmakename = tempexfilen.Substring(tempexfilen.Length - 16, 16);
                            string pmixmakename = tempexfilen.Substring(tempexfilen.Length - 20, 20);
                            string firstFile = file100;
                            string secondFile = "E:\\ImpotSheiV2\\Extract\\Pmixprocess\\" + pmixmakename + filedatetake + temstorloc + teexmakename + ".xml";

                            File.Move(firstFile, secondFile);

                        }
                        catch
                        {
                            File.Delete(file100);
                        }
                        // file100.Replace(file100, temploca);

                        foreach (string file10 in Directory.GetFiles("E:\\ImpotSheiV2\\Extract\\Pmixprocess", "*.Dat"))
                        {
                            try
                            {

                                File.Delete(file10);
                            }
                            catch
                            {


                            }
                        }

                    }


                    // File.MoveTo(file99.ToDictionary.FullName + "\\" + newName);
                    File.Delete(file99);
                }
                catch
                {
                    //File.Delete(file99);
                    continue;
                }



            }
            btn_way_pmix.PerformClick();
        }

        private void btn_way_pmix_Click(object sender, EventArgs e)
        {

            foreach (string file in Directory.EnumerateFiles("E:\\ImpotSheiV2\\Extract\\Pmixprocess", "*.xml"))
            {
                string contents = File.ReadAllText(file);
                ///////////////////// File Reading/////////////////////////

                pmix_file.Text = Convert.ToString(file.ToString());
                if (pmix_file.Text.Contains("PmixReport"))
                {

                    string connString = SQLCON.ConnectionString2;
                    lbl_stld_pmix.Text = file;

                    SQLCON vb = new SQLCON();
                    vb.OpenConnection();
                    DataSet ds = new DataSet();
                    ds.ReadXml(lbl_stld_pmix.Text);

                    DataTable CHKTLD = ds.Tables["Response"];
                    foreach (DataRow row in CHKTLD.Rows)
                    {
                        // ... Write value of first field as integer.
                        string storloc1 = row.ItemArray[3].ToString();
                        string storbusinessdate1 = row.ItemArray[9].ToString();

                        pmix_location.Text = storloc1;
                        pmix_businessdate.Text = storbusinessdate1;
                    }

                    using (SqlConnection conchk = new SqlConnection(connString))
                    {

                        conchk.Open();

                        SqlCommand comm = new SqlCommand("SELECT COUNT(*) FROM [PMIXPROCESS_STATUS] where [STLD_Location]=@STLD_Location and business_date=@business_date", conchk);
                        comm.Parameters.AddWithValue("@STLD_Location", SqlDbType.VarChar).Value = pmix_location.Text;
                        comm.Parameters.AddWithValue("@business_date", SqlDbType.VarChar).Value = pmix_businessdate.Text;
                        Int32 count = Convert.ToInt32(comm.ExecuteScalar());

                        if (count > 0)
                        {
                            pmixcount.Text = Convert.ToString(count.ToString()); //For example a Label
                        }
                        else
                        {
                            pmixcount.Text = "0";

                            SqlCommand comm2 = new SqlCommand("SELECT [StoreID]   FROM [STLD].[dbo].[TB_STOREMASTER] where [StoreID]=@STLD_Location", conchk);
                            comm2.Parameters.AddWithValue("@STLD_Location", SqlDbType.VarChar).Value = pmix_location.Text;
                            Int32 count2 = Convert.ToInt32(comm2.ExecuteScalar());

                            if (count2 > 0)
                            {
                                pmixcount.Text = "0"; //For example a Label
                            }
                            else
                            {
                                pmixcount.Text = "wrong";
                            }
                        }

                        conchk.Close();
                    }
                    if (pmixcount.Text.Contains('0'))
                    {
                        using (SqlConnection conp = new SqlConnection(connString))
                        {
                            conp.Open();
                            using (var connection = conp)
                            {
                                using (var tran2 = conp.BeginTransaction())
                                {

                                    try
                                    {

                                        using (SqlBulkCopy bc = new SqlBulkCopy(conp, SqlBulkCopyOptions.Default, tran2))
                                        {
                                            DataTable ProductInfo = ds.Tables["ProductInfo"];

                                            ProductInfo.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "STLD_Location",
                                                DataType = typeof(string)
                                            });

                                            ProductInfo.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "business_date",
                                                DataType = typeof(string)
                                            });
                                            foreach (DataRow vrow in ProductInfo.Rows)
                                            {

                                                vrow["STLD_Location"] = pmix_location.Text;
                                                vrow["business_date"] = pmix_businessdate.Text;

                                            }

                                            bc.DestinationTableName = "PMIXPRODUCTINFO";
                                            bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            bc.ColumnMappings.Add("business_date", "business_date");
                                            bc.ColumnMappings.Add("id", "id");
                                            bc.ColumnMappings.Add("name", "name");
                                            bc.ColumnMappings.Add("familyGroup", "familyGroup");
                                            bc.ColumnMappings.Add("class", "class");
                                            bc.ColumnMappings.Add("department", "department");
                                            bc.ColumnMappings.Add("ProductTable_Id", "ProductTable_Id");
                                            bc.WriteToServer(ProductInfo);

                                        }


                                        using (SqlBulkCopy bc = new SqlBulkCopy(conp, SqlBulkCopyOptions.Default, tran2))
                                        {
                                            DataTable FamilyGroup = ds.Tables["FamilyGroup"];

                                            FamilyGroup.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "STLD_Location",
                                                DataType = typeof(string)
                                            });

                                            FamilyGroup.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "business_date",
                                                DataType = typeof(string)
                                            });
                                            foreach (DataRow vrow in FamilyGroup.Rows)
                                            {

                                                vrow["STLD_Location"] = pmix_location.Text;
                                                vrow["business_date"] = pmix_businessdate.Text;

                                            }

                                            bc.DestinationTableName = "PMIXFamilyGroup";
                                            bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            bc.ColumnMappings.Add("business_date", "business_date");
                                            bc.ColumnMappings.Add("FamilyGroup_Id", "FamilyGroup_Id");
                                            bc.ColumnMappings.Add("groupCode", "groupCode");
                                            bc.ColumnMappings.Add("groupName", "groupName");
                                            // bc.ColumnMappings.Add("familyGroup", "familyGroup");
                                            bc.ColumnMappings.Add("POS_Id", "POS_Id");
                                            bc.ColumnMappings.Add("Response_Id", "Response_Id");
                                            bc.WriteToServer(FamilyGroup);

                                        }


                                        using (SqlBulkCopy bc = new SqlBulkCopy(conp, SqlBulkCopyOptions.Default, tran2))
                                        {
                                            DataTable Product = ds.Tables["Product"];

                                            Product.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "STLD_Location",
                                                DataType = typeof(string)
                                            });

                                            Product.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "business_date",
                                                DataType = typeof(string)
                                            });
                                            foreach (DataRow vrow in Product.Rows)
                                            {

                                                vrow["STLD_Location"] = pmix_location.Text;
                                                vrow["business_date"] = pmix_businessdate.Text;

                                            }

                                            bc.DestinationTableName = "PMIXProduct";
                                            bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            bc.ColumnMappings.Add("business_date", "business_date");
                                            bc.ColumnMappings.Add("Product_Id", "Product_Id");
                                            bc.ColumnMappings.Add("id", "id");
                                            bc.ColumnMappings.Add("priceType", "priceType");
                                            if (Product.Columns.Contains("eatinPrice"))
                                            {
                                                bc.ColumnMappings.Add("eatinPrice", "eatingPrice");
                                            }
                                            if (Product.Columns.Contains("eatinTax"))
                                            {
                                                bc.ColumnMappings.Add("eatinTax", "eatingTax"); 
                                            }
                                            if (Product.Columns.Contains("takeoutPrice"))
                                            {
                                                bc.ColumnMappings.Add("takeoutPrice", "takeoutPrice");
                                            }
                                            if (Product.Columns.Contains("takeoutTax"))
                                            {
                                                bc.ColumnMappings.Add("takeoutTax", "takeoutTax");
                                            }

                                            
                                            bc.ColumnMappings.Add("FamilyGroup_Id", "FamilyGroup_Id");
                                            if (Product.Columns.Contains("otherPrice"))
                                            {
                                                bc.ColumnMappings.Add("otherPrice", "otherPrice");
                                            }
                                            if (Product.Columns.Contains("otherTax"))
                                            {
                                                bc.ColumnMappings.Add("otherTax", "otherTax");
                                            }
                                            //bc.ColumnMappings.Add("OperatorSession_Id", "OperatorSession_Id");
                                            bc.WriteToServer(Product);

                                        }

                                        using (SqlBulkCopy bc = new SqlBulkCopy(conp, SqlBulkCopyOptions.Default, tran2))
                                        {
                                            DataTable OperationType = ds.Tables["OperationType"];

                                            OperationType.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "STLD_Location",
                                                DataType = typeof(string)
                                            });

                                            OperationType.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "business_date",
                                                DataType = typeof(string)
                                            });
                                            foreach (DataRow vrow in OperationType.Rows)
                                            {

                                                vrow["STLD_Location"] = pmix_location.Text;
                                                vrow["business_date"] = pmix_businessdate.Text;

                                            }

                                            bc.DestinationTableName = "PMIXOperationType";
                                            bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            bc.ColumnMappings.Add("business_date", "business_date");
                                            bc.ColumnMappings.Add("OperationType_Id", "OperationType_Id");
                                            bc.ColumnMappings.Add("OperationType", "OperationType");
                                            bc.ColumnMappings.Add("Product_Id", "Product_Id");
                                            bc.WriteToServer(OperationType);

                                        }




                                        using (SqlBulkCopy bc = new SqlBulkCopy(conp, SqlBulkCopyOptions.Default, tran2))
                                        {
                                            DataTable PMix = ds.Tables["PMix"];

                                            PMix.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "STLD_Location",
                                                DataType = typeof(string)
                                            });

                                            PMix.Columns.Add(new DataColumn()
                                            {
                                                ColumnName = "business_date",
                                                DataType = typeof(string)
                                            });
                                            foreach (DataRow vrow in PMix.Rows)
                                            {

                                                vrow["STLD_Location"] = pmix_location.Text;
                                                vrow["business_date"] = pmix_businessdate.Text;

                                            }

                                            bc.DestinationTableName = "PMIXPMix";
                                            bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                            bc.ColumnMappings.Add("business_date", "business_date");
                                            if (PMix.Columns.Contains("qtyEatin"))
                                            {
                                                bc.ColumnMappings.Add("qtyEatin", "qtyEatin");
                                            }
                                            if (PMix.Columns.Contains("netBeforeDiscountEatin"))
                                            {
                                                bc.ColumnMappings.Add("netBeforeDiscountEatin", "netBeforeDiscountEatin");
                                            }
                                            if (PMix.Columns.Contains("taxBeforeDiscountEatin"))
                                            {
                                                bc.ColumnMappings.Add("taxBeforeDiscountEatin", "taxBeforeDiscountEatin");
                                            }
                                            if (PMix.Columns.Contains("qtyTakeOut"))
                                            {
                                                bc.ColumnMappings.Add("qtyTakeOut", "qtyTakeOut");
                                            }
                                            if (PMix.Columns.Contains("netAmtTakeOut"))
                                            {
                                                bc.ColumnMappings.Add("netAmtTakeOut", "netAmtTakeOut");
                                            }
                                            if (PMix.Columns.Contains("taxTakeOut"))
                                            {
                                                bc.ColumnMappings.Add("taxTakeOut", "taxTakeOut");
                                            }
                                            if (PMix.Columns.Contains("netBeforeDiscountTakeOut"))
                                            {
                                                bc.ColumnMappings.Add("netBeforeDiscountTakeOut", "netBeforeDiscountTakeOut");
                                            }
                                            if (PMix.Columns.Contains("taxBeforeDiscountTakeOut"))
                                            {
                                                bc.ColumnMappings.Add("taxBeforeDiscountTakeOut", "taxBeforeDiscountTakeOut");
                                            }
                                            if (PMix.Columns.Contains("netAmtEatin"))
                                            {
                                                bc.ColumnMappings.Add("netAmtEatin", "netAmtEatin");
                                            }
                                            if (PMix.Columns.Contains("taxEatIn"))
                                            {
                                                bc.ColumnMappings.Add("taxEatIn", "taxEatIn");
                                            }
                                            if (PMix.Columns.Contains("Price_Id"))
                                            {
                                                bc.ColumnMappings.Add("Price_Id", "Price_Id");
                                            }
                                            if (PMix.Columns.Contains("OperationType_Id"))
                                            {
                                                bc.ColumnMappings.Add("OperationType_Id", "OperationType_Id");
                                            }
                                            if (PMix.Columns.Contains("qtyOther"))
                                            {
                                                bc.ColumnMappings.Add("qtyOther", "qtyOther");
                                            }
                                            if (PMix.Columns.Contains("netAmtOther"))
                                            {
                                                bc.ColumnMappings.Add("netAmtOther", "netAmtOther");
                                            }
                                            if (PMix.Columns.Contains("taxOther"))
                                            {
                                                bc.ColumnMappings.Add("taxOther", "taxOther");
                                            }
                                            if (PMix.Columns.Contains("netBeforeDiscountOther"))
                                            {
                                                bc.ColumnMappings.Add("netBeforeDiscountOther", "netBeforeDiscountOther");
                                            }
                                            if (PMix.Columns.Contains("taxBeforeDiscountOther"))
                                            {
                                                bc.ColumnMappings.Add("taxBeforeDiscountOther", "taxBeforeDiscountOther");
                                            }

                                            bc.WriteToServer(PMix);

                                        }

                                        string smt = "INSERT INTO [STLD].[dbo].[PMIXPROCESS_STATUS] ([STLD_Location] ,[business_date]) VALUES (@STLD_Location,@business_date)";
                                        SqlCommand cmd = new SqlCommand(smt, conp, tran2);
                                        cmd.Parameters.AddWithValue("@STLD_Location", SqlDbType.VarChar).Value = pmix_location.Text;
                                        cmd.Parameters.AddWithValue("@business_date", SqlDbType.VarChar).Value = pmix_businessdate.Text;
                                        cmd.ExecuteNonQuery();
                                        File.Move(file, Path.ChangeExtension(file, ".Proceed"));


                                        tran2.Commit();
                                    }
                                    catch
                                    {
                                        tran2.Rollback();

                                    }






                                }

                            }
                            conp.Close();
                        }
                    }





                }


            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            foreach (string file in Directory.EnumerateFiles("E:\\ImpotSheiV2\\Extract\\4pm", "*.xml"))
            {

                string connString = SQLCON.ConnectionString3;
                lbl_stld_pmix.Text = file;

                SQLCON vb = new SQLCON();
                vb.OpenConnection();
                DataSet ds = new DataSet();
                ds.ReadXml(lbl_stld_pmix.Text);
                DataTable CHKTLD = ds.Tables["Order"];
                foreach (DataRow row in CHKTLD.Rows)
                {
                    // ... Write value of first field as integer.
                    string storloc1 = row.ItemArray[2].ToString();
                    string storbusinessdate1 = row.ItemArray[3].ToString();

                    pmix_location.Text = storloc1;
                    pmix_businessdate.Text = storbusinessdate1;

                }




                using (SqlConnection conp = new SqlConnection(connString))
                {
                    conp.Open();

                    using (var connection = conp)
                    {
                        using (var tran2 = conp.BeginTransaction())
                        {

                            try
                            {
                                using (SqlBulkCopy bc = new SqlBulkCopy(conp, SqlBulkCopyOptions.Default, tran2))
                                {

                                    DataTable Tender = ds.Tables["Tender"];

                                    Tender.Columns.Add(new DataColumn()
                                    {
                                        ColumnName = "STLD_Location",
                                        DataType = typeof(string)
                                    });

                                    Tender.Columns.Add(new DataColumn()
                                    {
                                        ColumnName = "business_date",
                                        DataType = typeof(string)
                                    });
                                    foreach (DataRow vrow in Tender.Rows)
                                    {

                                        vrow["STLD_Location"] = pmix_location.Text;
                                        vrow["business_date"] = pmix_businessdate.Text;

                                    }

                                    bc.DestinationTableName = "TB_4pmSales";
                                    bc.ColumnMappings.Add("STLD_Location", "Location");
                                    bc.ColumnMappings.Add("business_date", "B_date");
                                    //bc.ColumnMappings.Add("NameType", "NameType");
                                    bc.ColumnMappings.Add("TotalAmount", "TotalAmount");
                                    bc.ColumnMappings.Add("OrderType", "OrderType");
                                    bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                    bc.WriteToServer(Tender);




                                }
                                tran2.Commit();
                                File.Move(file, Path.ChangeExtension(file, ".Proceed"));

                            }
                            catch
                            {
                                tran2.Rollback();
                            }
                        }

                    }






                }
            }
        }

        private void button5_Click_1(object sender, EventArgs e)

        {
            BTN_CLEARLBLES.PerformClick();
            foreach (string file1113 in Directory.GetFiles("E:\\ImpotSheiV2\\Extract\\STLDProcess", "*.xml"))
            {
                try
                {

                    File.Delete(file1113);
                }
                catch
                {


                }
            }



            foreach (string file111 in Directory.GetFiles(@"\\192.168.1.200\OrginalSTLD", "*.zip"))

                try
                {
                    ZipFile.ExtractToDirectory(file111, "E:\\ImpotSheiV2\\Extract\\STLDProcess");





                    foreach (string file in Directory.EnumerateFiles("E:\\ImpotSheiV2\\Extract\\STLDProcess", "*.xml"))
                    {


                        string contents = File.ReadAllText(file);
                        ///////////////////// File Reading/////////////////////////

                        lbl_nameval.Text = Convert.ToString(file.ToString());
                        string tempfile = Convert.ToString(file.ToString());
                        if (lbl_nameval.Text.Contains("STLD"))
                        {



                            string connString = SQLCON.ConnectionString2;
                            lbl_STLD.Text = file;

                            SQLCON vb = new SQLCON();
                            vb.OpenConnection();

                            DataSet ds = new DataSet();

                            ds.ReadXml(lbl_STLD.Text);









                            DataTable CHKTLD = ds.Tables["TLD"];
                            foreach (DataRow row in CHKTLD.Rows)
                            {
                                // ... Write value of first field as integer.
                                //  string temploca = file.Substring(0, 57);
                                // string tempdate = file.Substring(0, 66);
                                string storloc = row.ItemArray[2].ToString();
                                string storbusinessdate = row.ItemArray[3].ToString();


                                lbl_location.Text = storloc;
                                lbl_business.Text = storbusinessdate;
                            }

                            using (SqlConnection con1 = new SqlConnection(connString))
                            {

                                con1.Open();

                                SqlCommand comm = new SqlCommand("SELECT COUNT(*) FROM [STLDPROCESS_STATUS] where [STLD_Location]=@STLD_Location and business_date=@business_date", con1);
                                comm.Parameters.AddWithValue("@STLD_Location", SqlDbType.VarChar).Value = lbl_location.Text;
                                comm.Parameters.AddWithValue("@business_date", SqlDbType.VarChar).Value = lbl_business.Text;
                                Int32 count = Convert.ToInt32(comm.ExecuteScalar());
                                if (count > 0)
                                {
                                    lblCount.Text = Convert.ToString(count.ToString()); //For example a Label
                                }
                                else
                                {
                                    if (lbl_location.Text.Contains("online"))
                                    {
                                        lblCount.Text = "wrong";
                                    }
                                    else
                                    {
                                        lblCount.Text = "0";

                                        SqlCommand comm2 = new SqlCommand("SELECT [StoreID]   FROM [STLD].[dbo].[TB_STOREMASTER] where [StoreID]=@STLD_Location", con1);
                                        comm2.Parameters.AddWithValue("@STLD_Location", SqlDbType.VarChar).Value = lbl_location.Text;
                                        Int32 count2 = Convert.ToInt32(comm2.ExecuteScalar());

                                        if (count2 > 0)
                                        {
                                            lblCount.Text = "0"; //For example a Label
                                        }
                                        else
                                        {
                                            lblCount.Text = "wrong";
                                        }
                                    }
                                }
                                con1.Close();

                            }
                            ///////////////////////////// Check dublicate records////////////////////////






                            using (SqlConnection con = new SqlConnection(connString))
                            {
                                con.Open();

                                if (lblCount.Text.Contains('0'))
                                {

                                    using (var connection = con)
                                    {
                                        using (var tran = con.BeginTransaction())
                                        {


                                            try
                                            {




                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {

                                                    DataTable TLD = ds.Tables["TLD"];
                                                    foreach (DataRow row in TLD.Rows)
                                                    {
                                                        // ... Write value of first field as integer.
                                                        // string storloc = row.ItemArray[2].ToString();
                                                        // string storbusinessdate = row.ItemArray[3].ToString();

                                                        //lbl_location.Text = storloc;
                                                        //lbl_business.Text = storbusinessdate;
                                                    }





                                                    DataTable TRX_Sale = ds.Tables["TRX_Sale"];

                                                    TRX_Sale.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    TRX_Sale.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in TRX_Sale.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }



                                                    bc.DestinationTableName = "TRX_Sale";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("TRX_Sale_Id", "TRX_Sale_Id");
                                                    bc.ColumnMappings.Add("status", "status");
                                                    bc.ColumnMappings.Add("POD", "POD");
                                                    bc.ColumnMappings.Add("RemPOD", "RemPOD");
                                                    bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                    bc.WriteToServer(TRX_Sale);

                                                }




                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {

                                                    //------- Order Table////////////////
                                                    DataTable Order_TB = ds.Tables["Order"];
                                                    Order_TB.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    Order_TB.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in Order_TB.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "Order_TB";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                    bc.ColumnMappings.Add("Timestamp", "Timestamp");
                                                    bc.ColumnMappings.Add("uniqueId", "uniqueId");
                                                    bc.ColumnMappings.Add("kind", "kind");
                                                    bc.ColumnMappings.Add("key", "key");
                                                    bc.ColumnMappings.Add("major", "major");
                                                    bc.ColumnMappings.Add("minor", "minor");
                                                    bc.ColumnMappings.Add("side", "side");
                                                    bc.ColumnMappings.Add("receiptNumber", "receiptNumber");
                                                    bc.ColumnMappings.Add("fpReceiptNumber", "fpReceiptNumber");
                                                    //bc.ColumnMappings.Add("boot", "boot");
                                                    bc.ColumnMappings.Add("saleType", "saleType");
                                                    bc.ColumnMappings.Add("totalAmount", "totalAmount");
                                                    bc.ColumnMappings.Add("nonProductAmount", "nonProductAmount");
                                                    bc.ColumnMappings.Add("totalTax", "totalTax");
                                                    bc.ColumnMappings.Add("nonProductTax", "nonProductTax");
                                                    bc.ColumnMappings.Add("orderSrc", "orderSrc");
                                                    bc.ColumnMappings.Add("startSaleDate", "startSaleDate");
                                                    bc.ColumnMappings.Add("startSaleTime", "startSaleTime");
                                                    bc.ColumnMappings.Add("endSaleDate", "endSaleDate");
                                                    bc.ColumnMappings.Add("endSaleTime", "endSaleTime");
                                                    bc.ColumnMappings.Add("TRX_Sale_Id", "TRX_Sale_Id");
                                                    bc.WriteToServer(Order_TB);

                                                    //----------------------------------



                                                }

                                                //------refund
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con))
                                                {
                                                    // DataTable TRX_Refund = ds.Tables["TRX_Refund"];

                                                    // TRX_Refund.Columns.Add(new DataColumn()
                                                    // {
                                                    //     ColumnName = "STLD_Location",
                                                    //     DataType = typeof(string)
                                                    // });

                                                    // TRX_Refund.Columns.Add(new DataColumn()
                                                    // {
                                                    //     ColumnName = "business_date",
                                                    //     DataType = typeof(string)
                                                    // });
                                                    // foreach (DataRow vrow in TRX_Refund.Rows)
                                                    // {

                                                    //     vrow["STLD_Location"] = lbl_location.Text;
                                                    //     vrow["business_date"] = lbl_business.Text;

                                                    // }



                                                    // bc.DestinationTableName = "TRX_Refund";
                                                    // bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    // bc.ColumnMappings.Add("business_date", "business_date");
                                                    // bc.ColumnMappings.Add("TRX_Refund_Id", "TRX_Refund_Id");
                                                    //bc.ColumnMappings.Add("status", "status");
                                                    // bc.ColumnMappings.Add("POD", "POD");
                                                    // bc.ColumnMappings.Add("RemPOD", "RemPOD");
                                                    // bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                    // bc.WriteToServer(TRX_Refund);

                                                }



                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {

                                                    //------- Item Table////////////////
                                                    DataTable Item_TB = ds.Tables["Item"];
                                                    if (Item_TB != null)
                                                    {
                                                        Item_TB.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        Item_TB.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in Item_TB.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }

                                                        bc.DestinationTableName = "Item_TB";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("Item_Id", "Item_Id");
                                                        bc.ColumnMappings.Add("code", "code");
                                                        bc.ColumnMappings.Add("type", "type");
                                                        bc.ColumnMappings.Add("action", "action");
                                                        bc.ColumnMappings.Add("level", "level");
                                                        bc.ColumnMappings.Add("id", "id");
                                                        bc.ColumnMappings.Add("displayOrder", "displayOrder");
                                                        bc.ColumnMappings.Add("qty", "qty");
                                                        bc.ColumnMappings.Add("grillQty", "grillQty");
                                                        bc.ColumnMappings.Add("grillModifier", "grillModifier");
                                                        bc.ColumnMappings.Add("qtyPromo", "qtyPromo");
                                                        bc.ColumnMappings.Add("chgAfterTotal", "chgAfterTotal");
                                                        bc.ColumnMappings.Add("BPPrice", "BPPrice");
                                                        bc.ColumnMappings.Add("BPTax", "BPTax");
                                                        bc.ColumnMappings.Add("BDPrice", "BDPrice");
                                                        bc.ColumnMappings.Add("BDTax", "BDTax");
                                                        bc.ColumnMappings.Add("totalPrice", "totalPrice");
                                                        bc.ColumnMappings.Add("totalTax", "totalTax");
                                                        bc.ColumnMappings.Add("category", "category");
                                                        bc.ColumnMappings.Add("familyGroup", "familyGroup");
                                                        bc.ColumnMappings.Add("daypart", "daypart");
                                                        bc.ColumnMappings.Add("description", "description");
                                                        bc.ColumnMappings.Add("department", "department");
                                                        bc.ColumnMappings.Add("departmentClass", "departmentClass");
                                                        bc.ColumnMappings.Add("departmentSubClass", "departmentSubClass");
                                                        bc.ColumnMappings.Add("unitPrice", "unitPrice");
                                                        bc.ColumnMappings.Add("unitTax", "unitTax");
                                                        bc.ColumnMappings.Add("solvedChoice", "solvedChoice");
                                                        bc.ColumnMappings.Add("isUpcharge", "isUpcharge");
                                                        bc.ColumnMappings.Add("Item_Id_0", "Item_Id_0");
                                                        bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                        bc.WriteToServer(Item_TB);

                                                    }
                                                    ////////////////////////////////////

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {
                                                    ///-------------- Promo table-----------------
                                                    DataTable TRX_Overring = ds.Tables["TRX_Overring"];
                                                    if (TRX_Overring != null)
                                                    {
                                                        TRX_Overring.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });
                                                        TRX_Overring.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in TRX_Overring.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "TRX_Overring";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("TRX_Overring_Id", "TRX_Overring_Id");
                                                        bc.ColumnMappings.Add("POD", "POD");
                                                        bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                        bc.WriteToServer(TRX_Overring);
                                                    }
                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {
                                                    ///-------------- Promo table-----------------
                                                    DataTable Product = ds.Tables["Product"];
                                                    if (Product != null)
                                                    {
                                                        Product.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });
                                                        Product.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in Product.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "Product";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("code", "code");
                                                        bc.ColumnMappings.Add("quantity", "quantity");
                                                        bc.ColumnMappings.Add("Components_Id", "Components_Id");
                                                        bc.WriteToServer(Product);
                                                    }
                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {
                                                    ///-------------- Promo table-----------------
                                                    DataTable Components = ds.Tables["Components"];
                                                    if (Components != null)
                                                    {
                                                        Components.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });
                                                        Components.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in Components.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "Components";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("Components_Id", "Components_Id");
                                                        bc.ColumnMappings.Add("Ev_BreakValueMeal_Id", "Ev_BreakValueMeal_Id");
                                                        bc.WriteToServer(Components);
                                                    }
                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {
                                                    ///-------------- Promo table-----------------
                                                    DataTable Ev_BreakValueMeal = ds.Tables["Ev_BreakValueMeal"];
                                                    if (Ev_BreakValueMeal != null)
                                                    {
                                                        Ev_BreakValueMeal.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });
                                                        Ev_BreakValueMeal.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in Ev_BreakValueMeal.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "Ev_BreakValueMeal";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("ProductCode", "ProductCode");
                                                        bc.ColumnMappings.Add("Quantity", "Quantity");
                                                        bc.ColumnMappings.Add("Ev_BreakValueMeal_Id", "Ev_BreakValueMeal_Id");
                                                        bc.WriteToServer(Ev_BreakValueMeal);
                                                    }
                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {
                                                    ///-------------- Promo table-----------------
                                                    DataTable Promo = ds.Tables["Promo"];
                                                    if (Promo != null)
                                                    {
                                                        Promo.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        Promo.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in Promo.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }


                                                        bc.DestinationTableName = "Promo";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("id", "id");
                                                        bc.ColumnMappings.Add("name", "name");
                                                        bc.ColumnMappings.Add("qty", "qty");
                                                        bc.ColumnMappings.Add("Item_id", "Item_id");
                                                        bc.WriteToServer(Promo);
                                                    }
                                                    //------------------------------------------
                                                }

                                                using (SqlBulkCopy bc = new SqlBulkCopy(con))
                                                {
                                                    ///////------------------------
                                                    // DataTable PromotionApplied = ds.Tables["PromotionApplied"];
                                                    //  if (PromotionApplied != null)
                                                    //  {
                                                    //     PromotionApplied.Columns.Add(new DataColumn()
                                                    //    {
                                                    //        ColumnName = "STLD_Location",
                                                    //        DataType = typeof(string)
                                                    //    });

                                                    //    PromotionApplied.Columns.Add(new DataColumn()
                                                    //    {
                                                    //        ColumnName = "business_date",
                                                    //        DataType = typeof(string)
                                                    //    });
                                                    //    foreach (DataRow vrow in PromotionApplied.Rows)
                                                    //    {

                                                    //        vrow["STLD_Location"] = lbl_location.Text;
                                                    //        vrow["business_date"] = lbl_business.Text;

                                                    //    }
                                                    //    bc.DestinationTableName = "PromotionApplied";
                                                    //    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    //    bc.ColumnMappings.Add("business_date", "business_date");
                                                    //    bc.ColumnMappings.Add("promotionId", "promotionId");
                                                    //    bc.ColumnMappings.Add("promotionCounter", "promotionCounter");
                                                    //    bc.ColumnMappings.Add("eligible", "eligible");
                                                    //    bc.ColumnMappings.Add("originalPrice", "originalPrice");
                                                    //    bc.ColumnMappings.Add("discountAmount", "discountAmount");
                                                    //    bc.ColumnMappings.Add("discountType", "discountType");
                                                    //    bc.ColumnMappings.Add("originalItemPromoQty", "originalItemPromoQty");
                                                    //    bc.ColumnMappings.Add("originalProductCode", "originalProductCode");
                                                    //    bc.ColumnMappings.Add("offerId", "offerId");
                                                    //    bc.ColumnMappings.Add("Item_Id", "Item_Id");
                                                    //    bc.WriteToServer(PromotionApplied);
                                                    //    ///////////////////////

                                                    //}
                                                }

                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {
                                                    ////////// Offer ////// Table
                                                    DataTable Offers = ds.Tables["Offers"];


                                                    if (Offers != null)
                                                    {

                                                        Offers.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        Offers.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in Offers.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "Offers";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("offerId", "offerId");

                                                        // bc.ColumnMappings.Add("beforeOfferPrice", "beforeOfferPrice");
                                                        // bc.ColumnMappings.Add("discountAmount", "discountAmount");
                                                        //  bc.ColumnMappings.Add("discountType", "discountType");
                                                        // bc.ColumnMappings.Add("Item_Id", "Item_Id");
                                                        bc.ColumnMappings.Add("customerId", "customerId");
                                                        bc.ColumnMappings.Add("offerName", "offerName");
                                                        bc.ColumnMappings.Add("override", "override");
                                                        bc.ColumnMappings.Add("applied", "applied");
                                                        bc.ColumnMappings.Add("promotionId", "promotionId");
                                                        bc.ColumnMappings.Add("offerBarcodeType", "offerBarcodeType");
                                                        bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                        bc.WriteToServer(Offers);
                                                    }
                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {
                                                    DataTable TaxChain = ds.Tables["TaxChain"];

                                                    TaxChain.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    TaxChain.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in TaxChain.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "TaxChain";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("id", "id");
                                                    bc.ColumnMappings.Add("name", "name");
                                                    bc.ColumnMappings.Add("rate", "rate");
                                                    bc.ColumnMappings.Add("baseAmount", "baseAmount");
                                                    bc.ColumnMappings.Add("amount", "amount");
                                                    bc.ColumnMappings.Add("BDBaseAmount", "BDBaseAmount");
                                                    bc.ColumnMappings.Add("BDAmount", "BDAmount");
                                                    bc.ColumnMappings.Add("Item_Id", "Item_Id");
                                                    // bc.ColumnMappings.Add("BPBaseAmount", "BPBaseAmount");
                                                    //bc.ColumnMappings.Add("BPAmount", "BPAmount");
                                                    bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                    bc.WriteToServer(TaxChain);

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {

                                                    DataTable Promotions = ds.Tables["Promotions"];
                                                    if (Promotions != null)
                                                    {
                                                        Promotions.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        Promotions.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in Promotions.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }

                                                        bc.DestinationTableName = "Promotions";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("Promotions_Id", "Promotions_Id");
                                                        bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                        bc.WriteToServer(Promotions);
                                                    }
                                                }


                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {

                                                    DataTable Promotion = ds.Tables["Promotion"];
                                                    if (Promotion != null)
                                                    {


                                                        Promotion.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        Promotion.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in Promotion.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }

                                                        bc.DestinationTableName = "Promotion";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("promotionId", "promotionId");
                                                        bc.ColumnMappings.Add("promotionName", "promotionName");
                                                        bc.ColumnMappings.Add("promotionCounter", "promotionCounter");
                                                        bc.ColumnMappings.Add("discountType", "discountType");
                                                        bc.ColumnMappings.Add("discountAmount", "discountAmount");
                                                        bc.ColumnMappings.Add("offerId", "offerId");
                                                        bc.ColumnMappings.Add("exclusive", "exclusive");
                                                        bc.ColumnMappings.Add("promotionOnTender", "promotionOnTender");
                                                        bc.ColumnMappings.Add("countTowardsPromotionLimit", "countTowardsPromotionLimit");
                                                        bc.ColumnMappings.Add("returnedValue", "returnedValue");
                                                        bc.ColumnMappings.Add("Promotions_Id", "Promotions_Id");
                                                        bc.WriteToServer(Promotion);

                                                    }
                                                }


                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {

                                                    DataTable Customer = ds.Tables["Customer"];
                                                    if (Customer != null)
                                                    {
                                                        Customer.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        Customer.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in Customer.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }


                                                        bc.DestinationTableName = "Customer";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("id", "id");
                                                        bc.ColumnMappings.Add("nickname", "nickname");
                                                        bc.ColumnMappings.Add("greeting", "greeting");
                                                        bc.ColumnMappings.Add("loyaltyCardId", "loyaltyCardId");
                                                        bc.ColumnMappings.Add("loyaltyCardType", "loyaltyCardType");
                                                        bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                        bc.WriteToServer(Customer);
                                                    }
                                                }


                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {

                                                    DataTable CustomInfo = ds.Tables["CustomInfo"];
                                                    if (CustomInfo != null)
                                                    {
                                                        CustomInfo.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        CustomInfo.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in CustomInfo.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "CustomInfo";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("CustomInfo_Id", "CustomInfo_Id");
                                                        bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                        bc.WriteToServer(CustomInfo);


                                                    }
                                                }


                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {

                                                    DataTable Tenders = ds.Tables["Tenders"];
                                                    if (Tenders != null)
                                                    {
                                                        Tenders.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        Tenders.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in Tenders.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "Tenders";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("Tenders_Id", "Tenders_Id");
                                                        bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                        bc.WriteToServer(Tenders);
                                                    }
                                                }

                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {

                                                    DataTable Tender = ds.Tables["Tender"];
                                                    if (Tender != null)
                                                    {
                                                        Tender.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        Tender.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in Tender.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "Tender";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("TenderId", "TenderId");
                                                        bc.ColumnMappings.Add("TenderKind", "TenderKind");
                                                        bc.ColumnMappings.Add("TenderName", "TenderName");
                                                        bc.ColumnMappings.Add("TenderQuantity", "TenderQuantity");
                                                        bc.ColumnMappings.Add("FaceValue", "FaceValue");
                                                        bc.ColumnMappings.Add("TenderAmount", "TenderAmount");
                                                        bc.ColumnMappings.Add("BaseAction", "BaseAction");
                                                        bc.ColumnMappings.Add("Persisted", "Persisted");
                                                        bc.ColumnMappings.Add("CardProviderID", "CardProviderID");
                                                        bc.ColumnMappings.Add("CashlessData", "CashlessData");
                                                        bc.ColumnMappings.Add("TaxOption", "TaxOption");
                                                        bc.ColumnMappings.Add("SubtotalOption", "SubtotalOption");
                                                        bc.ColumnMappings.Add("ForeignCurrencyIndicator", "ForeignCurrencyIndicator");
                                                        bc.ColumnMappings.Add("DiscountDescription", "DiscountDescription");
                                                        bc.ColumnMappings.Add("CashlessTransactionID", "CashlessTransactionID");
                                                        bc.ColumnMappings.Add("PaymentChannel", "PaymentChannel");
                                                        bc.ColumnMappings.Add("Tenders_Id", "Tenders_Id");
                                                        bc.WriteToServer(Tender);

                                                    }
                                                }

                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable POSTiming = ds.Tables["POSTiming"];
                                                    if (POSTiming != null)
                                                    {
                                                        POSTiming.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        POSTiming.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in POSTiming.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }

                                                    }


                                                }

                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable EventsDetail = ds.Tables["EventsDetail"];
                                                    if (EventsDetail != null)
                                                    {
                                                        EventsDetail.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        EventsDetail.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in EventsDetail.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "EventsDetail";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("EventsDetail_Id", "EventsDetail_Id");
                                                        bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                        bc.WriteToServer(EventsDetail);

                                                    }


                                                }

                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable SaleEvent = ds.Tables["SaleEvent"];
                                                    if (SaleEvent != null)
                                                    {
                                                        SaleEvent.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        SaleEvent.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in SaleEvent.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        //bc.DestinationTableName = "SaleEvent";
                                                        //bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        //bc.ColumnMappings.Add("business_date", "business_date");
                                                        //bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                        //bc.ColumnMappings.Add("Ev_SaleStored", "Ev_SaleStored");
                                                        //bc.ColumnMappings.Add("Ev_SaleRecalled", "Ev_SaleRecalled");
                                                        //bc.ColumnMappings.Add("Ev_BackFromTotal", "Ev_BackFromTotal");
                                                        //bc.ColumnMappings.Add("Ev_SaleTotal", "Ev_SaleTotal");
                                                        //bc.ColumnMappings.Add("Type", "Type");
                                                        //bc.ColumnMappings.Add("Time", "Time");
                                                        //bc.ColumnMappings.Add("EventsDetail_Id", "EventsDetail_Id");
                                                        //bc.WriteToServer(SaleEvent);

                                                    }

                                                }

                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable Ev_SaleIncrementItemQty = ds.Tables["Ev_SaleIncrementItemQty"];
                                                    if (Ev_SaleIncrementItemQty != null)
                                                    {
                                                        Ev_SaleIncrementItemQty.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        Ev_SaleIncrementItemQty.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in Ev_SaleIncrementItemQty.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "Ev_SaleIncrementItemQty";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("SaleIndex", "SaleIndex");
                                                        bc.ColumnMappings.Add("Quantity", "Quantity");
                                                        bc.ColumnMappings.Add("ProductCode", "ProductCode");
                                                        bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                        bc.WriteToServer(Ev_SaleIncrementItemQty);

                                                    }

                                                }

                                                using (SqlBulkCopy bc = new SqlBulkCopy(con))
                                                {


                                                    DataTable TRX_GetAuthorization = ds.Tables["TRX_GetAuthorization"];
                                                    if (TRX_GetAuthorization != null)
                                                    {
                                                        TRX_GetAuthorization.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        TRX_GetAuthorization.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in TRX_GetAuthorization.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        //bc.DestinationTableName = "TRX_GetAuthorization";
                                                        //bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        //bc.ColumnMappings.Add("business_date", "business_date");
                                                        //bc.ColumnMappings.Add("Action", "Action");
                                                        //bc.ColumnMappings.Add("ManagerID", "ManagerID");
                                                        //bc.ColumnMappings.Add("ManagerName", "ManagerName");
                                                        //bc.ColumnMappings.Add("SecurityLevel", "SecurityLevel");
                                                        //bc.ColumnMappings.Add("ExpirationDate", "ExpirationDate");
                                                        //bc.ColumnMappings.Add("Password", "Password");
                                                        //bc.ColumnMappings.Add("Islogged", "Islogged");
                                                        //bc.ColumnMappings.Add("Method", "Method");
                                                        ////bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                        //bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                        //bc.WriteToServer(TRX_GetAuthorization);

                                                    }

                                                }

                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable EV_NotChargedPromotional = ds.Tables["EV_NotChargedPromotional"];
                                                    if (EV_NotChargedPromotional != null)
                                                    {
                                                        EV_NotChargedPromotional.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        EV_NotChargedPromotional.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in EV_NotChargedPromotional.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "EV_NotChargedPromotional";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("ProductCode", "ProductCode");
                                                        bc.ColumnMappings.Add("Quantity", "Quantity");
                                                        bc.ColumnMappings.Add("NotChargedValue", "NotChargedValue");
                                                        bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                        bc.WriteToServer(EV_NotChargedPromotional);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable Ev_AddTender = ds.Tables["Ev_AddTender"];
                                                    if (Ev_AddTender != null)
                                                    {
                                                        Ev_AddTender.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        Ev_AddTender.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in Ev_AddTender.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "Ev_AddTender";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("TenderId", "TenderId");
                                                        bc.ColumnMappings.Add("FaceValue", "FaceValue");
                                                        bc.ColumnMappings.Add("TenderAmount", "TenderAmount");
                                                        bc.ColumnMappings.Add("BaseAction", "BaseAction");
                                                        bc.ColumnMappings.Add("Persisted", "Persisted");
                                                        bc.ColumnMappings.Add("CardProviderID", "CardProviderID");
                                                        bc.ColumnMappings.Add("CashlessData", "CashlessData");
                                                        bc.ColumnMappings.Add("CashlessTransactionID", "CashlessTransactionID");
                                                        bc.ColumnMappings.Add("PreAuthorization", "PreAuthorization");
                                                        bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                        bc.WriteToServer(Ev_AddTender);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable Ev_SetSaleType = ds.Tables["Ev_SetSaleType"];
                                                    if (Ev_SetSaleType != null)
                                                    {
                                                        Ev_SetSaleType.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        Ev_SetSaleType.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in Ev_SetSaleType.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "Ev_SetSaleType";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("Type", "Type");
                                                        bc.ColumnMappings.Add("ForceExhibition", "ForceExhibition");
                                                        bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                        bc.WriteToServer(Ev_SetSaleType);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable Ev_SaleChoice = ds.Tables["Ev_SaleChoice"];
                                                    if (Ev_SaleChoice != null)
                                                    {
                                                        Ev_SaleChoice.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        Ev_SaleChoice.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in Ev_SaleChoice.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "Ev_SaleChoice";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("ProductCode", "ProductCode");
                                                        bc.ColumnMappings.Add("ChoiceCode", "ChoiceCode");
                                                        bc.ColumnMappings.Add("Quantity", "Quantity");
                                                        bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                        bc.WriteToServer(Ev_SaleChoice);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable Ev_SaleItem = ds.Tables["Ev_SaleItem"];
                                                    if (Ev_SaleItem != null)
                                                    {
                                                        Ev_SaleItem.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        Ev_SaleItem.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in Ev_SaleItem.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "Ev_SaleItem";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("ProductCode", "ProductCode");
                                                        bc.ColumnMappings.Add("Quantity", "Quantity");
                                                        bc.ColumnMappings.Add("UpdatedQuantity", "UpdatedQuantity");
                                                        bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                        bc.WriteToServer(Ev_SaleItem);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable Ev_SaleCutomInfo = ds.Tables["Ev_SaleCutomInfo"];
                                                    if (Ev_SaleCutomInfo != null)
                                                    {
                                                        Ev_SaleCutomInfo.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        Ev_SaleCutomInfo.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in Ev_SaleCutomInfo.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        //bc.DestinationTableName = "Ev_SaleCutomInfo";
                                                        //bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        //bc.ColumnMappings.Add("business_date", "business_date");
                                                        //bc.ColumnMappings.Add("ProductCode", "ProductCode");
                                                        //bc.ColumnMappings.Add("Quantity", "Quantity");
                                                        //bc.ColumnMappings.Add("UpdatedQuantity", "UpdatedQuantity");
                                                        //bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                        //bc.WriteToServer(Ev_SaleCutomInfo);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable Ev_SaleEnd = ds.Tables["Ev_SaleEnd"];
                                                    if (Ev_SaleEnd != null)
                                                    {
                                                        Ev_SaleEnd.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        Ev_SaleEnd.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in Ev_SaleEnd.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "Ev_SaleEnd";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("Type", "Type");
                                                        bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                        bc.WriteToServer(Ev_SaleEnd);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable Ev_SaleStart = ds.Tables["Ev_SaleStart"];
                                                    if (Ev_SaleStart != null)
                                                    {
                                                        Ev_SaleStart.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        Ev_SaleStart.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in Ev_SaleStart.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "Ev_SaleStart";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("DisabledChoices", "DisabledChoices");
                                                        bc.ColumnMappings.Add("TenderPersisted", "TenderPersisted");
                                                        bc.ColumnMappings.Add("Multiorder", "Multiorder");
                                                        bc.ColumnMappings.Add("SaleEvent_Id", "SaleEvent_Id");
                                                        bc.WriteToServer(Ev_SaleStart);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable Fiscal_Information = ds.Tables["Fiscal_Information"];
                                                    if (Fiscal_Information != null)
                                                    {
                                                        Fiscal_Information.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        Fiscal_Information.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in Fiscal_Information.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "Fiscal_Information";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("TIN", "TIN");
                                                        bc.ColumnMappings.Add("name", "name");
                                                        bc.ColumnMappings.Add("address", "address");
                                                        bc.ColumnMappings.Add("ZIP", "ZIP");
                                                        bc.ColumnMappings.Add("Order_Id", "Order_Id");
                                                        bc.WriteToServer(Fiscal_Information);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable Ev_DrawerClose = ds.Tables["Ev_DrawerClose"];
                                                    if (Ev_DrawerClose != null)
                                                    {
                                                        Ev_DrawerClose.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        Ev_DrawerClose.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in Ev_DrawerClose.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "Ev_DrawerClose";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("TotalOpenTime", "TotalOpenTime");
                                                        bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                        bc.WriteToServer(Ev_DrawerClose);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable TRX_OperLogin = ds.Tables["TRX_OperLogin"];
                                                    if (TRX_OperLogin != null)
                                                    {
                                                        TRX_OperLogin.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        TRX_OperLogin.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in TRX_OperLogin.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "TRX_OperLogin";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("CrewID", "CrewID");
                                                        bc.ColumnMappings.Add("CrewName", "CrewName");
                                                        bc.ColumnMappings.Add("CrewSecurityLevel", "CrewSecurityLevel");
                                                        bc.ColumnMappings.Add("POD", "POD");
                                                        bc.ColumnMappings.Add("RemotePOD", "RemotePOD");
                                                        bc.ColumnMappings.Add("AutoLogin", "AutoLogin");
                                                        bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                        bc.WriteToServer(TRX_OperLogin);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable TRX_SetPOD = ds.Tables["TRX_SetPOD"];
                                                    if (TRX_SetPOD != null)
                                                    {
                                                        TRX_SetPOD.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        TRX_SetPOD.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in TRX_SetPOD.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "TRX_SetPOD";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("PODId", "PODId");
                                                        bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                        bc.WriteToServer(TRX_SetPOD);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable TRX_DayOpen = ds.Tables["TRX_DayOpen"];
                                                    if (TRX_DayOpen != null)
                                                    {
                                                        TRX_DayOpen.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        TRX_DayOpen.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in TRX_DayOpen.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "TRX_DayOpen";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("BusinessDate", "BusinessDate");
                                                        bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                        bc.WriteToServer(TRX_DayOpen);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable TRX_TaxTable = ds.Tables["TRX_TaxTable"];
                                                    if (TRX_TaxTable != null)
                                                    {
                                                        TRX_TaxTable.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        TRX_TaxTable.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in TRX_TaxTable.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "TRX_TaxTable";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("TRX_TaxTable_Id", "TRX_TaxTable_Id");
                                                        bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                        bc.WriteToServer(TRX_TaxTable);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable TaxType = ds.Tables["TaxType"];
                                                    if (TaxType != null)
                                                    {
                                                        TaxType.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        TaxType.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in TaxType.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "TaxType";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("TaxId", "TaxId");
                                                        bc.ColumnMappings.Add("TaxDescription", "TaxDescription");
                                                        bc.ColumnMappings.Add("TaxRate", "TaxRate");
                                                        bc.ColumnMappings.Add("TaxBasis", "TaxBasis");
                                                        bc.ColumnMappings.Add("TaxCalcType", "TaxCalcType");
                                                        bc.ColumnMappings.Add("Rounding", "Rounding");
                                                        bc.ColumnMappings.Add("Precision", "Precision");
                                                        bc.ColumnMappings.Add("TRX_TaxTable_Id", "TRX_TaxTable_Id");
                                                        bc.WriteToServer(TaxType);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable TRX_TenderTable = ds.Tables["TRX_TenderTable"];
                                                    if (TRX_TenderTable != null)
                                                    {
                                                        TRX_TenderTable.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        TRX_TenderTable.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in TRX_TenderTable.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "TRX_TenderTable";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("TRX_TenderTable_Id", "TRX_TenderTable_Id");
                                                        bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                        bc.WriteToServer(TRX_TenderTable);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable TenderType = ds.Tables["TenderType"];
                                                    if (TenderType != null)
                                                    {
                                                        TenderType.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        TenderType.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in TenderType.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "TenderType";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("TenderId", "TenderId");
                                                        bc.ColumnMappings.Add("TenderFiscalIndex", "TenderFiscalIndex");
                                                        bc.ColumnMappings.Add("TenderName", "TenderName");
                                                        bc.ColumnMappings.Add("TenderCategory", "TenderCategory");
                                                        bc.ColumnMappings.Add("Taxoption", "Taxoption");
                                                        bc.ColumnMappings.Add("DefaultSkimLimit", "DefaultSkimLimit");
                                                        bc.ColumnMappings.Add("DefaultHaloLimit", "DefaultHaloLimit");
                                                        bc.ColumnMappings.Add("SubtotalOption", "SubtotalOption");
                                                        bc.ColumnMappings.Add("CurrencyDecimals", "CurrencyDecimals");
                                                        bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                        bc.ColumnMappings.Add("TRX_TenderTable_Id", "TRX_TenderTable_Id");
                                                        bc.WriteToServer(TenderType);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable TenderFlags = ds.Tables["TenderFlags"];
                                                    if (TenderFlags != null)
                                                    {
                                                        TenderFlags.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        TenderFlags.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in TenderFlags.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "TenderFlags";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                        bc.ColumnMappings.Add("TenderFlags_Id", "TenderFlags_Id");
                                                        bc.WriteToServer(TenderFlags);

                                                    }

                                                }

                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable Info = ds.Tables["Info"];
                                                    if (Info != null)
                                                    {
                                                        Info.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        Info.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in Info.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "Info";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("name", "name");
                                                        bc.ColumnMappings.Add("value", "value");
                                                        bc.ColumnMappings.Add("CustomInfo_Id", "CustomInfo_Id");
                                                        bc.WriteToServer(Info);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con))
                                                {


                                                    DataTable TenderFlag = ds.Tables["TenderFlag"];
                                                    if (TenderFlag != null)
                                                    {
                                                        // TenderFlag.Columns.Add(new DataColumn()
                                                        //  {
                                                        //      ColumnName = "STLD_Location",
                                                        //      DataType = typeof(string)
                                                        //  });

                                                        //  TenderFlag.Columns.Add(new DataColumn()
                                                        //  {
                                                        //      ColumnName = "business_date",
                                                        //      DataType = typeof(string)
                                                        //  });
                                                        //foreach (DataRow vrow in TenderFlag.Rows)
                                                        // {

                                                        //     vrow["STLD_Location"] = lbl_location.Text;
                                                        //     vrow["business_date"] = lbl_business.Text;

                                                        // }
                                                        //  bc.DestinationTableName = "TenderFlag";
                                                        //  bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        //  bc.ColumnMappings.Add("business_date", "business_date");
                                                        //   bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                        //   bc.ColumnMappings.Add("TenderFlags_Id", "TenderFlags_Id");
                                                        //   bc.WriteToServer(TenderFlag);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable TenderChange = ds.Tables["TenderChange"];
                                                    if (TenderChange != null)
                                                    {
                                                        TenderChange.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        TenderChange.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in TenderChange.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "TenderChange";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("id", "id");
                                                        bc.ColumnMappings.Add("type", "type");
                                                        bc.ColumnMappings.Add("roundToMinAmount", "roundToMinAmount");
                                                        bc.ColumnMappings.Add("maxAllowed", "maxAllowed");
                                                        bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                        bc.WriteToServer(TenderChange);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable GiftCoupon = ds.Tables["GiftCoupon"];
                                                    if (GiftCoupon != null)
                                                    {
                                                        GiftCoupon.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        GiftCoupon.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in GiftCoupon.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "GiftCoupon";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("LegacyId", "LegacyId");
                                                        bc.ColumnMappings.Add("OperatorDefined", "OperatorDefined");
                                                        bc.ColumnMappings.Add("Amount", "Amount");
                                                        bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                        bc.WriteToServer(GiftCoupon);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable OtherPayments = ds.Tables["OtherPayments"];
                                                    if (OtherPayments != null)
                                                    {
                                                        //OtherPayments.Columns.Add(new DataColumn()
                                                        //{
                                                        //    ColumnName = "STLD_Location",
                                                        //    DataType = typeof(string)
                                                        //});

                                                        //OtherPayments.Columns.Add(new DataColumn()
                                                        //{
                                                        //    ColumnName = "business_date",
                                                        //    DataType = typeof(string)
                                                        //});
                                                        //foreach (DataRow vrow in OtherPayments.Rows)
                                                        //{

                                                        //    vrow["STLD_Location"] = lbl_location.Text;
                                                        //    vrow["business_date"] = lbl_business.Text;

                                                        //}
                                                        //bc.DestinationTableName = "OtherPayments";
                                                        //bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        //bc.ColumnMappings.Add("business_date", "business_date");
                                                        //bc.ColumnMappings.Add("LegacyId", "LegacyId");
                                                        //bc.ColumnMappings.Add("OperatorDefined", "OperatorDefined");
                                                        //bc.ColumnMappings.Add("Amount", "Amount");
                                                        //bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                        //bc.WriteToServer(OtherPayments);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable ForeignCurrency = ds.Tables["ForeignCurrency"];
                                                    if (ForeignCurrency != null)
                                                    {
                                                        ForeignCurrency.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        ForeignCurrency.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in ForeignCurrency.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "ForeignCurrency";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("LegacyId", "LegacyId");
                                                        bc.ColumnMappings.Add("ExchangeRate", "ExchangeRate");
                                                        bc.ColumnMappings.Add("Precision", "Precision");
                                                        bc.ColumnMappings.Add("Rounding", "Rounding");
                                                        bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                        bc.WriteToServer(ForeignCurrency);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable ElectronicPayment = ds.Tables["ElectronicPayment"];
                                                    if (ElectronicPayment != null)
                                                    {
                                                        ElectronicPayment.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        ElectronicPayment.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in ElectronicPayment.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "ElectronicPayment";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("LegacyId", "LegacyId");
                                                        bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                        bc.WriteToServer(ElectronicPayment);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable CreditSales = ds.Tables["CreditSales"];
                                                    if (CreditSales != null)
                                                    {
                                                        CreditSales.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        CreditSales.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in CreditSales.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "CreditSales";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("LegacyId", "LegacyId");
                                                        bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                        bc.WriteToServer(CreditSales);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable Trx_DayPart = ds.Tables["Trx_DayPart"];
                                                    if (Trx_DayPart != null)
                                                    {
                                                        Trx_DayPart.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        Trx_DayPart.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in Trx_DayPart.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        //bc.DestinationTableName = "Trx_DayPart";
                                                        //bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        //bc.ColumnMappings.Add("business_date", "business_date");
                                                        //bc.ColumnMappings.Add("LegacyId", "LegacyId");
                                                        //bc.ColumnMappings.Add("TenderType_Id", "TenderType_Id");
                                                        //bc.WriteToServer(Trx_DayPart);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable TRX_BaseConfig = ds.Tables["TRX_BaseConfig"];
                                                    if (TRX_BaseConfig != null)
                                                    {
                                                        TRX_BaseConfig.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        TRX_BaseConfig.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in TRX_BaseConfig.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "TRX_BaseConfig";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("TRX_BaseConfig_Id", "TRX_BaseConfig_Id");
                                                        bc.ColumnMappings.Add("POS", "POS");
                                                        bc.ColumnMappings.Add("POD", "POD");
                                                        bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                        bc.WriteToServer(TRX_BaseConfig);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable Config = ds.Tables["Config"];
                                                    if (Config != null)
                                                    {
                                                        Config.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        Config.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in Config.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "Config";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("MenuPriceBasis", "MenuPriceBasis");
                                                        bc.ColumnMappings.Add("WeekEndBreakfastStartTime", "WeekEndBreakfastStartTime");
                                                        bc.ColumnMappings.Add("WeekEndBreakfastStopTime", "WeekEndBreakfastStopTime");
                                                        bc.ColumnMappings.Add("WeekDayBreakfastStartTime", "WeekDayBreakfastStartTime");
                                                        bc.ColumnMappings.Add("WeekDayBreakfastStopTime", "WeekDayBreakfastStopTime");
                                                        bc.ColumnMappings.Add("DecimalPlaces", "DecimalPlaces");
                                                        bc.ColumnMappings.Add("CheckRefund", "CheckRefund");
                                                        bc.ColumnMappings.Add("GrandTotalFlag", "GrandTotalFlag");
                                                        bc.ColumnMappings.Add("StoreId", "StoreId");
                                                        bc.ColumnMappings.Add("StoreName", "StoreName");
                                                        bc.ColumnMappings.Add("AcceptNegativeQty", "AcceptNegativeQty");
                                                        bc.ColumnMappings.Add("AcceptZeroPricePMix", "AcceptZeroPricePMix");
                                                        bc.ColumnMappings.Add("FloatPriceTenderId", "FloatPriceTenderId");
                                                        bc.ColumnMappings.Add("MinCirculatingAmount", "MinCirculatingAmount");
                                                        bc.ColumnMappings.Add("TRX_BaseConfig_Id", "TRX_BaseConfig_Id");
                                                        bc.WriteToServer(Config);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable POSConfig = ds.Tables["POSConfig"];
                                                    if (POSConfig != null)
                                                    {
                                                        POSConfig.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        POSConfig.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in POSConfig.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "POSConfig";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("CountTCsFullDiscEM", "CountTCsFullDiscEM");
                                                        bc.ColumnMappings.Add("RefundBehaviour", "RefundBehaviour");
                                                        bc.ColumnMappings.Add("OverringBehaviour", "OverringBehaviour");
                                                        bc.ColumnMappings.Add("TRX_BaseConfig_Id", "TRX_BaseConfig_Id");
                                                        bc.WriteToServer(POSConfig);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable TRX_SetSMState = ds.Tables["TRX_SetSMState"];
                                                    if (TRX_SetSMState != null)
                                                    {
                                                        TRX_SetSMState.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        TRX_SetSMState.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in TRX_SetSMState.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "TRX_SetSMState";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("POSState", "POSState");
                                                        bc.ColumnMappings.Add("CrewId", "CrewId");
                                                        bc.ColumnMappings.Add("CrewName", "CrewName");
                                                        bc.ColumnMappings.Add("CrewSecurityLevel", "CrewSecurityLevel");
                                                        bc.ColumnMappings.Add("LoginTime", "LoginTime");
                                                        bc.ColumnMappings.Add("LogoutTime", "LogoutTime");
                                                        bc.ColumnMappings.Add("InitialFloat", "InitialFloat");
                                                        bc.ColumnMappings.Add("PODId", "PODId");
                                                        bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                        bc.WriteToServer(TRX_SetSMState);

                                                    }

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {


                                                    DataTable TRX_InitGTotal = ds.Tables["TRX_InitGTotal"];
                                                    if (TRX_InitGTotal != null)
                                                    {
                                                        TRX_InitGTotal.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "STLD_Location",
                                                            DataType = typeof(string)
                                                        });

                                                        TRX_InitGTotal.Columns.Add(new DataColumn()
                                                        {
                                                            ColumnName = "business_date",
                                                            DataType = typeof(string)
                                                        });
                                                        foreach (DataRow vrow in TRX_InitGTotal.Rows)
                                                        {

                                                            vrow["STLD_Location"] = lbl_location.Text;
                                                            vrow["business_date"] = lbl_business.Text;

                                                        }
                                                        bc.DestinationTableName = "TRX_InitGTotal";
                                                        bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                        bc.ColumnMappings.Add("business_date", "business_date");
                                                        bc.ColumnMappings.Add("amount", "amount");
                                                        bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                        bc.WriteToServer(TRX_InitGTotal);

                                                    }

                                                }



                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {
                                                    DataTable TLD = ds.Tables["TLD"];
                                                    bc.DestinationTableName = "TLD";
                                                    bc.ColumnMappings.Add("TLD_Id", "TLD_Id");
                                                    bc.ColumnMappings.Add("LogVersion", "LogVersion");
                                                    bc.ColumnMappings.Add("storeId", "storeId");
                                                    bc.ColumnMappings.Add("businessDate", "businessDate");
                                                    bc.ColumnMappings.Add("swVersion", "swVersion");
                                                    bc.ColumnMappings.Add("checkPoint", "checkPoint");
                                                    bc.ColumnMappings.Add("end", "end");
                                                    bc.ColumnMappings.Add("productionStatus", "productionStatus");
                                                    bc.ColumnMappings.Add("hasMoreContent", "hasMoreContent");
                                                    bc.WriteToServer(TLD);

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {
                                                    DataTable Node = ds.Tables["Node"];
                                                    Node.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    Node.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in Node.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "Node";
                                                    bc.ColumnMappings.Add("Node_Id", "Node_Id");
                                                    bc.ColumnMappings.Add("TLD_Id", "TLD_Id");
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("nodeStatus", "nodeStatus");
                                                    bc.ColumnMappings.Add("id", "id");
                                                    bc.WriteToServer(Node);

                                                }
                                                using (SqlBulkCopy bc = new SqlBulkCopy(con, SqlBulkCopyOptions.Default, tran))
                                                {
                                                    DataTable Event = ds.Tables["Event"];
                                                    Event.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "STLD_Location",
                                                        DataType = typeof(string)
                                                    });

                                                    Event.Columns.Add(new DataColumn()
                                                    {
                                                        ColumnName = "business_date",
                                                        DataType = typeof(string)
                                                    });
                                                    foreach (DataRow vrow in Event.Rows)
                                                    {

                                                        vrow["STLD_Location"] = lbl_location.Text;
                                                        vrow["business_date"] = lbl_business.Text;

                                                    }
                                                    bc.DestinationTableName = "Event";
                                                    bc.ColumnMappings.Add("STLD_Location", "STLD_Location");
                                                    bc.ColumnMappings.Add("business_date", "business_date");
                                                    bc.ColumnMappings.Add("Event_Id", "Event_Id");
                                                    //  bc.ColumnMappings.Add("TRX_UnaDrawerOpening", "TRX_UnaDrawerOpening");
                                                    bc.ColumnMappings.Add("RegId", "RegId");
                                                    bc.ColumnMappings.Add("Type", "Type");
                                                    bc.ColumnMappings.Add("Time", "Time");
                                                    bc.ColumnMappings.Add("Node_Id", "Node_Id");
                                                    bc.WriteToServer(Event);

                                                }





                                                string smt = "INSERT INTO [STLD].[dbo].[STLDPROCESS_STATUS] ([STLD_Location] ,[business_date]) VALUES (@STLD_Location,@business_date)";
                                                SqlCommand cmd = new SqlCommand(smt, con, tran);
                                                cmd.Parameters.AddWithValue("@STLD_Location", SqlDbType.VarChar).Value = lbl_location.Text;
                                                cmd.Parameters.AddWithValue("@business_date", SqlDbType.VarChar).Value = lbl_business.Text;
                                                cmd.ExecuteNonQuery();
                                                string timeforstld = DateTime.Now.ToString("h:mm:ss tt");
                                                File.Move(file, Path.ChangeExtension(file, ".Proceed"));

                                                //////////////////////

                                                tran.Commit();
                                             


                                            }
                                            catch
                                            {
                                                tran.Rollback();
                                               

                                                // throw;
                                            }

                                        }

                                    }









                                }
                            }

                            //////////////////////////////////////////////////////////////////

                        }
                        else
                        {
                            //File.Delete(file);
                        }
                        File.Delete(file111);
                        try
                        {

                        }
                        catch
                        {

                        }




                    }



                }

                catch 
                {
                    continue;
                }


           
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void BTN_CLEARLBLES_Click(object sender, EventArgs e)
        {
            lbl_location.Text = "";
            lblCount.Text="WRONGSHJ";
            lbl_business.Text = ""; 

        }

        private void btn_Product_Click(object sender, EventArgs e)
        {
           
        }
    }

}

