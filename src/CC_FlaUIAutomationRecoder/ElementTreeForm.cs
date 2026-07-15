using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using System.Drawing;

namespace CC_FlaUIAutomationRecoder
{
    public class ElementTreeForm : Form
    {
        private readonly AutomationBase _automation;
        private readonly int _targetProcessId;
        private readonly TreeView _treeView;
        private readonly System.Windows.Forms.Button _btnOk;
        private readonly System.Windows.Forms.Button _btnCancel;
        private AutomationElement _selectedElement;
        private readonly Dictionary<TreeNode, AutomationElement> _nodeElements = new Dictionary<TreeNode, AutomationElement>();

        public AutomationElement SelectedElement => _selectedElement;

        public ElementTreeForm(AutomationBase automation, int targetProcessId, AutomationElement initialElement)
        {
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Font = new Font("Segoe UI", 9F);

            _automation = automation;
            _targetProcessId = targetProcessId;

            Text = "Select Element";
            Size = new Size(480, 420);
            StartPosition = FormStartPosition.CenterParent;

            _treeView = new TreeView
            {
                Dock = DockStyle.Top,
                Height = 320,
                HideSelection = false
            };
            _treeView.AfterSelect += TreeView_AfterSelect;

            _btnOk = new System.Windows.Forms.Button { Text = "Select", DialogResult = DialogResult.OK, Location = new System.Drawing.Point(280, 340), Width = 80 };
            _btnCancel = new System.Windows.Forms.Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new System.Drawing.Point(370, 340), Width = 80 };

            Controls.Add(_treeView);
            Controls.Add(_btnOk);
            Controls.Add(_btnCancel);
            AcceptButton = _btnOk;
            CancelButton = _btnCancel;

            BuildTree(initialElement ?? GetElementAtMouse());
        }

        private AutomationElement GetElementAtMouse()
        {
            try
            {
                var pos = Mouse.Position;
                var element = _automation.FromPoint(pos);
                if (IsTargetElement(element))
                    return element;
            }
            catch { }
            return null;
        }

        private void BuildTree(AutomationElement element)
        {
            _treeView.Nodes.Clear();
            _nodeElements.Clear();

            if (element == null)
            {
                _treeView.Nodes.Add("(no element at cursor)");
                return;
            }

            var walker = _automation.TreeWalkerFactory.GetControlViewWalker();
            var ancestors = new List<AutomationElement>();
            var current = element;
            var visited = new HashSet<AutomationElement>();

            while (current != null && !visited.Contains(current))
            {
                visited.Add(current);
                ancestors.Insert(0, current);
                try { current = walker.GetParent(current); }
                catch { break; }
                if (ancestors.Count > 30)
                    break;
            }

            TreeNode parentNode = null;
            foreach (var ancestor in ancestors)
            {
                var node = CreateNode(ancestor);
                _nodeElements[node] = ancestor;

                if (parentNode == null)
                    _treeView.Nodes.Add(node);
                else
                    parentNode.Nodes.Add(node);

                parentNode = node;
            }

            if (parentNode != null)
            {
                AddChildren(parentNode, element, walker);
                parentNode.Expand();
                _treeView.SelectedNode = parentNode;
            }
        }

        private void AddChildren(TreeNode parentNode, AutomationElement parentElement, ITreeWalker walker)
        {
            try
            {
                var children = parentElement.FindAllChildren();
                foreach (var child in children)
                {
                    if (!IsTargetElement(child))
                        continue;

                    var childNode = CreateNode(child);
                    _nodeElements[childNode] = child;
                    parentNode.Nodes.Add(childNode);
                }
            }
            catch { }
        }

        private TreeNode CreateNode(AutomationElement element)
        {
            string name = "(unnamed)";
            string controlType = "Unknown";
            string automationId = string.Empty;

            try
            {
                if (element.Properties.Name.TryGetValue(out var n) && !string.IsNullOrEmpty(n))
                    name = n;
                if (element.Properties.ControlType.TryGetValue(out var ct))
                    controlType = ct.ToString();
                if (element.Properties.AutomationId.TryGetValue(out var aid) && !string.IsNullOrEmpty(aid))
                    automationId = " [" + aid + "]";
            }
            catch (System.Runtime.InteropServices.COMException) { }
            catch (InvalidOperationException) { }

            return new TreeNode($"{controlType}: {name}{automationId}");
        }

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null && _nodeElements.TryGetValue(e.Node, out var element))
                _selectedElement = element;
        }

        private bool IsTargetElement(AutomationElement element)
        {
            if (element == null)
                return false;
            try { return element.Properties.ProcessId == _targetProcessId; }
            catch { return false; }
        }
    }
}
