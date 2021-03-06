﻿using InControls.PLC.Mitsubishi;
using log4net.Config;
using Ramiel.UI_Update;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using TransferControl.CommandConvert;
using TransferControl.Engine;
using TransferControl.Management;

namespace Ramiel
{
    public partial class FormMain : Form, IUserInterfaceReport
    {
        delegate void MessageShow(string msg);
        Dictionary<string, string> Position = new Dictionary<string, string>
        {
            {"SMIF_1", "22"},
            {"SMIF_2", "23"},
            {"SHELF_1", "1"},
            {"SHELF_2", "2"},
            {"SHELF_3", "3"},
            {"SHELF_4", "4"},
            {"SHELF_5", "5"},
            {"SHELF_6", "6"},
            {"SHELF_7", "7"},
            {"SHELF_8", "8"},
            {"SHELF_9", "9"},
            {"SHELF_10", "10"},
            {"SHELF_11", "11"},
            {"SHELF_12", "12"},
            {"SHELF_13", "13"},
            {"SHELF_14", "14"},
            {"SHELF_15", "15"},
            {"SHELF_16", "16"},
            {"SHELF_17", "17"},
            {"SHELF_18", "18"},
            {"SHELF_19", "19"},
            {"SHELF_20", "20"},
            {"SHELF_21", "21"},
            {"BUFFER_1", "24"},
            {"BUFFER_2", "25"},
        };
        Dictionary<string, string> ModeList = new Dictionary<string, string>
        {
            {"Normal", "0"},
            {"Dry", "1"}
        };

        string Mode = "0";
        bool IsRun = false;
        public FormMain()
        {
            InitializeComponent();
            XmlConfigurator.Configure();

            //cbCmd.DataSource= Enum.GetValues(typeof(TaskFlowManagement.Command));
        }
        private void FormMain_Load(object sender, EventArgs e)
        {
            TaskFlowManagement ctrl = new TaskFlowManagement(this);

            CstRobot_Source_cbx.DataSource = new BindingSource(Position, null);
            CstRobot_Source_cbx.DisplayMember = "Key";
            CstRobot_Source_cbx.ValueMember = "Value";

            CstRobot_Destination_cbx.DataSource = new BindingSource(Position, null);
            CstRobot_Destination_cbx.DisplayMember = "Key";
            CstRobot_Destination_cbx.ValueMember = "Value";

            Mode_cb.DataSource = new BindingSource(ModeList, null);
            Mode_cb.DisplayMember = "Key";
            Mode_cb.ValueMember = "Value";
            Mode_cb.SelectedIndex = 0;
            Mode = "0";

            //ThreadPool.QueueUserWorkItem(new WaitCallback(CCLINKAutoRefresh), NodeManagement.Get("CSTROBOT"));
        }
        public void On_Alarm_Happen(AlarmManagement.Alarm Alarm)
        {
            FormMainUpdate.SetFormEnable("FormMain", true);
            FormMainUpdate.SetButtonEnable("btnAutoRun", true);
            FormMainUpdate.SetButtonEnable("CycleStop_btn", false);
        }

        public void On_Command_Error(Node Node, Transaction Txn, CommandReturnMessage Msg)
        {

        }

        public void On_Command_Excuted(Node Node, Transaction Txn, CommandReturnMessage Msg)
        {
            switch (Node.Type)
            {
                case "LOADPORT":
                    switch (Txn.Method)
                    {
                        case Transaction.Command.LoadPortType.Unload:
                        case Transaction.Command.LoadPortType.InitialPos:
                        case Transaction.Command.LoadPortType.ForceInitialPos:

                            if (Node.Name.ToUpper().Equals("SMIF1"))
                            {
                                NodeManagement.Get("CSTROBOT").SetIO("INTERLOCK", 10,  0);
                                NodeManagement.Get("CSTROBOT").SetIO("INTERLOCK", 11,  0);
                                NodeManagement.Get("CSTROBOT").SetIO("INTERLOCK", 12,  0);
                            }
                            if (Node.Name.ToUpper().Equals("SMIF2"))
                            {
                                NodeManagement.Get("CSTROBOT").SetIO("INTERLOCK", 13,  0);
                                NodeManagement.Get("CSTROBOT").SetIO("INTERLOCK", 14,  0);
                                NodeManagement.Get("CSTROBOT").SetIO("INTERLOCK", 15,  0);
                            }
                            break;
                        case Transaction.Command.LoadPortType.UnLift:
                            if (Node.Name.ToUpper().Equals("SMIF1"))
                            {
                                NodeManagement.Get("CSTROBOT").SetIO("INTERLOCK", 10, 0);

                            }
                            if (Node.Name.ToUpper().Equals("SMIF2"))
                            {
                                NodeManagement.Get("CSTROBOT").SetIO("INTERLOCK", 13, 0);

                            }
                            break;
                    }
                    break;
            }
        }

        public void On_Command_Finished(Node Node, Transaction Txn, CommandReturnMessage Msg)
        {

            switch (Node.Type)
            {
                case "LOADPORT":
                    switch (Txn.Method)
                    {
                        case Transaction.Command.LoadPortType.ReadStatus:
                            if (Node.Status["POS"].Equals("3") && Node.Status["LPS"].Equals("TRUE") && Node.Status["LLS"].Equals("TRUE"))
                            {
                                FormMainUpdate.Update_IO(Node.Name, "Placement");
                            }
                            else
                            {
                                FormMainUpdate.Update_IO(Node.Name, "N/A");
                            }
                            if (Node.Name.ToUpper().Equals("SMIF1")) {
                                NodeManagement.Get("CSTROBOT").SetIO("INTERLOCK", 10, (byte)(Node.Status["LLS"].Equals("TRUE")?1:0));
                                NodeManagement.Get("CSTROBOT").SetIO("INTERLOCK", 11, (byte)(Node.Status["LPS"].Equals("TRUE") ? 1 : 0));
                                NodeManagement.Get("CSTROBOT").SetIO("INTERLOCK", 12, (byte)(Node.Status["POS"].Equals("3") ? 1 : 0));
                            }
                            if (Node.Name.ToUpper().Equals("SMIF2"))
                            {
                                NodeManagement.Get("CSTROBOT").SetIO("INTERLOCK", 13, (byte)(Node.Status["LLS"].Equals("TRUE") ? 1 : 0));
                                NodeManagement.Get("CSTROBOT").SetIO("INTERLOCK", 14, (byte)(Node.Status["LPS"].Equals("TRUE") ? 1 : 0));
                                NodeManagement.Get("CSTROBOT").SetIO("INTERLOCK", 15, (byte)(Node.Status["POS"].Equals("3") ? 1 : 0));
                            }
                            break;
                        case Transaction.Command.LoadPortType.Lift:
                            if (Node.Name.ToUpper().Equals("SMIF1"))
                            {
                                NodeManagement.Get("CSTROBOT").SetIO("INTERLOCK", 10,  1);
                                
                            }
                            if (Node.Name.ToUpper().Equals("SMIF2"))
                            {
                                NodeManagement.Get("CSTROBOT").SetIO("INTERLOCK", 13, 1 );
                               
                            }
                            break;
                    }
                    break;
            }
        }

        public void On_Command_TimeOut(Node Node, Transaction Txn)
        {

        }

        public void On_Connection_Error(string DIOName, string ErrorMsg)
        {

        }

        public void On_Connection_Status_Report(string DIOName, string Status)
        {

        }

        public void On_DIO_Data_Chnaged(string Parameter, string Value, string Type)
        {
            UpdateUI(Type, Convert.ToInt32(Parameter), Convert.ToByte(Value));
        }

        public void On_Event_Trigger(Node Node, CommandReturnMessage Msg)
        {
            switch (Node.Type.ToUpper())
            {
                case "LOADPORT":
                    switch (Msg.Command)
                    {
                        case "STS__":
                            if (Msg.Value.Equals("R-Present,1"))
                            {
                                Node.Foup_Presence = true;
                                Node.Foup_Placement = true;
                            }
                            else if (Msg.Value.Equals("R-Present,0"))
                            {
                                Node.Foup_Presence = false;
                                Node.Foup_Placement = false;
                            }
                            break;
                    }
                    break;
            }
        }

        public void On_Job_Location_Changed(Job Job)
        {

        }
        delegate void UpdateMessage_Log(string Type, string Message);
        public void On_Message_Log(string Type, string Message)
        {
            try
            {

                //if (Type.Equals("CMD"))
                //{


                UI_Update.FormMainUpdate.LogUpdate(Message);

                //}
            }
            catch
            {

            }
        }


