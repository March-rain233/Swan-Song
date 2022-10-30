using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using GameToolKit.Utility;
using System.Collections.Generic;
using System;
using System.Linq;

namespace GameToolKit.Dialog.Editor {
    public class TextEditor : EditorWindow
    {
        public VisualTreeAsset VisualTree;

        /// <summary>
        /// Դ�ı�
        /// </summary>
        public string RawText
        {
            get => _tree.ToString();
            set
            {
                _tree = new RichTextTree(value);
                plainTextField.SetValueWithoutNotify(_tree.GetPlainText());
                OnTreeUpdata();
            }
        }
        
        RichTextTree _tree;

        public int BeginIndex => beginIndexField.value;
        public int EndIndex => endIndexField.value;

        bool plainTextFocus = false;
        bool _showRawText = true;
        bool _tagValid => !tagField.ClassListContains("errorField");

        List<(string menuPath, Type type)> _pairTagList;
        List<(string menuPath, Type type)> _emptyTagList;

        TextField rawTextField;
        TextField plainTextField;
        Label previewTextField;

        IntegerField rawTextLength;
        IntegerField plainTextLength;
        IntegerField beginIndexField;
        IntegerField endIndexField;

        RadioButton flag;
        Button playBtn;
        Button tagApply;

        VisualElement floatBtn;

        TextField tagField;
        DropdownField tagSelector;
        VisualElement tagBody;
        Label tagTitle;
        TagView tagView => tagBody.Q<TagView>();
        bool isCustomTag => tagSelector.value == "Custom";

        [MenuItem("GameToolKit/Text Editor")]
        public static void ShowExample()
        {
            TextEditor wnd = GetWindow<TextEditor>();
            wnd.titleContent = new GUIContent("Text Editor");
        }

        public void CreateGUI()
        {
            InitTagList();

            VisualElement root  = rootVisualElement;
            VisualTree.CloneTree(root);

            rawTextField = root.Q<TextField>("raw-text");
            plainTextField = root.Q<TextField>("plain-text");
            previewTextField = root.Q("preview-text").Q<Label>("body");
            rawTextLength = root.Q<IntegerField>("raw-text-length");
            plainTextLength = root.Q<IntegerField>("plain-text-length");
            beginIndexField = root.Q<IntegerField>("begin-index");
            endIndexField = root.Q<IntegerField>("end-index");
            playBtn = root.Q<Button>("play");
            floatBtn = root.Q("float-button");
            flag = root.Q<RadioButton>("empty-flag");
            tagBody = root.Q("tag-body");
            tagSelector = root.Q<DropdownField>("tag-selector");
            tagTitle = root.Q<Label>("tag-title");
            tagField = root.Q<TextField>("tag-field");
            tagApply = root.Q<Button>("tag-apply");
            var floatBtnView = floatBtn.Q<Label>("button-view");
            var left = root.Q("left");

            rawTextField.RegisterValueChangedCallback(e => RawText = e.newValue);
            plainTextField.RegisterCallback<KeyDownEvent>(e =>
            {
                if(e.character != char.MinValue) //�������ַ�ʱ�����������λ�ò����ַ�
                {
                    if(BeginIndex == EndIndex) //�����һ��ʱΪ�����ַ�
                    {
                        _tree.InsertPlainText(BeginIndex, e.character.ToString());
                    }
                    else //����Ϊ����ַ�
                    {
                        _tree.ReplacePlainText(BeginIndex, EndIndex, e.character.ToString());
                    }
                }
                else if(e.keyCode == KeyCode.Backspace) //��ɾ���ַ�ʱ��ɾ��������ڵ��ַ�
                {
                    if(BeginIndex == EndIndex)//�����λ����ͬʱɾ��ǰһ���ַ�
                    {
                        if(BeginIndex != 0)
                        {
                            _tree.RemovePlainText(BeginIndex - 1, EndIndex);
                        }
                    }
                    else //����ɾ����Χ���ַ�
                    {
                        _tree.RemovePlainText(BeginIndex, EndIndex);
                    }
                }
                else if(e.ctrlKey && e.keyCode == KeyCode.V) //����ı�
                {
                    Paste();
                }
                else
                {
                    return;
                }
                OnTreeUpdata();
            });
            plainTextField.RegisterCallback<ExecuteCommandEvent>(e =>
            {
                if(e.commandName == "Paste")
                {
                    Paste();
                }
                else if(e.commandName == "Cut")
                {
                    _tree.RemovePlainText(BeginIndex, EndIndex);
                    rawTextField.SetValueWithoutNotify(_tree.ToString());
                    previewTextField.text = _tree.ToString();
                }
                else
                {
                    return;
                }
                OnTreeUpdata();
            });
            plainTextField.RegisterCallback<FocusInEvent>(e=>plainTextFocus=true);
            plainTextField.RegisterCallback<FocusOutEvent>(e=>plainTextFocus=false);
            endIndexField.RegisterValueChangedCallback(e =>
            {
                var value = e.newValue;
                if(value > plainTextField.text.Length)
                {
                    value = plainTextField.text.Length;
                }
                else if(value < beginIndexField.value)
                {
                    value = beginIndexField.value;
                }
                plainTextField.SelectRange(beginIndexField.value, value);
                endIndexField.SetValueWithoutNotify(value);
            });
            beginIndexField.RegisterValueChangedCallback(e =>
            {
                var value = e.newValue;
                if (value > endIndexField.value)
                {
                    value = endIndexField.value;
                }
                else if (value < 0)
                {
                    value = 0;
                }
                plainTextField.SelectRange(value, endIndexField.value);
                beginIndexField.SetValueWithoutNotify(value);
            });
            playBtn.clicked += () =>
            {
                Debug.Log("to be implement");
            };
            floatBtn.RegisterCallback<ClickEvent>(e =>
            {
                if (e.button == 0)
                {
                    _showRawText = !_showRawText;
                    if (_showRawText)
                    {
                        left.style.width = 250;
                        floatBtnView.text = "��";
                    }
                    else
                    {
                        left.style.width = 0;
                        floatBtnView.text = "��";
                    }
                }
            });
            flag.RegisterValueChangedCallback(e=>OnFlagValueChange(e.newValue));
            tagSelector.RegisterValueChangedCallback(e =>
            {
                tagBody.Clear();
                tagField.value = "";
                if (isCustomTag)
                {
                    tagTitle.text = "Custom";
                }
                else
                {
                    Type type;
                    if (flag.value)
                    {
                        type = _emptyTagList.Find((tup) => tup.menuPath == e.newValue).type;
                    }
                    else
                    {
                        type = _pairTagList.Find((tup) => tup.menuPath == e.newValue).type;
                    }
                    var view = Activator.CreateInstance(type) as TagView;
                    tagBody.Add(view);
                    tagTitle.text = view.Tag.Substring(0, 1).ToUpper() + view.Tag.Substring(1);
                    tagField.value = view.ConvertToText();
                    view.ValueChanged += ()=>tagField.SetValueWithoutNotify(view.ConvertToText());
                }
            });
            tagField.RegisterValueChangedCallback(e =>
            {
                if(tagView?.SetValueFromText(e.newValue) ?? true)
                {
                    tagField.EnableInClassList("errorField", false);
                }
                else
                {
                    tagField.EnableInClassList("errorField", true);
                }
            });
            tagApply.clicked += () =>
            {
                if (_tagValid && !string.IsNullOrEmpty(tagField.text))
                {
                    var tag = tagView?.Tag ?? RichTextUtility.GetTagType(tagField.text);
                    if (flag.value)
                    {
                        _tree.InsertEmptyTag(BeginIndex, tag, tagField.text);
                    }
                    else
                    {
                        _tree.SetPairTag(BeginIndex, EndIndex, tag, tagField.text);
                    }
                    OnTreeUpdata();
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "The tag attribute is empty or the format is invalid", "Sure");
                }
            };

