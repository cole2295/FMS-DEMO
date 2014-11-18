using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.WEB.Main;
using System.Text;
using LMS.Util;

namespace RFD.FMS.WEB.test.workflow
{
    public partial class UCRulePeople : BaseUserControl,IRuleUI
    {
        #region IRuleUI 成员

        public event Action<RuleParameter> DoSave;

        public int NodeId
        {
            get { return nodeId; }
            set { nodeId = value; }
        }

        public int RuleType
        {
            get { return DataConvert.ToInt(ViewState["RuleType"]); }
            set { ViewState["RuleType"] = value; }
        }

        public string Condition
        {
            get { return DoResolve(txtResultPeople.Text.Trim()); }
            set { txtResultPeople.Text = UnDoResolve(value); }
        }

        public void InitUI()
        {
            IList<LMS.FMS.Model.FMS_AudiRole> roles = DBService.GetObjects<LMS.FMS.Model.FMS_AudiRole, int>("FlowPointId = " + nodeId);

            IList<int> roleIds = new List<int>();

            foreach (var item in roles)
            {
                roleIds.Add(item.RoleId.Value);
            }

            IDictionary<int, string> dicUser = UserService.GetRoleUsers(roleIds);

            foreach (var item in dicUser)
            {
                cmbSelectUser.Items.Add(new ListItem(item.Value, item.Key.ToString()));
            }
        }

        #endregion

        private int nodeId;
        private string condition;
        private IRuleResolve resolve;

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (cmbSelectUser.SelectedIndex == -1)
            {
                Alert("请选择审批人!");

                return;
            }

            if (cmbOperatorPeople.SelectedIndex == -1)
            {
                Alert("请选择操作!");

                return;
            }

            StringBuilder builder = new StringBuilder();

            builder.Append(txtResultPeople.Text);

            builder.Append("审核人 ");
            builder.Append("<");
            builder.Append(cmbSelectUser.SelectedItem.Text);
            builder.Append("> ");
            builder.Append("{");
            builder.Append(cmbOperatorPeople.SelectedItem.Text);
            builder.Append("}");
            builder.Append(",");
            builder.Append(Environment.NewLine);

            txtResultPeople.Text = builder.ToString();
        }

        private string DoResolve(string condition)
        {
            int startIndexPeople;
            int stopIndexPeople;
            int startIndexOperator;
            int stopIndexOperator;

            StringBuilder builer = new StringBuilder();

            string[] subCondition = condition.Split(',');
            string subString = "";
            string strOperator = "";
            string strPeople = "";

            for (int i = 0; i < subCondition.Length; i++)
            {
                subString = subCondition[i];

                startIndexPeople = subString.IndexOf('<') + 1;
                stopIndexPeople = subString.IndexOf('>');
                startIndexOperator = subString.IndexOf('{') + 1;
                stopIndexOperator = subString.IndexOf('}');

                strPeople = subString.Substring(startIndexPeople, stopIndexPeople - startIndexPeople);
                strOperator = subString.Substring(startIndexOperator, stopIndexOperator - startIndexOperator);

                builer.Append(GetPeopleByName(strPeople));
                builer.Append(";");
                builer.Append(GetOperatorByName(strOperator));
                builer.Append(",");
            }

            builer.Remove(builer.Length - 1, 1);

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

                builder.Append("审核人 ");
                builder.Append("<");
                builder.Append(GetPeopleName(arrayItems[0]));
                builder.Append("> ");
                builder.Append("{");
                builder.Append(GetOperatorName(arrayItems[1]));
                builder.Append("}");
                builder.Append(",");
                builder.Append(Environment.NewLine);
            }

            return builder.ToString();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (DoSave != null)
            {
                RuleParameter rulePara = new RuleParameter();

                rulePara.NodeId = NodeId;
                rulePara.RuleType = RuleType;
                rulePara.Condition = Condition;

                DoSave(rulePara);
            }
        }

        private string GetOperatorByName(string strOperator)
        {
            ListItem item = null;

            for (int i = 0; i < cmbOperatorPeople.Items.Count; i++)
            {
                item = cmbOperatorPeople.Items[i];

                if (item.Text == strOperator) return item.Value;
            }

            throw new Exception("未知的操作符：" + strOperator);
        }

        private string GetPeopleByName(string strPeople)
        {
            ListItem item = null;

            for (int i = 0; i < cmbSelectUser.Items.Count; i++)
            {
                item = cmbSelectUser.Items[i];

                if (item.Text == strPeople) return item.Value;
            }

            throw new Exception("未知的操作符：" + strPeople);
        }

        private string GetPeopleName(string strPeople)
        {
            ListItem item = null;

            for (int i = 0; i < cmbSelectUser.Items.Count; i++)
            {
                item = cmbSelectUser.Items[i];

                if (item.Value == strPeople) return item.Text;
            }

            throw new Exception("未知的节点：" + strPeople);
        }

        private string GetOperatorName(string strOperator)
        {
            ListItem item = null;

            for (int i = 0; i < cmbOperatorPeople.Items.Count; i++)
            {
                item = cmbOperatorPeople.Items[i];

                if (item.Value == strOperator) return item.Text;
            }

            throw new Exception("未知的节点：" + strOperator);
        } 
    }
}