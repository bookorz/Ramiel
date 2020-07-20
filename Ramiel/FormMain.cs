using InControls.PLC.Mitsubishi;
using log4net.Config;
using Ramiel.UI_Update;
using System;
using System.Collections.Generic;
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
            ThreadPool.QueueUserWorkItem(new WaitCallback(CCLINKAutoRefresh), NodeManagement.Get("CSTROBOT"));
        }
        public void On_Alarm_Happen(AlarmManagement.Alarm Alarm)
        {

        }

        public void On_Command_Error(Node Node, Transaction Txn, CommandReturnMessage Msg)
        {

        }

        public void On_Command_Excuted(Node Node, Transaction Txn, CommandReturnMessage Msg)
        {

        }

        public void On_Command_Finished(Node Node, Transaction Txn, CommandReturnMessage Msg)
        {

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

        }

        public void On_Event_Trigger(Node Node, CommandReturnMessage Msg)
        {

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

        }

        public void On_TaskJob_Ack(TaskFlowManagement.CurrentProcessTask Task)
        {

        }

        public void On_TaskJob_Finished(TaskFlowManagement.CurrentProcessTask Task)
        {

        }




        private void SMIF1_Stage_btn_Click(object sender, EventArgs e)
        {
            TaskFlowManagement.Excute(TaskFlowManagement.Command.OPEN_FOUP, new Dictionary<string, string>() { { "@Target", "SMIF1" } });
        }

        private void SMIF2_Stage_btn_Click(object sender, EventArgs e)
        {
            TaskFlowManagement.Excute(TaskFlowManagement.Command.OPEN_FOUP, new Dictionary<string, string>() { { "@Target", "SMIF2" } });
        }

        private void SMIF1_Home_btn_Click(object sender, EventArgs e)
        {
            TaskFlowManagement.Excute(TaskFlowManagement.Command.CLOSE_FOUP, new Dictionary<string, string>() { { "@Target", "SMIF1" } });
        }

        private void SMIF2_Home_btn_Click(object sender, EventArgs e)
        {
            TaskFlowManagement.Excute(TaskFlowManagement.Command.CLOSE_FOUP, new Dictionary<string, string>() { { "@Target", "SMIF2" } });
        }

        private void SMIF1_SetSpeed_btn_Click(object sender, EventArgs e)
        {
            int speed = 0;
            if (int.TryParse(SMIF1_Speed_cbx.Text, out speed))
            {
                TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_SET_SPEED, new Dictionary<string, string>() { { "@Target", "SMIF1" }, { "@Value", speed == 100 ? "00" : speed == 0 ? "10" : speed.ToString() } });
            }
            else
            {
                MessageBox.Show("Speed invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SMIF2_SetSpeed_btn_Click(object sender, EventArgs e)
        {
            int speed = 0;
            if (int.TryParse(SMIF2_Speed_cbx.Text, out speed))
            {
                TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_SET_SPEED, new Dictionary<string, string>() { { "@Target", "SMIF2" }, { "@Value", speed == 100 ? "00" : speed == 0 ? "10" : speed.ToString() } });
            }
            else
            {
                MessageBox.Show("Speed invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SMIF1_MoveTo_btn_Click(object sender, EventArgs e)
        {
            int slot = 0;
            if (int.TryParse(SMIF1_Slot_cbx.Text, out slot))
            {
                TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_SLOT, new Dictionary<string, string>() { { "@Target", "SMIF1" }, { "@Value", slot.ToString() } });
            }
            else
            {
                MessageBox.Show("Slot invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SMIF2_MoveTo_btn_Click(object sender, EventArgs e)
        {
            int slot = 0;
            if (int.TryParse(SMIF2_Slot_cbx.Text, out slot))
            {
                TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_SLOT, new Dictionary<string, string>() { { "@Target", "SMIF2" }, { "@Value", slot.ToString() } });
            }
            else
            {
                MessageBox.Show("Slot invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SMIF1_TwkUp_btn_Click(object sender, EventArgs e)
        {
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_TWKUP, new Dictionary<string, string>() { { "@Target", "SMIF1" } });
        }
        private void SMIF2_TwkUp_btn_Click(object sender, EventArgs e)
        {
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_TWKUP, new Dictionary<string, string>() { { "@Target", "SMIF2" } });
        }

        private void SMIF1_TwkDn_btn_Click(object sender, EventArgs e)
        {
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_TWKDN, new Dictionary<string, string>() { { "@Target", "SMIF1" } });
        }

        private void SMIF2_TwkDn_btn_Click(object sender, EventArgs e)
        {
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_TWKDN, new Dictionary<string, string>() { { "@Target", "SMIF2" } });
        }

        private void SMIF1_ReMap_btn_Click(object sender, EventArgs e)
        {
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_RE_MAPPING, new Dictionary<string, string>() { { "@Target", "SMIF1" } });
        }

        private void SMIF2_ReMap_btn_Click(object sender, EventArgs e)
        {
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_RE_MAPPING, new Dictionary<string, string>() { { "@Target", "SMIF2" } });
        }

        private void SMIF1_Clamp_btn_Click(object sender, EventArgs e)
        {
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_CLAMP, new Dictionary<string, string>() { { "@Target", "SMIF1" } });
        }

        private void SMIF2_Clamp_btn_Click(object sender, EventArgs e)
        {
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_CLAMP, new Dictionary<string, string>() { { "@Target", "SMIF2" } });
        }

        private void SMIF1_UnClamp_btn_Click(object sender, EventArgs e)
        {
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_UNCLAMP, new Dictionary<string, string>() { { "@Target", "SMIF1" } });
        }

        private void SMIF2_UnClamp_btn_Click(object sender, EventArgs e)
        {
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_UNCLAMP, new Dictionary<string, string>() { { "@Target", "SMIF2" } });
        }

        private void SMIF1_Org_btn_Click(object sender, EventArgs e)
        {
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_ORGSH, new Dictionary<string, string>() { { "@Target", "SMIF1" } });
        }

        private void SMIF2_Org_btn_Click(object sender, EventArgs e)
        {
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_ORGSH, new Dictionary<string, string>() { { "@Target", "SMIF2" } });
        }

        private void SMIF1_Reset_btn_Click(object sender, EventArgs e)
        {
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_RESET, new Dictionary<string, string>() { { "@Target", "SMIF1" } });
        }

        private void SMIF2_Reset_btn_Click(object sender, EventArgs e)
        {
            TaskFlowManagement.Excute(TaskFlowManagement.Command.LOADPORT_RESET, new Dictionary<string, string>() { { "@Target", "SMIF2" } });
        }

        private void CstRobot_Switch_btn_Click(object sender, EventArgs e)
        {
            string Swap = CstRobot_Source_cbx.Text;
            CstRobot_Source_cbx.Text = CstRobot_Destination_cbx.Text;
            CstRobot_Destination_cbx.Text = Swap;
        }

        private void CstRobot_Source_GetWait_btn_Click(object sender, EventArgs e)
        {
            if (!CstRobot_Source_cbx.SelectedValue.ToString().Equals(""))
            {
                TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_GETWAIT, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Position", CstRobot_Source_cbx.SelectedValue.ToString() }, { "@Speed", CstRobot_Speed_cbx.Text }, { "@Mode", Mode } });
            }
            else
            {
                MessageBox.Show("Source invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CstRobot_Source_Get_btn_Click(object sender, EventArgs e)
        {
            if (!CstRobot_Source_cbx.SelectedValue.ToString().Equals(""))
            {
                TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_GET, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Position", CstRobot_Source_cbx.SelectedValue.ToString() }, { "@Speed", CstRobot_Speed_cbx.Text }, { "@Mode", Mode } });
            }
            else
            {
                MessageBox.Show("Source invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void CstRobot_Reset_btn_Click(object sender, EventArgs e)
        {
            TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_RESET, new Dictionary<string, string>() { { "@Target", "CSTROBOT" } });
        }
        private void CstRobot_Org_btn_Click(object sender, EventArgs e)
        {
            TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_ORGSH, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Speed", CstRobot_Speed_cbx.Text }, { "@Mode", Mode } });
        }

        private void CstRobot_Home_btn_Click(object sender, EventArgs e)
        {
            TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_SHOME, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Speed", CstRobot_Speed_cbx.Text }, { "@Mode", Mode } });
        }



        private void CstRobot_Source_PutWait_btn_Click(object sender, EventArgs e)
        {
            if (!CstRobot_Destination_cbx.SelectedValue.ToString().Equals(""))
            {
                TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_PUTWAIT, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Position", CstRobot_Destination_cbx.SelectedValue.ToString() }, { "@Speed", CstRobot_Speed_cbx.Text }, { "@Mode", Mode } });
            }
            else
            {
                MessageBox.Show("Destination invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CstRobot_Source_Put_btn_Click(object sender, EventArgs e)
        {
            if (!CstRobot_Destination_cbx.SelectedValue.ToString().Equals(""))
            {
                TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_PUT, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Position", CstRobot_Destination_cbx.SelectedValue.ToString() }, { "@Speed", CstRobot_Speed_cbx.Text }, { "@Mode", Mode } });
            }
            else
            {
                MessageBox.Show("Destination invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private byte[] ConvertToBit(int[] WResult)
        {
            string BitStr = "";
            byte[] result = new byte[512];
            for (int i = 0; i < WResult.Length; i++)
            {
                BitStr += new String(Convert.ToString(Convert.ToInt16(WResult[i]), 2).PadLeft(16, '0').Reverse().ToArray());
            }

            for (int i = 0; i < BitStr.Length; i++)
            {
                result[i] = Convert.ToByte(BitStr[i].ToString());
            }
            return result;
        }
        private void CCLINKAutoRefresh(object node)
        {
            int AddrOffset = 1280;
            int CstRobotStation = 9;
            Node Target = (Node)node;
            //SpinWait.SpinUntil(() => Target.GetController().GetStatus().Equals("Connected"), 9999999);
            McProtocolTcp PLC = new McProtocolTcp("192.168.3.39", 2000);
            PLC.Open();
            byte[] result = new byte[512];
            int[] WResult = new int[32];

            //INIT
            result = new byte[512];
            PLC.GetBitDevice(PlcDeviceType.Y, AddrOffset, 512, result);

            Target.SetIO("OUTPUT", result);
            result = new byte[512];
            Target.SetIO("OUTPUT_OLD", result);
            result = new byte[512];
            PLC.GetBitDevice(PlcDeviceType.X, AddrOffset, 512, result);
            Target.SetIO("INPUT", result);
            result = new byte[512];
            Target.SetIO("INPUT_OLD", result);
            WResult = new int[32];
            PLC.ReadDeviceBlock(PlcDeviceType.D, 24576, 32, WResult);
            result = ConvertToBit(WResult);
            Target.SetIO("PRESENCE", result);
            result = new byte[512];
            Target.SetIO("PRESENCE_OLD", result);

            //PLC.SetBitDevice(PlcDeviceType.Y, new Dictionary<int, byte>() { { 1280,1}, { 1281, 1 }, { 1285, 1 } });

            while (true)
            {
                try
                {
                    //SpinWait.SpinUntil(() => false, 10);

                    if (!Target.GetIO("OUTPUT").SequenceEqual(Target.GetIO("OUTPUT_OLD")))
                    {
                        Dictionary<int, byte> changedList = new Dictionary<int, byte>();
                        for (int i = 0; i < Target.GetIO("OUTPUT").Length; i++)
                        {
                            if (Target.GetIO("OUTPUT")[i] != Target.GetIO("OUTPUT_OLD")[i])
                            {
                                changedList.Add(i + AddrOffset, Target.GetIO("OUTPUT")[i]);
                                //_TaskReport.On_Message_Log("IO", "Y Area [" + (i + AddrOffset).ToString("X4") + "] " + Target.GetIO("OUTPUT_OLD")[i] + "->" + Target.GetIO("OUTPUT")[i]);
                                Target.SetIO("OUTPUT_OLD", i, Target.GetIO("OUTPUT")[i]);
                                UpdateUI("OUTPUT", i, Target.GetIO("OUTPUT")[i], Target);
                            }
                        }
                        PLC.SetBitDevice(PlcDeviceType.Y, changedList);


                    }

                    PLC.GetBitDevice(PlcDeviceType.X, AddrOffset, 512, result);
                    Target.SetIO("INPUT", result);
                    if (!Target.GetIO("INPUT").SequenceEqual(Target.GetIO("INPUT_OLD")))
                    {
                        for (int i = 0; i < Target.GetIO("INPUT").Length; i++)
                        {

                            if (Target.GetIO("INPUT")[i] != Target.GetIO("INPUT_OLD")[i])
                            {
                                //_TaskReport.On_Message_Log("IO", "X Area [" + (i + AddrOffset).ToString("X4") + "] " + Target.GetIO("INPUT_OLD")[i] + "->" + Target.GetIO("INPUT")[i]);
                                UpdateUI("INPUT", i, Target.GetIO("INPUT")[i], Target);
                            }
                        }
                        if (Target.GetIO("INPUT")[6 + (CstRobotStation - 1) * 32] == 1 && Target.GetIO("INPUT_OLD")[6 + (CstRobotStation - 1) * 32] == 0)
                        {
                            string errAry = "";
                            for (int i = 32; i <= 64; i++)
                            {
                                errAry += Target.GetIO("INPUT")[i + (CstRobotStation - 1) * 32].ToString();
                            }

                            string error = new String(errAry.Reverse().ToArray());
                            error = Convert.ToInt32(error, 2).ToString("X");
                            this.On_Alarm_Happen(new AlarmManagement.Alarm(Target, error));
                        }
                        Target.SetIO("INPUT_OLD", Target.GetIO("INPUT"));

                    }

                    WResult = new int[32];
                    PLC.ReadDeviceBlock(PlcDeviceType.D, 24576, 32, WResult);
                    result = ConvertToBit(WResult);
                    Target.SetIO("PRESENCE", result);
                    if (!Target.GetIO("PRESENCE").SequenceEqual(Target.GetIO("PRESENCE_OLD")))
                    {
                        for (int i = 0; i < Target.GetIO("PRESENCE").Length; i++)
                        {

                            if (Target.GetIO("PRESENCE")[i] != Target.GetIO("PRESENCE_OLD")[i])
                            {
                                //_TaskReport.On_Message_Log("IO", "X Area [" + (i + AddrOffset).ToString("X4") + "] " + Target.GetIO("INPUT_OLD")[i] + "->" + Target.GetIO("INPUT")[i]);
                                UpdateUI("PRESENCE", i, Target.GetIO("PRESENCE")[i], Target);
                            }
                        }

                        Target.SetIO("PRESENCE_OLD", Target.GetIO("PRESENCE"));

                    }
                }
                catch (Exception e)
                {
                    On_Message_Log("IO", "Lost connection with PLC");
                    SpinWait.SpinUntil(() => false, 5000);
                }
            }
        }
        private void UpdateUI(string Type, int Pos, byte Val, Node Target)
        {
            switch (Type)
            {
                case "INPUT":
                    switch (Pos)
                    {
                        case Input.CST_In_Press:
                            FormMainUpdate.Update_IO("CST_In_Press", Val == 0 ? "Placement" : "N/A");
                            break;
                        case Input.CST_Out_Press:
                            FormMainUpdate.Update_IO("CST_Out_Press", Val == 0 ? "Placement" : "N/A");
                            break;
                        case Input.Tx_Pause_Front_Press:
                            FormMainUpdate.Update_IO("Tx_Pause_Front_Press", Val == 1 ? "Placement" : "N/A");
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
            if (IsRun)
            {
                MessageBox.Show("Already running!");
                return;
            }
            speed = AutoRunSpeed_cbx.Text;
            ThreadPool.QueueUserWorkItem(new WaitCallback(AutoRun));
        }
        string speed = "";
        private void AutoRun(object input)
        {

            IsRun = true;
            while (IsRun)
            {
                if (!IsRun || !TaskFlowManagement.Excute(TaskFlowManagement.Command.OPEN_FOUP, new Dictionary<string, string>() { { "@Target", "SMIF1" } }).Promise())
                {
                    IsRun = false;
                    return;
                }

                if (!IsRun || !TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_GET, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Position", "22" }, { "@Speed", speed }, { "@Mode", Mode } }).Promise())
                {
                    IsRun = false;
                    return;
                }

                if (!IsRun || !TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_PUT, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Position", "1" }, { "@Speed", speed }, { "@Mode", Mode } }).Promise())
                {
                    IsRun = false;
                    return;
                }
                for (int i = 0; i < 21; i++)
                {
                    if (i == 8)
                    {
                        continue;
                    }
                    if (!IsRun||!TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_GET, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Position", (1 + (i==9?8:i)).ToString() }, { "@Speed", speed }, { "@Mode", Mode } }).Promise())
                    {
                        IsRun = false;
                        return;
                    }

                    if (!IsRun||!TaskFlowManagement.Excute(TaskFlowManagement.Command.ROBOT_PUT, new Dictionary<string, string>() { { "@Target", "CSTROBOT" }, { "@Position", (2 + i).ToString() }, { "@Speed", speed }, { "@Mode", Mode } }).Promise())
                    {
                        IsRun = false;
                        return;
                    }

                    

                }

                if (!IsRun || !TaskFlowManagement.Excute(TaskFlowManagement.Command.CLOSE_FOUP, new Dictionary<string, string>() { { "@Target", "SMIF1" } }).Promise())
                {
                    IsRun = false;
                    return;
                }
            }
        }

        private void CycleStop_btn_Click(object sender, EventArgs e)
        {
            IsRun = false;
        }

        class Input
        {
            public const int CST_In_Press = 2;
            public const int CST_Out_Press = 3;
            public const int Tx_Pause_Front_Press = 4;

        }
        class Output
        {
            public const int SignalTower_Red = 128;
            public const int SignalTower_Yellow = 129;
            public const int SignalTower_Green = 130;
            public const int SignalTower_Blue = 131;
            public const int SignalTower_Buzz1 = 132;
            public const int SignalTower_Buzz2 = 133;
            public const int Pod1_Lock = 138;
            public const int Pod2_Lock = 139;
            public const int CST_In = 140;
            public const int CST_Out = 141;
            public const int Tx_Pause_Front = 142;
            public const int Tx_Pause_Rear = 143;
        }
        class Presence
        {
            public const int Shelf_1_1 = 0;
            public const int Shelf_1_2 = 1;
            public const int Shelf_2_1 = 2;
            public const int Shelf_2_2 = 3;
            public const int Shelf_3_1 = 4;
            public const int Shelf_3_2 = 5;
            public const int Shelf_4_1 = 6;
            public const int Shelf_4_2 = 7;
            public const int Shelf_5_1 = 8;
            public const int Shelf_5_2 = 9;
            public const int Shelf_6_1 = 10;
            public const int Shelf_6_2 = 11;
            public const int Shelf_7_1 = 12;
            public const int Shelf_7_2 = 13;
            public const int Shelf_8_1 = 14;
            public const int Shelf_8_2 = 15;
            public const int Shelf_9_1 = 16;
            public const int Shelf_9_2 = 17;
            public const int Shelf_10_1 = 18;
            public const int Shelf_10_2 = 19;
            public const int Shelf_11_1 = 20;
            public const int Shelf_11_2 = 21;
            public const int Shelf_12_1 = 22;
            public const int Shelf_12_2 = 23;
            public const int Shelf_13_1 = 32;
            public const int Shelf_13_2 = 33;
            public const int Shelf_14_1 = 34;
            public const int Shelf_14_2 = 35;
            public const int Shelf_15_1 = 36;
            public const int Shelf_15_2 = 37;
            public const int Shelf_16_1 = 38;
            public const int Shelf_16_2 = 39;
            public const int Shelf_17_1 = 40;
            public const int Shelf_17_2 = 41;
            public const int Shelf_18_1 = 24;
            public const int Shelf_18_2 = 25;
            public const int Shelf_19_1 = 26;
            public const int Shelf_19_2 = 27;
            public const int Shelf_20_1 = 28;
            public const int Shelf_20_2 = 29;
            public const int Shelf_21_1 = 30;
            public const int Shelf_21_2 = 31;
            public const int CstRobot_1 = 64;
            public const int CstRobot_2 = 65;
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




    }
}
