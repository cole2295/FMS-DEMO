using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFD.FMS.Service.COD;
using RFD.FMS.MODEL.COD;
using RFD.FMS.Domain.COD;
using System.Data;

namespace RFD.FMS.WEBLOGIC.COD
{
    public class DistributionFeeService : IDistributionFeeService
    {
        private IDistributionFeeDao _distributionFeeDao;

        #region error code

        private static readonly string _errorMsg_00 = "成功";
        private static readonly string _errorCode_00 = "E00";

        private static readonly string _errorMsg_01 = "查询信息未找到";
        private static readonly string _errorCode_01 = "E01";

        private static readonly string _errorMsg_02 = "查询信息未找到:调用方";
        private static readonly string _errorCode_02 = "E02";

        private static readonly string _errorMsg_03 = "查询信息未找到:订单号";
        private static readonly string _errorCode_03 = "E03";

        private static readonly string _errorMsg_04 = "查询信息未找到:配送公司";
        private static readonly string _errorCode_04 = "E04";

        private static readonly string _errorMsg_05 = "查询信息未找到:商家编码";
        private static readonly string _errorCode_05 = "E05";

        private static readonly string _errorMsg_06 = "查询信息未找到:状态";
        private static readonly string _errorCode_06 = "E06";

        private static readonly string _errorMsg_07 = "配送费未取到";
        private static readonly string _errorCode_07 = "E07";

        #endregion

        public DistributionFeeDTO GetistributionFee(DistributionFeeDTO searchModel)
        {
            DistributionFeeDTO dto = JudgeInput(searchModel);

            if (!dto.IsSuccess)
                return null;

            GetFare(ref dto);

            return dto;
        }

        public DistributionFeeDTO GetistributionFeeV2(DistributionFeeDTO searchModel)
        {
            DistributionFeeDTO dto = JudgeInput(searchModel);

            if (!dto.IsSuccess)
                return null;

            GetFareV2(ref dto);

            return dto;
        }

        private void GetFareV2(ref DistributionFeeDTO dto)
        {
            DataTable dtFare = _distributionFeeDao.GetFareV2(dto);

            //没有结果时
            if (dtFare == null || dtFare.Rows.Count <= 0)
            {
                dto.IsSuccess = false;
                dto.ErrorMsg = "获取配送费失败";
                dto.Fare = 0M;
            }
            else
            {
                dto.IsSuccess = true;
                dto.ErrorMsg = "";
                decimal fare = 0M;
                string companyName = dtFare.Rows[0]["AccountCompanyName"].ToString();
                if (companyName.Contains("宅急送"))//全算配送费
                {
                    fare = UniteFare(dtFare, true);

                }
                else if (companyName.Contains("邮政速递"))//只算发货
                {
                    fare = UniteFare(dtFare, false);
                }
                else
                {
                    //拒收时发货也不计
                    if (dto.MerchantID == 8 && dto.Status == -6)
                        fare = 0M;
                    else if (dto.MerchantID == 9 && dto.Status == -6)
                        fare = 0M;
                    else if (dto.MerchantID != 9 && dto.MerchantID != 8 && dto.Status == 5)
                        fare = 0M;
                    else
                        fare = UniteFare(dtFare, false);//这里才计费 只有妥投才计算
                }

                if (fare == 0M)
                {
                    dto.IsSuccess = false;
                    dto.ErrorMsg = "配送费合并失败";
                }
                dto.Fare = fare;
            }
        }

        /// <summary>
        /// 获取配送费
        /// </summary>
        /// <param name="dto"></param>
        private void GetFare(ref DistributionFeeDTO dto)
        {
            DataTable dtFare = _distributionFeeDao.GetFare(dto);
            //没有结果时
            if (dtFare == null || dtFare.Rows.Count <= 0)
            {
                dto.IsSuccess = false;
                dto.ErrorMsg = _errorMsg_07;
                dto.ErrorCode = _errorCode_07;
                dto.Fare = -1M;
            }
            else
            {
                decimal fare=-1M;
                string companyName = dtFare.Rows[0]["AccountCompanyName"].ToString();
                if (companyName.Contains("宅急送"))//全算配送费
                {
                    fare = UniteFare(dtFare, true);
                }
                else if (companyName.Contains("邮政速递"))//只算发货
                {
                    fare = UniteFare(dtFare, false);
                }
                else
                {
                    //拒收时发货也不计
                    if (dto.Source == 0 && dto.Status == 24)//vancl
                        fare = 0M;
                    else if (dto.Source == 1 && dto.Status == 24)//vjia
                        fare = 0M;
                    else if (dto.Source ==2 && dto.Status == 5)//rfd
                        fare = 0M;
                    else
                        fare = UniteFare(dtFare, false);//这里才计费 只有妥投才计算
                }

                if (fare == -1M)
                {
                    dto.IsSuccess = false;
                    dto.ErrorMsg = _errorMsg_07;
                    dto.ErrorCode = _errorCode_07;
                }

                dto.IsSuccess = true;
                dto.ErrorMsg = _errorMsg_00;
                dto.ErrorCode = _errorCode_00;
                dto.Fare = fare;
            }
        }

