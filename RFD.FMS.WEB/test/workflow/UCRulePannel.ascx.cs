using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEBLOGIC;
using LMS.FMS.Model;
using LMS.Util;
using RFD.FMS.WEB.Main;

namespace RFD.FMS.WEB.test.workflow
{
    public partial class UCRulePannel : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitRuleType();
            }

            LoadControl();
        }

        public FMS_RuleVsFlowNode RuleNode
        {
            get 
            {
                try
                {
                    return ViewState["RuleNode"] as FMS_RuleVsFlowNode; 
                }
                catch(Exception ex)
                {
                    return null;
                }
            }
            set { ViewState["RuleNode"] = value; }
        }

        private void InitRuleType()
        {
            IList<LMS.FMS.Model.FMS_Rule> rules = DBService.GetObjects<FMS_Rule, int>("1=1");

            cmbRuleType.Items.Add(new ListItem("请选择规则", "-1"));

            foreach (var item in rules)
            {
                cmbRuleType.Items.Add(new ListItem(item.Name,item.ID.ToString()));
            }
        }

        public void InitUI(FMS_RuleVsFlowNode ruleNode)
        {
            RuleNode = ruleNode;

            FMS_Rule rule = DBService.GetModel<FMS_Rule, int>(ruleNode.RuleId);

            System.Web.UI.UserControl ucCtrl = Page.LoadControl(@rule.UIPath) as System.Web.UI.UserControl;

            (ucCtrl as IRuleUI).NodeId = RuleNode.NodeId;
            (ucCtrl as IRuleUI).RuleType = RuleNode.RuleId;
            (ucCtrl as IRuleUI).Condition = RuleNode.Condition;

            (ucCtrl as IRuleUI).InitUI();
            (ucCtrl as IRuleUI).DoSave -= UCRulePannel_DoSave;
            (ucCtrl as IRuleUI).DoSave += UCRulePannel_DoSave;

            pnlCtrl.Controls.Add(ucCtrl);
        }

        private void LoadControl()
        {
            if (DataConvert.ToInt(cmbRuleType.SelectedItem.Value) < 0) return;

            int ruleId = DataConvert.ToInt(cmbRuleType.SelectedItem.Value);

            FMS_Rule rule = DBService.GetModel<FMS_Rule, int>(ruleId);

            System.Web.UI.UserControl ucCtrl = Page.LoadControl(@rule.UIPath) as System.Web.UI.UserControl;

            (ucCtrl as IRuleUI).NodeId = 4242;
            (ucCtrl as IRuleUI).RuleType = ruleId;
            
            (ucCtrl as IRuleUI).InitUI();
            (ucCtrl as IRuleUI).DoSave -= UCRulePannel_DoSave;
            (ucCtrl as IRuleUI).DoSave += UCRulePannel_DoSave;

            pnlCtrl.Controls.Add(ucCtrl);
        }

        void UCRulePannel_DoSave(RuleParameter ruleParameter)
        {
            if (RuleNode != null)
            {
                FMS_RuleVsFlowNode ruleNode = RuleNode;

                ruleNode.Condition = ruleParameter.Condition;

                if (DBService.Update<FMS_RuleVsFlowNode, int>(ruleNode) == true)
                {
                    Alert("节点规则更新成功!");
                }
                else
                {
                    Alert("节点规则更新失败!");
                }
            }
            else
            {
                FMS_RuleVsFlowNode ruleNode = new FMS_RuleVsFlowNode();

                ruleNode.NodeId = ruleParameter.NodeId;
                ruleNode.RuleId = ruleParameter.RuleType;
                ruleNode.Condition = ruleParameter.Condition;

                if (DBService.Add<FMS_RuleVsFlowNode, int>(ruleNode) == true)
                {
                    Alert("节点规则添加成功!");
                }
                else
                {
                    Alert("节点规则添加失败!");
                }
            }
        }
    }
}