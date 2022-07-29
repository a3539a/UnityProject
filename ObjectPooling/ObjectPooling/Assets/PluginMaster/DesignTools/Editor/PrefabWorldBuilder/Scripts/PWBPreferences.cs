/*
Copyright (c) 2022 Omar Duarte
Unauthorized copying of this file, via any medium is strictly prohibited.
Writen by Omar Duarte, 2022.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using UnityEditor;
using UnityEngine;

namespace PluginMaster
{
    public class PWBPreferences : EditorWindow
    {
        private Vector2 _mainScrollPosition = Vector2.zero;
        private bool _undoGroupOpen = true;
        private bool _autoSaveGroupOpen = true;
        private bool _unsavedChangesGroupOpen = true;


        [MenuItem("Tools/Plugin Master/Prefab World Builder/Preferences...", false, 1250)]
        public static void ShowWindow() => GetWindow<PWBPreferences>("Prefab World Builder Preferences");

        private void OnGUI()
        {
            using (var scrollView = new EditorGUILayout.ScrollViewScope(_mainScrollPosition,
                false, false, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUIStyle.none))
            {
                _mainScrollPosition = scrollView.scrollPosition;
                
                _autoSaveGroupOpen = EditorGUILayout.BeginFoldoutHeaderGroup(_autoSaveGroupOpen, "Auto-Save Settings");
                if (_autoSaveGroupOpen) AutoSaveGroup();
                EditorGUILayout.EndFoldoutHeaderGroup();

                _undoGroupOpen = EditorGUILayout.BeginFoldoutHeaderGroup(_undoGroupOpen, "Undo Settings");
                if (_undoGroupOpen) UndoGroup();
                EditorGUILayout.EndFoldoutHeaderGroup();

                _unsavedChangesGroupOpen = EditorGUILayout.BeginFoldoutHeaderGroup(_unsavedChangesGroupOpen,
                    "Unsaved Changes");
                if (_unsavedChangesGroupOpen) UnsavedChangesGroup();
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }

        private void AutoSaveGroup()
        {
            using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Auto-Save Every:");
                PWBCore.staticData.autoSavePeriodMinutes
                    = EditorGUILayout.IntSlider(PWBCore.staticData.autoSavePeriodMinutes, 1, 10);
                GUILayout.Label("minutes");
                GUILayout.FlexibleSpace();
            }
        }

        private void UndoGroup()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    PWBCore.staticData.undoBrushProperties = EditorGUILayout.ToggleLeft("Undo Brush properties changes",
                        PWBCore.staticData.undoBrushProperties);
                    if (check.changed && !PWBCore.staticData.undoBrushProperties) BrushProperties.ClearUndo();
                }
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    PWBCore.staticData.undoToolProperties
                        = EditorGUILayout.ToggleLeft("Undo Tool properties changes", PWBCore.staticData.undoToolProperties);
                    if (check.changed && !PWBCore.staticData.undoToolProperties) ToolProperties.ClearUndo();
                }
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    PWBCore.staticData.undoPalette
                        = EditorGUILayout.ToggleLeft("Undo Palette changes", PWBCore.staticData.undoPalette);
                    if (check.changed && !PWBCore.staticData.undoPalette) PrefabPalette.ClearUndo();
                }
            }
        }

        private static readonly string[] _unsavedChangesActionNames = { "Ask if want to save", "Save", "Discard" };
        private void UnsavedChangesGroup()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUIUtility.labelWidth = 45;
                PWBCore.staticData.unsavedChangesAction = (PWBData.UnsavedChangesAction) EditorGUILayout.Popup("Action:",
                    (int)PWBCore.staticData.unsavedChangesAction, _unsavedChangesActionNames);
            }
        }
    }
}