        private decimal UniteFare(DataTable dtFare,bool isAllFare)
        {
            decimal fare = 0M;
            //只有一条结果时
            if (dtFare.Rows.Count == 1)
            {
                if (int.Parse(dtFare.Rows[0]["IsFare"].ToString()) == 1)
                    fare = Convert.ToDecimal(dtFare.Rows[0]["Fare"].ToString());
                else
                {
                    fare = -1M;
                }
            }
            else
            {
                if (isAllFare)
                {
                    DataRow[] drs = dtFare.Select("IsFare=1");
                    if (drs.Length == 0)
                    {
                        fare = -1M;
                    }
                    foreach (DataRow dr in drs)
                    {
                        if (int.Parse(dr["OperateType"].ToString()) == 4)
                            fare += Convert.ToDecimal(dr["Fare"].ToString()) * 0.8M;//0.8折
                        else
                            fare += Convert.ToDecimal(dr["Fare"].ToString());
                    }
                }
                else
                {
                    DataRow[] drs = dtFare.Select("IsFare=1 and OperateType<>4");
                    if (drs.Length == 0)
                    {
                        fare = -1M;
                    }
                    foreach (DataRow dr in drs)
                    {
                        if (int.Parse(dr["OperateType"].ToString()) == 2)
                            fare -= Convert.ToDecimal(dr["Fare"].ToString());
                        else
                            fare += Convert.ToDecimal(dr["Fare"].ToString());
                    }
                }
            }

            return fare;
        }

        /// <summary>
        /// 验证输入信息
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        private DistributionFeeDTO JudgeInput(DistributionFeeDTO searchModel)
        {
            searchModel.IsSuccess = true;
            long formCode = 0;
            if (!long.TryParse(searchModel.FormCode.ToString(),out formCode))
            {
                searchModel.IsSuccess = false;
                searchModel.ErrorMsg = _errorMsg_01;
                searchModel.ErrorCode = _errorCode_01;
            }

            if (string.IsNullOrEmpty(searchModel.Source.ToString()) || searchModel.Source < 0)
            {
                searchModel.IsSuccess = false;
                searchModel.ErrorMsg = _errorMsg_02;
                searchModel.ErrorCode = _errorCode_02;
            }

            if (string.IsNullOrEmpty(searchModel.FormCode.ToString()) || searchModel.FormCode <= 0)
            {
                searchModel.IsSuccess = false;
                searchModel.ErrorMsg = _errorMsg_03;
                searchModel.ErrorCode = _errorCode_03;
            }

            if (string.IsNullOrEmpty(searchModel.ExpressCompanyID.ToString()) || searchModel.ExpressCompanyID <= 0)
            {
                searchModel.IsSuccess = false;
                searchModel.ErrorMsg = _errorMsg_04;
                searchModel.ErrorCode = _errorCode_04;
            }

            if (string.IsNullOrEmpty(searchModel.MerchantID.ToString()) || searchModel.MerchantID <= 0)
            {
                searchModel.IsSuccess = false;
                searchModel.ErrorMsg = _errorMsg_05;
                searchModel.ErrorCode = _errorCode_04;
            }

            if (string.IsNullOrEmpty(searchModel.Status.ToString()))
            {
                searchModel.IsSuccess = false;
                searchModel.ErrorMsg = _errorMsg_06;
                searchModel.ErrorCode = _errorCode_06;
            }

            if (!searchModel.IsSuccess)
                return searchModel;

            searchModel.ErrorMsg = "";
            return searchModel;
        }
    }
}
