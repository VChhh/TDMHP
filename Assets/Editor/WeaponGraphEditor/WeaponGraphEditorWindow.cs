using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using TDMHP.Combat;
using TDMHP.Combat.Weapons;
using TDMHP.Input;

namespace TDMHP.Editor.Weapons
{
    public sealed class WeaponGraphEditorWindow : EditorWindow
    {
        private WeaponData _currentWeapon;
        private WeaponGraphView _graphView;
        private ObjectField _weaponField;
        private ObjectField _addMoveField;
        private Label _statusLabel;

        [MenuItem("TDMHP/Combat/Weapon Graph Editor")]
        public static void Open()
        {
            var window = GetWindow<WeaponGraphEditorWindow>();
            window.titleContent = new GUIContent("Weapon Graph Editor");
            window.Show();
        }

        private void OnEnable()
        {
            ConstructUI();
            RefreshToolbarState();
        }

        private void OnDisable()
        {
            if (_graphView != null)
                rootVisualElement.Remove(_graphView);
        }

        private void ConstructUI()
        {
            rootVisualElement.Clear();
            rootVisualElement.style.flexDirection = FlexDirection.Column;

            var toolbar = new Toolbar();

            _weaponField = new ObjectField("Weapon")
            {
                objectType = typeof(WeaponData),
                allowSceneObjects = false
            };
            _weaponField.RegisterValueChangedCallback(evt =>
            {
                _currentWeapon = evt.newValue as WeaponData;
                RefreshGraph();
            });
            toolbar.Add(_weaponField);

            _addMoveField = new ObjectField("Add Move")
            {
                objectType = typeof(AttackMoveData),
                allowSceneObjects = false
            };
            toolbar.Add(_addMoveField);

            var addMoveButton = new Button(AddMoveFromToolbar) { text = "Add Node" };
            toolbar.Add(addMoveButton);

            var refreshButton = new Button(RefreshGraph) { text = "Refresh" };
            toolbar.Add(refreshButton);

            var saveButton = new Button(SaveWeapon) { text = "Save" };
            toolbar.Add(saveButton);

            _statusLabel = new Label("No weapon loaded");
            toolbar.Add(_statusLabel);

            rootVisualElement.Add(toolbar);

            _graphView = new WeaponGraphView(this) { name = "WeaponGraphView" };
            _graphView.style.flexGrow = 1f;
            _graphView.style.flexShrink = 1f;
            rootVisualElement.Add(_graphView);
        }

        private void AddMoveFromToolbar()
        {
            var move = _addMoveField.value as AttackMoveData;
            if (move == null)
            {
                NotifyStatus("Select an AttackMoveData asset first.");
                return;
            }

            _graphView.AddMoveNode(move, _graphView.GetCanvasCenter());
            NotifyStatus($"Added node for {move.name}.");
        }

        private void RefreshGraph()
        {
            _graphView.LoadFromWeapon(_currentWeapon);
            RefreshToolbarState();
        }

        private void RefreshToolbarState()
        {
            if (_currentWeapon == null)
            {
                _statusLabel.text = "No weapon loaded";
            }
            else
            {
                _weaponField.SetValueWithoutNotify(_currentWeapon);
                _statusLabel.text = $"Editing {_currentWeapon.weaponId}";
            }
        }

        private void SaveWeapon()
        {
            if (_currentWeapon == null)
            {
                NotifyStatus("Nothing to save - assign a WeaponData asset.");
                return;
            }

            var snapshot = _graphView.BuildSnapshot();
            Undo.RecordObject(_currentWeapon, "Edit Weapon Graph");

            _currentWeapon.lightEntry = snapshot.lightEntry;
            _currentWeapon.heavyEntry = snapshot.heavyEntry;

            _currentWeapon.transitions.Clear();
            _currentWeapon.transitions.AddRange(snapshot.transitions);

            _currentWeapon.nodeLayout.Clear();
            _currentWeapon.nodeLayout.AddRange(snapshot.layout);

            _currentWeapon.connectionLayout.Clear();
            _currentWeapon.connectionLayout.AddRange(snapshot.connectionLayout);

            _currentWeapon.graphEdges.Clear();
            _currentWeapon.graphEdges.AddRange(snapshot.graphEdges);

            EditorUtility.SetDirty(_currentWeapon);
            AssetDatabase.SaveAssets();

            NotifyStatus("Saved weapon graph.");
        }

        public void NotifyStatus(string message)
        {
            _statusLabel.text = message;
            Repaint();
        }
    }
}