            rawTextLength.SetEnabled(false);
            plainTextLength.SetEnabled(false);
            plainTextField.selectAllOnFocus = false;
            plainTextField.selectAllOnMouseUp = false;
            tagField.selectAllOnFocus = false;
            tagField.selectAllOnMouseUp = false;
            flag.SetEnabled(false);
            flag.value = true;

            RawText = "";
        }

        private void Update()
        {
            if (plainTextFocus)
            {
                OnTextIndexChanged();
            }
            else
            {
                plainTextField.SelectRange(BeginIndex, EndIndex);
            }
            flag.value = BeginIndex == EndIndex;
        }

        /// <summary>
        /// ���ձ�ǩ״̬�л�
        /// </summary>
        /// <param name="isEmpty"></param>
        void OnFlagValueChange(bool isEmpty)
        {
            flag.tooltip = isEmpty ? "EmptyTag" : "PairTag";
            tagSelector.choices = isEmpty ? _emptyTagList.Select(tup => tup.menuPath).ToList() : _pairTagList.Select(tup => tup.menuPath).ToList();
            tagSelector.choices.Add("Custom");
            tagSelector.value = "Custom";
        }

        void InitTagList()
        {
            var list = TypeCache.GetTypesWithAttribute<TagAttribute>()
                .Select(type=>((type.GetCustomAttributes(typeof(TagAttribute), false)[0] as TagAttribute), type));
            _pairTagList = list.Where(tup=> !tup.Item1.IsEmptyTag)
                .Select(tup=>(tup.Item1.MenuPath, tup.type))
                .ToList();
            _emptyTagList = list.Where(tup => tup.Item1.IsEmptyTag)
                .Select(tup => (tup.Item1.MenuPath, tup.type))
                .ToList();
        }

        void OnTreeUpdata()
        {
            var rawText = RawText;
            var plainText = _tree.GetPlainText();

            rawTextField.SetValueWithoutNotify(rawText);
            previewTextField.text = RichTextHelper.GetOutPutText(_tree);

            rawTextLength.SetValueWithoutNotify(rawText.Length);
            plainTextLength.SetValueWithoutNotify(plainText.Length);

            OnTextIndexChanged();
        }

        void Paste()
        {
            if (BeginIndex == EndIndex) //�����һ��ʱΪ�����ַ�
            {
                _tree.InsertPlainText(BeginIndex, GUIUtility.systemCopyBuffer);
            }
            else //����Ϊ����ַ�
            {
                _tree.ReplacePlainText(BeginIndex, EndIndex, GUIUtility.systemCopyBuffer);
            }
            rawTextField.SetValueWithoutNotify(_tree.ToString());
            previewTextField.text = _tree.ToString();
        }

        void OnTextIndexChanged()
        {
            endIndexField.SetValueWithoutNotify(Mathf.Max(plainTextField.selectIndex, plainTextField.cursorIndex));
            beginIndexField.SetValueWithoutNotify(Mathf.Min(plainTextField.selectIndex, plainTextField.cursorIndex));
        }
    }
}