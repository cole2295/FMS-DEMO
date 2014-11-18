using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.MODEL
{
    /*
* ***********************
* 通用分页class ( from lms)
* Terry
* 2011-09-08
* ***********************
*/
    public class PageInfo
    {
        private int m_PageSize;
        private int m_PageCount;
        private int m_CurrentPageIndex = 1;
        private int m_ItemCount;
        private int m_CurrentPageBeginItemIndex;
        private int m_CurrentPageItemCount;

        private int m_CurrentPageStartRowNum;
        private int m_CurrentPageEndRowNum;

        /// <summary>
        /// 构造一个分页信息。
        /// </summary>
        /// <param name="pageSize">每页条数</param>
        public PageInfo(int pageSize)
        {
            m_PageSize = pageSize;
        }

        /// <summary>
        /// 界面层使用此方法设置希望获取的页码。(从1开始，小于1认为是最后一页)
        /// </summary>
        /// <param name="pageIndex">页码</param>
        public void SetWantedPageIndex(int pageIndex)
        {
            m_CurrentPageIndex = pageIndex;
        }

        /// <summary>
        /// 逻辑层使用此方法设置实际条数，并计算实际的当前页码。
        /// </summary>
        /// <param name="itemCount">记录条数</param>
        public void SetItemCount(int itemCount)
        {
            if (itemCount == 0)
            {
                m_ItemCount = 0;
                m_PageCount = 0;
                m_CurrentPageIndex = 0;
                m_CurrentPageBeginItemIndex = 0;
                m_CurrentPageItemCount = 0;
                return;
            }

            m_ItemCount = itemCount;
            m_PageCount = m_ItemCount / m_PageSize;
            if (m_ItemCount % m_PageSize > 0)
            {
                m_PageCount++;	//向上取整
            }

            //超出实际页数，则取最后一页。
            m_CurrentPageIndex = m_CurrentPageIndex < 1 ? m_PageCount : Math.Min(m_PageCount, m_CurrentPageIndex);
            m_CurrentPageBeginItemIndex = (m_CurrentPageIndex - 1) * m_PageSize + 1;
            //最后一页不一定达到每页条数
            m_CurrentPageItemCount = Math.Min(m_PageSize, m_ItemCount - m_CurrentPageBeginItemIndex + 1);
        }

        /// <summary>
        /// 每页条数。
        /// </summary>

        public int PageSize
        {
            get { return m_PageSize; }
            set { m_PageSize = value; }
        }

        /// <summary>
        /// 总页数。
        /// </summary>

        public int PageCount
        {
            get { return m_PageCount > 0 ? m_PageCount : Convert.ToInt32(Math.Ceiling(ItemCount * 1.0 / PageSize)); }
            set { m_PageCount = value; }
        }

        /// <summary>
        /// 当前页。(从1开始)
        /// </summary>

        public int CurrentPageIndex
        {
            get { return m_CurrentPageIndex; }
            set { m_CurrentPageIndex = value; }
        }

        /// <summary>
        /// 总条数。
        /// </summary>

        public int ItemCount
        {
            get { return m_ItemCount; }
            set { m_ItemCount = value; }
        }

        /// <summary>
        /// 当前页起始记录号。(从1开始)
        /// </summary>

        public int CurrentPageBeginItemIndex
        {
            get { return m_CurrentPageBeginItemIndex; }
            set { m_CurrentPageBeginItemIndex = value; }
        }

        /// <summary>
        /// 当前页条数。
        /// </summary>

        public int CurrentPageItemCount
        {
            get { return m_CurrentPageItemCount; }
            set { m_CurrentPageItemCount = value; }
        }

        /// <summary>
        /// 当前页开始记录编号
        /// </summary>
        public int CurrentPageStartRowNum
        {
            get { return m_CurrentPageStartRowNum > 0 ? m_CurrentPageStartRowNum : (CurrentPageIndex - 1) * PageSize + 1; }
            set { m_CurrentPageStartRowNum = value; }
        }

        /// <summary>
        /// 当前页结束记录编号
        /// </summary>
        public int CurrentPageEndRowNum
        {
            get { return m_CurrentPageEndRowNum > 0 ? m_CurrentPageEndRowNum : CurrentPageStartRowNum + PageSize - 1; }
            set { m_CurrentPageEndRowNum = value; }
        }
    }
}
