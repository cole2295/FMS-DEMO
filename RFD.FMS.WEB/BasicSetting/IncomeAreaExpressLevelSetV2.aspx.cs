using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util;
using RFD.FMS.WEB.Main;
using System.Data;
using RFD.FMS.MODEL.BasicSetting;
using RFD.FMS.MODEL.Enumeration;
using System.Text;

namespace RFD.FMS.WEB.BasicSetting
{
    public partial class IncomeAreaExpressLevelSetV2 : BasePage
    {
        readonly IAreaExpressLevelIncomeService areaExpressLevelIncomeService = ServiceLocator.GetService<IAreaExpressLevelIncomeService>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitForm();
                Master.DistributionCode = base.DistributionCode;
               // UCExpressCompanyTV.Editable = false;
            }
        }

        private void InitForm()
        {
            IStatusCodeService statusCodeService=ServiceLocator.GetService<IStatusCodeService>();
            statusCodeService.BindDropDownListByCodeType(ddlSetAreaType, "请选择", "", "AreaType", base.DistributionCode);
        }     

        protected void btnImport_Click(object sender, EventArgs e)
        {
            try
            {

                if (!fuExprot.HasFile)
                {
                    Alert("请选择要导入的文件");
                    return;
                }
                string path = "~/file/UpFile" + fuExprot.FileName.ToString(CultureInfo.InvariantCulture).Trim();
                path = Server.MapPath(path);
                this.fuExprot.SaveAs(path);
                DataSet ds = Excel.ExcelToDataSetFor03And07(path);
                var checks = ds.Tables[0].AsEnumerable().Where(r => !string.IsNullOrWhiteSpace(r[0].ToString())).ToList();
                if (checks.Count > 7000)
                {
                    Alert("已经超出导入上限7000，不允许导入");
                    return;
                }
                var areaExpressLevelBll = ServiceLocator.GetService<IAreaExpressLevelIncomeService>();
                DataTable dtError;
                if (areaExpressLevelBll.ExportAreaType(ds.Tables[0], Userid, out dtError, base.DistributionCode, Master.ExpressCompanyID))
                {
                    if (dtError != null && dtError.Rows.Count > 0)
                    {
                        ExportExcel(dtError, (from DataColumn column in dtError.Columns select column.ColumnName).ToArray(), null, "收入区域类型导入错误反馈");
                    }
                    else
                        Alert("导入完成");
                }
                else
                {
                    Alert("导入失败");
                }
            }
            catch (Exception ex)
            {
                Alert("导入失败<br>" + ex.Message);
            }
        }

       

        protected void btnAddAreaType_Click(object sender, EventArgs e)
        {
            try
            {
                IList<KeyValuePair<DataKey, GridViewRow>> keyValuePairs = Master.GridViewChecked;
                if (!JudgeInput(keyValuePairs, 0))
                    return;
                string goodsCategory = UCGoodsCategoryCheckBoxSet.SelectCategoryID;
                string expresscompanyid = Master.ExpressCompanyID.ToString(CultureInfo.InvariantCulture);
                if (UCExpressCompanyTV.GetCheckStatus)
                {
                    expresscompanyid = UCExpressCompanyTV.SelectExpressID;
                }
                var areaExpressLevelIncomes = new List<AreaExpressLevelIncome>();
                foreach (var keyPair in keyValuePairs)
                {
                    var dk=keyPair.Key as DataKey;
                    GridViewRow gvr = keyPair.Value;

                    var areaExpressLevelIncome = new AreaExpressLevelIncome
                                                     {
                                                         DistributionCode = base.DistributionCode,
                                                         AreaID = dk.Values[1].ToString(),
                                                         AreaName = gvr.Cells[4].Text,
                                                         ExpressCompanyID = int.Parse(expresscompanyid),
                                                         IsExpress = UCExpressCompanyTV.GetCheckStatus ? 1 : 0,
                                                         CompanyName = gvr.Cells[5].Text,
                                                         MerchantID = int.Parse(dk.Values[3].ToString()),
                                                         WareHouseID = dk.Values[2].ToString(),
                                                         MerchantName = gvr.Cells[6].Text,
                                                         EffectAreaType = int.Parse(ddlSetAreaType.SelectedValue),
                                                         Enable = (int) AreaLevelEnable.E3,
                                                         CreateBy = base.Userid,
                                                         AuditStatus = (int) AreaLevelStatus.S1,
                                                         ExpressName = UCExpressCompanyTV.GetCheckStatus ? UCExpressCompanyTV.SelectExpressName :"全部",

                                                     };
                    //areaExpressLevelIncome.ExpressCompanyID = Master.ExpressCompanyID;
                    if (UCGoodsCategoryCheckBoxSet.Visible && !string.IsNullOrEmpty(goodsCategory))
                    {
                        string[] goodsCategorys = goodsCategory.Split(',');
                        string[] categorysName = UCGoodsCategoryCheckBoxSet.SelectCategoryNames.Split(',');
                        int n = 0;
                        foreach (string category in goodsCategorys)
                        {
                            var newModel = new AreaExpressLevelIncome();
                            areaExpressLevelIncomeService.MapAreaExpressLevelIncome(areaExpressLevelIncome, ref newModel);
                            newModel.GoodsCategoryCode = category;
                            newModel.GoodsCategoryName = categorysName[n];
                            areaExpressLevelIncomes.Add(newModel);
                            n++;
                        }
                    }
                    else
                    {
                        areaExpressLevelIncomes.Add(areaExpressLevelIncome);
                    }
                }
                if (areaExpressLevelIncomes.Count <= 0)
                {
                    Alert("未能找到添加行");
                    return;
                }
                string msg;
                if (areaExpressLevelIncomeService.AddAreaType(areaExpressLevelIncomes, out msg))
                {
                    Master.SearchData(Master.UCPager.CurrentPageIndex);
                    if (!string.IsNullOrEmpty(msg))
                        Alert("添加成功，以下为提示信息：<br>" + msg);
                    else
                        Alert("添加成功");
                }
                else
                    Alert("添加失败");
            }
            catch (Exception ex)
            {
                Alert("添加失败<br>" + ex.Message);
            }
        }

        protected void btnEditAreaType_Click(object sender, EventArgs e)
        {
            try
            {
                IList<KeyValuePair<DataKey, GridViewRow>> keyValuePairs = Master.GridViewChecked;
                if (!JudgeInput(keyValuePairs, 1))
                    return;

                var areaExpressLevelIncomes = new List<AreaExpressLevelIncome>();
                foreach (var keyPair in keyValuePairs)
                {
                    var dk = keyPair.Key as DataKey;

                    var areaExpressLevelIncome = new AreaExpressLevelIncome
                                                     {
                                                         AutoId = int.Parse(dk.Values[0].ToString()),
                                                         EffectAreaType = int.Parse(ddlSetAreaType.SelectedValue)
                                                     };
                  
                    areaExpressLevelIncome.UpdateBy = base.Userid;
                    areaExpressLevelIncome.AuditStatus = (int)AreaLevelStatus.S1;
                    areaExpressLevelIncomes.Add(areaExpressLevelIncome);
                }
                if (areaExpressLevelIncomes.Count <= 0)
                {
                    Alert("未能找到更新元素");
                    return;
                }
                var areaExpressLevelIncomeService = ServiceLocator.GetService<IAreaExpressLevelIncomeService>();
                if (areaExpressLevelIncomeService.UpdateAreaTypeV2(areaExpressLevelIncomes))
                {
                    Master.SearchData(Master.UCPager.CurrentPageIndex);
                    Alert("更新成功");
                }
                else
                    Alert("选择的区域类型与已经设置的行存在重复");
            }
            catch (Exception ex)
            {
                Alert("更新失败<br>" + ex.Message);
            }
        }

       

        public bool JudgeInput(IList<KeyValuePair<DataKey, GridViewRow>> keyValuePairs, int n)
        {
            const int auditIndex = 12;
           
            if (n == 0 || n == 1)
            {
                if (ddlSetAreaType.SelectedIndex == 0)
                {
                    Alert("没有选择区域类型");
                    return false;
                }

                if (Master.IsCategory && n == 0)
                {
                    if (UCGoodsCategoryCheckBoxSet.IsSelectAll&&UCGoodsCategoryCheckBoxSet.SelectCategoryID.Split(',').Length!=1)
                    {
                        Alert("不能全选货物品类");
                        return false;
                    }
                    if (string.IsNullOrEmpty(UCGoodsCategoryCheckBoxSet.SelectCategoryID))
                    {
                        Alert("没有选择货物品类");
                        return false;
                    }
                }
                if (UCExpressCompanyTV.GetCheckStatus)
                {

                    if (string.IsNullOrEmpty(UCExpressCompanyTV.SelectExpressName))
                    {
                        Alert("没有选择配送商");
                        return false;
                    }
                    else
                    {
                        if (UCExpressCompanyTV.SelectExpressName == "全部")
                        {
                            Alert("区分配送商不能选择全部");
                            return false;
                        }
                    }
                }
            }

            if (keyValuePairs.Count <= 0)
            {
                Alert(string.Format("请选择需要操作的行"));
                return false;
            }

            if (n == 1 || n == 2)
            {
                if (keyValuePairs.Any(k => k.Value.Cells[auditIndex].Text == EnumHelper.GetDescription((AreaLevelStatus.S0))))
                {
                    Alert("请先设置区域类型");
                    return false;
                }
            }

            return true;
        }

        public bool JudgeInputExpress(IList<KeyValuePair<DataKey, GridViewRow>> keyValuePairs, int n)
        {
            const int auditIndex = 12;
            if (n == 0 || n == 1)
            {
                if (UCExpressCompanyTV.GetCheckStatus)
                {

                    if (string.IsNullOrEmpty(UCExpressCompanyTV.SelectExpressName))
                    {
                        Alert("没有选择配送商");
                        return false;
                    }
                    else
                    {
                        if (UCExpressCompanyTV.SelectExpressName == "全部")
                        {
                            Alert("区分配送商不能选择全部");
                            return false;
                        }
                    }
                }
                else
                {
                    ExpressId = 11;
                    ExpressName = "全部";
                }
            }

            if (keyValuePairs.Count <= 0)
            {
                Alert(string.Format("请选择需要操作的行"));
                return false;
            }

            if (n == 0)
            {
                 if (keyValuePairs.Any(k => k.Value.Cells[auditIndex].Text != EnumHelper.GetDescription((AreaLevelStatus.S0))))
                    {
                        Alert("只能选择待维护状态的进行设置");
                        return false;
                    }
              
            }

            if (n == 1 || n == 2)
            {
                if (keyValuePairs.Any(k => k.Value.Cells[auditIndex].Text == EnumHelper.GetDescription((AreaLevelStatus.S0))))
                {
                    Alert("请先设置区域配送商信息");
                    return false;
                }
            }

            return true;
        }
        protected void btnDeleteAreaType_Click(object sender, EventArgs e)
        {
            try
            {
                IList<KeyValuePair<DataKey, GridViewRow>> keyValuePairs = Master.GridViewChecked;
                if (!JudgeInput(keyValuePairs, 2))
                    return;

                IList<KeyValuePair<string, string>> deleteList = new List<KeyValuePair<string, string>>();
                foreach (var dk in keyValuePairs.Select(k => k.Key as DataKey))
                {
                    if (string.IsNullOrEmpty(dk.Values[0].ToString()))
                    {
                        Alert("唯一键获取失败");
                        return;
                    }
                    deleteList.Add(new KeyValuePair<string, string>(dk.Values[0].ToString(), dk.Values[0].ToString()));
                }

                var areaExpressLevelIncomeService = ServiceLocator.GetService<IAreaExpressLevelIncomeService>();
                if (areaExpressLevelIncomeService.DeleteAreaType(deleteList, base.Userid))
                {
                    Master.SearchData(Master.UCPager.CurrentPageIndex);
                    Alert("删除成功");
                }
                else
                    Alert("删除失败");
            }
            catch(Exception ex)
            {
                Alert("删除失败<br>" + ex.Message);
            }
        }

        protected void btnEditExpress_Click(object sender, EventArgs e)
        {
            try
            {
                IList<KeyValuePair<DataKey, GridViewRow>> keyValuePairs = Master.GridViewChecked;
                if (!JudgeInputExpress(keyValuePairs, 1))
                    return;

                var areaExpressLevelIncomes = new List<AreaExpressLevelIncome>();
                foreach (var keyPair in keyValuePairs)
                {
                    var dk = keyPair.Key as DataKey;

                    var areaExpressLevelIncome = new AreaExpressLevelIncome
                    {
                        AutoId = int.Parse(dk.Values[0].ToString()),
                    };
                    if (UCExpressCompanyTV.GetCheckStatus  )
                    {  
                         areaExpressLevelIncome.IsExpress = 1;
                        if (!string.IsNullOrEmpty(UCExpressCompanyTV.SelectExpressName) )
                        {
                          
                            areaExpressLevelIncome.ExpressCompanyID = int.Parse(UCExpressCompanyTV.SelectExpressID);
                            areaExpressLevelIncome.ExpressName = UCExpressCompanyTV.SelectExpressName;
                        }
                        else
                        {
                            areaExpressLevelIncome.ExpressCompanyID =Master.ExpressCompanyID;
                            areaExpressLevelIncome.ExpressName =Master.CompanyName;  
                        }
                        
                    }
                    else
                    {
                        areaExpressLevelIncome.IsExpress = 0;
                        areaExpressLevelIncome.ExpressCompanyID =11;
                        areaExpressLevelIncome.ExpressName ="全部";  
                    }
                    areaExpressLevelIncome.UpdateBy = base.Userid;
                    areaExpressLevelIncome.AuditStatus = (int)AreaLevelStatus.S1;
                    areaExpressLevelIncomes.Add(areaExpressLevelIncome);
                }
                if (areaExpressLevelIncomes.Count <= 0)
                {
                    Alert("未能找到更新元素");
                    return;
                }
                var areaExpressLevelIncomeService = ServiceLocator.GetService<IAreaExpressLevelIncomeService>();
                if (areaExpressLevelIncomeService.UpdateExpressV2(areaExpressLevelIncomes))
                {
                    Master.SearchData(Master.UCPager.CurrentPageIndex);
                    Alert("更新成功");
                }
                else
                    Alert("选择的配送商与已经设置的行存在重复");
            }
            catch (Exception ex)
            {
                Alert("更新失败<br>" + ex.Message);
            }
        }
    }
}