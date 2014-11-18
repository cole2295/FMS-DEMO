using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LMS.FMS.DAL;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.WEB.Main;
using System.Text;
using LMS.Util;

namespace RFD.FMS.WEB.test.workflow
{
    public partial class UCRuleFee : BaseUserControl, IRuleUI
    {
        #region IRuleUI 成员

        public event Action<RuleParameter> DoSave;

        public int NodeId
        {
            get { return DataConvert.ToInt(ViewState["nodeId"]); }
            set { ViewState["nodeId"] = value; }
        }

        public int RuleType
        {
            get { return DataConvert.ToInt(ViewState["RuleType"]); }
            set { ViewState["RuleType"] = value; }
        }

        public string Condition
        {
            get { return DoResolve(txtResultFee.Text.Trim()); }
            set { txtResultFee.Text = UnDoResolve(value); }
        }

        public IRuleResolve Resolve
        {
            get { return resolve; }
            set { resolve = value; }
        }

        public void InitUI()
        {
            LMS.FMS.Model.FMS_FlowPointInfo curNode = DBService.GetModel<LMS.FMS.Model.FMS_FlowPointInfo, int>(NodeId);

            IList<LMS.FMS.Model.FMS_FlowPointInfo> nodes = DBService.GetObjects<LMS.FMS.Model.FMS_FlowPointInfo, int>("FlowId = " + curNode.FlowId);

            foreach (var item in nodes)
            {
                if (item.ID != curNode.ID)
                {
                    cmbSelectNode.Items.Add(new ListItem(item.PointName, item.ID.ToString()));
                }
            }
        }

        #endregion

        private IRuleResolve resolve;

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (cmbOperatorFee.SelectedIndex == -1)
            {
                Alert("请选择操作符!");

                return;
            }

            if (txtFee.Text.Trim().Length == 0)
            {
                Alert("请输入金额!");

                return;
            }

            if (cmbSelectNode.SelectedIndex == -1)
            {
                Alert("请选择转到的审批节点!");

                return;
            }

            StringBuilder builder = new StringBuilder();

            builder.Append(txtResultFee.Text);

            builder.Append("如果金额 [");
            builder.Append(cmbOperatorFee.SelectedItem.Text);
            builder.Append("] ");
            builder.Append("<");
            builder.Append(txtFee.Text);
            builder.Append("> ");
            builder.Append(" 就转到 ");
            builder.Append("{");
            builder.Append(cmbSelectNode.SelectedItem.Text);
            builder.Append("}");
            builder.Append(" 进行审批, ");
            builder.Append(Environment.NewLine);

            txtResultFee.Text = builder.ToString();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (txtResultFee.Text.Trim().Length == 0)
            {
                Alert("请选择规则条件!");

                return;
            }

            string condition = txtResultFee.Text.Trim().Remove(txtResultFee.Text.Trim().LastIndexOf(','));

            condition = DoResolve(condition);

            if (DoSave != null)
            {
                RuleParameter rulePara = new RuleParameter();

                rulePara.NodeId = NodeId;
                rulePara.RuleType = RuleType;
                rulePara.Condition = condition;

                DoSave(rulePara);
            }
        }

        private string DoResolve(string condition)
        {
            int startIndexOperator;
            int stopIndexOperator;
            int startIndexFee;
            int stopIndexFee;
            int startIndexNode;
            int stopIndexNode;

            StringBuilder builer = new StringBuilder();

            string[] subCondition = condition.Split(',');
            string subString = "";
            string strOperator = "";
            string strFee = "";
            string strNode = "";

            for (int i = 0; i < subCondition.Length; i++)
            {
                subString = subCondition[i];

                startIndexOperator = subString.IndexOf('[') + 1;
                stopIndexOperator = subString.IndexOf(']');
                startIndexFee = subString.IndexOf('<') + 1;
                stopIndexFee = subString.IndexOf('>');
                startIndexNode = subString.IndexOf('{') + 1;
                stopIndexNode = subString.IndexOf('}');

                strOperator = subString.Substring(startIndexOperator, stopIndexOperator - startIndexOperator);
                strFee = subString.Substring(startIndexFee, stopIndexFee - startIndexFee);
                strNode = subString.Substring(startIndexNode, stopIndexNode - startIndexNode);

                builer.Append(strFee);
                builer.Append(";");
                builer.Append(GetOperatorByName(strOperator));
                builer.Append(";");
                builer.Append(GetNodeByName(strNode));
                builer.Append(",");
            }

            builer.Remove(builer.Length - 1,1);

            txtResultFee.Text = "";

            return builer.ToString();
        }

        private string UnDoResolve(string sqlCondition)
        {
            string[] subConditions = sqlCondition.Split(',');
            string subCondition = "";
            string[] arrayItems;

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < subConditions.Length; i++)
            {
                subCondition = subConditions[i];

                arrayItems = subCondition.Split(';');

                builder.Append("如果金额 [");
                builder.Append(GetOperatorName(arrayItems[1]));
                builder.Append("] ");
                builder.Append("<");
                builder.Append(arrayItems[0]);
                builder.Append("> ");
                builder.Append(" 就转到 ");
                builder.Append("{");
                builder.Append(GetNodeName(arrayItems[2]));
                builder.Append("}");
                builder.Append(" 进行审批, ");
                builder.Append(Environment.NewLine);
            }

            return builder.ToString();
        }

        private string GetOperatorName(string strOperator)
        {
            ListItem item = null;

            for (int i = 0; i < cmbOperatorFee.Items.Count; i++)
            {
                item = cmbOperatorFee.Items[i];

                if (item.Value == strOperator) return item.Text;
            }

            throw new Exception("未知的操作符：" + strOperator);
        }

        private string GetNodeName(string strNode)
        {
            ListItem item = null;

            for (int i = 0; i < cmbSelectNode.Items.Count; i++)
            {
                item = cmbSelectNode.Items[i];

                if (item.Value == strNode) return item.Text;
            }

            throw new Exception("未知的节点：" + strNode);
        }

        private string GetOperatorByName(string strOperator)
        {
            ListItem item = null;

            for (int i = 0; i < cmbOperatorFee.Items.Count; i++)
            {
                item = cmbOperatorFee.Items[i];

                if (item.Text == strOperator) return item.Value;
            }

            throw new Exception("未知的操作符：" + strOperator);
        }

        private string GetNodeByName(string strNode)
        {
            ListItem item = null;

            for (int i = 0; i < cmbSelectNode.Items.Count; i++)
            {
                item = cmbSelectNode.Items[i];

                if (item.Text == strNode) return item.Value;
            }

            throw new Exception("未知的节点：" + strNode);
        }
    }
}