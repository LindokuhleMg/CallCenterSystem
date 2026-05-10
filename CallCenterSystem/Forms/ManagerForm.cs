using CallCenterSystem.Manager;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static CallCenterSystem.Patterns.CompositePattern;
using static CallCenterSystem.Patterns.IteratorPattern;
using static CallCenterSystem.Patterns.StatePattern;

namespace CallCenterSystem.Forms
{
    public class ManagerForm : Form
    {
     
        private TextBox txtSearchCall_Manager;
        private Button btnNextCall_Manager, btnPrevCall_Manager, btnReturnCall_Manager;
        private TreeView tvCallGroups_Manager;
        private Label lblSelectedCall_Manager;

        // Iterator for navigation
        private CallLogIterator selectedIterator_Manager;
        private Call selectedCall_Manager;

        public ManagerForm()
        {
            Text = "Manager Portal";
            ClientSize = new Size(800, 500);
            StartPosition = FormStartPosition.CenterScreen;

            InitializeControls();
            LoadCallGroups();
        }

        private void InitializeControls()
        {
            Controls.Add(new Label() { Text = "Search Calls:", Location = new Point(20, 18), AutoSize = true });
            txtSearchCall_Manager = new TextBox() { Location = new Point(100, 14), Width = 200 };
            txtSearchCall_Manager.TextChanged += TxtSearchCall_Manager_TextChanged;
            Controls.Add(txtSearchCall_Manager);

            btnNextCall_Manager = new Button() { Text = "Next", Location = new Point(320, 12), Width = 80 };
            btnNextCall_Manager.Click += BtnNextCall_Manager_Click;
            Controls.Add(btnNextCall_Manager);

            btnPrevCall_Manager = new Button() { Text = "Previous", Location = new Point(410, 12), Width = 80 };
            btnPrevCall_Manager.Click += BtnPrevCall_Manager_Click;
            Controls.Add(btnPrevCall_Manager);

            btnReturnCall_Manager = new Button() { Text = "Return Call", Location = new Point(500, 12), Width = 120 };
            btnReturnCall_Manager.Click += BtnReturnCall_Manager_Click;
            Controls.Add(btnReturnCall_Manager);

            tvCallGroups_Manager = new TreeView() { Location = new Point(20, 50), Size = new Size(750, 400) };
            tvCallGroups_Manager.AfterSelect += TvCallGroups_Manager_AfterSelect;
            Controls.Add(tvCallGroups_Manager);

            lblSelectedCall_Manager = new Label() { Text = "Selected Call: None", Location = new Point(20, 460), AutoSize = true };
            Controls.Add(lblSelectedCall_Manager);
        }

        // This load calls grouped by Student using Composite
        private void LoadCallGroups()
        {
            tvCallGroups_Manager.Nodes.Clear();

            // Create Composite structure
            GroupedCalls rootGroup = new GroupedCalls("All Students");

            var groupedCalls = AppManager.CallLog.GetAll()
                .Where(c => c.StateName == "Call Ended") // only ended calls
                .GroupBy(c => c.StudentNumber);

            foreach (var group in groupedCalls)
            {
                GroupedCalls studentGroup = new GroupedCalls(group.Key); // each student
                foreach (var call in group)
                {
                    studentGroup.Add(new CallLeaf(call));
                }
                rootGroup.Add(studentGroup);
            }

            // Populate TreeView
            PopulateTreeView(rootGroup, tvCallGroups_Manager.Nodes);
            tvCallGroups_Manager.ExpandAll();
        }

        // Recursive method to populate TreeView from Composite
        private void PopulateTreeView(CallComponent component, TreeNodeCollection nodes)
        {
            TreeNode node = new TreeNode(component is GroupedCalls gc ? $"Student: {gc.Name}" :
                                           component is CallLeaf cl ? $"{cl.Call.CallerName} - {cl.Call.StateName} - {cl.Call.DurationString()}" : "");
            if (component is CallLeaf leaf) node.Tag = leaf.Call;

            nodes.Add(node);

            foreach (var child in component)
            {
                PopulateTreeView(child, node.Nodes);
            }
        }

        private void TvCallGroups_Manager_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is Call call)
            {
                selectedCall_Manager = call;
                lblSelectedCall_Manager.Text = $"Selected Call: {call.StudentNumber} - {call.CallerName}";
            }
            else
            {
                selectedCall_Manager = null;
                lblSelectedCall_Manager.Text = "Selected Call: None";
            }
        }

        //Iterator Navigation
        private void BtnNextCall_Manager_Click(object sender, EventArgs e)
        {
            if (selectedIterator_Manager == null)
                selectedIterator_Manager = (CallLogIterator)AppManager.CallLog.CreateIterator();

            selectedCall_Manager = selectedIterator_Manager.Next();
            UpdateSelectedCallLabel();
        }

        private void BtnPrevCall_Manager_Click(object sender, EventArgs e)
        {
            if (selectedIterator_Manager == null)
                selectedIterator_Manager = (CallLogIterator)AppManager.CallLog.CreateIterator();

            selectedCall_Manager = selectedIterator_Manager.Previous();
            UpdateSelectedCallLabel();
        }

        private void UpdateSelectedCallLabel()
        {
            if (selectedCall_Manager != null)
                lblSelectedCall_Manager.Text = $"Selected Call: {selectedCall_Manager.StudentNumber} - {selectedCall_Manager.CallerName}";
            else
                lblSelectedCall_Manager.Text = "Selected Call: None";
        }

        //This return Call via Proxy
        private void BtnReturnCall_Manager_Click(object sender, EventArgs e)
        {
            if (selectedCall_Manager == null)
            {
                MessageBox.Show("Please select a call first.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //This create new call via proxy
            Call newCall = new Call(
                selectedCall_Manager.StudentNumber,
                selectedCall_Manager.CallerName,
                fromTechnician: true
            );
            AppManager.PhoneProxy.MakeCall(newCall);

            MessageBox.Show($"Returned call to {selectedCall_Manager.CallerName}.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //Open ActiveCallForm for the new call
            ActiveCallForm activeForm = new ActiveCallForm(newCall);
            activeForm.FormClosed += (s, args) =>
            {
                LoadCallGroups();
            };
            activeForm.ShowDialog();
        }


        private void TxtSearchCall_Manager_TextChanged(object sender, EventArgs e)
        {
            string filter = txtSearchCall_Manager.Text.Trim();

            tvCallGroups_Manager.Nodes.Clear();

            GroupedCalls rootGroup = new GroupedCalls("All Students");

            // Filter calls by student number
            var groupedCalls = AppManager.CallLog.GetAll()
                .Where(c => c.StateName == "Call Ended" && c.StudentNumber.Contains(filter))
                .GroupBy(c => c.StudentNumber);

            foreach (var group in groupedCalls)
            {
                GroupedCalls studentGroup = new GroupedCalls(group.Key);
                foreach (var call in group)
                {
                    studentGroup.Add(new CallLeaf(call));
                }
                rootGroup.Add(studentGroup);
            }

            PopulateTreeView(rootGroup, tvCallGroups_Manager.Nodes);
            tvCallGroups_Manager.ExpandAll();
        }
    }
}