        public void On_Node_Connection_Changed(string NodeName, string Status)
        {

        }

        public void On_Node_State_Changed(Node Node, string Status)
        {

        }

        public void On_Status_Changed(string Type, string Message)
        {

        }

        public void On_TaskJob_Aborted(TaskFlowManagement.CurrentProcessTask Task)
        {
            FormMainUpdate.SetFormEnable("FormMain", true);
        }

        public void On_TaskJob_Ack(TaskFlowManagement.CurrentProcessTask Task)
        {

        }

        public void On_TaskJob_Finished(TaskFlowManagement.CurrentProcessTask Task)
        {
            if (!Task.IsSubTask)
            {
                FormMainUpdate.SetFormEnable("FormMain", true);
            }
        }




        private void SMIF1_Stage_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.OPEN_FOUP, new Dictionary<string, string>() { { "@Target", "SMIF1" } }, Guid.NewGuid().ToString());
        }

        private void SMIF2_Stage_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.OPEN_FOUP, new Dictionary<string, string>() { { "@Target", "SMIF2" } }, Guid.NewGuid().ToString());
        }

        private void SMIF1_Home_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.CLOSE_FOUP, new Dictionary<string, string>() { { "@Target", "SMIF1" } }, Guid.NewGuid().ToString());
        }

        private void SMIF2_Home_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.CLOSE_FOUP, new Dictionary<string, string>() { { "@Target", "SMIF2" } }, Guid.NewGuid().ToString());
        }

        private void SMIF1_SetSpeed_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            int speed = 0;
            if (int.TryParse(SMIF1_Speed_cbx.Text, out speed))
            {
                FormMainUpdate.SetFormEnable("FormMain", false);
                TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_SET_SPEED, new Dictionary<string, string>() { { "@Target", "SMIF1" }, { "@Value", speed == 100 ? "00" : speed == 0 ? "10" : speed.ToString() } }, Guid.NewGuid().ToString());
            }
            else
            {
                MessageBox.Show("Speed invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SMIF2_SetSpeed_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            int speed = 0;
            if (int.TryParse(SMIF2_Speed_cbx.Text, out speed))
            {
                FormMainUpdate.SetFormEnable("FormMain", false);
                TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_SET_SPEED, new Dictionary<string, string>() { { "@Target", "SMIF2" }, { "@Value", speed == 100 ? "00" : speed == 0 ? "10" : speed.ToString() } }, Guid.NewGuid().ToString());
            }
            else
            {
                MessageBox.Show("Speed invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SMIF1_MoveTo_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            int slot = 0;
            if (int.TryParse(SMIF1_Slot_cbx.Text, out slot))
            {
                FormMainUpdate.SetFormEnable("FormMain", false);
                TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_SLOT, new Dictionary<string, string>() { { "@Target", "SMIF1" }, { "@Value", slot.ToString() } }, Guid.NewGuid().ToString());
            }
            else
            {
                MessageBox.Show("Slot invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SMIF2_MoveTo_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            int slot = 0;
            if (int.TryParse(SMIF2_Slot_cbx.Text, out slot))
            {
                FormMainUpdate.SetFormEnable("FormMain", false);
                TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_SLOT, new Dictionary<string, string>() { { "@Target", "SMIF2" }, { "@Value", slot.ToString() } }, Guid.NewGuid().ToString());
            }
            else
            {
                MessageBox.Show("Slot invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SMIF1_TwkUp_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_TWKUP, new Dictionary<string, string>() { { "@Target", "SMIF1" } }, Guid.NewGuid().ToString());
        }
        private void SMIF2_TwkUp_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_TWKUP, new Dictionary<string, string>() { { "@Target", "SMIF2" } }, Guid.NewGuid().ToString());
        }

        private void SMIF1_TwkDn_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_TWKDN, new Dictionary<string, string>() { { "@Target", "SMIF1" } }, Guid.NewGuid().ToString());
        }

        private void SMIF2_TwkDn_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_TWKDN, new Dictionary<string, string>() { { "@Target", "SMIF2" } }, Guid.NewGuid().ToString());
        }

        private void SMIF1_ReMap_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_RE_MAPPING, new Dictionary<string, string>() { { "@Target", "SMIF1" } }, Guid.NewGuid().ToString());
        }

        private void SMIF2_ReMap_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_RE_MAPPING, new Dictionary<string, string>() { { "@Target", "SMIF2" } }, Guid.NewGuid().ToString());
        }

        private void SMIF1_Clamp_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_CLAMP, new Dictionary<string, string>() { { "@Target", "SMIF1" } }, Guid.NewGuid().ToString());
        }

        private void SMIF2_Clamp_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_CLAMP, new Dictionary<string, string>() { { "@Target", "SMIF2" } }, Guid.NewGuid().ToString());
        }

        private void SMIF1_UnClamp_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_UNCLAMP, new Dictionary<string, string>() { { "@Target", "SMIF1" } }, Guid.NewGuid().ToString());
        }

        private void SMIF2_UnClamp_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_UNCLAMP, new Dictionary<string, string>() { { "@Target", "SMIF2" } }, Guid.NewGuid().ToString());
        }

        private void SMIF1_Org_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_ORGSH, new Dictionary<string, string>() { { "@Target", "SMIF1" } }, Guid.NewGuid().ToString());
        }

        private void SMIF2_Org_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_ORGSH, new Dictionary<string, string>() { { "@Target", "SMIF2" } }, Guid.NewGuid().ToString());
        }

        private void SMIF1_Reset_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_RESET, new Dictionary<string, string>() { { "@Target", "SMIF1" } }, Guid.NewGuid().ToString());
        }

        private void SMIF2_Reset_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_RESET, new Dictionary<string, string>() { { "@Target", "SMIF2" } }, Guid.NewGuid().ToString());
        }

        private void CstRobot_Switch_btn_Click(object sender, EventArgs e)
        {
            string Swap = CstRobot_Source_cbx.Text;
            CstRobot_Source_cbx.Text = CstRobot_Destination_cbx.Text;
            CstRobot_Destination_cbx.Text = Swap;
        }

        private void CstRobot_Source_GetWait_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (!checkDryMode())
            {
                return;
            }
            if (!CstRobot_Source_cbx.SelectedValue.ToString().Equals(""))
            {
                FormMainUpdate.SetFormEnable("FormMain", false);
                TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_GETWAIT, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Position", CstRobot_Source_cbx.SelectedValue.ToString() }, { "@Speed", CstRobot_Speed_cbx.Text }, { "@Mode", Mode } }, Guid.NewGuid().ToString());
            }
            else
            {
                MessageBox.Show("Source invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CstRobot_Source_Get_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (!checkDryMode())
            {
                return;
            }
            if (!CstRobot_Source_cbx.SelectedValue.ToString().Equals(""))
            {
                FormMainUpdate.SetFormEnable("FormMain", false);
                TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_GET, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Position", CstRobot_Source_cbx.SelectedValue.ToString() }, { "@Speed", CstRobot_Speed_cbx.Text }, { "@Mode", Mode } ,{ "@ByPassCheck",BypassSafetyCheck_ck.Checked?"TRUE":"FALSE" } }, Guid.NewGuid().ToString());
            }
            else
            {
                MessageBox.Show("Source invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void CstRobot_Reset_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_RESET, new Dictionary<string, string>() { { "@Target", "CSTROBOT" } }, Guid.NewGuid().ToString());
        }
        private void CstRobot_Org_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_ORGSH, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Speed", CstRobot_Speed_cbx.Text }, { "@Mode", Mode } }, Guid.NewGuid().ToString());
        }

        private void CstRobot_Home_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_SHOME, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Speed", CstRobot_Speed_cbx.Text }, { "@Mode", Mode } }, Guid.NewGuid().ToString());
        }



        private void CstRobot_Source_PutWait_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (!checkDryMode())
            {
                return;
            }
            if (!CstRobot_Destination_cbx.SelectedValue.ToString().Equals(""))
            {
                FormMainUpdate.SetFormEnable("FormMain", false);
                TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_PUTWAIT, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Position", CstRobot_Destination_cbx.SelectedValue.ToString() }, { "@Speed", CstRobot_Speed_cbx.Text }, { "@Mode", Mode } }, Guid.NewGuid().ToString());
            }
            else
            {
                MessageBox.Show("Destination invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CstRobot_Source_Put_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (!checkDryMode())
            {
                return;
            }
            if (!CstRobot_Destination_cbx.SelectedValue.ToString().Equals(""))
            {
                FormMainUpdate.SetFormEnable("FormMain", false);
                TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_PUT, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Position", CstRobot_Destination_cbx.SelectedValue.ToString() }, { "@Speed", CstRobot_Speed_cbx.Text }, { "@Mode", Mode }, { "@ByPassCheck", BypassSafetyCheck_ck.Checked ? "TRUE" : "FALSE" } }, Guid.NewGuid().ToString());
            }
            else
            {
                MessageBox.Show("Destination invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void UpdateUI(string Type, int Pos, byte Val)
        {
            Node Target = NodeManagement.Get("CSTROBOT");
            switch (Type)
            {
                case "CSTINTERLOCK":
                    FormMainUpdate.Update_IO("Inter_Sig_"+(Pos).ToString(), Val == 1 ? "InterOn" : "InterOff");
                    break;
                case "SHELFINTERLOCK":
                    FormMainUpdate.Update_IO("ShelfInter_Sig_" + (Pos).ToString(), Val == 1 ? "InterOn" : "InterOff");
                    break;
                case "VIPINTERLOCK":
                    FormMainUpdate.Update_IO("VIPInter_Sig_" + (Pos).ToString(), Val == 1 ? "InterOn" : "InterOff");
                    break;
                case "INPUT":
                    switch (Pos)
                    {
                        case Input.CST_In_Press:
                            FormMainUpdate.Update_IO("CST_In_Press", Val == 1 ? "Placement" : "N/A");
                            break;
                        case Input.CST_Out_Press:
                            FormMainUpdate.Update_IO("CST_Out_Press", Val == 1 ? "Placement" : "N/A");
                            break;
                        case Input.Tx_Pause_Front_Press:
                            FormMainUpdate.Update_IO("Tx_Pause_Front_Press", Val == 1 ? "Placement" : "N/A");
                            break;
                        case Input.Tx_Pause_Rear_Press:
                            FormMainUpdate.Update_IO("Tx_Pause_Rear_Press", Val == 1 ? "Placement" : "N/A");
                            break;
                    }
                    break;
                case "OUTPUT":
                    switch (Pos)
                    {
                        case Output.SignalTower_Red:
                            FormMainUpdate.Update_IO("SignalTower_Red", Val == 1 ? "Red" : "N/A");
                            break;
                        case Output.SignalTower_Yellow:
                            FormMainUpdate.Update_IO("SignalTower_Yellow", Val == 1 ? "Yellow" : "N/A");
                            break;
                        case Output.SignalTower_Green:
                            FormMainUpdate.Update_IO("SignalTower_Green", Val == 1 ? "Green" : "N/A");
                            break;
                        case Output.SignalTower_Blue:
                            FormMainUpdate.Update_IO("SignalTower_Blue", Val == 1 ? "Blue" : "N/A");
                            break;
                        case Output.SignalTower_Buzz1:
                            FormMainUpdate.Update_IO("SignalTower_Buzz1", Val == 1 ? "Placement" : "N/A");
                            break;
                        case Output.SignalTower_Buzz2:
                            FormMainUpdate.Update_IO("SignalTower_Buzz2", Val == 1 ? "Placement" : "N/A");
                            break;
                        case Output.Pod1_Lock:
                            FormMainUpdate.Update_IO("Pod1_Lock", Val == 1 ? "Placement" : "N/A");
                            break;
                        case Output.Pod2_Lock:
                            FormMainUpdate.Update_IO("Pod2_Lock", Val == 1 ? "Placement" : "N/A");
                            break;
                        case Output.CST_In:
                            FormMainUpdate.Update_IO("CST_In", Val == 1 ? "Placement" : "N/A");
                            break;
                        case Output.CST_Out:
                            FormMainUpdate.Update_IO("CST_Out", Val == 1 ? "Placement" : "N/A");
                            break;
                        case Output.Tx_Pause_Front:
                            FormMainUpdate.Update_IO("Tx_Pause_Front", Val == 1 ? "Placement" : "N/A");
                            break;
                        case Output.Tx_Pause_Rear:
                            FormMainUpdate.Update_IO("Tx_Pause_Rear", Val == 1 ? "Placement" : "N/A");
                            break;
                    }

                    break;
                case "PRESENCE":
                    switch (Pos)
                    {
                        case 96:
                            FormMainUpdate.Update_IO("Inter_In_00", Target.GetIO("PRESENCE")[Pos] == 1? "InterOn" : "InterOff");
                            break;
                        case 97:
                            FormMainUpdate.Update_IO("Inter_In_01", Target.GetIO("PRESENCE")[Pos] == 1 ? "InterOn" : "InterOff");
                            break;
                        case 98:
                            FormMainUpdate.Update_IO("Inter_In_02", Target.GetIO("PRESENCE")[Pos] == 1 ? "InterOn" : "InterOff");
                            break;
                        case 99:
                            FormMainUpdate.Update_IO("Inter_In_03", Target.GetIO("PRESENCE")[Pos] == 1 ? "InterOn" : "InterOff");
                            break;
                        case 100:
                            FormMainUpdate.Update_IO("Inter_In_04", Target.GetIO("PRESENCE")[Pos] == 1 ? "InterOn" : "InterOff");
                            break;
                        case 102:
                            FormMainUpdate.Update_IO("Inter_In_06", Target.GetIO("PRESENCE")[Pos] == 1 ? "InterOn" : "InterOff");
                            break;
                        case 103:
                            FormMainUpdate.Update_IO("Inter_In_07", Target.GetIO("PRESENCE")[Pos] == 1 ? "InterOn" : "InterOff");
                            break;

                        case Presence.Shelf_1_1:
                        case Presence.Shelf_1_2:
                            if (Target.GetIO("PRESENCE")[Presence.Shelf_1_1] == 1 && Target.GetIO("PRESENCE")[Presence.Shelf_1_2] == 1)
                            {
                                FormMainUpdate.Update_IO("Shelf_1", "N/A");
                            }
                            else if (Target.GetIO("PRESENCE")[Presence.Shelf_1_1] == 0 && Target.GetIO("PRESENCE")[Presence.Shelf_1_2] == 0)
                            {
                                FormMainUpdate.Update_IO("Shelf_1", "Placement");
                            }
                            else
                            {
                                FormMainUpdate.Update_IO("Shelf_1", "Presence");
                            }
                            if (Pos == Presence.Shelf_1_1)
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_1_1", Target.GetIO("PRESENCE")[Presence.Shelf_1_1].ToString());
                            }
                            else
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_1_2", Target.GetIO("PRESENCE")[Presence.Shelf_1_2].ToString());
                            }
                            break;
                        case Presence.Shelf_2_1:
                        case Presence.Shelf_2_2:
                            if (Target.GetIO("PRESENCE")[Presence.Shelf_2_1] == 1 && Target.GetIO("PRESENCE")[Presence.Shelf_2_2] == 1)
                            {
                                FormMainUpdate.Update_IO("Shelf_2", "N/A");
                            }
                            else if (Target.GetIO("PRESENCE")[Presence.Shelf_2_1] == 0 && Target.GetIO("PRESENCE")[Presence.Shelf_2_2] == 0)
                            {
                                FormMainUpdate.Update_IO("Shelf_2", "Placement");
                            }
                            else
                            {
                                FormMainUpdate.Update_IO("Shelf_2", "Presence");
                            }
                            if (Pos == Presence.Shelf_2_1)
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_2_1", Target.GetIO("PRESENCE")[Presence.Shelf_2_1].ToString());
                            }
                            else
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_2_2", Target.GetIO("PRESENCE")[Presence.Shelf_2_2].ToString());
                            }
                            break;
                        case Presence.Shelf_3_1:
                        case Presence.Shelf_3_2:
                            if (Target.GetIO("PRESENCE")[Presence.Shelf_3_1] == 1 && Target.GetIO("PRESENCE")[Presence.Shelf_3_2] == 1)
                            {
                                FormMainUpdate.Update_IO("Shelf_3", "N/A");
                            }
                            else if (Target.GetIO("PRESENCE")[Presence.Shelf_3_1] == 0 && Target.GetIO("PRESENCE")[Presence.Shelf_3_2] == 0)
                            {
                                FormMainUpdate.Update_IO("Shelf_3", "Placement");
                            }
                            else
                            {
                                FormMainUpdate.Update_IO("Shelf_3", "Presence");
                            }
                            if (Pos == Presence.Shelf_3_1)
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_3_1", Target.GetIO("PRESENCE")[Presence.Shelf_3_1].ToString());
                            }
                            else
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_3_2", Target.GetIO("PRESENCE")[Presence.Shelf_3_2].ToString());
                            }
                            break;
                        case Presence.Shelf_4_1:
                        case Presence.Shelf_4_2:
                            if (Target.GetIO("PRESENCE")[Presence.Shelf_4_1] == 1 && Target.GetIO("PRESENCE")[Presence.Shelf_4_2] == 1)
                            {
                                FormMainUpdate.Update_IO("Shelf_4", "N/A");
                            }
                            else if (Target.GetIO("PRESENCE")[Presence.Shelf_4_1] == 0 && Target.GetIO("PRESENCE")[Presence.Shelf_4_2] == 0)
                            {
                                FormMainUpdate.Update_IO("Shelf_4", "Placement");
                            }
                            else
                            {
                                FormMainUpdate.Update_IO("Shelf_4", "Presence");
                            }
                            if (Pos == Presence.Shelf_4_1)
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_4_1", Target.GetIO("PRESENCE")[Presence.Shelf_4_1].ToString());
                            }
                            else
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_4_2", Target.GetIO("PRESENCE")[Presence.Shelf_4_2].ToString());
                            }
                            break;
                        case Presence.Shelf_5_1:
                        case Presence.Shelf_5_2:
                            if (Target.GetIO("PRESENCE")[Presence.Shelf_5_1] == 1 && Target.GetIO("PRESENCE")[Presence.Shelf_5_2] == 1)
                            {
                                FormMainUpdate.Update_IO("Shelf_5", "N/A");
                            }
                            else if (Target.GetIO("PRESENCE")[Presence.Shelf_5_1] == 0 && Target.GetIO("PRESENCE")[Presence.Shelf_5_2] == 0)
                            {
                                FormMainUpdate.Update_IO("Shelf_5", "Placement");
                            }
                            else
                            {
                                FormMainUpdate.Update_IO("Shelf_5", "Presence");
                            }
                            if (Pos == Presence.Shelf_5_1)
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_5_1", Target.GetIO("PRESENCE")[Presence.Shelf_5_1].ToString());
                            }
                            else
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_5_2", Target.GetIO("PRESENCE")[Presence.Shelf_5_2].ToString());
                            }
                            break;
                        case Presence.Shelf_6_1:
                        case Presence.Shelf_6_2:
                            if (Target.GetIO("PRESENCE")[Presence.Shelf_6_1] == 1 && Target.GetIO("PRESENCE")[Presence.Shelf_6_2] == 1)
                            {
                                FormMainUpdate.Update_IO("Shelf_6", "N/A");
                            }
                            else if (Target.GetIO("PRESENCE")[Presence.Shelf_6_1] == 0 && Target.GetIO("PRESENCE")[Presence.Shelf_6_2] == 0)
                            {
                                FormMainUpdate.Update_IO("Shelf_6", "Placement");
                            }
                            else
                            {
                                FormMainUpdate.Update_IO("Shelf_6", "Presence");
                            }
                            if (Pos == Presence.Shelf_6_1)
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_6_1", Target.GetIO("PRESENCE")[Presence.Shelf_6_1].ToString());
                            }
                            else
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_6_2", Target.GetIO("PRESENCE")[Presence.Shelf_6_2].ToString());
                            }
                            break;
                        case Presence.Shelf_7_1:
                        case Presence.Shelf_7_2:
                            if (Target.GetIO("PRESENCE")[Presence.Shelf_7_1] == 1 && Target.GetIO("PRESENCE")[Presence.Shelf_7_2] == 1)
                            {
                                FormMainUpdate.Update_IO("Shelf_7", "N/A");
                            }
                            else if (Target.GetIO("PRESENCE")[Presence.Shelf_7_1] == 0 && Target.GetIO("PRESENCE")[Presence.Shelf_7_2] == 0)
                            {
                                FormMainUpdate.Update_IO("Shelf_7", "Placement");
                            }
                            else
                            {
                                FormMainUpdate.Update_IO("Shelf_7", "Presence");
                            }
                            if (Pos == Presence.Shelf_7_1)
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_7_1", Target.GetIO("PRESENCE")[Presence.Shelf_7_1].ToString());
                            }
                            else
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_7_2", Target.GetIO("PRESENCE")[Presence.Shelf_7_2].ToString());
                            }
                            break;
                        case Presence.Shelf_8_1:
                        case Presence.Shelf_8_2:
                            if (Target.GetIO("PRESENCE")[Presence.Shelf_8_1] == 1 && Target.GetIO("PRESENCE")[Presence.Shelf_8_2] == 1)
                            {
                                FormMainUpdate.Update_IO("Shelf_8", "N/A");
                            }
                            else if (Target.GetIO("PRESENCE")[Presence.Shelf_8_1] == 0 && Target.GetIO("PRESENCE")[Presence.Shelf_8_2] == 0)
                            {
                                FormMainUpdate.Update_IO("Shelf_8", "Placement");
                            }
                            else
                            {
                                FormMainUpdate.Update_IO("Shelf_8", "Presence");
                            }
                            if (Pos == Presence.Shelf_8_1)
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_8_1", Target.GetIO("PRESENCE")[Presence.Shelf_8_1].ToString());
                            }
                            else
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_8_2", Target.GetIO("PRESENCE")[Presence.Shelf_8_2].ToString());
                            }
                            break;
                        case Presence.Shelf_9_1:
                        case Presence.Shelf_9_2:
                            if (Target.GetIO("PRESENCE")[Presence.Shelf_9_1] == 1 && Target.GetIO("PRESENCE")[Presence.Shelf_9_2] == 1)
                            {
                                FormMainUpdate.Update_IO("Shelf_9", "N/A");
                            }
                            else if (Target.GetIO("PRESENCE")[Presence.Shelf_9_1] == 0 && Target.GetIO("PRESENCE")[Presence.Shelf_9_2] == 0)
                            {
                                FormMainUpdate.Update_IO("Shelf_9", "Placement");
                            }
                            else
                            {
                                FormMainUpdate.Update_IO("Shelf_9", "Presence");
                            }
                            if (Pos == Presence.Shelf_9_1)
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_9_1", Target.GetIO("PRESENCE")[Presence.Shelf_9_1].ToString());
                            }
                            else
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_9_2", Target.GetIO("PRESENCE")[Presence.Shelf_9_2].ToString());
                            }
                            break;
                        case Presence.Shelf_10_1:
                        case Presence.Shelf_10_2:
                            if (Target.GetIO("PRESENCE")[Presence.Shelf_10_1] == 1 && Target.GetIO("PRESENCE")[Presence.Shelf_10_2] == 1)
                            {
                                FormMainUpdate.Update_IO("Shelf_10", "N/A");
                            }
                            else if (Target.GetIO("PRESENCE")[Presence.Shelf_10_1] == 0 && Target.GetIO("PRESENCE")[Presence.Shelf_10_2] == 0)
                            {
                                FormMainUpdate.Update_IO("Shelf_10", "Placement");
                            }
                            else
                            {
                                FormMainUpdate.Update_IO("Shelf_10", "Presence");
                            }
                            if (Pos == Presence.Shelf_10_1)
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_10_1", Target.GetIO("PRESENCE")[Presence.Shelf_10_1].ToString());
                            }
                            else
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_10_2", Target.GetIO("PRESENCE")[Presence.Shelf_10_2].ToString());
                            }
                            break;
                        case Presence.Shelf_11_1:
                        case Presence.Shelf_11_2:
                            if (Target.GetIO("PRESENCE")[Presence.Shelf_11_1] == 1 && Target.GetIO("PRESENCE")[Presence.Shelf_11_2] == 1)
                            {
                                FormMainUpdate.Update_IO("Shelf_11", "N/A");
                            }
                            else if (Target.GetIO("PRESENCE")[Presence.Shelf_11_1] == 0 && Target.GetIO("PRESENCE")[Presence.Shelf_11_2] == 0)
                            {
                                FormMainUpdate.Update_IO("Shelf_11", "Placement");
                            }
                            else
                            {
                                FormMainUpdate.Update_IO("Shelf_11", "Presence");
                            }
                            if (Pos == Presence.Shelf_11_1)
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_11_1", Target.GetIO("PRESENCE")[Presence.Shelf_11_1].ToString());
                            }
                            else
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_11_2", Target.GetIO("PRESENCE")[Presence.Shelf_11_2].ToString());
                            }
                            break;
                        case Presence.Shelf_12_1:
                        case Presence.Shelf_12_2:
                            if (Target.GetIO("PRESENCE")[Presence.Shelf_12_1] == 1 && Target.GetIO("PRESENCE")[Presence.Shelf_12_2] == 1)
                            {
                                FormMainUpdate.Update_IO("Shelf_12", "N/A");
                            }
                            else if (Target.GetIO("PRESENCE")[Presence.Shelf_12_1] == 0 && Target.GetIO("PRESENCE")[Presence.Shelf_12_2] == 0)
                            {
                                FormMainUpdate.Update_IO("Shelf_12", "Placement");
                            }
                            else
                            {
                                FormMainUpdate.Update_IO("Shelf_12", "Presence");
                            }
                            if (Pos == Presence.Shelf_12_1)
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_12_1", Target.GetIO("PRESENCE")[Presence.Shelf_12_1].ToString());
                            }
                            else
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_12_2", Target.GetIO("PRESENCE")[Presence.Shelf_12_2].ToString());
                            }
                            break;
                        case Presence.Shelf_13_1:
                        case Presence.Shelf_13_2:
                            if (Target.GetIO("PRESENCE")[Presence.Shelf_13_1] == 1 && Target.GetIO("PRESENCE")[Presence.Shelf_13_2] == 1)
                            {
                                FormMainUpdate.Update_IO("Shelf_13", "N/A");
                            }
                            else if (Target.GetIO("PRESENCE")[Presence.Shelf_13_1] == 0 && Target.GetIO("PRESENCE")[Presence.Shelf_13_2] == 0)
                            {
                                FormMainUpdate.Update_IO("Shelf_13", "Placement");
                            }
                            else
                            {
                                FormMainUpdate.Update_IO("Shelf_13", "Presence");
                            }
                            if (Pos == Presence.Shelf_13_1)
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_13_1", Target.GetIO("PRESENCE")[Presence.Shelf_13_1].ToString());
                            }
                            else
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_13_2", Target.GetIO("PRESENCE")[Presence.Shelf_13_2].ToString());
                            }
                            break;
                        case Presence.Shelf_14_1:
                        case Presence.Shelf_14_2:
                            if (Target.GetIO("PRESENCE")[Presence.Shelf_14_1] == 1 && Target.GetIO("PRESENCE")[Presence.Shelf_14_2] == 1)
                            {
                                FormMainUpdate.Update_IO("Shelf_14", "N/A");
                            }
                            else if (Target.GetIO("PRESENCE")[Presence.Shelf_14_1] == 0 && Target.GetIO("PRESENCE")[Presence.Shelf_14_2] == 0)
                            {
                                FormMainUpdate.Update_IO("Shelf_14", "Placement");
                            }
                            else
                            {
                                FormMainUpdate.Update_IO("Shelf_14", "Presence");
                            }
                            if (Pos == Presence.Shelf_14_1)
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_14_1", Target.GetIO("PRESENCE")[Presence.Shelf_14_1].ToString());
                            }
                            else
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_14_2", Target.GetIO("PRESENCE")[Presence.Shelf_14_2].ToString());
                            }
                            break;

                        case Presence.Shelf_15_1:
                        case Presence.Shelf_15_2:
                            if (Target.GetIO("PRESENCE")[Presence.Shelf_15_1] == 1 && Target.GetIO("PRESENCE")[Presence.Shelf_15_2] == 1)
                            {
                                FormMainUpdate.Update_IO("Shelf_15", "N/A");
                            }
                            else if (Target.GetIO("PRESENCE")[Presence.Shelf_15_1] == 0 && Target.GetIO("PRESENCE")[Presence.Shelf_15_2] == 0)
                            {
                                FormMainUpdate.Update_IO("Shelf_15", "Placement");
                            }
                            else
                            {
                                FormMainUpdate.Update_IO("Shelf_15", "Presence");
                            }
                            if (Pos == Presence.Shelf_15_1)
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_15_1", Target.GetIO("PRESENCE")[Presence.Shelf_15_1].ToString());
                            }
                            else
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_15_2", Target.GetIO("PRESENCE")[Presence.Shelf_15_2].ToString());
                            }
                            break;
                        case Presence.Shelf_16_1:
                        case Presence.Shelf_16_2:
                            if (Target.GetIO("PRESENCE")[Presence.Shelf_16_1] == 1 && Target.GetIO("PRESENCE")[Presence.Shelf_16_2] == 1)
                            {
                                FormMainUpdate.Update_IO("Shelf_16", "N/A");
                            }
                            else if (Target.GetIO("PRESENCE")[Presence.Shelf_16_1] == 0 && Target.GetIO("PRESENCE")[Presence.Shelf_16_2] == 0)
                            {
                                FormMainUpdate.Update_IO("Shelf_16", "Placement");
                            }
                            else
                            {
                                FormMainUpdate.Update_IO("Shelf_16", "Presence");
                            }
                            if (Pos == Presence.Shelf_16_1)
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_16_1", Target.GetIO("PRESENCE")[Presence.Shelf_16_1].ToString());
                            }
                            else
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_16_2", Target.GetIO("PRESENCE")[Presence.Shelf_16_2].ToString());
                            }
                            break;
                        case Presence.Shelf_17_1:
                        case Presence.Shelf_17_2:
                            if (Target.GetIO("PRESENCE")[Presence.Shelf_17_1] == 1 && Target.GetIO("PRESENCE")[Presence.Shelf_17_2] == 1)
                            {
                                FormMainUpdate.Update_IO("Shelf_17", "N/A");
                            }
                            else if (Target.GetIO("PRESENCE")[Presence.Shelf_17_1] == 0 && Target.GetIO("PRESENCE")[Presence.Shelf_17_2] == 0)
                            {
                                FormMainUpdate.Update_IO("Shelf_17", "Placement");
                            }
                            else
                            {
                                FormMainUpdate.Update_IO("Shelf_17", "Presence");
                            }
                            if (Pos == Presence.Shelf_17_1)
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_17_1", Target.GetIO("PRESENCE")[Presence.Shelf_17_1].ToString());
                            }
                            else
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_17_2", Target.GetIO("PRESENCE")[Presence.Shelf_17_2].ToString());
                            }
                            break;
                        case Presence.Shelf_18_1:
                        case Presence.Shelf_18_2:
                            if (Target.GetIO("PRESENCE")[Presence.Shelf_18_1] == 1 && Target.GetIO("PRESENCE")[Presence.Shelf_18_2] == 1)
                            {
                                FormMainUpdate.Update_IO("Shelf_18", "N/A");
                            }
                            else if (Target.GetIO("PRESENCE")[Presence.Shelf_18_1] == 0 && Target.GetIO("PRESENCE")[Presence.Shelf_18_2] == 0)
                            {
                                FormMainUpdate.Update_IO("Shelf_18", "Placement");
                            }
                            else
                            {
                                FormMainUpdate.Update_IO("Shelf_18", "Presence");
                            }
                            if (Pos == Presence.Shelf_18_1)
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_18_1", Target.GetIO("PRESENCE")[Presence.Shelf_18_1].ToString());
                            }
                            else
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_18_2", Target.GetIO("PRESENCE")[Presence.Shelf_18_2].ToString());
                            }
                            break;
                        case Presence.Shelf_19_1:
                        case Presence.Shelf_19_2:
                            if (Target.GetIO("PRESENCE")[Presence.Shelf_19_1] == 1 && Target.GetIO("PRESENCE")[Presence.Shelf_19_2] == 1)
                            {
                                FormMainUpdate.Update_IO("Shelf_19", "N/A");
                            }
                            else if (Target.GetIO("PRESENCE")[Presence.Shelf_19_1] == 0 && Target.GetIO("PRESENCE")[Presence.Shelf_19_2] == 0)
                            {
                                FormMainUpdate.Update_IO("Shelf_19", "Placement");
                            }
                            else
                            {
                                FormMainUpdate.Update_IO("Shelf_19", "Presence");
                            }
                            if (Pos == Presence.Shelf_19_1)
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_19_1", Target.GetIO("PRESENCE")[Presence.Shelf_19_1].ToString());
                            }
                            else
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_19_2", Target.GetIO("PRESENCE")[Presence.Shelf_19_2].ToString());
                            }
                            break;
                        case Presence.Shelf_20_1:
                        case Presence.Shelf_20_2:
                            if (Target.GetIO("PRESENCE")[Presence.Shelf_20_1] == 1 && Target.GetIO("PRESENCE")[Presence.Shelf_20_2] == 1)
                            {
                                FormMainUpdate.Update_IO("Shelf_20", "N/A");
                            }
                            else if (Target.GetIO("PRESENCE")[Presence.Shelf_20_1] == 0 && Target.GetIO("PRESENCE")[Presence.Shelf_20_2] == 0)
                            {
                                FormMainUpdate.Update_IO("Shelf_20", "Placement");
                            }
                            else
                            {
                                FormMainUpdate.Update_IO("Shelf_20", "Presence");
                            }
                            if (Pos == Presence.Shelf_20_1)
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_20_1", Target.GetIO("PRESENCE")[Presence.Shelf_20_1].ToString());
                            }
                            else
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_20_2", Target.GetIO("PRESENCE")[Presence.Shelf_20_2].ToString());
                            }
                            break;
                        case Presence.Shelf_21_1:
                        case Presence.Shelf_21_2:
                            if (Target.GetIO("PRESENCE")[Presence.Shelf_21_1] == 1 && Target.GetIO("PRESENCE")[Presence.Shelf_21_2] == 1)
                            {
                                FormMainUpdate.Update_IO("Shelf_21", "N/A");
                            }
                            else if (Target.GetIO("PRESENCE")[Presence.Shelf_21_1] == 0 && Target.GetIO("PRESENCE")[Presence.Shelf_21_2] == 0)
                            {
                                FormMainUpdate.Update_IO("Shelf_21", "Placement");
                            }
                            else
                            {
                                FormMainUpdate.Update_IO("Shelf_21", "Presence");
                            }
                            if (Pos == Presence.Shelf_21_1)
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_21_1", Target.GetIO("PRESENCE")[Presence.Shelf_21_1].ToString());
                            }
                            else
                            {
                                FormMainUpdate.Update_IO_Table("Shelf_21_2", Target.GetIO("PRESENCE")[Presence.Shelf_21_2].ToString());
                            }
                            break;
                        case Presence.CstRobot_1:
                        case Presence.CstRobot_2:
                            if (Target.GetIO("PRESENCE")[Presence.CstRobot_1] == 1 && Target.GetIO("PRESENCE")[Presence.CstRobot_2] == 1)
                            {
                                FormMainUpdate.Update_IO("CstRobot", "N/A");
                            }
                            else if (Target.GetIO("PRESENCE")[Presence.CstRobot_1] == 0 && Target.GetIO("PRESENCE")[Presence.CstRobot_2] == 0)
                            {
                                FormMainUpdate.Update_IO("CstRobot", "Placement");
                            }
                            else
                            {
                                FormMainUpdate.Update_IO("CstRobot", "Presence");
                            }
                            break;
                    }
                    break;
            }
        }

        private void btnAutoRun_Click(object sender, EventArgs e)
        {
            BypassSafetyCheck_ck.Checked = false;
            if (IsRun)
            {
                MessageBox.Show("Already running!");
                return;
            }
            //btnAutoRun.Enabled = false;
            IsRun = true;
            DiabledControls(this);
            CycleStop_btn.Enabled = true;
            RepeatCnt = Convert.ToInt64(tbTimes.Text);
            speed = AutoRunSpeed_cbx.Text;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AutoRun));
            ThreadPool.QueueUserWorkItem(new WaitCallback(AutoOutput));


        }
        private void DiabledControls(Control input)
        {
            foreach (Control each in input.Controls)
            {
                if (each is Button || each is ComboBox)
                {
                    each.Enabled = false;
                }
                DiabledControls(each);
            }
        }

        private void AutoOutput(object input)
        {
            Node Target = NodeManagement.Get("CSTROBOT");
            while (IsRun)
            {
                Target.SetIO("OUTPUT", Output.SignalTower_Red, 1);
                SpinWait.SpinUntil(() => false, 500);
                if (!IsRun)
                {
                    break;
                }
                Target.SetIO("OUTPUT", Output.SignalTower_Red, 0);
                Target.SetIO("OUTPUT", Output.SignalTower_Yellow, 1);
                SpinWait.SpinUntil(() => false, 500);
                if (!IsRun)
                {
                    break;
                }
                Target.SetIO("OUTPUT", Output.SignalTower_Yellow, 0);
                Target.SetIO("OUTPUT", Output.SignalTower_Green, 1);
                SpinWait.SpinUntil(() => false, 500);
                if (!IsRun)
                {
                    break;
                }
                Target.SetIO("OUTPUT", Output.SignalTower_Green, 0);
                Target.SetIO("OUTPUT", Output.SignalTower_Blue, 1);
                SpinWait.SpinUntil(() => false, 500);
                if (!IsRun)
                {
                    break;
                }
                Target.SetIO("OUTPUT", Output.SignalTower_Blue, 0);
                Target.SetIO("OUTPUT", Output.Pod1_Lock, 1);
                SpinWait.SpinUntil(() => false, 500);
                if (!IsRun)
                {
                    break;
                }
                Target.SetIO("OUTPUT", Output.Pod1_Lock, 0);
                Target.SetIO("OUTPUT", Output.Pod2_Lock, 1);
                SpinWait.SpinUntil(() => false, 500);
                if (!IsRun)
                {
                    break;
                }
                Target.SetIO("OUTPUT", Output.Pod2_Lock, 0);
                Target.SetIO("OUTPUT", Output.CST_In, 1);
                SpinWait.SpinUntil(() => false, 500);
                if (!IsRun)
                {
                    break;
                }
                Target.SetIO("OUTPUT", Output.CST_In, 0);
                Target.SetIO("OUTPUT", Output.CST_Out, 1);
                SpinWait.SpinUntil(() => false, 500);
                if (!IsRun)
                {
                    break;
                }
                Target.SetIO("OUTPUT", Output.CST_Out, 0);
                Target.SetIO("OUTPUT", Output.Tx_Pause_Front, 1);
                SpinWait.SpinUntil(() => false, 500);
                if (!IsRun)
                {
                    break;
                }
                Target.SetIO("OUTPUT", Output.Tx_Pause_Front, 0);
                Target.SetIO("OUTPUT", Output.Tx_Pause_Rear, 1);
                SpinWait.SpinUntil(() => false, 500);
                if (!IsRun)
                {
                    break;
                }
                Target.SetIO("OUTPUT", Output.Tx_Pause_Rear, 0);
            }
        }
        long CurrntCnt = 0;
        long RepeatCnt = 0;
        string speed = "";
        string CurrentSmif = "23";
        private void AutoRun(object input)
        {
            CurrentSmif = "23";

            int sp = 0;

            if (int.TryParse(speed, out sp))
            {

                if (!Skip[22])
                {
                    if (!TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_SET_SPEED, new Dictionary<string, string>() { { "@Target", "SMIF1" }, { "@Value", sp == 100 ? "00" : sp == 0 ? "10" : sp.ToString() } }, Guid.NewGuid().ToString()).Promise())
                    {
                        IsRun = false;
                    }
                }
                if (!Skip[23])
                {
                    if (!TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_SET_SPEED, new Dictionary<string, string>() { { "@Target", "SMIF2" }, { "@Value", sp == 100 ? "00" : sp == 0 ? "10" : sp.ToString() } }, Guid.NewGuid().ToString()).Promise())
                    {
                        IsRun = false;
                    }
                }
            }

            while (IsRun)
            {
                if (CurrntCnt >= RepeatCnt)
                {
                    IsRun = false;
                    break;
                }
                #region test1


                //if (!IsRun || !TaskFlowManagement.Excute(TaskFlowManagement.Command.OPEN_FOUP, new Dictionary<string, string>() { { "@Target", "SMIF1" } }).Promise())
                //{
                //    IsRun = false;
                //    return;
                //}

                //if (!IsRun || !TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_GET, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Position", "22" }, { "@Speed", speed }, { "@Mode", Mode } }).Promise())
                //{
                //    IsRun = false;
                //    return;
                //}

                //if (!IsRun || !TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_PUT, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Position", "1" }, { "@Speed", speed }, { "@Mode", Mode } }).Promise())
                //{
                //    IsRun = false;
                //    return;
                //}
                //for (int i = 0; i < 20; i++)
                //{
                //    if (i == 8)
                //    {
                //        continue;
                //    }
                //    if (!IsRun || !TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_GET, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Position", (1 + (i == 9 ? 8 : i)).ToString() }, { "@Speed", speed }, { "@Mode", Mode } }).Promise())
                //    {
                //        IsRun = false;
                //        return;
                //    }

                //    if (!IsRun || !TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_PUT, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Position", (2 + i).ToString() }, { "@Speed", speed }, { "@Mode", Mode } }).Promise())
                //    {
                //        IsRun = false;
                //        return;
                //    }



                //}
                //if (!IsRun || !TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_GET, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Position", "21" }, { "@Speed", speed }, { "@Mode", Mode } }).Promise())
                //{
                //    IsRun = false;
                //    return;
                //}

                //if (!IsRun || !TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_PUT, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Position", "1" }, { "@Speed", speed }, { "@Mode", Mode } }).Promise())
                //{
                //    IsRun = false;
                //    return;
                //}
                //if (!IsRun || !TaskFlowManagement.Excute(TaskFlowManagement.Command.CLOSE_FOUP, new Dictionary<string, string>() { { "@Target", "SMIF1" } }).Promise())
                //{
                //    IsRun = false;
                //    return;
                //}
                #endregion

                #region test2



                Stopwatch sw = new Stopwatch();


                //耗時巨大的程式碼  



                for (int i = 0; i < 21; i++)
                //for (int i = 0; i < 2; i++)
                {
                   
                    CurrentSmif = CurrentSmif.Equals("23") ? "22" : "23";
                    if (CurrentSmif.Equals("22") && Skip[22])
                    {
                        CurrentSmif = "23";
                    }
                    if (CurrentSmif.Equals("23") && Skip[23])
                    {
                        CurrentSmif = "22";
                    }
                    if (!Skip[22] || !Skip[23])
                    {
                        if (!IsRun || !TaskFlowManagement.Excute(TaskFlowManagement.Command.OPEN_FOUP, new Dictionary<string, string>() { { "@Target", CurrentSmif.Equals("22") ? "SMIF1" : "SMIF2" } }).Promise())
                        {
                            IsRun = false;
                            break;
                        }

                        sw.Reset();
                        sw.Start();
                    }
                    if (!Skip[22] || !Skip[23])
                    {
                        //if (!IsRun || !TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_GET, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Position", ((i % 2 == 0) ? "22" : "23") }, { "@Speed", speed }, { "@Mode", Mode } }).Promise())
                        if (!IsRun || !TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_GET, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Position", CurrentSmif }, { "@Speed", speed }, { "@Mode", Mode } }).Promise())

                        {
                            IsRun = false;
                            break;
                        }
                    }
                    if (!Skip[i + 1])
                    {


                        if (!IsRun || !TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_PUT, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Position", (1 + i).ToString() }, { "@Speed", speed }, { "@Mode", Mode } }).Promise())
                        {
                            IsRun = false;
                            break;
                        }

                        if (!IsRun || !TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_GET, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Position", (1 + i).ToString() }, { "@Speed", speed }, { "@Mode", Mode } }).Promise())
                        {
                            IsRun = false;
                            break;
                        }
                    }
                    if (!Skip[22] || !Skip[23])
                    {
                        if (!IsRun || !TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_PUT, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Position", CurrentSmif }, { "@Speed", speed }, { "@Mode", Mode } }).Promise())
                        {
                            IsRun = false;
                            break;
                        }

                        sw.Stop();
                        TimeSpan ts2 = sw.Elapsed;
                        UI_Update.FormMainUpdate.LogUpdate((CurrentSmif) + " -> " + "Shelf_" + (1 + i).ToString() + " -> " + CurrentSmif + " Transfer time:" + ts2.TotalSeconds.ToString());

                        if (!IsRun || !TaskFlowManagement.Excute(TaskFlowManagement.Command.CLOSE_FOUP, new Dictionary<string, string>() { { "@Target", CurrentSmif.Equals("22") ? "SMIF1" : "SMIF2" } }).Promise())
                        {
                            IsRun = false;
                            break;
                        }
                    }
                }


                #endregion
                CurrntCnt++;
                FormMainUpdate.CounterUpdate(CurrntCnt.ToString());
            }

            //FormMainUpdate.SetButtonEnable("btnAutoRun", true);
            FormMainUpdate.SetEnable("FormMain");
            FormMainUpdate.SetButtonEnable("CycleStop_btn", false);
        }

        private void CycleStop_btn_Click(object sender, EventArgs e)
        {
            IsRun = false;
            CycleStop_btn.Enabled = false;
        }



        private void SignalTower_Red_Click(object sender, EventArgs e)
        {
            Node Target = NodeManagement.Get("CSTROBOT");
            Target.SetIO("OUTPUT", Output.SignalTower_Red, Target.GetIO("OUTPUT")[Output.SignalTower_Red] == 1 ? (byte)0 : (byte)1);
        }

        private void SignalTower_Yellow_Click(object sender, EventArgs e)
        {
            Node Target = NodeManagement.Get("CSTROBOT");
            Target.SetIO("OUTPUT", Output.SignalTower_Yellow, Target.GetIO("OUTPUT")[Output.SignalTower_Yellow] == 1 ? (byte)0 : (byte)1);
        }

        private void SignalTower_Green_Click(object sender, EventArgs e)
        {
            Node Target = NodeManagement.Get("CSTROBOT");
            Target.SetIO("OUTPUT", Output.SignalTower_Green, Target.GetIO("OUTPUT")[Output.SignalTower_Green] == 1 ? (byte)0 : (byte)1);
        }

        private void SignalTower_Blue_Click(object sender, EventArgs e)
        {
            Node Target = NodeManagement.Get("CSTROBOT");
            Target.SetIO("OUTPUT", Output.SignalTower_Blue, Target.GetIO("OUTPUT")[Output.SignalTower_Blue] == 1 ? (byte)0 : (byte)1);
        }

        private void SignalTower_Buzz1_Click(object sender, EventArgs e)
        {
            Node Target = NodeManagement.Get("CSTROBOT");
            Target.SetIO("OUTPUT", Output.SignalTower_Buzz1, Target.GetIO("OUTPUT")[Output.SignalTower_Buzz1] == 1 ? (byte)0 : (byte)1);
        }

        private void SignalTower_Buzz2_Click(object sender, EventArgs e)
        {
            Node Target = NodeManagement.Get("CSTROBOT");
            Target.SetIO("OUTPUT", Output.SignalTower_Buzz2, Target.GetIO("OUTPUT")[Output.SignalTower_Buzz2] == 1 ? (byte)0 : (byte)1);
        }

        private void Pod1_Lock_Click(object sender, EventArgs e)
        {
            Node Target = NodeManagement.Get("CSTROBOT");
            Target.SetIO("OUTPUT", Output.Pod1_Lock, Target.GetIO("OUTPUT")[Output.Pod1_Lock] == 1 ? (byte)0 : (byte)1);
        }

        private void Pod2_Lock_Click(object sender, EventArgs e)
        {
            Node Target = NodeManagement.Get("CSTROBOT");
            Target.SetIO("OUTPUT", Output.Pod2_Lock, Target.GetIO("OUTPUT")[Output.Pod2_Lock] == 1 ? (byte)0 : (byte)1);
        }

        private void CST_In_Click(object sender, EventArgs e)
        {
            Node Target = NodeManagement.Get("CSTROBOT");
            Target.SetIO("OUTPUT", Output.CST_In, Target.GetIO("OUTPUT")[Output.CST_In] == 1 ? (byte)0 : (byte)1);
        }

        private void CST_Out_Click(object sender, EventArgs e)
        {
            Node Target = NodeManagement.Get("CSTROBOT");
            Target.SetIO("OUTPUT", Output.CST_Out, Target.GetIO("OUTPUT")[Output.CST_Out] == 1 ? (byte)0 : (byte)1);
        }

        private void Tx_Pause_Front_Click(object sender, EventArgs e)
        {
            Node Target = NodeManagement.Get("CSTROBOT");
            Target.SetIO("OUTPUT", Output.Tx_Pause_Front, Target.GetIO("OUTPUT")[Output.Tx_Pause_Front] == 1 ? (byte)0 : (byte)1);
        }

        private void Tx_Pause_Rear_Click(object sender, EventArgs e)
        {
            Node Target = NodeManagement.Get("CSTROBOT");
            Target.SetIO("OUTPUT", Output.Tx_Pause_Rear, Target.GetIO("OUTPUT")[Output.Tx_Pause_Rear] == 1 ? (byte)0 : (byte)1);
        }

        private void tabStocker_Click(object sender, EventArgs e)
        {

        }

        private void gbCmdArea_Enter(object sender, EventArgs e)
        {

        }
        bool[] Skip = new bool[25];
        private void Shelf_MouseClick(object sender, MouseEventArgs e)
        {
            int ShelfNo = Convert.ToInt32(((TextBox)sender).Text.Replace("SHELF_", ""));

            Skip[ShelfNo] = !Skip[ShelfNo];
            if (Skip[ShelfNo])
            {
                ((TextBox)sender).BackColor = System.Drawing.Color.Black;
            }
            else
            {
                ((TextBox)sender).BackColor = System.Drawing.Color.White;
            }

        }

        private void Smif_1_MouseClick(object sender, MouseEventArgs e)
        {
            Skip[22] = !Skip[22];
            if (Skip[22])
            {
                ((TextBox)sender).BackColor = System.Drawing.Color.Black;
            }
            else
            {
                ((TextBox)sender).BackColor = System.Drawing.Color.White;
            }
        }

        private void Smif_2_MouseClick(object sender, MouseEventArgs e)
        {
            Skip[23] = !Skip[23];
            if (Skip[23])
            {
                ((TextBox)sender).BackColor = System.Drawing.Color.Black;
            }
            else
            {
                ((TextBox)sender).BackColor = System.Drawing.Color.White;
            }
        }

        private void tabMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabMode.SelectedIndex == 1)
            {

                NodeManagement.Get("CSTROBOT").SetIO("INPUT_OLD", new byte[512]);
            }
        }

        private void SMIF1_Lift_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_LIFT, new Dictionary<string, string>() { { "@Target", "SMIF1" }, { "@Value", "1" } }, Guid.NewGuid().ToString());
        }

        private void SMIF1_UnLift_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_UNLIFT, new Dictionary<string, string>() { { "@Target", "SMIF1" }, { "@Value", "0" } }, Guid.NewGuid().ToString());
        }

        private void SMIF2_Lift_btn_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_LIFT, new Dictionary<string, string>() { { "@Target", "SMIF2" }, { "@Value", "1" } }, Guid.NewGuid().ToString());
        }

        private void UnLift_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                MessageBox.Show("偵測到正在Auto Run", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_UNLIFT, new Dictionary<string, string>() { { "@Target", "SMIF2" }, { "@Value", "0" } }, Guid.NewGuid().ToString());
        }

        private void Test_btn_Click(object sender, EventArgs e)
        {

            FormMainUpdate.SetFormEnable("FormMain", false);
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_READ_STATUS, new Dictionary<string, string>() { { "@Target", "SMIF1" } }, Guid.NewGuid().ToString());
        }
        private bool checkDryMode()
        {
            if (Mode.Equals("0"))
                return true;

            DialogResult dialogResult = MessageBox.Show("目前 Robot 不在 Normal Mode，\n確定要執行?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (dialogResult == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Mode_cb_SelectedIndexChanged(object sender, EventArgs e)
        {
            Mode = Mode_cb.SelectedValue.ToString();
        }
        Stopwatch Elapsed = new Stopwatch();
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (IsRun)
            {
                if (!Elapsed.IsRunning)
                {
                    Elapsed.Restart();
                }
                Elapsed_lb.Text = Elapsed.Elapsed.Days + ":" + Elapsed.Elapsed.Hours.ToString("00") + ":" + Elapsed.Elapsed.Minutes.ToString("00") + ":" + Elapsed.Elapsed.Seconds.ToString("00") + "." + Elapsed.Elapsed.Milliseconds.ToString("000");
            }
            else
            {
                Elapsed.Stop();
            }
        }

        private void Inter_on_btn_Click(object sender, EventArgs e)
        {
            int pos = Convert.ToInt16(((Button)sender).Name.Split(new string[] { "_" }, StringSplitOptions.None)[1]);
            NodeManagement.Get("CSTROBOT").SetIO("CSTINTERLOCK", pos, 1);
        }

        private void Inter_off_btn_Click(object sender, EventArgs e)
        {
            int pos = Convert.ToInt16(((Button)sender).Name.Split(new string[] { "_" }, StringSplitOptions.None)[1]);
            NodeManagement.Get("CSTROBOT").SetIO("CSTINTERLOCK", pos, 0);
        }

        private void ShelfInter_on_btn_Click(object sender, EventArgs e)
        {
            int pos = Convert.ToInt16(((Button)sender).Name.Split(new string[] { "_" }, StringSplitOptions.None)[1]);
            NodeManagement.Get("CSTROBOT").SetIO("SHELFINTERLOCK", pos, 1);
        }

        private void ShelfInter_off_btn_Click(object sender, EventArgs e)
        {
            int pos = Convert.ToInt16(((Button)sender).Name.Split(new string[] { "_" }, StringSplitOptions.None)[1]);
            NodeManagement.Get("CSTROBOT").SetIO("SHELFINTERLOCK", pos, 0);
        }

        private void VIPInter_on_btn_Click(object sender, EventArgs e)
        {
            int pos = Convert.ToInt16(((Button)sender).Name.Split(new string[] { "_" }, StringSplitOptions.None)[1]);
            NodeManagement.Get("CSTROBOT").SetIO("VIPINTERLOCK", pos, 1);
        }

        private void VIPInter_off_btn_Click(object sender, EventArgs e)
        {
            int pos = Convert.ToInt16(((Button)sender).Name.Split(new string[] { "_" }, StringSplitOptions.None)[1]);
            NodeManagement.Get("CSTROBOT").SetIO("VIPINTERLOCK", pos,0 );
        }
    }

    public class Input
    {
        private const int StationNo = 5;
        private const int Offset = (StationNo - 1) * 32;
        public const int CST_In_Press = 2 + Offset;
        public const int CST_Out_Press = 3 + Offset;
        public const int Tx_Pause_Front_Press = 4 + Offset;
        public const int Tx_Pause_Rear_Press = 5 + Offset;


    }
    class Output
    {
        private const int StationNo = 7;
        private const int Offset = (StationNo - 1) * 32;
        public const int SignalTower_Red = 0 + Offset;
        public const int SignalTower_Yellow = 1 + Offset;
        public const int SignalTower_Green = 2 + Offset;
        public const int SignalTower_Blue = 3 + Offset;
        public const int SignalTower_Buzz1 = 4 + Offset;
        public const int SignalTower_Buzz2 = 5 + Offset;
        public const int Pod1_Lock = 10 + Offset;
        public const int Pod2_Lock = 11 + Offset;
        public const int CST_In = 12 + Offset;
        public const int CST_Out = 13 + Offset;
        public const int Tx_Pause_Front = 14 
            + Offset;
        public const int Tx_Pause_Rear = 15 + Offset;
    }
    class Presence
    {
        private const int Offset = 0;
        public const int Shelf_1_1 = 0 + Offset;
        public const int Shelf_1_2 = 1 + Offset;
        public const int Shelf_2_1 = 2 + Offset;
        public const int Shelf_2_2 = 3 + Offset;
        public const int Shelf_3_1 = 4 + Offset;
        public const int Shelf_3_2 = 5 + Offset;
        public const int Shelf_4_1 = 6 + Offset;
        public const int Shelf_4_2 = 7 + Offset;
        public const int Shelf_5_1 = 8 + Offset;
        public const int Shelf_5_2 = 9 + Offset;  
        public const int Shelf_6_1 = 10 + Offset;
        public const int Shelf_6_2 = 11 + Offset;
        public const int Shelf_7_1 = 12 + Offset;
        public const int Shelf_7_2 = 13 + Offset;
        public const int Shelf_8_1 = 14 + Offset;
        public const int Shelf_8_2 = 15 + Offset;
        public const int Shelf_9_1 = 16 + Offset;
        public const int Shelf_9_2 = 17 + Offset;
        public const int Shelf_10_1 = 18 + Offset;
        public const int Shelf_10_2 = 19 + Offset;
        public const int Shelf_11_1 = 20 + Offset;
        public const int Shelf_11_2 = 21 + Offset;
        public const int Shelf_12_1 = 22 + Offset;
        public const int Shelf_12_2 = 23 + Offset;
        public const int Shelf_13_1 = 32 + Offset;
        public const int Shelf_13_2 = 33 + Offset;
        public const int Shelf_14_1 = 34 + Offset;
        public const int Shelf_14_2 = 35 + Offset;
        public const int Shelf_15_1 = 36 + Offset;
        public const int Shelf_15_2 = 37 + Offset;
        public const int Shelf_16_1 = 38 + Offset;
        public const int Shelf_16_2 = 39 + Offset;
        public const int Shelf_17_1 = 40 + Offset;
        public const int Shelf_17_2 = 41 + Offset;
        public const int Shelf_18_1 = 24 + Offset;
        public const int Shelf_18_2 = 25 + Offset;
        public const int Shelf_19_1 = 26 + Offset;
        public const int Shelf_19_2 = 27 + Offset;
        public const int Shelf_20_1 = 28 + Offset;
        public const int Shelf_20_2 = 29 + Offset;
        public const int Shelf_21_1 = 30 + Offset;
        public const int Shelf_21_2 = 31 + Offset;
        public const int CstRobot_1 = 48;
        public const int CstRobot_2 = 49;
    }
}
