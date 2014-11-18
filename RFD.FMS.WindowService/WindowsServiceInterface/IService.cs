using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsServiceInterface
{
    public interface IService
    {
        /// <summary>
        /// 细节处理方法
        /// </summary>
        void DealDetail(TaskModel taskModel);
    }
}
